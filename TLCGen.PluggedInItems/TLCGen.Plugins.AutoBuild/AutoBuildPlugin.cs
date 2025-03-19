using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Plugins.AutoBuild
{
    [TLCGenTabItem(-1, TabItemTypeEnum.MainWindow)]
    [TLCGenPlugin(TLCGenPluginElems.ToolBarControl | TLCGenPluginElems.PlugMessaging | TLCGenPluginElems.TabControl
		| TLCGenPluginElems.HasSettings | TLCGenPluginElems.MenuControl)]
    public class AutoBuildPlugin : ITLCGenToolBar, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenHasSettings, ITLCGenMenuItem
	{

		#region Fields

		private readonly AutoBuildToolBarViewModel _myVm;
		private AutoBuildSettingsModel _settings;
		private RelayCommand _showSettingsCommand;
		DataTemplate _contentDataTemplate;

		#endregion // Fields

		#region Properties

		public AutoBuildSettingsModel Settings => _settings;

		public AutoBuildConsoleTabViewModel ConsoleVM { get; }

		public ControllerModel Controller { get; set; }

		public string DisplayName => "AutoBuild console";

		public string GetPluginName() => "AutoBuild";

		public bool IsEnabled { get; set; } = true;

		public bool Visibility
		{
			get => _visibility;
			set
			{
				_visibility = value;
			}
		}

		#endregion // ITLCGen plugin shared items

		#region ITLCGenTabItem

        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_contentDataTemplate == null)
                {
	                _contentDataTemplate = new DataTemplate {VisualTree = new FrameworkElementFactory(typeof(AutoBuildConsoleTabView))};
                }
                return _contentDataTemplate;
            }
        }

        public ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "Resources/Icon.xaml");
                dict.Source = u;
                return (DrawingImage)dict["AutoBuildIconDrawingImage"];
            }
        }

        public bool CanBeEnabled()
        {
            return true;
        }

        public void OnSelected()
        {

        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        public void OnDeselected()
        {

        }

        public bool OnDeselectedPreview()
        {
            return true;
        }


        public void LoadTabs()
        {
            
        }

		#endregion // ITLCGenTabItem

		#region ITLCGenToolBar

		public UserControl ToolBarView { get; }
		
		public bool IsVisible { get; set; }

		#endregion // ITLCGenToolBar

		#region ITLCGenMenuItem

		private MenuItem _pluginMenuItem;
		private bool _visibility;

		public MenuItem Menu
		{
			get
			{
				if (_pluginMenuItem == null)
				{
					_pluginMenuItem = new MenuItem
					{
						Header = "AutoBuild"
					};

					var sitem1 = new MenuItem
					{
						Header = "AutoBuild settings",
						Command = ShowSettingsCommand
					};

					_pluginMenuItem.Items.Add(sitem1);
				}
				return _pluginMenuItem;
			}
		}

		#endregion // ITLCGenMenuItem

		#region ITLCGenPlugMessaging

		public void UpdateTLCGenMessaging()
        {
            _myVm?.UpdateTLCGenMessaging();
        }

		#endregion // ITLCGenPlugMessaging

		#region ITLCGenHasSettings

		public void LoadSettings()
		{
			_settings =
				TLCGenSerialization.DeSerializeData<AutoBuildSettingsModel>(
					ResourceReader.GetResourceTextFile("TLCGen.Plugins.AutoBuild.Resources.settings.xml", this));

			var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var setpath = Path.Combine(appdatpath, @"TLCGen\AutoBuilder\");
			if (!Directory.Exists(setpath))
				Directory.CreateDirectory(setpath);
			var setfile = Path.Combine(setpath, @"settings.xml");

			// read custom settings and overwrite defaults
			if (File.Exists(setfile))
			{
				_settings = TLCGenSerialization.DeSerialize<AutoBuildSettingsModel>(setfile);
			}

			if (_settings != null)
			{
				_visibility = _settings.TabVisibility;
				IsVisible = _settings.ToolBarVisibility;
			}
		}

		public void SaveSettings()
		{
			var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			var setpath = Path.Combine(appdatpath, @"TLCGen\AutoBuilder\");
			if (!Directory.Exists(setpath))
				Directory.CreateDirectory(setpath);
			var setfile = Path.Combine(setpath, @"settings.xml");
			TLCGenSerialization.Serialize(setfile, _settings);
		}

		#endregion // ITLCGenHasSettings

		#region Commands

		public ICommand ShowSettingsCommand
		{
			get
			{
				if (_showSettingsCommand == null)
				{
					_showSettingsCommand = new RelayCommand(ShowSettingsCommand_Executed, ShowSettingsCommand_CanExecute);
				}
				return _showSettingsCommand;
			}
		}

		#endregion // Commands
		
		#region Command Functionality

		private void ShowSettingsCommand_Executed()
		{
			var w = new AutoBuildSettingsView
			{
				DataContext = new AutoBuildSettingsViewModel(_settings)
			};
			var window = new Window
			{
				Title = "AutoBuild instellingen",
				Content = w,
				Width = 560,
				Height = 450
			};

			window.ShowDialog();
		}

		private bool ShowSettingsCommand_CanExecute()
		{
			return true;
		}

		#endregion // Command Functionality

		#region Constructor

		public AutoBuildPlugin()
        {
            ToolBarView = new AutoBuildToolBarView();
            _myVm = new AutoBuildToolBarViewModel(this);
            ConsoleVM = new AutoBuildConsoleTabViewModel();
            ToolBarView.DataContext = _myVm;
            Logger.MyDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        #endregion // Constructor
    }
}
