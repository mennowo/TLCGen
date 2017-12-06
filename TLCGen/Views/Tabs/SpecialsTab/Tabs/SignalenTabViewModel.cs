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
        private List<string> _ControllerDetectoren;
        private string _SelectedRatelTikkerFaseToAdd;

        private RelayCommand _AddWaarschuwingsGroepCommand;
        private RelayCommand _RemoveWaarschuwingsGroepCommand;
        private RelayCommand _AddRatelTikkerCommand;
        private RelayCommand _RemoveRatelTikkerCommand;

        #endregion // Fields

        #region Properties

        public WaarschuwingsGroepViewModel SelectedWaarschuwingsGroep
        {
            get { return _SelectedWaarschuwingsGroep; }
            set
            {
                _SelectedWaarschuwingsGroep = value;
                RaisePropertyChanged("SelectedWaarschuwingsGroep");
            }
        }

        public RatelTikkerViewModel SelectedRatelTikker
        {
            get { return _SelectedRatelTikker; }
            set
            {
                _SelectedRatelTikker = value;
                RaisePropertyChanged("SelectedRatelTikker");
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

        public string SelectedRatelTikkerFaseToAdd
        {
            get { return _SelectedRatelTikkerFaseToAdd; }
            set
            {
                _SelectedRatelTikkerFaseToAdd = value;
                RaisePropertyChanged("SelectedRatelTikkerFaseToAdd");
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

        #endregion // Commands

        #region Command functionality

        void AddWaarschuwingsGroepCommand_Executed(object prm)
        {
            var grm = new WaarschuwingsGroepModel();
            int i = WaarschuwingsGroepen.Count + 1;
            grm.Naam = "groep" + i.ToString();
            while (!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, grm.Naam))
            {
                ++i;
                grm.Naam = "groep" + i.ToString();
            }
            var grvm = new WaarschuwingsGroepViewModel(grm);
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
            var rtm = new RatelTikkerModel()
            {
                FaseCyclus = SelectedRatelTikkerFaseToAdd
            };
            foreach (var fc in _Controller.Fasen)
            {
                if(fc.Naam == SelectedRatelTikkerFaseToAdd)
                {
                    foreach(var d in fc.Detectoren)
                    {
                        if(d.Type == Models.Enumerations.DetectorTypeEnum.Knop ||
                           d.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen ||
                           d.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)
                        rtm.Detectoren.Add(new RatelTikkerDetectorModel { Detector = d.Naam });
                    }
                }
            }
            var rtvm = new RatelTikkerViewModel(rtm);
            RatelTikkers.Add(rtvm);
            SelectedRatelTikker = rtvm;
            Messenger.Default.Send(new ControllerDataChangedMessage());
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

        #endregion // Command functionality

        #region Private methods

        private void UpdateSelectables()
        {
            ControllerFasen.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                ControllerFasen.Add(fcm.Naam);
            }

            _ControllerDetectoren = new List<string>();
            foreach (var fcm in _Controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    _ControllerDetectoren.Add(dm.Naam);
                }
            }
            foreach (var dm in _Controller.Detectoren)
            {
                _ControllerDetectoren.Add(dm.Naam);
            }

            string tempfc = SelectedRatelTikkerFaseToAdd;
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
                    tempfc;
            }
            else
            {
                SelectedRatelTikkerFaseToAdd = null;
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
                RaisePropertyChanged("WaarschuwingsGroepen");
                RaisePropertyChanged("RatelTikkers");
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RatelTikkers.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            UpdateSelectables();
            WaarschuwingsGroepen.Rebuild();
            RatelTikkers.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public SignalenTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor
    }
}
