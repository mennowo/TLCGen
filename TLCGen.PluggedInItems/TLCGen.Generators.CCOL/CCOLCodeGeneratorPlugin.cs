using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator | TLCGenPluginElems.HasSettings | TLCGenPluginElems.MenuControl | TLCGenPluginElems.PlugMessaging)]
    public class CCOLCodeGeneratorPlugin : ITLCGenGenerator, ITLCGenHasSettings, ITLCGenMenuItem, ITLCGenPlugMessaging
    {
        private static bool _noView;

        #region ITLCGenGenerator

        private readonly CCOLGenerator _generator;

        [Browsable(false)]
        public UserControl GeneratorView { get; }

        public string GetGeneratorName()
        {
            return "CCOL";
        }

        public string GetGeneratorVersion()
        {
            return Controller?.Data.TLCGenVersie ?? "0";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }

        [Browsable(false)]
        public ControllerModel Controller
        {
            get;
            set;
        }

        public void GenerateController()
        {
            if(_myVm.GenerateCodeCommand.CanExecute(null))
            {
                _myVm.GenerateCodeCommand.Execute(null);
            }
        }

        public bool CanGenerateController()
        {
            return _myVm.GenerateCodeCommand.CanExecute(null);
        }

        #endregion // ITLCGenGenerator

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerFileNameChangedMessage>(OnControllerFileNameChanged));
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerDataChangedMessage>(OnControllerDataChanged));
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenHasSettings

        public void LoadSettings()
        {
            CCOLGeneratorSettingsProvider.Default.Settings =
                TLCGenSerialization.DeSerializeData<CCOLGeneratorSettingsModel>(
                    ResourceReader.GetResourceTextFile("TLCGen.Generators.CCOL.Settings.ccolgendefaults.xml", this));

            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"ccolgensettings.xml");
	        
            // read custom settings and overwrite defaults
            if (File.Exists(setfile))
            {
                var userSettings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(setfile);

                // always overwrite visual settings
                var userVisualSettings = userSettings.AllVisualSettings; 
                var currVisualSettings = CCOLGeneratorSettingsProvider.Default.Settings.AllVisualSettings;

                var max = Enum.GetValues(typeof(CCOLVersieEnum)).Cast<int>().Max();
                for (int i = 0; i <= max; i++)
                {
                    currVisualSettings[i].CCOLIncludesPaden = userVisualSettings[i].CCOLIncludesPaden;
                    currVisualSettings[i].CCOLLibs = userVisualSettings[i].CCOLLibs;
                    currVisualSettings[i].CCOLLibsPath = userVisualSettings[i].CCOLLibsPath;
                    currVisualSettings[i].CCOLLibsPathNoTig = userVisualSettings[i].CCOLLibsPathNoTig;
                    currVisualSettings[i].CCOLPreprocessorDefinitions = userVisualSettings[i].CCOLPreprocessorDefinitions;
                    currVisualSettings[i].CCOLResPath = userVisualSettings[i].CCOLResPath;
                }
                
                // always overwrite visual tabspace and others
                CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating = userSettings.AlterAddHeadersWhileGenerating;
				CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating = userSettings.AlterAddFunctionsWhileGenerating;
                CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources = userSettings.AlwaysOverwriteSources;
                CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods = userSettings.ReplaceRepeatingCommentsTextWithPeriods;
                CCOLGeneratorSettingsProvider.Default.Settings.TabSpace = userSettings.TabSpace;

                // prefixes: overwrite where needed
                if (userSettings.Prefixes.Any())
                {
                    foreach (var pf in userSettings.Prefixes)
                    {
                        var pf2 = CCOLGeneratorSettingsProvider.Default.Settings.Prefixes.FirstOrDefault(x => pf.Default == x.Default);
                        if (pf2 != null) pf2.Setting = pf.Setting;
                    }
                }

                // element name settings: overwrite where needed
                if (userSettings.CodePieceGeneratorSettings.Any())
                {
                    foreach (var cpg in userSettings.CodePieceGeneratorSettings)
                    {
                        foreach (var cpg2 in CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings)
                        {
                            if (cpg.Item1 == cpg2.Item1)
                            {
                                foreach(var set in cpg.Item2.Settings)
                                {
                                    foreach (var set2 in cpg2.Item2.Settings)
                                    {
                                        if (set.Type == set2.Type && set.Default == set2.Default)
                                        {
                                            set2.Setting = set.Setting;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            CCOLGeneratorSettingsProvider.Default.Reset();
            _generator.LoadSettings();

            if (!_noView)
            {

                if (_alwaysOverwriteSourcesMenuItem == null)
                {
                    _alwaysOverwriteSourcesMenuItem = new MenuItem
                    {
                        Header = "Altijd overschrijven bronbestanden"
                    };
                }

                _alwaysOverwriteSourcesMenuItem.IsChecked =
                    CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources;

                if (_alterAddHeadersWhileGeneratingMenuItem == null)
                {
                    _alterAddHeadersWhileGeneratingMenuItem = new MenuItem
                    {
                        Header = "Bijwerken add headers tijdens genereren"
                    };
                }

                _alterAddHeadersWhileGeneratingMenuItem.IsChecked =
                    CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating;

                if (_alterAddFunctionsWhileGeneratingMenuItem == null)
                {
                    _alterAddFunctionsWhileGeneratingMenuItem = new MenuItem
                    {
                        Header = "Bijwerken functies in add files tijdens genereren"
                    };
                }

                _alterAddFunctionsWhileGeneratingMenuItem.IsChecked =
                    CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating;

                if (_replaceRepeatingCommentsTextWithPeriodsMenuItem == null)
                {
                    _replaceRepeatingCommentsTextWithPeriodsMenuItem = new MenuItem
                    {
                        Header = "Herhalend commentaar vervangen door ..."
                    };
                }

                _replaceRepeatingCommentsTextWithPeriodsMenuItem.IsChecked =
                    CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods;
            }
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"ccolgensettings.xml");

            var settings = new CCOLGeneratorSettingsModel();

            // always save visual settings
            var saveVisualSettings = settings.AllVisualSettings; 
            var currVisualSettings = CCOLGeneratorSettingsProvider.Default.Settings.AllVisualSettings;

            var max = Enum.GetValues(typeof(CCOLVersieEnum)).Cast<int>().Max();
            for (int i = 0; i <= max; i++)
            {
                saveVisualSettings[i].CCOLIncludesPaden = currVisualSettings[i].CCOLIncludesPaden;
                saveVisualSettings[i].CCOLLibs = currVisualSettings[i].CCOLLibs;
                saveVisualSettings[i].CCOLLibsPath = currVisualSettings[i].CCOLLibsPath;
                saveVisualSettings[i].CCOLLibsPathNoTig = currVisualSettings[i].CCOLLibsPathNoTig;
                saveVisualSettings[i].CCOLPreprocessorDefinitions = currVisualSettings[i].CCOLPreprocessorDefinitions;
                saveVisualSettings[i].CCOLResPath = currVisualSettings[i].CCOLResPath;
            }
            
            // always save visual tabspace and others
            settings.AlterAddHeadersWhileGenerating = CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating;
            settings.AlterAddFunctionsWhileGenerating = CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating;
            settings.AlwaysOverwriteSources = CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources;
            settings.ReplaceRepeatingCommentsTextWithPeriods = CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods;
            settings.TabSpace = CCOLGeneratorSettingsProvider.Default.Settings.TabSpace;

            // save prefixes where needed
            foreach(var pf in CCOLGeneratorSettingsProvider.Default.Settings.Prefixes)
            {
                if(pf.Setting != pf.Default)
                {
                    settings.Prefixes.Add(new CCOLGeneratorCodeStringSettingModel
                    {
                        Type = CCOLGeneratorSettingTypeEnum.Prefix,
                        Description = pf.Description,
                        Default = pf.Default,
                        Setting = pf.Setting
                    });
                }
            }

            // save element naming where needed
            foreach (var cpg in CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings)
            {
                var ncpg = new CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>
                {
                    Item1 = cpg.Item1,
                    Item2 = new CCOLGeneratorClassWithSettingsModel()
                };
                ncpg.Item2.ClassName = cpg.Item2.ClassName;
                ncpg.Item2.Description = cpg.Item2.Description;
                ncpg.Item2.Settings = new List<CCOLGeneratorCodeStringSettingModel>();

                foreach(var s in cpg.Item2.Settings)
                {
                    if(s.Setting != s.Default)
                    {
                        ncpg.Item2.Settings.Add(new CCOLGeneratorCodeStringSettingModel
                        {
                            Type = s.Type,
                            Default = s.Default,
                            Description = s.Description,
                            Setting = s.Setting
                        });
                    }
                }
                if (ncpg.Item2.Settings.Any())
                {
                    settings.CodePieceGeneratorSettings.Add(ncpg);
                }
            }

            TLCGenSerialization.Serialize(setfile, settings);
        }

		#endregion // ITLCGenHasSettings

        #region ITLCGenMenuItem

        private MenuItem _alwaysOverwriteSourcesMenuItem;
        private MenuItem _alterAddHeadersWhileGeneratingMenuItem;
        private MenuItem _alterAddFunctionsWhileGeneratingMenuItem;
        private MenuItem _resetSettingsMenuItem;
        private MenuItem _replaceRepeatingCommentsTextWithPeriodsMenuItem;
        private MenuItem _pluginMenuItem;

        public MenuItem Menu
        {
            get
            {
                if (_pluginMenuItem == null)
                {
                    _pluginMenuItem = new MenuItem
                    {
                        Header = "CCOL code generator"
                    };
                    var sitem1 = new MenuItem
                    {
                        Header = "CCOL code generator instellingen",
                        Command = ShowSettingsCommand
                    };
                    if (_alwaysOverwriteSourcesMenuItem == null)
                    {
                        _alwaysOverwriteSourcesMenuItem = new MenuItem
                        {
                            Header = "Altijd overschrijven bronbestanden"
                        };
                    }
                    if (_alterAddHeadersWhileGeneratingMenuItem == null)
                    {
                        _alterAddHeadersWhileGeneratingMenuItem = new MenuItem
                        {
                            Header = "Bijwerken add headers tijdens genereren"
                        };
                    }
                    if (_alterAddFunctionsWhileGeneratingMenuItem == null)
                    {
                        _alterAddFunctionsWhileGeneratingMenuItem = new MenuItem
                        {
                            Header = "Bijwerken code in add files tijdens genereren"
                        };
                    }
                    if (_resetSettingsMenuItem == null)
	                {
		                _resetSettingsMenuItem = new MenuItem
		                {
			                Header = "Reset CCOL generator instellingen"
		                };
	                }
                    if (_replaceRepeatingCommentsTextWithPeriodsMenuItem == null)
                    {
                        _replaceRepeatingCommentsTextWithPeriodsMenuItem = new MenuItem
                        {
                            Header = "Herhalend commentaar vervangen door ..."
                        };
                    }
					_alwaysOverwriteSourcesMenuItem.Click += (o, e) =>
                    {
                        CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources =
                            !CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources;
                        _alwaysOverwriteSourcesMenuItem.IsChecked =
                            CCOLGeneratorSettingsProvider.Default.Settings.AlwaysOverwriteSources;
                    };
                    _alterAddHeadersWhileGeneratingMenuItem.Click += (o, e) =>
                    {
                        CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating =
                            !CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating;
                        _alterAddHeadersWhileGeneratingMenuItem.IsChecked =
                            CCOLGeneratorSettingsProvider.Default.Settings.AlterAddHeadersWhileGenerating;
                    };
                    _alterAddFunctionsWhileGeneratingMenuItem.Click += (o, e) =>
                    {
                        CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating =
                            !CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating;
                        _alterAddFunctionsWhileGeneratingMenuItem.IsChecked =
                            CCOLGeneratorSettingsProvider.Default.Settings.AlterAddFunctionsWhileGenerating;
                    };
                    _resetSettingsMenuItem.Click += (o, e) =>
	                {
						CCOLGeneratorSettingsProvider.Default.Settings =
                            TLCGenSerialization.DeSerializeData<CCOLGeneratorSettingsModel>(
                                ResourceReader.GetResourceTextFile("TLCGen.Generators.CCOL.Settings.ccolgendefaults.xml", this));
                    };
                    _replaceRepeatingCommentsTextWithPeriodsMenuItem.Click += (o, e) =>
                    {
                        CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods =
                            !CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods;
                        _replaceRepeatingCommentsTextWithPeriodsMenuItem.IsChecked =
                            CCOLGeneratorSettingsProvider.Default.Settings.ReplaceRepeatingCommentsTextWithPeriods;
                    };
					_pluginMenuItem.Items.Add(sitem1);
                    _pluginMenuItem.Items.Add(_alwaysOverwriteSourcesMenuItem);
                    _pluginMenuItem.Items.Add(_alterAddHeadersWhileGeneratingMenuItem);
                    _pluginMenuItem.Items.Add(_alterAddFunctionsWhileGeneratingMenuItem);
                    _pluginMenuItem.Items.Add(_replaceRepeatingCommentsTextWithPeriodsMenuItem);
                }
                return _pluginMenuItem;
            }
        }

        #endregion ITLCGenMenuItem

        #region Properties

        [Browsable(false)]
        public string ControllerFileName { get; set; }

        #endregion // Properties

        #region Fields

        private readonly CCOLGeneratorViewModel _myVm;

        #endregion // Fields

        #region Commands

        RelayCommand _showSettingsCommand;
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
            var w = new CCOLGeneratorSettingsView
            {
                DataContext =
                    new CCOLGeneratorSettingsViewModel(CCOLGeneratorSettingsProvider.Default.Settings, _generator)
            };
            var window = new Window
            {
                Title = "CCOL Code Generator instellingen",
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

        #region Static Public Methods

        public static string GetVersion()
        {
            return _noView ? "UnitTest" : Assembly.GetEntryAssembly()?.GetName().Version.ToString();
        }

        #endregion // Static Public Methods

        #region TLCGen Events

        private void OnControllerFileNameChanged(Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            if (msg.NewFileName == null) return;

            ControllerFileName = msg.NewFileName;
        }

        private void OnControllerDataChanged(Messaging.Messages.ControllerDataChangedMessage msg)
        {
        }

        #endregion // TLCGen Events

        #region Constructor

        public CCOLCodeGeneratorPlugin() : this(false)
        {
        }

        public CCOLCodeGeneratorPlugin(bool noView = false)
        {
            _noView = noView;
            _generator = new CCOLGenerator();
            _myVm = new CCOLGeneratorViewModel(this, _generator);
            if (!noView)
            {
                GeneratorView = new CCOLGeneratorView {DataContext = _myVm};
            }

	        var filesDef = Directory.GetFiles(
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\VisualTemplates"),
				"*.xml", SearchOption.TopDirectoryOnly);

	        foreach (var t in filesDef)
	        {
		        if (!t.ToLower().EndsWith("_filters.xml"))
		        {
					_myVm.VisualProjects.Add(Path.GetFileNameWithoutExtension(t).Replace("_", " "));
				}
	        }

            _myVm.SelectedVisualProject = _myVm.VisualProjects.FirstOrDefault(x => x.StartsWith("Visual 2017"));
        }

        #endregion // Constructor
    }
}
