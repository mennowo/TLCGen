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

        private List<IGeneratorViewModel> _Generators;
        private List<ITLCGenPlugin> _ApplicationPlugins;
        private IGeneratorViewModel _SelectedGenerator;
        private TLCGenSettingsViewModel _SettingsVM;
        private ControllerViewModel _ControllerVM;
        private List<MenuItem> _ImportMenuItems;
        private List<MenuItem> _PluginMenuItems;

        private SortedDictionary<int, Type> OwnTabTypes = new SortedDictionary<int, Type>();
        private List<Type> XmlNodeWriterPlugins = new List<Type>();

        #endregion // Fields

        #region Properties

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

                foreach(var gen in Generators)
                {
                    if (value != null)
                        gen.Generator.Controller = _ControllerVM.Controller;
                    else
                        gen.Generator.Controller = null;

                    var messpl = gen.Generator as ITLCGenPlugMessaging;
                    if (messpl != null)
                    {
                        messpl.UpdateTLCGenMessaging();
                    }
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
            get { return ControllerVM != null; }
        }

        /// <summary>
        /// ViewModel for the settings of the application
        /// </summary>
        public TLCGenSettingsViewModel SettingsVM
        {
            get
            {
                if(_SettingsVM == null)
                {
                    _SettingsVM = new TLCGenSettingsViewModel();
                }
                return _SettingsVM;
            }
        }

        /// <summary>
        /// A string to be used in the View as the title of the program.
        /// File operations should call OnPropertyChanged for this property,
        /// so the View updates the program title.
        /// </summary>
        public string ProgramTitle
        {
            get
            {
                if(HasController && !string.IsNullOrEmpty(DataProvider.Instance.FileName))
                    return "TLCGen - " + DataProvider.Instance.FileName;
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
            if (!ControllerHasChanged())
            {
                string lastfilename = DataProvider.Instance.FileName;
                DataProvider.Instance.SetController();
                ControllerVM = new ControllerViewModel(DataProvider.Instance.Controller);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                Messenger.Default.Send(new ControllerFileNameChangedMessage(DataProvider.Instance.FileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        private bool NewFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void OpenFileCommand_Executed(object prm)
        {
            if (!ControllerHasChanged())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = "TLCGen files|*.tlc;*.tlcgz";
                if (openFileDialog.ShowDialog() == true)
                {
                    string lastfilename = DataProvider.Instance.FileName;
                    DataProvider.Instance.FileName = openFileDialog.FileName;
                    if (DataProvider.Instance.LoadController())
                    {
                        ControllerVM = null;
                        ControllerVM = new ControllerViewModel(DataProvider.Instance.Controller);
                        ControllerVM.LoadPluginDataFromXmlDocument(DataProvider.Instance.ControllerXml);
                        ControllerVM.SelectedTabIndex = 0;
                        OnPropertyChanged("ProgramTitle");
                        Messenger.Default.Send(new ControllerFileNameChangedMessage(DataProvider.Instance.FileName, lastfilename));
                        Messenger.Default.Send(new UpdateTabsEnabledMessage());
                        ControllerVM.SetStatusText("regeling geopend");
                    }
                }
            }
        }

        private bool OpenFileCommand_CanExecute(object prm)
        {
            return true;
        }

        private void SaveFileCommand_Executed(object prm)
        {
            if (string.IsNullOrWhiteSpace(DataProvider.Instance.FileName))
                SaveAsFileCommand.Execute(null);
            else
            {
                // Save all changes to model
#warning TODO: change to message call
                //ControllerVM.ProcessAllChanges();

                // Check data integrity: do not save wrong data
                string s = IntegrityChecker.IsControllerDataOK(_ControllerVM.Controller);
                if(s != null)
                {
                    System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                    return;
                }

                // Save data to disk, update saved state
                DataProvider.Instance.SaveController(_ControllerVM.GetControllerXmlData());
                //DataProvider.Instance.SaveController();

                ControllerVM.HasChanged = false;
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                ControllerVM.SetStatusText("regeling opgeslagen");
            }
        }

        private bool SaveFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.HasChanged;
        }

        private void SaveAsFileCommand_Executed(object prm)
        {
            // Save all changes to model
#warning TODO: change to message call
            //ControllerVM.ProcessAllChanges();

            // Check data integrity: do not save wrong data
            string s = IntegrityChecker.IsControllerDataOK(_ControllerVM.Controller);
            if (s != null)
            {
                System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                return;
            }

            // Save data to disk, update saved state
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "TLCGen files|*.tlc|TLCGen compressed files|*.tlcgz";
            if (!string.IsNullOrWhiteSpace(DataProvider.Instance.FileName))
                saveFileDialog.FileName = DataProvider.Instance.FileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                string lastfilename = DataProvider.Instance.FileName;
                DataProvider.Instance.FileName = saveFileDialog.FileName;

                DataProvider.Instance.SaveController(_ControllerVM.GetControllerXmlData());
                //DataProvider.Instance.SaveController();

                ControllerVM.HasChanged = false;
                OnPropertyChanged("ProgramTitle");
                Messenger.Default.Send(new ControllerFileNameChangedMessage(DataProvider.Instance.FileName, lastfilename));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                ControllerVM.SetStatusText("regeling opgeslagen");
            }
        }

        private bool SaveAsFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null;
        }

        private void CloseFileCommand_Executed(object prm)
        {
            if (!ControllerHasChanged())
            {
                DataProvider.Instance.CloseController();
                ControllerVM = null;
                OnPropertyChanged("ProgramTitle");
            }
        }

        private bool CloseFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null;
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
            if (!ControllerHasChanged())
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
                    if (ControllerVM != null)
                        ControllerVM.HasChanged = false; // Set forcefully, in case the user decided to ignore changes above
                    SetController(c2);
                    ControllerVM.ReloadController();
                    ControllerVM.HasChanged = true;
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
                    if (ControllerVM != null)
                        ControllerVM.HasChanged = false; // Set forcefully, in case the user decided to ignore changes above
                    SetNewController(c1);
                    ControllerVM.ReloadController();
                    ControllerVM.HasChanged = true;
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
                return ControllerVM != null;

            return true;
        }

        private void ShowSettingsWindowCommand_Executed(object obj)
        {
            TLCGen.Views.Dialogs.TLCGenSettingsWindow settingswin = new Views.Dialogs.TLCGenSettingsWindow();
            settingswin.DataContext = this;
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
            if (ControllerHasChanged())
            {
                e.Cancel = true;
            }
            else
            {
                foreach (var pl in _ApplicationPlugins)
                {
                    var setpl = pl as ITLCGenHasSettings;
                    if (setpl != null)
                    {
                        setpl.SaveSettings();
                    }
                }

                SaveGeneratorControllerSettingsToModel();
                SettingsProvider.Default.SaveApplicationSettings();
            }
        }

#warning Make this generic, just like how settings are loaded
        private void SaveGeneratorControllerSettingsToModel()
        {
            SettingsVM.Settings.CustomData.AddinSettings.Clear();
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
                SettingsVM.Settings.CustomData.AddinSettings.Add(gendata);
            }
        }

        #endregion // Private methods

        #region Public methods

        /// <summary>
        /// Checks wether or not the currently loaded Controller has changes. If it does,
        /// the method offers the user to save them.
        /// </summary>
        /// <returns>True if there are unsaved changes, false if there are not or if
        /// the user decides to discard them.</returns>
        public bool ControllerHasChanged()
        {
            if (ControllerVM != null && ControllerVM.HasChanged)
            {
                System.Windows.MessageBoxResult r = System.Windows.MessageBox.Show("Wijzigingen opslaan?", "De regeling is gewijzigd. Opslaan?", System.Windows.MessageBoxButton.YesNoCancel);
                if (r == System.Windows.MessageBoxResult.Yes)
                {
                    SaveFileCommand.Execute(null);
                    if (ControllerVM.HasChanged)
                        return true;
                }
                else if (r == System.Windows.MessageBoxResult.Cancel)
                {
                    return true;
                }
            }
            return false;
        }

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
            if (!ControllerHasChanged())
            {
                if (ControllerVM != null)
                {
                    ControllerVM.SelectedTabIndex = 0;
                }
                string filename = DataProvider.Instance.FileName;
                DataProvider.Instance.SetController(cm);
                DataProvider.Instance.FileName = filename;
                ControllerVM = new ControllerViewModel(cm);
                ControllerVM.SelectedTabIndex = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Used to set the loaded Controller to a new instance of ControllerModel. The method also 
        /// checks for changes, sets program title, and updates bound properties.
        /// </summary>
        /// <param name="cm">The instance of ControllerModel to be loaded.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetNewController(ControllerModel cm)
        {
            if (!ControllerHasChanged())
            {
                ControllerVM = null;
                DataProvider.Instance.SetController(cm);
                ControllerVM = new ControllerViewModel(cm);
                ControllerVM.SelectedTabIndex = 0;
                UpdateController();
                return true;
            }
            return false;
        }

        #endregion // Public methods

        #region Constructor

        public MainWindowViewModel()
        {
            _ApplicationPlugins = new List<ITLCGenPlugin>();

            // Load application settings (defaults, etc.)
            SettingsProvider.Default.LoadApplicationSettings();

            // Load tab types
            string myfolder = System.AppDomain.CurrentDomain.BaseDirectory;
            string nspace = "TLCGen.ViewModels";
            
            var q = from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    where t.IsClass && t.Namespace == nspace
                    select t;
            foreach(var type in q)
            {
                TLCGenPluginManager.Default.ApplicationParts.Add(new Tuple<TLCGenPluginElems, Type>(TLCGenPluginElems.TabControl, type));
            }

            // Load addins
            TLCGenPluginManager.Default.LoadPlugins(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Plugins\\"));

            // Load application wide plugins
            foreach (var plugin in TLCGenPluginManager.Default.Plugins)
            {
                ITLCGenPlugin instpl = null;
                if (plugin.Item1.HasFlag(TLCGenPluginElems.Generator) ||
                    plugin.Item1.HasFlag(TLCGenPluginElems.Importer) ||
                    plugin.Item1.HasFlag(TLCGenPluginElems.MenuControl))
                {
                    instpl = (ITLCGenPlugin)Activator.CreateInstance(plugin.Item2);
                    if(plugin.Item1.HasFlag(TLCGenPluginElems.Generator))
                    {
                        Generators.Add(new IGeneratorViewModel(instpl as ITLCGenGenerator));
                    }
                    if(plugin.Item1.HasFlag(TLCGenPluginElems.Importer))
                    {
                       MenuItem mi = new MenuItem();
                       mi.Header = instpl.GetPluginName();
                       mi.Command = ImportControllerCommand;
                       mi.CommandParameter = instpl;
                       ImportMenuItems.Add(mi);
                    }
                    if (plugin.Item1.HasFlag(TLCGenPluginElems.MenuControl))
                    {
                        PluginMenuItems.Add(((ITLCGenMenuItem)instpl).Menu);
                    }
                }
                if (instpl != null)
                {
                    TLCGenPluginManager.LoadAddinSettings(instpl, plugin.Item2, SettingsProvider.Default.Settings.CustomData);
                    _ApplicationPlugins.Add(instpl as ITLCGenPlugin);
                }
            }
            if (Generators.Count > 0) SelectedGenerator = Generators[0];

            foreach(var pl in _ApplicationPlugins)
            {
                var setpl = pl as ITLCGenHasSettings;
                if(setpl != null)
                {
                    setpl.LoadSettings();
                }
            }

#warning TODO: also load menu items, tabs, etc.

            // If we are in debug mode, the code below tries loading a file
            // called 'test.tlc' from the folder where the application runs.
#if DEBUG
            DataProvider.Instance.FileName = @"C:\Users\NL33478\Documents\Ontwikkeling\TLCGen_test\2017-01-17-testreg\tlcgen\12345.tlc";
            if(!System.IO.File.Exists(DataProvider.Instance.FileName))
            {
                MessageBox.Show("huh? " + DataProvider.Instance.FileName);
            }
            if (DataProvider.Instance.LoadController())
            {
                ControllerVM = new ControllerViewModel(DataProvider.Instance.Controller);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                Messenger.Default.Send(new ControllerFileNameChangedMessage(DataProvider.Instance.FileName, null));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
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
