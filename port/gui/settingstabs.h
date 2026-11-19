// settingstabs.h - the Video / Audio / Subtitle / Attachment / Advanced tabs.
//
// Each tab shows the first selected queue item. Encoder-wide settings apply to
// every selected item; plugin-specific ones (preset, mode, value, quality...)
// only to items that use the same plugin; track edits (language, title, on/off)
// to the item shown.
#pragma once

#include <QJsonArray>
#include <QVector>
#include <QWidget>
#include <functional>

#include "queueitem.h"

class QCheckBox;
class QComboBox;
class QDoubleSpinBox;
class QLabel;
class QLineEdit;
class LanguageCombo;
class QPushButton;
class QSpinBox;
class QTreeWidget;
class QTreeWidgetItem;
namespace app { class Core; struct Plugin; }

class SettingsTab : public QWidget
{
	Q_OBJECT

public:
	SettingsTab(app::Core &core, QWidget *parent = nullptr) : QWidget(parent), m_core(core) {}
	void setItems(const QVector<QueueItem *> &items);

signals:
	void changed();

protected:
	app::Core &m_core;
	QVector<QueueItem *> m_items;
	bool m_loading = false;

	QueueItem *current() const { return m_items.isEmpty() ? nullptr : m_items.first(); }
	virtual void reload() = 0;
	void each(const std::function<void(QueueItem &)> &fn);   // edit all, then changed()
};

// ---- Video ------------------------------------------------------------------

class VideoTab : public SettingsTab
{
	Q_OBJECT

public:
	explicit VideoTab(app::Core &core, QWidget *parent = nullptr);
	// Give `item` a video entry for `plugin` ("copy" or a GUID), keeping its
	// size, frame rate and filters (C# kept them when the encoder changed).
	static void switchPlugin(app::Core &core, QueueItem &item, const QString &plugin);

protected:
	void reload() override;

private:
	QLabel *m_notice;
	QTreeWidget *m_streams;
	QComboBox *m_encoder, *m_preset, *m_tune, *m_mode, *m_fps, *m_bits, *m_chroma;
	QComboBox *m_deintMode, *m_deintField;
	QDoubleSpinBox *m_value;
	QSpinBox *m_passes, *m_width, *m_height;
	QCheckBox *m_deint;
	QLineEdit *m_args, *m_filter;
	LanguageCombo *m_lang;
	QWidget *m_settings;

	const app::Plugin *plugin() const;
	void setField(const QString &key, const QJsonValue &value, bool pluginSpecific);
	void modeChanged();
};

// ---- Audio --------------------------------------------------------------------

class AudioTab : public SettingsTab
{
	Q_OBJECT

public:
	explicit AudioTab(app::Core &core, QWidget *parent = nullptr);
	// Reset an audio entry to `plugin`'s defaults ("copy" or a GUID).
	static void switchPlugin(app::Core &core, QJsonObject &entry, const QString &plugin);
	static QJsonObject entryFor(app::Core &core, const QJsonObject &stream, const QString &input,
	                            const QString &plugin);

protected:
	void reload() override;

private:
	QTreeWidget *m_tracks;
	QComboBox *m_encoder, *m_mode, *m_quality, *m_rate, *m_channels;
	QCheckBox *m_copy;
	QLineEdit *m_args, *m_filter;
	QPushButton *m_add, *m_remove;
	QWidget *m_settings;

	QVector<int> targets() const;          // audio entries of current() the settings apply to
	QJsonObject shown() const;             // the entry whose settings are shown
	void loadSettings();
	void setField(const QString &key, const QJsonValue &value, bool pluginSpecific);
	void changePlugin(const QString &guid);
	void addFile();
	void removeSelected();
	void trackEdited(QTreeWidgetItem *it, int column);
};

// ---- Subtitle -------------------------------------------------------------------

class SubtitleTab : public SettingsTab
{
	Q_OBJECT

public:
	explicit SubtitleTab(app::Core &core, QWidget *parent = nullptr);

protected:
	void reload() override;

private:
	QTreeWidget *m_tracks;
	QCheckBox *m_burn;
	QLabel *m_notice;
	void addFile();
	void removeSelected();
	void trackEdited(QTreeWidgetItem *it, int column);
};

// ---- Attachment -------------------------------------------------------------------

class AttachmentTab : public SettingsTab
{
	Q_OBJECT

public:
	explicit AttachmentTab(app::Core &core, QWidget *parent = nullptr);

protected:
	void reload() override;

private:
	QTreeWidget *m_list;
	QLabel *m_notice;
	void addFiles();
	void removeSelected();
};

// ---- Advanced ------------------------------------------------------------------------

class AdvancedTab : public SettingsTab
{
	Q_OBJECT

public:
	explicit AdvancedTab(app::Core &core, QWidget *parent = nullptr);

protected:
	void reload() override;

private:
	QCheckBox *m_trim, *m_metadata, *m_chapters;
	QLineEdit *m_start, *m_duration, *m_movflags;
	QLabel *m_end;
	QComboBox *m_cover;
	QLabel *m_coverFile;
	void applyTrim();
	void applyCover(int index);
};
