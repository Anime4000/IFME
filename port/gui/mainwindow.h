#pragma once

#include <QMainWindow>
#include <memory>

#include "ifme.h"

class QPlainTextEdit;

class MainWindow : public QMainWindow
{
	Q_OBJECT

public:
	explicit MainWindow(const QString &rootDir = QString(), QWidget *parent = nullptr);

	void probeFile(const QString &path);

protected:
	void dragEnterEvent(QDragEnterEvent *event) override;
	void dropEvent(QDropEvent *event) override;

private:
	struct CtxDeleter { void operator()(ifme_ctx *c) const { ifme_destroy(c); } };
	std::unique_ptr<ifme_ctx, CtxDeleter> m_ctx;

	QPlainTextEdit *m_log = nullptr;

	void showSystemInfo();
	void showPlugins();
};
