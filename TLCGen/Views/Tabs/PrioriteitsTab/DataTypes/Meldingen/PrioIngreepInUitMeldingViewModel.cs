﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;
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

        public ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper> MeldingenTypes { get; } = new ObservableCollection<PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper>();

        public PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper Type
        {
            get => _prioIngreepInUitMeldingType;
            set
            {
                _prioIngreepInUitMeldingType = value;
                
                if (value != null && PrioIngreepInUitMelding != null)
                {
                    var raise = PrioIngreepInUitMelding.Type != value.Value;
                    PrioIngreepInUitMelding.Type = value.Value;
                    if (raise) OnPropertyChanged(broadcast: true);
                }

                if (Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
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
        }
        
        public bool HasKAR => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;
        public bool HasSD => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;
        public bool HasInpSD => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector;
        public bool HasDet => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;
        public bool HasRis => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde;
        public bool HasInp => Type?.Value == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang;

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
                        InUit = this.InUit
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

        public void RefreshAvailableTypes()
        {
            var sel = Type;
            MeldingenTypes.Clear();
            switch (_ingreep.Type)
            {
                case PrioIngreepVoertuigTypeEnum.Tram:
                case PrioIngreepVoertuigTypeEnum.Bus:
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector));
                    if (ControllerAccessProvider.Default.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                    }
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang));
                    break;
                case PrioIngreepVoertuigTypeEnum.Fiets:
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton));
                    if (ControllerAccessProvider.Default.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                    }
                    break;
                case PrioIngreepVoertuigTypeEnum.Vrachtwagen:
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
                    if (ControllerAccessProvider.Default.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                    }
                    break;
                case PrioIngreepVoertuigTypeEnum.Auto:
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton));
                    if (ControllerAccessProvider.Default.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                    }
                    break;
                case PrioIngreepVoertuigTypeEnum.NG:
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton));
                    MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton));
                    if (ControllerAccessProvider.Default.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                    {
                        MeldingenTypes.Add(new PrioIngreepInUitMeldingVoorwaardeTypeEnumWrapper(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Type = MeldingenTypes.FirstOrDefault(x => x.Value == sel?.Value);
            if (Type == null) Type = MeldingenTypes.FirstOrDefault();
        }

        #endregion // Public Methods

        #region Private Methods

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
                //case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VrachtRIS:
                //case PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsRISPeloton:
                //case PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoRISPeloton:
                    TLCGenModelManager.Default.UpdateControllerAlerts();
                    ActualViewModel = RISViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton:
                    ActualViewModel = FietsPrioriteitViewModel;
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.AutoMassaPeloton:
                    ActualViewModel = PelotonViewModel;
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
            _ingreep = ingreep;
            RefreshAvailableTypes();
            _parent = parent;

            PrioIngreepInUitMelding = oVIngreepMassaDetectieMelding;
            
            ViewModel = new PrioIngreepRegularMeldingViewModel(this);
            RISViewModel = new PrioIngreepRISMeldingViewModel(this);
            PelotonViewModel = new PrioIngreepPelotonMeldingViewModel(this);
            FietsPrioriteitViewModel = new PrioIngreepFietsPrioriteitMeldingViewModel(this);

            if (PrioIngreepInUitMelding.MeldingBijstoring != null)
            {
                MeldingBijstoring.Add(new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring, this, ingreep));
            }

            Type = MeldingenTypes.FirstOrDefault(x => x.Value == PrioIngreepInUitMelding.Type);
            if (Type == null)
            {
                Type = MeldingenTypes.FirstOrDefault();
            }

            SetActualViewModel();
        }


        #endregion //Constructor
    }
}
