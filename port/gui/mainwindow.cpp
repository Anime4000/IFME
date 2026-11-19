#include "mainwindow.h"

#include <QFontDatabase>
#include <QPlainTextEdit>
#include <QStatusBar>

MainWindow::MainWindow(const QString &rootDir, QWidget *parent)
	: QMainWindow(parent)
	, m_ctx(ifme_create())
{
	setWindowTitle(QStringLiteral("IFME %1").arg(QString::fromUtf8(ifme_version())));
	resize(900, 560);

	m_log = new QPlainTextEdit(this);
	m_log->setReadOnly(true);
	m_log->setFont(QFontDatabase::systemFont(QFontDatabase::FixedFont));
	setCentralWidget(m_log);

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

	statusBar()->showMessage(tr("%1 of %2 plugins usable").arg(usable).arg(count));
}
