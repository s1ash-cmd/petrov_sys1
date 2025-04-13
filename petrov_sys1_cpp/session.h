#pragma once
#include "message.h"
#include <queue>
#include <Windows.h>

class Session {
	std::queue<Message> messages;
	CRITICAL_SECTION cs;
	CONDITION_VARIABLE cv;

public:
	int sessionID;

	Session(int sessionID) : sessionID(sessionID) {
		InitializeCriticalSection(&cs);
		InitializeConditionVariable(&cv);
	}

	~Session() {
		DeleteCriticalSection(&cs);
	}

	void addMessage(Message& m) {
		EnterCriticalSection(&cs);
		messages.push(m);
		WakeConditionVariable(&cv);
		LeaveCriticalSection(&cs);
	}

	bool getMessage(Message& m) {
		EnterCriticalSection(&cs);
		while (messages.empty()) {
			SleepConditionVariableCS(&cv, &cs, INFINITE);
		}
		m = messages.front();
		messages.pop();
		LeaveCriticalSection(&cs);
		return true;
	}

	void addMessage(MessageTypes messageType, const std::wstring& data = L"") {
		Message m(messageType, data);
		addMessage(m);
	}
};