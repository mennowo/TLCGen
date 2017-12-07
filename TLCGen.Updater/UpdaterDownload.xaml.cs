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
	/// Interaction logic for UpdaterDownload.xaml
	/// </summary>
	public partial class UpdaterDownload : UserControl, ISwitchable
	{
		#region Constructor

		public UpdaterDownload()
		{
			InitializeComponent();
		}

		#endregion // Constructor

		#region ISwitchable

		public void UtilizeState(object state)
		{
			this.DataContext = state;
			var vm = (UpdaterViewModel)this.DataContext;
			vm.StartDownload();
		}

		#endregion // ISwitchable

		private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
		{
			var vm = (UpdaterViewModel) this.DataContext;
			vm.StartUpdate();
			Application.Current.Shutdown();
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			var vm = (UpdaterViewModel)this.DataContext;
			vm.AbortUpdate();
			Application.Current.Shutdown();
		}
	}
}
