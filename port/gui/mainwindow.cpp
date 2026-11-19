#include "mainwindow.h"

#include <QFontDatabase>
#include <QPlainTextEdit>
#include <QStatusBar>

MainWindow::MainWindow(QWidget *parent)
	: QMainWindow(parent)
	, m_ctx(ifme_create())
{
	setWindowTitle(QStringLiteral("IFME %1").arg(QString::fromUtf8(ifme_version())));
	resize(900, 560);

	m_log = new QPlainTextEdit(this);
	m_log->setReadOnly(true);
	m_log->setFont(QFontDatabase::systemFont(QFontDatabase::FixedFont));
	setCentralWidget(m_log);

	statusBar()->showMessage(tr("Ready"));

	showSystemInfo();
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
