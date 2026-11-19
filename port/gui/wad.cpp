#include "wad.h"

#include <QCoreApplication>
#include <QDir>
#include <QFile>
#include <QFileInfo>

#include <zlib.h>

namespace wad {

namespace {

// Little-endian fields; out-of-range reads give 0 (the callers check sizes too).
quint16 u16(const QByteArray &d, qsizetype at)
{
	if (at < 0 || at + 2 > d.size())
		return 0;
	return quint16(uchar(d[at]) | uchar(d[at + 1]) << 8);
}

quint32 u32(const QByteArray &d, qsizetype at)
{
	if (at < 0 || at + 4 > d.size())
		return 0;
	return quint32(u16(d, at)) | quint32(u16(d, at + 2)) << 16;
}

// Raw deflate (zip method 8) -> `size` bytes.
QByteArray inflateRaw(const char *src, quint32 packed, quint32 size)
{
	QByteArray out(static_cast<qsizetype>(size), Qt::Uninitialized);
	z_stream z{};
	if (inflateInit2(&z, -MAX_WBITS) != Z_OK)
		return {};
	z.next_in = reinterpret_cast<Bytef *>(const_cast<char *>(src));
	z.avail_in = packed;
	z.next_out = reinterpret_cast<Bytef *>(out.data());
	z.avail_out = size;
	const int r = inflate(&z, Z_FINISH);
	inflateEnd(&z);
	return r == Z_STREAM_END && z.total_out == size ? out : QByteArray();
}

Archive g_resources;

}  // namespace

bool Archive::open(const QString &path)
{
	m_path.clear();
	m_data.clear();
	m_entries.clear();
	QFile f(path);
	if (!f.open(QIODevice::ReadOnly))
		return false;
	const QByteArray d = f.readAll();

	// End of central directory: the last "PK\5\6" within the final 64 KiB + 22 bytes.
	qsizetype eocd = -1;
	for (qsizetype i = d.size() - 22; i >= 0 && i >= d.size() - 22 - 65535; i--)
		if (u32(d, i) == 0x06054b50) {
			eocd = i;
			break;
		}
	if (eocd < 0)
		return false;
	const quint16 count = u16(d, eocd + 10);
	qsizetype at = u32(d, eocd + 16);

	QHash<QString, Entry> entries;
	for (quint16 n = 0; n < count; n++) {
		if (at + 46 > d.size() || u32(d, at) != 0x02014b50)
			return false;
		Entry e;
		e.method = u16(d, at + 10);
		e.packed = u32(d, at + 20);
		e.size = u32(d, at + 24);
		const quint16 nameLen = u16(d, at + 28), extraLen = u16(d, at + 30), commentLen = u16(d, at + 32);
		const quint32 local = u32(d, at + 42);
		const QString name = QString::fromUtf8(d.mid(at + 46, nameLen));
		at += 46 + nameLen + extraLen + commentLen;

		// The data follows the local header, whose name/extra lengths may differ.
		if (qsizetype(local) + 30 > d.size() || u32(d, local) != 0x04034b50)
			continue;
		e.offset = local + 30 + u16(d, local + 26) + u16(d, local + 28);
		if (qsizetype(e.offset) + e.packed > d.size())
			continue;
		entries.insert(name.toLower(), e);
	}
	m_path = path;
	m_data = d;
	m_entries = entries;
	return true;
}

bool Archive::contains(const QString &name) const
{
	return m_entries.contains(name.toLower());
}

QByteArray Archive::read(const QString &name) const
{
	const auto it = m_entries.constFind(name.toLower());
	if (it == m_entries.constEnd())
		return {};
	const char *src = m_data.constData() + it->offset;
	if (it->method == 0)
		return QByteArray(src, it->packed);
	if (it->method == 8)
		return inflateRaw(src, it->packed, it->size);
	return {};   // other methods (bzip2, LZMA...) are not used for .wad files
}

QString find(const QString &root, const QString &file)
{
	for (const QString &dir : { root, QCoreApplication::applicationDirPath() }) {
		const QString p = dir.isEmpty() ? QString() : QDir(dir).filePath(file);
		if (!p.isEmpty() && QFileInfo(p).isFile())
			return p;
	}
	return {};
}

void openResources(const QString &root)
{
	const QString p = find(root, QStringLiteral("resources.wad"));
	if (p.isEmpty() || !g_resources.open(p))
		g_resources = Archive();
}

QString resourcesPath()
{
	return g_resources.path();
}

QPixmap image(const QString &name)
{
	QPixmap px;
	if (!px.loadFromData(g_resources.read(name)))
		return {};
	return px;
}

}  // namespace wad
