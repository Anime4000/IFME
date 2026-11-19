// aboutdialog.h - About IFME: banner, version, the four characters, credits.
#pragma once

#include <QDialog>

class AboutDialog : public QDialog
{
	Q_OBJECT

public:
	explicit AboutDialog(QWidget *parent = nullptr);
};
