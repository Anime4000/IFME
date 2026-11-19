#include "i18n.h"

#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <QFileInfo>
#include <QHash>
#include <QJsonDocument>
#include <QJsonObject>
#include <QLibraryInfo>
#include <QLocale>
#include <QTranslator>

#include "app.h"

namespace i18n {

namespace {

// tr() strings are looked up by their English text, whatever the class.
class JsonTranslator : public QTranslator
{
public:
	QHash<QString, QString> strings;

	QString translate(const char *, const char *source, const char *, int) const override
	{
		return strings.value(QString::fromUtf8(source));
	}
	bool isEmpty() const override { return strings.isEmpty(); }
};

QJsonObject readFile(const QString &path)
{
	QFile f(path);
	if (!f.open(QIODevice::ReadOnly))
		return {};
	QByteArray data = f.readAll();
	if (data.startsWith("\xEF\xBB\xBF"))
		data.remove(0, 3);
	return QJsonDocument::fromJson(data).object();
}

QString nativeName(const QString &code)
{
	const QLocale l(QString(code).replace(QLatin1Char('-'), QLatin1Char('_')));
	const QString lang = l.nativeLanguageName(), territory = l.nativeTerritoryName();
	if (lang.isEmpty())
		return code;
	return territory.isEmpty() ? lang : QStringLiteral("%1 (%2)").arg(lang, territory);
}

}  // namespace

QString folder()
{
	return QDir(QCoreApplication::applicationDirPath()).filePath(QStringLiteral("i18n_qt"));
}

QVector<Language> available()
{
	QVector<Language> out;
	out.push_back({ QStringLiteral("en-US"), QStringLiteral("English"), QStringLiteral("Anime4000"), {}, {}, 0, 0 });
	const QDir dir(folder());
	for (const QString &file : dir.entryList({ QStringLiteral("*.json") }, QDir::Files, QDir::Name)) {
		const QJsonObject o = readFile(dir.filePath(file));
		const QJsonObject strings = o.value(QStringLiteral("Strings")).toObject();
		if (strings.isEmpty())
			continue;
		Language l;
		l.code = QFileInfo(file).completeBaseName();
		if (l.code.compare(QLatin1String("en-US"), Qt::CaseInsensitive) == 0)
			continue;
		l.name = nativeName(l.code);
		l.author = o.value(QStringLiteral("AuthorName")).toString();
		l.profile = o.value(QStringLiteral("AuthorProfile")).toString();
		l.email = o.value(QStringLiteral("AuthorEmail")).toString();
		l.total = strings.size();
		for (auto i = strings.constBegin(); i != strings.constEnd(); ++i)
			l.translated += !i.value().toString().isEmpty();
		for (const QJsonValue &d : o.value(QStringLiteral("Drafts")).toArray())
			l.drafts += !strings.value(d.toString()).toString().isEmpty();
		out.push_back(l);
	}
	return out;
}

QString chosen()
{
	const QVector<Language> langs = available();
	auto has = [&](const QString &code) {
		for (const Language &l : langs)
			if (l.code.compare(code, Qt::CaseInsensitive) == 0)
				return l.code;
		return QString();
	};
	const QString setting = has(app::settings().value(QStringLiteral("ui/language")).toString());
	if (!setting.isEmpty())
		return setting;
	// The system's language: exact (ja-JP), else the same language (ja-*).
	const QString sys = QLocale::system().name().replace(QLatin1Char('_'), QLatin1Char('-'));
	if (!has(sys).isEmpty())
		return has(sys);
	const QString prefix = sys.section(QLatin1Char('-'), 0, 0) + QLatin1Char('-');
	for (const Language &l : langs)
		if (l.code.startsWith(prefix, Qt::CaseInsensitive))
			return l.code;
	return QStringLiteral("en-US");
}

void install(QCoreApplication &app, const QString &code)
{
	if (code.isEmpty() || code.compare(QLatin1String("en-US"), Qt::CaseInsensitive) == 0)
		return;

	auto *mine = new JsonTranslator;
	mine->setParent(&app);
	const QJsonObject strings = readFile(QDir(folder()).filePath(code + QStringLiteral(".json")))
		.value(QStringLiteral("Strings")).toObject();
	for (auto i = strings.constBegin(); i != strings.constEnd(); ++i)
		if (!i.value().toString().isEmpty())
			mine->strings.insert(i.key(), i.value().toString());
	if (mine->isEmpty() || !app.installTranslator(mine))
		delete mine;

	// Qt's own strings: shipped next to the program, else Qt's installation.
	const QLocale locale(QString(code).replace(QLatin1Char('-'), QLatin1Char('_')));
	for (const QString &dir : { folder(), QLibraryInfo::path(QLibraryInfo::TranslationsPath) }) {
		auto *qt = new QTranslator(&app);
		if (qt->load(locale, QStringLiteral("qtbase"), QStringLiteral("_"), dir) && app.installTranslator(qt))
			break;
		delete qt;
	}
}

}  // namespace i18n
