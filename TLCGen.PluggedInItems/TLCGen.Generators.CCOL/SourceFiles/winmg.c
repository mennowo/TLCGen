#ifndef ___MGBARSRGV
#define ___MGBARSRGV


/*
* include files
*/

#include "winmg.h"
#include "fcvar.h"
#include "control.h"
#include "TCHAR.H" /* MVW: include tbv voorkomen compiler warning voor gebruik van '_tcsstr' */

/*
*
*/

/* default waarden */
#define MAX_HEIGHT  400
#define BAR_WIDTH   15
#define DEF_NR_FC   5      /* bepaald initiele breedte van het scherm, ruimte voor [nr] fasecycli */

#define AUTO_FC -1

/*
*
*/

struct fc_gegevens {
	char  show;

	mulv* vg_set;
	mulv* vg_current;

	mulv  vg_set_prev;
	mulv  vg_current_prev;
	mulv  vg_time_prev;
};

/*
* Functie prototypes
*/

LRESULT CALLBACK FaseMgWndProc(HWND, UINT, WPARAM, LPARAM);

static void fase_mg_update(void);

#define HIDEBUTTON 1

/*
* Globale data
*/

static int maxautofc;

static HWND    hWnd = NULL;
static HWND hTemp;

static HBRUSH  brushes[3]; /* 0 = basis, 1 = actueel 2 = time */

static int i_char_w;
static int i_char_h;
static int win_height;
static int winx = 0;
static int winy = 0;
static int fc_width = 0;
static int bar_width = BAR_WIDTH;
static int fc_height = 0;
static int borderheight;

static HWND hHideButton;

static struct fc_gegevens fc_mg[FCMAX];
static int g_initialised = 0;

/*
*
*/
ATOM RegisterFaseMgClass(HINSTANCE hInstance)
{
	WNDCLASS wc;

	wc.style = CS_HREDRAW | CS_VREDRAW;
	wc.lpfnWndProc = (WNDPROC)FaseMgWndProc;
	wc.cbClsExtra = 0;
	wc.cbWndExtra = 0;
	wc.hInstance = hInstance;
	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.hbrBackground = GetStockObject(WHITE_BRUSH);
	wc.lpszMenuName = NULL;
	wc.lpszClassName = "FaseMgClass";

	return RegisterClass(&wc);
}

/*
*
*/
void MG_Fasen_Venster_init(LPCTSTR lpWindowName, ...)
{

	if (!g_initialised)
	{
		HWND hParent;
		HINSTANCE hInst;
		CREATESTRUCT cs;
		HDC hDC;
		TEXTMETRIC tm;
		RECT rect;
		char windowtext[100];
		int fc, i;
		va_list argp;

		/** MVW: in principe wordt dit deel niet aangeroepen, wanneer MG_Bars_init() wordt aangeroepen voor MG_Bars in de reg.add */
		if (!g_initialised)
		{

			/* vraag hoogte op in pixels van bovenkant window (border + systeem caption */
			borderheight = GetSystemMetrics(SM_CYCAPTION) + 3 * GetSystemMetrics(SM_CYSIZEFRAME);

			/* initiatie globale data */
			for (fc = 0; fc < FCMAX; ++fc)
			{
				fc_mg[fc].show = 0;
				if (fc_mg[fc].vg_set == NULL) fc_mg[fc].vg_set = &TVG_max[fc];   /* anders krijg ik nooit iets in beeld, indien niet init functie wordt aangeroepen */
				if (fc_mg[fc].vg_current == NULL) fc_mg[fc].vg_current = &TVG_max[fc];

				fc_mg[fc].vg_set_prev = 0;
				fc_mg[fc].vg_current_prev = 0;
				fc_mg[fc].vg_time_prev = 0;
			}

			maxautofc = 0;

			va_start(argp, lpWindowName);
			fc = va_arg(argp, va_count);
			if (fc == AUTO_FC)
			{
				for (i = 0; i < FCMAX; ++i)
				{
					if (strlen(FC_code[i]) == 2 &&
						(strstr((FC_code[i]), "01") ||
							strstr((FC_code[i]), "02") ||
							strstr((FC_code[i]), "03") ||
							strstr((FC_code[i]), "04") ||
							strstr((FC_code[i]), "05") ||
							strstr((FC_code[i]), "06") ||
							strstr((FC_code[i]), "07") ||
							strstr((FC_code[i]), "08") ||
							strstr((FC_code[i]), "09") ||
							strstr((FC_code[i]), "10") ||
							strstr((FC_code[i]), "11") ||
							strstr((FC_code[i]), "12")) ||
							(strlen(FC_code[i]) == 3 &&
						(strstr((FC_code[i]), "0") == (FC_code[i] + 1) || /* Voor 01 tot en met 09 -> match op 0 op positie 2 */
							strstr((FC_code[i]), "10") == (FC_code[i] + 1) ||
							strstr((FC_code[i]), "11") == (FC_code[i] + 0)))) /* Voor #11 en #12 -> match op eerste twee karakters*/
					{
						fc_mg[i].show = 1;
						++maxautofc;
					}
				}
			}
			else
			{
				while (fc != END)
				{
					fc_mg[fc].show = 1;
					++maxautofc;
					fc = va_arg(argp, va_count);
				}
			}
			va_end(argp);

			/* standaard waarden */
			win_height = i_char_h + 2 + fc_height + borderheight;
			SetWindowPos(hWnd, HWND_TOP, 0, 200, maxautofc * fc_width, win_height, SWP_SHOWWINDOW);
			winx = winy = 0;

			/* benodigde breedte per bar */
			fc_width = max(3 * (bar_width + 1), 3 * i_char_w + 2);
			/* niet de g_initialised op 1 zetten !! Dan kan de init functie alsnog geevalueerd worden (weliswaar in verkeerde volgorde) */


		}

		/** MVW: Opzoeken app window obv (gedeeltelijke) WindowName ipv ClassName */
		/* hParent = FindWindow(__TEXT("CCOLWIN"), lpWindowName); */
		/* opzoeken parent window */
		/* De window name wordt doorgegeven vanuit MG_Bars() als argument van die functie. */
		/* In de aanroep ervan in de reg.add wordt 'SYSTEM' ingevuld, wat gelijk is aan het kruispuntnummer als c-style string ofwel char[] */
		hParent = FindWindow(NULL, szWndTitleName);
		if (hParent)
		{
			/* opvragen window instance */
			hInst = (HINSTANCE)GetWindowLong(hParent, GWL_HINSTANCE);
			/* registreren window class voor deze window instance */
			if (hInst && RegisterFaseMgClass(hInst))
			{
				/* window titel */
				strcpy(windowtext, "Groentijden - ");
				strncat(windowtext, lpWindowName, 100 - strlen(windowtext));

				/* class naam en window attributen */
				hWnd = CreateWindow("FaseMgClass",
					windowtext,
					WS_POPUP | WS_CAPTION | WS_SIZEBOX | WS_VISIBLE,
					/* hier opgeven: x, y, w en h van nieuwe window */
					/** MVW: window attributen obv variabelen */
					winx, winy, fc_width * maxautofc + bar_width + 5, win_height, hParent, NULL,
					hInst,
					(LPVOID)&cs);

				/* check of er nu een window is */
				if (!hWnd)
				{
					return;
				}

				/* create necessary brushes */
				brushes[0] = CreateSolidBrush(RGB(255, 0, 0)); /* RED        */
				brushes[1] = CreateSolidBrush(RGB(0, 0, 255)); /* YELLOW     */
				brushes[2] = CreateSolidBrush(RGB(0, 255, 0)); /* LIGHTGREEN */

															   /* get device context */
				hDC = GetDC(hWnd);

				/* vraag text grootte op en sla dat op */
				GetTextMetrics(hDC, &tm);
				i_char_w = tm.tmAveCharWidth;
				i_char_h = tm.tmHeight + tm.tmExternalLeading;

				/* schilderen! */
				InvalidateRect(hWnd, &rect, TRUE);

				/* release device context */
				ReleaseDC(hWnd, hDC);

			}
		}

		/* opzetten init bool */
		g_initialised = 1;
	}
}

/*
*
*/
void MG_Bars_init(mulv* basis, mulv* actueel, mulv _bar_width, mulv height, mulv x, mulv y)
{
	if (!g_initialised)
	{
		int fc;
		HDC hDC;
		TEXTMETRIC tm;
		/* opvragen systeem border hoogte */
		borderheight = GetSystemMetrics(SM_CYCAPTION) + 3 * GetSystemMetrics(SM_CYSIZEFRAME);
		/* get device context */
		hDC = GetDC(hWnd);

		/* vraag text grootte op en sla dat op */
		GetTextMetrics(hDC, &tm);
		i_char_w = tm.tmAveCharWidth;
		i_char_h = tm.tmHeight + tm.tmExternalLeading;

		/* initiatie globale data */

		/* hier worden de balken gekoppeld aan data uit de reg.add van de rgv*/
		for (fc = 0; fc<min(FC_MAX, 64); ++fc)
		{
			if (fc_mg[fc].vg_set == NULL) fc_mg[fc].vg_set = &TVG_max[fc];   /* anders krijg ik nooit iets in beeld, indien niet init functie wordt aangeroepen */
			if (fc_mg[fc].vg_current == NULL) fc_mg[fc].vg_current = &TVG_max[fc];
			fc_mg[fc].show = 0;
			fc_mg[fc].vg_set = ((basis != NULL) ? &basis[fc] : &TVG_max[fc]);
			fc_mg[fc].vg_current = ((actueel != NULL) ? &actueel[fc] : &TVG_max[fc]);
			fc_mg[fc].vg_set_prev = 0;
			fc_mg[fc].vg_current_prev = 0;
			fc_mg[fc].vg_time_prev = 0;
		}
		/* toekennen window attribuut variabelen */
		fc_height = height;
		bar_width = _bar_width;
		win_height = i_char_h + 2 + fc_height + borderheight;
		fc_width = max(3 * (bar_width + 1), 3 * i_char_w + 2);
		winx = x;
		winy = y;
	}
}

/*
*
*/
void MG_Bars()
{
	if (TE)
	{
		if (hWnd)
		{
			fase_mg_update();
		}
	}
}

/*
*
*/
LRESULT CALLBACK FaseMgWndProc(HWND hWnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	PAINTSTRUCT ps;
	HDC         hDC;
	int         i, j;
	HINSTANCE hInst;

	switch (uMsg) {
	case WM_CREATE:
		hInst = (HINSTANCE)GetWindowLong(hWnd, GWL_HINSTANCE);
		hHideButton = CreateWindowEx(
			0,          // no extended style
			"BUTTON",   // Static class name
			"Sluiten",   // Static control's text
			WS_CHILD | WS_VISIBLE,  // control style
			5,         // x position
			5,         // y position
			100,        // control width
			20,         // control height
			hWnd,       // parent control
			(HMENU)HIDEBUTTON,       // no menu/ID info
			hInst,     // instance handler
			NULL        // no extra creation data
		);
	case WM_SIZE:
		fc_height = HIWORD(lParam) - i_char_h - 2;
		win_height = i_char_h + 2 + fc_height + borderheight;
		InvalidateRect(hWnd, NULL, TRUE);
		return 0;

	case WM_COMMAND:
		switch (LOWORD(wParam))
		{
		case HIDEBUTTON:
			ShowWindow(hWnd, SW_HIDE);
		}
		break;


	case WM_PAINT:
		// win_height = i_char_h + 2 + fc_height + borderheight;
		hDC = BeginPaint(hWnd, &ps);

		/* FC labels */
		/* --------- */
		SetTextColor(hDC, RGB(0, 0, 0));
		SetBkMode(hDC, TRANSPARENT);
		j = 0;

		for (i = 0; i < FCMAX; ++i)
		{
			if (fc_mg[i].vg_set != NULL &&
				fc_mg[i].vg_current != NULL)
			{
				if (*(fc_mg[i].vg_set) >= 0)  /* TVG_max[i] >= 0 */
				{
					if (fc_mg[i].show)
					{
						/* label */
						TextOut(hDC,
							j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME),
							win_height - i_char_h - borderheight - 1,
							FC_code[i],
							lstrlen(FC_code[i]));

						/* basis */
						SelectObject(hDC, brushes[0]);
						Rectangle(hDC,
							j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME),
							fc_height - (*(fc_mg[i].vg_set) + TFG_max[i]),
							j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + bar_width,
							win_height - i_char_h - borderheight - 2);

						/* actueel */
						SelectObject(hDC, brushes[1]);
						Rectangle(hDC,
							j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 1 + bar_width,
							fc_height - (*(fc_mg[i].vg_current) + TFG_max[i]),
							j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 1 + bar_width + bar_width,
							win_height - i_char_h - borderheight - 2);


						/* time */
						if (G[i])
						{
							SelectObject(hDC, brushes[2]);
							Rectangle(hDC,
								j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 2 * (1 + bar_width),
								fc_height - (TVG_timer[i] + TFG_timer[i]),
								j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 2 * (1 + bar_width) + bar_width,
								win_height - i_char_h - borderheight - 2);
						}
						j++;
					}
				}
			}
		}
		EndPaint(hWnd, &ps);
		return 0;
	}
	return DefWindowProc(hWnd, uMsg, wParam, lParam);
}

/*
*
*/
static void fase_mg_update(void)
{
	int  i;
	int  j;
	RECT rect;


	j = 0;
	for (i = 0; i < FCMAX; ++i)
	{
		if (fc_mg[i].vg_set != NULL &&
			fc_mg[i].vg_current != NULL)
		{
			if (*(fc_mg[i].vg_set) >= 0) /* verlenggroentijd opgegeven */
			{
				if (fc_mg[i].show)
				{
					if (*(fc_mg[i].vg_set) != fc_mg[i].vg_set_prev)
					{
						rect.left = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME);
						rect.top = fc_height - (max(*(fc_mg[i].vg_set), fc_mg[i].vg_set_prev) + TFG_max[i]);
						rect.right = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + bar_width;
						rect.bottom = win_height - i_char_h - borderheight - 2;

						InvalidateRect(hWnd, &rect, TRUE);

						fc_mg[i].vg_set_prev = *(fc_mg[i].vg_set);
					}
					if (*(fc_mg[i].vg_current) != fc_mg[i].vg_current_prev)
					{
						rect.left = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 1 + bar_width;
						rect.top = fc_height - (max(*(fc_mg[i].vg_current), fc_mg[i].vg_current_prev) + TFG_max[i]);
						rect.right = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 1 + bar_width + bar_width;
						rect.bottom = win_height - i_char_h - borderheight - 2;

						InvalidateRect(hWnd, &rect, TRUE);

						fc_mg[i].vg_current_prev = *(fc_mg[i].vg_current);
					}
					if (G[i] && TE || EG[i])
					{
						rect.left = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 2 * (1 + bar_width);
						rect.top = fc_height - max((TVG_timer[i] + TFG_timer[i]), fc_mg[i].vg_time_prev);
						rect.right = j * fc_width + GetSystemMetrics(SM_CXSIZEFRAME) + 2 * (1 + bar_width) + bar_width,
							rect.bottom = win_height - i_char_h - borderheight - 2;

						InvalidateRect(hWnd, &rect, TRUE);

						fc_mg[i].vg_time_prev = TVG_timer[i] + TFG_timer[i];
					}
					j++;
				}
				else
				{ /* wel data bijwerken, ook al wordt hij nu niet getoond */
					fc_mg[i].vg_set_prev = *(fc_mg[i].vg_set);
					fc_mg[i].vg_current_prev = *(fc_mg[i].vg_current);
					if (G[i] && TE) fc_mg[i].vg_time_prev = TVG_timer[i] + TFG_timer[i];
				}
			}
		}
	}
	SendMessage(hWnd, WM_PAINT, 0, 0);
}

#endif
