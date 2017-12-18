using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        private UserControl _GeneratorView;
        private CCOLGenerator _Generator;

        [Browsable(false)]
        public UserControl GeneratorView
        {
            get
            {
                return _GeneratorView;
            }
        }

        public string GetGeneratorName()
        {
            return "CCOL";
        }

        public string GetGeneratorVersion()
        {
            return "0.11 (alfa)";
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
            if(this._MyVM.GenerateCodeCommand.CanExecute(null))
            {
                this._MyVM.GenerateCodeCommand.Execute(null);
            }
        }

        public bool CanGenerateController()
        {
            return this._MyVM.GenerateCodeCommand.CanExecute(null);
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
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"ccolgensettings.xml");
	        if (File.Exists(setfile))
            {
                CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(setfile);
	            var defsetfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\ccolgendefaults.xml");
				if (File.Exists(defsetfile))
				{
					var corrected = false;
					var updated = false;
					var defaultSettings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(defsetfile);
					foreach (var def in defaultSettings.CodePieceGeneratorSettings)
					{
						var found = false;
						foreach (var def2 in CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings)
						{
							if (def.Item1 == def2.Item1)
							{
								found = true;
								if (def.Item2.ClassName != def2.Item2.ClassName)
								{
									def2.Item2.ClassName = def.Item2.ClassName;
									corrected = true;
								}
								foreach (var s in def.Item2.Settings)
								{
									bool foundSet = false;
									foreach (var s2 in def2.Item2.Settings)
									{
										if (s.Type == s2.Type &&
										    s.Default == s2.Default)
										{
											foundSet = true;
										}
									}
									if (!foundSet)
									{
										def2.Item2.Settings.Add(s);
										updated = true;
									}
								}
								var remSs = new List<CCOLGeneratorCodeStringSettingModel>();
								foreach (var s in def2.Item2.Settings)
								{
									bool foundSet = false;
									foreach (var s2 in def.Item2.Settings)
									{
										if (s.Type == s2.Type &&
										    s.Default == s2.Default)
										{
											foundSet = true;
										}
									}
									if (!foundSet)
									{
										updated = true;
										remSs.Add(s);
									}
								}
								foreach (var s in remSs)
								{
									def2.Item2.Settings.Remove(s);
								}
							}
						}
						if (!found)
						{
							CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings.Add(def);
							updated = false;
						}
					}
					var remDs = new List<CodePieceSettingsTuple<string, CCOLGeneratorClassWithSettingsModel>>();
					foreach (var d in CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings)
					{
						bool found = false;
						foreach (var d2 in defaultSettings.CodePieceGeneratorSettings)
						{
							if (d.Item1 == d2.Item1)
							{
								found = true;
							}
						}
						if (!found)
						{
							updated = true;
							remDs.Add(d);
						}
					}
					foreach (var d in remDs)
					{
						CCOLGeneratorSettingsProvider.Default.Settings.CodePieceGeneratorSettings.Remove(d);
					}
					if (updated) MessageBox.Show("CCOL defaults updated", "CCOL defaults updated");
					if (corrected) MessageBox.Show("CCOL defaults corrected", "CCOL defaults corrected");
				}
			}
            else
            {
                CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\ccolgendefaults.xml"));
            }
            _Generator.LoadSettings();

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
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"ccolgensettings.xml");
            TLCGenSerialization.Serialize<CCOLGeneratorSettingsModel>(setfile, CCOLGeneratorSettingsProvider.Default.Settings);
        }

#endregion // ITLCGenHasSettings

        #region ITLCGenMenuItem

        private MenuItem _alwaysOverwriteSourcesMenuItem;
        private MenuItem _alterAddHeadersWhileGeneratingMenuItem;
        private MenuItem _resetSettingsMenuItem;
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
	                if (_resetSettingsMenuItem == null)
	                {
		                _resetSettingsMenuItem = new MenuItem
		                {
			                Header = "Reset CCOL generator instellingen"
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
	                _resetSettingsMenuItem.Click += (o, e) =>
	                {
						CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\ccolgendefaults.xml"));
					};
					_pluginMenuItem.Items.Add(sitem1);
                    _pluginMenuItem.Items.Add(_alwaysOverwriteSourcesMenuItem);
                    _pluginMenuItem.Items.Add(_alterAddHeadersWhileGeneratingMenuItem);
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

        private CCOLGeneratorViewModel _MyVM;

        #endregion // Fields

        #region Commands

        RelayCommand _ShowSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get
            {
                if (_ShowSettingsCommand == null)
                {
                    _ShowSettingsCommand = new RelayCommand(ShowSettingsCommand_Executed, ShowSettingsCommand_CanExecute);
                }
                return _ShowSettingsCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void ShowSettingsCommand_Executed(object obj)
        {
            var w = new CCOLGeneratorSettingsView();
            w.DataContext = new CCOLGeneratorSettingsViewModel(CCOLGeneratorSettingsProvider.Default.Settings, _Generator);
            Window window = new Window
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

        private void OnControllerFileNameChanged(TLCGen.Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            ControllerFileName = msg.NewFileName;
        }

        private void OnControllerDataChanged(TLCGen.Messaging.Messages.ControllerDataChangedMessage msg)
        {
        }

        #endregion // TLCGen Events

        #region Constructor

        public CCOLCodeGeneratorPlugin()
        {
            _GeneratorView = new CCOLGeneratorView();
            _Generator = new CCOLGenerator();
            _MyVM = new CCOLGeneratorViewModel(this, _Generator);
            _GeneratorView.DataContext = _MyVM;

	        var filesDef = Directory.GetFiles(
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\VisualTemplates"),
				"*.xml", SearchOption.TopDirectoryOnly);

	        foreach (var t in filesDef)
	        {
		        if (!t.ToLower().EndsWith("_filters.xml"))
		        {
					_MyVM.VisualProjects.Add(Path.GetFileNameWithoutExtension(t).Replace("_", " "));
				}
	        }
        }

        #endregion // Constructor
    }
}
