using GalaSoft.MvvmLight.Messaging;
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
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Operations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the View of detectors belonging to phases.
    /// </summary>
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenFasenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<string> _Templates;
        private string _SelectedFaseNaam;
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();
        private List<string> _Fasen;
        private FaseCyclusModel _SelectedFase;

        #endregion // Fields

        #region Properties


        public List<string> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new List<string>();
                }
                return _Fasen;
            }
        }

        private ObservableCollection<DetectorViewModel> _Detectoren;
        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                return _Detectoren;
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

        public string SelectedFaseNaam
        {
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                _SelectedFase = _Controller.Fasen.Where(x => x.Naam == value).First();

                Detectoren.Clear();
                foreach(DetectorModel dm in _SelectedFase.Detectoren)
                {
                    Detectoren.Add(new DetectorViewModel(dm) { FaseCyclus = _SelectedFaseNaam });
                }
                if(Detectoren.Count > 0)
                    SelectedDetector = Detectoren[0];

                OnPropertyChanged("SelectedFaseNaam");
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
            DetectorModel _dm = new DetectorModel();
            string newname = "1";
            int inewname = 1;
            foreach (DetectorModel dm in _SelectedFase.Detectoren)
            {
                if (Regex.IsMatch(dm.Naam, @"[0-9]$"))
                {

                    Match m = Regex.Match(dm.Naam, @"[0-9]$");
                    string next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        var message = new IsElementIdentifierUniqueRequest("", ElementIdentifierType.Naam);
                        do
                        {
                            newname = inewname.ToString();
                            message = new IsElementIdentifierUniqueRequest(_SelectedFase.Naam + newname, ElementIdentifierType.Naam);
                            Messenger.Default.Send(message);
                            if(!message.IsUnique)
                                inewname++;
                        }
                        while (!message.IsUnique);
                    }
                }
            }
            _dm.Naam = _SelectedFase.Naam + newname;
            _dm.VissimNaam = _dm.Naam;
            DetectorViewModel dvm1 = new DetectorViewModel(_dm);
            dvm1.FaseCyclus = _SelectedFase.Naam;
            _SelectedFase.Detectoren.Add(_dm);
            _Detectoren.Add(dvm1);
        }

        bool AddNewDetectorCommand_CanExecute(object prm)
        {
            return _SelectedFase != null && _SelectedFase.Detectoren != null;
        }

        void RemoveDetectorCommand_Executed(object prm)
        {
            bool changed = false;
            if (SelectedDetectoren != null && SelectedDetectoren.Count > 0)
            {
                changed = true;
                foreach (DetectorViewModel dvm in SelectedDetectoren)
                {
                    ControllerModifier.RemoveDetectorFromController(_Controller, dvm.Naam);
                }
            }
            else if (SelectedDetector != null)
            {
                changed = true;
                ControllerModifier.RemoveDetectorFromController(_Controller, SelectedDetector.Naam);
            }

            if (changed)
            {
                SelectedFaseNaam = SelectedFaseNaam;
            }
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return _SelectedFase != null &&
                   _SelectedFase.Detectoren != null &&
                   (SelectedDetector != null ||
                    SelectedDetectoren != null && SelectedDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Fasen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(fcm.Naam);
            }
        }

        public override void OnDeselected()
        {
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                fcm.Detectoren.BubbleSort();
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public DetectorenFasenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
