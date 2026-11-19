// dialogs.h - Preferences and Plugins.
#pragma once

#include <QDialog>

#include "app.h"

class QCheckBox;
class QComboBox;
class QRadioButton;
class QLabel;
class QLineEdit;
class QPushButton;
class QTreeWidget;
namespace app { class Core; }

class PreferencesDialog : public QDialog
{
	Q_OBJECT

public:
	explicit PreferencesDialog(QWidget *parent = nullptr);
	bool rootChanged() const { return m_rootChanged; }
	bool languageChanged() const { return m_languageChanged; }

	void accept() override;

private:
	QLineEdit *m_root, *m_output, *m_temp;
	QCheckBox *m_keepTemp, *m_test;
	QComboBox *m_language;
	// File name prefix / suffix (C#: rdoPrefix*, rdoPostfix*, txtPrefix...).
	QRadioButton *m_prefix[3], *m_suffix[3];
	QLineEdit *m_prefixText, *m_suffixText;
	QLabel *m_namePreview;
	QComboBox *m_existing;
	bool m_rootChanged = false, m_languageChanged = false;

	app::NameStyle nameStyle() const;
	void updatePreview();
};

class PluginsDialog : public QDialog
{
	Q_OBJECT

public:
	PluginsDialog(app::Core &core, bool canTest, QWidget *parent = nullptr);
	bool changed() const { return m_changed; }

private:
	app::Core &m_core;
	QTreeWidget *m_list;
	QPushButton *m_testSome, *m_testAll;
	QLabel *m_status;
	bool m_loading = false, m_changed = false, m_testing = false;

	void reload();
	void runTest(bool all);
	void reject() override;
};
