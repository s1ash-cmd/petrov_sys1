#include "C:\Users\s1ash\source\repos\petrov_sys1\petrov_dll\dllmain.cpp"
#include "SysProg.h"

using namespace std;

enum MessageTypes {
	MT_CLOSE,
	MT_DATA,
};

struct MessageHeader {
	int messageType;
	int size;
};

struct Message {
	MessageHeader header = { 0 };
	wstring data;
	Message() = default;
	Message(MessageTypes messageType, const wstring& data = L"") : data(data) {
		header = { messageType, static_cast<int>(data.length()) };
	}
};

class Session {
	queue<Message> messages;
	CRITICAL_SECTION cs;
	HANDLE hEvent;

public:
	int sessionID;

	Session(int sessionID) : sessionID(sessionID) {
		InitializeCriticalSection(&cs);
		hEvent = CreateEventW(NULL, TRUE, FALSE, NULL);
	}

	~Session() {
		DeleteCriticalSection(&cs);
		CloseHandle(hEvent);
	}

	void addMessage(Message& m) {
		EnterCriticalSection(&cs);
		messages.push(m);
		SetEvent(hEvent);
		LeaveCriticalSection(&cs);
	}

	bool getMessage(Message& m) {
		bool res = false;
		WaitForSingleObject(hEvent, INFINITE);
		EnterCriticalSection(&cs);
		if (!messages.empty()) {
			res = true;
			m = messages.front();
			messages.pop();
		}
		if (messages.empty()) {
			ResetEvent(hEvent);
		}
		LeaveCriticalSection(&cs);
		return res;
	}

	void addMessage(MessageTypes messageType, const wstring& data = L"") {
		Message m(messageType, data);
		addMessage(m);
	}
};

void MyThread(Session* session) {
	EnterCriticalSection(&cs);
	wcout << L"Сессия " << session->sessionID << L" создана" << endl;
	LeaveCriticalSection(&cs);

	while (true) {
		Message m;
		if (session->getMessage(m)) {
			switch (m.header.messageType) {
			case MT_CLOSE:
				EnterCriticalSection(&cs);
				wcout << L"Сессия " << session->sessionID << L" закрыта" << endl;
				LeaveCriticalSection(&cs);
				delete session;
				return;
			case MT_DATA: {
				if (session->sessionID >= 0) {
					wstring filename = to_wstring(session->sessionID) + L".txt";
					wofstream fout(filename, ios::app);

					fout.imbue(locale("ru_RU.UTF-8"));

					if (!fout.is_open()) {
						wcout << L"Ошибка открытия файла: " << filename << endl;
					}
					else {
						fout << m.data << L"\n";
						fout.flush();
						fout.close();
					}
				}
//send recive в класс сообщениий
				break;
			}
			}
		}
	}
}

void start() {
	vector<Session*> sessions;
	vector<thread> threads;
	InitializeCriticalSection(&cs);

	int sessionCounter = 1;

	HANDLE hStartEvent = CreateEventW(NULL, FALSE, FALSE, L"StartEvent");
	HANDLE hStopEvent = CreateEventW(NULL, FALSE, FALSE, L"StopEvent");
	HANDLE hConfirmEvent = CreateEventW(NULL, FALSE, FALSE, L"ConfirmEvent");
	HANDLE hSendEvent = CreateEventW(NULL, FALSE, FALSE, L"SendEvent");
	HANDLE hCloseEvent = CreateEventW(NULL, FALSE, FALSE, L"CloseEvent");
	HANDLE hControlEvents[4] = { hStartEvent, hStopEvent, hSendEvent, hCloseEvent };

	SetConsoleOutputCP(65001);
	SetConsoleCP(65001);
	wcout.imbue(locale("ru_RU.UTF-8"));

	while (true) {
		int n = WaitForMultipleObjects(4, hControlEvents, FALSE, INFINITE) -
			WAIT_OBJECT_0;

		switch (n) {
		case 0: {
			sessions.push_back(new Session(sessionCounter++));
			threads.emplace_back(MyThread, sessions.back());
			SetEvent(hConfirmEvent);
			break;
		}
		case 1: {
			if (!sessions.empty()) {
				sessions.back()->addMessage(MT_CLOSE);
				threads.back().join();
				sessions.pop_back();
				threads.pop_back();
				sessionCounter--;
				SetEvent(hConfirmEvent);
			}
			if (sessions.empty()) {
				CloseHandle(hStartEvent);
				CloseHandle(hStopEvent);
				CloseHandle(hConfirmEvent);
				CloseHandle(hCloseEvent);
				DeleteCriticalSection(&cs);
				return;
			}
				break;
		}

		case 2: {
		header h;
		wstring data = ReceiveData(h);

			wstring message(data.begin(), data.end());
			if (h.addr == -2) {
				for (auto& sess : sessions)
					sess->addMessage(MT_DATA, message);

				SafeWrite(L"Главный поток: " + message);
				SafeWrite(L"Сообщение записано в файлы для каждого потока");
			}
			else if (h.addr == -1) {
				SafeWrite(L"Главный поток: " + message);
			}
			else {
				int index = h.addr;
				if (index >= 0 && index < sessions.size())
				{
					sessions[index]->addMessage(MT_DATA, message);
				}
				SafeWrite(L"Сообщение записано в файл ", index + 1, L".txt");
			}
			ResetEvent(hSendEvent);
			SetEvent(hConfirmEvent);
			break;
		}

		case 3: {
			SetEvent(hCloseEvent);
			return;
			break;
		}
		}
	}
}

int main() {
	start();
	return 0;
}
