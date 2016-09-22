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
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the View of detectors belonging to phases.
    /// </summary>
    public class DetectorenFasenLijstViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ObservableCollection<string> _Templates;
        private FaseCyclusViewModel _SelectedFase;
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();

        #endregion // Fields

        #region Properties


        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                return _ControllerVM.Fasen;
            }
        }

        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                return SelectedFase?.Detectoren;
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

        public FaseCyclusViewModel SelectedFase
        {
            get { return _SelectedFase; }
            set
            {
                _SelectedFase = value;
                OnPropertyChanged("SelectedFase");
                OnPropertyChanged("Detectoren");
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
            string newname = "1";
            int inewname = 1;
            foreach (DetectorViewModel dvm in SelectedFase.Detectoren)
            {
                if (Regex.IsMatch(dvm.Naam, @"[0-9]$"))
                {

                    Match m = Regex.Match(dvm.Naam, @"[0-9]$");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        inewname++;
                        newname = inewname.ToString();
                        while (!_ControllerVM.IsElementNaamUnique(SelectedFase.Naam + newname))
                        {
                            inewname++;
                            newname = inewname.ToString();
                        }
                    }
                }
            }
            dm.Define = SettingsProvider.GetDetectorDefinePrefix() + SelectedFase.Naam + newname;
            dm.Naam = SelectedFase.Naam + newname;
            DetectorViewModel dvm1 = new DetectorViewModel(_ControllerVM, dm);
            dvm1.FaseVM = SelectedFase;
            SelectedFase.Detectoren.Add(dvm1);
        }

        bool AddNewDetectorCommand_CanExecute(object prm)
        {
            return SelectedFase != null && SelectedFase.Detectoren != null;
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
                    SelectedFase.Detectoren.Remove(dvm);
                }
            }
            else if (SelectedDetector != null)
            {
                SelectedFase.Detectoren.Remove(SelectedDetector);
            }
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return SelectedFase != null &&
                   SelectedFase.Detectoren != null &&
                   (SelectedDetector != null ||
                    SelectedDetectoren != null && SelectedDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public DetectorenFasenLijstViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
        }

        #endregion // Constructor
    }
}
