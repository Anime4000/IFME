// widgets.h - small widgets: the main-window banner and a progress-bar cell.
#pragma once

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

// Draws a progress bar for cells whose Qt::UserRole holds 0..100 (or -1: text).
class ProgressDelegate : public QStyledItemDelegate
{
public:
	using QStyledItemDelegate::QStyledItemDelegate;
	void paint(QPainter *p, const QStyleOptionViewItem &opt, const QModelIndex &idx) const override;
};
