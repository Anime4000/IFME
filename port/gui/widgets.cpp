#include "widgets.h"

#include <QApplication>
#include <QEvent>
#include <QTreeView>
#include <QPainter>

#include "app.h"

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
	// The right image fades in over the left one through its alpha channel.
	p.drawPixmap(QRect(0, 0, w, height()), m_left);
	p.drawPixmap(QRect(width() - w, 0, w, height()), m_right);
}

namespace {

class StickySelection : public QObject
{
public:
	explicit StickySelection(QAbstractItemView *view) : QObject(view), m_view(view)
	{
		apply();
		view->installEventFilter(this);
	}

	bool eventFilter(QObject *, QEvent *e) override
	{
		if (e->type() == QEvent::ApplicationPaletteChange || e->type() == QEvent::StyleChange)
			apply();
		return false;
	}

private:
	void apply()
	{
		QPalette p = QApplication::palette(m_view);
		for (QPalette::ColorRole r : { QPalette::Highlight, QPalette::HighlightedText })
			p.setColor(QPalette::Inactive, r, p.color(QPalette::Active, r));
		m_view->setPalette(p);
	}

	QAbstractItemView *m_view;
};

}  // namespace

void stickySelection(QAbstractItemView *view)
{
	view->setSelectionBehavior(QAbstractItemView::SelectRows);
	if (auto *tree = qobject_cast<QTreeView *>(view))
		tree->setAllColumnsShowFocus(true);
	new StickySelection(view);
}

LanguageCombo::LanguageCombo(QWidget *parent)
	: QComboBox(parent)
{
	setMaxVisibleItems(20);
	for (const app::LanguageCode &l : app::languages())
		addItem(l.name, l.code);
}

void LanguageCombo::setCode(const QString &code)
{
	const QString c = code.isEmpty() ? QStringLiteral("und") : code;
	int i = findData(c);
	if (i < 0) {
		insertItem(0, c, c);
		i = 0;
	}
	setCurrentIndex(i);
}

QString LanguageCombo::code() const
{
	return currentData().toString();
}

QWidget *LanguageDelegate::createEditor(QWidget *parent, const QStyleOptionViewItem &, const QModelIndex &) const
{
	return new LanguageCombo(parent);
}

void LanguageDelegate::setEditorData(QWidget *editor, const QModelIndex &idx) const
{
	auto *c = static_cast<LanguageCombo *>(editor);
	c->setCode(idx.data(Qt::UserRole).toString());
	c->showPopup();
}

void LanguageDelegate::setModelData(QWidget *editor, QAbstractItemModel *model, const QModelIndex &idx) const
{
	auto *c = static_cast<LanguageCombo *>(editor);
	model->setData(idx, c->code(), Qt::UserRole);   // first: the edit handler reads it
	model->setData(idx, c->currentText(), Qt::DisplayRole);
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
