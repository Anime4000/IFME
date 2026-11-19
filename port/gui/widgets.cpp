#include "widgets.h"

#include <QApplication>
#include <QPainter>

BannerWidget::BannerWidget(QWidget *parent)
	: QWidget(parent)
	, m_left(QStringLiteral(":/ifme/banner-left.png"))
	, m_right(QStringLiteral(":/ifme/banner-right.png"))
{
	setFixedHeight(64);
	setSizePolicy(QSizePolicy::Expanding, QSizePolicy::Fixed);
}

void BannerWidget::paintEvent(QPaintEvent *)
{
	QPainter p(this);
	p.setRenderHint(QPainter::SmoothPixmapTransform);
	p.fillRect(rect(), Qt::black);
	if (m_left.isNull() || m_left.height() <= 0)
		return;

	const double scale = height() / static_cast<double>(m_left.height());
	const int w = qMax(1, static_cast<int>(m_left.width() * scale));
	p.drawPixmap(QRect(width() - w, 0, w, height()), m_right);
	p.drawPixmap(QRect(0, 0, w, height()), m_left);
}

void ProgressDelegate::paint(QPainter *p, const QStyleOptionViewItem &opt, const QModelIndex &idx) const
{
	const QVariant v = idx.data(Qt::UserRole);
	if (!v.isValid() || v.toDouble() < 0) {
		QStyledItemDelegate::paint(p, opt, idx);
		return;
	}

	QStyleOptionProgressBar bar;
	bar.rect = opt.rect.adjusted(2, 2, -2, -2);
	bar.minimum = 0;
	bar.maximum = 1000;
	bar.progress = static_cast<int>(v.toDouble() * 10);
	bar.text = QStringLiteral("%1%").arg(v.toDouble(), 0, 'f', 1);
	bar.textVisible = true;
	bar.state = opt.state | QStyle::State_Horizontal;
	QApplication::style()->drawControl(QStyle::CE_ProgressBar, &bar, p);
}
