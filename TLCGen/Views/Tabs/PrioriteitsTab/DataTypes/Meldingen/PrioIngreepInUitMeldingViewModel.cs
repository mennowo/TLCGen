using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper
    {
        public PrioIngreepInUitMeldingVoorwaardeTypeEnum Value { get; }
        public string Description { get; }

        public PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum value)
        {
            Value = value;
            Description = Value.GetDescription();
        }
    }

    public class PrioIngreepInUitMeldingViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private readonly PrioIngreepViewModel _ingreep;
        private RelayCommand _removeMeldingCommand;
        private readonly object _parent;
        private ObservableObjectEx _actualViewModel;
        private PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper _prioIngreepInUitMeldingType;
        private ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper> _meldingenTypes = new ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper>();

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get => string.IsNullOrWhiteSpace(PrioIngreepInUitMelding.Naam) ? "geen_naam" : PrioIngreepInUitMelding.Naam;
            set
            {
                PrioIngreepInUitMelding.Naam = value;
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingModel PrioIngreepInUitMelding { get; }
        
        public PrioIngreepInUitMeldingTypeEnum InUit => PrioIngreepInUitMelding.InUit;

        /// <summary>
        /// The melding types that can be chosen (PrioMeldingTypes.GetAllowed). Replaced as a whole by
        /// RefreshAvailableTypes, never cleared: clearing a bound ItemsSource makes the ComboBox write null to Type.
        /// </summary>
        public ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper> MeldingenTypes
        {
            get => _meldingenTypes;
            private set
            {
                _meldingenTypes = value;
                OnPropertyChanged();
            }
        }

        public PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper Type
        {
            get => _prioIngreepInUitMeldingType;
            set
            {
                // The ComboBox writes null while its ItemsSource is replaced; that is not a choice of the user
                if (value == null) return;
                _prioIngreepInUitMeldingType = value;
                OnMeldingTypeChanged();
            }
        }
        
        public bool HasKAR => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;
        public bool HasSD => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;
        public bool HasInpSD => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector;
        public bool HasDet => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;
        public bool HasRis => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde;
        public bool HasInp => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang;
        public bool HasOpticom => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom;

        public ObservableObjectEx ActualViewModel
        {
            get => _actualViewModel;
            set
            {
                _actualViewModel = value; 
                OnPropertyChanged();
            }
        }

        public PrioIngreepRegularMeldingViewModel ViewModel { get; }

        public PrioIngreepRISMeldingViewModel RISViewModel { get; }

        public PrioIngreepPelotonMeldingViewModel PelotonViewModel { get; }

        public PrioIngreepFietsPrioriteitMeldingViewModel FietsPrioriteitViewModel { get; set; }

        public PrioIngreepOpticomMeldingViewModel OpticomViewModel { get; }

        /// <summary>
        /// Whether this is the fallback melding (melding bij storing) of another melding.
        /// </summary>
        public bool IsMeldingBijStoring => _parent is PrioIngreepInUitMeldingViewModel;

        /// <summary>
        /// Opvang storing is only possible for Detector and Vecom meldingen, and not for a melding bij storing itself.
        /// </summary>
        public bool CanHaveOpvangStoring => !IsMeldingBijStoring && Type != null && PrioMeldingTypes.SupportsOpvangStoring(Type.Value);

        public bool OpvangStoring
        {
            get => PrioIngreepInUitMelding.OpvangStoring;
            set
            {
                PrioIngreepInUitMelding.OpvangStoring = value;
                if (value)
                {
                    PrioIngreepInUitMelding.MeldingBijstoring = new PrioIngreepInUitMeldingModel()
                    {
                        InUit = this.InUit,
                        Type = PrioMeldingTypes.MeldingBijStoringType
                    };
                    MeldingBijstoring.Add(new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring, this, _ingreep));
                }
                else
                {
                    MeldingBijstoring.Clear();
                    PrioIngreepInUitMelding.MeldingBijstoring = null;
                }
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(MeldingBijstoring));
            }
        }
        
        public ObservableCollection<PrioIngreepInUitMeldingViewModel> MeldingBijstoring { get; set; } = new ObservableCollection<PrioIngreepInUitMeldingViewModel>();

        #endregion // Properties

        #region Commands

        
        public ICommand RemoveMeldingCommand => _removeMeldingCommand ??= new RelayCommand(() =>
            {
                switch (_parent)
                {
                    case PrioIngreepInUitMeldingViewModel iu:
                        iu.OpvangStoring = false;
                        break;
                    case PrioIngreepMeldingenListViewModel list:
                        list.Meldingen.Remove(this);
                        if (PrioIngreepInUitMelding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
                        {
                            foreach (var melding in list.Meldingen) melding.RefreshAvailableTypes();
                        }
                        WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(_ingreep.PrioIngreep.FaseCyclus, PrioIngreepInUitMelding, true));
                        break;
                }
            });

        #endregion // Commands

        #region TLCGen events

        #endregion // TLCGen events

        #region IViewModelWithItem

        public object GetItem()
        {
            return PrioIngreepInUitMelding;
        }

        #endregion // IViewModelWithItem

        #region Public Methods

        /// <summary>
        /// Rebuilds the melding types that can be chosen and selects the type of the model (or the first allowed type).
        /// The model normally already has an allowed type: PrioIngreepModelSync takes care of that.
        /// </summary>
        public void RefreshAvailableTypes()
        {
            // the model may have dropped the melding bij storing (PrioIngreepModelSync): follow it
            if (PrioIngreepInUitMelding.MeldingBijstoring == null && MeldingBijstoring.Any()) MeldingBijstoring.Clear();

            var allowed = IsMeldingBijStoring
                ? new[] { PrioMeldingTypes.MeldingBijStoringType }
                : PrioMeldingTypes.GetAllowed(_ingreep.PrioIngreep, PrioIngreepInUitMelding, ControllerAccessProvider.Default.Controller.Data.CCOLVersie);
            MeldingenTypes = new ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper>(
                allowed.Select(x => new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(x)));
            Type = MeldingenTypes.FirstOrDefault(x => x.Value == PrioIngreepInUitMelding.Type) ?? MeldingenTypes.FirstOrDefault();
        }

        #endregion // Public Methods

        #region Private Methods

        /// <summary>
        /// Side effects of choosing a melding type: store it in the model, RIS defaults, the name of the melding,
        /// notify the rest of the application (dummy KAR detectors etc.) and switch the sub view-model.
        /// </summary>
        private void OnMeldingTypeChanged()
        {
            var oldType = PrioIngreepInUitMelding.Type;
            var raise = oldType != Type.Value;
            if (raise) PrioTrace.Write($"{_ingreep.FaseCyclus}{_ingreep.Naam}: {InUit} type {PrioIngreepInUitMelding.Type} -> {Type.Value} (UI)");
            PrioIngreepInUitMelding.Type = Type.Value;
            if (raise) OnPropertyChanged(nameof(Type), broadcast: true);

            // a type without opvang storing loses its melding bij storing (only on a real change: older files keep
            // theirs until then; the generator ignores it)
            if (raise && OpvangStoring && !CanHaveOpvangStoring) OpvangStoring = false;

            // at most one RIS melding per side: the other meldingen offer RIS again, or no longer
            if (raise && (oldType == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde ||
                          Type.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
            {
                RefreshSiblingTypes();
            }

            if (Type.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                RISViewModel.RisEta ??= _ingreep.RijTijdGehinderd;
                if (PrioIngreepInUitMelding is { RisRole: 0, RisSubrole: 0 })
                {
                    _ingreep.SetRisRoles(this);
                }
            }

            OnPropertyChanged("");

            var msg = new PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage(this);
            WeakReferenceMessengerEx.Default.Send(msg);
            if (msg.FaseCyclus == null) return;
            Naam = msg.FaseCyclus
                   + msg.Ingreep
                   + DefaultsProvider.Default.GetMeldingShortcode(PrioIngreepInUitMelding)
                   + (InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "in" : "uit");
            WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(msg.FaseCyclus, PrioIngreepInUitMelding));
            SetActualViewModel();
        }

        /// <summary>
        /// Refreshes the melding types of the other meldingen in the same list (eg. RIS available or not).
        /// </summary>
        private void RefreshSiblingTypes()
        {
            if (_parent is not PrioIngreepMeldingenListViewModel { Meldingen: not null } list) return;
            foreach (var melding in list.Meldingen.Where(x => x != this))
            {
                melding.RefreshAvailableTypes();
            }
        }

        private void SetActualViewModel()
        {
            if (Type == null) return;
            switch (Type.Value)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang:
                    ActualViewModel = ViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde:
                    TLCGenModelManager.Default.UpdateControllerAlerts();
                    ActualViewModel = RISViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton:
                    ActualViewModel = FietsPrioriteitViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton:
                    ActualViewModel = PelotonViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Opticom:
                    ActualViewModel = OpticomViewModel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ActualViewModel.OnPropertyChanged("");
        }

        #endregion // Private Methods

        #region Constructor

        public PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMeldingModel oVIngreepMassaDetectieMelding, object parent, PrioIngreepViewModel ingreep)
        {
            // The model and the sub view-models must exist before a type is selected: OnMeldingTypeChanged uses them
            _ingreep = ingreep;
            _parent = parent;
            PrioIngreepInUitMelding = oVIngreepMassaDetectieMelding;

            ViewModel = new PrioIngreepRegularMeldingViewModel(this);
            RISViewModel = new PrioIngreepRISMeldingViewModel(this);
            PelotonViewModel = new PrioIngreepPelotonMeldingViewModel(this);
            FietsPrioriteitViewModel = new PrioIngreepFietsPrioriteitMeldingViewModel(this);
            OpticomViewModel = new PrioIngreepOpticomMeldingViewModel(this);

            if (PrioIngreepInUitMelding.MeldingBijstoring != null)
            {
                MeldingBijstoring.Add(new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring, this, ingreep));
            }

            RefreshAvailableTypes();
            SetActualViewModel();
        }

        #endregion //Constructor
    }
}
