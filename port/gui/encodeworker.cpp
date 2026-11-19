#include "encodeworker.h"

#include <QDir>

EncodeWorker::EncodeWorker(const QString &root, const QStringList &disabled, const QVector<EncodeJob> &jobs)
	: m_root(root)
	, m_disabled(disabled)
	, m_jobs(jobs)
	, m_task(ifme_task_create())
{
}

EncodeWorker::~EncodeWorker()
{
	ifme_task_destroy(m_task);
}

void EncodeWorker::pause() { ifme_task_pause(m_task); }
void EncodeWorker::resume() { ifme_task_resume(m_task); }
bool EncodeWorker::isPaused() const { return ifme_task_is_paused(m_task); }

void EncodeWorker::stop()
{
	m_stop = true;
	ifme_task_stop(m_task);   // ends the running step and every later job at once
}

static QString clock(long long sec)
{
	if (sec < 0)
		return QStringLiteral("--:--:--");
	return QStringLiteral("%1:%2:%3").arg(sec / 3600, 2, 10, QLatin1Char('0'))
		.arg(sec / 60 % 60, 2, 10, QLatin1Char('0')).arg(sec % 60, 2, 10, QLatin1Char('0'));
}

void EncodeWorker::onOutput(void *user, const char *line, bool is_status)
{
	auto *self = static_cast<EncodeWorker *>(user);
	const QString text = QString::fromUtf8(line);
	if (!is_status) {
		emit self->output(self->m_current, text);
	} else if (!self->m_haveProgress && self->m_lastProgress.elapsed() > 250) {
		// Progress-like text the parser does not know (MP4Box, ...).
		self->m_lastProgress.restart();
		emit self->progress(self->m_current, -1, text);
	}
}

void EncodeWorker::onProgress(void *user, const ifme_progress *p)
{
	auto *self = static_cast<EncodeWorker *>(user);
	self->m_haveProgress = true;
	if (self->m_lastProgress.elapsed() < 100)   // at most ten updates a second
		return;
	self->m_lastProgress.restart();

	QString text;
	if (p->frame >= 0) {
		text = p->total_frames > 0 ? tr("frame %1/%2").arg(p->frame).arg(p->total_frames)
		                           : tr("frame %1").arg(p->frame);
		if (p->fps >= 0)
			text += QStringLiteral("  %1 fps").arg(p->fps, 0, 'f', 2);
	} else if (p->time_sec >= 0) {
		text = tr("time %1").arg(clock(static_cast<long long>(p->time_sec)));
		if (p->total_sec > 0)
			text += QStringLiteral("/%1").arg(clock(static_cast<long long>(p->total_sec + 0.5)));
	}
	if (p->kbps >= 0)
		text += QStringLiteral("  %1 kb/s").arg(p->kbps, 0, 'f', 0);
	text += tr("  ETA %1").arg(clock(p->eta_sec));
	emit self->progress(self->m_current, p->percent, text);
}

void EncodeWorker::onStage(void *user, const char *stage)
{
	auto *self = static_cast<EncodeWorker *>(user);
	self->m_haveProgress = false;
	emit self->stage(self->m_current, QString::fromUtf8(stage));
}

void EncodeWorker::run()
{
	// A context of its own: the UI keeps using its context meanwhile.
	ifme_ctx *ctx = ifme_create();
	ifme_set_root_dir(ctx, QDir::toNativeSeparators(m_root).toUtf8().constData());
	ifme_plugins_load(ctx);
	QVector<QByteArray> utf8;
	QVector<const char *> ptrs;
	for (const QString &g : m_disabled)
		utf8.push_back(g.toUtf8());
	for (const QByteArray &b : utf8)
		ptrs.push_back(b.constData());
	ifme_plugins_set_disabled(ctx, ptrs.data(), static_cast<size_t>(ptrs.size()));

	ifme_task_callbacks cb{};
	cb.on_output = onOutput;
	cb.on_progress = onProgress;
	cb.on_stage = onStage;
	cb.user = this;

	for (const EncodeJob &job : m_jobs) {
		if (m_stop)
			break;
		m_current = job.id;
		m_haveProgress = false;
		m_lastProgress.start();
		emit itemStarted(job.id);

		char err[2048] = "";
		ifme_task_result r = ifme_encode(m_task, ctx, job.json.constData(), &cb, err, sizeof(err));
		emit itemFinished(job.id, static_cast<int>(r), QString::fromUtf8(err));
	}

	ifme_destroy(ctx);
	emit finished();
}
