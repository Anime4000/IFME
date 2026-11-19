#include <QApplication>
#include <QCommandLineParser>
#include <QDir>
#include <QIcon>
#include <QTimer>

#include "app.h"
#include "mainwindow.h"
#include "splash.h"

int main(int argc, char *argv[])
{
	QApplication app(argc, argv);
	QApplication::setApplicationName(QStringLiteral("IFME"));
	QApplication::setApplicationDisplayName(QStringLiteral("Internet Friendly Media Encoder"));
	QApplication::setApplicationVersion(QString::fromUtf8(ifme_version()));
	QApplication::setWindowIcon(QIcon(QStringLiteral(":/ifme/icon.ico")));

	QCommandLineParser parser;
	parser.setApplicationDescription(QStringLiteral("Internet Friendly Media Encoder"));
	parser.addHelpOption();
	parser.addVersionOption();
	QCommandLineOption root({ QStringLiteral("r"), QStringLiteral("root") },
		QStringLiteral("IFME folder holding Plugins/ and Samples/ (saved in Preferences)."), QStringLiteral("dir"));
	QCommandLineOption format({ QStringLiteral("f"), QStringLiteral("format") },
		QStringLiteral("Output format for the files given, e.g. mkv, mp4, m4a."), QStringLiteral("ext"));
	QCommandLineOption output({ QStringLiteral("o"), QStringLiteral("output-dir") },
		QStringLiteral("Folder to save the encoded files to."), QStringLiteral("dir"));
	QCommandLineOption start(QStringLiteral("start"), QStringLiteral("Start encoding the files at once."));
	QCommandLineOption quit(QStringLiteral("quit-when-done"),
		QStringLiteral("Quit after encoding (exit code 0 if every file worked)."));
	QCommandLineOption noSplash(QStringLiteral("no-splash"), QStringLiteral("Skip the splash screen and the encoder test."));
	QCommandLineOption shots(QStringLiteral("screenshots"), QStringLiteral("Save PNGs of the windows to dir, then quit."),
		QStringLiteral("dir"));
	shots.setFlags(QCommandLineOption::HiddenFromHelp);
	QCommandLineOption settingsFile(QStringLiteral("settings"), QStringLiteral("Use this settings file."),
		QStringLiteral("file"));
	settingsFile.setFlags(QCommandLineOption::HiddenFromHelp);
	for (const QCommandLineOption &o : { root, format, output, start, quit, noSplash, shots, settingsFile })
		parser.addOption(o);
	parser.addPositionalArgument(QStringLiteral("files"), QStringLiteral("Media files or folders to add."),
	                             QStringLiteral("[files...]"));
	parser.process(app);

	if (parser.isSet(settingsFile))
		app::setSettingsFile(parser.value(settingsFile));
	app::Core core;
	if (parser.isSet(root))
		app::settings().setValue(QStringLiteral("paths/root"), QDir(parser.value(root)).absolutePath());
	const QString rootDir = app::settings().value(QStringLiteral("paths/root")).toString();
	core.setRoot(rootDir.isEmpty() ? QCoreApplication::applicationDirPath() : rootDir);

	MainWindow *window = nullptr;
	auto showMain = [&] {
		window = new MainWindow(core);
		window->setAttribute(Qt::WA_DeleteOnClose);
		if (parser.isSet(output))
			window->setOutputDir(QDir(parser.value(output)).absolutePath());
		if (parser.isSet(format))
			window->setFormat(parser.value(format));
		window->addFiles(parser.positionalArguments());
		window->show();
		if (parser.isSet(shots)) {
			window->saveScreenshots(parser.value(shots));
			QTimer::singleShot(0, &app, &QApplication::quit);
			return;
		}
		window->setQuitWhenDone(parser.isSet(quit));
		if (parser.isSet(start))
			QTimer::singleShot(0, window, [window] { window->startEncoding(false); });
	};

	if (parser.isSet(noSplash)) {
		core.loadPlugins();
		showMain();
	} else {
		const bool test = app::settings().value(QStringLiteral("startup/test_encoders"), true).toBool();
		auto *splash = new SplashScreen(core, test);
		QObject::connect(splash, &SplashScreen::finished, &app, showMain);
		if (parser.isSet(shots)) {
			// Capture the splash once it shows its first status.
			QTimer::singleShot(600, splash, [splash, dir = parser.value(shots)] {
				QDir().mkpath(dir);
				splash->grab().save(QDir(dir).filePath(QStringLiteral("splash.png")));
			});
		}
		splash->start();
	}

	return app.exec();
}
