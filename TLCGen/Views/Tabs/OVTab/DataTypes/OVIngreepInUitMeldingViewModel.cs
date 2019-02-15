using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepInUitMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private OVIngreepInUitMeldingViewModel _meldingBijstoring;

        #endregion // Fields

        #region Properties

        public OVIngreepInUitMeldingModel OVIngreepInUitMelding { get; }

        public ObservableCollection<string> Detectoren { get; }

        public ObservableCollection<string> VecomDetectoren { get; }

        public ObservableCollection<string> SelectieveDetectoren { get; }

        public ObservableCollection<string> AvailableInputs
        {
            get
            {
                switch (Type)
                {
                    case OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                        return Detectoren;
                    case OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return SelectieveDetectoren;
                    case OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                        break;
                    case OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                        return VecomDetectoren;
                }
                return null;
            }
        }

        public OVIngreepInUitMeldingTypeEnum InUit => OVIngreepInUitMelding.InUit;

        public bool HasInput1 => Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;

        public bool DisabledIfDSI => Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding;

        public bool HasInput1Type => Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding && 
            Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public bool HasInput2 => CanHaveInput2 && TweedeInput;
        
        public bool CanHaveInput2 => Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
            Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public OVIngreepInUitMeldingVoorwaardeTypeEnum Type
        {
            get => OVIngreepInUitMelding.Type;
            set
            {
                OVIngreepInUitMelding.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged("");

                var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
                MessengerInstance.Send(msg);
                if (msg.FaseCyclus == null) return;
                MessengerInstance.Send(new OVIngreepMeldingChangedMessage(msg.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            }
        }

        public bool TweedeInput
        {
            get => OVIngreepInUitMelding.TweedeInput;
            set
            {
                OVIngreepInUitMelding.TweedeInput = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasInput2));
            }
        }

        public string RelatedInput1
        {
            get => OVIngreepInUitMelding.RelatedInput1;
            set
            {
                if(value != null)
                {
                    OVIngreepInUitMelding.RelatedInput1 = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type
        {
            get => OVIngreepInUitMelding.RelatedInput1Type;
            set
            {
                OVIngreepInUitMelding.RelatedInput1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string RelatedInput2
        {
            get => OVIngreepInUitMelding.RelatedInput2;
            set
            {
                if (value != null)
                {
                    OVIngreepInUitMelding.RelatedInput2 = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type
        {
            get => OVIngreepInUitMelding.RelatedInput2Type;
            set
            {
                OVIngreepInUitMelding.RelatedInput2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool KijkNaarWisselStand
        {
            get => OVIngreepInUitMelding.KijkNaarWisselStand;
            set
            {
                OVIngreepInUitMelding.KijkNaarWisselStand = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool OpvangStoring
        {
            get => OVIngreepInUitMelding.OpvangStoring;
            set
            {
                OVIngreepInUitMelding.OpvangStoring = value;
                if (value)
                {
                    _meldingBijstoring = null;
                    OVIngreepInUitMelding.MeldingBijstoring = new OVIngreepInUitMeldingModel()
                    {
                        InUit = this.InUit
                    };
                }
                else
                {
                    OVIngreepInUitMelding.MeldingBijstoring = null;
                    _meldingBijstoring = null;
                }
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(MeldingBijstoring));
            }
        }

        public bool AlleenIndienGeenInmelding
        {
            get => OVIngreepInUitMelding.AlleenIndienGeenInmelding;
            set
            {
                OVIngreepInUitMelding.AlleenIndienGeenInmelding = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool AntiJutterTijdToepassen
        {
            get => OVIngreepInUitMelding.AntiJutterTijdToepassen;
            set
            {
                OVIngreepInUitMelding.AntiJutterTijdToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int AntiJutterTijd
        {
            get => OVIngreepInUitMelding.AntiJutterTijd;
            set
            {
                OVIngreepInUitMelding.AntiJutterTijd= value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepInUitMeldingViewModel MeldingBijstoring
        {
            get => OpvangStoring ? _meldingBijstoring ?? (_meldingBijstoring = new OVIngreepInUitMeldingViewModel(OVIngreepInUitMelding.MeldingBijstoring)) : null;
        }

        #endregion //Properties

        #region TLCGen events

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd1 = OVIngreepInUitMelding != null ? RelatedInput1 : "";
            var sd2 = OVIngreepInUitMelding != null ? RelatedInput2 : "";
            
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

            if (OVIngreepInUitMelding != null && Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector)
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

            if (OVIngreepInUitMelding != null && Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector)
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
            if (OVIngreepInUitMelding != null && Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)
            {
                sd = RelatedInput1;
            }

            SelectieveDetectoren.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                SelectieveDetectoren.Add(seld.Naam);
            }

            if (OVIngreepInUitMelding != null && Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector &&
                SelectieveDetectoren.Contains(sd))
            {
                RelatedInput1 = sd;
            }
        }

        #endregion // TLCGen events

        #region IViewModelWithItem

        public object GetItem()
        {
            return OVIngreepInUitMelding;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepInUitMeldingViewModel(OVIngreepInUitMeldingModel oVIngreepMassaDetectieMelding)
        {
            OVIngreepInUitMelding = oVIngreepMassaDetectieMelding;

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
