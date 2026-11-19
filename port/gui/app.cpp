#include "app.h"
#include "wad.h"

#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <algorithm>
#include <QJsonDocument>

namespace app {

static QString g_settingsFile;

void setSettingsFile(const QString &path)
{
	g_settingsFile = path;
}

QSettings &settings()
{
	static QSettings *s = [] {
		if (!g_settingsFile.isEmpty())
			return new QSettings(g_settingsFile, QSettings::IniFormat);
		ifme_ctx *c = ifme_create();
		QString dir = QString::fromUtf8(c ? ifme_config_dir(c) : "");
		ifme_destroy(c);
		QDir().mkpath(dir);
		return new QSettings(QDir(dir).filePath(QStringLiteral("ifme-qt.ini")), QSettings::IniFormat);
	}();
	return *s;
}

QJsonObject Plugin::section() const
{
	return jget(manifest, audio ? QStringLiteral("Audio") : QStringLiteral("Video")).toObject();
}

const QVector<Container> &containers()
{
	// C#'s FileContainer list, MKV and MP4 first.
	static const QVector<Container> list = {
		{ "mkv", true }, { "mp4", true }, { "webm", true }, { "avi", true }, { "ts", true },
		{ "m2ts", true }, { "mpg", true }, { "wmv", true }, { "3gp", true },
		{ "m4a", false }, { "mp3", false }, { "opus", false }, { "ogg", false },
		{ "flac", false }, { "mp2", false }, { "wma", false },
	};
	return list;
}

bool isVideoContainer(const QString &ext)
{
	for (const Container &c : containers())
		if (ext.compare(QLatin1String(c.ext), Qt::CaseInsensitive) == 0)
			return c.video;
	return true;
}

// ---- Core --------------------------------------------------------------------

Core::Core() : m_ctx(ifme_create()) {}

Core::~Core()
{
	ifme_destroy(m_ctx);
}

bool Core::setRoot(const QString &dir)
{
	return m_ctx && ifme_set_root_dir(m_ctx, QDir::toNativeSeparators(dir).toUtf8().constData());
}

QString Core::root() const
{
	return QString::fromUtf8(m_ctx ? ifme_root_dir(m_ctx) : "");
}

int Core::loadPlugins()
{
	if (!m_ctx)
		return -1;
	int n = ifme_plugins_load(m_ctx);
	setDisabled(settings().value(QStringLiteral("plugins/disabled")).toStringList());
	return n;
}

void Core::setDisabled(const QStringList &guids)
{
	m_disabled = guids;
	m_disabled.removeAll(QString());
	m_disabled.removeDuplicates();
	settings().setValue(QStringLiteral("plugins/disabled"), m_disabled);

	QVector<QByteArray> utf8;
	QVector<const char *> ptrs;
	for (const QString &g : m_disabled)
		utf8.push_back(g.toUtf8());
	for (const QByteArray &b : utf8)
		ptrs.push_back(b.constData());
	ifme_plugins_set_disabled(m_ctx, ptrs.data(), static_cast<size_t>(ptrs.size()));
	refresh();
}

void Core::refresh()
{
	m_plugins.clear();
	char *text = m_ctx ? ifme_plugins_json(m_ctx) : nullptr;
	if (!text)
		return;
	const QJsonArray arr = QJsonDocument::fromJson(QByteArray(text)).array();
	ifme_free(text);

	for (const QJsonValue &v : arr) {
		const QJsonObject o = v.toObject();
		Plugin p;
		p.guid = o.value(QStringLiteral("guid")).toString().toLower();
		p.name = o.value(QStringLiteral("name")).toString();
		p.version = o.value(QStringLiteral("version")).toString();
		p.status = o.value(QStringLiteral("status")).toString();
		p.message = o.value(QStringLiteral("message")).toString();
		p.audio = o.value(QStringLiteral("kind")).toString() == QLatin1String("audio");
		p.testRequired = o.value(QStringLiteral("test_required")).toBool();
		p.manifest = o.value(QStringLiteral("manifest_json")).toObject();
		for (const QString &f : jstrings(jget(p.manifest, QStringLiteral("Format"))))
			p.formats << f.toLower();
		m_plugins.push_back(p);
	}
}

const Plugin *Core::find(const QString &guid) const
{
	for (const Plugin &p : m_plugins)
		if (p.guid.compare(guid, Qt::CaseInsensitive) == 0 && p.usable())
			return &p;
	return nullptr;
}

QVector<const Plugin *> Core::compatible(bool audio, const QString &container) const
{
	QVector<const Plugin *> out;
	for (const Plugin &p : m_plugins)
		if (p.audio == audio && p.usable() && p.formats.contains(container.toLower()))
			out.push_back(&p);
	return out;
}

QString Core::defaultVideo(const QString &container, const QString &preferred) const
{
	if (!isVideoContainer(container))
		return QStringLiteral("none");
	if (preferred == QLatin1String("copy"))
		return preferred;
	const auto list = compatible(false, container);
	for (const Plugin *p : list)
		if (p->guid == preferred)
			return p->guid;
	for (const Plugin *p : list)   // x265, the C# default
		if (p->guid == QLatin1String("deadbeef-0265-0265-0265-026502650265"))
			return p->guid;
	return list.isEmpty() ? QStringLiteral("copy") : list.first()->guid;
}

QString Core::defaultAudio(const QString &container, const QString &preferred) const
{
	if (preferred == QLatin1String("copy"))
		return preferred;
	const auto list = compatible(true, container);
	for (const Plugin *p : list)
		if (p->guid == preferred)
			return p->guid;
	// Same choice as the core: Opus (MKV, WebM), AAC (MP4), else the first.
	for (const char *g : { "deadface-f00d-f00d-f00d-f00df00df00d", "deadbeef-0aac-0aac-0aac-0aac0aac0aac" })
		for (const Plugin *p : list)
			if (p->guid == QLatin1String(g))
				return p->guid;
	return list.isEmpty() ? QStringLiteral("copy") : list.first()->guid;
}

QJsonObject Core::jobNew(const QString &input, const QString &output, const QString &video,
                         const QString &audio, QString *error) const
{
	char err[1024] = "";
	QByteArray in = input.toUtf8(), out = output.toUtf8(), v = video.toUtf8(), a = audio.toUtf8();
	char *text = ifme_job_new(m_ctx, in.constData(), output.isEmpty() ? nullptr : out.constData(),
	                          video.isEmpty() ? nullptr : v.constData(),
	                          audio.isEmpty() ? nullptr : a.constData(), err, sizeof(err));
	if (!text) {
		if (error)
			*error = QString::fromUtf8(err);
		return {};
	}
	QJsonObject job = QJsonDocument::fromJson(QByteArray(text)).object();
	ifme_free(text);
	return job;
}

QJsonObject Core::probe(const QString &path, QString *error) const
{
	char err[1024] = "";
	ifme_media *m = ifme_probe(m_ctx, path.toUtf8().constData(), nullptr, err, sizeof(err));
	if (!m) {
		if (error)
			*error = QString::fromUtf8(err);
		return {};
	}
	char *text = ifme_media_json(m, false);
	ifme_media_free(m);
	QJsonObject o = text ? QJsonDocument::fromJson(QByteArray(text)).object() : QJsonObject();
	ifme_free(text);
	return o;
}

// ---- JSON helpers --------------------------------------------------------------

QJsonValue jget(const QJsonObject &o, const QString &key)
{
	auto it = o.constFind(key);
	if (it != o.constEnd())
		return *it;
	for (auto i = o.constBegin(); i != o.constEnd(); ++i)
		if (i.key().compare(key, Qt::CaseInsensitive) == 0)
			return *i;
	return {};
}

QString jstr(const QJsonValue &v)
{
	if (v.isString())
		return v.toString();
	if (v.isDouble())
		return QString::number(v.toDouble(), 'g', 12);
	if (v.isBool())
		return v.toBool() ? QStringLiteral("true") : QStringLiteral("false");
	return {};
}

double jnum(const QJsonValue &v, double fallback)
{
	if (v.isDouble())
		return v.toDouble();
	bool ok = false;
	double d = v.toString().toDouble(&ok);
	return ok ? d : fallback;
}

QStringList jstrings(const QJsonValue &arr)
{
	QStringList out;
	for (const QJsonValue &v : arr.toArray())
		if (v.isString() || v.isDouble())
			out << jstr(v);
	return out;
}

NameStyle nameStyle()
{
	QSettings &s = settings();
	NameStyle n;
	n.prefixMode = qBound(0, s.value(QStringLiteral("naming/prefix_mode"), n.prefixMode).toInt(), 2);
	n.prefix = s.value(QStringLiteral("naming/prefix"), n.prefix).toString();
	n.suffixMode = qBound(0, s.value(QStringLiteral("naming/suffix_mode"), n.suffixMode).toInt(), 2);
	n.suffix = s.value(QStringLiteral("naming/suffix"), n.suffix).toString();
	return n;
}

void setNameStyle(const NameStyle &n)
{
	QSettings &s = settings();
	s.setValue(QStringLiteral("naming/prefix_mode"), n.prefixMode);
	s.setValue(QStringLiteral("naming/prefix"), n.prefix);
	s.setValue(QStringLiteral("naming/suffix_mode"), n.suffixMode);
	s.setValue(QStringLiteral("naming/suffix"), n.suffix);
}

QString decorateName(const QString &base, const NameStyle &n, const QDateTime &when)
{
	const QString stamp = when.toString(QStringLiteral("yyyy-MM-dd_HH-mm-ss"));
	auto clean = [](QString s) {
		static const QString bad = QStringLiteral("<>:\"/\\|?*");
		s.removeIf([](QChar c) { return c.unicode() < 32 || bad.contains(c); });
		return s;
	};
	const QString prefix = n.prefixMode == 1 ? QStringLiteral("[%1] ").arg(stamp)
	                     : n.prefixMode == 2 ? clean(n.prefix) : QString();
	const QString suffix = n.suffixMode == 1 ? QStringLiteral(" [%1]").arg(stamp)
	                     : n.suffixMode == 2 ? clean(n.suffix) : QString();
	return prefix + base + suffix;
}

static QVector<LanguageCode> g_languages;

const QVector<LanguageCode> &languages()
{
	if (g_languages.isEmpty())
		loadLanguages(QString());
	return g_languages;
}

void loadLanguages(const QString &root)
{
	QVector<LanguageCode> &list = g_languages;
	list.clear();

	QByteArray json;
	wad::Archive jason;
	if (jason.open(wad::find(root, QStringLiteral("jason.wad"))))
		json = jason.read(QStringLiteral("Language.json"));
	if (json.isEmpty()) {
		QFile f(QStringLiteral(":/ifme/Language.json"));
		if (f.open(QIODevice::ReadOnly))
			json = f.readAll();
	}
	if (json.startsWith("\xEF\xBB\xBF"))   // UTF-8 BOM
		json.remove(0, 3);
	const QJsonObject o = QJsonDocument::fromJson(json).object();
	for (auto i = o.constBegin(); i != o.constEnd(); ++i)
		list.push_back({ i.key(), i.value().toString() });
	std::sort(list.begin(), list.end(), [](const LanguageCode &a, const LanguageCode &b) {
		return QString::localeAwareCompare(a.name, b.name) < 0;
	});
}

QString languageName(const QString &code)
{
	for (const LanguageCode &l : languages())
		if (l.code == code)
			return l.name;
	return code;
}

double parseTime(const QString &s)
{
	const QStringList parts = s.trimmed().split(QLatin1Char(':'));
	if (parts.isEmpty() || parts.size() > 3)
		return -1;
	double total = 0;
	for (const QString &p : parts) {
		bool ok = false;
		double v = p.toDouble(&ok);
		if (!ok || v < 0)
			return -1;
		total = total * 60 + v;
	}
	return total;
}

QString formatDuration(double seconds)
{
	if (seconds < 0)
		return QStringLiteral("--:--:--");
	qint64 ms = qRound64(seconds * 1000);
	return QStringLiteral("%1:%2:%3.%4")
		.arg(ms / 3600000, 2, 10, QLatin1Char('0'))
		.arg(ms / 60000 % 60, 2, 10, QLatin1Char('0'))
		.arg(ms / 1000 % 60, 2, 10, QLatin1Char('0'))
		.arg(ms % 1000, 3, 10, QLatin1Char('0'));
}

QString formatSize(qint64 bytes)
{
	const char *unit[] = { "B", "KiB", "MiB", "GiB", "TiB" };
	double v = static_cast<double>(bytes);
	int u = 0;
	while (v >= 1024 && u < 4) {
		v /= 1024;
		u++;
	}
	return QStringLiteral("%1 %2").arg(v, 0, 'f', u ? 1 : 0).arg(QLatin1String(unit[u]));
}

}  // namespace app
