using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Interfaces.Public;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Utilities;

namespace TLCGen.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private List<IGeneratorViewModel> _Generators;
        private IGeneratorViewModel _SelectedGenerator;
        public List<IImporter> _Importers;
        private IImporter _SelectedImporter;

        private TLCGenSettingsViewModel _SettingsVM;
        private ControllerViewModel _ControllerVM;

        private List<MenuItem> _ImportMenuItems;

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
                if(HasController && !string.IsNullOrEmpty(DataProvider.FileName))
                    return "TLCGen - " + DataProvider.FileName;
                else
                    return "TLCGen";
            }
        }

        /// <summary>
        /// Holds a list of available code generators. Available generators are resolved at runtime
        /// by looking in the folder 'Generators' for DLL's with appropriate attributes.
        /// </summary>
        public List<IGeneratorViewModel> Generators
        {
            get
            {
                if (_Generators == null)
                {
                    _Generators = new List<IGeneratorViewModel>();
                }
                return _Generators;
            }
        }

        /// <summary>
        /// Holds a list of available importers. Available importers are resolved at runtime
        /// by looking in the folder 'Importers' for DLL's with appropriate attributes.
        /// </summary>
        public List<IImporter> Importers
        {
            get
            {
                if (_Importers == null)
                {
                    _Importers = new List<IImporter>();
                }
                return _Importers;
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
        /// Holds the selected importer, on which the appropriate function calls are invoked
        /// when commands relating to importing data are executed.
        /// </summary>
        public IImporter SelectedImporter
        {
            get { return _SelectedImporter; }
            set
            {
                _SelectedImporter = value;
                OnPropertyChanged("SelectedImporter");
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

        #endregion // Properties

        #region Commands

        RelayCommand _NewFileCommand;
        RelayCommand _OpenFileCommand;
        RelayCommand _SaveFileCommand;
        RelayCommand _SaveAsFileCommand;
        RelayCommand _CloseFileCommand;
        RelayCommand _ExitApplicationCommand;
        RelayCommand _GenerateCommand;
        RelayCommand _GenerateVisualCommand;
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

        public ICommand GenerateCommand
        {
            get
            {
                if (_GenerateCommand == null)
                {
                    _GenerateCommand = new RelayCommand(GenerateCodeCommand_Executed, GenerateCodeCommand_CanExecute);
                }
                return _GenerateCommand;
            }
        }

        public ICommand GenerateVisualCommand
        {
            get
            {
                if (_GenerateVisualCommand == null)
                {
                    _GenerateVisualCommand = new RelayCommand(GenerateVisualCommand_Executed, GenerateVisualCommand_CanExecute);
                }
                return _GenerateVisualCommand;
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
                DataProvider.SetController();
                ControllerVM = new ControllerViewModel(this, DataProvider.Controller);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.UpdateTabsEnabled();
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
                    DataProvider.FileName = openFileDialog.FileName;
                    if (DataProvider.LoadController())
                    {
                        ControllerVM = new ControllerViewModel(this, DataProvider.Controller);
                        ControllerVM.SelectedTabIndex = 0;
                        OnPropertyChanged("ProgramTitle");
                        ControllerVM.DoUpdateFasen();
                        ControllerVM.UpdateTabsEnabled();
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
            if (string.IsNullOrWhiteSpace(DataProvider.FileName))
                SaveAsFileCommand.Execute(null);
            else
            {
                // Save all changes to model
                ControllerVM.ProcessAllChanges();

                // Check data integrity: do not save wrong data
                string s = IntegrityChecker.IsControllerDataOK(ControllerVM.Controller);
                if(s != null)
                {
                    System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                    return;
                }

                // Save data to disk, update saved state
                DataProvider.SaveController();
                ControllerVM.HasChanged = false;
                ControllerVM.UpdateTabsEnabled();
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
            ControllerVM.ProcessAllChanges();

            // Check data integrity: do not save wrong data
            string s = IntegrityChecker.IsControllerDataOK(ControllerVM.Controller);
            if (s != null)
            {
                System.Windows.MessageBox.Show(s + "\n\nRegeling niet opgeslagen.", "Error bij opslaan: fout in regeling");
                return;
            }

            // Save data to disk, update saved state
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "TLCGen files|*.tlc|TLCGen compressed files|*.tlcgz";
            if (!string.IsNullOrWhiteSpace(DataProvider.FileName))
                saveFileDialog.FileName = DataProvider.FileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                DataProvider.FileName = saveFileDialog.FileName;
                DataProvider.SaveController();
                ControllerVM.HasChanged = false;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.UpdateTabsEnabled();
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
                DataProvider.CloseController();
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

        private void GenerateCodeCommand_Executed(object prm)
        {
            string result = SelectedGenerator.Generator.GenerateSourceFiles(ControllerVM.Controller, System.IO.Path.GetDirectoryName(DataProvider.FileName));
            ControllerVM.SetStatusText(result);
        }

        private bool GenerateCodeCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.Fasen != null && ControllerVM.Fasen.Count > 0;
        }

        private void GenerateVisualCommand_Executed(object prm)
        {
            string result = SelectedGenerator?.Generator?.GenerateProjectFiles(ControllerVM.Controller, System.IO.Path.GetDirectoryName(DataProvider.FileName));
            ControllerVM.SetStatusText(result);
        }

        private bool GenerateVisualCommand_CanExecute(object prm)
        {
            return SelectedGenerator != null && ControllerVM != null && ControllerVM.Fasen != null && ControllerVM.Fasen.Count > 0;
        }

        private void ImportControllerCommand_Executed(object obj)
        {
            if (obj == null)
                throw new NotImplementedException();
            IImporter imp = obj as IImporter;
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
                    // Check data integrity
                    s1 = IntegrityChecker.IsControllerDataOK(c2);
                    if (s1 != null)
                    {
                        System.Windows.MessageBox.Show("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data");
                        return;
                    }
                    SetNewController(c2);
                    ControllerVM.HasChanged = true;
                }
                // Import as new controller
                else
                {
                    ControllerModel c1 = imp.ImportController();
                    // Check data integrity
                    string s1 = IntegrityChecker.IsControllerDataOK(c1);
                    if (s1 != null)
                    {
                        System.Windows.MessageBox.Show("Fout bij importeren:\n\n" + s1, "Error bij importeren: fout in data");
                        return;
                    }
                    SetNewController(c1);
                    ControllerVM.HasChanged = true;
                }
            }
        }

        private bool ImportControllerCommand_CanExecute(object obj)
        {
            if (obj == null)
                return false;

            IImporter imp = obj as IImporter;
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

        private void LoadAllGenerators()
        {
            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Generators\\");
                if (Directory.Exists(path))
                {
                    // Find all Generator DLL's
                    foreach (String file in Directory.GetFiles(path))
                    {
                        if (Path.GetExtension(file).ToLower() == ".dll")
                        {
                            // Find and loop all types from the Generators
                            Assembly assemblyInstance = Assembly.LoadFrom(file);
                            Type[] types = assemblyInstance.GetTypes();
                            foreach (Type t in types)
                            {
                                // Find TLCGenGenerator attribute, and if found, continue
                                TLCGenGeneratorAttribute genattr = (TLCGenGeneratorAttribute)Attribute.GetCustomAttribute(t, typeof(TLCGenGeneratorAttribute));
                                if (genattr != null)
                                {
                                    // Cast the Generator to IGenerator so we can read its name
                                    var generator = Activator.CreateInstance(t);
                                    var igenerator = generator as IGenerator;
                                    // Loop the settings data, to see if we have settings for this Generator
                                    foreach (AddinSettingsModel gendata in SettingsVM.Settings.CustomData.AddinSettings)
                                    {
                                        if (gendata.Naam == igenerator.Name)
                                        {
                                            // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                                            var dllprops = t.GetProperties().Where(
                                                prop => Attribute.IsDefined(prop, typeof(TLCGenCustomSettingAttribute)));
                                            // Loop the saved settings, and load if applicable
                                            foreach (AddinSettingsPropertyModel dataprop in gendata.Properties)
                                            {
                                                foreach (var propinfo in dllprops)
                                                {
                                                    // Only load here, if it is a controller specific setting
                                                    TLCGenCustomSettingAttribute propattr = (TLCGenCustomSettingAttribute)Attribute.GetCustomAttribute(propinfo, typeof(TLCGenCustomSettingAttribute));
                                                    if (propinfo.Name == dataprop.Naam)
                                                    {
                                                        if (propattr != null && propattr.SettingType == TLCGenCustomSettingAttribute.SettingTypeEnum.Application)
                                                        {
                                                            try
                                                            {
                                                                string type = propinfo.PropertyType.ToString();
                                                                switch (type)
                                                                {
                                                                    case "System.Double":
                                                                        double d;
                                                                        if (Double.TryParse(dataprop.Setting, out d))
                                                                            propinfo.SetValue(generator, d);
                                                                        break;
                                                                    case "System.Int32":
                                                                        int i32;
                                                                        if (Int32.TryParse(dataprop.Setting, out i32))
                                                                            propinfo.SetValue(generator, i32);
                                                                        break;
                                                                    case "System.String":
                                                                        propinfo.SetValue(generator, dataprop.Setting);
                                                                        break;
                                                                    default:
                                                                        throw new NotImplementedException("False IGenerator property type: " + type);
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                System.Windows.MessageBox.Show("Error load generator settings.");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // Add the generator to our list of available generators
                                    Generators.Add(new IGeneratorViewModel(igenerator));
                                    break;
                                }
                                else
                                {
#if !DEBUG
                                    System.Windows.MessageBox.Show($"Library {file} wordt niet herkend als TLCGen genrator module.");
#endif
                                }
                            }
                        }
                    }
                }
                else
                {
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ControllerHasChanged())
            {
                e.Cancel = true;
            }
            else
            {
                SaveGeneratorControllerSettingsToModel();
                SettingsProvider.SaveApplicationSettings();
            }
        }

        private void SaveGeneratorControllerSettingsToModel()
        {
            SettingsVM.Settings.CustomData.AddinSettings.Clear();
            foreach(IGeneratorViewModel genvm in Generators)
            {
                IGenerator gen = genvm.Generator;
                AddinSettingsModel gendata = new AddinSettingsModel();
                gendata.Naam = gen.Name;
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
                                string value = propertyInfo.GetValue(gen, null).ToString();
                                AddinSettingsPropertyModel prop = new AddinSettingsPropertyModel();
                                prop.Naam = name;
                                prop.Setting = value;
                                gendata.Properties.Add(prop);
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
        /// Used to set the loaded Controller to a new instance of ControllerModel. The method also 
        /// checks for changes, sets program title, and updates bound properties.
        /// </summary>
        /// <param name="cm">The instance of ControllerModel to be loaded.</param>
        /// <returns></returns>
        public bool SetNewController(ControllerModel cm)
        {
            if (!ControllerHasChanged())
            {
                if(ControllerVM != null)
                {
                    ControllerVM.SelectedTabIndex = 0;
                }
                DataProvider.SetController(cm);
                ControllerVM = new ControllerViewModel(this, cm);
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
            // Load application settings (defaults, etc.)
            SettingsProvider.LoadApplicationSettings();
            
            // Load addins: generators, importers
            List<IGenerator> generators = AddInLoaderT.LoadAllAddins<IGenerator, TLCGenGeneratorAttribute>(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Generators\\"));
            List<IImporter> importers = AddInLoaderT.LoadAllAddins<IImporter, TLCGenImporterAttribute>(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Importers\\"));

            foreach(IGenerator gen in generators)
            {
                Type t = gen.GetType();
                AddInLoaderT.LoadAddinSettings(gen, t, SettingsProvider.CustomSettings);
                Generators.Add(new IGeneratorViewModel(gen));
            }
            if (Generators.Count > 0) SelectedGenerator = Generators[0];

            foreach (IImporter imp in importers)
            {
                Type t = imp.GetType();
                AddInLoaderT.LoadAddinSettings(imp, t, SettingsProvider.CustomSettings);
                Importers.Add(imp);
                MenuItem mi = new MenuItem();
                mi.Header = imp.Name;
                mi.Command = ImportControllerCommand;
                mi.CommandParameter = imp;
                ImportMenuItems.Add(mi);
            }

            // If we are in debug mode, the code below tries loading a file
            // called 'test.tlc' from the folder where the application runs.
#if DEBUG
            DataProvider.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "test.tlc";
            if (DataProvider.LoadController())
            {
                ControllerVM = new ControllerViewModel(this, DataProvider.Controller);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.DoUpdateFasen();
                ControllerVM.UpdateTabsEnabled();
            }
#endif

            if (!DesignMode.IsInDesignMode)
            {
                Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            }
        }

        #endregion // Constructor

    }
}
