#include <QApplication>
#include <QCommandLineParser>

#include "mainwindow.h"

int main(int argc, char *argv[])
{
	QApplication app(argc, argv);
	QApplication::setApplicationName(QStringLiteral("IFME"));
	QApplication::setApplicationVersion(QString::fromUtf8(ifme_version()));

	QCommandLineParser parser;
	parser.addHelpOption();
	parser.addVersionOption();
	QCommandLineOption rootOption({ QStringLiteral("r"), QStringLiteral("root") },
		QStringLiteral("IFME folder holding Plugins/ and Samples/."), QStringLiteral("dir"));
	parser.addOption(rootOption);
	parser.process(app);

	MainWindow window(parser.value(rootOption));
	window.show();

	return app.exec();
}
