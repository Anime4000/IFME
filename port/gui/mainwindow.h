// mainwindow.h - IFME's main window: the queue, the settings tabs, encoding.
#pragma once

#include <QMainWindow>
#include <memory>
#include <vector>

#include "profiles.h"
#include "queueitem.h"

class QComboBox;
class QLabel;
class QLineEdit;
class QPlainTextEdit;
class QPushButton;
class QTabWidget;
class QThread;
class QTreeWidget;
class QToolButton;
class QTreeWidgetItem;
class EncodeWorker;
class SettingsTab;
namespace app { class Core; }

class MainWindow : public QMainWindow
{
	Q_OBJECT

public:
	explicit MainWindow(app::Core &core, QWidget *parent = nullptr);
	~MainWindow() override;

	void addFiles(const QStringList &paths);
	void setFormat(const QString &ext);
	void setOutputDir(const QString &dir);
	// Use the profile whose name (or file name) is `name` for the files added
	// next and the selected ones; false when there is none.
	bool setProfile(const QString &name);
	void startEncoding(bool askOverwrite = true);
	void setQuitWhenDone(bool on) { m_quitWhenDone = on; }
	// Developer aid: write a PNG of every tab and dialog into `dir`.
	void saveScreenshots(const QString &dir);

protected:
	void dragEnterEvent(QDragEnterEvent *event) override;
	void dropEvent(QDropEvent *event) override;
	void closeEvent(QCloseEvent *event) override;

private:
	app::Core &m_core;
	std::vector<std::unique_ptr<QueueItem>> m_items;
	int m_nextId = 1;

	QTreeWidget *m_queue;
	QTabWidget *m_tabs;
	QVector<SettingsTab *> m_settingsTabs;
	QPlainTextEdit *m_info, *m_log;
	QComboBox *m_format, *m_profile;
	QToolButton *m_profileMenu;
	QVector<profiles::Profile> m_profiles;
	QString m_profilePath;   // profile new files get (C#: what cboProfile shows)
	QLineEdit *m_outDir;
	QPushButton *m_start, *m_pause, *m_stop;
	QLabel *m_progressText;
	QList<QAction *> m_editActions;

	QThread *m_thread = nullptr;
	EncodeWorker *m_worker = nullptr;
	bool m_quitWhenDone = false;
	bool m_loadingSelection = false;

	QueueItem *itemById(int id) const;
	QTreeWidgetItem *rowOf(int id) const;
	QVector<QueueItem *> selected() const;
	QString outputFor(const QString &input, const QString &ext) const;

	void addFile(const QString &path);
	void refreshRow(const QueueItem &it);
	void selectionChanged();
	void formatChanged();
	void settingsChanged();
	void removeSelected();
	void moveSelected(int delta);
	void setEncoding(bool on);
	void showPlugins();
	void showPreferences();
	void showAbout();
	void log(const QString &line);
	void reloadPlugins();

	void reloadProfiles();
	void profileChosen(int index);
	void showProfile(const QString &path);   // select it in the box, or none
	const profiles::Profile *currentProfile() const;
	void saveProfile();
	void renameProfile();
	void deleteProfile();
};
