// profiles.h - IFME's encoding profiles (Profiles/*.json in the IFME folder),
// in the same format as the .NET version (Profiles.cs, ProfilesManager.cs),
// so both read each other's files.
#pragma once

#include <QJsonObject>
#include <QString>
#include <QStringList>
#include <QVector>
#include <functional>

#include "queueitem.h"

namespace app { class Core; }

namespace profiles {

struct Profile {
	QString path;          // the .json file
	QString name, author, url;
	QString container;     // lower-case extension ("mkv"), empty when unknown
	QJsonObject json;      // the whole file
};

// Every Profiles/**/*.json under `root`, sorted by path (as C#). Files that do
// not parse are skipped and reported in `warnings`.
QVector<Profile> load(const QString &root, QStringList *warnings = nullptr);
QString folder(const QString &root);

// Apply `p` to `item`: container (output via `outputFor`), video and audio
// encoders and their settings, fast remux. Encoders the profile names that are
// missing, disabled or cannot go into its container are replaced by a default,
// each with a line in the returned warnings.
using OutputFor = std::function<QString(const QString &input, const QString &ext)>;
QStringList apply(app::Core &core, const Profile &p, QueueItem &item, const OutputFor &outputFor);

// A profile from `item`'s settings (its first video and audio track).
QJsonObject fromItem(const QueueItem &item, const QString &name, const QString &author);

// Write `profile` as a new Profiles/Profile_<UTC time>.json; its path, or
// empty with `error` set.
QString save(const QString &root, const QJsonObject &profile, QString *error);
bool rename(const Profile &p, const QString &name, QString *error);
bool remove(const Profile &p, QString *error, bool toTrash = true);   // recycle bin when possible

}  // namespace profiles
