using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        RelayCommand _GenerateCCOLCommand;
        public ICommand GenerateCCOLCommand
        {
            get
            {
                if (_GenerateCCOLCommand == null)
                {
                    _GenerateCCOLCommand = new RelayCommand(GenerateCCOLCommand_Executed, GenerateCCOLCommand_CanExecute);
                }
                return _GenerateCCOLCommand;
            }
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
                SaveGeneratorControllerSettingsToModel();
                MyDataProvider.SaveController();
                ControllerVM.HasChanged = false;
                ControllerVM.UpdateTabsEnabled();
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
                SaveGeneratorControllerSettingsToModel();
                MyDataProvider.SaveController();
                ControllerVM.HasChanged = false;
                OnPropertyChanged("ProgramTitle");
                ControllerVM.UpdateTabsEnabled();
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

        void GenerateCCOLCommand_Executed(object prm)
        {
            CCOLCodeGenerator generator = new CCOLCodeGenerator();
            generator.GenerateSourceFiles(ControllerVM.Controller, System.IO.Path.GetDirectoryName(MyDataProvider.FileName));
        }

        bool GenerateCCOLCommand_CanExecute(object prm)
        {
            return ControllerVM != null && ControllerVM.Fasen != null && ControllerVM.Fasen.Count > 0;
        }

        #endregion // Command functionality

        #region Private methods

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (ControllerHasChanged())
            {
                e.Cancel = true;
            }
        }

        void SaveGeneratorControllerSettingsToModel()
        {
            ControllerVM.Controller.CustomData.GeneratorsData.Clear();
            foreach(IGenerator gen in ControllerVM.ControllerDataVM.Generators)
            {
                GeneratorDataModel gendata = new GeneratorDataModel();
                gendata.Naam = gen.GetGeneratorName();
                Type t = gen.GetType();
                // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                var dllprops = t.GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(TLCGenGeneratorSettingAttribute)));
                foreach (PropertyInfo propertyInfo in dllprops)
                {
                    if (propertyInfo.CanRead)
                    {
                        TLCGenGeneratorSettingAttribute propattr = (TLCGenGeneratorSettingAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(TLCGenGeneratorSettingAttribute));
                        if (propattr.SettingType == "controller")
                        {
                            string name = propertyInfo.Name;
                            string value = propertyInfo.GetValue(gen, null).ToString();
                            GeneratorDataPropertyModel prop = new GeneratorDataPropertyModel();
                            prop.Naam = name;
                            prop.Setting = value;
                            gendata.Properties.Add(prop);
                        }
                    }
                }
                ControllerVM.Controller.CustomData.GeneratorsData.Add(gendata);
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
