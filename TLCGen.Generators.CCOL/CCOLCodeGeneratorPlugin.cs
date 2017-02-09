using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.CustomPropertyEditors;
using TLCGen.DataAccess;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL
{
    [TLCGenPlugin(TLCGenPluginElems.Generator | TLCGenPluginElems.HasSettings | TLCGenPluginElems.MenuControl)]
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
            var setfile = Path.Combine(setpath, @"settings.xml");
#if DEBUG
            CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\defaults.xml"));
#else
            if (File.Exists(setfile))
            {
                CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(setfile);
            }
            else
            {
                CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerialize<CCOLGeneratorSettingsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\defaults.xml"));
            }
#endif
            _Generator.LoadSettings();
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\CCOLGeneratorSettings\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");
            TLCGenSerialization.Serialize<CCOLGeneratorSettingsModel>(setfile, CCOLGeneratorSettingsProvider.Default.Settings);
        }

#endregion // ITLCGenHasSettings

        #region ITLCGenMenuItem

        public MenuItem Menu
        {
            get
            {
                MenuItem item = new MenuItem();
                item.Header = "CCOL instellingen";
                item.Command = ShowSettingsCommand;
                return item;
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
            return "0.11 (alfa)";
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
        }

        #endregion // Constructor
    }
}
