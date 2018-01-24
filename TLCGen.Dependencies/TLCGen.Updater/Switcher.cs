using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TLCGen.Updater
{
	// The (elegant!) solution used here for switching between UserControls comes from here:
	// https://azerdark.wordpress.com/2010/04/23/multi-page-application-in-wpf/
	public static class Switcher
	{
		public static UpdaterWindow pageSwitcher;

		public static void Switch(UserControl newPage)
		{
			pageSwitcher.Navigate(newPage);
		}

		public static void Switch(UserControl newPage, object state)
		{
			pageSwitcher.Navigate(newPage, state);
		}
	}
}
