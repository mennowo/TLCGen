using System;
using System.Windows.Threading;

namespace TLCGen
{
    internal static class TLCGenSplashScreenHelper
	{
		public static TLCGenSplashScreenView SplashScreen { get; set; }

		public static void Show()
		{
			SplashScreen?.Show();
		}

		public static void Hide()
		{
			SplashScreen?.Hide();
			SplashScreen = null;
		}

		public static void ShowText(string text)
		{
			SplashScreen?.Dispatcher.Invoke(
				DispatcherPriority.Render,
				new Action(delegate { SplashScreen.SplashText.Text = text; }));
		}
	}
}