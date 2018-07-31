#include "windows.h"
#include "TCHAR.H"		/* voorkomen compiler warning voor gebruik van '_tcsstr' */
#include "math.h"		/* voor cos, sin, etc. */
#include "time.h"
#include <control.h>	/* Tbv combo boxes */
#include <CommCtrl.h>

HWND hWndMain;
HWND hWndParent;
HINSTANCE hInst;

#ifdef UNICODE
WCHAR lpstrThis[128];
LPCWSTR lpstrThisp = lpstrThis;
#else
CHAR lpstrThis[128];
#endif

static int iFasen[20];
static int iUitgangen[20];
static int iType[20] = { -1 };
int iFasenIndex = 0;
int iFasenSel = -1;

HBITMAP bufBMP;
HDC memHDC;
HWND hWndComboBox;

RECT refrect;

#define DROPDOWNFIETS	10000

extern const char * FC_code[];
extern short MM[];

HPEN hPens[10];
HBRUSH hBrushes[10];
HFONT font_wtv_wacht;

#define TYPE_LEDS 0
#define TYPE_SEGM 1

#define BRUSH_BLACK	0
#define BRUSH_RED	1
#define BRUSH_WHITE	2
#define BRUSH_GRAY	3

#define PEN_WHITE 0
#define PEN_BLACK 1

#define PI 3.1415
const int iLedsMax = 31;

extern short CIF_GUS[];
#define GUS CIF_GUS

int iWTVleds = 0;

void extrawin_init();
ATOM RegisterMainWinClass(HINSTANCE hInstance);
BOOL CALLBACK WorkerProc(HWND hwnd, LPARAM lParam);
LRESULT CALLBACK WtvTestWndProc(HWND hWndMir, UINT uMsg, WPARAM wParam, LPARAM lParam);
void DisplayWTVDigit(HDC hdc, int iNumber, int x, int y);
void extrawin_wtv(int fc, int mm);

void extrawin_wtv(int fc, int mm)
{
	if (!(iFasen[iFasenSel] == fc))
		return;

	iWTVleds = MM[mm];
	// refresh...
	//if (!(TE == 5))
	//{
		InvalidateRect(hWndMain, &refrect, TRUE);
	//}
}

void extrawin_add_fc(short fc, short usbus, short type)
{
	if(iFasenIndex >= 20)
		return;

#ifdef UNICODE
	WCHAR tmpp[256];
	mbstowcs(tmpp, FC_code[fc], strlen(FC_code[fc]));
#else
	CHAR tmpp[256];
	sprintf_s(tmpp, 256, FC_code[fc]);
#endif
	SendMessage(hWndComboBox, CB_ADDSTRING, (WPARAM)0, (LPARAM)tmpp);

	iFasen[iFasenIndex] = fc;
	iUitgangen[iFasenIndex] = usbus;
	iType[iFasenIndex] = type;
	iFasenIndex++;
}

void extrawin_init(char * system)
{
	DWORD dwStyle;
	CREATESTRUCT css;

#ifdef UNICODE
	WCHAR tmpp[256];
	mbstowcs(tmpp, system, strlen(system));
	swprintf_s(lpstrThis, 128, TEXT("%s - wachttijdvoorspellers"), tmpp);
#else
	sprintf_s(lpstrThis, 128, "%s - wachttijdvoorspellers", system);
#endif

	dwStyle = (DWORD)(WS_VISIBLE);

	/* opvragen window instance */
	hInst = (HINSTANCE)GetModuleHandle(NULL);
	RegisterMainWinClass(hInst);
	/* class naam en window attributen */
	if (!hWndMain)
		hWndMain = CreateWindowEx(0, TEXT("WTVTestWindow"), TEXT(lpstrThis), dwStyle,
		//0, 0, 505, 65,
		0, 0, 230, 275,
		hWndParent, NULL, hInst, (LPVOID)&css);

	SetWindowPos(hWndMain,       // handle to window
		HWND_TOPMOST,  // placement-order handle
		0,     // horizontal position
		0,      // vertical position
		0,  // width
		0, // height
		SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOMOVE// window-positioning options
		);

	refrect.left = 0;
	refrect.top = 30;
	refrect.right = 230;
	refrect.bottom = 275;

	hWndComboBox = CreateWindow(WC_COMBOBOX, TEXT(""),
		CBS_DROPDOWN | CBS_HASSTRINGS | WS_CHILD | WS_VSCROLL | WS_OVERLAPPED | WS_VISIBLE,
		7, 7, 200, 170, hWndMain, (HMENU)DROPDOWNFIETS, hInst,
		NULL);

	hBrushes[BRUSH_BLACK] = CreateSolidBrush(RGB(0, 0, 0));
	hBrushes[BRUSH_RED] = CreateSolidBrush(RGB(255, 0, 0));
	hBrushes[BRUSH_WHITE] = CreateSolidBrush(RGB(255, 255, 255));
	hBrushes[BRUSH_GRAY] = CreateSolidBrush(RGB(127, 127, 127));
	
	hPens[PEN_WHITE] = CreatePen(PS_SOLID, 1, RGB(255, 255, 255));
	hPens[PEN_BLACK] = CreatePen(PS_SOLID, 1, RGB(0, 0, 0));
}

/** Functie registreren window class */
ATOM RegisterMainWinClass(HINSTANCE hInstance)
{
	WNDCLASS wcc;

	wcc.style = CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS;
	wcc.lpfnWndProc = (WNDPROC)WtvTestWndProc;
	wcc.cbClsExtra = 0;
	wcc.cbWndExtra = 0;
	wcc.hInstance = hInstance;
	wcc.hIcon = NULL;
	wcc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wcc.hbrBackground = NULL;
	wcc.lpszMenuName = NULL;
	wcc.lpszClassName = "WTVTestWindow";

	return RegisterClass(&wcc);
}

/** WINDOWS PROCEDURE */
LRESULT CALLBACK WtvTestWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	int i;
	HDC hdc;
	RECT rect;
	PAINTSTRUCT ps;

	int x, y;
	double round = PI;
	double radius = 90;
	int ledsize = 12;


	switch (uMsg)
	{
	case WM_CREATE:
		/* Get the sizes of the system font, for formatting */
		hdc = GetDC(hWnd);
		memHDC = CreateCompatibleDC(hdc);
		bufBMP = CreateCompatibleBitmap(hdc, 1600, 1600);
		font_wtv_wacht = CreateFont(40, 0, 0, 0, FW_BOLD, 0, 0, 0, 0, 0, 0, 0, 0, TEXT("Arial"));
		SelectObject(memHDC, bufBMP);
		return 0;
	case WM_PAINT:

		// Erase background
		GetClientRect(hWnd, &rect);
		SelectObject(memHDC, hBrushes[BRUSH_GRAY]);
		Rectangle(memHDC, 0, 0, rect.right, rect.bottom);

		if (iType[iFasenSel] == TYPE_LEDS)
		{
			/* Tekst: zwart, doorzichtig */
			SetTextColor(memHDC, RGB(0, 0, 0));
			SetBkMode(memHDC, TRANSPARENT);

			for (i = iLedsMax - 1, round = 0.5 * PI + (((float)iLedsMax / 8.0) * ((2 * PI) / (iLedsMax * 1.2)));
				/* (round < ((2.5 * PI) - (((float)iLedsMax / 10.0) * ((2 * PI) / (iLedsMax * 1.2))))) && */ i >= 0;
				round += ((2 * PI) / (iLedsMax * 1.2)), --i)
				//for (round = 0; round < 2 * PI; round += ((2 * PI) / 39.0))
				//for (i = 0; i < iLedsMax; ++i)
			{
				SelectObject(memHDC, i >= iWTVleds ? hBrushes[BRUSH_BLACK] : hBrushes[BRUSH_RED]);
				//SelectObject(memHDC, hBrushes[BRUSH_RED]);
				x = (int)(cos(round) * radius) + 100;
				y = (int)(sin(round) * radius) + 135;
				//x = i * 15 + 10;
				//y = 10;
				Ellipse(memHDC, x, y, x + ledsize, y + ledsize);
			}

			if (iUitgangen[iFasenSel] != -1 && GUS[iUitgangen[iFasenSel]])
			{
#ifdef UNICODE
				WCHAR tmp[128];
				LPCWSTR lpTmp;
				lpTmp = tmp;
				swprintf_s(tmp, 128, TEXT("BUS"));
#else
				CHAR tmp[128];
				sprintf_s(tmp, 128, "BUS");
#endif
				SelectObject(memHDC, font_wtv_wacht);
				SetTextColor(memHDC, RGB(255, 255, 255));
				TextOut(memHDC, 70, 110, tmp, strlen("BUS"));
				SetTextColor(memHDC, RGB(0, 0, 0));
			}
		}

		if (iType[iFasenSel] == TYPE_SEGM)
		{
			SelectObject(memHDC, hBrushes[BRUSH_WHITE]);
			SelectObject(memHDC, hPens[PEN_BLACK]);
			DisplayWTVDigit(memHDC, (iWTVleds / 100), 30, 50);
			DisplayWTVDigit(memHDC, (iWTVleds / 10), 80, 50);
			DisplayWTVDigit(memHDC, (iWTVleds % 10), 130, 50);
		}

		/*
			if (wtvledsfc1)
			{

				SelectObject(hDC, wtv_wacht);
				SetTextColor(hDC, RGB(255, 0, 0));
				sprintf_s(lpszTemp1, SZBUFFERSIZE, "WACHT");
				TextOut(hDC, 60, 90 + iWTVSTop, lpszTemp1, strlen(lpszTemp1));
				SetTextColor(hDC, RGB(0, 0, 0));
				SelectObject(hDC, mir_default_font);
				sprintf_s(lpszTemp1, SZBUFFERSIZE, "Resterend: %d", rest_tijd1);
				TextOut(hDC, 60, 130 + iWTVSTop, lpszTemp1, strlen(lpszTemp1));
			}*/


		/* Buffer kopieren naar scherm dc */
		hdc = BeginPaint(hWnd, &ps);
		BitBlt(hdc, 0, 0, 800, 800, memHDC, 0, 0, SRCCOPY);
		EndPaint(hWnd, &ps);
		return 0;
	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case DROPDOWNFIETS:
			if (HIWORD(wParam) == CBN_SELCHANGE)
			{
				iFasenSel = SendMessage((HWND)lParam, (UINT)CB_GETCURSEL, (WPARAM)0, (LPARAM)0);
			}
			break;
		}
		return 0;
	}
	return DefWindowProc(hWnd, uMsg, wParam, lParam);
}

/* Function below: From WIN32 API book by Charles Petzhold */
void DisplayWTVDigit(HDC hdc, int iNumber, int x, int y)
{
	int iSeg;
	static BOOL fSevenSegment[10][7] =
	{
		1, 1, 1, 0, 1, 1, 1, // 0
		0, 0, 1, 0, 0, 1, 0, // 1
		1, 0, 1, 1, 1, 0, 1, // 2
		1, 0, 1, 1, 0, 1, 1, // 3
		0, 1, 1, 1, 0, 1, 0, // 4
		1, 1, 0, 1, 0, 1, 1, // 5
		1, 1, 0, 1, 1, 1, 1, // 6
		1, 0, 1, 0, 0, 1, 0, // 7
		1, 1, 1, 1, 1, 1, 1, // 8
		1, 1, 1, 1, 0, 1, 1  // 9
	};
	static POINT ptSegment[7][6] =
	{ //        o       o
		//o						   o
		//        					            o            	o
		7, 6, 11, 2, 31, 2, 35, 6, 31, 10, 11, 10, 		//  -
		6, 7, 10, 11, 10, 31, 6, 35, 2, 31, 2, 11,		// |
		36, 7, 40, 11, 40, 31, 36, 35, 32, 31, 32, 11,		//   |
		7, 36, 11, 32, 31, 32, 35, 36, 31, 40, 11, 40,		//  -
		6, 37, 10, 41, 10, 61, 6, 65, 2, 61, 2, 41,		// |
		36, 37, 40, 41, 40, 61, 36, 65, 32, 61, 32, 41,		//   |
		7, 66, 11, 62, 31, 62, 35, 66, 31, 70, 11, 70			//  -
	};

	if (iNumber >= 10)
		return;
	SetWindowOrgEx(hdc, -x, -y, NULL);
	for (iSeg = 0; iSeg < 7; iSeg++)
		if (fSevenSegment[iNumber][iSeg])
			Polygon(hdc, ptSegment[iSeg], 6);
	SetWindowOrgEx(hdc, 0, 0, NULL);
}