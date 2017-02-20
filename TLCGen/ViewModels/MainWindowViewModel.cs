using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private TLCGenSettingsViewModel _SettingsVM;


        private List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> _ApplicationParts;

        private List<MenuItem> _ImportMenuItems;
        private List<MenuItem> _PluginMenuItems;
        private List<IGeneratorViewModel> _Generators;
        
        private IGeneratorViewModel _SelectedGenerator;

        #endregion // Fields

        #region Properties

        public List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationParts
        {
            get { return _ApplicationParts; }
        }

        /// <summary>
        /// ViewModel for the Controller data object that is being edited via the application.
        /// This is the main ViewModel for the application, which holds all other ViewModels.
        /// (Except for the Settings ViewModel, which belongs to the application rather than 
        /// the Controller.)
        /// </summary>
        public ControllerViewModel ControllerVM
        {
            get { return _ControllerVM; }
            set
            {
                _ControllerVM = value;
                foreach(var pl in ApplicationParts)
                {
                    pl.Item2.Controller = TLCGenControllerDataProvider.Default.Controller;
                }
                
                OnPropertyChanged("ControllerVM");
                OnPropertyChanged("HasController");
            }
        }

        /// <summary>
        /// Boolean used by the View to determine of a Controller has been loaded.
        /// This is used in the View to enable/disable appropriate UI elementents.
        /// </summary>
        public bool HasController
        {
            get { return TLCGenControllerDataProvider.Default.Controller != null; }
        }

        /// <summary>
        /// ViewModel for the settings of the application
        /// </summary>
        //public TLCGenSettingsViewModel SettingsVM
        //{
        //    get
        //    {
        //        if(_SettingsVM == null)
        //        {
        //            _SettingsVM = new TLCGenSettingsViewModel();
        //        }
        //        return _SettingsVM;
        //    }
        //}

        /// <summary>
        /// A string to be used in the View as the title of the program.
        /// File operations should call OnPropertyChanged for this property,
        /// so the View updates the program title.
        /// </summary>
        public string ProgramTitle
        {
            get
            {
                if(HasController && !string.IsNullOrEmpty(TLCGenControllerDataProvider.Default.ControllerFileName))
                    return "TLCGen - " + TLCGenControllerDataProvider.Default.ControllerFileName;
                else
                    return "TLCGen";
            }
        }
        
        public List<IGeneratorViewModel> Generators
        {
            get
            {
                if(_Generators == null)
                {
                    _Generators = new List<IGeneratorViewModel>();
                }
                return _Generators;
            }
        }

        private List<ITLCGenImporter> _Importers;
        public List<ITLCGenImporter> Importers
        {
            get
            {
                if(_Importers == null)
                {
                    _Importers = new List<ITLCGenImporter>();
                }
                return _Importers;
            }
        }

        private List<ITLCGenTabItem> _TabItems;
        public List<ITLCGenTabItem> TabItems
        {
            get
            {
                if(_TabItems == null)
                {
                    _TabItems = new List<ITLCGenTabItem>();
                }
                return _TabItems;
            }
        }

        /// <summary>
        /// Holds the selected code generator, on which the appropriate function calls are invoked
        /// when commands relating to generators are executed.
        /// </summary>
        public IGeneratorViewModel SelectedGenerator
        {
            get { return _SelectedGenerator; }
            set
            {
                _SelectedGenerator = value;
                OnPropertyChanged("SelectedGenerator");
            }
        }

        /// <summary>
        /// Holds a list of available menu items that are bound to the View. This allows the user
        /// to click a menu item to instruct an importer to import data.
        /// </summary>
        public List<MenuItem> ImportMenuItems
        {
            get
            {
                if(_ImportMenuItems == null)
                {
                    _ImportMenuItems = new List<MenuItem>();
                }
                return _ImportMenuItems;
            }
        }

        public List<MenuItem> PluginMenuItems
        {
            get
            {
                if (_PluginMenuItems == null)
                {
                    _PluginMenuItems = new List<MenuItem>();
                }
                return _PluginMenuItems;
            }
        }

        public bool IsPluginMenuVisible
        {
            get
            {
                return _PluginMenuItems?.Count > 0;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _NewFileCommand;
        RelayCommand _OpenFileCommand;
        RelayCommand _SaveFileCommand;
        RelayCommand _SaveAsFileCommand;
        RelayCommand _CloseFileCommand;
        RelayCommand _ExitApplicationCommand;
        RelayCommand _ShowSettingsWindowCommand;
        RelayCommand _ShowAboutCommand;

        public ICommand NewFileCommand
        {
            get
            {
                if (_NewFileCommand == null)
                {
                    _NewFileCommand = new RelayCommand(NewFileCommand_Executed, NewFileCommand_CanExecute);
                }
                return _NewFileCommand;
            }
        }

        public ICommand OpenFileCommand
        {
            get
            {
                if (_OpenFileCommand == null)
                {
                    _OpenFileCommand = new RelayCommand(OpenFileCommand_Executed, OpenFileCommand_CanExecute);
                }
                return _OpenFileCommand;
            }
        }

        public ICommand SaveFileCommand
        {
            get
            {
                if (_SaveFileCommand == null)
                {
                    _SaveFileCommand = new RelayCommand(SaveFileCommand_Executed, SaveFileCommand_CanExecute);
                }
                return _SaveFileCommand;
            }
        }

        public ICommand SaveAsFileCommand
        {
            get
            {
                if (_SaveAsFileCommand == null)
                {
                    _SaveAsFileCommand = new RelayCommand(SaveAsFileCommand_Executed, SaveAsFileCommand_CanExecute);
                }
                return _SaveAsFileCommand;
            }
        }

        public ICommand CloseFileCommand
        {
            get
            {
                if (_CloseFileCommand == null)
                {
                    _CloseFileCommand = new RelayCommand(CloseFileCommand_Executed, CloseFileCommand_CanExecute);
                }
                return _CloseFileCommand;
            }
        }

        public ICommand ExitApplicationCommand
        {
            get
            {
                if (_ExitApplicationCommand == null)
                {
                    _ExitApplicationCommand = new RelayCommand(ExitApplicationCommand_Executed, ExitApplicationCommand_CanExecute);
                }
                return _ExitApplicationCommand;
            }
        }


        RelayCommand _GenerateControllerCommand;
        public ICommand GenerateControllerCommand
        {
            get
            {
                if (_GenerateControllerCommand == null)
                {
                    _GenerateControllerCommand = new RelayCommand(GenerateControllerCommand_Executed, GenerateControllerCommand_CanExecute);
                }
                return _GenerateControllerCommand;
            }
        }

        RelayCommand _ImportControllerCommand;
        public ICommand ImportControllerCommand
        {
            get
            {
                if (_ImportControllerCommand == null)
                {
                    _ImportControllerCommand = new RelayCommand(ImportControllerCommand_Executed, ImportControllerCommand_CanExecute);
                }
                return _ImportControllerCommand;
            }
        }

        public ICommand ShowSettingsWindowCommand
        {
            get
            {
                if (_ShowSettingsWindowCommand == null)
                {
                    _ShowSettingsWindowCommand = new RelayCommand(ShowSettingsWindowCommand_Executed, null);
                }
                return _ShowSettingsWindowCommand;
            }
        }

        public ICommand ShowAboutCommand
        {
            get
            {
                if (_ShowAboutCommand == null)
                {
                    _ShowAboutCommand = new RelayCommand(ShowAboutCommand_Executed, null);
                }
                return _ShowAboutCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private void NewFileCommand_Executed(object prm)
        {
            if (TLCGenControllerDataProvider.Default.NewController())
            {
                string lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
                TLCGenControllerDataProvider.Default.NewController();
                ControllerVM.Controller = TLCGenControllerDataProvider.Default.Controller;
                DefaultsProvider.Default.Controller = TLCGenControllerDataProvider.Default.Controller;
                ControllerVM.SelectedTabIndex = 0;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                OnPropertyChanged("ProgramTitle");
                OnPropertyChanged("HasController");
            }
        }

        private bool NewFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void OpenFileCommand_Executed(object prm)
        {
            if(TLCGenControllerDataProvider.Default.OpenController())
            {
                string lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
                ControllerVM.Controller = TLCGenControllerDataProvider.Default.Controller;
                DefaultsProvider.Default.Controller = TLCGenControllerDataProvider.Default.Controller;
#warning: move the below logic to the data provider somehow!
                //ControllerVM.LoadPluginDataFromXmlDocument(TLCGenControllerDataProvider.Default.ControllerXml);
                ControllerVM.SelectedTabIndex = 0;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                OnPropertyChanged("ProgramTitle");
                OnPropertyChanged("HasController");
            }
        }

        private bool OpenFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void SaveFileCommand_Executed(object prm)
        {
            if (TLCGenControllerDataProvider.Default.SaveController())
            {
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        private bool SaveFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null &&
                   TLCGenControllerDataProvider.Default.ControllerHasChanged;
        }

        private void SaveAsFileCommand_Executed(object prm)
        {
            string lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
            if(TLCGenControllerDataProvider.Default.SaveControllerAs())
            {
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                OnPropertyChanged("ProgramTitle");
            }
        }

        private bool SaveAsFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null;
        }

        private void CloseFileCommand_Executed(object prm)
        {
            string lastfilename = TLCGenControllerDataProvider.Default.ControllerFileName;
            if(TLCGenControllerDataProvider.Default.CloseController())
            {
                DefaultsProvider.Default.Controller = null;
                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, lastfilename));
                OnPropertyChanged("HasController");
                OnPropertyChanged("ProgramTitle");
            }
        }

        private bool CloseFileCommand_CanExecute(object prm)
        {
            return TLCGenControllerDataProvider.Default.Controller != null;
        }

        private void ExitApplicationCommand_Executed(object prm)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool ExitApplicationCommand_CanExecute(object prm)
        {
            return true;
        }

        private void ImportControllerCommand_Executed(object obj)
        {
            if (obj == null)
                throw new NotImplementedException();
            ITLCGenImporter imp = obj as ITLCGenImporter;
            if (imp == null)
                throw new NotImplementedException();

            // Import into existing controller
            if (!TLCGenControllerDataProvider.Default.CheckChanged())
            {
                if (imp.ImportsIntoExisting)
                {
                    // Check data integrity
                    string s1 = IntegrityChecker.IsControllerDataOK(ControllerVM.Controller);
                    if (s1 != null)
                    {
                        System.Windows.MessageBox.Show("Kan niet importeren:\n\n" + s1, "Error bij importeren: fout in regeling");
                        return;
                    }
                    // Import to clone of original (so we can discard if wrong)
                    ControllerModel c1 = DeepCloner.DeepClone(ControllerVM.Controller);
                    ControllerModel c2 = imp.ImportController(c1);

                    // Do nothing if the importer returned nothing
                    if (c2 == null)
                        return;

                    // Check data integrity
                    s1 = IntegrityChecker.IsControllerDataOK(c2);
                    if (s1 != null)
                    {
                        System.Windows.MessageBox.Show("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data");
                        return;
                    }
                    SetController(c2);
                    ControllerVM.ReloadController();
                }
                // Import as new controller
                else
                {
                    ControllerModel c1 = imp.ImportController();

                    // Do nothing if the importer returned nothing
                    if (c1 == null)
                        return;

                    // Check data integrity
                    string s1 = IntegrityChecker.IsControllerDataOK(c1);
                    if (s1 != null)
                    {
                        System.Windows.MessageBox.Show("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data");
                        return;
                    }
                    TLCGenControllerDataProvider.Default.CloseController();
                    SetController(c1);
                    ControllerVM.ReloadController();
                }
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        private bool GenerateControllerCommand_CanExecute(object obj)
        {
            return SelectedGenerator != null && SelectedGenerator.Generator != null && SelectedGenerator.Generator.CanGenerateController();
        }

        private void GenerateControllerCommand_Executed(object obj)
        {
            SelectedGenerator.Generator.GenerateController();
        }

        private bool ImportControllerCommand_CanExecute(object obj)
        {
            if (obj == null)
                return false;

            ITLCGenImporter imp = obj as ITLCGenImporter;
            if (imp == null)
                throw new NotImplementedException();

            if (imp.ImportsIntoExisting)
                return TLCGenControllerDataProvider.Default.Controller != null;

            return true;
        }

        private void ShowSettingsWindowCommand_Executed(object obj)
        {
            Settings.Views.TLCGenSettingsWindow settingswin = new Settings.Views.TLCGenSettingsWindow();
            settingswin.DataContext = new TLCGenSettingsViewModel();
            settingswin.ShowDialog();
        }

        private void ShowAboutCommand_Executed(object obj)
        {
            TLCGen.Views.AboutWindow about = new Views.AboutWindow();
            about.ShowDialog();
        }

        #endregion // Command functionality

        #region Private methods

        /// <summary>
        /// Called before the application is closed. This allows to save data and settings
        /// </summary>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (TLCGenControllerDataProvider.Default.CheckChanged())
            {
                e.Cancel = true;
            }
            else
            {
                foreach (var pl in _ApplicationParts)
                {
                    var setpl = pl.Item2 as ITLCGenHasSettings;
                    if (setpl != null)
                    {
                        setpl.SaveSettings();
                    }
                }

                SaveGeneratorControllerSettingsToModel();
                SettingsProvider.Default.SaveApplicationSettings();
                DefaultsProvider.Default.SaveSettings();
                TemplatesProvider.Default.SaveSettings();
            }
        }

#warning Make this generic, just like how settings are loaded
        private void SaveGeneratorControllerSettingsToModel()
        {
            //SettingsVM.Settings.CustomData.AddinSettings.Clear();
            foreach(IGeneratorViewModel genvm in Generators)
            {
                ITLCGenGenerator gen = genvm.Generator;
                AddinSettingsModel gendata = new AddinSettingsModel();
                gendata.Naam = gen.GetGeneratorName();
                Type t = gen.GetType();
                // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                var dllprops = t.GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(TLCGenCustomSettingAttribute)));
                foreach (PropertyInfo propertyInfo in dllprops)
                {
                    if (propertyInfo.CanRead)
                    {
                        TLCGenCustomSettingAttribute propattr = (TLCGenCustomSettingAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TLCGenCustomSettingAttribute));
                        if (propattr.SettingType == TLCGenCustomSettingAttribute.SettingTypeEnum.Application)
                        {
                            try
                            {

                                string name = propertyInfo.Name;
                                var v = propertyInfo.GetValue(gen);
                                if (v != null)
                                {
                                    string value = v.ToString();
                                    AddinSettingsPropertyModel prop = new AddinSettingsPropertyModel();
                                    prop.Naam = name;
                                    prop.Setting = value;
                                    gendata.Properties.Add(prop);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
#warning TODO!
                //SettingsVM.Settings.CustomData.AddinSettings.Add(gendata);
            }
        }

        #endregion // Private methods

        #region Public methods

        /// <summary>
        /// Updates the ViewModel structure, causing the View to reload all bound properties.
        /// </summary>
        public void UpdateController()
        {
            ControllerVM.ReloadController();
            OnPropertyChanged(null);
        }

        /// <summary>
        /// Used to set the loaded Controller to a the parsed instance of ControllerModel. The method also 
        /// checks for changes, sets program title, and updates bound properties.
        /// </summary>
        /// <param name="cm">The instance of ControllerModel to be loaded.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetController(ControllerModel cm)
        {
            if (TLCGenControllerDataProvider.Default.SetController(cm))
            {
                if (ControllerVM != null)
                {
                    ControllerVM.SelectedTabIndex = 0;
                }
                string filename = TLCGenControllerDataProvider.Default.ControllerFileName;
                TLCGenControllerDataProvider.Default.SetController(cm);
                ControllerVM.Controller = cm;
                DefaultsProvider.Default.Controller = cm;
                ControllerVM.SelectedTabIndex = 0;
                return true;
            }
            return false;
        }

        #endregion // Public methods

        #region TLCGen Messaging

        private void OnPrepareForGenerationRequest(Messaging.Requests.PrepareForGenerationRequest request)
        {
            Messenger.Default.Send(new Messaging.Requests.ProcessSynchronisationsRequest());
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public MainWindowViewModel()
        {
            Messenger.Default.Register(this, new Action<Messaging.Requests.PrepareForGenerationRequest>(OnPrepareForGenerationRequest));

            // Load application settings and defaults
            SettingsProvider.Default.LoadApplicationSettings();
            DefaultsProvider.Default.LoadSettings();
            TemplatesProvider.Default.LoadSettings();

            // Load available applicationparts and plugins
            TLCGenPluginManager.Default.LoadApplicationParts("TLCGen.ViewModels");
            TLCGenPluginManager.Default.LoadPlugins(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins\\"));

            // Instantiate all parts
            _ApplicationParts = new List<Tuple<TLCGenPluginElems, ITLCGenPlugin>>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default.ApplicationPlugins);
            foreach (var part in parts)
            {
                ITLCGenPlugin instpl = part.Item2;
                var flags = Enum.GetValues(typeof(TLCGenPluginElems));
                foreach(TLCGenPluginElems elem in flags)
                {
                    if ((part.Item1 & elem) == elem)
                    {
                        switch (elem)
                        {
                            case TLCGenPluginElems.Generator:
                                Generators.Add(new IGeneratorViewModel(instpl as ITLCGenGenerator));
                                break;
                            case TLCGenPluginElems.HasSettings:
                                ((ITLCGenHasSettings)instpl).LoadSettings();
                                break;
                            case TLCGenPluginElems.Importer:
                                MenuItem mi = new MenuItem();
                                mi.Header = instpl.GetPluginName();
                                mi.Command = ImportControllerCommand;
                                mi.CommandParameter = instpl;
                                ImportMenuItems.Add(mi);
                                break;
                            case TLCGenPluginElems.IOElementProvider:
                                break;
                            case TLCGenPluginElems.MenuControl:
                                PluginMenuItems.Add(((ITLCGenMenuItem)instpl).Menu);
                                break;
                            case TLCGenPluginElems.TabControl:
                                break;
                            case TLCGenPluginElems.ToolBarControl:
                                break;
                            case TLCGenPluginElems.XMLNodeWriter:
                                break;
                            case TLCGenPluginElems.PlugMessaging:
                                (instpl as ITLCGenPlugMessaging).UpdateTLCGenMessaging();
                                break;
                        }
                    }
                    TLCGenPluginManager.LoadAddinSettings(instpl, part.Item2.GetType(), SettingsProvider.Default.Settings.CustomData);
                }
                _ApplicationParts.Add(new Tuple<TLCGenPluginElems, ITLCGenPlugin>(part.Item1, instpl as ITLCGenPlugin));
            }
            if (Generators.Count > 0) SelectedGenerator = Generators[0];
            
            // Construct the ViewModel
            ControllerVM = new ControllerViewModel();

            // If we are in debug mode, the code below tries loading default file
#if DEBUG
            TLCGenControllerDataProvider.Default.OpenDebug();
            if (TLCGenControllerDataProvider.Default.Controller != null)
            {
                ControllerVM.Controller = TLCGenControllerDataProvider.Default.Controller;
                DefaultsProvider.Default.Controller = TLCGenControllerDataProvider.Default.Controller;

                Messenger.Default.Send(new ControllerFileNameChangedMessage(TLCGenControllerDataProvider.Default.ControllerFileName, null));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                TLCGenControllerDataProvider.Default.ControllerHasChanged = false;
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                OnPropertyChanged("ProgramTitle");
            }
#endif

            if (!DesignMode.IsInDesignMode)
            {
                if(Application.Current != null && Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            }
        }

        #endregion // Constructor

    }
}
