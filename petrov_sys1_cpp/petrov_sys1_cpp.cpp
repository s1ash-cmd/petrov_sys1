#include "../petrov_dll/dllmain.cpp"
#include "../petrov_dll/asio.h"
#include "SysProg.h"

using namespace std;

void launchClient(wstring path) {
	wstring pathCopy = path;

	STARTUPINFOW si = { sizeof(si) };
	PROCESS_INFORMATION pi;

	if (CreateProcessW(NULL, &pathCopy[0],
		NULL, NULL, TRUE, CREATE_NEW_CONSOLE,
		NULL, NULL, &si, &pi))
	{
		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
	}
	else
	{
		wcerr << L"Не удалось запустить клиент: " << path << endl;
	}
}

vector<Session*> sessions;
vector<thread> threads;
mutex sessionsMutex;

void MyThread(Session* session) {
	SafeWrite(L"Сессия", session->sessionID + 1, L"создана");

	while (true) {
		Message m;
		if (session->getMessage(m)) {
			switch (m.header.messageType) {
			case MT_CLOSE: {
				SafeWrite(L"Сессия", session->sessionID + 1, L"закрыта");
				delete session;
				return;
			}
			case MT_DATA: {
				if (session->sessionID >= 0) {
					wstring filename = to_wstring(session->sessionID + 1) + L".txt";
					wofstream fout(filename, ios::app);

					fout.imbue(locale("ru_RU.UTF-8"));

					if (!fout.is_open()) {
						wcout << L"Ошибка открытия файла: " << filename << endl;
					}
					else {
						fout << m.data << "\n";
						fout.close();
					}
				}
				break;
			}
			}
		}
	}
}

void processClient(tcp::socket s) {
	try {
		Message m;
		int code = m.receive(s);
		// wcout << L"Получено сообщение: "
		// 	<< m.header.messageType << L", размер: "
		// 	<< m.header.size << L", код: "
		// 	<< code << endl;

		switch (code) {
		case MT_INIT: {
			unique_lock<mutex> lock(sessionsMutex);
			int id = sessions.size();
			Session* newSession = new Session(id);
			sessions.push_back(newSession);
			threads.emplace_back(MyThread, newSession);
			threads.back().detach();
			break;
		}

		case MT_EXIT: {
			unique_lock<mutex> lock(sessionsMutex);
			if (m.header.from == -1) {
				for (auto& s : sessions) {
					s->addMessage(MT_CLOSE);
				}
				sessions.clear();
				threads.clear();
			}
			else {
				int id = m.header.from;
				if (id == sessions.size() - 1) {
					sessions[id]->addMessage(MT_CLOSE);
					sessions.pop_back();
					threads.pop_back();
				}

			}
			break;
		}
		case MT_SENDDATA: {
			unique_lock<mutex> lock(sessionsMutex);

			int id = m.header.from;
			wstring text = m.data;

			if (id == -1) {
				SafeWrite(L"Главный поток: " + text);
			}
			else if (id == -2) {
				SafeWrite(L"Главный поток: " + text);
				SafeWrite(L"Сообщение записано в файлы для каждого потока");

				for (auto session : sessions)
					if (session)
						session->addMessage(MT_DATA, text);
			}
			else if (id >= 0 && id < sessions.size() && sessions[id]) {
				sessions[id]->addMessage(MT_DATA, text);
				SafeWrite(L"Сообщение записано в файл ", id + 1, L".txt");
			}
			break;
		}
		case MT_GETDATA: {
			unique_lock<mutex> lock(sessionsMutex);
			int sessionCount = sessions.size();

			MessageHeader responseHeader;
			responseHeader.messageType = MT_GETDATA;
			responseHeader.from = -1;
			responseHeader.size = sizeof(int);

			sendData(s, &responseHeader, sizeof(responseHeader));
			sendData(s, &sessionCount, sizeof(int));
			break;
		}

		}
	}
	catch (std::exception& e)
	{
		std::wcerr << "Возникала ошибка: " << e.what() << endl;
	}
}


int main(int argc, char* argv[]) {
	SetConsoleOutputCP(65001);
	SetConsoleCP(65001);
	wcout.imbue(locale("ru_RU.UTF-8"));

	int port = 12345;
	boost::asio::io_context io;
	tcp::acceptor a(io, tcp::endpoint(tcp::v4(), port));
	wcout << L"Сервер запущен (порт: " << port << ")" << endl;

	int client_num = 2;
	for (int i = 0; i < client_num; i++) {
		launchClient(L"C:/Users/s1ash/source/repos/petrov_sys1/Debug/petrov_sys1.exe");
	}

	while (true)
	{
		std::thread(processClient, a.accept()).detach();
	}
}
