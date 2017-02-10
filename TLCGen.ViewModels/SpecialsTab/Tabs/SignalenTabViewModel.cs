using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.SpecialsTab)]
    public class SignalenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private WaarschuwingsGroepViewModel _SelectedWaarschuwingsGroep;
        private RatelTikkerViewModel _SelectedRatelTikker;

        private ObservableCollection<string> _ControllerFasen;
        private ObservableCollection<string> _SelectableRatelTikkerFasen;
        private ObservableCollection<string> _SelectableRatelTikkerDetectoren;
        private List<string> _ControllerDetectoren;
        private string _SelectedRatelTikkerFaseToAdd;
        private string _SelectedRatelTikkerDetectorToAdd;

        private RelayCommand _AddWaarschuwingsGroepCommand;
        private RelayCommand _RemoveWaarschuwingsGroepCommand;
        private RelayCommand _AddRatelTikkerCommand;
        private RelayCommand _RemoveRatelTikkerCommand;
        private RelayCommand _AddRatelTikkerDetectorCommand;
        private RelayCommand _RemoveRatelTikkerDetectorCommand;

        #endregion // Fields

        #region Properties

        public WaarschuwingsGroepViewModel SelectedWaarschuwingsGroep
        {
            get { return _SelectedWaarschuwingsGroep; }
            set
            {
                _SelectedWaarschuwingsGroep = value;
                OnPropertyChanged("SelectedWaarschuwingsGroep");
            }
        }

        public RatelTikkerViewModel SelectedRatelTikker
        {
            get { return _SelectedRatelTikker; }
            set
            {
                _SelectedRatelTikker = value;
                OnPropertyChanged("SelectedRatelTikker");
                UpdateSelectables();
            }
        }

        public ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel> WaarschuwingsGroepen
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RatelTikkerViewModel, RatelTikkerModel> RatelTikkers
        {
            get;
            private set;
        }

        public ObservableCollection<string> ControllerFasen
        {
            get
            {
                if (_ControllerFasen == null)
                {
                    _ControllerFasen = new ObservableCollection<string>();
                }
                return _ControllerFasen;
            }
        }
        
        public ObservableCollection<string> SelectableRatelTikkerDetectoren
        {
            get
            {
                if (_SelectableRatelTikkerDetectoren == null)
                {
                    _SelectableRatelTikkerDetectoren = new ObservableCollection<string>();
                }
                return _SelectableRatelTikkerDetectoren;
            }
        }

        public ObservableCollection<string> SelectableRatelTikkerFasen
        {
            get
            {
                if (_SelectableRatelTikkerFasen == null)
                {
                    _SelectableRatelTikkerFasen = new ObservableCollection<string>();
                }
                return _SelectableRatelTikkerFasen;
            }
        }

        public string SelectedRatelTikkerDetectorToAdd
        {
            get { return _SelectedRatelTikkerDetectorToAdd; }
            set
            {
                _SelectedRatelTikkerDetectorToAdd = value;
                OnPropertyChanged("SelectedRatelTikkerDetectorToAdd");
            }
        }

        public string SelectedRatelTikkerFaseToAdd
        {
            get { return _SelectedRatelTikkerFaseToAdd; }
            set
            {
                _SelectedRatelTikkerFaseToAdd = value;
                OnPropertyChanged("SelectedRatelTikkerFaseToAdd");
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddWaarschuwingsGroepCommand
        {
            get
            {
                if (_AddWaarschuwingsGroepCommand == null)
                {
                    _AddWaarschuwingsGroepCommand = new RelayCommand(AddWaarschuwingsGroepCommand_Executed, AddWaarschuwingsGroepCommand_CanExecute);
                }
                return _AddWaarschuwingsGroepCommand;
            }
        }

        public ICommand RemoveWaarschuwingsGroepCommand
        {
            get
            {
                if (_RemoveWaarschuwingsGroepCommand == null)
                {
                    _RemoveWaarschuwingsGroepCommand = new RelayCommand(RemoveWaarschuwingsGroepCommand_Executed, RemoveWaarschuwingsGroepCommand_CanExecute);
                }
                return _RemoveWaarschuwingsGroepCommand;
            }
        }

        public ICommand AddRatelTikkerCommand
        {
            get
            {
                if (_AddRatelTikkerCommand == null)
                {
                    _AddRatelTikkerCommand = new RelayCommand(AddRatelTikkerCommand_Executed, AddRatelTikkerCommand_CanExecute);
                }
                return _AddRatelTikkerCommand;
            }
        }

        public ICommand RemoveRatelTikkerCommand
        {
            get
            {
                if (_RemoveRatelTikkerCommand == null)
                {
                    _RemoveRatelTikkerCommand = new RelayCommand(RemoveRatelTikkerCommand_Executed, RemoveRatelTikkerCommand_CanExecute);
                }
                return _RemoveRatelTikkerCommand;
            }
        }

        public ICommand AddRatelTikkerDetectorCommand
        {
            get
            {
                if (_AddRatelTikkerDetectorCommand == null)
                {
                    _AddRatelTikkerDetectorCommand = new RelayCommand(AddRatelTikkerDetectorCommand_Executed, AddRatelTikkerDetectorCommand_CanExecute);
                }
                return _AddRatelTikkerDetectorCommand;
            }
        }

        public ICommand RemoveRatelTikkerDetectorCommand
        {
            get
            {
                if (_RemoveRatelTikkerDetectorCommand == null)
                {
                    _RemoveRatelTikkerDetectorCommand = new RelayCommand(RemoveRatelTikkerDetectorCommand_Executed, RemoveRatelTikkerDetectorCommand_CanExecute);
                }
                return _RemoveRatelTikkerDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddWaarschuwingsGroepCommand_Executed(object prm)
        {
            var grm = new WaarschuwingsGroepModel();
            int i = WaarschuwingsGroepen.Count + 1;
            grm.Naam = "groep" + i.ToString();
            while (!Integrity.IntegrityChecker.IsElementNaamUnique(_Controller, grm.Naam))
            {
                ++i;
                grm.Naam = "groep" + i.ToString();
            }
            WaarschuwingsGroepViewModel grvm = new WaarschuwingsGroepViewModel(grm);
            WaarschuwingsGroepen.Add(grvm);
            SelectedWaarschuwingsGroep = grvm;
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            UpdateSelectables();
        }

        bool AddWaarschuwingsGroepCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveWaarschuwingsGroepCommand_Executed(object prm)
        {
            int id = WaarschuwingsGroepen.IndexOf(SelectedWaarschuwingsGroep);
            WaarschuwingsGroepen.Remove(SelectedWaarschuwingsGroep);
            SelectedWaarschuwingsGroep = null;
            if(WaarschuwingsGroepen.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= WaarschuwingsGroepen.Count ? WaarschuwingsGroepen.Count - 1 : id;
                SelectedWaarschuwingsGroep = WaarschuwingsGroepen[id];
            }
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            UpdateSelectables();
        }

        bool RemoveWaarschuwingsGroepCommand_CanExecute(object prm)
        {
            return SelectedWaarschuwingsGroep != null;
        }

        void AddRatelTikkerCommand_Executed(object prm)
        {
            int id = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
            var rtm = new RatelTikkerModel();
            rtm.FaseCyclus = SelectedRatelTikkerFaseToAdd;
            foreach(var fc in _Controller.Fasen)
            {
                if(fc.Naam == SelectedRatelTikkerFaseToAdd)
                {
                    foreach(var d in fc.Detectoren)
                    {
                        rtm.Detectoren.Add(new RatelTikkerDetectorModel() { Detector = d.Naam });
                    }
                }
            }
            RatelTikkerViewModel rtvm = new RatelTikkerViewModel(rtm);
            RatelTikkers.Add(rtvm);
            SelectedRatelTikker = rtvm;
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            UpdateSelectables();
            if (SelectableRatelTikkerFasen.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= SelectableRatelTikkerFasen.Count ? SelectableRatelTikkerFasen.Count - 1 : id;
                SelectedRatelTikkerFaseToAdd = SelectableRatelTikkerFasen[id];
            }
        }

        bool AddRatelTikkerCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveRatelTikkerCommand_Executed(object prm)
        {
            int id = RatelTikkers.IndexOf(SelectedRatelTikker);
            int id2 = SelectableRatelTikkerFasen.IndexOf(SelectedRatelTikkerFaseToAdd);
            RatelTikkers.Remove(SelectedRatelTikker);
            UpdateSelectables();
            SelectedRatelTikker = null;
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            if (RatelTikkers.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= RatelTikkers.Count ? RatelTikkers.Count - 1 : id;
                SelectedRatelTikker = RatelTikkers[id];
            }
            if (SelectableRatelTikkerFasen.Count > 0)
            {
                id2 = id2 < 0 ? 0 : id2;
                id2 = id2 >= SelectableRatelTikkerFasen.Count ? SelectableRatelTikkerFasen.Count - 1 : id2;
                SelectedRatelTikkerFaseToAdd = SelectableRatelTikkerFasen[id2];
            }
        }

        bool RemoveRatelTikkerCommand_CanExecute(object prm)
        {
            return SelectedRatelTikker != null;
        }

        void AddRatelTikkerDetectorCommand_Executed(object prm)
        {
            int id = SelectedRatelTikker.Detectoren.IndexOf(SelectedRatelTikker.SelectedDetector);
            SelectedRatelTikker.Detectoren.Add(
                new RatelTikkerDetectorViewModel(
                    new RatelTikkerDetectorModel()
                    {
                        Detector = SelectedRatelTikkerDetectorToAdd
                    }));
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            SelectedRatelTikker.SelectedDetector = null;
            UpdateSelectables();
            if (SelectedRatelTikker.Detectoren.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= SelectedRatelTikker.Detectoren.Count ? SelectedRatelTikker.Detectoren.Count - 1 : id;
                SelectedRatelTikker.SelectedDetector = SelectedRatelTikker.Detectoren[id];
            }
        }

        bool AddRatelTikkerDetectorCommand_CanExecute(object prm)
        {
            return SelectedRatelTikker != null && SelectedRatelTikkerDetectorToAdd != null;
        }

        void RemoveRatelTikkerDetectorCommand_Executed(object prm)
        {
            int id = SelectedRatelTikker.Detectoren.IndexOf(SelectedRatelTikker.SelectedDetector);
            SelectedRatelTikker.Detectoren.Remove(SelectedRatelTikker.SelectedDetector);
            SelectedRatelTikker.SelectedDetector = null;
            UpdateSelectables();
            if (SelectedRatelTikker.Detectoren.Count > 0)
            {
                id = id < 0 ? 0 : id;
                id = id >= SelectedRatelTikker.Detectoren.Count ? SelectedRatelTikker.Detectoren.Count - 1 : id;
                SelectedRatelTikker.SelectedDetector = SelectedRatelTikker.Detectoren[id];
            }
            Messenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }

        bool RemoveRatelTikkerDetectorCommand_CanExecute(object prm)
        {
            return SelectedRatelTikker != null;
        }

        #endregion // Command functionality

        #region Private methods

        private void UpdateSelectables()
        {
            string tempfc = SelectedRatelTikkerFaseToAdd;
            string tempd = SelectedRatelTikkerDetectorToAdd;
            SelectableRatelTikkerFasen.Clear();
            if (ControllerFasen.Count > 0 && RatelTikkers.Count > 0)
            {
                foreach (var fc in ControllerFasen)
                {
                    if (!RatelTikkers.Where(x => x.FaseCyclus == fc).Any())
                    {
                        SelectableRatelTikkerFasen.Add(fc);
                    }
                }
            }
            else
            {
                foreach (var fc in ControllerFasen)
                {
                    SelectableRatelTikkerFasen.Add(fc);
                }
            }
            if (_SelectableRatelTikkerFasen.Count > 0)
            {
                SelectedRatelTikkerFaseToAdd = tempfc == null ?
                    SelectableRatelTikkerFasen[0] :
                    tempd;
            }
            else
            {
                SelectedRatelTikkerFaseToAdd = null;
            }

            SelectableRatelTikkerDetectoren.Clear();
            if (SelectedRatelTikker?.Detectoren.Count > 0)
            {
                foreach (var d in _ControllerDetectoren)
                {
                    if (!SelectedRatelTikker.Detectoren.Where(x => x.Detector == d).Any())
                    {
                        SelectableRatelTikkerDetectoren.Add(d);
                    }
                }
            }
            else
            {
                foreach (var d in _ControllerDetectoren)
                {
                    SelectableRatelTikkerDetectoren.Add(d);
                }
            }

            if (_SelectableRatelTikkerDetectoren.Count > 0)
            {
                SelectedRatelTikkerDetectorToAdd = tempd == null ?
                    SelectableRatelTikkerDetectoren[0] :
                    tempd;
            }
            else
            {
                SelectedRatelTikkerDetectorToAdd = null;
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "Signalen"; }
        }

        public override void OnSelected()
        {
            ControllerFasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam);
            }

            _ControllerDetectoren = new List<string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    _ControllerDetectoren.Add(dm.Naam);
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                if (dm.Type == Models.Enumerations.DetectorTypeEnum.File)
                    _ControllerDetectoren.Add(dm.Naam);
            }

            UpdateSelectables();
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    WaarschuwingsGroepen = new ObservableCollectionAroundList<WaarschuwingsGroepViewModel, WaarschuwingsGroepModel>(_Controller.Signalen.WaarschuwingsGroepen);
                    RatelTikkers = new ObservableCollectionAroundList<RatelTikkerViewModel, RatelTikkerModel>(_Controller.Signalen.Rateltikkers);
                }
                else
                {
                    WaarschuwingsGroepen = null;
                    RatelTikkers = null;
                }
                OnPropertyChanged("WaarschuwingsGroepen");
                OnPropertyChanged("RatelTikkers");
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            WaarschuwingsGroepen.Rebuild();
            RatelTikkers.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SignalenTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
        }

        #endregion // Constructor
    }
}
