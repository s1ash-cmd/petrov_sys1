// dllmain.cpp : Определяет точку входа для приложения DLL.
#include "pch.h"
#include "asio.h"
#include "../petrov_sys1_cpp/message.h"

//#include <codecvt>
//#include <locale>

BOOL APIENTRY DllMain( HMODULE hModule,
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

    __declspec(dllexport) void __stdcall connectClient(int selected_thread, const wchar_t* text)
    {
        try {
            boost::asio::io_context io;
            tcp::socket s(io);
            tcp::resolver r(io);
            boost::asio::connect(s, r.resolve("127.0.0.1", "12345"));

        }
        catch (std::exception& e)
        {
            std::wcerr << L"[DLL] Exception in connect: " << e.what() << std::endl;
        }
    }

    __declspec(dllexport) void __stdcall disconnectClient(int selected_thread, const wchar_t* text)
    {
        //try {
        //    boost::asio::io_context io;
        //    tcp::socket socket(io);
        //    tcp::resolver resolver(io);
        //    boost::asio::connect(socket, resolver.resolve("server_address", "12345"));
        //}
        //catch (std::exception& e)
        //{
        //    std::wcerr << L"[DLL] Exception in disconnect: " << e.what() << std::endl;
        //}
    }

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
            std::wcerr << L"[DLL] Exception in sendCommand: " << e.what() << std::endl;
        }
    }
}