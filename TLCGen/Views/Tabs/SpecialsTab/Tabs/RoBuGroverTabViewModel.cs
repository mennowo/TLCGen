
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
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
        private RoBuGroverConflictGroepViewModel _selectedConflictGroep;
        private RoBuGroverSignaalGroepInstellingenViewModel _selectedSignaalGroepInstelling;

        private ObservableCollection<RoBuGroverTabFaseViewModel> _fasen;
        private RoBuGroverTabFaseViewModel _selectedFaseCyclus;

        private Dictionary<string, string> _controllerRgvFileDetectoren;
        private Dictionary<string, string> _controllerRgvHiaatDetectoren;
        
        private RelayCommand _addConflictGroepCommand;
        private RelayCommand _removeConflictGroepCommand;
        private RelayCommand<object> _addRemoveFaseCommand;

        #endregion // Fields

        #region Properties

        public RoBuGroverModel RoBuGrover => _Controller.RoBuGrover;

        public bool ToestaanNietConflictenInConflictGroepen
        {
            get => RoBuGrover.ToestaanNietConflictenInConflictGroepen;
            set
            {
                RoBuGrover.ToestaanNietConflictenInConflictGroepen = value;
WeakReferenceMessenger.Default.Send(new SelectedConflictGroepChangedMessage(_selectedConflictGroep?.ConflictGroep, null, !RoBuGrover.ToestaanNietConflictenInConflictGroepen));
                TLCGenControllerModifier.Default.CorrectModel_AlteredConflicts();
                OnConflictsChanged(this, new ConflictsChangedMessage());
                OnPropertyChanged(broadcast: true);
            }
        }

        public RoBuGroverConflictGroepViewModel SelectedConflictGroep
        {
            get => _selectedConflictGroep;
            set
            {
                var oldval = _selectedConflictGroep;
                _selectedConflictGroep = value;
                OnPropertyChanged();
WeakReferenceMessenger.Default.Send(new SelectedConflictGroepChangedMessage(_selectedConflictGroep?.ConflictGroep, oldval?.ConflictGroep, !RoBuGrover.ToestaanNietConflictenInConflictGroepen));
            }
        }

        public RoBuGroverSignaalGroepInstellingenViewModel SelectedSignaalGroepInstelling
        {
            get => _selectedSignaalGroepInstelling;
            set
            {
                _selectedSignaalGroepInstelling = value;
                // TODO Check OK ? OnPropertyChanged(nameof(SelectedSignaalGroepInstelling), null, null, true);
                OnPropertyChanged(broadcast: true);
            }
        }

        public RoBuGroverTabFaseViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value;
                OnPropertyChanged();
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

        public ObservableCollection<RoBuGroverTabFaseViewModel> Fasen => _fasen ??= new ObservableCollection<RoBuGroverTabFaseViewModel>();

        public RoBuGroverInstellingenViewModel RoBuGroverInstellingen =>
            _roBuGroverInstellingenInstellingen ??= new RoBuGroverInstellingenViewModel(_Controller.RoBuGrover);

        #endregion // Properties

        #region Commands

        public ICommand AddConflictGroepCommand
        {
            get
            {
                if (_addConflictGroepCommand == null)
                {
                    _addConflictGroepCommand = new RelayCommand(AddConflictGroepCommand_Executed, AddConflictGroepCommand_CanExecute);
                }
                return _addConflictGroepCommand;
            }
        }

        public ICommand RemoveConflictGroepCommand
        {
            get
            {
                if (_removeConflictGroepCommand == null)
                {
                    _removeConflictGroepCommand = new RelayCommand(RemoveConflictGroepCommand_Executed, RemoveConflictGroepCommand_CanExecute);
                }
                return _removeConflictGroepCommand;
            }
        }

        public ICommand AddRemoveFaseCommand
        {
            get
            {
                if (_addRemoveFaseCommand == null)
                {
                    _addRemoveFaseCommand = new RelayCommand<object>(AddRemoveFaseCommand_Executed, AddRemoveFaseCommand_CanExecute);
                }
                return _addRemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddConflictGroepCommand_CanExecute()
        {
            return true;
        }

        private void AddConflictGroepCommand_Executed()
        {
            var cgm = new RoBuGroverConflictGroepModel();
            ConflictGroepen.Add(new RoBuGroverConflictGroepViewModel(cgm));
            WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
        }

        private bool RemoveConflictGroepCommand_CanExecute()
        {
            return SelectedConflictGroep != null;
        }

        private void RemoveConflictGroepCommand_Executed()
        {
            var selectedConflictGroep = SelectedConflictGroep;
            ConflictGroepen.Remove(SelectedConflictGroep);

            if (ConflictGroepen.Count == 0)
            {
                SignaalGroepInstellingen.RemoveAll();
            }
            else if(SignaalGroepInstellingen.Count > 0)
            {
                foreach(var fc in selectedConflictGroep.Fasen)
                {

                    if ((!ConflictGroepen.SelectMany(x => x.Fasen).Any() || 
                         ConflictGroepen.SelectMany(x => x.Fasen).All(y => y.FaseCyclus != fc.FaseCyclus)) &&
                         SignaalGroepInstellingen.Any(x => x.FaseCyclus == fc.FaseCyclus))
                    {
                        var instvm = SignaalGroepInstellingen.First(x => x.FaseCyclus == fc.FaseCyclus);
                        SignaalGroepInstellingen.Remove(instvm);
                        SignaalGroepInstellingen.BubbleSort();
                        SignaalGroepInstellingen.RebuildList();
                    }
                }
            }

            SelectedConflictGroep = null;
            WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
        }

        void AddRemoveFaseCommand_Executed(object prm)
        {
            var fc = prm as RoBuGroverTabFaseViewModel;
            SelectedFaseCyclus = fc;
            if (fc is {CanBeAddedToConflictGroep: true, IsInConflictGroep: false})
            {
                var fcm = new RoBuGroverConflictGroepFaseModel {FaseCyclus = fc.FaseCyclusNaam};
                var fcvm = new RoBuGroverConflictGroepFaseViewModel(fcm);
                SelectedConflictGroep.Fasen.Add(fcvm);
                if(SignaalGroepInstellingen.All(x => x.FaseCyclus != fc.FaseCyclusNaam))
                {
                    var inst = new RoBuGroverFaseCyclusInstellingenModel();
                    inst.FaseCyclus = fc.FaseCyclusNaam;
                    if (Controller.Fasen.Any(x => x.Naam == fc.FaseCyclusNaam))
                    {
                        var type = Controller.Fasen.First(x => x.Naam == fc.FaseCyclusNaam).Type.ToString();
                        DefaultsProvider.Default.SetDefaultsOnModel(inst, type);
                    }
                    var instvm = new RoBuGroverSignaalGroepInstellingenViewModel(inst);
                    try
                    {
                        var addfc = _Controller.Fasen.First(x => x.Naam == instvm.FaseCyclus);
                        if (addfc.Type != FaseTypeEnum.Fiets || addfc.Type == FaseTypeEnum.Voetganger)
                        {
                            foreach (var dm in addfc.Detectoren)
                            {
                                if (dm.Type == DetectorTypeEnum.Lang)
                                {
                                    var hd = new RoBuGroverHiaatDetectorModel {Detector = dm.Naam};
                                    DefaultsProvider.Default.SetDefaultsOnModel(hd);
                                    instvm.HiaatDetectoren.Add(new RoBuGroverHiaatDetectorViewModel(hd));
                                }

                                if (dm.Type == DetectorTypeEnum.Kop)
                                {
                                    var fd = new RoBuGroverFileDetectorModel {Detector = dm.Naam};
                                    DefaultsProvider.Default.SetDefaultsOnModel(fd);
                                    instvm.FileDetectoren.Add(new RoBuGroverFileDetectorViewModel(fd));
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                    SignaalGroepInstellingen.Add(instvm);
                    SignaalGroepInstellingen.BubbleSort();
                    SignaalGroepInstellingen.RebuildList();
                }
                WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
            }
            else if (fc is {IsInConflictGroep: true})
            {
                // Use custom method instead of Remove method:
                // it removes based on name instead of reference
                var removevm = SelectedConflictGroep.Fasen.First(x => x.FaseCyclus == fc.FaseCyclusNaam);
                SelectedConflictGroep.Fasen.Remove(removevm);
                if (ConflictGroepen.SelectMany(x => x.Fasen).All(y => y.FaseCyclus != fc.FaseCyclusNaam) &&
                    SignaalGroepInstellingen.Any(x => x.FaseCyclus == fc.FaseCyclusNaam))
                {
                    var instvm = SignaalGroepInstellingen.First(x => x.FaseCyclus == fc.FaseCyclusNaam);
                    SignaalGroepInstellingen.Remove(instvm);
                    SignaalGroepInstellingen.BubbleSort();
                    SignaalGroepInstellingen.RebuildList();
                }
                WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
            }
            foreach (var tfc in Fasen)
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
            if (_Controller == null) return;
            
            Fasen.Clear();
            foreach (var fc in _Controller.Fasen.Select(fcm => new RoBuGroverTabFaseViewModel(fcm.Naam)))
            {
                Fasen.Add(fc);
            }
            SelectedConflictGroep = SelectedConflictGroep;

            _controllerRgvFileDetectoren = new Dictionary<string, string>();
            _controllerRgvHiaatDetectoren = new Dictionary<string, string>();
            foreach (var fcm in _Controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    if (dm.Type == DetectorTypeEnum.Kop)
                        _controllerRgvFileDetectoren.Add(dm.Naam, fcm.Naam);
                    if (dm.Type == DetectorTypeEnum.Lang)
                        _controllerRgvHiaatDetectoren.Add(dm.Naam, fcm.Naam);
                }
            }
        }

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName => "RoBuGrover";

        public override void OnSelected()
        {
            UpdateFasenEnDetectoren();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

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
                OnPropertyChanged(nameof(ConflictGroepen));
                OnPropertyChanged(nameof(SignaalGroepInstellingen));
            }
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            SignaalGroepInstellingen.Rebuild();
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            SignaalGroepInstellingen.Rebuild();
        }

        private void OnConflictsChanged(object sender, ConflictsChangedMessage message)
        {
            UpdateFasenEnDetectoren();
            ConflictGroepen.Rebuild();
            SignaalGroepInstellingen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public RoBuGroverTabViewModel()
        {
            WeakReferenceMessenger.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessenger.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessenger.Default.Register<ConflictsChangedMessage>(this, OnConflictsChanged);
        }

        #endregion // Constructor
    }
}
