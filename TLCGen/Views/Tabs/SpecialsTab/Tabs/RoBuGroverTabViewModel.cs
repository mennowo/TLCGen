using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.SpecialsTab)]
    public class RoBuGroverTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RoBuGroverInstellingenViewModel _roBuGroverInstellingenInstellingen;
        private RoBuGroverConflictGroepViewModel _SelectedConflictGroep;
        private RoBuGroverSignaalGroepInstellingenViewModel _SelectedSignaalGroepInstelling;

        private ObservableCollection<RoBuGroverTabFaseViewModel> _Fasen;
        private RoBuGroverTabFaseViewModel _SelectedFaseCyclus;

        private Dictionary<string, string> _ControllerRGVFileDetectoren;
        private Dictionary<string, string> _ControllerRGVHiaatDetectoren;

        private bool _AutomaticallySetSelectableSignalGroups;

        #endregion // Fields

        #region Properties

        public RoBuGroverModel RoBuGrover
        {
            get { return _Controller.RoBuGrover; }
        }

        public bool AutomaticallySetSelectableSignalGroups
        {
            get { return _AutomaticallySetSelectableSignalGroups; }
            set
            {
                _AutomaticallySetSelectableSignalGroups = value;
                Messenger.Default.Send(new SelectedConflictGroepChangedMessage(_SelectedConflictGroep?.ConflictGroep, null, AutomaticallySetSelectableSignalGroups));
                RaisePropertyChanged("AutomaticallySetSelectableSignalGroups");
            }
        }

        public RoBuGroverConflictGroepViewModel SelectedConflictGroep
        {
            get { return _SelectedConflictGroep; }
            set
            {
                var oldval = _SelectedConflictGroep;
                _SelectedConflictGroep = value;
                RaisePropertyChanged<object>("SelectedConflictGroep", null, null, true);
                Messenger.Default.Send(new SelectedConflictGroepChangedMessage(_SelectedConflictGroep?.ConflictGroep, oldval?.ConflictGroep, AutomaticallySetSelectableSignalGroups));
            }
        }

        public RoBuGroverSignaalGroepInstellingenViewModel SelectedSignaalGroepInstelling
        {
            get { return _SelectedSignaalGroepInstelling; }
            set
            {
                _SelectedSignaalGroepInstelling = value;
                RaisePropertyChanged<object>("SelectedSignaalGroepInstelling", null, null, true);
            }
        }

        public RoBuGroverTabFaseViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged("SelectedFaseCyclus");
            }
        }

        public ObservableCollectionAroundList<RoBuGroverConflictGroepViewModel, RoBuGroverConflictGroepModel> ConflictGroepen
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RoBuGroverSignaalGroepInstellingenViewModel, RoBuGroverFaseCyclusInstellingenModel> SignaalGroepInstellingen
        {
            get;
            private set;
        }

        public ObservableCollection<RoBuGroverTabFaseViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<RoBuGroverTabFaseViewModel>();
                }
                return _Fasen;
            }
        }

        public RoBuGroverInstellingenViewModel RoBuGroverInstellingen
        {
            get
            {
                if(_roBuGroverInstellingenInstellingen == null)
                {
                    _roBuGroverInstellingenInstellingen = new RoBuGroverInstellingenViewModel(_Controller.RoBuGrover);
                }
                return _roBuGroverInstellingenInstellingen;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddConflictGroepCommand;
        public ICommand AddConflictGroepCommand
        {
            get
            {
                if (_AddConflictGroepCommand == null)
                {
                    _AddConflictGroepCommand = new RelayCommand(AddConflictGroepCommand_Executed, AddConflictGroepCommand_CanExecute);
                }
                return _AddConflictGroepCommand;
            }
        }

        RelayCommand _RemoveConflictGroepCommand;
        public ICommand RemoveConflictGroepCommand
        {
            get
            {
                if (_RemoveConflictGroepCommand == null)
                {
                    _RemoveConflictGroepCommand = new RelayCommand(RemoveConflictGroepCommand_Executed, RemoveConflictGroepCommand_CanExecute);
                }
                return _RemoveConflictGroepCommand;
            }
        }

        RelayCommand _AddRemoveFaseCommand;
        public ICommand AddRemoveFaseCommand
        {
            get
            {
                if (_AddRemoveFaseCommand == null)
                {
                    _AddRemoveFaseCommand = new RelayCommand(AddRemoveFaseCommand_Executed, AddRemoveFaseCommand_CanExecute);
                }
                return _AddRemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddConflictGroepCommand_CanExecute(object obj)
        {
            return true;
        }

        private void AddConflictGroepCommand_Executed(object obj)
        {
            RoBuGroverConflictGroepModel cgm = new RoBuGroverConflictGroepModel();
            ConflictGroepen.Add(new RoBuGroverConflictGroepViewModel(cgm));
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        private bool RemoveConflictGroepCommand_CanExecute(object obj)
        {
            return SelectedConflictGroep != null;
        }

        private void RemoveConflictGroepCommand_Executed(object obj)
        {
            var _SelectedConflictGroep = SelectedConflictGroep;
            ConflictGroepen.Remove(SelectedConflictGroep);

            if (ConflictGroepen.Count == 0)
            {
                SignaalGroepInstellingen.RemoveAll();
            }
            else if(SignaalGroepInstellingen.Count > 0)
            {
                foreach(var fc in _SelectedConflictGroep.Fasen)
                {

                    if ((!ConflictGroepen.SelectMany(x => x.Fasen).Any() ||
                         !ConflictGroepen.SelectMany(x => x.Fasen).Where(y => y.FaseCyclus == fc.FaseCyclus).Any()) &&
                          SignaalGroepInstellingen.Where(x => x.FaseCyclus == fc.FaseCyclus).Any())
                    {
                        var instvm = SignaalGroepInstellingen.Where(x => x.FaseCyclus == fc.FaseCyclus).First();
                        SignaalGroepInstellingen.Remove(instvm);
                        SignaalGroepInstellingen.BubbleSort();
                        SignaalGroepInstellingen.RebuildList();
                    }
                }
            }

            SelectedConflictGroep = null;
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        void AddRemoveFaseCommand_Executed(object prm)
        {
            var fc = prm as RoBuGroverTabFaseViewModel;
            SelectedFaseCyclus = fc;
            if (fc.CanBeAddedToConflictGroep && !fc.IsInConflictGroep)
            {
                var fcm = new RoBuGroverConflictGroepFaseModel();
                fcm.FaseCyclus = fc.FaseCyclusNaam;
                var fcvm = new RoBuGroverConflictGroepFaseViewModel(fcm);
                SelectedConflictGroep.Fasen.Add(fcvm);
                if(!SignaalGroepInstellingen.Where(x => x.FaseCyclus == fc.FaseCyclusNaam).Any())
                {
                    var inst = new RoBuGroverFaseCyclusInstellingenModel();
                    inst.FaseCyclus = fc.FaseCyclusNaam;
                    if (Controller.Fasen.Where(x => x.Naam == fc.FaseCyclusNaam).Any())
                    {
                        var type = Controller.Fasen.Where(x => x.Naam == fc.FaseCyclusNaam).First().Type.ToString();
                        DefaultsProvider.Default.SetDefaultsOnModel(inst, type);
                    }
                    var instvm = new RoBuGroverSignaalGroepInstellingenViewModel(inst);
                    try
                    {
                        var addfc = _Controller.Fasen.Where(x => x.Naam == instvm.FaseCyclus).First();
                        if(addfc.Type != FaseTypeEnum.Fiets || addfc.Type == FaseTypeEnum.Voetganger)
                        foreach (DetectorModel dm in addfc.Detectoren)
                        {
                            if (dm.Type == DetectorTypeEnum.Lang)
                            {
                                var hd = new RoBuGroverHiaatDetectorModel();
                                hd.Detector = dm.Naam;
                                DefaultsProvider.Default.SetDefaultsOnModel(hd);
                                instvm.HiaatDetectoren.Add(new RoBuGroverHiaatDetectorViewModel(hd));
                            }
                            if (dm.Type == DetectorTypeEnum.Kop)
                            {
                                var fd = new RoBuGroverFileDetectorModel();
                                fd.Detector = dm.Naam;
                                DefaultsProvider.Default.SetDefaultsOnModel(fd);
                                instvm.FileDetectoren.Add(new RoBuGroverFileDetectorViewModel(fd));
                            }
                        }
                    }
                    catch
                    {

                    }
                    SignaalGroepInstellingen.Add(instvm);
                    SignaalGroepInstellingen.BubbleSort();
                    SignaalGroepInstellingen.RebuildList();
                }
                MessengerInstance.Send(new ControllerDataChangedMessage());
            }
            else if (fc.IsInConflictGroep)
            {
                // Use custom method instead of Remove method:
                // it removes based on name instead of reference
                RoBuGroverConflictGroepFaseViewModel removevm = null;
                removevm = SelectedConflictGroep.Fasen.Where(x => x.FaseCyclus == fc.FaseCyclusNaam).First();
                SelectedConflictGroep.Fasen.Remove(removevm);
                if (!ConflictGroepen.SelectMany(x => x.Fasen).Where(y => y.FaseCyclus == fc.FaseCyclusNaam).Any() &&
                    SignaalGroepInstellingen.Where(x => x.FaseCyclus == fc.FaseCyclusNaam).Any())
                {
                    var instvm = SignaalGroepInstellingen.Where(x => x.FaseCyclus == fc.FaseCyclusNaam).First();
                    SignaalGroepInstellingen.Remove(instvm);
                    SignaalGroepInstellingen.BubbleSort();
                    SignaalGroepInstellingen.RebuildList();
                }
                MessengerInstance.Send(new ControllerDataChangedMessage());
            }
            foreach (RoBuGroverTabFaseViewModel tfc in Fasen)
            {
                tfc.UpdateConflictGroepInfo();
            }
        }

        bool AddRemoveFaseCommand_CanExecute(object prm)
        {
            return SelectedConflictGroep != null;
        }

        #endregion // Command functionality

        #region Private methods

        private void UpdateFasenEnDetectoren()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                var fc = new RoBuGroverTabFaseViewModel(fcm.Naam);
                Fasen.Add(fc);
            }
            SelectedConflictGroep = SelectedConflictGroep;

            _ControllerRGVFileDetectoren = new Dictionary<string, string>();
            _ControllerRGVHiaatDetectoren = new Dictionary<string, string>();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.Type == Models.Enumerations.DetectorTypeEnum.Kop)
                        _ControllerRGVFileDetectoren.Add(dm.Naam, fcm.Naam);
                    if (dm.Type == Models.Enumerations.DetectorTypeEnum.Lang)
                        _ControllerRGVHiaatDetectoren.Add(dm.Naam, fcm.Naam);
                }
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "RoBuGrover"; }
        }

        public override void OnSelected()
        {
            UpdateFasenEnDetectoren();
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
                    ConflictGroepen = new ObservableCollectionAroundList<RoBuGroverConflictGroepViewModel, RoBuGroverConflictGroepModel>(_Controller.RoBuGrover.ConflictGroepen);
                    SignaalGroepInstellingen = new ObservableCollectionAroundList<RoBuGroverSignaalGroepInstellingenViewModel, RoBuGroverFaseCyclusInstellingenModel>(_Controller.RoBuGrover.SignaalGroepInstellingen);
                }
                else
                {
                    ConflictGroepen = null;
                    SignaalGroepInstellingen = null;
                }
                RaisePropertyChanged("ConflictGroepen");
                RaisePropertyChanged("SignaalGroepInstellingen");
                AutomaticallySetSelectableSignalGroups = true;
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            SignaalGroepInstellingen.Rebuild();
        }

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            SignaalGroepInstellingen.Rebuild();
        }

        private void OnConflictsChanged(ConflictsChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            ConflictGroepen.Rebuild();
            SignaalGroepInstellingen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public RoBuGroverTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
            Messenger.Default.Register(this, new Action<ConflictsChangedMessage>(OnConflictsChanged));
        }

        #endregion // Constructor
    }
}
