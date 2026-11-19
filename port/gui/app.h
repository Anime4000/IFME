// app.h - shared GUI state: the IFME core context, settings, plugin list,
// containers and small JSON helpers.
#pragma once

#include <QJsonArray>
#include <QJsonObject>
#include <QSettings>
#include <QString>
#include <QStringList>
#include <QVector>

#include "ifme.h"

namespace app {

// Shown on the splash screen and in About, as in the C# release.
inline constexpr const char *CodeName = "Yoake no Shirabe";   // same as the .NET 8.7.1 release
inline constexpr const char *Contributors = "Nemu, Jel42, Robin Lawrie, Ron Mitschke, Project Angel";
inline constexpr const char *TradeMarks =
	"Anime4000, Rigaya, MulticoreWare, VideoLAN, IETF, Xiph.Org Foundation, Josh Coalson, "
	"Erik de Castro Lopo, Jean Le Feuvre, People@GPAC, FFmpeg Team, Nero AG, Moritz Bunkus, "
	"AMD, Intel, Nvidia, Google, Alliance for Open Media, et al.";

// User settings: an INI file in IFME's config folder (or the file given to
// setSettingsFile() before the first use, for tests).
QSettings &settings();
void setSettingsFile(const QString &path);

struct Plugin {
	QString guid, name, version, status, message;
	bool audio = false;
	bool testRequired = false;
	QJsonObject manifest;     // resolved manifest (Video / Audio section inside)
	QStringList formats;      // lower case
	bool usable() const { return status == QLatin1String("ok"); }
	QJsonObject section() const;   // manifest["Video"] or ["Audio"]
};

// One output container of the format list (C#: FileContainer).
struct Container {
	const char *ext;
	bool video;
};
const QVector<Container> &containers();
bool isVideoContainer(const QString &ext);

// The core context for the UI thread, with the plugin list read from it.
class Core {
public:
	Core();
	~Core();
	Core(const Core &) = delete;
	Core &operator=(const Core &) = delete;

	ifme_ctx *ctx() const { return m_ctx; }
	bool setRoot(const QString &dir);
	QString root() const;

	int loadPlugins();                       // load + apply the disabled list
	void setDisabled(const QStringList &guids);
	QStringList disabled() const { return m_disabled; }
	void refresh();                          // re-read statuses after a self-test

	const QVector<Plugin> &plugins() const { return m_plugins; }
	const Plugin *find(const QString &guid) const;
	QVector<const Plugin *> compatible(bool audio, const QString &container) const;
	QString defaultVideo(const QString &container, const QString &preferred) const;
	QString defaultAudio(const QString &container, const QString &preferred) const;

	QJsonObject jobNew(const QString &input, const QString &output, const QString &video,
	                   const QString &audio, QString *error) const;
	QJsonObject probe(const QString &path, QString *error) const;

private:
	ifme_ctx *m_ctx = nullptr;
	QVector<Plugin> m_plugins;
	QStringList m_disabled;
};

// ---- JSON helpers (manifests accept keys in any case, as Newtonsoft did) ----
QJsonValue jget(const QJsonObject &o, const QString &key);
QString jstr(const QJsonValue &v);           // strings and numbers as text
double jnum(const QJsonValue &v, double fallback = 0);
QStringList jstrings(const QJsonValue &arr);

// ISO 639-2 languages (code, name), sorted by name: Language.json from
// jason.wad in the IFME folder, as C# reads it (a built-in copy otherwise).
struct LanguageCode {
	QString code, name;
};
void loadLanguages(const QString &root);    // once, before languages()
const QVector<LanguageCode> &languages();
QString languageName(const QString &code);   // the code itself when unknown

// "00:01:02.500" / "62.5" -> seconds, -1 when invalid.
double parseTime(const QString &s);
QString formatDuration(double seconds);
QString formatSize(qint64 bytes);

}  // namespace app
