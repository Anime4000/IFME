#include "mainwindow.h"

#include <QDragEnterEvent>
#include <QDropEvent>
#include <QFontDatabase>
#include <QMimeData>
#include <QPlainTextEdit>
#include <QStatusBar>
#include <QUrl>

MainWindow::MainWindow(const QString &rootDir, QWidget *parent)
	: QMainWindow(parent)
	, m_ctx(ifme_create())
{
	setWindowTitle(QStringLiteral("IFME %1").arg(QString::fromUtf8(ifme_version())));
	resize(900, 560);

	m_log = new QPlainTextEdit(this);
	m_log->setReadOnly(true);
	m_log->setFont(QFontDatabase::systemFont(QFontDatabase::FixedFont));
	m_log->setAcceptDrops(false);   // let the window handle drops
	setCentralWidget(m_log);
	setAcceptDrops(true);

	if (m_ctx && !rootDir.isEmpty() && !ifme_set_root_dir(m_ctx.get(), rootDir.toUtf8().constData()))
		m_log->appendPlainText(tr("Folder not found: %1").arg(rootDir));

	showSystemInfo();
	showPlugins();
}

void MainWindow::showSystemInfo()
{
	if (!m_ctx) {
		m_log->appendPlainText(tr("Failed to initialise IFME core."));
		return;
	}

	const ifme_cpu_info *cpu = ifme_cpu(m_ctx.get());
	auto yn = [](bool v) { return v ? QStringLiteral("yes") : QStringLiteral("no"); };

	m_log->appendPlainText(QStringLiteral("IFME core   %1").arg(QString::fromUtf8(ifme_version())));
	m_log->appendPlainText(QStringLiteral("App folder  %1").arg(QString::fromUtf8(ifme_app_dir(m_ctx.get()))));
	m_log->appendPlainText(QStringLiteral("Root        %1").arg(QString::fromUtf8(ifme_root_dir(m_ctx.get()))));
	m_log->appendPlainText(QStringLiteral("Config      %1").arg(QString::fromUtf8(ifme_config_dir(m_ctx.get()))));
	m_log->appendPlainText(QStringLiteral("CPU         %1 (%2)")
		.arg(QString::fromUtf8(cpu->brand), QString::fromUtf8(cpu->arch)));

	if (cpu->is_x86) {
		m_log->appendPlainText(QStringLiteral("SIMD        SSE4.2:%1 AVX:%2 AVX2:%3 FMA3:%4 AVX-512F:%5")
			.arg(yn(cpu->sse42), yn(cpu->avx), yn(cpu->avx2), yn(cpu->fma3), yn(cpu->avx512f)));
	}

	if (cpu->hypervisor)
		m_log->appendPlainText(QStringLiteral("Hypervisor  %1").arg(QString::fromUtf8(cpu->hypervisor_vendor)));
}

void MainWindow::showPlugins()
{
	if (!m_ctx)
		return;

	m_log->appendPlainText(QString());

	int usable = ifme_plugins_load(m_ctx.get());
	if (usable < 0) {
		m_log->appendPlainText(tr("No Plugins folder in %1 (start with --root DIR)")
			.arg(QString::fromUtf8(ifme_root_dir(m_ctx.get()))));
		statusBar()->showMessage(tr("No plugins"));
		return;
	}

	const size_t count = ifme_plugin_count(m_ctx.get());
	for (size_t i = 0; i < count; i++) {
		const ifme_plugin_info *p = ifme_plugin_at(m_ctx.get(), i);
		m_log->appendPlainText(QStringLiteral("%1 %2 %3 %4")
			.arg(QString::fromUtf8(p->kind == IFME_PLUGIN_AUDIO ? "audio" : "video"))
			.arg(QString::fromUtf8(ifme_plugin_status_name(p->status)), -11)
			.arg(QString::fromUtf8(p->name), QString::fromUtf8(p->version)));
	}

	statusBar()->showMessage(tr("%1 of %2 plugins usable - drop media files here to inspect them")
		.arg(usable).arg(count));
}

void MainWindow::dragEnterEvent(QDragEnterEvent *event)
{
	if (event->mimeData()->hasUrls())
		event->acceptProposedAction();
}

void MainWindow::dropEvent(QDropEvent *event)
{
	for (const QUrl &url : event->mimeData()->urls()) {
		if (url.isLocalFile())
			probeFile(url.toLocalFile());
	}
}

void MainWindow::probeFile(const QString &path)
{
	if (!m_ctx)
		return;

	m_log->appendPlainText(QString());

	char err[512];
	ifme_media *m = ifme_probe(m_ctx.get(), path.toUtf8().constData(), nullptr, err, sizeof(err));
	if (!m) {
		m_log->appendPlainText(tr("%1: %2").arg(path, QString::fromUtf8(err)));
		return;
	}

	auto row = [this](const char *label, const QString &value) {
		m_log->appendPlainText(QStringLiteral("%1: %2").arg(QString::fromUtf8(label), -28).arg(value));
	};
	auto s = [](const char *v) { return QString::fromUtf8(v); };

	m_log->appendPlainText(QStringLiteral("General"));
	row("Complete name", s(m->path));
	row("Format", QStringLiteral("%1 (%2)").arg(s(m->format_name), s(m->format_long_name)));
	row("File size", QStringLiteral("%1 bytes").arg(m->file_size));
	row("Duration", QStringLiteral("%1 seconds").arg(m->duration, 0, 'f', 3));

	for (size_t i = 0; i < m->video_count; i++) {
		const ifme_stream_video &v = m->video[i];
		m_log->appendPlainText(QStringLiteral("Video #%1  %2 %3x%4 %5 fps %6-bit %7 [%8] %9")
			.arg(i).arg(s(v.codec)).arg(v.width).arg(v.height)
			.arg(v.frame_rate_avg, 0, 'f', 3).arg(v.bit_depth).arg(v.chroma)
			.arg(s(v.language), s(v.title)));
	}
	for (size_t i = 0; i < m->audio_count; i++) {
		const ifme_stream_audio &a = m->audio[i];
		m_log->appendPlainText(QStringLiteral("Audio #%1  %2 %3 Hz %4 ch %5-bit [%6] %7")
			.arg(i).arg(s(a.codec)).arg(a.sample_rate).arg(a.channels).arg(a.bit_depth)
			.arg(s(a.language), s(a.title)));
	}
	for (size_t i = 0; i < m->subtitle_count; i++) {
		const ifme_stream_subtitle &t = m->subtitle[i];
		m_log->appendPlainText(QStringLiteral("Subtitle #%1  %2 [%3] %4")
			.arg(i).arg(s(t.codec), s(t.language), s(t.title)));
	}
	for (size_t i = 0; i < m->attachment_count; i++) {
		const ifme_stream_attachment &t = m->attachment[i];
		m_log->appendPlainText(QStringLiteral("Attachment #%1  %2 (%3)")
			.arg(i).arg(s(t.filename), s(t.mime_type)));
	}

	ifme_media_free(m);
}
