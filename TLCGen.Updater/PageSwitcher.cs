using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TLCGen.Updater
{
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
