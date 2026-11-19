// splash.h - IFME's splash screen (SplashScreen14.png), shown while the
// plugins load and, as in C#, the plugins that ask for it are tested.
#pragma once

#include <QPixmap>
#include <QStringList>
#include <QWidget>

class QLabel;
namespace app { class Core; }

class SplashScreen : public QWidget
{
	Q_OBJECT

public:
	SplashScreen(app::Core &core, bool testEncoders, QWidget *parent = nullptr);

	// Fade in, load (and test) in the background, fade out, emit finished().
	void start();

signals:
	void finished();

protected:
	void paintEvent(QPaintEvent *) override;

private:
	app::Core &m_core;
	bool m_test;
	bool m_blur = false;    // acrylic blur behind the window (Windows)
	QPixmap m_image;
	QLabel *m_version, *m_loading, *m_status, *m_log, *m_contrib;
	QStringList m_logLines;
	QStringList m_failed;   // GUIDs that failed the test (disabled afterwards)

	void appendLog(const QString &line);
	void done();
};
