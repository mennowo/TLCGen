using System;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PrioIngreepInUitMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private PrioIngreepInUitMeldingViewModel _meldingBijstoring;

        #endregion // Fields

        #region Properties



        public string Naam
        {
            get => string.IsNullOrWhiteSpace(PrioIngreepInUitMelding.Naam) ? "geen_naam" : PrioIngreepInUitMelding.Naam;
            set
            {
                PrioIngreepInUitMelding.Naam = value;
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingModel PrioIngreepInUitMelding { get; }

        public ObservableCollection<string> AvailableInputs
        {
            get
            {
                switch (Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                        return ControllerAccessProvider.Default.AllDetectorStrings;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return ControllerAccessProvider.Default.AllSelectiveDetectorStrings;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                        return ControllerAccessProvider.Default.AllVecomDetectorStrings;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return null;
            }
        }

        public PrioIngreepInUitMeldingTypeEnum InUit => PrioIngreepInUitMelding.InUit;

        public bool HasInput1 => Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;

        public bool DisabledIfDSI => Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;

        public bool HasInput1Type => Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding && 
            Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public bool HasInput2 => CanHaveInput2 && TweedeInput;
        
        public bool CanHaveInput2 => Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
            Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public PrioIngreepInUitMeldingVoorwaardeTypeEnum Type
        {
            get => PrioIngreepInUitMelding.Type;
            set
            {
                PrioIngreepInUitMelding.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged("");

                var msg = new PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
                MessengerInstance.Send(msg);
                if (msg.FaseCyclus == null) return;
                MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(msg.FaseCyclus, PrioIngreepInUitMelding));
                RaisePropertyChanged(nameof(HasDet));
                RaisePropertyChanged(nameof(HasInpSD));
                RaisePropertyChanged(nameof(HasSD));
                RaisePropertyChanged(nameof(HasKAR));
            }
        }

        public bool HasKAR => Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;
        public bool HasSD => Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;
        public bool HasInpSD => Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector;
        public bool HasDet => Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;

        public bool TweedeInput
        {
            get => PrioIngreepInUitMelding.TweedeInput;
            set
            {
                PrioIngreepInUitMelding.TweedeInput = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasInput2));
            }
        }

        public string RelatedInput1
        {
            get => PrioIngreepInUitMelding.RelatedInput1;
            set
            {
                if(value != null)
                {
                    PrioIngreepInUitMelding.RelatedInput1 = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type
        {
            get => PrioIngreepInUitMelding.RelatedInput1Type;
            set
            {
                PrioIngreepInUitMelding.RelatedInput1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string RelatedInput2
        {
            get => PrioIngreepInUitMelding.RelatedInput2;
            set
            {
                if (value != null)
                {
                    PrioIngreepInUitMelding.RelatedInput2 = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public PrioIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type
        {
            get => PrioIngreepInUitMelding.RelatedInput2Type;
            set
            {
                PrioIngreepInUitMelding.RelatedInput2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool KijkNaarWisselStand
        {
            get => PrioIngreepInUitMelding.KijkNaarWisselStand;
            set
            {
                PrioIngreepInUitMelding.KijkNaarWisselStand = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool OpvangStoring
        {
            get => PrioIngreepInUitMelding.OpvangStoring;
            set
            {
                PrioIngreepInUitMelding.OpvangStoring = value;
                if (value)
                {
                    _meldingBijstoring = null;
                    PrioIngreepInUitMelding.MeldingBijstoring = new PrioIngreepInUitMeldingModel()
                    {
                        InUit = this.InUit
                    };
                    MeldingBijstoring.Add(new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring));
                }
                else
                {
                    MeldingBijstoring.Clear();
                    PrioIngreepInUitMelding.MeldingBijstoring = null;
                    _meldingBijstoring = null;
                }
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(MeldingBijstoring));
            }
        }

        public bool AlleenIndienGeenInmelding
        {
            get => PrioIngreepInUitMelding.AlleenIndienGeenInmelding;
            set
            {
                PrioIngreepInUitMelding.AlleenIndienGeenInmelding = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AlleenIndienRood
        {
            get => PrioIngreepInUitMelding.AlleenIndienRood;
            set
            {
                PrioIngreepInUitMelding.AlleenIndienRood = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AntiJutterTijdToepassen
        {
            get => PrioIngreepInUitMelding.AntiJutterTijdToepassen;
            set
            {
                PrioIngreepInUitMelding.AntiJutterTijdToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AntiJutterTijd
        {
            get => PrioIngreepInUitMelding.AntiJutterTijd;
            set
            {
                PrioIngreepInUitMelding.AntiJutterTijd= value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public ObservableCollection<PrioIngreepInUitMeldingViewModel> MeldingBijstoring { get; } = new ObservableCollection<PrioIngreepInUitMeldingViewModel>();

        #endregion //Properties

        #region TLCGen events

        #endregion // TLCGen events

        #region IViewModelWithItem

        public object GetItem()
        {
            return PrioIngreepInUitMelding;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMeldingModel oVIngreepMassaDetectieMelding)
        {
            PrioIngreepInUitMelding = oVIngreepMassaDetectieMelding;

            if (PrioIngreepInUitMelding.MeldingBijstoring != null)
            {
                MeldingBijstoring.Add(new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring));
            }
        }

        #endregion //Constructor
    }
}
