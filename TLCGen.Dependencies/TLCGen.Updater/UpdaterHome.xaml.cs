using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TLCGen.Updater
{
	/// <summary>
	/// Interaction logic for UpdaterHome.xaml
	/// </summary>
	public partial class UpdaterHome : UserControl, ISwitchable
	{
		#region Constructor

		public UpdaterHome()
		{
			InitializeComponent();
		}

		#endregion // Constructor

		#region ISwitchable
		
		public void UtilizeState(object state)
		{
			this.DataContext = state;
		}
		
		#endregion // ISwitchable

		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			Switcher.Switch(new UpdaterDownload(), this.DataContext);
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
