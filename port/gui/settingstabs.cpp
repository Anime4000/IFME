#include "settingstabs.h"

#include <QCheckBox>
#include <QComboBox>
#include <QDoubleSpinBox>
#include <QFileDialog>
#include <QFileInfo>
#include <QFormLayout>
#include <QGroupBox>
#include <QHBoxLayout>
#include <QHeaderView>
#include <QLabel>
#include <QLineEdit>
#include <QMessageBox>
#include <QPushButton>
#include <QSpinBox>
#include <QTreeWidget>
#include <QVBoxLayout>

#include "app.h"

using app::jget;
using app::jnum;
using app::jstr;

namespace {

const QString kVideoCopy = QStringLiteral("00000000-0000-0000-0000-000000000000");
const QString kAudioCopy = QStringLiteral("aaaaaaaa-0000-0000-0000-000000000000");

QString normPlugin(const QJsonValue &v)
{
	const QString p = v.toString().toLower();
	return (p == kVideoCopy || p == kAudioCopy) ? QStringLiteral("copy") : p;
}

QJsonObject videoEntry(const QueueItem &it)
{
	const QJsonArray a = it.job.value(QStringLiteral("video")).toArray();
	return a.isEmpty() ? QJsonObject() : a.first().toObject();
}

void setVideoEntry(QueueItem &it, const QJsonObject &e)
{
	QJsonArray a = it.job.value(QStringLiteral("video")).toArray();
	if (a.isEmpty())
		a.append(e);
	else
		a[0] = e;
	it.job[QStringLiteral("video")] = a;
}

QJsonObject firstVideoStream(const QueueItem &it)
{
	for (const QJsonValue &v : it.media.value(QStringLiteral("video")).toArray())
		if (!v.toObject().value(QStringLiteral("attached_pic")).toBool())
			return v.toObject();
	return {};
}

int supportedOr(const QJsonArray &list, const QString &key, int want, int fallback)
{
	int first = 0;
	bool hasFallback = false;
	for (const QJsonValue &v : list) {
		int x = static_cast<int>(jnum(jget(v.toObject(), key)));
		if (x == want)
			return want;
		if (!first)
			first = x;
		hasFallback |= x == fallback;
	}
	return hasFallback || !first ? fallback : first;
}

// Fill a combo with (text, data) pairs and select `current` (by data).
void fill(QComboBox *c, const QList<QPair<QString, QVariant>> &items, const QVariant &current)
{
	c->clear();
	for (const auto &p : items)
		c->addItem(p.first, p.second);
	int i = c->findData(current);
	if (i < 0 && c->isEditable() && current.isValid() && !current.toString().isEmpty()) {
		c->addItem(current.toString(), current);
		i = c->count() - 1;
	}
	c->setCurrentIndex(i < 0 ? 0 : i);
}

QTreeWidget *makeTree(const QStringList &headers, QWidget *parent)
{
	auto *t = new QTreeWidget(parent);
	t->setHeaderLabels(headers);
	t->setRootIsDecorated(false);
	t->setSelectionMode(QAbstractItemView::ExtendedSelection);
	t->setEditTriggers(QAbstractItemView::DoubleClicked | QAbstractItemView::EditKeyPressed |
	                   QAbstractItemView::SelectedClicked);
	t->header()->setStretchLastSection(true);
	return t;
}

}  // namespace

// ---- SettingsTab -------------------------------------------------------------

void SettingsTab::setItems(const QVector<QueueItem *> &items)
{
	m_items = items;
	m_loading = true;
	setEnabled(!items.isEmpty());
	reload();
	m_loading = false;
}

void SettingsTab::each(const std::function<void(QueueItem &)> &fn)
{
	if (m_loading)
		return;
	for (QueueItem *it : m_items)
		fn(*it);
	emit changed();
}

// ---- Video ------------------------------------------------------------------

VideoTab::VideoTab(app::Core &core, QWidget *parent)
	: SettingsTab(core, parent)
{
	auto *top = new QVBoxLayout(this);
	m_notice = new QLabel(this);
	m_notice->setWordWrap(true);
	top->addWidget(m_notice);

	m_streams = makeTree({ tr("Id"), tr("Codec"), tr("Resolution"), tr("fps"), tr("Bit Depth"),
	                       tr("Pixel Format"), tr("Language") }, this);
	m_streams->setMaximumHeight(70);
	m_streams->setSelectionMode(QAbstractItemView::NoSelection);
	top->addWidget(m_streams);

	m_settings = new QWidget(this);
	top->addWidget(m_settings, 1);
	auto *cols = new QHBoxLayout(m_settings);
	cols->setContentsMargins(0, 0, 0, 0);

	auto *enc = new QGroupBox(tr("Encoder"), m_settings);
	auto *ef = new QFormLayout(enc);
	m_encoder = new QComboBox(enc);
	m_preset = new QComboBox(enc);
	m_tune = new QComboBox(enc);
	m_mode = new QComboBox(enc);
	m_value = new QDoubleSpinBox(enc);
	m_passes = new QSpinBox(enc);
	m_passes->setRange(2, 9);
	m_args = new QLineEdit(enc);
	m_args->setPlaceholderText(tr("extra encoder arguments"));
	ef->addRow(tr("&Encoder:"), m_encoder);
	ef->addRow(tr("&Preset:"), m_preset);
	ef->addRow(tr("&Tune:"), m_tune);
	ef->addRow(tr("Rate &control:"), m_mode);
	auto *vrow = new QHBoxLayout;
	vrow->addWidget(m_value, 1);
	vrow->addWidget(new QLabel(tr("Passes:"), enc));
	vrow->addWidget(m_passes);
	ef->addRow(tr("Value:"), vrow);
	ef->addRow(tr("Encoder CLI:"), m_args);
	cols->addWidget(enc, 3);

	auto *pic = new QGroupBox(tr("Picture"), m_settings);
	auto *pf = new QFormLayout(pic);
	m_width = new QSpinBox(pic);
	m_height = new QSpinBox(pic);
	m_width->setRange(0, 16384);
	m_height->setRange(0, 16384);
	auto *res = new QHBoxLayout;
	res->addWidget(m_width);
	res->addWidget(new QLabel(QStringLiteral("x"), pic));
	res->addWidget(m_height);
	m_fps = new QComboBox(pic);
	m_fps->setEditable(true);
	m_bits = new QComboBox(pic);
	m_chroma = new QComboBox(pic);
	m_deint = new QCheckBox(tr("&Deinterlace"), pic);
	m_deintMode = new QComboBox(pic);
	m_deintMode->addItem(tr("frame"), 0);
	m_deintMode->addItem(tr("field (double rate)"), 1);
	m_deintField = new QComboBox(pic);
	m_deintField->addItem(tr("top field first"), 0);
	m_deintField->addItem(tr("bottom field first"), 1);
	m_deintField->addItem(tr("auto"), -1);
	auto *drow = new QHBoxLayout;
	drow->addWidget(m_deint);
	drow->addWidget(m_deintMode);
	drow->addWidget(m_deintField);
	m_filter = new QLineEdit(pic);
	m_filter->setPlaceholderText(tr("extra ffmpeg -vf filter"));
	m_lang = new QLineEdit(pic);
	pf->addRow(tr("Resolution:"), res);
	pf->addRow(tr("&Frame rate:"), m_fps);
	pf->addRow(tr("&Bit depth:"), m_bits);
	pf->addRow(tr("C&hroma:"), m_chroma);
	pf->addRow(drow);
	pf->addRow(tr("Filter:"), m_filter);
	pf->addRow(tr("&Language:"), m_lang);
	cols->addWidget(pic, 2);

	connect(m_encoder, &QComboBox::activated, this, [this] {
		const QString guid = m_encoder->currentData().toString();
		each([&](QueueItem &it) { switchPlugin(m_core, it, guid); });
		setItems(m_items);
	});
	connect(m_preset, &QComboBox::activated, this, [this] { setField(QStringLiteral("preset"), m_preset->currentData().toString(), true); });
	connect(m_tune, &QComboBox::activated, this, [this] { setField(QStringLiteral("tune"), m_tune->currentData().toString(), true); });
	connect(m_mode, &QComboBox::activated, this, &VideoTab::modeChanged);
	connect(m_value, &QDoubleSpinBox::valueChanged, this, [this](double v) { setField(QStringLiteral("value"), v, true); });
	connect(m_passes, &QSpinBox::valueChanged, this, [this](int v) { setField(QStringLiteral("passes"), v, true); });
	connect(m_args, &QLineEdit::textEdited, this, [this](const QString &s) { setField(QStringLiteral("encoder_args"), s, true); });
	connect(m_bits, &QComboBox::activated, this, [this] { setField(QStringLiteral("bit_depth"), m_bits->currentData().toInt(), true); });
	connect(m_chroma, &QComboBox::activated, this, [this] { setField(QStringLiteral("chroma"), m_chroma->currentData().toInt(), true); });
	connect(m_width, &QSpinBox::valueChanged, this, [this](int v) { setField(QStringLiteral("width"), v, false); });
	connect(m_height, &QSpinBox::valueChanged, this, [this](int v) { setField(QStringLiteral("height"), v, false); });
	connect(m_fps, &QComboBox::currentTextChanged, this, [this](const QString &text) {
		// A listed rate stands for its exact value ("23.976" -> "24000/1001");
		// anything typed is passed as written.
		const int i = m_fps->findText(text);
		const QString v = i >= 0 ? m_fps->itemData(i).toString() : text.trimmed();
		if (!v.isEmpty())
			setField(QStringLiteral("fps"), v, false);
	});
	connect(m_filter, &QLineEdit::textEdited, this, [this](const QString &s) { setField(QStringLiteral("filter"), s, false); });
	auto deint = [this] {
		m_deintMode->setEnabled(m_deint->isChecked());
		m_deintField->setEnabled(m_deint->isChecked());
		QJsonObject d{ { QStringLiteral("mode"), m_deintMode->currentData().toInt() },
		               { QStringLiteral("field"), m_deintField->currentData().toInt() } };
		each([&](QueueItem &it) {
			QJsonObject e = videoEntry(it);
			if (e.isEmpty())
				return;
			if (m_deint->isChecked())
				e[QStringLiteral("deinterlace")] = d;
			else
				e.remove(QStringLiteral("deinterlace"));
			setVideoEntry(it, e);
		});
	};
	connect(m_deint, &QCheckBox::toggled, this, deint);
	connect(m_deintMode, &QComboBox::activated, this, deint);
	connect(m_deintField, &QComboBox::activated, this, deint);
	connect(m_lang, &QLineEdit::textEdited, this, [this](const QString &s) {
		if (QueueItem *it = current()) {   // this item only
			QJsonObject e = videoEntry(*it);
			e[QStringLiteral("language")] = s.trimmed().isEmpty() ? QStringLiteral("und") : s.trimmed();
			setVideoEntry(*it, e);
			emit changed();
		}
	});
}

const app::Plugin *VideoTab::plugin() const
{
	return current() ? m_core.find(normPlugin(videoEntry(*current()).value(QStringLiteral("plugin")))) : nullptr;
}

void VideoTab::switchPlugin(app::Core &core, QueueItem &it, const QString &guid)
{
	if (guid == QLatin1String("none")) {
		it.job[QStringLiteral("video")] = QJsonArray();
		return;
	}
	const QJsonObject src = firstVideoStream(it);
	if (src.isEmpty())
		return;

	const QJsonObject old = videoEntry(it);
	QJsonObject e;
	e[QStringLiteral("stream")] = old.contains(QStringLiteral("stream")) ? old.value(QStringLiteral("stream")) : src.value(QStringLiteral("id"));
	e[QStringLiteral("language")] = old.contains(QStringLiteral("language")) ? old.value(QStringLiteral("language")) : src.value(QStringLiteral("language"));
	if (guid == QLatin1String("copy")) {
		e[QStringLiteral("plugin")] = QStringLiteral("copy");
		setVideoEntry(it, e);
		return;
	}

	const app::Plugin *p = core.find(guid);
	if (!p)
		return;
	const QJsonObject vc = p->section();
	const QJsonObject mode0 = jget(vc, QStringLiteral("Mode")).toArray().first().toObject();
	auto keep = [&](const char *key, const QJsonValue &fallback) {
		const QString k = QLatin1String(key);
		e[k] = old.contains(k) ? old.value(k) : fallback;
	};

	e[QStringLiteral("plugin")] = p->guid;
	e[QStringLiteral("preset")] = jstr(jget(vc, QStringLiteral("PresetDefault")));
	e[QStringLiteral("tune")] = jstr(jget(vc, QStringLiteral("TuneDefault")));
	e[QStringLiteral("mode")] = 0;
	e[QStringLiteral("value")] = jnum(jget(jget(mode0, QStringLiteral("Value")).toObject(), QStringLiteral("Default")));
	e[QStringLiteral("passes")] = jget(mode0, QStringLiteral("MultiPass")).toBool() ? 2 : 1;
	e[QStringLiteral("encoder_args")] = QString();
	keep("width", src.value(QStringLiteral("width")));
	keep("height", src.value(QStringLiteral("height")));
	keep("fps", old.value(QStringLiteral("fps")));
	if (!e.value(QStringLiteral("fps")).isString() || e.value(QStringLiteral("fps")).toString().isEmpty())
		e.remove(QStringLiteral("fps"));   // the core uses the source's
	e[QStringLiteral("bit_depth")] = supportedOr(jget(vc, QStringLiteral("Encoder")).toArray(), QStringLiteral("BitDepth"),
		src.value(QStringLiteral("bit_depth")).toInt(8), 8);
	e[QStringLiteral("chroma")] = supportedOr(jget(vc, QStringLiteral("Chroma")).toArray(), QStringLiteral("Value"),
		src.value(QStringLiteral("chroma")).toInt(420), 420);
	keep("filter", QString());
	if (old.contains(QStringLiteral("deinterlace")))
		e[QStringLiteral("deinterlace")] = old.value(QStringLiteral("deinterlace"));
	setVideoEntry(it, e);
}

void VideoTab::reload()
{
	QueueItem *it = current();
	m_streams->clear();
	if (!it)
		return;

	for (const QJsonValue &v : it->media.value(QStringLiteral("video")).toArray()) {
		const QJsonObject s = v.toObject();
		if (s.value(QStringLiteral("attached_pic")).toBool())
			continue;
		new QTreeWidgetItem(m_streams, {
			QString::number(s.value(QStringLiteral("id")).toInt()), s.value(QStringLiteral("codec")).toString(),
			QStringLiteral("%1x%2").arg(s.value(QStringLiteral("width")).toInt()).arg(s.value(QStringLiteral("height")).toInt()),
			QString::number(s.value(QStringLiteral("frame_rate_avg")).toDouble(), 'f', 3),
			QString::number(s.value(QStringLiteral("bit_depth")).toInt()),
			s.value(QStringLiteral("pix_fmt")).toString(), s.value(QStringLiteral("language")).toString() });
	}

	const QString container = it->container();
	const QJsonObject e = videoEntry(*it);
	if (!app::isVideoContainer(container) || firstVideoStream(*it).isEmpty() || e.isEmpty()) {
		m_notice->setText(!app::isVideoContainer(container)
			? tr("<b>.%1</b> is an audio format: the video is left out.").arg(container)
			: firstVideoStream(*it).isEmpty() ? tr("This file has no video.")
			                                  : tr("The video is left out."));
		m_notice->show();
		m_settings->setEnabled(false);
		return;
	}
	m_notice->setVisible(m_items.size() > 1);
	m_notice->setText(tr("%1 files selected: changes apply to all of them.").arg(m_items.size()));
	m_settings->setEnabled(true);

	QList<QPair<QString, QVariant>> encs;
	for (const app::Plugin *p : m_core.compatible(false, container))
		encs.append({ p->name, p->guid });
	encs.append({ tr("Copy (keep the stream as it is)"), QStringLiteral("copy") });
	const QString cur = normPlugin(e.value(QStringLiteral("plugin")));
	fill(m_encoder, encs, cur);

	const app::Plugin *p = m_core.find(cur);
	const QJsonObject vc = p ? p->section() : QJsonObject();
	for (QWidget *w : { static_cast<QWidget *>(m_preset), static_cast<QWidget *>(m_tune), static_cast<QWidget *>(m_mode),
	                    static_cast<QWidget *>(m_value), static_cast<QWidget *>(m_passes), static_cast<QWidget *>(m_args),
	                    static_cast<QWidget *>(m_width), static_cast<QWidget *>(m_height), static_cast<QWidget *>(m_fps),
	                    static_cast<QWidget *>(m_bits), static_cast<QWidget *>(m_chroma), static_cast<QWidget *>(m_deint),
	                    static_cast<QWidget *>(m_filter) })
		w->setEnabled(p != nullptr);

	auto listOf = [&](const char *key) {
		QList<QPair<QString, QVariant>> out;
		for (const QString &s : app::jstrings(jget(vc, QLatin1String(key))))
			out.append({ s, s });
		return out;
	};
	const auto presets = listOf("Preset"), tunes = listOf("Tune");
	fill(m_preset, presets, e.value(QStringLiteral("preset")).toString());
	fill(m_tune, tunes.isEmpty() ? QList<QPair<QString, QVariant>>{ { tr("(none)"), QString() } } : tunes,
	     e.value(QStringLiteral("tune")).toString());
	m_preset->setEnabled(p && !presets.isEmpty());
	m_tune->setEnabled(p && !tunes.isEmpty());

	QList<QPair<QString, QVariant>> modes;
	const QJsonArray ma = jget(vc, QStringLiteral("Mode")).toArray();
	for (int i = 0; i < ma.size(); i++)
		modes.append({ jstr(jget(ma[i].toObject(), QStringLiteral("Name"))), i });
	fill(m_mode, modes, e.value(QStringLiteral("mode")).toInt());

	const QJsonObject mode = ma.at(m_mode->currentIndex()).toObject();
	const QJsonObject mv = jget(mode, QStringLiteral("Value")).toObject();
	const int dp = qBound(0, static_cast<int>(jnum(jget(mv, QStringLiteral("DecimalPlace")))), 6);
	m_value->setDecimals(dp);
	m_value->setRange(jnum(jget(mv, QStringLiteral("Min"))), jnum(jget(mv, QStringLiteral("Max")), 1e9));
	m_value->setSingleStep(jnum(jget(mv, QStringLiteral("Step")), dp ? 0.1 : 1));
	m_value->setValue(e.value(QStringLiteral("value")).toDouble(jnum(jget(mv, QStringLiteral("Default")))));
	const bool multi = jget(mode, QStringLiteral("MultiPass")).toBool();
	m_passes->setEnabled(p && multi);
	m_passes->setValue(qMax(2, e.value(QStringLiteral("passes")).toInt(2)));
	m_args->setText(e.value(QStringLiteral("encoder_args")).toString());

	const QJsonObject src = firstVideoStream(*it);
	m_width->setValue(e.value(QStringLiteral("width")).toInt(src.value(QStringLiteral("width")).toInt()));
	m_height->setValue(e.value(QStringLiteral("height")).toInt(src.value(QStringLiteral("height")).toInt()));

	const QString fps = e.value(QStringLiteral("fps")).toString();
	fill(m_fps, { { tr("23.976 (24000/1001)"), QStringLiteral("24000/1001") }, { QStringLiteral("24"), QStringLiteral("24") },
	              { QStringLiteral("25"), QStringLiteral("25") }, { tr("29.97 (30000/1001)"), QStringLiteral("30000/1001") },
	              { QStringLiteral("30"), QStringLiteral("30") }, { QStringLiteral("50"), QStringLiteral("50") },
	              { tr("59.94 (60000/1001)"), QStringLiteral("60000/1001") }, { QStringLiteral("60"), QStringLiteral("60") } },
	     fps.isEmpty() ? QVariant(QString::number(src.value(QStringLiteral("frame_rate_avg")).toDouble(), 'g', 6)) : QVariant(fps));

	QList<QPair<QString, QVariant>> bits, chromas;
	for (const QJsonValue &v : jget(vc, QStringLiteral("Encoder")).toArray()) {
		int b = static_cast<int>(jnum(jget(v.toObject(), QStringLiteral("BitDepth"))));
		bits.append({ tr("%1-bit").arg(b), b });
	}
	for (const QJsonValue &v : jget(vc, QStringLiteral("Chroma")).toArray()) {
		int c = static_cast<int>(jnum(jget(v.toObject(), QStringLiteral("Value"))));
		chromas.append({ QStringLiteral("4:%1:%2").arg(c / 10 % 10).arg(c % 10), c });
	}
	fill(m_bits, bits, e.value(QStringLiteral("bit_depth")).toInt(8));
	fill(m_chroma, chromas, e.value(QStringLiteral("chroma")).toInt(420));

	const QJsonObject d = e.value(QStringLiteral("deinterlace")).toObject();
	m_deint->setChecked(!d.isEmpty());
	m_deintMode->setCurrentIndex(qMax(0, m_deintMode->findData(d.value(QStringLiteral("mode")).toInt(0))));
	m_deintField->setCurrentIndex(qMax(0, m_deintField->findData(d.value(QStringLiteral("field")).toInt(0))));
	m_deintMode->setEnabled(p && m_deint->isChecked());
	m_deintField->setEnabled(p && m_deint->isChecked());
	m_filter->setText(e.value(QStringLiteral("filter")).toString());
	m_lang->setText(e.value(QStringLiteral("language")).toString());
}

void VideoTab::setField(const QString &key, const QJsonValue &value, bool pluginSpecific)
{
	const QJsonValue shownPlugin = current() ? videoEntry(*current()).value(QStringLiteral("plugin")) : QJsonValue();
	each([&](QueueItem &it) {
		QJsonObject e = videoEntry(it);
		if (e.isEmpty() || normPlugin(e.value(QStringLiteral("plugin"))) == QLatin1String("copy"))
			return;
		if (pluginSpecific && normPlugin(e.value(QStringLiteral("plugin"))) != normPlugin(shownPlugin))
			return;
		e[key] = value;
		setVideoEntry(it, e);
	});
}

void VideoTab::modeChanged()
{
	const app::Plugin *p = plugin();
	if (!p)
		return;
	const int m = m_mode->currentData().toInt();
	const QJsonObject mode = jget(p->section(), QStringLiteral("Mode")).toArray().at(m).toObject();
	const double def = jnum(jget(jget(mode, QStringLiteral("Value")).toObject(), QStringLiteral("Default")));
	const bool multi = jget(mode, QStringLiteral("MultiPass")).toBool();
	const QJsonValue shownPlugin = videoEntry(*current()).value(QStringLiteral("plugin"));
	each([&](QueueItem &it) {
		QJsonObject e = videoEntry(it);
		if (normPlugin(e.value(QStringLiteral("plugin"))) != normPlugin(shownPlugin))
			return;
		e[QStringLiteral("mode")] = m;
		e[QStringLiteral("value")] = def;         // the new mode's range and default
		e[QStringLiteral("passes")] = multi ? 2 : 1;
		setVideoEntry(it, e);
	});
	setItems(m_items);
}

// ---- Audio --------------------------------------------------------------------

AudioTab::AudioTab(app::Core &core, QWidget *parent)
	: SettingsTab(core, parent)
{
	auto *top = new QVBoxLayout(this);
	m_tracks = makeTree({ tr("Id"), tr("Language"), tr("Title"), tr("Source"), tr("Encoder"), tr("File") }, this);
	auto *row = new QHBoxLayout;
	row->addWidget(m_tracks, 1);
	auto *btns = new QVBoxLayout;
	m_add = new QPushButton(tr("&Add file..."), this);
	m_remove = new QPushButton(tr("&Remove"), this);
	btns->addWidget(m_add);
	btns->addWidget(m_remove);
	btns->addStretch();
	row->addLayout(btns);
	top->addLayout(row, 1);

	m_settings = new QGroupBox(tr("Encoder (selected tracks; all tracks when several files are selected)"), this);
	auto *f = new QFormLayout(m_settings);
	m_encoder = new QComboBox(m_settings);
	m_mode = new QComboBox(m_settings);
	m_quality = new QComboBox(m_settings);
	m_quality->setEditable(true);
	m_rate = new QComboBox(m_settings);
	m_channels = new QComboBox(m_settings);
	m_copy = new QCheckBox(tr("Copy the track as it is when the format allows it (fast remux)"), m_settings);
	m_args = new QLineEdit(m_settings);
	m_args->setPlaceholderText(tr("extra encoder arguments"));
	m_filter = new QLineEdit(m_settings);
	m_filter->setPlaceholderText(tr("extra ffmpeg -af filter"));
	auto *r1 = new QHBoxLayout;
	r1->addWidget(m_encoder, 2);
	r1->addWidget(new QLabel(tr("Mode:"), m_settings));
	r1->addWidget(m_mode, 2);
	r1->addWidget(new QLabel(tr("Quality:"), m_settings));
	r1->addWidget(m_quality, 1);
	auto *r2 = new QHBoxLayout;
	r2->addWidget(m_rate, 1);
	r2->addWidget(new QLabel(tr("Channels:"), m_settings));
	r2->addWidget(m_channels, 1);
	f->addRow(tr("Encoder:"), r1);
	f->addRow(tr("Sample rate:"), r2);
	f->addRow(QString(), m_copy);
	f->addRow(tr("Encoder CLI:"), m_args);
	f->addRow(tr("Filter:"), m_filter);
	top->addWidget(m_settings);

	connect(m_tracks, &QTreeWidget::itemSelectionChanged, this, [this] {
		m_loading = true;
		loadSettings();
		m_loading = false;
	});
	connect(m_tracks, &QTreeWidget::itemChanged, this, &AudioTab::trackEdited);
	connect(m_add, &QPushButton::clicked, this, &AudioTab::addFile);
	connect(m_remove, &QPushButton::clicked, this, &AudioTab::removeSelected);
	connect(m_encoder, &QComboBox::activated, this, [this] { changePlugin(m_encoder->currentData().toString()); });
	connect(m_mode, &QComboBox::activated, this, [this] {
		const app::Plugin *p = m_core.find(normPlugin(shown().value(QStringLiteral("plugin"))));
		if (!p)
			return;
		const int m = m_mode->currentData().toInt();
		const QJsonObject mode = jget(p->section(), QStringLiteral("Mode")).toArray().at(m).toObject();
		setField(QStringLiteral("mode"), m, true);
		setField(QStringLiteral("quality"), jstr(jget(mode, QStringLiteral("Default"))), true);
		setItems(m_items);
	});
	connect(m_quality, &QComboBox::currentTextChanged, this, [this](const QString &s) {
		if (!s.trimmed().isEmpty())
			setField(QStringLiteral("quality"), s.trimmed(), true);
	});
	connect(m_rate, &QComboBox::activated, this, [this] { setField(QStringLiteral("sample_rate"), m_rate->currentData().toInt(), true); });
	connect(m_channels, &QComboBox::activated, this, [this] { setField(QStringLiteral("channels"), m_channels->currentData().toInt(), true); });
	connect(m_copy, &QCheckBox::toggled, this, [this](bool on) { setField(QStringLiteral("copy"), on, false); });
	connect(m_args, &QLineEdit::textEdited, this, [this](const QString &s) { setField(QStringLiteral("encoder_args"), s, true); });
	connect(m_filter, &QLineEdit::textEdited, this, [this](const QString &s) { setField(QStringLiteral("filter"), s, false); });
}

QJsonObject AudioTab::entryFor(app::Core &core, const QJsonObject &s, const QString &input, const QString &plugin)
{
	QJsonObject e;
	e[QStringLiteral("stream")] = s.value(QStringLiteral("id"));
	e[QStringLiteral("language")] = s.value(QStringLiteral("language")).toString(QStringLiteral("und"));
	e[QStringLiteral("title")] = s.value(QStringLiteral("title")).toString();
	if (!input.isEmpty())
		e[QStringLiteral("input")] = input;
	e[QStringLiteral("_help")] = QJsonObject{ { QStringLiteral("source"), QStringLiteral("%1, %2 Hz, %3 channels")
		.arg(s.value(QStringLiteral("codec")).toString()).arg(s.value(QStringLiteral("sample_rate")).toInt())
		.arg(s.value(QStringLiteral("channels")).toInt()) } };
	switchPlugin(core, e, plugin);
	return e;
}

void AudioTab::switchPlugin(app::Core &core, QJsonObject &e, const QString &guid)
{
	for (const char *k : { "mode", "quality", "sample_rate", "channels", "encoder_args", "ffmpeg_args" })
		e.remove(QLatin1String(k));
	if (guid == QLatin1String("copy")) {
		e[QStringLiteral("plugin")] = QStringLiteral("copy");
		return;
	}
	const app::Plugin *p = core.find(guid);
	if (!p)
		return;
	const QJsonObject ac = p->section();
	const QJsonObject mode0 = jget(ac, QStringLiteral("Mode")).toArray().first().toObject();
	e[QStringLiteral("plugin")] = p->guid;
	e[QStringLiteral("mode")] = 0;
	e[QStringLiteral("quality")] = jstr(jget(mode0, QStringLiteral("Default")));
	e[QStringLiteral("sample_rate")] = static_cast<int>(jnum(jget(ac, QStringLiteral("SampleRateDefault"))));
	e[QStringLiteral("channels")] = static_cast<int>(jnum(jget(ac, QStringLiteral("ChannelDefault"))));
	if (!e.contains(QStringLiteral("copy")))
		e[QStringLiteral("copy")] = false;
	e[QStringLiteral("encoder_args")] = QString();
}

void AudioTab::reload()
{
	m_tracks->clear();
	QueueItem *it = current();
	if (!it)
		return;

	const QJsonArray audio = it->job.value(QStringLiteral("audio")).toArray();
	for (int i = 0; i < audio.size(); i++) {
		const QJsonObject e = audio[i].toObject();
		const QString plug = normPlugin(e.value(QStringLiteral("plugin")));
		const app::Plugin *p = m_core.find(plug);
		const QString input = e.value(QStringLiteral("input")).toString();
		auto *row = new QTreeWidgetItem(m_tracks, {
			QString::number(e.value(QStringLiteral("stream")).toInt()), e.value(QStringLiteral("language")).toString(),
			e.value(QStringLiteral("title")).toString(),
			jget(e.value(QStringLiteral("_help")).toObject(), QStringLiteral("source")).toString(),
			plug == QLatin1String("copy") ? tr("Copy") : p ? p->name : tr("(unavailable: %1)").arg(plug),
			input.isEmpty() ? tr("(this file)") : QFileInfo(input).fileName() });
		row->setFlags(row->flags() | Qt::ItemIsUserCheckable | Qt::ItemIsEditable);
		row->setCheckState(0, e.value(QStringLiteral("enable")).toBool(true) ? Qt::Checked : Qt::Unchecked);
		row->setData(0, Qt::UserRole, i);
	}
	m_remove->setEnabled(!audio.isEmpty());
	loadSettings();
}

QVector<int> AudioTab::targets() const
{
	QVector<int> out;
	for (QTreeWidgetItem *row : m_tracks->selectedItems())
		out << row->data(0, Qt::UserRole).toInt();
	if (out.isEmpty())
		for (int i = 0; i < m_tracks->topLevelItemCount(); i++)
			out << i;
	return out;
}

QJsonObject AudioTab::shown() const
{
	if (!current())
		return {};
	const QJsonArray audio = current()->job.value(QStringLiteral("audio")).toArray();
	const QVector<int> t = targets();
	return t.isEmpty() || t.first() >= audio.size() ? QJsonObject() : audio[t.first()].toObject();
}

void AudioTab::loadSettings()
{
	const QJsonObject e = shown();
	m_settings->setEnabled(!e.isEmpty());
	if (e.isEmpty() || !current())
		return;

	QList<QPair<QString, QVariant>> encs;
	for (const app::Plugin *p : m_core.compatible(true, current()->container()))
		encs.append({ p->name, p->guid });
	encs.append({ tr("Copy (keep the track as it is)"), QStringLiteral("copy") });
	const QString cur = normPlugin(e.value(QStringLiteral("plugin")));
	fill(m_encoder, encs, cur);

	const app::Plugin *p = m_core.find(cur);
	const QJsonObject ac = p ? p->section() : QJsonObject();
	for (QWidget *w : { static_cast<QWidget *>(m_mode), static_cast<QWidget *>(m_quality), static_cast<QWidget *>(m_rate),
	                    static_cast<QWidget *>(m_channels), static_cast<QWidget *>(m_args), static_cast<QWidget *>(m_copy),
	                    static_cast<QWidget *>(m_filter) })
		w->setEnabled(p != nullptr);

	const QJsonArray modes = jget(ac, QStringLiteral("Mode")).toArray();
	QList<QPair<QString, QVariant>> ml;
	for (int i = 0; i < modes.size(); i++)
		ml.append({ jstr(jget(modes[i].toObject(), QStringLiteral("Name"))), i });
	fill(m_mode, ml, e.value(QStringLiteral("mode")).toInt());

	QList<QPair<QString, QVariant>> ql;
	for (const QString &q : app::jstrings(jget(modes.at(m_mode->currentIndex()).toObject(), QStringLiteral("Quality"))))
		ql.append({ q, q });
	fill(m_quality, ql, jstr(e.value(QStringLiteral("quality"))));

	QList<QPair<QString, QVariant>> rates{ { tr("Auto (keep)"), 0 } };
	for (const QString &r : app::jstrings(jget(ac, QStringLiteral("SampleRate"))))
		if (r.toInt() > 0)
			rates.append({ tr("%1 Hz").arg(r), r.toInt() });
	fill(m_rate, rates, e.value(QStringLiteral("sample_rate")).toInt());

	QList<QPair<QString, QVariant>> chans;
	const QJsonObject ch = jget(ac, QStringLiteral("Channels")).toObject();
	for (auto i = ch.constBegin(); i != ch.constEnd(); ++i)
		chans.append({ jstr(*i), i.key().toInt() });
	std::sort(chans.begin(), chans.end(), [](const auto &a, const auto &b) { return a.second.toInt() < b.second.toInt(); });
	if (chans.isEmpty())
		chans.append({ tr("Auto"), 0 });
	fill(m_channels, chans, e.value(QStringLiteral("channels")).toInt());

	m_copy->setChecked(e.value(QStringLiteral("copy")).toBool());
	m_args->setText(e.value(QStringLiteral("encoder_args")).toString());
	m_filter->setText(e.value(QStringLiteral("filter")).toString());
}

void AudioTab::setField(const QString &key, const QJsonValue &value, bool pluginSpecific)
{
	if (m_loading)
		return;
	const QString shownPlugin = normPlugin(shown().value(QStringLiteral("plugin")));
	const QVector<int> sel = targets();
	const bool many = m_items.size() > 1;
	each([&](QueueItem &it) {
		QJsonArray audio = it.job.value(QStringLiteral("audio")).toArray();
		for (int i = 0; i < audio.size(); i++) {
			if (!many && !sel.contains(i))
				continue;
			QJsonObject e = audio[i].toObject();
			const QString plug = normPlugin(e.value(QStringLiteral("plugin")));
			if (plug == QLatin1String("copy") || (pluginSpecific && plug != shownPlugin))
				continue;
			e[key] = value;
			audio[i] = e;
		}
		it.job[QStringLiteral("audio")] = audio;
	});
}

void AudioTab::changePlugin(const QString &guid)
{
	if (m_loading)
		return;
	const QVector<int> sel = targets();
	const bool many = m_items.size() > 1;
	each([&](QueueItem &it) {
		QJsonArray audio = it.job.value(QStringLiteral("audio")).toArray();
		for (int i = 0; i < audio.size(); i++) {
			if (!many && !sel.contains(i))
				continue;
			QJsonObject e = audio[i].toObject();
			switchPlugin(m_core, e, guid);
			audio[i] = e;
		}
		it.job[QStringLiteral("audio")] = audio;
	});
	setItems(m_items);
}

void AudioTab::addFile()
{
	QueueItem *it = current();
	if (!it)
		return;
	const QString file = QFileDialog::getOpenFileName(this, tr("Add audio from"), QFileInfo(it->input).path());
	if (file.isEmpty())
		return;
	QString err;
	const QJsonObject media = m_core.probe(file, &err);
	const QJsonArray streams = media.value(QStringLiteral("audio")).toArray();
	if (streams.isEmpty()) {
		QMessageBox::warning(this, tr("Add audio"), err.isEmpty() ? tr("%1 has no audio.").arg(file) : err);
		return;
	}
	const QString plugin = m_core.defaultAudio(it->container(), m_encoder->currentData().toString());
	QJsonArray audio = it->job.value(QStringLiteral("audio")).toArray();
	for (const QJsonValue &s : streams)
		audio.append(entryFor(m_core, s.toObject(), file, plugin));
	it->job[QStringLiteral("audio")] = audio;
	emit changed();
	setItems(m_items);
}

void AudioTab::removeSelected()
{
	QueueItem *it = current();
	if (!it)
		return;
	QVector<int> rows;
	for (QTreeWidgetItem *r : m_tracks->selectedItems())
		rows << r->data(0, Qt::UserRole).toInt();
	std::sort(rows.begin(), rows.end(), std::greater<int>());
	QJsonArray audio = it->job.value(QStringLiteral("audio")).toArray();
	for (int r : rows)
		audio.removeAt(r);
	it->job[QStringLiteral("audio")] = audio;
	emit changed();
	setItems(m_items);
}

void AudioTab::trackEdited(QTreeWidgetItem *row, int column)
{
	QueueItem *it = current();
	if (m_loading || !it)
		return;
	const int i = row->data(0, Qt::UserRole).toInt();
	QJsonArray audio = it->job.value(QStringLiteral("audio")).toArray();
	QJsonObject e = audio.at(i).toObject();
	if (column == 0)
		e[QStringLiteral("enable")] = row->checkState(0) == Qt::Checked;
	else if (column == 1)
		e[QStringLiteral("language")] = row->text(1).trimmed().isEmpty() ? QStringLiteral("und") : row->text(1).trimmed();
	else if (column == 2)
		e[QStringLiteral("title")] = row->text(2);
	audio[i] = e;
	it->job[QStringLiteral("audio")] = audio;
	emit changed();
}

// ---- Subtitle -------------------------------------------------------------------

SubtitleTab::SubtitleTab(app::Core &core, QWidget *parent)
	: SettingsTab(core, parent)
{
	auto *top = new QVBoxLayout(this);
	m_notice = new QLabel(this);
	m_notice->setWordWrap(true);
	top->addWidget(m_notice);
	m_tracks = makeTree({ tr("Id"), tr("Language"), tr("Title"), tr("Default"), tr("Format"), tr("File") }, this);
	auto *row = new QHBoxLayout;
	row->addWidget(m_tracks, 1);
	auto *btns = new QVBoxLayout;
	auto *add = new QPushButton(tr("&Add file..."), this);
	auto *remove = new QPushButton(tr("&Remove"), this);
	btns->addWidget(add);
	btns->addWidget(remove);
	btns->addStretch();
	row->addLayout(btns);
	top->addLayout(row, 1);
	m_burn = new QCheckBox(tr("&Burn subtitle into the video (the first ticked one; text formats only)"), this);
	top->addWidget(m_burn);

	connect(add, &QPushButton::clicked, this, &SubtitleTab::addFile);
	connect(remove, &QPushButton::clicked, this, &SubtitleTab::removeSelected);
	connect(m_tracks, &QTreeWidget::itemChanged, this, &SubtitleTab::trackEdited);
	connect(m_burn, &QCheckBox::toggled, this, [this](bool on) {
		each([&](QueueItem &it) { it.job[QStringLiteral("burn_subtitle")] = on; });
	});
}

void SubtitleTab::reload()
{
	m_tracks->clear();
	QueueItem *it = current();
	if (!it)
		return;
	const QString c = it->container();
	const bool soft = c == QLatin1String("mkv") || c == QLatin1String("mp4") || c == QLatin1String("webm") ||
	                  c == QLatin1String("m4v") || c == QLatin1String("mov");
	m_notice->setVisible(!soft);
	m_notice->setText(tr("<b>.%1</b> cannot hold subtitles: tick <i>Burn subtitle</i> to keep one in the picture.").arg(c));

	const QJsonArray subs = it->job.value(QStringLiteral("subtitles")).toArray();
	for (int i = 0; i < subs.size(); i++) {
		const QJsonObject e = subs[i].toObject();
		const QString input = e.value(QStringLiteral("input")).toString();
		auto *row = new QTreeWidgetItem(m_tracks, {
			QString::number(e.value(QStringLiteral("stream")).toInt()), e.value(QStringLiteral("language")).toString(),
			e.value(QStringLiteral("title")).toString(), QString(),
			jget(e.value(QStringLiteral("_help")).toObject(), QStringLiteral("format")).toString(),
			input.isEmpty() ? tr("(this file)") : QFileInfo(input).fileName() });
		row->setFlags(row->flags() | Qt::ItemIsUserCheckable | Qt::ItemIsEditable);
		row->setCheckState(0, e.value(QStringLiteral("enable")).toBool(true) ? Qt::Checked : Qt::Unchecked);
		row->setCheckState(3, e.value(QStringLiteral("default")).toBool() ? Qt::Checked : Qt::Unchecked);
		row->setData(0, Qt::UserRole, i);
	}
	m_burn->setChecked(it->job.value(QStringLiteral("burn_subtitle")).toBool());
}

void SubtitleTab::trackEdited(QTreeWidgetItem *row, int column)
{
	QueueItem *it = current();
	if (m_loading || !it)
		return;
	const int i = row->data(0, Qt::UserRole).toInt();
	QJsonArray subs = it->job.value(QStringLiteral("subtitles")).toArray();
	QJsonObject e = subs.at(i).toObject();
	if (column == 0)
		e[QStringLiteral("enable")] = row->checkState(0) == Qt::Checked;
	else if (column == 1)
		e[QStringLiteral("language")] = row->text(1).trimmed().isEmpty() ? QStringLiteral("und") : row->text(1).trimmed();
	else if (column == 2)
		e[QStringLiteral("title")] = row->text(2);
	else if (column == 3)
		e[QStringLiteral("default")] = row->checkState(3) == Qt::Checked;
	subs[i] = e;
	it->job[QStringLiteral("subtitles")] = subs;
	emit changed();
}

void SubtitleTab::addFile()
{
	QueueItem *it = current();
	if (!it)
		return;
	const QStringList files = QFileDialog::getOpenFileNames(this, tr("Add subtitles"), QFileInfo(it->input).path(),
		tr("Subtitles (*.srt *.ass *.ssa *.vtt *.sup *.mks *.mkv *.mp4);;All files (*)"));
	QJsonArray subs = it->job.value(QStringLiteral("subtitles")).toArray();
	for (const QString &file : files) {
		QString err;
		const QJsonArray streams = m_core.probe(file, &err).value(QStringLiteral("subtitle")).toArray();
		if (streams.isEmpty()) {
			QMessageBox::warning(this, tr("Add subtitles"), err.isEmpty() ? tr("%1 has no subtitles.").arg(file) : err);
			continue;
		}
		for (const QJsonValue &v : streams) {
			const QJsonObject s = v.toObject();
			subs.append(QJsonObject{
				{ QStringLiteral("input"), file }, { QStringLiteral("stream"), s.value(QStringLiteral("id")) },
				{ QStringLiteral("language"), s.value(QStringLiteral("language")).toString(QStringLiteral("und")) },
				{ QStringLiteral("title"), s.value(QStringLiteral("title")).toString() },
				{ QStringLiteral("default"), subs.isEmpty() },
				{ QStringLiteral("_help"), QJsonObject{ { QStringLiteral("format"), s.value(QStringLiteral("codec")) } } } });
		}
	}
	it->job[QStringLiteral("subtitles")] = subs;
	emit changed();
	setItems(m_items);
}

void SubtitleTab::removeSelected()
{
	QueueItem *it = current();
	if (!it)
		return;
	QVector<int> rows;
	for (QTreeWidgetItem *r : m_tracks->selectedItems())
		rows << r->data(0, Qt::UserRole).toInt();
	std::sort(rows.begin(), rows.end(), std::greater<int>());
	QJsonArray subs = it->job.value(QStringLiteral("subtitles")).toArray();
	for (int r : rows)
		subs.removeAt(r);
	it->job[QStringLiteral("subtitles")] = subs;
	emit changed();
	setItems(m_items);
}

// ---- Attachment -------------------------------------------------------------------

AttachmentTab::AttachmentTab(app::Core &core, QWidget *parent)
	: SettingsTab(core, parent)
{
	auto *top = new QVBoxLayout(this);
	m_notice = new QLabel(this);
	m_notice->setWordWrap(true);
	top->addWidget(m_notice);
	m_list = makeTree({ tr("File name"), tr("MIME"), tr("Source") }, this);
	auto *row = new QHBoxLayout;
	row->addWidget(m_list, 1);
	auto *btns = new QVBoxLayout;
	auto *add = new QPushButton(tr("&Add files..."), this);
	auto *remove = new QPushButton(tr("&Remove"), this);
	btns->addWidget(add);
	btns->addWidget(remove);
	btns->addStretch();
	row->addLayout(btns);
	top->addLayout(row, 1);

	connect(add, &QPushButton::clicked, this, &AttachmentTab::addFiles);
	connect(remove, &QPushButton::clicked, this, &AttachmentTab::removeSelected);
	connect(m_list, &QTreeWidget::itemChanged, this, [this](QTreeWidgetItem *r, int column) {
		QueueItem *it = current();
		if (m_loading || !it || column != 0)
			return;
		QJsonArray a = it->job.value(QStringLiteral("attachments")).toArray();
		const int i = r->data(0, Qt::UserRole).toInt();
		QJsonObject e = a.at(i).toObject();
		e[QStringLiteral("enable")] = r->checkState(0) == Qt::Checked;
		a[i] = e;
		it->job[QStringLiteral("attachments")] = a;
		emit changed();
	});
}

void AttachmentTab::reload()
{
	m_list->clear();
	QueueItem *it = current();
	if (!it)
		return;
	const QString c = it->container();
	m_notice->setVisible(c != QLatin1String("mkv"));
	m_notice->setText(tr("Only MKV keeps attachments; for <b>.%1</b> they are only used to burn ASS subtitles.").arg(c));

	const QJsonArray a = it->job.value(QStringLiteral("attachments")).toArray();
	for (int i = 0; i < a.size(); i++) {
		const QJsonObject e = a[i].toObject();
		const QString file = e.value(QStringLiteral("file")).toString();
		auto *row = new QTreeWidgetItem(m_list, {
			file.isEmpty() ? e.value(QStringLiteral("filename")).toString() : QFileInfo(file).fileName(),
			e.value(QStringLiteral("mime_type")).toString(file.isEmpty() ? QString() : tr("(from the file name)")),
			file.isEmpty() ? tr("(this file)") : file });
		row->setFlags(row->flags() | Qt::ItemIsUserCheckable);
		row->setCheckState(0, e.value(QStringLiteral("enable")).toBool(true) ? Qt::Checked : Qt::Unchecked);
		row->setData(0, Qt::UserRole, i);
	}
}

void AttachmentTab::addFiles()
{
	QueueItem *it = current();
	if (!it)
		return;
	const QStringList files = QFileDialog::getOpenFileNames(this, tr("Attach files"), QString(),
		tr("Fonts (*.ttf *.otf *.ttc *.woff *.woff2);;All files (*)"));
	QJsonArray a = it->job.value(QStringLiteral("attachments")).toArray();
	for (const QString &f : files)
		a.append(QJsonObject{ { QStringLiteral("file"), f } });
	it->job[QStringLiteral("attachments")] = a;
	emit changed();
	setItems(m_items);
}

void AttachmentTab::removeSelected()
{
	QueueItem *it = current();
	if (!it)
		return;
	QVector<int> rows;
	for (QTreeWidgetItem *r : m_list->selectedItems())
		rows << r->data(0, Qt::UserRole).toInt();
	std::sort(rows.begin(), rows.end(), std::greater<int>());
	QJsonArray a = it->job.value(QStringLiteral("attachments")).toArray();
	for (int r : rows)
		a.removeAt(r);
	it->job[QStringLiteral("attachments")] = a;
	emit changed();
	setItems(m_items);
}

// ---- Advanced ------------------------------------------------------------------------

AdvancedTab::AdvancedTab(app::Core &core, QWidget *parent)
	: SettingsTab(core, parent)
{
	auto *top = new QVBoxLayout(this);

	auto *trim = new QGroupBox(tr("Trim"), this);
	auto *tf = new QFormLayout(trim);
	m_trim = new QCheckBox(tr("&Trim video && audio"), trim);
	m_start = new QLineEdit(QStringLiteral("00:00:00.000"), trim);
	m_duration = new QLineEdit(QStringLiteral("00:01:00.000"), trim);
	m_end = new QLabel(trim);
	tf->addRow(m_trim);
	tf->addRow(tr("Start:"), m_start);
	tf->addRow(tr("Duration:"), m_duration);
	tf->addRow(tr("End:"), m_end);
	top->addWidget(trim);

	auto *extra = new QGroupBox(tr("Cover art, tags and chapters"), this);
	auto *xf = new QFormLayout(extra);
	m_cover = new QComboBox(extra);
	m_cover->addItem(tr("Keep the file's cover art"));
	m_cover->addItem(tr("No cover art"));
	m_cover->addItem(tr("Use an image..."));
	m_coverFile = new QLabel(extra);
	m_metadata = new QCheckBox(tr("Keep the file's tags (title, artist...)"), extra);
	m_chapters = new QCheckBox(tr("Keep chapters"), extra);
	m_movflags = new QLineEdit(extra);
	m_movflags->setPlaceholderText(tr("MP4 only, e.g. +faststart or +frag_keyframe+empty_moov"));
	xf->addRow(tr("Cover:"), m_cover);
	xf->addRow(QString(), m_coverFile);
	xf->addRow(m_metadata);
	xf->addRow(m_chapters);
	xf->addRow(tr("MP4 flags:"), m_movflags);
	top->addWidget(extra);
	top->addStretch();

	connect(m_trim, &QCheckBox::toggled, this, &AdvancedTab::applyTrim);
	connect(m_start, &QLineEdit::textEdited, this, &AdvancedTab::applyTrim);
	connect(m_duration, &QLineEdit::textEdited, this, &AdvancedTab::applyTrim);
	connect(m_cover, &QComboBox::activated, this, &AdvancedTab::applyCover);
	connect(m_metadata, &QCheckBox::toggled, this, [this](bool on) {
		each([&](QueueItem &it) { it.job[QStringLiteral("metadata")] = on; });
	});
	connect(m_chapters, &QCheckBox::toggled, this, [this](bool on) {
		each([&](QueueItem &it) { it.job[QStringLiteral("chapters")] = on; });
	});
	connect(m_movflags, &QLineEdit::textEdited, this, [this](const QString &s) {
		each([&](QueueItem &it) { it.job[QStringLiteral("movflags")] = s.trimmed(); });
	});
}

void AdvancedTab::reload()
{
	QueueItem *it = current();
	if (!it)
		return;
	const QJsonObject trim = it->job.value(QStringLiteral("trim")).toObject();
	m_trim->setChecked(!trim.isEmpty());
	if (!trim.isEmpty()) {
		m_start->setText(trim.value(QStringLiteral("start")).toString());
		m_duration->setText(trim.value(QStringLiteral("duration")).toString());
	}
	m_start->setEnabled(m_trim->isChecked());
	m_duration->setEnabled(m_trim->isChecked());
	const double s = app::parseTime(m_start->text()), d = app::parseTime(m_duration->text());
	m_end->setText(s >= 0 && d > 0 ? app::formatDuration(s + d) : tr("(invalid)"));

	const QJsonObject cover = it->job.value(QStringLiteral("cover")).toObject();
	const QString file = cover.value(QStringLiteral("input")).toString();
	m_cover->setCurrentIndex(cover.isEmpty() ? 1 : file.isEmpty() ? 0 : 2);
	m_coverFile->setText(file);
	m_metadata->setChecked(it->job.value(QStringLiteral("metadata")).toBool(true));
	m_chapters->setChecked(it->job.value(QStringLiteral("chapters")).toBool(true));
	m_movflags->setText(it->job.value(QStringLiteral("movflags")).toString());
}

void AdvancedTab::applyTrim()
{
	const double s = app::parseTime(m_start->text()), d = app::parseTime(m_duration->text());
	const bool valid = s >= 0 && d > 0;
	m_start->setEnabled(m_trim->isChecked());
	m_duration->setEnabled(m_trim->isChecked());
	m_start->setStyleSheet(s >= 0 ? QString() : QStringLiteral("background: #fdd;"));
	m_duration->setStyleSheet(d > 0 ? QString() : QStringLiteral("background: #fdd;"));
	m_end->setText(valid ? app::formatDuration(s + d) : tr("(invalid)"));
	if (m_trim->isChecked() && !valid)
		return;
	each([&](QueueItem &it) {
		if (m_trim->isChecked())
			it.job[QStringLiteral("trim")] = QJsonObject{ { QStringLiteral("start"), m_start->text().trimmed() },
			                                              { QStringLiteral("duration"), m_duration->text().trimmed() } };
		else
			it.job.remove(QStringLiteral("trim"));
	});
}

void AdvancedTab::applyCover(int index)
{
	QString file;
	if (index == 2) {
		file = QFileDialog::getOpenFileName(this, tr("Cover art"), QString(),
			tr("Images (*.jpg *.jpeg *.png *.webp *.bmp);;All files (*)"));
		if (file.isEmpty()) {
			setItems(m_items);
			return;
		}
	}
	each([&](QueueItem &it) {
		it.job.remove(QStringLiteral("cover"));
		if (index == 2) {
			it.job[QStringLiteral("cover")] = QJsonObject{ { QStringLiteral("input"), file } };
		} else if (index == 0) {
			for (const QJsonValue &v : it.media.value(QStringLiteral("video")).toArray())
				if (v.toObject().value(QStringLiteral("attached_pic")).toBool()) {
					it.job[QStringLiteral("cover")] = QJsonObject{ { QStringLiteral("stream"), v.toObject().value(QStringLiteral("id")) } };
					break;
				}
		}
	});
	setItems(m_items);
}
