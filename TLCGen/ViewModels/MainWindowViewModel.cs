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
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Generators.CCOL;
using TLCGen.Helpers;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private DataProvider _MyDataProvider;
        private DataImporter _MyDataImporter;
        private List<IGeneratorViewModel> _Generators;
        private IGeneratorViewModel _SelectedGenerator;
        private TLCGenSettingsViewModel _SettingsVM;

        #endregion // Fields

        #region Properties

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

        public bool HasController
        {
            get { return ControllerVM != null; }
        }

        public DataProvider MyDataProvider
        {
            get { return _MyDataProvider; }
        }

        public DataImporter MyDataImporter
        {
            get { return _MyDataImporter; }
        }

        public TLCGenSettingsViewModel SettingsVM
        {
            get
            {
                if(_SettingsVM == null)
                {
                    _SettingsVM = new TLCGenSettingsViewModel();
                    _SettingsVM.LoadApplicationSettings();
                }
                return _SettingsVM;
            }
        }

        public string ProgramTitle
        {
            get
            {
                if(HasController && !string.IsNullOrEmpty(MyDataProvider.FileName))
                    return "TLCGen - " + MyDataProvider.FileName;
                else
                    return "TLCGen";
            }
        }

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

        public IGeneratorViewModel SelectedGenerator
        {
            get { return _SelectedGenerator; }
            set
            {
                _SelectedGenerator = value;
                OnPropertyChanged("SelectedGenerator");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _NewFileCommand;
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

        RelayCommand _OpenFileCommand;
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

        RelayCommand _SaveFileCommand;
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

        RelayCommand _SaveAsFileCommand;
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


        RelayCommand _CloseFileCommand;
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

        RelayCommand _ExitApplicationCommand;
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

        RelayCommand _GenerateCommand;
        public ICommand GenerateCommand
        {
            get
            {
                if (_GenerateCommand == null)
                {
                    _GenerateCommand = new RelayCommand(GenerateCommand_Executed, GenerateCommand_CanExecute);
                }
                return _GenerateCommand;
            }
        }

        RelayCommand _GenerateVisualCommand;
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

        RelayCommand _ShowSettingsWindowCommand;
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

        private void ShowSettingsWindowCommand_Executed(object obj)
        {
            TLCGen.Views.Dialogs.TLCGenSettingsWindow settingswin = new Views.Dialogs.TLCGenSettingsWindow();
            settingswin.DataContext = this;
            settingswin.ShowDialog();
        }

        RelayCommand _ShowAboutCommand;
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

        private void ShowAboutCommand_Executed(object obj)
        {
            TLCGen.Views.AboutWindow about = new Views.AboutWindow();
            about.ShowDialog();
        }

        void NewFileCommand_Executed(object prm)
        {
            if (!ControllerHasChanged())
            {
                MyDataProvider.NewEmptyController();
                ControllerVM = new ControllerViewModel(this, MyDataProvider.Controller);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.UpdateTabsEnabled();
            }
        }

        bool NewFileCommand_CanExecute(object prm)
        {
            return true;
        }

        void OpenFileCommand_Executed(object prm)
        {
            if (!ControllerHasChanged())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Filter = "TLCGen files|*.tlc;*.tlcgz";
                if (openFileDialog.ShowDialog() == true)
                {
                    MyDataProvider.FileName = openFileDialog.FileName;
                    if (MyDataProvider.LoadController())
                    {
                        ControllerVM = new ControllerViewModel(this, MyDataProvider.Controller);
                        ControllerVM.SelectedTabIndex = 0;
                        OnPropertyChanged("ProgramTitle");
                        ControllerVM.DoUpdateFasen();
                        ControllerVM.UpdateTabsEnabled();
                        ControllerVM.SetStatusText("regeling geopend");
                    }
                }
            }
        }

        bool OpenFileCommand_CanExecute(object prm)
        {
            return true;
        }

        void SaveFileCommand_Executed(object prm)
        {
            if (string.IsNullOrWhiteSpace(MyDataProvider.FileName))
                SaveAsFileCommand.Execute(null);
            else
            {
                // Save the conflict matrix if needed
                if (ControllerVM.SelectedTab != null &&
                    ControllerVM.SelectedTab.Header.ToString() == "Conflicten" &&
                    ControllerVM.ConflictMatrixVM.MatrixChanged)
                {
                    string s = ControllerVM.ConflictMatrixVM.IsMatrixSymmetrical();
                    if (!string.IsNullOrEmpty(s))
                    {
                        System.Windows.MessageBox.Show(s, "Error: Conflict matrix niet symmetrisch. Kan niet opslaan.");
                        return;
                    }
                    ControllerVM.ConflictMatrixVM.SaveConflictMatrix();
                }
                MyDataProvider.SaveController();
                ControllerVM.HasChanged = false;
                ControllerVM.UpdateTabsEnabled();
                ControllerVM.SetStatusText("regeling opgeslagen");
            }
        }

        bool SaveFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.HasChanged;
        }

        void SaveAsFileCommand_Executed(object prm)
        {

            // Save the conflict matrix if needed
            if (ControllerVM.SelectedTab != null &&
                ControllerVM.SelectedTab.Header.ToString() == "Conflicten" &&
                ControllerVM.ConflictMatrixVM.MatrixChanged)
            {
                string s = ControllerVM.ConflictMatrixVM.IsMatrixSymmetrical();
                if (!string.IsNullOrEmpty(s))
                {
                    System.Windows.MessageBox.Show(s, "Error: Conflict matrix niet symmetrisch. Kan niet opslaan.");
                    return;
                }
                ControllerVM.ConflictMatrixVM.SaveConflictMatrix();
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.Filter = "TLCGen files|*.tlc|TLCGen compressed files|*.tlcgz";
            if (!string.IsNullOrWhiteSpace(MyDataProvider.FileName))
                saveFileDialog.FileName = MyDataProvider.FileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                MyDataProvider.FileName = saveFileDialog.FileName;
                MyDataProvider.SaveController();
                ControllerVM.HasChanged = false;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.UpdateTabsEnabled();
                ControllerVM.SetStatusText("regeling opgeslagen");
            }
        }

        bool SaveAsFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null;
        }

        void CloseFileCommand_Executed(object prm)
        {
            if (!ControllerHasChanged())
            {
                MyDataProvider.CloseController();
                ControllerVM = null;
                OnPropertyChanged("ProgramTitle");
            }
        }

        bool CloseFileCommand_CanExecute(object prm)
        {
            return ControllerVM != null;
        }

        void ExitApplicationCommand_Executed(object prm)
        {
            System.Windows.Application.Current.Shutdown();
        }

        bool ExitApplicationCommand_CanExecute(object prm)
        {
            return true;
        }

        void GenerateCommand_Executed(object prm)
        {
            string result = SelectedGenerator.Generator.GenerateSourceFiles(ControllerVM.Controller, System.IO.Path.GetDirectoryName(MyDataProvider.FileName));
            ControllerVM.SetStatusText(result);
        }

        bool GenerateCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.Fasen != null && ControllerVM.Fasen.Count > 0;
        }

        void GenerateVisualCommand_Executed(object prm)
        {
            string result = SelectedGenerator.Generator.GenerateProjectFiles(ControllerVM.Controller, System.IO.Path.GetDirectoryName(MyDataProvider.FileName));
            ControllerVM.SetStatusText(result);
        }

        bool GenerateVisualCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.Fasen != null && ControllerVM.Fasen.Count > 0;
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
                                    foreach (GeneratorDataModel gendata in SettingsVM.Settings.CustomData.GeneratorsData)
                                    {
                                        if (gendata.Naam == igenerator.GetGeneratorName())
                                        {
                                            // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                                            var dllprops = t.GetProperties().Where(
                                                prop => Attribute.IsDefined(prop, typeof(TLCGenCustomSettingAttribute)));
                                            // Loop the saved settings, and load if applicable
                                            foreach (GeneratorDataPropertyModel dataprop in gendata.Properties)
                                            {
                                                foreach (var propinfo in dllprops)
                                                {
                                                    // Only load here, if it is a controller specific setting
                                                    TLCGenCustomSettingAttribute propattr = (TLCGenCustomSettingAttribute)Attribute.GetCustomAttribute(propinfo, typeof(TLCGenCustomSettingAttribute));
                                                    if (propinfo.Name == dataprop.Naam)
                                                    {
                                                        if (propattr != null && propattr.SettingType == "application")
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
                                    System.Windows.MessageBox.Show($"Library {file} wordt niet herkend als TLCGen genrator module.");
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
                SettingsVM.SaveApplicationSettings();
            }
        }

        private void SaveGeneratorControllerSettingsToModel()
        {
            SettingsVM.Settings.CustomData.GeneratorsData.Clear();
            foreach(IGeneratorViewModel genvm in Generators)
            {
                IGenerator gen = genvm.Generator;
                GeneratorDataModel gendata = new GeneratorDataModel();
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
                        if (propattr.SettingType == "application")
                        {
                            try
                            {

                                string name = propertyInfo.Name;
                                string value = propertyInfo.GetValue(gen, null).ToString();
                                GeneratorDataPropertyModel prop = new GeneratorDataPropertyModel();
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
                SettingsVM.Settings.CustomData.GeneratorsData.Add(gendata);
            }
        }

        #endregion // Private methods

        #region Public methods

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

        public void UpdateController()
        {
            ControllerVM.DoUpdateFasen();
            ControllerVM.ConflictMatrixVM.BuildConflictMatrix();
            OnPropertyChanged(null);
        }

        public bool SetNewController(ControllerModel cm)
        {
            if (!ControllerHasChanged())
            {
                if(ControllerVM != null)
                {
                    ControllerVM.SelectedTabIndex = 0;
                }
                MyDataProvider.SetNewController(cm);
                ControllerVM = new ControllerViewModel(this, cm);
                ControllerVM.SelectedTabIndex = 0;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.SortFasen();
                ControllerVM.ConflictMatrixVM.BuildConflictMatrix();
                return true;
            }
            return false;
        }

        #endregion // Public methods

        #region Constructor

        public MainWindowViewModel()
        { 
            _MyDataProvider = new DataProvider();
            _MyDataImporter = new DataImporter(this);

            LoadAllGenerators();
            if (Generators.Count > 0) SelectedGenerator = Generators[0];

#if DEBUG
            MyDataProvider.FileName = System.AppDomain.CurrentDomain.BaseDirectory + "test.tlc";
            if (MyDataProvider.LoadController())
            {
                ControllerVM = new ControllerViewModel(this, MyDataProvider.Controller);
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
