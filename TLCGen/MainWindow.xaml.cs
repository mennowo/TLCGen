using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TLCGen.Dialogs;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen
{
    public static class DispatcherHelper
    {
        public static Dispatcher UIDispatcher { get; private set; }

        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null)
                return;

            CheckDispatcher();

            if (UIDispatcher.CheckAccess())
                action();
            else
                UIDispatcher.BeginInvoke(action);
        }

        private static void CheckDispatcher()
        {
            if (UIDispatcher == null)
                throw new InvalidOperationException("The DispatcherHelper is not initialized.\n" +
                                                    "Call DispatcherHelper.Initialize() in the static App constructor.");
        }

        public static DispatcherOperation RunAsync(Action action)
        {
            CheckDispatcher();
            return UIDispatcher.BeginInvoke(action);
        }

        public static void Initialize()
        {
            if (UIDispatcher != null && UIDispatcher.Thread.IsAlive)
                return;

            UIDispatcher = Dispatcher.CurrentDispatcher;
        }

        public static void Reset()
        {
            UIDispatcher = null;
        }
    }

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

            MainToolBarTray.DataContextChanged += (_, e) =>
            {
	            if (!(e.NewValue is MainWindowViewModel vm)) return;
                foreach (var pl in vm.ApplicationParts)
                {
                    if ((pl.Item1 & TLCGenPluginElems.ToolBarControl) != TLCGenPluginElems.ToolBarControl) continue;
                    var tb = new ToolBar();
                    if (pl.Item2 is ITLCGenToolBar {IsVisible: true} tlcGenToolBar)
                    {
	                    tb.Items.Add(tlcGenToolBar.ToolBarView);
						MainToolBarTray.ToolBars.Add(tb);
                    }
                }
            };

            Loaded += (_, _) =>
            {
	            var verNow = Assembly.GetEntryAssembly()?.GetName().Version;
	            var verEula = new System.Version(0, 0, 0, 0);
	            if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.EulaSeen) && 
	                System.Version.TryParse(SettingsProvider.Default.Settings.EulaSeen, out var v))
	            {
		            verEula = v;
	            }
	            if (verNow > verEula)
	            {
		            var about = new EulaWindow();
		            var result = about.ShowDialog();
		            if (result != true) Application.Current.Shutdown();
		            else SettingsProvider.Default.Settings.EulaSeen = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
	            }
            };

            RecentFileList.MenuClick += (_, args) =>
            {
                if (DataContext == null) return;
                var mymvm = DataContext as MainWindowViewModel;
                mymvm?.LoadController(args.Filepath);
            };

            var mvm = new MainWindowViewModel();
            DataContext = mvm;

            mvm.FileSaved += (_, s) => RecentFileList.InsertFile(s);
            mvm.FileOpened += (_, s) => RecentFileList.InsertFile(s);
            mvm.FileOpenFailed += (_, s) => RecentFileList.RemoveFile(s);

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
