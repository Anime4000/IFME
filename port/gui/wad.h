// wad.h - IFME's .wad files: plain zip archives next to the program, as IFME 8
// reads them (WAD.cs): resources.wad holds the artwork (splash screen, banners,
// About), jason.wad the language list. Replace a file inside to change the
// artwork without rebuilding. Entries may be stored (7za -mx=0, as IFME 8's
// build makes them) or deflated (any zip tool).
#pragma once

#include <QByteArray>
#include <QHash>
#include <QPixmap>
#include <QString>

namespace wad {

class Archive
{
public:
	bool open(const QString &path);          // false: missing or not a zip
	bool isOpen() const { return !m_data.isEmpty(); }
	QString path() const { return m_path; }
	bool contains(const QString &name) const;
	QByteArray read(const QString &name) const;   // empty when missing or unreadable

private:
	struct Entry { quint16 method = 0; quint32 packed = 0, size = 0, offset = 0; };
	QString m_path;
	QByteArray m_data;
	QHash<QString, Entry> m_entries;   // lower-case name
};

// Find `file` (e.g. "resources.wad") in `root` (the IFME folder) or next to
// the program; empty when there is none.
QString find(const QString &root, const QString &file);

// The artwork: resources.wad from the IFME folder, opened once.
void openResources(const QString &root);
QString resourcesPath();                      // empty when not found
QPixmap image(const QString &name);           // e.g. "SplashScreen14.png"; null when missing

}  // namespace wad
