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
using GalaSoft.MvvmLight.Threading;
using TLCGen.Plugins;
using TLCGen.ViewModels;

namespace TLCGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

	        TLCGenSplashScreenHelper.SplashScreen = new TLCGenSplashScreenView();
	        TLCGenSplashScreenHelper.Show();
	        TLCGenSplashScreenHelper.ShowText("TLCGen wordt gestart...");

			DispatcherHelper.Initialize();

            MainToolBarTray.DataContextChanged += (s, e) =>
            {
	            if (!(e.NewValue is MainWindowViewModel vm)) return;
                foreach (var pl in vm.ApplicationParts)
                {
                    if ((pl.Item1 & TLCGenPluginElems.ToolBarControl) != TLCGenPluginElems.ToolBarControl) continue;
                    var tb = new ToolBar();
	                if (pl.Item2 is ITLCGenToolBar tlcGenToolBar) tb.Items.Add(tlcGenToolBar.ToolBarView);
                    MainToolBarTray.ToolBars.Add(tb);
                }
            };

            RecentFileList.MenuClick += (sender, args) =>
            {
                if (DataContext == null) return;
                var mymvm = DataContext as MainWindowViewModel;
                mymvm?.LoadController(args.Filepath);
            };

            var mvm = new ViewModels.MainWindowViewModel();
            DataContext = mvm;

            mvm.FileSaved += (sender, s) => RecentFileList.InsertFile(s);
            mvm.FileOpened += (sender, s) => RecentFileList.InsertFile(s);
            mvm.FileOpenFailed += (sender, s) => RecentFileList.RemoveFile(s);

            mvm.CheckCommandLineArgs();

	        TLCGenSplashScreenHelper.Hide();
        }

	    protected override void OnClosed(EventArgs e)
	    {
		    base.OnClosed(e);

		    Application.Current.Shutdown();
	    }
    }
}
