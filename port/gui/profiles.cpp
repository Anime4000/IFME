#include "profiles.h"

#include <QDateTime>
#include <QDir>
#include <QDirIterator>
#include <QFile>
#include <QFileInfo>
#include <QJsonArray>
#include <QJsonDocument>
#include <QSaveFile>
#include <cmath>
#include <iterator>

#include "app.h"
#include "settingstabs.h"

using app::jget;
using app::jnum;
using app::jstr;

namespace profiles {

namespace {

const QString kVideoCopy = QStringLiteral("00000000-0000-0000-0000-000000000000");
const QString kAudioCopy = QStringLiteral("aaaaaaaa-0000-0000-0000-000000000000");

// C#'s FileContainer enum, in order (a number in "Container" is its index).
const char *const kCsContainers[] = { "3gp", "avi", "mp4", "mkv", "ts", "m2ts", "mpg", "mpeg", "webm",
                                      "wmv", "mp2", "mp3", "m4a", "ogg", "opus", "flac", "wma" };

QString containerOf(const QJsonValue &v)
{
	QString ext;
	if (v.isDouble()) {
		const int i = v.toInt(-1);
		if (i >= 0 && i < static_cast<int>(std::size(kCsContainers)))
			ext = QLatin1String(kCsContainers[i]);
	} else {
		ext = v.toString().trimmed().toLower();
	}
	if (ext == QLatin1String("mpeg"))
		ext = QStringLiteral("mpg");
	for (const app::Container &c : app::containers())
		if (ext == QLatin1String(c.ext))
			return ext;
	return {};
}

QJsonObject readJson(const QString &path, QString *error)
{
	QFile f(path);
	if (!f.open(QIODevice::ReadOnly)) {
		if (error)
			*error = f.errorString();
		return {};
	}
	QByteArray data = f.readAll();
	if (data.startsWith("\xEF\xBB\xBF"))   // the bundled profiles have a BOM
		data.remove(0, 3);
	QJsonParseError pe;
	const QJsonDocument doc = QJsonDocument::fromJson(data, &pe);
	if (!doc.isObject()) {
		if (error)
			*error = pe.errorString();
		return {};
	}
	return doc.object();
}

bool writeJson(const QString &path, const QJsonObject &o, QString *error)
{
	QSaveFile f(path);
	if (!f.open(QIODevice::WriteOnly) || f.write(QJsonDocument(o).toJson(QJsonDocument::Indented)) < 0 || !f.commit()) {
		if (error)
			*error = f.errorString();
		return false;
	}
	return true;
}

QString guidOf(const QJsonValue &v)
{
	return v.toString().trimmed().toLower();
}

// 23.976 -> "24000/1001" (the NTSC rates), 25 -> "25"; 0 -> "" (keep the source's).
QString fpsText(double f)
{
	if (f <= 0)
		return {};
	const double ntsc = f * 1.001;
	if (std::fabs(f - std::round(f)) > 0.001 && std::fabs(ntsc - std::round(ntsc)) < 0.01)
		return QStringLiteral("%1/1001").arg(static_cast<int>(std::round(ntsc)) * 1000);
	return QString::number(f, 'g', 6);
}

double fpsValue(const QString &s)
{
	const QStringList p = s.split(QLatin1Char('/'));
	if (p.size() == 2 && p[1].toDouble() > 0)
		return std::round(p[0].toDouble() / p[1].toDouble() * 1000) / 1000;
	return s.toDouble();
}

bool listHas(const QJsonArray &list, const char *key, int value)
{
	for (const QJsonValue &v : list)
		if (static_cast<int>(jnum(jget(v.toObject(), QLatin1String(key)))) == value)
			return true;
	return false;
}

QJsonObject firstSourceVideo(const QueueItem &it)
{
	for (const QJsonValue &v : it.media.value(QStringLiteral("video")).toArray())
		if (!v.toObject().value(QStringLiteral("attached_pic")).toBool())
			return v.toObject();
	return {};
}

}  // namespace

QString folder(const QString &root)
{
	return QDir(root).filePath(QStringLiteral("Profiles"));
}

QVector<Profile> load(const QString &root, QStringList *warnings)
{
	QStringList files;
	QDirIterator it(folder(root), { QStringLiteral("*.json") }, QDir::Files, QDirIterator::Subdirectories);
	while (it.hasNext())
		files << it.next();
	files.sort();   // as C#: OrderBy(file)

	QVector<Profile> out;
	for (const QString &f : files) {
		QString err;
		const QJsonObject o = readJson(f, &err);
		const QString name = jstr(jget(o, QStringLiteral("ProfileName"))).trimmed();
		if (o.isEmpty() || name.isEmpty()) {
			if (warnings)
				*warnings << QStringLiteral("Profile %1 skipped: %2").arg(QFileInfo(f).fileName(),
					err.isEmpty() ? QStringLiteral("no ProfileName") : err);
			continue;
		}
		Profile p;
		p.path = f;
		p.name = name;
		p.author = jstr(jget(o, QStringLiteral("ProfileAuthor")));
		p.url = jstr(jget(o, QStringLiteral("ProfileWebUrl")));
		p.container = containerOf(jget(o, QStringLiteral("Container")));
		p.json = o;
		out.push_back(p);
	}
	return out;
}

QStringList apply(app::Core &core, const Profile &p, QueueItem &item, const OutputFor &outputFor)
{
	QStringList warn;
	const QString ext = p.container.isEmpty() ? item.container() : p.container;
	if (p.container.isEmpty())
		warn << QStringLiteral("%1: unknown container \"%2\", kept .%3")
			.arg(p.name, jstr(jget(p.json, QStringLiteral("Container"))), ext);
	item.job[QStringLiteral("output")] = outputFor(item.input, ext);

	// ---- video ----
	const QJsonObject pv = jget(p.json, QStringLiteral("Video")).toObject();
	const QJsonObject venc = jget(pv, QStringLiteral("Encoder")).toObject();
	const QJsonObject vq = jget(pv, QStringLiteral("Quality")).toObject();
	const QJsonObject vdi = jget(pv, QStringLiteral("DeInterlace")).toObject();
	const QJsonObject src = firstSourceVideo(item);
	if (!app::isVideoContainer(ext) || src.isEmpty()) {
		item.job[QStringLiteral("video")] = QJsonArray();
	} else {
		QString guid = guidOf(jget(venc, QStringLiteral("Id")));
		if (guid == kVideoCopy)
			guid = QStringLiteral("copy");
		const app::Plugin *plug = core.find(guid);
		if (guid != QLatin1String("copy") && (!plug || !plug->formats.contains(ext))) {
			const QString fallback = core.defaultVideo(ext, QString());
			const app::Plugin *fp = core.find(fallback);
			warn << QStringLiteral("%1: video encoder %2 is not available for .%3, using %4")
				.arg(p.name, guid, ext, fp ? fp->name : fallback);
			VideoTab::switchPlugin(core, item, fallback);
		} else {
			VideoTab::switchPlugin(core, item, guid);
			QJsonArray va = item.job.value(QStringLiteral("video")).toArray();
			QJsonObject e = va.isEmpty() ? QJsonObject() : va.first().toObject();
			if (!e.isEmpty() && plug) {
				const QJsonObject vc = plug->section();
				auto setStr = [&](const char *key, const QJsonValue &v) {
					const QString s = jstr(v);
					if (!s.isEmpty())
						e[QLatin1String(key)] = s;
				};
				setStr("preset", jget(venc, QStringLiteral("Preset")));
				setStr("tune", jget(venc, QStringLiteral("Tune")));
				const QJsonArray modes = jget(vc, QStringLiteral("Mode")).toArray();
				int mode = static_cast<int>(jnum(jget(venc, QStringLiteral("Mode"))));
				if (mode < 0 || mode >= modes.size()) {
					warn << QStringLiteral("%1: %2 has no rate-control mode %3, using the first").arg(p.name, plug->name).arg(mode);
					mode = 0;
				}
				const QJsonObject m = modes.at(mode).toObject();
				e[QStringLiteral("mode")] = mode;
				e[QStringLiteral("value")] = jnum(jget(venc, QStringLiteral("Value")),
					jnum(jget(jget(m, QStringLiteral("Value")).toObject(), QStringLiteral("Default"))));
				e[QStringLiteral("passes")] = jget(m, QStringLiteral("MultiPass")).toBool()
					? qMax(2, static_cast<int>(jnum(jget(venc, QStringLiteral("MultiPass")), 2))) : 1;
				e[QStringLiteral("encoder_args")] = jstr(jget(venc, QStringLiteral("Command")));

				// 0 keeps the source's size / frame rate (C#: "auto").
				const int w = static_cast<int>(jnum(jget(vq, QStringLiteral("Width"))));
				const int h = static_cast<int>(jnum(jget(vq, QStringLiteral("Height"))));
				e[QStringLiteral("width")] = w > 0 && h > 0 ? w : src.value(QStringLiteral("width")).toInt();
				e[QStringLiteral("height")] = w > 0 && h > 0 ? h : src.value(QStringLiteral("height")).toInt();
				const QString fps = fpsText(jnum(jget(vq, QStringLiteral("FrameRate"))));
				if (fps.isEmpty())
					e.remove(QStringLiteral("fps"));
				else
					e[QStringLiteral("fps")] = fps;

				const int bits = static_cast<int>(jnum(jget(vq, QStringLiteral("BitDepth")), 8));
				if (listHas(jget(vc, QStringLiteral("Encoder")).toArray(), "BitDepth", bits))
					e[QStringLiteral("bit_depth")] = bits;
				else
					warn << QStringLiteral("%1: %2 has no %3-bit encoder, using %4-bit")
						.arg(p.name, plug->name).arg(bits).arg(e.value(QStringLiteral("bit_depth")).toInt());
				const int chroma = static_cast<int>(jnum(jget(vq, QStringLiteral("PixelFormat")), 420));
				if (listHas(jget(vc, QStringLiteral("Chroma")).toArray(), "Value", chroma))
					e[QStringLiteral("chroma")] = chroma;

				e[QStringLiteral("ffmpeg_args")] = jstr(jget(vq, QStringLiteral("Command")));
				e[QStringLiteral("filter")] = jstr(jget(vq, QStringLiteral("CommandFilter")));
				if (jget(vdi, QStringLiteral("Enable")).toBool())
					e[QStringLiteral("deinterlace")] = QJsonObject{
						{ QStringLiteral("mode"), static_cast<int>(jnum(jget(vdi, QStringLiteral("Mode")))) },
						{ QStringLiteral("field"), static_cast<int>(jnum(jget(vdi, QStringLiteral("Field")))) } };
				else
					e.remove(QStringLiteral("deinterlace"));
			}
			if (!e.isEmpty()) {
				e[QStringLiteral("copy")] = jget(p.json, QStringLiteral("TryRemuxVideo")).toBool();
				va[0] = e;
				item.job[QStringLiteral("video")] = va;
			}
		}
	}

	// ---- audio: every track gets the profile's encoder ----
	const QJsonObject pa = jget(p.json, QStringLiteral("Audio")).toObject();
	const QJsonObject aenc = jget(pa, QStringLiteral("Encoder")).toObject();
	QString aguid = guidOf(jget(aenc, QStringLiteral("Id")));
	if (aguid == kAudioCopy)
		aguid = QStringLiteral("copy");
	const app::Plugin *aplug = core.find(aguid);
	const bool aok = aguid == QLatin1String("copy") || (aplug && aplug->formats.contains(ext));
	QString use = aguid;
	if (!aok) {
		use = core.defaultAudio(ext, QString());
		const app::Plugin *fp = core.find(use);
		warn << QStringLiteral("%1: audio encoder %2 is not available for .%3, using %4")
			.arg(p.name, aguid, ext, fp ? fp->name : use);
	}
	const bool acopy = jget(p.json, QStringLiteral("TryRemuxAudio")).toBool() || jget(pa, QStringLiteral("Copy")).toBool();
	QJsonArray audio = item.job.value(QStringLiteral("audio")).toArray();
	for (int i = 0; i < audio.size(); i++) {
		QJsonObject e = audio[i].toObject();
		AudioTab::switchPlugin(core, e, use);
		if (aok && aplug) {
			const QJsonArray modes = jget(aplug->section(), QStringLiteral("Mode")).toArray();
			int mode = static_cast<int>(jnum(jget(aenc, QStringLiteral("Mode"))));
			if (mode < 0 || mode >= modes.size())
				mode = 0;
			e[QStringLiteral("mode")] = mode;
			const QString q = jstr(jget(aenc, QStringLiteral("Quality")));
			e[QStringLiteral("quality")] = q.isEmpty() ? jstr(jget(modes.at(mode).toObject(), QStringLiteral("Default"))) : q;
			e[QStringLiteral("sample_rate")] = static_cast<int>(jnum(jget(aenc, QStringLiteral("SampleRate")), e.value(QStringLiteral("sample_rate")).toInt()));
			e[QStringLiteral("channels")] = static_cast<int>(jnum(jget(aenc, QStringLiteral("Channel")), e.value(QStringLiteral("channels")).toInt()));
			e[QStringLiteral("encoder_args")] = jstr(jget(aenc, QStringLiteral("Command")));
			e[QStringLiteral("ffmpeg_args")] = jstr(jget(pa, QStringLiteral("Command")));
			e[QStringLiteral("filter")] = jstr(jget(pa, QStringLiteral("CommandFilter")));
		}
		if (e.value(QStringLiteral("plugin")).toString() != QLatin1String("copy"))
			e[QStringLiteral("copy")] = acopy;
		audio[i] = e;
	}
	item.job[QStringLiteral("audio")] = audio;
	item.profile = p.path;
	return warn;
}

QJsonObject fromItem(const QueueItem &item, const QString &name, const QString &author)
{
	const QJsonArray va = item.job.value(QStringLiteral("video")).toArray();
	const QJsonObject v = va.isEmpty() ? QJsonObject() : va.first().toObject();
	const QJsonArray aa = item.job.value(QStringLiteral("audio")).toArray();
	const QJsonObject a = aa.isEmpty() ? QJsonObject() : aa.first().toObject();
	const QJsonObject src = firstSourceVideo(item);

	auto plugin = [](const QJsonObject &e, const QString &copyGuid) {
		const QString p = e.value(QStringLiteral("plugin")).toString().toLower();
		return p.isEmpty() || p == QLatin1String("copy") ? copyGuid : p;
	};
	// The source's own size is saved as 0 ("auto"), so the profile fits any file.
	int w = v.value(QStringLiteral("width")).toInt(), h = v.value(QStringLiteral("height")).toInt();
	if (w == src.value(QStringLiteral("width")).toInt() && h == src.value(QStringLiteral("height")).toInt())
		w = h = 0;
	const double fps = fpsValue(v.value(QStringLiteral("fps")).toString());
	const QJsonObject di = v.value(QStringLiteral("deinterlace")).toObject();

	// C#'s FileContainer names (StringEnumConverter): MKV, MP4, 3GP...
	const QString container = item.container().toUpper();

	return QJsonObject{
		{ QStringLiteral("ProfileName"), name },
		{ QStringLiteral("ProfileAuthor"), author },
		{ QStringLiteral("ProfileWebUrl"), QString() },
		{ QStringLiteral("TryRemuxVideo"), v.value(QStringLiteral("copy")).toBool() },
		{ QStringLiteral("TryRemuxAudio"), a.value(QStringLiteral("copy")).toBool() },
		{ QStringLiteral("Container"), container },
		{ QStringLiteral("Video"), QJsonObject{
			{ QStringLiteral("Encoder"), QJsonObject{
				{ QStringLiteral("Id"), plugin(v, kVideoCopy) },
				{ QStringLiteral("Preset"), v.value(QStringLiteral("preset")).toString() },
				{ QStringLiteral("Tune"), v.value(QStringLiteral("tune")).toString() },
				{ QStringLiteral("Mode"), v.value(QStringLiteral("mode")).toInt() },
				{ QStringLiteral("Value"), v.value(QStringLiteral("value")).toDouble() },
				{ QStringLiteral("MultiPass"), qMax(1, v.value(QStringLiteral("passes")).toInt(1)) },
				{ QStringLiteral("Command"), v.value(QStringLiteral("encoder_args")).toString() } } },
			{ QStringLiteral("Quality"), QJsonObject{
				{ QStringLiteral("Command"), v.value(QStringLiteral("ffmpeg_args")).toString() },
				{ QStringLiteral("CommandFilter"), v.value(QStringLiteral("filter")).toString() },
				{ QStringLiteral("Width"), w },
				{ QStringLiteral("Height"), h },
				{ QStringLiteral("FrameRate"), fps },
				{ QStringLiteral("FrameRateAvg"), fps },
				{ QStringLiteral("BitDepth"), v.value(QStringLiteral("bit_depth")).toInt(8) },
				{ QStringLiteral("PixelFormat"), v.value(QStringLiteral("chroma")).toInt(420) } } },
			{ QStringLiteral("DeInterlace"), QJsonObject{
				{ QStringLiteral("Enable"), !di.isEmpty() },
				{ QStringLiteral("Field"), di.value(QStringLiteral("field")).toInt(0) },
				{ QStringLiteral("Mode"), di.value(QStringLiteral("mode")).toInt(1) } } } } },
		{ QStringLiteral("Audio"), QJsonObject{
			{ QStringLiteral("Encoder"), QJsonObject{
				{ QStringLiteral("Id"), plugin(a, kAudioCopy) },
				{ QStringLiteral("Mode"), a.value(QStringLiteral("mode")).toInt() },
				{ QStringLiteral("Quality"), jstr(a.value(QStringLiteral("quality"))) },
				{ QStringLiteral("Command"), a.value(QStringLiteral("encoder_args")).toString() },
				{ QStringLiteral("SampleRate"), a.value(QStringLiteral("sample_rate")).toInt() },
				{ QStringLiteral("Channel"), a.value(QStringLiteral("channels")).toInt() } } },
			{ QStringLiteral("Command"), a.value(QStringLiteral("ffmpeg_args")).toString() },
			{ QStringLiteral("CommandFilter"), a.value(QStringLiteral("filter")).toString() } } },
	};
}

QString save(const QString &root, const QJsonObject &profile, QString *error)
{
	const QDir dir(folder(root));
	if (!QDir().mkpath(dir.path())) {
		if (error)
			*error = QStringLiteral("cannot create %1").arg(QDir::toNativeSeparators(dir.path()));
		return {};
	}
	// C#: Profile_yyyy-MM-dd_HH-mm-ss_ffff.json (UTC, 1/10000 s).
	QDateTime now = QDateTime::currentDateTimeUtc();
	QString path;
	do {
		path = dir.filePath(QStringLiteral("Profile_%1_%2.json")
			.arg(now.toString(QStringLiteral("yyyy-MM-dd_HH-mm-ss")))
			.arg(now.time().msec() * 10, 4, 10, QLatin1Char('0')));
		now = now.addMSecs(1);
	} while (QFileInfo::exists(path));
	return writeJson(path, profile, error) ? path : QString();
}

bool rename(const Profile &p, const QString &name, QString *error)
{
	QJsonObject o = readJson(p.path, error);
	if (o.isEmpty())
		return false;
	o[QStringLiteral("ProfileName")] = name;
	return writeJson(p.path, o, error);
}

bool remove(const Profile &p, QString *error, bool toTrash)
{
	QFile f(p.path);
	if ((toTrash && f.moveToTrash()) || f.remove())
		return true;
	if (error)
		*error = f.errorString();
	return false;
}

}  // namespace profiles
