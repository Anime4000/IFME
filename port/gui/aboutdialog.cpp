#include "aboutdialog.h"

#include <QDate>
#include <QLabel>
#include <QPainter>
#include <QPixmap>

#include "app.h"
#include "wad.h"

// Same layout as C#'s frmAbout (600 x 700).
AboutDialog::AboutDialog(QWidget *parent)
	: QDialog(parent)
{
	setWindowTitle(tr("About IFME"));
	setFixedSize(600, 700);
	// frmAbout: black window, white text, a (14,14,14) strip behind the links.
	setStyleSheet(QStringLiteral("AboutDialog { background: black; } QLabel { color: white; background: transparent; }"
	                             " QLabel#footer { background: rgb(14,14,14); }"));
	auto *footer = new QLabel(this);
	footer->setObjectName(QStringLiteral("footer"));
	footer->setGeometry(0, 610, 600, 90);

	auto label = [this](const QRect &r, const QString &text, double pt, bool bold) {
		auto *l = new QLabel(text, this);
		l->setGeometry(r);
		QFont f(QStringLiteral("Tahoma"));
		f.setPointSizeF(pt);
		f.setBold(bold);
		l->setFont(f);
		l->setAlignment(Qt::AlignCenter);
		l->setWordWrap(true);
		return l;
	};
	auto link = [&](const QRect &r, const QString &text, const char *url, double pt, bool bold) {
		QLabel *l = label(r, QStringLiteral("<a style=\"color: white;\" href=\"%1\">%2</a>")
		                     .arg(QLatin1String(url), text.toHtmlEscaped()), pt, bold);
		l->setTextFormat(Qt::RichText);
		l->setOpenExternalLinks(true);
		return l;
	};

	auto *banner = new QLabel(this);
	banner->setGeometry(0, 0, 600, 64);
	banner->setScaledContents(true);
	banner->setPixmap(wad::image(QStringLiteral("Banner_About.png")));   // resources.wad, as IFME 8

	label({ 0, 64, 600, 48 }, QStringLiteral("'%1'").arg(QLatin1String(app::CodeName)), 14, false);
	label({ 0, 112, 600, 50 }, tr("Version %1 (Qt)").arg(QString::fromUtf8(ifme_version())), 14, true);
	label({ 0, 162, 600, 48 },
	      tr("© 2011-%1 Anime4000 & Contributor, Some Right Reserved.").arg(QDate::currentDate().year()),
	      10, false);

	link({ 0, 210, 600, 56 }, tr("Character illustration by Ray-en aka 53C for IFME Project."),
	     "http://pixiv.me/ray53c", 10, true);
	struct Char { const char *file, *name; int x; };
	const Char chars[] = {
		{ "Avatar1_Ichika.png", "Ichika", 91 }, { "Avatar2_Fumiko.png", "Fumiko", 197 },
		{ "Avatar3_Miho.png", "Miho", 303 }, { "Avatar4_Erika.png", "Erika", 409 },
	};
	for (const Char &c : chars) {
		auto *pic = new QLabel(this);
		pic->setGeometry(c.x, 210 + 59, 100, 150);
		const QPixmap art = wad::image(QString::fromLatin1(c.file));
		if (!art.isNull())
			pic->setPixmap(art.scaled(100, 150, Qt::KeepAspectRatio, Qt::SmoothTransformation));
		pic->setAlignment(Qt::AlignCenter);
		label({ c.x, 210 + 59 + 150, 100, 24 }, QString::fromLatin1(c.name), 9, true);
	}

	label({ 75, 503, 450, 107 }, QStringLiteral("%1\n\n%2")
		.arg(QLatin1String(app::Contributors), QLatin1String(app::TradeMarks)), 9, false);

	link({ 0, 610, 297, 90 }, tr("GitHub Project"), "https://github.com/Anime4000/IFME", 9, false);
	link({ 303, 610, 297, 90 }, tr("Discord Server"), "https://discord.gg/4f6MDpfug2", 9, false);
}
