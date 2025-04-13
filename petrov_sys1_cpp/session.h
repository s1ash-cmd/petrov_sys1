#pragma once
#include "message.h"

class Session {
	queue<Message> messages;
	CRITICAL_SECTION cs;

public:
	int sessionID;

	Session(int sessionID) : sessionID(sessionID) {
		InitializeCriticalSection(&cs);
	}

	~Session() {
		DeleteCriticalSection(&cs);
	}

	void addMessage(Message& m) {
		EnterCriticalSection(&cs);
		messages.push(m);
		LeaveCriticalSection(&cs);
	}

	bool getMessage(Message& m) {
		bool res = false;
		EnterCriticalSection(&cs);
		if (!messages.empty()) {
			res = true;
			m = messages.front();
			messages.pop();
		}
		LeaveCriticalSection(&cs);
		return res;
	}

	void addMessage(MessageTypes messageType, const wstring& data = L"") {
		Message m(messageType, data);
		addMessage(m);
	}
};