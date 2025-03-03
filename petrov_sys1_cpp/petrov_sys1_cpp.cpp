#include "SysProg.h"

using namespace std;

enum MessageTypes
{
	MT_CLOSE,
	MT_DATA,
};

struct MessageHeader
{
	int messageType;
	int size;
};

struct Message
{
	MessageHeader header = { 0 };
	string data;
	Message() = default;
	Message(MessageTypes messageType, const string& data = "")
		:data(data)
	{
		header = { messageType, int(data.length()) };
	}
};

class Session
{
	queue<Message> messages;
	CRITICAL_SECTION cs;
	HANDLE hEvent;
public:
	int sessionID;

	Session(int sessionID) :sessionID(sessionID)
	{
		InitializeCriticalSection(&cs);
		hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
	}

	~Session()
	{
		DeleteCriticalSection(&cs);
		CloseHandle(hEvent);
	}

	void addMessage(Message& m)
	{
		EnterCriticalSection(&cs);
		messages.push(m);
		SetEvent(hEvent);
		LeaveCriticalSection(&cs);
	}

	bool getMessage(Message& m)
	{
		bool res = false;
		WaitForSingleObject(hEvent, INFINITE);
		EnterCriticalSection(&cs);
		if (!messages.empty())
		{
			res = true;
			m = messages.front();
			messages.pop();
		}
		if (messages.empty())
		{
			ResetEvent(hEvent);
		}
		LeaveCriticalSection(&cs);
		return res;
	}

	void addMessage(MessageTypes messageType, const string& data = "")
	{
		Message m(messageType, data);
		addMessage(m);
	}
};

DWORD WINAPI MyThread(LPVOID lpParameter)
{
	auto session = static_cast<Session*>(lpParameter);
	EnterCriticalSection(&cs);
	cout << "Session " << session->sessionID << " created" << endl;
	LeaveCriticalSection(&cs);

	while (true)
	{
		Message m;
		if (session->getMessage(m))
		{
			switch (m.header.messageType)
			{
			case MT_CLOSE: {
				EnterCriticalSection(&cs);
				cout << "Session " << session->sessionID << " closed" << endl;
				LeaveCriticalSection(&cs);
				delete session;
				return 0;
			}
			case MT_DATA: {
				EnterCriticalSection(&cs);
				cout << "Session " << session->sessionID << " data: " << m.data << endl;
				LeaveCriticalSection(&cs);
				Sleep(500 * session->sessionID);
				break;
			}
			}
		}
	}
	return 0;
}

void start()
{
	vector<Session*> sessions;
	vector<HANDLE> threads;
	InitializeCriticalSection(&cs);

	int sessionCounter = 1;

	HANDLE hStartEvent = CreateEventW(NULL, FALSE, FALSE, L"StartEvent");
	HANDLE hStopEvent = CreateEventW(NULL, FALSE, FALSE, L"StopEvent");
	HANDLE hConfirmEvent = CreateEventW(NULL, FALSE, FALSE, L"ConfirmEvent");
	HANDLE hCloseEvent = CreateEventW(NULL, FALSE, FALSE, L"CloseEvent");
	HANDLE hControlEvents[3] = { hStartEvent, hStopEvent, hCloseEvent };

	while (true)
	{
		int n = WaitForMultipleObjects(3, hControlEvents, FALSE, INFINITE) - WAIT_OBJECT_0;
		switch (n)
		{
		case 0:
			sessions.push_back(new Session(sessionCounter++));
			threads.push_back(CreateThread(NULL, 0, MyThread, (LPVOID)sessions.back(), 0, NULL));
			SetEvent(hConfirmEvent);
			break;
		case 1:
			if (!sessions.empty())
			{
				sessions.back()->addMessage(MT_CLOSE);
				WaitForSingleObject(threads.back(), INFINITE);
				CloseHandle(threads.back());
				sessions.pop_back();
				threads.pop_back();
				sessionCounter--;
				SetEvent(hConfirmEvent);
			}
			if (sessions.empty())
			{
				CloseHandle(hStartEvent);
				CloseHandle(hStopEvent);
				CloseHandle(hConfirmEvent);
				DeleteCriticalSection(&cs);
				return;
			}
			break;

		case 2: 
			SetEvent(hCloseEvent);
			return;
			break;
		}

	}
}

int main()
{
	start();
	return 0;
}