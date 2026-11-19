// i18n.h - interface languages. Translations are JSON files in i18n_qt/
// next to the program (port/i18n/ in the sources; IFME 8's are in i18n/):
// the English text of each tr() string and its translation, kept up to
// date by tools/i18n_update.py. "Drafts" lists machine-drafted strings
// nobody has reviewed yet.
// Strings without a translation stay in English. Qt's own (standard buttons,
// file dialogs) come from qtbase_*.qm when those are installed.
#pragma once

#include <QString>
#include <QVector>

class QCoreApplication;

namespace i18n {

struct Language {
	QString code;          // "ja-JP"
	QString name;          // in the language itself: "日本語 (日本)"
	QString author, profile, email;
	int translated = 0, total = 0;
	int drafts = 0;        // of `translated`, not yet reviewed
};

QString folder();                     // <program>/i18n_qt
QVector<Language> available();        // English first, then every i18n_qt/*.json
// The language to use: the setting, else the system's if there is a file
// for it, else English (as C#'s i18n.LoadLangFiles).
QString chosen();
// Load `code`'s translation (and Qt's) into `app`. English needs none.
void install(QCoreApplication &app, const QString &code);

}  // namespace i18n
