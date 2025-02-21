#include <windows.h>
#include <iostream>

using namespace std;

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

HANDLE hEvents[100];
CRITICAL_SECTION cs;

DWORD WINAPI MyThread(LPVOID lpParam)
{
    int i = (intptr_t)lpParam; 
    EnterCriticalSection(&cs);
    cout << i << endl;
    LeaveCriticalSection(&cs);

    WaitForSingleObject(hEvents[i], INFINITE);
    cout << i << " done" << endl;
    CloseHandle(hEvents[i]);

    return 0;
}

void start()
{
    InitializeCriticalSection(&cs);
    int i;
    for (i = 0; i < 10; ++i)
    {
        hEvents[i] = CreateEvent(NULL, FALSE, FALSE, NULL);
        CreateThread(NULL, 0, MyThread, (LPVOID)(intptr_t)i, 0, NULL);
    }

    HANDLE hStartEvent = CreateEventW(NULL, FALSE, FALSE, L"StartEvent");
    HANDLE hStopEvent = CreateEventW(NULL, FALSE, FALSE, L"StopEvent");
    HANDLE hConfirmEvent = CreateEventW(NULL, FALSE, FALSE, L"ConfirmEvent");

    HANDLE hControlEvents[2] = { hStartEvent, hStopEvent };
    while (i)
    {
        int n = WaitForMultipleObjects(2, hControlEvents, FALSE, INFINITE) - WAIT_OBJECT_0;
        switch (n)
        {
        case 0:
            hEvents[i] = CreateEvent(NULL, FALSE, FALSE, NULL);
            CreateThread(NULL, 0, MyThread, (LPVOID)(intptr_t)i, 0, NULL);
            SetEvent(hConfirmEvent);
            i++;
            break;
        case 1:
            ResetEvent(hStopEvent);
            SetEvent(hEvents[--i]);
            SetEvent(hConfirmEvent);
            break;
        }
    }
    SetEvent(hConfirmEvent);
    DeleteCriticalSection(&cs);
}



int main()
{
    start();
    return 0;
}
