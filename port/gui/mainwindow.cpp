#include "mainwindow.h"

#include <QAction>
#include <QApplication>
#include <QCloseEvent>
#include <QComboBox>
#include <QCoreApplication>
#include <QDesktopServices>
#include <QDir>
#include <QDirIterator>
#include <QDragEnterEvent>
#include <QDropEvent>
#include <QFileDialog>
#include <QFileInfo>
#include <QFontDatabase>
#include <QHBoxLayout>
#include <QHeaderView>
#include <QJsonDocument>
#include <QLabel>
#include <QLineEdit>
#include <QMessageBox>
#include <QMimeData>
#include <QPlainTextEdit>
#include <QPushButton>
#include <QSplitter>
#include <QStandardPaths>
#include <QStatusBar>
#include <QStyle>
#include <QTabWidget>
#include <QThread>
#include <QTimer>
#include <QToolBar>
#include <QTreeWidget>
#include <QUrl>
#include <QVBoxLayout>

#include "aboutdialog.h"
#include "app.h"
#include "dialogs.h"
#include "encodeworker.h"
#include "settingstabs.h"
#include "widgets.h"

namespace {

enum Column { ColName, ColType, ColDuration, ColSize, ColStatus, ColProgress };

const QStringList kMediaExt = {
	QStringLiteral("mkv"), QStringLiteral("mp4"), QStringLiteral("m4v"), QStringLiteral("mov"),
	QStringLiteral("avi"), QStringLiteral("webm"), QStringLiteral("ts"), QStringLiteral("m2ts"),
	QStringLiteral("mts"), QStringLiteral("mpg"), QStringLiteral("mpeg"), QStringLiteral("vob"),
	QStringLiteral("wmv"), QStringLiteral("flv"), QStringLiteral("3gp"), QStringLiteral("ogv"),
	QStringLiteral("m4a"), QStringLiteral("mp3"), QStringLiteral("flac"), QStringLiteral("ogg"),
	QStringLiteral("opus"), QStringLiteral("wav"), QStringLiteral("wma"), QStringLiteral("aac"),
	QStringLiteral("ac3"), QStringLiteral("mka"),
};

// Media Info tab: the same layout as C#'s MediaInfo view.
QString mediaText(const QJsonObject &m)
{
	QStringList out;
	auto row = [&](const QString &label, const QString &value) {
		out << QStringLiteral("%1 : %2").arg(label, -28).arg(value);
	};
	auto s = [](const QJsonObject &o, const char *k) { return o.value(QLatin1String(k)).toString(); };
	auto n = [](const QJsonObject &o, const char *k) { return o.value(QLatin1String(k)).toDouble(); };

	out << QStringLiteral("General");
	row(QStringLiteral("Complete name"), s(m, "path"));
	row(QStringLiteral("Format"), QStringLiteral("%1 (%2)").arg(s(m, "format_name"), s(m, "format_long_name")));
	row(QStringLiteral("File size"), app::formatSize(static_cast<qint64>(n(m, "file_size"))));
	row(QStringLiteral("Duration"), app::formatDuration(n(m, "duration")));
	row(QStringLiteral("Overall bit rate"), QStringLiteral("%1 kb/s").arg(n(m, "bit_rate") / 1000, 0, 'f', 0));
	row(QStringLiteral("Writing application"), s(m, "encoder"));

	const QJsonArray video = m.value(QStringLiteral("video")).toArray();
	for (int i = 0; i < video.size(); i++) {
		const QJsonObject v = video[i].toObject();
		out << QString() << QStringLiteral("Video #%1%2").arg(i)
			.arg(v.value(QStringLiteral("attached_pic")).toBool() ? QStringLiteral(" (cover art)") : QString());
		row(QStringLiteral("ID"), QString::number(v.value(QStringLiteral("id")).toInt()));
		row(QStringLiteral("Format"), s(v, "codec"));
		row(QStringLiteral("Resolution"), QStringLiteral("%1 x %2").arg(n(v, "width")).arg(n(v, "height")));
		row(QStringLiteral("Frame rate"), QStringLiteral("%1 fps (%2)").arg(n(v, "frame_rate_avg"), 0, 'f', 3)
			.arg(v.value(QStringLiteral("frame_rate_constant")).toBool() ? QStringLiteral("constant") : QStringLiteral("variable")));
		row(QStringLiteral("Frame count"), QString::number(static_cast<qint64>(n(v, "frame_count"))));
		row(QStringLiteral("Chroma / bit depth"), QStringLiteral("%1 (%2), %3-bit").arg(n(v, "chroma")).arg(s(v, "pix_fmt")).arg(n(v, "bit_depth")));
		row(QStringLiteral("Language"), s(v, "language"));
		row(QStringLiteral("Title"), s(v, "title"));
	}
	const QJsonArray audio = m.value(QStringLiteral("audio")).toArray();
	for (int i = 0; i < audio.size(); i++) {
		const QJsonObject a = audio[i].toObject();
		out << QString() << QStringLiteral("Audio #%1").arg(i);
		row(QStringLiteral("ID"), QString::number(a.value(QStringLiteral("id")).toInt()));
		row(QStringLiteral("Format"), s(a, "codec"));
		row(QStringLiteral("Channel(s)"), QString::number(n(a, "channels")));
		row(QStringLiteral("Sampling rate"), QStringLiteral("%1 Hz").arg(n(a, "sample_rate")));
		row(QStringLiteral("Bit depth"), QStringLiteral("%1 bits").arg(n(a, "bit_depth")));
		row(QStringLiteral("Language"), s(a, "language"));
		row(QStringLiteral("Title"), s(a, "title"));
	}
	const QJsonArray subs = m.value(QStringLiteral("subtitle")).toArray();
	for (int i = 0; i < subs.size(); i++) {
		const QJsonObject t = subs[i].toObject();
		out << QString() << QStringLiteral("Subtitle #%1").arg(i);
		row(QStringLiteral("ID"), QString::number(t.value(QStringLiteral("id")).toInt()));
		row(QStringLiteral("Format"), s(t, "codec"));
		row(QStringLiteral("Language"), s(t, "language"));
		row(QStringLiteral("Title"), s(t, "title"));
	}
	const QJsonArray att = m.value(QStringLiteral("attachment")).toArray();
	for (int i = 0; i < att.size(); i++) {
		const QJsonObject t = att[i].toObject();
		out << QString() << QStringLiteral("Attachment #%1").arg(i);
		row(QStringLiteral("File name"), s(t, "filename"));
		row(QStringLiteral("MIME"), s(t, "mime_type"));
	}
	return out.join(QLatin1Char('\n'));
}

QString stateText(QueueItem::State s)
{
	switch (s) {
	case QueueItem::Ready: return QObject::tr("Ready");
	case QueueItem::Encoding: return QObject::tr("Encoding");
	case QueueItem::Done: return QObject::tr("Done");
	case QueueItem::Failed: return QObject::tr("Failed");
	case QueueItem::Stopped: return QObject::tr("Stopped");
	}
	return {};
}

}  // namespace

MainWindow::MainWindow(app::Core &core, QWidget *parent)
	: QMainWindow(parent)
	, m_core(core)
{
	setWindowTitle(QStringLiteral("Internet Friendly Media Encoder %1").arg(QString::fromUtf8(ifme_version())));
	setAcceptDrops(true);

	// ---- banner on top, toolbar under it (as frmMain) ----
	setMenuWidget(new BannerWidget(this));
	QToolBar *tb = addToolBar(tr("Queue"));
	tb->setMovable(false);
	tb->setToolButtonStyle(Qt::ToolButtonTextBesideIcon);
	QStyle *st = style();
	auto act = [&](QStyle::StandardPixmap icon, const QString &text, auto slot, bool edit = true) {
		QAction *a = tb->addAction(st->standardIcon(icon), text);
		connect(a, &QAction::triggered, this, slot);
		if (edit)
			m_editActions << a;
		return a;
	};
	act(QStyle::SP_DialogOpenButton, tr("&Add files"), [this] {
		addFiles(QFileDialog::getOpenFileNames(this, tr("Add media files"),
			app::settings().value(QStringLiteral("ui/last_open")).toString()));
	});
	act(QStyle::SP_DirOpenIcon, tr("Add &folder"), [this] {
		const QString dir = QFileDialog::getExistingDirectory(this, tr("Add a folder"));
		if (!dir.isEmpty())
			addFiles({ dir });
	});
	act(QStyle::SP_TrashIcon, tr("&Remove"), &MainWindow::removeSelected);
	act(QStyle::SP_DialogResetButton, tr("&Clear"), [this] {
		m_items.clear();
		m_queue->clear();
		selectionChanged();
	});
	act(QStyle::SP_ArrowUp, tr("Up"), [this] { moveSelected(-1); });
	act(QStyle::SP_ArrowDown, tr("Down"), [this] { moveSelected(1); });
	tb->addSeparator();
	act(QStyle::SP_FileDialogDetailedView, tr("&Plugins"), &MainWindow::showPlugins, false);
	act(QStyle::SP_FileDialogInfoView, tr("Pr&eferences"), &MainWindow::showPreferences);
	act(QStyle::SP_MessageBoxInformation, tr("A&bout"), &MainWindow::showAbout, false);
	auto *spacer = new QWidget(tb);
	spacer->setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Preferred);
	tb->addWidget(spacer);
	QAction *donate = tb->addAction(tr("&Donate"));   // text only, as frmMain
	connect(donate, &QAction::triggered, this, [] {
		QDesktopServices::openUrl(QUrl(QString::fromLatin1(app::DonateUrl)));
	});

	// ---- banner, queue, tabs ----
	auto *central = new QWidget(this);
	auto *v = new QVBoxLayout(central);
	v->setContentsMargins(0, 0, 0, 0);
	v->setSpacing(0);

	auto *body = new QWidget(central);
	auto *bv = new QVBoxLayout(body);
	v->addWidget(body, 1);

	auto *split = new QSplitter(Qt::Vertical, body);
	m_queue = new QTreeWidget(split);
	m_queue->setHeaderLabels({ tr("Name"), tr("Type"), tr("Duration"), tr("Size"), tr("Status"), tr("Progress") });
	m_queue->setRootIsDecorated(false);
	m_queue->setSelectionMode(QAbstractItemView::ExtendedSelection);
	m_queue->setItemDelegateForColumn(ColProgress, new ProgressDelegate(m_queue));
	m_queue->header()->setSectionResizeMode(ColName, QHeaderView::Stretch);
	m_queue->header()->setStretchLastSection(false);
	m_queue->setColumnWidth(ColType, 110);
	m_queue->setColumnWidth(ColDuration, 100);
	m_queue->setColumnWidth(ColSize, 90);
	m_queue->setColumnWidth(ColStatus, 80);
	m_queue->setColumnWidth(ColProgress, 160);

	m_tabs = new QTabWidget(split);
	m_info = new QPlainTextEdit(m_tabs);   // first tab, as in frmMain
	m_info->setReadOnly(true);
	m_info->setFont(QFontDatabase::systemFont(QFontDatabase::FixedFont));
	m_tabs->addTab(m_info, tr("Media Info"));
	auto addTab = [&](SettingsTab *t, const QString &name) {
		m_settingsTabs << t;
		m_tabs->addTab(t, name);
		connect(t, &SettingsTab::changed, this, &MainWindow::settingsChanged);
	};
	addTab(new VideoTab(m_core, m_tabs), tr("Video"));
	addTab(new AudioTab(m_core, m_tabs), tr("Audio"));
	addTab(new SubtitleTab(m_core, m_tabs), tr("Subtitle"));
	addTab(new AttachmentTab(m_core, m_tabs), tr("Attachment"));
	addTab(new AdvancedTab(m_core, m_tabs), tr("Advanced"));
	m_log = new QPlainTextEdit(m_tabs);
	m_log->setReadOnly(true);
	m_log->setFont(QFontDatabase::systemFont(QFontDatabase::FixedFont));
	m_log->setMaximumBlockCount(20000);
	m_tabs->addTab(m_log, tr("Logging"));
	split->setStretchFactor(0, 2);
	split->setStretchFactor(1, 3);
	bv->addWidget(split, 1);

	// ---- format, output folder, start/pause/stop ----
	auto *bar = new QHBoxLayout;
	m_format = new QComboBox(body);
	for (const app::Container &c : app::containers())
		m_format->addItem(QString::fromLatin1(c.ext).toUpper(), QString::fromLatin1(c.ext));
	m_outDir = new QLineEdit(app::settings().value(QStringLiteral("paths/output"),
		QDir::toNativeSeparators(QStandardPaths::writableLocation(QStandardPaths::MoviesLocation))).toString(), body);
	m_outDir->setPlaceholderText(tr("next to the source file"));
	auto *browse = new QPushButton(tr("Browse..."), body);
	m_start = new QPushButton(st->standardIcon(QStyle::SP_MediaPlay), tr("&Start"), body);
	m_pause = new QPushButton(st->standardIcon(QStyle::SP_MediaPause), tr("&Pause"), body);
	m_stop = new QPushButton(st->standardIcon(QStyle::SP_MediaStop), tr("S&top"), body);
	m_pause->setEnabled(false);
	m_stop->setEnabled(false);
	bar->addWidget(new QLabel(tr("Format:"), body));
	bar->addWidget(m_format);
	bar->addWidget(new QLabel(tr("Save to:"), body));
	bar->addWidget(m_outDir, 1);
	bar->addWidget(browse);
	bar->addWidget(m_start);
	bar->addWidget(m_pause);
	bar->addWidget(m_stop);
	bv->addLayout(bar);

	m_progressText = new QLabel(this);
	statusBar()->addWidget(m_progressText, 1);
	setCentralWidget(central);

	const QString fmt = app::settings().value(QStringLiteral("ui/format"), QStringLiteral("mkv")).toString();
	m_format->setCurrentIndex(qMax(0, m_format->findData(fmt)));

	connect(m_queue, &QTreeWidget::itemSelectionChanged, this, &MainWindow::selectionChanged);
	connect(m_format, &QComboBox::activated, this, &MainWindow::formatChanged);
	connect(browse, &QPushButton::clicked, this, [this] {
		const QString dir = QFileDialog::getExistingDirectory(this, tr("Save encoded files to"), m_outDir->text());
		if (!dir.isEmpty())
			setOutputDir(dir);
	});
	connect(m_outDir, &QLineEdit::editingFinished, this, [this] { setOutputDir(m_outDir->text()); });
	connect(m_start, &QPushButton::clicked, this, [this] { startEncoding(); });
	connect(m_pause, &QPushButton::clicked, this, [this] {
		if (!m_worker)
			return;
		if (m_worker->isPaused()) {
			m_worker->resume();
			m_pause->setText(tr("&Pause"));
		} else {
			m_worker->pause();
			m_pause->setText(tr("&Resume"));
		}
	});
	connect(m_stop, &QPushButton::clicked, this, [this] {
		if (m_worker)
			m_worker->stop();
	});

	resize(1024, 720);
	restoreGeometry(app::settings().value(QStringLiteral("ui/geometry")).toByteArray());
	selectionChanged();

	const int usable = static_cast<int>(std::count_if(m_core.plugins().begin(), m_core.plugins().end(),
		[](const app::Plugin &p) { return p.usable(); }));
	m_progressText->setText(usable ? tr("%1 encoders ready. Drop media files here to add them.").arg(usable)
	                               : tr("No plugins found in %1 - set the IFME folder in Preferences.")
	                                     .arg(QDir::toNativeSeparators(m_core.root())));
}

MainWindow::~MainWindow()
{
	app::settings().setValue(QStringLiteral("ui/geometry"), saveGeometry());
}

// ---- queue -----------------------------------------------------------------------

QueueItem *MainWindow::itemById(int id) const
{
	for (const auto &it : m_items)
		if (it->id == id)
			return it.get();
	return nullptr;
}

QTreeWidgetItem *MainWindow::rowOf(int id) const
{
	for (int i = 0; i < m_queue->topLevelItemCount(); i++)
		if (m_queue->topLevelItem(i)->data(0, Qt::UserRole).toInt() == id)
			return m_queue->topLevelItem(i);
	return nullptr;
}

QVector<QueueItem *> MainWindow::selected() const
{
	QVector<QueueItem *> out;
	// In queue order, the current row first (the tabs show it).
	QTreeWidgetItem *cur = m_queue->currentItem();
	if (cur && cur->isSelected())
		out << itemById(cur->data(0, Qt::UserRole).toInt());
	for (int i = 0; i < m_queue->topLevelItemCount(); i++) {
		QTreeWidgetItem *r = m_queue->topLevelItem(i);
		if (r->isSelected() && r != cur)
			out << itemById(r->data(0, Qt::UserRole).toInt());
	}
	out.removeAll(nullptr);
	return out;
}

QString MainWindow::outputFor(const QString &input, const QString &ext) const
{
	const QFileInfo fi(input);
	const QString dir = m_outDir->text().trimmed().isEmpty() ? fi.absolutePath() : m_outDir->text().trimmed();
	QString out = QDir(dir).filePath(fi.completeBaseName() + QLatin1Char('.') + ext);
	if (QFileInfo(out).absoluteFilePath().compare(fi.absoluteFilePath(), Qt::CaseInsensitive) == 0)
		out = QDir(dir).filePath(fi.completeBaseName() + QStringLiteral(".ifme.") + ext);
	return QDir::toNativeSeparators(out);
}

void MainWindow::addFiles(const QStringList &paths)
{
	QStringList files;
	for (const QString &p : paths) {
		const QFileInfo fi(p);
		if (fi.isDir()) {
			QDirIterator it(p, QDir::Files, QDirIterator::Subdirectories);
			QStringList inDir;
			while (it.hasNext()) {
				const QString f = it.next();
				if (kMediaExt.contains(QFileInfo(f).suffix().toLower()))
					inDir << f;
			}
			inDir.sort(Qt::CaseInsensitive);
			files << inDir;
		} else if (fi.exists()) {
			files << fi.absoluteFilePath();
		}
	}
	if (files.isEmpty())
		return;
	app::settings().setValue(QStringLiteral("ui/last_open"), QFileInfo(files.first()).absolutePath());

	QApplication::setOverrideCursor(Qt::WaitCursor);
	for (const QString &f : files) {
		m_progressText->setText(tr("Reading %1...").arg(QFileInfo(f).fileName()));
		QCoreApplication::processEvents(QEventLoop::ExcludeUserInputEvents);
		addFile(f);
	}
	QApplication::restoreOverrideCursor();
	m_progressText->setText(tr("%n file(s) in the queue.", nullptr, static_cast<int>(m_items.size())));
	if (!m_queue->currentItem() && m_queue->topLevelItemCount())
		m_queue->setCurrentItem(m_queue->topLevelItem(0));
}

void MainWindow::addFile(const QString &path)
{
	const QString ext = m_format->currentData().toString();
	QSettings &s = app::settings();
	const QString video = m_core.defaultVideo(ext, s.value(QStringLiteral("ui/video_plugin")).toString());
	// No audio encoder for this format: keep the audio as it is.
	const QString audio = m_core.defaultAudio(ext, s.value(QStringLiteral("ui/audio_plugin")).toString());

	QString err;
	auto item = std::make_unique<QueueItem>();
	item->input = path;
	item->media = m_core.probe(path, &err);
	if (!item->media.isEmpty())
		item->job = m_core.jobNew(path, outputFor(path, ext), video, audio, &err);
	if (item->media.isEmpty() || item->job.isEmpty()) {
		log(tr("Cannot add %1: %2").arg(path, err));
		m_progressText->setText(tr("Cannot add %1: %2").arg(QFileInfo(path).fileName(), err));
		return;
	}
	item->id = m_nextId++;
	item->info = mediaText(item->media);

	auto *row = new QTreeWidgetItem(m_queue);
	row->setData(0, Qt::UserRole, item->id);
	row->setFlags(row->flags() | Qt::ItemIsUserCheckable);
	row->setCheckState(ColName, Qt::Checked);
	refreshRow(*item);
	m_items.push_back(std::move(item));
}

void MainWindow::refreshRow(const QueueItem &it)
{
	QTreeWidgetItem *row = rowOf(it.id);
	if (!row) {
		// addFile() calls this before the item is stored: the row is the last one.
		row = m_queue->topLevelItem(m_queue->topLevelItemCount() - 1);
	}
	if (!row)
		return;
	const QFileInfo in(it.input);
	row->setText(ColName, in.fileName());
	row->setToolTip(ColName, QDir::toNativeSeparators(it.input) + QStringLiteral("\n→ ") +
	                         it.job.value(QStringLiteral("output")).toString());
	row->setText(ColType, QStringLiteral("%1 ► %2").arg(in.suffix().toLower(), it.container()));
	row->setText(ColDuration, app::formatDuration(it.media.value(QStringLiteral("duration")).toDouble()));
	row->setText(ColSize, app::formatSize(static_cast<qint64>(it.media.value(QStringLiteral("file_size")).toDouble())));
	row->setText(ColStatus, stateText(it.state));
	row->setToolTip(ColStatus, it.message);
	row->setData(ColProgress, Qt::UserRole, it.state == QueueItem::Encoding || it.state == QueueItem::Done
	                                            ? QVariant(it.state == QueueItem::Done ? 100.0 : qMax(0.0, it.percent))
	                                            : QVariant());
	row->setText(ColProgress, it.state == QueueItem::Failed ? it.message.section(QLatin1Char('\n'), 0, 0) : QString());
}

void MainWindow::selectionChanged()
{
	const QVector<QueueItem *> items = selected();
	m_loadingSelection = true;
	for (SettingsTab *t : m_settingsTabs)
		t->setItems(m_worker ? QVector<QueueItem *>() : items);
	m_info->setPlainText(items.isEmpty() ? QString() : items.first()->info);
	if (!items.isEmpty())
		m_format->setCurrentIndex(qMax(0, m_format->findData(items.first()->container())));
	m_loadingSelection = false;
}

void MainWindow::formatChanged()
{
	const QString ext = m_format->currentData().toString();
	app::settings().setValue(QStringLiteral("ui/format"), ext);
	QVector<QueueItem *> items = selected();
	if (items.isEmpty())
		for (const auto &it : m_items)
			items << it.get();

	QSettings &s = app::settings();
	for (QueueItem *it : items) {
		it->job[QStringLiteral("output")] = outputFor(it->input, ext);

		// As C#: an encoder the new format cannot hold is replaced by one it can.
		const QJsonArray video = it->job.value(QStringLiteral("video")).toArray();
		if (!app::isVideoContainer(ext)) {
			it->job[QStringLiteral("video")] = QJsonArray();
		} else {
			const QString cur = video.isEmpty() ? QString() : video.first().toObject().value(QStringLiteral("plugin")).toString().toLower();
			const app::Plugin *p = m_core.find(cur);
			const bool ok = cur == QLatin1String("copy") || (p && p->formats.contains(ext));
			if (video.isEmpty() || !ok)
				VideoTab::switchPlugin(m_core, *it,
					m_core.defaultVideo(ext, video.isEmpty() ? s.value(QStringLiteral("ui/video_plugin")).toString() : QString()));
		}

		QJsonArray audio = it->job.value(QStringLiteral("audio")).toArray();
		for (int i = 0; i < audio.size(); i++) {
			QJsonObject e = audio[i].toObject();
			const QString cur = e.value(QStringLiteral("plugin")).toString().toLower();
			const app::Plugin *p = m_core.find(cur);
			if (cur == QLatin1String("copy") || (p && p->formats.contains(ext)))
				continue;
			AudioTab::switchPlugin(m_core, e, m_core.defaultAudio(ext, s.value(QStringLiteral("ui/audio_plugin")).toString()));
			audio[i] = e;
		}
		it->job[QStringLiteral("audio")] = audio;
		refreshRow(*it);
	}
	selectionChanged();
}

void MainWindow::settingsChanged()
{
	const QVector<QueueItem *> items = selected();
	for (QueueItem *it : items)
		refreshRow(*it);
	if (items.isEmpty())
		return;

	// New files get the encoders chosen last (C# kept the UI's choice).
	QSettings &s = app::settings();
	const QJsonArray video = items.first()->job.value(QStringLiteral("video")).toArray();
	if (!video.isEmpty())
		s.setValue(QStringLiteral("ui/video_plugin"), video.first().toObject().value(QStringLiteral("plugin")).toString());
	const QJsonArray audio = items.first()->job.value(QStringLiteral("audio")).toArray();
	if (!audio.isEmpty())
		s.setValue(QStringLiteral("ui/audio_plugin"), audio.first().toObject().value(QStringLiteral("plugin")).toString());
}

void MainWindow::setFormat(const QString &ext)
{
	const int i = m_format->findData(ext.toLower());
	if (i < 0)
		return;
	m_format->setCurrentIndex(i);
	formatChanged();
}

void MainWindow::setOutputDir(const QString &dir)
{
	m_outDir->setText(QDir::toNativeSeparators(dir));
	app::settings().setValue(QStringLiteral("paths/output"), m_outDir->text());
	for (const auto &it : m_items) {
		it->job[QStringLiteral("output")] = outputFor(it->input, it->container());
		refreshRow(*it);
	}
}

void MainWindow::removeSelected()
{
	for (QueueItem *it : selected()) {
		delete rowOf(it->id);
		m_items.erase(std::remove_if(m_items.begin(), m_items.end(),
			[it](const std::unique_ptr<QueueItem> &p) { return p.get() == it; }), m_items.end());
	}
	selectionChanged();
}

void MainWindow::moveSelected(int delta)
{
	QList<QTreeWidgetItem *> rows = m_queue->selectedItems();
	std::sort(rows.begin(), rows.end(), [this, delta](QTreeWidgetItem *a, QTreeWidgetItem *b) {
		return delta < 0 ? m_queue->indexOfTopLevelItem(a) < m_queue->indexOfTopLevelItem(b)
		                 : m_queue->indexOfTopLevelItem(a) > m_queue->indexOfTopLevelItem(b);
	});
	QTreeWidgetItem *cur = m_queue->currentItem();
	for (QTreeWidgetItem *r : rows) {
		const int i = m_queue->indexOfTopLevelItem(r), j = i + delta;
		if (j < 0 || j >= m_queue->topLevelItemCount())
			return;
		m_queue->takeTopLevelItem(i);
		m_queue->insertTopLevelItem(j, r);
	}
	for (QTreeWidgetItem *r : rows)
		r->setSelected(true);
	if (cur)
		m_queue->setCurrentItem(cur, 0, QItemSelectionModel::NoUpdate);
}

// ---- encoding ---------------------------------------------------------------------

void MainWindow::startEncoding(bool askOverwrite)
{
	if (m_worker)
		return;

	QSettings &s = app::settings();
	const QString tempRoot = s.value(QStringLiteral("paths/temp")).toString();
	QVector<EncodeJob> jobs;
	int existing = 0;
	for (int i = 0; i < m_queue->topLevelItemCount(); i++) {
		QTreeWidgetItem *row = m_queue->topLevelItem(i);
		QueueItem *it = itemById(row->data(0, Qt::UserRole).toInt());
		if (!it || row->checkState(ColName) != Qt::Checked || it->state == QueueItem::Done)
			continue;
		QJsonObject job = it->job;
		job[QStringLiteral("output")] = outputFor(it->input, it->container());
		job[QStringLiteral("keep_temp")] = s.value(QStringLiteral("encode/keep_temp"), false).toBool();
		if (!tempRoot.isEmpty())
			job[QStringLiteral("temp_dir")] = QDir(tempRoot).filePath(
				QStringLiteral("ifme-%1-%2").arg(QCoreApplication::applicationPid()).arg(it->id));
		existing += QFileInfo::exists(job.value(QStringLiteral("output")).toString());
		QDir().mkpath(QFileInfo(job.value(QStringLiteral("output")).toString()).absolutePath());
		jobs.push_back({ it->id, QJsonDocument(job).toJson(QJsonDocument::Compact) });
	}
	if (jobs.isEmpty()) {
		m_progressText->setText(tr("Nothing to encode: add files, or tick the ones to encode."));
		if (m_quitWhenDone)
			QTimer::singleShot(0, qApp, [] { QCoreApplication::exit(1); });
		return;
	}
	if (askOverwrite && existing &&
	    QMessageBox::question(this, tr("Overwrite?"),
	        tr("%n output file(s) already exist. Overwrite them?", nullptr, existing)) != QMessageBox::Yes)
		return;

	m_thread = new QThread(this);
	m_worker = new EncodeWorker(m_core.root(), m_core.disabled(), jobs);
	m_worker->moveToThread(m_thread);
	connect(m_thread, &QThread::started, m_worker, &EncodeWorker::run);

	connect(m_worker, &EncodeWorker::itemStarted, this, [this](int id) {
		if (QueueItem *it = itemById(id)) {
			it->state = QueueItem::Encoding;
			it->percent = 0;
			it->message.clear();
			refreshRow(*it);
			log(QStringLiteral("[%1] %2").arg(QFileInfo(it->input).fileName(), tr("started")));
		}
	});
	connect(m_worker, &EncodeWorker::progress, this, [this](int id, double pct, const QString &text) {
		QueueItem *it = itemById(id);
		if (!it)
			return;
		if (pct >= 0)
			it->percent = pct;
		it->message = text;
		refreshRow(*it);
		m_progressText->setText(QStringLiteral("%1: %2").arg(QFileInfo(it->input).fileName(), text));
	});
	connect(m_worker, &EncodeWorker::output, this, [this](int, const QString &line) { log(line); });
	connect(m_worker, &EncodeWorker::stage, this, [this](int id, const QString &text) {
		log(QStringLiteral("==> ") + text);
		if (QueueItem *it = itemById(id)) {
			it->percent = 0;
			refreshRow(*it);
			m_progressText->setText(QStringLiteral("%1: %2").arg(QFileInfo(it->input).fileName(), text));
		}
	});
	connect(m_worker, &EncodeWorker::itemFinished, this, [this](int id, int result, const QString &error) {
		QueueItem *it = itemById(id);
		if (!it)
			return;
		it->state = result == IFME_TASK_DONE ? QueueItem::Done
		          : result == IFME_TASK_STOPPED ? QueueItem::Stopped : QueueItem::Failed;
		it->message = result == IFME_TASK_DONE ? it->job.value(QStringLiteral("output")).toString() : error;
		refreshRow(*it);
		log(QStringLiteral("[%1] %2%3").arg(QFileInfo(it->input).fileName(), stateText(it->state),
			error.isEmpty() || result == IFME_TASK_DONE ? QString() : QStringLiteral(": ") + error));
	});
	connect(m_worker, &EncodeWorker::finished, this, [this] {
		m_thread->quit();
		m_thread->wait();
		m_worker->deleteLater();
		m_thread->deleteLater();
		m_worker = nullptr;
		m_thread = nullptr;
		setEncoding(false);

		int done = 0, failed = 0;
		for (const auto &it : m_items) {
			done += it->state == QueueItem::Done;
			failed += it->state == QueueItem::Failed || it->state == QueueItem::Stopped;
		}
		m_progressText->setText(tr("Finished: %1 done, %2 failed or stopped.").arg(done).arg(failed));
		if (m_quitWhenDone)
			QTimer::singleShot(0, qApp, [failed] { QCoreApplication::exit(failed ? 1 : 0); });
	});

	setEncoding(true);
	m_tabs->setCurrentWidget(m_log);
	m_thread->start();
}

void MainWindow::setEncoding(bool on)
{
	m_start->setEnabled(!on);
	m_pause->setEnabled(on);
	m_stop->setEnabled(on);
	m_pause->setText(tr("&Pause"));
	m_format->setEnabled(!on);
	m_outDir->setEnabled(!on);
	for (QAction *a : m_editActions)
		a->setEnabled(!on);
	setAcceptDrops(!on);
	selectionChanged();   // settings are read-only while encoding
}

void MainWindow::log(const QString &line)
{
	m_log->appendPlainText(line);
}

// ---- dialogs ----------------------------------------------------------------------

void MainWindow::reloadPlugins()
{
	m_core.setRoot(app::settings().value(QStringLiteral("paths/root")).toString().isEmpty()
		? QCoreApplication::applicationDirPath() : app::settings().value(QStringLiteral("paths/root")).toString());
	m_core.loadPlugins();
	selectionChanged();
}

void MainWindow::showPlugins()
{
	PluginsDialog dlg(m_core, m_worker == nullptr, this);
	dlg.exec();
	if (dlg.changed())
		selectionChanged();
}

void MainWindow::showPreferences()
{
	PreferencesDialog dlg(this);
	if (dlg.exec() != QDialog::Accepted)
		return;
	m_outDir->setText(app::settings().value(QStringLiteral("paths/output")).toString());
	setOutputDir(m_outDir->text());
	if (dlg.rootChanged())
		reloadPlugins();
}

void MainWindow::showAbout()
{
	AboutDialog(this).exec();
}

// ---- drag and drop, closing -------------------------------------------------------

void MainWindow::dragEnterEvent(QDragEnterEvent *event)
{
	if (event->mimeData()->hasUrls() && !m_worker)
		event->acceptProposedAction();
}

void MainWindow::dropEvent(QDropEvent *event)
{
	QStringList paths;
	for (const QUrl &url : event->mimeData()->urls())
		if (url.isLocalFile())
			paths << url.toLocalFile();
	addFiles(paths);
}

void MainWindow::closeEvent(QCloseEvent *event)
{
	if (m_worker) {
		if (!m_quitWhenDone && QMessageBox::question(this, tr("Encoding"),
		        tr("Encoding is in progress. Stop it and quit?")) != QMessageBox::Yes) {
			event->ignore();
			return;
		}
		m_worker->stop();
		m_thread->quit();
		m_thread->wait();
	}
	event->accept();
}

// ---- screenshots (developer aid: --screenshots DIR) -------------------------------

void MainWindow::saveScreenshots(const QString &dir)
{
	QDir().mkpath(dir);
	if (m_queue->topLevelItemCount()) {
		m_queue->setCurrentItem(m_queue->topLevelItem(0));
		m_queue->topLevelItem(0)->setSelected(true);
	}
	for (int i = 0; i < m_tabs->count(); i++) {
		m_tabs->setCurrentIndex(i);
		QCoreApplication::processEvents();
		grab().save(QDir(dir).filePath(QStringLiteral("main-%1-%2.png").arg(i)
			.arg(m_tabs->tabText(i).toLower().remove(QLatin1Char(' ')))));
	}
	AboutDialog about(this);
	about.show();
	QCoreApplication::processEvents();
	about.grab().save(QDir(dir).filePath(QStringLiteral("about.png")));
	PluginsDialog plugins(m_core, true, this);
	plugins.show();
	QCoreApplication::processEvents();
	plugins.grab().save(QDir(dir).filePath(QStringLiteral("plugins.png")));
	PreferencesDialog prefs(this);
	prefs.show();
	QCoreApplication::processEvents();
	prefs.grab().save(QDir(dir).filePath(QStringLiteral("preferences.png")));
}
