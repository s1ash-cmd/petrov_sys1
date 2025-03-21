#pragma once

#include <windows.h>
#include <conio.h>
#include <iostream>
#include <vector>
#include <queue>
#include <string>
#include <io.h>
#include <fcntl.h>
#include <thread>
#include <locale>
#include <codecvt>

using namespace std;

inline void DoWrite()
{
	wcout << endl;
}

template <class T, typename... Args> inline void DoWrite(T& value, Args... args)
{
	wcout << value << " ";
	DoWrite(args...);
}

static CRITICAL_SECTION cs;
static bool initCS = true;
template <typename... Args> inline void SafeWrite(Args... args)
{
	if (initCS)
	{
		InitializeCriticalSection(&cs);
		initCS = false;
	}
	EnterCriticalSection(&cs);
	DoWrite(args...);
	LeaveCriticalSection(&cs);
}

