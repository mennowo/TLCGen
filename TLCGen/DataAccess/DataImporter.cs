using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.DataAccess
{
    public class DataImporter
    {
        #region Fields

        private MainWindowViewModel _MainWindowVM;

        #endregion // Fields

        #region Commands

        RelayCommand _ImportTabCFileNewCommand;
        public ICommand ImportTabCFileNewCommand
        {
            get
            {
                if (_ImportTabCFileNewCommand == null)
                {
                    _ImportTabCFileNewCommand = new RelayCommand(ImportTabCFileNewCommand_Executed, ImportTabCFileNewCommand_CanExecute);
                }
                return _ImportTabCFileNewCommand;
            }
        }


        RelayCommand _ImportTabCFileExistingCommand;
        public ICommand ImportTabCFileExistingCommand
        {
            get
            {
                if (_ImportTabCFileExistingCommand == null)
                {
                    _ImportTabCFileExistingCommand = new RelayCommand(ImportTabCFileExistingCommand_Executed, ImportTabCFileExistingCommand_CanExecute);
                }
                return _ImportTabCFileExistingCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void ImportTabCFileNewCommand_Executed(object prm)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "tab.c files|*tab.c|Alle files|*.*";

            ControllerModel cm = new ControllerModel();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Count() <= 1)
                        throw new NotImplementedException("Het bestand heeft minder dan 2 regels.");

                    // Build a list of the Phases with conflicts from the tab.c file
                    List<FaseCyclusModel> NewFasen = GetNewFasenList(lines);

                    // Copy the results into the ControllerVM
                    List<ConflictViewModel> OldConflicts = new List<ConflictViewModel>();
                    foreach (FaseCyclusModel fcm in NewFasen)
                    {
                        cm.Fasen.Add(fcm);
                    }

                    _MainWindowVM.SetNewController(cm);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                }
            }
        }

        void ImportTabCFileExistingCommand_Executed(object prm)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "tab.c files|*tab.c|Alle files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    
                    ControllerViewModel cvm = _MainWindowVM.ControllerVM;

                    // Need to set to 0 here, so if user is in ConflictMatrix view, current matrix is saved now,
                    // not (false!) when user leaves the tab later.
                    cvm.SelectedTabIndex = 0;

                    // Check if at least all Phases in the Controller occur in the tab.c file
                    List<string> _Fasen = new List<string>();
                    foreach (string line in lines)
                    {
                        if (Regex.IsMatch(line, @"^\s+TO_max\["))
                        {
                            string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                            if (!_Fasen.Contains(fc1))
                                _Fasen.Add(fc1);
                        }
                    }
                    string AllPhasesMessage = "";
                    List<FaseCyclusViewModel> fcvms = new List<FaseCyclusViewModel>();
                    foreach(FaseCyclusViewModel fcvm in cvm.Fasen)
                    {
                        if (!_Fasen.Contains(fcvm.Define))
                        {
                            AllPhasesMessage = AllPhasesMessage + fcvm.Define + "\n";
                            fcvms.Add(fcvm);
                        }
                    }
                    MessageBoxResult result = MessageBoxResult.Yes;
                    if(!string.IsNullOrEmpty(AllPhasesMessage))
                    {
                        result = MessageBox.Show("Niet alle fasen uit de regeling komen voor in de tab.c file.\nConflicten van de volgende fasen worden verwijderd:\n\n" + 
                            AllPhasesMessage + "\nDoorgaan?", "Niet alle fasen gevonden", MessageBoxButton.YesNo);
                    }

                    // Continue...
                    if(result == MessageBoxResult.Yes)
                    {
                        // Clear conflicts from Phases not in tab.c file
                        foreach(FaseCyclusViewModel fcvm in fcvms)
                        {
                            fcvm.Conflicten.Clear();
                        }

                        // Build a list of the Phases with conflicts from the tab.c file
                        List<FaseCyclusModel> NewFasen = GetNewFasenList(lines);

                        // Copy the results into the ControllerVM
                        string NewPhasesMessage = "";
                        List<ConflictViewModel> OldConflicts = new List<ConflictViewModel>();
                        foreach(FaseCyclusModel fcm in NewFasen)
                        {
                            bool found = false;
                            foreach(FaseCyclusViewModel fcvm in cvm.Fasen)
                            {
                                if(fcm.Define == fcvm.Define)
                                {
                                    found = true;

                                    // Store current conflicts
                                    OldConflicts.Clear();
                                    foreach (ConflictViewModel _cvm in fcvm.Conflicten)
                                        OldConflicts.Add(_cvm);

                                    // Load new conflicts
                                    fcvm.Conflicten.Clear();
                                    foreach(ConflictModel cm in fcm.Conflicten)
                                    {
                                        ConflictViewModel _cvm = new ConflictViewModel(cvm, cm);
                                        fcvm.Conflicten.Add(_cvm);
                                    }

                                    // Check for new conflicts
#warning TODO - At this point: check if new conflicts have been added, and act accordingly
                                }
                            }
                            if(!found)
                            {
                                FaseCyclusViewModel _fcvm = new FaseCyclusViewModel(cvm, fcm);
                                cvm.Fasen.Add(_fcvm);
                                NewPhasesMessage = NewPhasesMessage + _fcvm.Define + "\n";
                            }
                        }
                        if (!string.IsNullOrEmpty(NewPhasesMessage))
                        {
                            MessageBox.Show("De volgende fasen uit de tab.c file zijn nieuw toegevoegd in de regeling:\n\n" +
                                NewPhasesMessage, "Nieuwe fasen toegevoegd", MessageBoxButton.OK);
                        }
                    }
                    _MainWindowVM.UpdateController();
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                }
            }
        }

        bool ImportTabCFileNewCommand_CanExecute(object prm)
        {
            return _MainWindowVM != null;
        }

        bool ImportTabCFileExistingCommand_CanExecute(object prm)
        {
            return _MainWindowVM != null && _MainWindowVM.ControllerVM != null;
        }

        #endregion // Command functionality

        #region Private Methods

        private List<FaseCyclusModel> GetNewFasenList(string [] lines)
        {
            List<FaseCyclusModel> NewFasen = new List<FaseCyclusModel>();
            
            // Compile a list of Phases with conflicts from the file
            foreach (string line in lines)
            {
                if (Regex.IsMatch(line, @"^\s+TO_max\["))
                {
                    string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*(fc[0-9]+).*", "$1");
                    string fc2 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*(fc[0-9]+).*", "$1");
                    string _conf = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*fc[0-9]+\s*\]\s*=\s*(([0-9]+|FK|GK|GKL)).*", "$1");

                    int conf = 0;
                    if (_conf == "FK") conf = -2;
                    else if (_conf == "GK") conf = -3;
                    else if (_conf == "GKL") conf = -4;
                    else if (!Int32.TryParse(_conf, out conf))
                    {
                        if (lines.Count() <= 1)
                            throw new NotImplementedException($"Conflict van {fc1} naar {fc2} heeft een foutieve waarde: {_conf}");
                    }

                    FaseCyclusModel _fcm1 = null;
                    foreach (FaseCyclusModel fcm in NewFasen)
                    {
                        if (fcm.Define == fc1)
                        {
                            _fcm1 = fcm;
                            break;
                        }
                    }
                    if (_fcm1 == null)
                    {
                        _fcm1 = new FaseCyclusModel();
                        _fcm1.Define = fc1;
                        _fcm1.Naam = fc1.Replace("fc", "");
                        SettingsProvider.ApplyDefaultFaseCyclusSettings(_fcm1, fc1);
                        NewFasen.Add(_fcm1);
                    }

                    FaseCyclusModel _fcm2 = null;
                    foreach (FaseCyclusModel fcm in NewFasen)
                    {
                        if (fcm.Define == fc2)
                        {
                            _fcm2 = fcm;
                            break;
                        }
                    }
                    if (_fcm2 == null)
                    {
                        _fcm2 = new FaseCyclusModel();
                        _fcm2.Define = fc2;
                        _fcm2.Naam = fc2.Replace("fc", "");
                        SettingsProvider.ApplyDefaultFaseCyclusSettings(_fcm2, fc2);
                        NewFasen.Add(_fcm2);
                    }
                    _fcm1.Conflicten.Add(new ConflictModel() { FaseVan = _fcm1.Define, FaseNaar = _fcm2.Define, Waarde = conf });
                }
            }
            return NewFasen;
        }

        #endregion // Private Methods

        #region Constructor

        public DataImporter(MainWindowViewModel _mainwindowvm)
        {
            _MainWindowVM = _mainwindowvm;
        }

        #endregion // Constructor
    }
}
