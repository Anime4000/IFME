// test_gui.cpp - the settings tabs' track rules, off-screen: the Language box
// on the Video, Audio and Subtitle tabs, batching over several queued files,
// and list selections that stay put (whole rows, kept across reloads).
#include <QApplication>
#include <QCheckBox>
#include <QDir>
#include <QJsonArray>
#include <QTemporaryDir>
#include <QTreeWidget>
#include <cstdio>

#include "app.h"
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

	std::printf("%d checks, %d failed\n", g_checks, g_failed);
	return g_failed ? 1 : 0;
}
