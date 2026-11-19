#include "dialogs.h"

#include <QCheckBox>
#include <QComboBox>
#include <QDialogButtonBox>
#include <QDir>
#include <QFileDialog>
#include <QFormLayout>
#include <QHBoxLayout>
#include <QHeaderView>
#include <QLabel>
#include <QLineEdit>
#include <QPushButton>
#include <QStandardPaths>
#include <QThread>
#include <QTreeWidget>
#include <QVBoxLayout>

#include "app.h"
#include "i18n.h"
#include "widgets.h"

// ---- Preferences --------------------------------------------------------------

static QWidget *pathRow(QWidget *parent, QLineEdit *edit, const QString &title)
{
	auto *w = new QWidget(parent);
	auto *h = new QHBoxLayout(w);
	h->setContentsMargins(0, 0, 0, 0);
	auto *browse = new QPushButton(QObject::tr("Browse..."), w);
	h->addWidget(edit, 1);
	h->addWidget(browse);
	QObject::connect(browse, &QPushButton::clicked, w, [=] {
		const QString dir = QFileDialog::getExistingDirectory(w, title, edit->text());
		if (!dir.isEmpty())
			edit->setText(QDir::toNativeSeparators(dir));
	});
	return w;
}

PreferencesDialog::PreferencesDialog(QWidget *parent)
	: QDialog(parent)
{
	setWindowTitle(tr("Preferences"));
	QSettings &s = app::settings();
	m_root = new QLineEdit(s.value(QStringLiteral("paths/root")).toString(), this);
	m_root->setPlaceholderText(tr("the folder of this program"));
	m_output = new QLineEdit(s.value(QStringLiteral("paths/output"),
		QDir::toNativeSeparators(QStandardPaths::writableLocation(QStandardPaths::MoviesLocation))).toString(), this);
	m_temp = new QLineEdit(s.value(QStringLiteral("paths/temp")).toString(), this);
	m_temp->setPlaceholderText(QDir::toNativeSeparators(QDir::tempPath()));
	m_keepTemp = new QCheckBox(tr("Keep temporary files after encoding (for troubleshooting)"), this);
	m_keepTemp->setChecked(s.value(QStringLiteral("encode/keep_temp"), false).toBool());
	m_test = new QCheckBox(tr("Test encoders at start-up and disable those that fail (GPU encoders)"), this);
	m_test->setChecked(s.value(QStringLiteral("startup/test_encoders"), true).toBool());

	// Interface language (C#: frmOptions grpLanguage), with its translators.
	m_language = new QComboBox(this);
	auto *credit = new QLabel(this);
	credit->setOpenExternalLinks(true);
	credit->setWordWrap(true);
	const QVector<i18n::Language> langs = i18n::available();
	for (const i18n::Language &l : langs)
		m_language->addItem(!l.total ? l.name
			: l.drafts ? tr("%1 - %2% translated, %3% not yet reviewed").arg(l.name).arg(100 * l.translated / l.total).arg(100 * l.drafts / l.total)
			           : tr("%1 - %2% translated").arg(l.name).arg(100 * l.translated / l.total), l.code);
	m_language->setCurrentIndex(qMax(0, m_language->findData(i18n::chosen())));
	auto showCredit = [=] {
		const int i = m_language->currentIndex();
		if (i <= 0 || i >= langs.size()) {
			credit->setText(tr("Changes take effect the next time IFME starts."));
			return;
		}
		const i18n::Language &l = langs[i];
		QString who = l.author.toHtmlEscaped();
		if (!l.profile.isEmpty())
			who = QStringLiteral("<a href=\"%1\">%2</a>").arg(l.profile.toHtmlEscaped(), who);
		credit->setText(tr("Translated by %1. Changes take effect the next time IFME starts.").arg(who));
	};
	connect(m_language, &QComboBox::currentIndexChanged, this, showCredit);
	showCredit();

	auto *f = new QFormLayout;
	f->addRow(tr("Interface &language:"), m_language);
	f->addRow(QString(), credit);
	f->addRow(tr("IFME folder (holds Plugins):"), pathRow(this, m_root, tr("IFME folder")));
	f->addRow(tr("Save encoded files to:"), pathRow(this, m_output, tr("Output folder")));
	f->addRow(tr("Temporary folder:"), pathRow(this, m_temp, tr("Temporary folder")));
	f->addRow(m_keepTemp);
	f->addRow(m_test);

	auto *buttons = new QDialogButtonBox(QDialogButtonBox::Ok | QDialogButtonBox::Cancel, this);
	connect(buttons, &QDialogButtonBox::accepted, this, &QDialog::accept);
	connect(buttons, &QDialogButtonBox::rejected, this, &QDialog::reject);
	auto *v = new QVBoxLayout(this);
	v->addLayout(f);
	v->addWidget(buttons);
	resize(640, sizeHint().height());
}

void PreferencesDialog::accept()
{
	QSettings &s = app::settings();
	m_rootChanged = s.value(QStringLiteral("paths/root")).toString() != m_root->text().trimmed();
	s.setValue(QStringLiteral("paths/root"), m_root->text().trimmed());
	s.setValue(QStringLiteral("paths/output"), m_output->text().trimmed());
	s.setValue(QStringLiteral("paths/temp"), m_temp->text().trimmed());
	s.setValue(QStringLiteral("encode/keep_temp"), m_keepTemp->isChecked());
	s.setValue(QStringLiteral("startup/test_encoders"), m_test->isChecked());
	const QString lang = m_language->currentData().toString();
	m_languageChanged = lang != i18n::chosen();
	s.setValue(QStringLiteral("ui/language"), lang);
	QDialog::accept();
}

// ---- Plugins --------------------------------------------------------------------

PluginsDialog::PluginsDialog(app::Core &core, bool canTest, QWidget *parent)
	: QDialog(parent)
	, m_core(core)
{
	setWindowTitle(tr("Plugins"));
	resize(900, 560);

	m_list = new QTreeWidget(this);
	m_list->setHeaderLabels({ tr("Name"), tr("Kind"), tr("Version"), tr("Status"), tr("Note") });
	m_list->setRootIsDecorated(false);
	stickySelection(m_list);
	m_list->setSortingEnabled(false);
	m_list->header()->setStretchLastSection(true);

	m_testSome = new QPushButton(tr("&Test GPU encoders"), this);
	m_testSome->setToolTip(tr("Encode the sample with every plugin that asks for a test, as at start-up"));
	m_testAll = new QPushButton(tr("Test &all"), this);
	m_testSome->setEnabled(canTest);
	m_testAll->setEnabled(canTest);
	m_status = new QLabel(canTest ? tr("Untick a plugin to disable it.") : tr("Tests are not available while encoding."), this);
	auto *close = new QPushButton(tr("Close"), this);

	auto *h = new QHBoxLayout;
	h->addWidget(m_status, 1);
	h->addWidget(m_testSome);
	h->addWidget(m_testAll);
	h->addWidget(close);
	auto *v = new QVBoxLayout(this);
	v->addWidget(new QLabel(tr("Root: %1").arg(QDir::toNativeSeparators(m_core.root())), this));
	v->addWidget(m_list, 1);
	v->addLayout(h);

	connect(close, &QPushButton::clicked, this, &QDialog::reject);
	connect(m_testSome, &QPushButton::clicked, this, [this] { runTest(false); });
	connect(m_testAll, &QPushButton::clicked, this, [this] { runTest(true); });
	connect(m_list, &QTreeWidget::itemChanged, this, [this](QTreeWidgetItem *it, int column) {
		if (m_loading || column != 0)
			return;
		QStringList off = m_core.disabled();
		const QString guid = it->data(0, Qt::UserRole).toString();
		off.removeAll(guid);
		if (it->checkState(0) == Qt::Unchecked)
			off << guid;
		m_core.setDisabled(off);
		m_changed = true;
		reload();
	});
	reload();
}

void PluginsDialog::reload()
{
	m_loading = true;
	m_list->clear();
	for (const app::Plugin &p : m_core.plugins()) {
		auto *it = new QTreeWidgetItem(m_list, { p.name, p.audio ? tr("audio") : tr("video"), p.version, p.status,
		                                         p.message.isEmpty() && p.testRequired ? tr("asks for a test") : p.message });
		it->setData(0, Qt::UserRole, p.guid);
		const bool canToggle = p.status == QLatin1String("ok") || p.status == QLatin1String("disabled") ||
		                       p.status == QLatin1String("test-failed");
		if (canToggle) {
			it->setFlags(it->flags() | Qt::ItemIsUserCheckable);
			it->setCheckState(0, m_core.disabled().contains(p.guid) ? Qt::Unchecked : Qt::Checked);
		}
		if (p.status != QLatin1String("ok") && p.status != QLatin1String("builtin"))
			for (int c = 0; c < 5; c++)
				it->setForeground(c, palette().color(QPalette::Disabled, QPalette::Text));
	}
	for (int c = 0; c < 4; c++)
		m_list->resizeColumnToContents(c);
	m_loading = false;
}

void PluginsDialog::runTest(bool all)
{
	m_testing = true;
	m_testSome->setEnabled(false);
	m_testAll->setEnabled(false);
	m_list->setEnabled(false);
	m_status->setText(tr("Testing..."));

	struct State { PluginsDialog *dlg; QStringList failed; };
	auto *st = new State{ this, {} };
	ifme_ctx *ctx = m_core.ctx();
	QThread *t = QThread::create([ctx, all, st] {
		ifme_selftest_callbacks cb{};
		cb.user = st;
		cb.on_start = [](void *user, const ifme_plugin_info *p) {
			auto *s = static_cast<State *>(user);
			const QString name = QString::fromUtf8(p->name);
			QMetaObject::invokeMethod(s->dlg, [s, name] { s->dlg->m_status->setText(tr("Testing %1...").arg(name)); },
			                          Qt::QueuedConnection);
		};
		cb.on_result = [](void *user, const ifme_plugin_info *p, ifme_test_outcome o, const char *, unsigned) {
			if (o == IFME_TEST_FAIL)
				static_cast<State *>(user)->failed << QString::fromUtf8(p->guid);
		};
		ifme_plugins_selftest(ctx, all, &cb);
	});
	connect(t, &QThread::finished, this, [this, t, st] {
		t->deleteLater();
		// As at start-up: plugins that fail are disabled (tick them to retry).
		m_core.setDisabled(m_core.disabled() + st->failed);
		m_status->setText(st->failed.isEmpty() ? tr("All tested plugins work.")
		                                       : tr("%n plugin(s) failed and were disabled.", nullptr, st->failed.size()));
		delete st;
		m_testing = false;
		m_changed = true;
		m_testSome->setEnabled(true);
		m_testAll->setEnabled(true);
		m_list->setEnabled(true);
		reload();
	});
	t->start();
}

void PluginsDialog::reject()
{
	if (!m_testing)   // the test thread still reports to this dialog
		QDialog::reject();
}
