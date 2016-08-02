using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        RelayCommand _ImportTabCFileCommand;
        public ICommand ImportTabCFileCommand
        {
            get
            {
                if (_ImportTabCFileCommand == null)
                {
                    _ImportTabCFileCommand = new RelayCommand(ImportTabCFileCommand_Executed, ImportTabCFileCommand_CanExecute);
                }
                return _ImportTabCFileCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void ImportTabCFileCommand_Executed(object prm)
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

                    foreach (string line in lines)
                    {
                        if (Regex.IsMatch(line, @"^\s+TO_max\["))
                        {
                            string fc1 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc([0-9]+).*", "$1");
                            string fc2 = Regex.Replace(line, @"^\s*TO_max\s*\[\s*fc[0-9]+\s*\]\s*\[\s*fc([0-9]+).*", "$1");
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
                            foreach (FaseCyclusModel fcm in cm.Fasen)
                            {
                                if (fcm.Naam == fc1)
                                {
                                    _fcm1 = fcm;
                                    break;
                                }
                            }
                            if (_fcm1 == null)
                            {
                                _fcm1 = new FaseCyclusModel();
                                _fcm1.Naam = fc1;
                                cm.Fasen.Add(_fcm1);
                            }

                            FaseCyclusModel _fcm2 = null;
                            foreach (FaseCyclusModel fcm in cm.Fasen)
                            {
                                if (fcm.Naam == fc2)
                                {
                                    _fcm2 = fcm;
                                    break;
                                }
                            }
                            if (_fcm2 == null)
                            {
                                _fcm2 = new FaseCyclusModel();
                                _fcm2.Naam = fc2;
                                cm.Fasen.Add(_fcm2);
                            }
                            _fcm1.Conflicten.Add(new ConflictModel() { FaseVan = _fcm1.Naam, FaseNaar = _fcm2.Naam, Waarde = conf });
                        }
                    }
                    _MainWindowVM.SetNewController(cm);
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c");
                }
            }
        }

        bool ImportTabCFileCommand_CanExecute(object prm)
        {
            return _MainWindowVM != null;
        }

        #endregion // Command functionality

        #region Constructor

        public DataImporter(MainWindowViewModel _mainwindowvm)
        {
            _MainWindowVM = _mainwindowvm;
        }

        #endregion // Constructor
    }
}
