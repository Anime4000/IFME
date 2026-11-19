// queueitem.h - one file in the queue and the job that encodes it.
#pragma once

#include <QFileInfo>
#include <QJsonObject>
#include <QString>

struct QueueItem {
	enum State { Ready, Encoding, Done, Failed, Stopped };

	int id = 0;
	QString input;
	QJsonObject job;      // edited by the settings tabs, passed to ifme_encode()
	QJsonObject media;    // ifme_media_json() of the input
	QString info;         // Media Info text
	State state = Ready;
	double percent = -1;
	QString message;      // last progress text or error
	QString profile;      // path of the profile last applied (empty: none)

	QString container() const
	{
		return QFileInfo(job.value(QStringLiteral("output")).toString()).suffix().toLower();
	}
};
