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

        public PrioIngreepInUitMeldingModel PrioIngreepInUitMelding { get; }

        public ObservableCollection<string> Detectoren { get; }

        public ObservableCollection<string> VecomDetectoren { get; }

        public ObservableCollection<string> SelectieveDetectoren { get; }

        public ObservableCollection<string> AvailableInputs
        {
            get
            {
                switch (Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                        return Detectoren;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return SelectieveDetectoren;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                        return VecomDetectoren;
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
                MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(msg.FaseCyclus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            }
        }

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
                }
                else
                {
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

        public PrioIngreepInUitMeldingViewModel MeldingBijstoring
        {
            get => OpvangStoring ? _meldingBijstoring ?? (_meldingBijstoring = new PrioIngreepInUitMeldingViewModel(PrioIngreepInUitMelding.MeldingBijstoring)) : null;
        }

        #endregion //Properties

        #region TLCGen events

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new PrioIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd1 = PrioIngreepInUitMelding != null ? RelatedInput1 : "";
            var sd2 = PrioIngreepInUitMelding != null ? RelatedInput2 : "";
            
            Detectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren))
            {
                Detectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren)
            {
                Detectoren.Add(d.Naam);
            }

            VecomDetectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.VecomDetector)))
            {
                VecomDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.VecomDetector))
            {
                VecomDetectoren.Add(d.Naam);
            }

            if (PrioIngreepInUitMelding != null && Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector)
            {
                if (Detectoren.Contains(sd1))
                {
                    RelatedInput1 = sd1;
                }
                if (Detectoren.Contains(sd2))
                {
                    RelatedInput2 = sd2;
                }
            }

            if (PrioIngreepInUitMelding != null && Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector)
            {
                if (VecomDetectoren.Contains(sd1))
                {
                    RelatedInput1 = sd1;
                }
                if (VecomDetectoren.Contains(sd2))
                {
                    RelatedInput2 = sd2;
                }
            }
        }

        private void OnSelectieveDetectorenChanged(SelectieveDetectorenChangedMessage obj)
        {
            var sd = "";
            if (PrioIngreepInUitMelding != null && Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)
            {
                sd = RelatedInput1;
            }

            SelectieveDetectoren.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                SelectieveDetectoren.Add(seld.Naam);
            }

            if (PrioIngreepInUitMelding != null && Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector &&
                SelectieveDetectoren.Contains(sd))
            {
                RelatedInput1 = sd;
            }
        }

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

            Detectoren = new ObservableCollection<string>();
            VecomDetectoren = new ObservableCollection<string>();
            SelectieveDetectoren = new ObservableCollection<string>();

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<SelectieveDetectorenChangedMessage>(this, OnSelectieveDetectorenChanged);
            OnDetectorenChanged(null);
            OnSelectieveDetectorenChanged(null);
        }

        #endregion //Constructor
    }
}
