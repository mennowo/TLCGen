using System.Windows.Controls;

namespace TLCGen.Updater
{
	// The (elegant!) solution used here for switching between UserControls comes from here:
	// https://azerdark.wordpress.com/2010/04/23/multi-page-application-in-wpf/
	public static class Switcher
	{
		public static UpdaterWindow PageSwitcher;

		public static void Switch(UserControl newPage)
		{
			PageSwitcher.Navigate(newPage);
		}

		public static void Switch(UserControl newPage, object state)
		{
			PageSwitcher.Navigate(newPage, state);
		}
	}
}
