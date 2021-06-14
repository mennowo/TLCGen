using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Plugins.Additor
{
    [TLCGenTabItem(-1, TabItemTypeEnum.MainWindow)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging | TLCGenPluginElems.TabControl | TLCGenPluginElems.HasSettings | TLCGenPluginElems.MenuControl)]
    public class AdditorPlugin : ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenHasSettings, ITLCGenMenuItem
    {
        #region Fields
        
        private readonly AdditorTabViewModel _additorTabVm;
        private ControllerModel _controller;
        private DataTemplate _contentDataTemplate;
        private bool _isEnabled = true;
        private AdditorSettingsModel _settings;
        private bool _visibility;
        private MenuItem _pluginMenuItem;
        private RelayCommand _showSettingsCommand;

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region ITLCGen plugin shared items

        public ControllerModel Controller
        {
            get { return _controller; }
            set
            {
                _controller = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return "Add editor";
            }
        }

        public string GetPluginName()
        {
            return "Additor";
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
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
                    _contentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(AdditorTabView));
                    tab.SetValue(FrameworkElement.DataContextProperty, _additorTabVm);
                    _contentDataTemplate.VisualTree = tab;
                }
                return _contentDataTemplate;
            }
        }

        public ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                                ";component/" + "Resources/Icon.xaml");
                dict.Source = u;
                return (DrawingImage)dict["AdditorIconDrawingImage"];
            }
        }
        
        public bool Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
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

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _additorTabVm?.UpdateTLCGenMessaging();
        }

        #endregion // ITLCGenPlugMessaging
        
        #region ITLCGenMenuItem
        
        public MenuItem Menu
        {
            get
            {
                if (_pluginMenuItem == null)
                {
                    _pluginMenuItem = new MenuItem
                    {
                        Header = "Additor"
                    };

                    var sitem1 = new MenuItem
                    {
                        Header = "Additor settings",
                        Command = ShowSettingsCommand
                    };

                    _pluginMenuItem.Items.Add(sitem1);
                }
                return _pluginMenuItem;
            }
        }

        #endregion // ITLCGenMenuItem
        
        #region ITLCGenHasSettings

        public void LoadSettings()
        {
            _settings =
                TLCGenSerialization.DeSerializeData<AdditorSettingsModel>(
                    ResourceReader.GetResourceTextFile("TLCGen.Plugins.Additor.Resources.settings.xml", this));

            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Additor\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");

            // read custom settings and overwrite defaults
            if (File.Exists(setfile))
            {
                _settings = TLCGenSerialization.DeSerialize<AdditorSettingsModel>(setfile);
            }

            if (_settings != null)
            {
                _visibility = _settings.TabVisibility;
            }
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Additor\");
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

        private void ShowSettingsCommand_Executed(object obj)
        {
            var w = new AdditorSettingsView()
            {
                DataContext = new AdditorSettingsViewModel(_settings)
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

        private bool ShowSettingsCommand_CanExecute(object obj)
        {
            return true;
        }

        #endregion // Command Functionality
        
        #region Constructor

        public AdditorPlugin()
        {
            _additorTabVm = new AdditorTabViewModel(this);
        }

        #endregion // Constructor
    }
}
