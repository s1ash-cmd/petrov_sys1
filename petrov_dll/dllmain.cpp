// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include "asio.h"
#include "../petrov_sys1_cpp/message.h"

//#include <codecvt>
//#include <locale>

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

extern "C" {
	__declspec(dllexport) void __stdcall sendCommand(int selected_thread, int commandId, const wchar_t* message)
	{
		try {
			boost::asio::io_context io;
			tcp::socket socket(io);
			tcp::resolver resolver(io);
			auto endpoints = resolver.resolve("127.0.0.1", "12345");
			boost::asio::connect(socket, endpoints);

			MessageHeader header;
			header.messageType = commandId;
			header.size = message ? static_cast<int>(wcslen(message) * sizeof(wchar_t)) : 0;
			header.from = selected_thread;

			sendData(socket, &header, sizeof(header));
			if (header.size > 0)
				sendData(socket, message, header.size);
		}
		catch (const std::exception& e) {
			std::wcerr << L"[DLL] Невозможно отправить команду: " << e.what() << std::endl;
		}
	}

	__declspec(dllexport) int __stdcall getSessionCount()
	{
		try {
			boost::asio::io_context io;
			tcp::socket socket(io);
			tcp::resolver resolver(io);
			auto endpoints = resolver.resolve("127.0.0.1", "12345");
			boost::asio::connect(socket, endpoints);

			MessageHeader header;
			header.messageType = MT_GETDATA;
			header.size = 0;
			header.from = -1;

			sendData(socket, &header, sizeof(header));

			MessageHeader responseHeader;
			boost::asio::read(socket, boost::asio::buffer(&responseHeader, sizeof(responseHeader)));

			if (responseHeader.messageType == MT_GETDATA && responseHeader.size == sizeof(int)) {
				int sessionCount;
				boost::asio::read(socket, boost::asio::buffer(&sessionCount, sizeof(int)));
				return sessionCount;
			}
			return 0;
		}
		catch (const std::exception& e) {
			std::wcerr << L"[DLL] Ошибка при получении количества сессий: " << e.what() << std::endl;
			return 0;
		}
	}
}