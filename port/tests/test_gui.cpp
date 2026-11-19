// test_gui.cpp - the settings tabs' track rules, off-screen: the Language box
// on the Video, Audio and Subtitle tabs, batching over several queued files,
// and list selections that stay put (whole rows, kept across reloads).
#include <QApplication>
#include <QCheckBox>
#include <QDir>
#include <QFileInfo>
#include <QJsonArray>
#include <QTemporaryDir>
#include <QTreeWidget>
#include <cstdio>

#include "app.h"
#include "profiles.h"
#include "queueitem.h"
#include "settingstabs.h"
#include "widgets.h"

static int g_checks = 0, g_failed = 0;

#define CHECK(cond)                                                                  \
	do {                                                                             \
		g_checks++;                                                                  \
		if (!(cond)) {                                                               \
			g_failed++;                                                              \
			std::fprintf(stderr, "FAIL %s:%d: %s\n", __FILE__, __LINE__, #cond);     \
		}                                                                            \
	} while (0)

static QJsonObject track(int stream, const char *lang)
{
	return { { QStringLiteral("stream"), stream }, { QStringLiteral("language"), QLatin1String(lang) },
	         { QStringLiteral("plugin"), QStringLiteral("copy") } };
}

static QueueItem item(const QString &name)
{
	QueueItem it;
	it.input = name + QStringLiteral(".mkv");
	it.job = QJsonObject{
		{ QStringLiteral("input"), it.input },
		{ QStringLiteral("output"), name + QStringLiteral(".out.mkv") },
		{ QStringLiteral("video"), QJsonArray{ track(0, "und") } },
		{ QStringLiteral("audio"), QJsonArray{ track(1, "eng"), track(2, "eng") } },
		{ QStringLiteral("subtitles"), QJsonArray{ track(3, "eng"), track(4, "eng") } },
	};
	it.media = QJsonObject{ { QStringLiteral("video"), QJsonArray{ QJsonObject{
		{ QStringLiteral("id"), 0 }, { QStringLiteral("codec"), QStringLiteral("hevc") },
		{ QStringLiteral("width"), 1280 }, { QStringLiteral("height"), 720 },
		{ QStringLiteral("language"), QStringLiteral("und") } } } } };
	return it;
}

static QString lang(const QueueItem &it, const char *key, int i)
{
	return it.job.value(QLatin1String(key)).toArray().at(i).toObject().value(QStringLiteral("language")).toString();
}

// Pick `code` in the tab's Language box as a user would.
static void choose(QWidget *tab, const char *code)
{
	LanguageCombo *c = tab->findChild<LanguageCombo *>();
	const int i = c->findData(QLatin1String(code));
	c->setCurrentIndex(i);
	emit c->activated(i);
}

static QTreeWidget *list(QWidget *tab)
{
	return tab->findChild<QTreeWidget *>();
}

static void select(QTreeWidget *t, std::initializer_list<int> rows)
{
	t->clearSelection();
	for (int r : rows)
		t->topLevelItem(r)->setSelected(true);
}

static void testTrackTab(SettingsTab *tab, const char *key)
{
	QueueItem a = item(QStringLiteral("a")), b = item(QStringLiteral("b"));
	QTreeWidget *t = list(tab);

	// Two files, second track selected: that position in both files.
	tab->setItems({ &a, &b });
	select(t, { 1 });
	choose(tab, "jpn");
	CHECK(lang(a, key, 0) == "eng" && lang(a, key, 1) == "jpn");
	CHECK(lang(b, key, 0) == "eng" && lang(b, key, 1) == "jpn");
	// The selection stays after the list reloads, and the row shows the name.
	CHECK(t->selectedItems().size() == 1 && t->indexOfTopLevelItem(t->selectedItems().first()) == 1);
	CHECK(t->topLevelItem(1)->text(1) == "Japanese");

	// Nothing selected: every track of every file.
	t->clearSelection();
	choose(tab, "kor");
	for (int i = 0; i < 2; i++)
		CHECK(lang(a, key, i) == "kor" && lang(b, key, i) == "kor");

	// One file, first track: only that track of that file.
	tab->setItems({ &a });
	select(t, { 0 });
	choose(tab, "spa");
	CHECK(lang(a, key, 0) == "spa" && lang(a, key, 1) == "kor");
	CHECK(lang(b, key, 0) == "kor");

	// The box follows the selected track.
	select(t, { 1 });
	CHECK(tab->findChild<LanguageCombo *>()->code() == "kor");
	select(t, { 0 });
	CHECK(tab->findChild<LanguageCombo *>()->code() == "spa");

	// Whole-row selection that keeps its colour without focus.
	CHECK(t->selectionBehavior() == QAbstractItemView::SelectRows);
	CHECK(t->palette().color(QPalette::Inactive, QPalette::Highlight) ==
	      t->palette().color(QPalette::Active, QPalette::Highlight));
	tab->setItems({});
}

static QJsonObject firstOf(const QueueItem &it, const char *key)
{
	return it.job.value(QLatin1String(key)).toArray().first().toObject();
}

static void testProfiles(const QString &root)
{
	app::Core core;
	core.setRoot(root);
	core.loadPlugins();
	QTemporaryDir out;
	const auto outputFor = [&](const QString &in, const QString &ext) {
		return QDir(out.path()).filePath(QFileInfo(in).completeBaseName() + QLatin1Char('.') + ext);
	};
	// The bundled samples: .mp4 has only video, .m4a only audio.
	auto newItem = [&](const QString &ext, const char *sample = "wonderland_online_dungeon.mp4") {
		QueueItem it;
		it.input = QDir(root).filePath(QStringLiteral("Samples/") + QLatin1String(sample));
		QString err;
		it.media = core.probe(it.input, &err);
		it.job = core.jobNew(it.input, outputFor(it.input, ext), core.defaultVideo(ext, QString()),
		                     core.defaultAudio(ext, QString()), &err);
		return it;
	};

	QStringList warnings;
	const QVector<profiles::Profile> list = profiles::load(root, &warnings);
	CHECK(list.size() >= 10 && warnings.isEmpty());
	const profiles::Profile *x265 = nullptr, *wmv = nullptr;
	for (const profiles::Profile &p : list) {
		if (p.path.endsWith(QStringLiteral("Profile_1000-00-00_00-00-00_0002.json")))
			x265 = &p;
		if (p.path.endsWith(QStringLiteral("longwu9t_wmv-mso_pp.json")))
			wmv = &p;
	}
	CHECK(x265 && x265->container == "mkv" && x265->name.contains("x265"));
	CHECK(wmv && wmv->container == "wmv" && wmv->author == "longwu9t");
	if (!x265 || !wmv)
		return;

	// x265 + Nero AAC into MKV (the file was queued as MP4).
	QueueItem a = newItem(QStringLiteral("mp4"));
	CHECK(!a.job.isEmpty());
	const QStringList w = profiles::apply(core, *x265, a, outputFor);
	CHECK(a.container() == "mkv" && a.profile == x265->path);
	const QJsonObject v = firstOf(a, "video");
	CHECK(v.value("plugin").toString() == "deadbeef-0265-0265-0265-026502650265");
	CHECK(v.value("preset").toString() == "veryslow" && v.value("tune").toString() == "ssim");
	CHECK(v.value("value").toDouble() == 24.0 && v.value("encoder_args").toString() == "--pme --pmode");
	CHECK(!v.contains("fps") && v.value("width").toInt() > 0);   // 0 in the profile: the source's
	CHECK(w.isEmpty() || !core.find(QStringLiteral("deadbeef-1aac-1aac-1aac-1aac2aac3aac")));

	// Its audio side on the audio sample, also queued as MP4.
	QueueItem m = newItem(QStringLiteral("mp4"), "wonderland_online_dungeon.m4a");
	const QStringList wa = profiles::apply(core, *x265, m, outputFor);
	CHECK(m.container() == "mkv" && m.job.value("video").toArray().isEmpty());
	const QJsonObject au = firstOf(m, "audio");
	if (core.find(QStringLiteral("deadbeef-1aac-1aac-1aac-1aac2aac3aac"))) {
		CHECK(au.value("plugin").toString() == "deadbeef-1aac-1aac-1aac-1aac2aac3aac");
		CHECK(au.value("quality").toString() == "0.6" && au.value("sample_rate").toInt() == 48000);
		CHECK(wa.isEmpty());
	} else {
		CHECK(!wa.isEmpty());   // Nero missing: replaced, and said so
	}

	// WMV for PowerPoint: container and encoder change together.
	QueueItem b = newItem(QStringLiteral("mkv"));
	profiles::apply(core, *wmv, b, outputFor);
	CHECK(b.container() == "wmv");
	if (core.find(QStringLiteral("deadbeef-9000-9000-9000-100920093009")))
		CHECK(firstOf(b, "video").value("plugin").toString() == "deadbeef-9000-9000-9000-100920093009");

	// Round trip in a scratch IFME folder, in the .NET file format.
	QTemporaryDir scratch;
	QString err;
	QueueItem both = a;   // the video item with the audio item's track
	both.job[QStringLiteral("audio")] = m.job.value(QStringLiteral("audio"));
	const QString path = profiles::save(scratch.path(),
		profiles::fromItem(both, QStringLiteral("Round trip"), QStringLiteral("tester")), &err);
	CHECK(!path.isEmpty() && QFileInfo(path).fileName().startsWith("Profile_"));
	QVector<profiles::Profile> mine = profiles::load(scratch.path());
	CHECK(mine.size() == 1 && mine.value(0).name == "Round trip" && mine.value(0).container == "mkv");
	if (mine.size() != 1)
		return;
	CHECK(mine[0].json.value("Container").toString() == "MKV");   // C#'s FileContainer name
	CHECK(mine[0].json.value("Video").toObject().value("Quality").toObject().value("Width").toInt() == 0);
	QueueItem c = newItem(QStringLiteral("mp4"));
	profiles::apply(core, mine[0], c, outputFor);
	const QJsonObject cv = firstOf(c, "video");
	for (const char *k : { "plugin", "preset", "tune", "value", "passes", "encoder_args", "bit_depth", "chroma" })
		CHECK(cv.value(QLatin1String(k)) == v.value(QLatin1String(k)));
	CHECK(c.container() == "mkv");
	QueueItem d = newItem(QStringLiteral("mp4"), "wonderland_online_dungeon.m4a");
	profiles::apply(core, mine[0], d, outputFor);
	const QJsonObject da = firstOf(d, "audio");
	for (const char *k : { "plugin", "mode", "quality", "sample_rate", "channels" })
		CHECK(da.value(QLatin1String(k)) == au.value(QLatin1String(k)));

	CHECK(profiles::rename(mine[0], QStringLiteral("Renamed"), &err));
	mine = profiles::load(scratch.path());
	CHECK(mine.size() == 1 && mine.value(0).name == "Renamed" && mine.value(0).container == "mkv");
	CHECK(!mine.isEmpty() && profiles::remove(mine[0], &err, false));
	CHECK(profiles::load(scratch.path()).isEmpty());
}

int main(int argc, char **argv)
{
	qputenv("QT_QPA_PLATFORM", "offscreen");
	QApplication qapp(argc, argv);
	QTemporaryDir tmp;
	app::setSettingsFile(QDir(tmp.path()).filePath(QStringLiteral("test.ini")));
	app::loadLanguages(QString());   // the built-in Language.json
	app::Core core;

	AudioTab audio(core);
	testTrackTab(&audio, "audio");
	SubtitleTab subs(core);
	testTrackTab(&subs, "subtitles");

	// Video: one entry per file; the Language box changes every selected file
	// and the stream list shows the language it will be written with.
	VideoTab video(core);
	QueueItem a = item(QStringLiteral("a")), b = item(QStringLiteral("b"));
	video.setItems({ &a, &b });
	choose(&video, "jpn");
	CHECK(lang(a, "video", 0) == "jpn" && lang(b, "video", 0) == "jpn");
	QTreeWidget *streams = list(&video);
	CHECK(streams->topLevelItemCount() == 1 && streams->topLevelItem(0)->text(6) == "Japanese");
	CHECK(streams->selectionMode() != QAbstractItemView::NoSelection);
	select(streams, { 0 });
	choose(&video, "eng");
	CHECK(streams->topLevelItem(0)->isSelected());   // kept across the reload

	// Fast remux: every selected file; disabled for "Copy", which keeps the
	// stream anyway (the test jobs use it).
	QCheckBox *remux = nullptr;
	for (QCheckBox *c : video.findChildren<QCheckBox *>())
		if (c->text().contains(QStringLiteral("emux")))
			remux = c;
	CHECK(remux && !remux->isEnabled());

	// Profiles: the bundled .NET ones, applied to a real file, and a round
	// trip through save / load / rename / delete. Needs the IFME folder.
	const QString root = argc > 1 ? QString::fromLocal8Bit(argv[1]) : QString();
	if (root.isEmpty() || !QFileInfo::exists(root + QStringLiteral("/Profiles")))
		std::printf("profiles: skipped (no IFME folder with Profiles/)\n");
	else
		testProfiles(root);

	std::printf("%d checks, %d failed\n", g_checks, g_failed);
	return g_failed ? 1 : 0;
}
