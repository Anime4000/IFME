// widgets.h - small widgets: the main-window banner and a progress-bar cell.
#pragma once

#include <QAbstractItemView>
#include <QComboBox>
#include <QPixmap>
#include <QStyledItemDelegate>
#include <QWidget>

// C#'s banner: Banner_4a on the left, Banner_2b on the right, both scaled to
// the banner's height (frmMain.RebuildBanner).
class BannerWidget : public QWidget
{
public:
	explicit BannerWidget(QWidget *parent = nullptr);
	QSize sizeHint() const override { return { 1024, 64 }; }

protected:
	void paintEvent(QPaintEvent *) override;

private:
	QPixmap m_left, m_right;
};

// Whole-row selection that keeps its highlight when the list loses focus (as
// a .NET ListView with HideSelection off), also after a theme change.
void stickySelection(QAbstractItemView *view);

// Language picker (C#: cboVideoLang etc.): full names shown, ISO 639-2 codes
// as data. A code missing from the list is added so it is not lost.
class LanguageCombo : public QComboBox
{
	Q_OBJECT

public:
	explicit LanguageCombo(QWidget *parent = nullptr);
	void setCode(const QString &code);
	QString code() const;
};

// Edits a tree column holding a language: shows the name, keeps the code in
// Qt::UserRole.
class LanguageDelegate : public QStyledItemDelegate
{
public:
	using QStyledItemDelegate::QStyledItemDelegate;
	QWidget *createEditor(QWidget *parent, const QStyleOptionViewItem &, const QModelIndex &) const override;
	void setEditorData(QWidget *editor, const QModelIndex &idx) const override;
	void setModelData(QWidget *editor, QAbstractItemModel *model, const QModelIndex &idx) const override;
};

// Draws a progress bar for cells whose Qt::UserRole holds 0..100 (or -1: text).
class ProgressDelegate : public QStyledItemDelegate
{
public:
	using QStyledItemDelegate::QStyledItemDelegate;
	void paint(QPainter *p, const QStyleOptionViewItem &opt, const QModelIndex &idx) const override;
};
