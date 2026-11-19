#include "splash.h"

#include <QElapsedTimer>
#include <QGuiApplication>
#include <QLabel>
#include <QPainter>
#include <QPropertyAnimation>
#include <QScreen>
#include <QThread>
#include <QTimer>

#include "app.h"

// Layout of C#'s frmSplashScreen (1066 x 600, labels in white).
static constexpr int BaseW = 1066, BaseH = 600;

SplashScreen::SplashScreen(app::Core &core, bool testEncoders, QWidget *parent)
	: QWidget(parent, Qt::SplashScreen | Qt::FramelessWindowHint | Qt::WindowStaysOnTopHint)
	, m_core(core)
	, m_test(testEncoders)
	, m_image(QStringLiteral(":/ifme/splash.png"))
{
	setAttribute(Qt::WA_DeleteOnClose);
	setWindowTitle(QStringLiteral("IFME"));

	// Smaller screens get a smaller splash, same proportions.
	double k = 1.0;
	if (QScreen *s = QGuiApplication::primaryScreen()) {
		const QSize avail = s->availableGeometry().size();
		k = qMin(1.0, qMin(avail.width() * 0.9 / BaseW, avail.height() * 0.9 / BaseH));
	}
	setFixedSize(qRound(BaseW * k), qRound(BaseH * k));

	auto label = [&](int x, int y, int w, int h, const char *family, double pt, bool bold,
	                 Qt::Alignment align) {
		auto *l = new QLabel(this);
		l->setGeometry(qRound(x * k), qRound(y * k), qRound(w * k), qRound(h * k));
		QFont f(QString::fromLatin1(family));
		f.setPointSizeF(pt * k);
		f.setBold(bold);
		l->setFont(f);
		l->setAlignment(align);
		l->setWordWrap(true);
		l->setStyleSheet(QStringLiteral("color: white; background: transparent;"));
		return l;
	};
	m_version = label(16, 171, 600, 46, "Tahoma", 10, true, Qt::AlignLeft | Qt::AlignVCenter);
	m_loading = label(16, 217, 400, 24, "Tahoma", 12, true, Qt::AlignLeft | Qt::AlignBottom);
	m_status = label(16, 241, 400, 32, "Tahoma", 10, false, Qt::AlignLeft | Qt::AlignTop);
	m_log = label(17, 280, 439, 160, "Tahoma", 7, false, Qt::AlignLeft | Qt::AlignBottom);
	m_contrib = label(16, 450, 440, 133, "Segoe UI", 9, false, Qt::AlignLeft | Qt::AlignBottom);

	m_version->setText(tr("Release Version %1 (%2)")
		.arg(QString::fromUtf8(ifme_version()), QString::fromLatin1(app::CodeName)));
	m_loading->setText(tr("Initializing..."));
	m_status->setText(tr("Loading..."));
	m_contrib->setText(QStringLiteral("%1\n\n%2")
		.arg(QString::fromLatin1(app::Contributors), QString::fromLatin1(app::TradeMarks)));
}

void SplashScreen::paintEvent(QPaintEvent *)
{
	QPainter p(this);
	p.fillRect(rect(), Qt::black);
	p.setRenderHint(QPainter::SmoothPixmapTransform);
	p.drawPixmap(rect(), m_image);
}

void SplashScreen::appendLog(const QString &line)
{
	m_logLines << line;
	while (m_logLines.size() > 12)
		m_logLines.removeFirst();
	m_log->setText(m_logLines.join(QLatin1Char('\n')));
}

void SplashScreen::start()
{
	setWindowOpacity(0.0);
	show();
	auto *fade = new QPropertyAnimation(this, "windowOpacity", this);
	fade->setDuration(400);
	fade->setEndValue(1.0);
	fade->start(QAbstractAnimation::DeleteWhenStopped);

	auto *timer = new QElapsedTimer;
	timer->start();

	m_loading->setText(tr("Loading plugins..."));
	const int usable = m_core.loadPlugins();
	if (usable < 0) {
		m_status->setText(tr("No Plugins folder in %1").arg(m_core.root()));
	} else {
		for (const app::Plugin &p : m_core.plugins())
			if (p.status != QLatin1String("ok") && p.status != QLatin1String("builtin"))
				appendLog(QStringLiteral("%1 (%2, skipping...)").arg(p.name, p.status));
		m_status->setText(tr("%1 of %2 plugins usable").arg(usable).arg(m_core.plugins().size()));
	}

	// The test encodes the bundled sample with each plugin that asks for it
	// (GPU encoders); a plugin that fails is disabled from then on, as C# did.
	const bool test = m_test && usable > 0;
	if (test)
		m_loading->setText(tr("Testing encoders..."));

	ifme_ctx *ctx = m_core.ctx();
	QThread *worker = QThread::create([this, ctx, test] {
		if (!test)
			return;
		ifme_selftest_callbacks cb{};
		cb.user = this;
		cb.on_start = [](void *user, const ifme_plugin_info *p) {
			auto *self = static_cast<SplashScreen *>(user);
			const QString name = QString::fromUtf8(p->name);
			QMetaObject::invokeMethod(self, [self, name] {
				self->m_status->setText(tr("Testing %1...").arg(name));
			}, Qt::QueuedConnection);
		};
		cb.on_result = [](void *user, const ifme_plugin_info *p, ifme_test_outcome outcome,
		                  const char *, unsigned ms) {
			auto *self = static_cast<SplashScreen *>(user);
			const QString name = QString::fromUtf8(p->name), guid = QString::fromUtf8(p->guid);
			QMetaObject::invokeMethod(self, [self, name, guid, outcome, ms] {
				if (outcome == IFME_TEST_FAIL) {
					self->m_failed << guid;
					self->appendLog(tr("%1 is not compatible with this system").arg(name));
				} else {
					self->appendLog(QStringLiteral("%1: %2 (%3 s)")
						.arg(name, outcome == IFME_TEST_PASS ? tr("ok") : tr("skipped"))
						.arg(ms / 1000.0, 0, 'f', 1));
				}
			}, Qt::QueuedConnection);
		};
		ifme_plugins_selftest(ctx, false, &cb);
	});

	connect(worker, &QThread::finished, this, [this, worker, timer] {
		worker->deleteLater();
		// As C#: keep the splash up a moment so it can be read.
		const int wait = qMax(0, 1500 - static_cast<int>(timer->elapsed()));
		delete timer;
		QTimer::singleShot(wait, this, &SplashScreen::done);
	});
	worker->start();
}

void SplashScreen::done()
{
	if (!m_failed.isEmpty())
		m_core.setDisabled(m_core.disabled() + m_failed);
	else
		m_core.refresh();

	m_loading->setText(QString());
	m_status->setText(QString());

	auto *fade = new QPropertyAnimation(this, "windowOpacity", this);
	fade->setDuration(400);
	fade->setEndValue(0.0);
	connect(fade, &QPropertyAnimation::finished, this, [this] {
		emit finished();
		close();
	});
	fade->start(QAbstractAnimation::DeleteWhenStopped);
}
