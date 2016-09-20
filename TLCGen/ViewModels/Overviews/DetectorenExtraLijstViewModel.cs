using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class DetectorenExtraLijstViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ObservableCollection<string> _Templates;
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();

        #endregion // Fields

        #region Properties

        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                return _ControllerVM.Detectoren;
            }
        }

        public ObservableCollection<string> Templates
        {
            get
            {
                if (_Templates == null)
                {
                    _Templates = new ObservableCollection<string>();
                    _Templates.Add("Template placeholder");
                }
                return _Templates;
            }
        }

        public DetectorViewModel SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
            }
        }

        public IList SelectedDetectoren
        {
            get { return _SelectedDetectoren; }
            set
            {
                _SelectedDetectoren = value;
                OnPropertyChanged("SelectedDetectoren");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_AddDetectorCommand == null)
                {
                    _AddDetectorCommand = new RelayCommand(AddNewDetectorCommand_Executed, AddNewDetectorCommand_CanExecute);
                }
                return _AddDetectorCommand;
            }
        }


        RelayCommand _RemoveDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_RemoveDetectorCommand == null)
                {
                    _RemoveDetectorCommand = new RelayCommand(RemoveDetectorCommand_Executed, RemoveDetectorCommand_CanExecute);
                }
                return _RemoveDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewDetectorCommand_Executed(object prm)
        {
            DetectorModel dm = new DetectorModel();
            string newname = "001";
            int inewname = 1;
            foreach (DetectorViewModel dvm in Detectoren)
            {
                if (Regex.IsMatch(dvm.Naam, @"[0-9]+"))
                {
                    Match m = Regex.Match(dvm.Naam, @"[0-9]+");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        inewname++;
                        newname = inewname.ToString();
                        newname = (inewname < 10 ? "0" : "") + newname;
                        newname = (inewname < 100 ? "0" : "") + newname;
                        while (!_ControllerVM.IsElementNaamUnique(newname))
                        {
                            inewname++;
                            newname = inewname.ToString();
                            newname = (inewname < 10 ? "0" : "") + newname;
                            newname = (inewname < 100 ? "0" : "") + newname;
                        }
                    }
                }
            }
            dm.Naam = newname;
            dm.Define = SettingsProvider.GetDetectorDefinePrefix() + newname;
            DetectorViewModel dvm1 = new DetectorViewModel(_ControllerVM, dm);
            Detectoren.Add(dvm1);
        }

        bool AddNewDetectorCommand_CanExecute(object prm)
        {
            return Detectoren != null;
        }

        void RemoveDetectorCommand_Executed(object prm)
        {
            if (SelectedDetectoren != null && SelectedDetectoren.Count > 0)
            {
                // Create temporary List cause we cannot directly remove the selection,
                // as it will cause the selection to change while we loop it
                List<DetectorViewModel> ldvm = new List<DetectorViewModel>();
                foreach (DetectorViewModel dvm in SelectedDetectoren)
                {
                    ldvm.Add(dvm);
                }
                foreach (DetectorViewModel dvm in ldvm)
                {
                    Detectoren.Remove(dvm);
                }
            }
            else if (SelectedDetector != null)
            {
                Detectoren.Remove(SelectedDetector);
            }
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return Detectoren != null &&
                (SelectedDetector != null ||
                 SelectedDetectoren != null && SelectedDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public DetectorenExtraLijstViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
        }

        #endregion // Constructor
    }
}
