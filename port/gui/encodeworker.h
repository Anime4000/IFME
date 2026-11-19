// encodeworker.h - runs the queue's jobs with ifme_encode() on a worker thread.
#pragma once

#include <QByteArray>
#include <QElapsedTimer>
#include <QObject>
#include <QString>
#include <QStringList>
#include <QVector>
#include <atomic>

#include "ifme.h"

struct EncodeJob {
	int id;            // QueueItem::id
	QByteArray json;   // the job for ifme_encode()
};

class EncodeWorker : public QObject
{
	Q_OBJECT

public:
	EncodeWorker(const QString &root, const QStringList &disabled, const QVector<EncodeJob> &jobs);
	~EncodeWorker() override;

	// Safe from the UI thread (the core's task controls are thread-safe).
	void pause();
	void resume();
	void stop();
	bool isPaused() const;

public slots:
	void run();

signals:
	void itemStarted(int id);
	void progress(int id, double percent, const QString &text);
	void output(int id, const QString &line);
	void stage(int id, const QString &text);
	void itemFinished(int id, int result, const QString &error);   // ifme_task_result
	void finished();

private:
	QString m_root;
	QStringList m_disabled;
	QVector<EncodeJob> m_jobs;
	ifme_task *m_task;
	std::atomic_bool m_stop{ false };

	int m_current = -1;
	QElapsedTimer m_lastProgress;
	bool m_haveProgress = false;

	static void onOutput(void *user, const char *line, bool is_status);
	static void onProgress(void *user, const ifme_progress *p);
	static void onStage(void *user, const char *stage);
};
