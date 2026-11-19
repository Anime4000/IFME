#include <QApplication>

#include "mainwindow.h"

int main(int argc, char *argv[])
{
	QApplication app(argc, argv);
	QApplication::setApplicationName(QStringLiteral("IFME"));
	QApplication::setApplicationVersion(QString::fromUtf8(ifme_version()));

	MainWindow window;
	window.show();

	return app.exec();
}
