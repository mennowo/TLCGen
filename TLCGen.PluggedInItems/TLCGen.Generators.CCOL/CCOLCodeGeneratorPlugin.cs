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
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator | TLCGenPluginElems.HasSettings | TLCGenPluginElems.MenuControl | TLCGenPluginElems.PlugMessaging)]
    public class CCOLCodeGeneratorPlugin : ITLCGenGenerator, ITLCGenHasSettings, ITLCGenMenuItem, ITLCGenPlugMessaging
    {
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
            return "0.7.1.0";
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
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLIncludesPaden = userSettings.VisualSettings.CCOLIncludesPaden;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibs = userSettings.VisualSettings.CCOLLibs;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibsPath = userSettings.VisualSettings.CCOLLibsPath;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLPreprocessorDefinitions = userSettings.VisualSettings.CCOLPreprocessorDefinitions;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLResPath = userSettings.VisualSettings.CCOLResPath;

                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLIncludesPaden = userSettings.VisualSettingsCCOL9.CCOLIncludesPaden;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLLibs = userSettings.VisualSettingsCCOL9.CCOLLibs;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLLibsPath = userSettings.VisualSettingsCCOL9.CCOLLibsPath;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLPreprocessorDefinitions = userSettings.VisualSettingsCCOL9.CCOLPreprocessorDefinitions;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLResPath = userSettings.VisualSettingsCCOL9.CCOLResPath;

                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLIncludesPaden = userSettings.VisualSettingsCCOL95.CCOLIncludesPaden;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibs = userSettings.VisualSettingsCCOL95.CCOLLibs;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibsPath = userSettings.VisualSettingsCCOL95.CCOLLibsPath;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibsPathNoTig = userSettings.VisualSettingsCCOL95.CCOLLibsPathNoTig;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLPreprocessorDefinitions = userSettings.VisualSettingsCCOL95.CCOLPreprocessorDefinitions;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLResPath = userSettings.VisualSettingsCCOL95.CCOLResPath;

                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLIncludesPaden = userSettings.VisualSettingsCCOL100.CCOLIncludesPaden;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibs = userSettings.VisualSettingsCCOL100.CCOLLibs;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibsPath = userSettings.VisualSettingsCCOL100.CCOLLibsPath;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibsPathNoTig = userSettings.VisualSettingsCCOL100.CCOLLibsPathNoTig;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLPreprocessorDefinitions = userSettings.VisualSettingsCCOL100.CCOLPreprocessorDefinitions;
                CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLResPath = userSettings.VisualSettingsCCOL100.CCOLResPath;

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
                        foreach (var pf2 in CCOLGeneratorSettingsProvider.Default.Settings.Prefixes)
                        {
                            if (pf.Default == pf2.Default)
                            {
                                pf2.Setting = pf.Setting;
                                break;
                            }
                        }
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
                                        if (set.Default == set2.Default)
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

            _generator.LoadSettings();

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

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"ccolgensettings.xml");

            var settings = new CCOLGeneratorSettingsModel();

            // always save visual settings
            settings.VisualSettings.CCOLIncludesPaden = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLIncludesPaden;
            settings.VisualSettings.CCOLLibs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibs;
            settings.VisualSettings.CCOLLibsPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibsPath;
            settings.VisualSettings.CCOLPreprocessorDefinitions = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLPreprocessorDefinitions;
            settings.VisualSettings.CCOLResPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLResPath;

            settings.VisualSettingsCCOL9.CCOLIncludesPaden = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLIncludesPaden;
            settings.VisualSettingsCCOL9.CCOLLibs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLLibs;
            settings.VisualSettingsCCOL9.CCOLLibsPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLLibsPath;
            settings.VisualSettingsCCOL9.CCOLPreprocessorDefinitions = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLPreprocessorDefinitions;
            settings.VisualSettingsCCOL9.CCOLResPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9.CCOLResPath;

            settings.VisualSettingsCCOL95.CCOLIncludesPaden = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLIncludesPaden;
            settings.VisualSettingsCCOL95.CCOLLibs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibs;
            settings.VisualSettingsCCOL95.CCOLLibsPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibsPath;
            settings.VisualSettingsCCOL95.CCOLLibsPathNoTig = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLLibsPathNoTig;
            settings.VisualSettingsCCOL95.CCOLPreprocessorDefinitions = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLPreprocessorDefinitions;
            settings.VisualSettingsCCOL95.CCOLResPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95.CCOLResPath;

            settings.VisualSettingsCCOL100.CCOLIncludesPaden = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLIncludesPaden;
            settings.VisualSettingsCCOL100.CCOLLibs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibs;
            settings.VisualSettingsCCOL100.CCOLLibsPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibsPath;
            settings.VisualSettingsCCOL100.CCOLLibsPathNoTig = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLLibsPathNoTig;
            settings.VisualSettingsCCOL100.CCOLPreprocessorDefinitions = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLPreprocessorDefinitions;
            settings.VisualSettingsCCOL100.CCOLResPath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100.CCOLResPath;
            
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
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        #endregion // Static Public Methods

        #region TLCGen Events

        private void OnControllerFileNameChanged(Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            ControllerFileName = msg.NewFileName;
        }

        private void OnControllerDataChanged(Messaging.Messages.ControllerDataChangedMessage msg)
        {
        }

        #endregion // TLCGen Events

        #region Constructor

        public CCOLCodeGeneratorPlugin()
        {
            GeneratorView = new CCOLGeneratorView();
            _generator = new CCOLGenerator();
            _myVm = new CCOLGeneratorViewModel(this, _generator);
            GeneratorView.DataContext = _myVm;

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
