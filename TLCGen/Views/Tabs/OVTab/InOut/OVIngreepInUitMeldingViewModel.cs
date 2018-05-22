using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepInUitMeldingViewModel : ViewModelBase, IViewModelWithItem
    {
        //private RelayCommand _removeMeldingCommand;
        //private RelayCommand _addVoorwaardeCommand;

        public ObservableCollection<string> Detectoren { get; }
        public ObservableCollection<string> SelectieveDetectoren { get; }

        public ObservableCollection<string> AvailableInputs
        {
            get
            {
                switch (Type)
                {
                    case OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.Detector:
                        return Detectoren;
                    case OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector:
                        return SelectieveDetectoren;
                }
                return null;
            }
        }

        public OVIngreepInUitMeldingType InUit => OVIngreepInUitMelding.InUit;

        public bool HasInput1 => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding;

        public bool DisabledIfDSI => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding;

        public bool HasInput1Type => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding && 
            Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public bool HasInput2 => CanHaveInput2 && TweedeInput;

        public bool HasInput2Storing => HasInput2 && OpvangStoring;
        public bool CanHaveInput2 => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding &&
            Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public OVIngreepInUitMeldingModel OVIngreepInUitMelding { get; }

        //public string Omschrijving
        //{
        //    get => OVIngreepInUitMelding.Omschrijving;
        //    set
        //    {
        //        OVIngreepInUitMelding.Omschrijving = value;
        //        RaisePropertyChanged<object>(broadcast: true);
        //    }
        //}

        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type
        {
            get => OVIngreepInUitMelding.Type;
            set
            {
                OVIngreepInUitMelding.Type = value;
                RaisePropertyChanged("");
            }
        }

        public bool TweedeInput
        {
            get => OVIngreepInUitMelding.TweedeInput;
            set
            {
                OVIngreepInUitMelding.TweedeInput = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasInput2));
                RaisePropertyChanged(nameof(HasInput2Storing));
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
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type
        {
            get => OVIngreepInUitMelding.RelatedInput1Type;
            set
            {
                OVIngreepInUitMelding.RelatedInput1Type = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type
        {
            get => OVIngreepInUitMelding.RelatedInput2Type;
            set
            {
                OVIngreepInUitMelding.RelatedInput2Type = value;
                RaisePropertyChanged();
            }
        }


        //public bool AlleenIndienGeenInmelding
        //{
        //    get => OVIngreepInUitMelding.AlleenIndienGeenInmelding;
        //    set
        //    {
        //        OVIngreepInUitMelding.AlleenIndienGeenInmelding = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //public bool AntiJutterTijdToepassen
        //{
        //    get => OVIngreepInUitMelding.AntiJutterTijdToepassen;
        //    set
        //    {
        //        OVIngreepInUitMelding.AntiJutterTijdToepassen = value;
        //        RaisePropertyChanged();
        //    }
        //}

        //public int AntiJutterTijd
        //{
        //    get => OVIngreepInUitMelding.AntiJutterTijd;
        //    set
        //    {
        //        OVIngreepInUitMelding.AntiJutterTijd = value;
        //        RaisePropertyChanged();
        //    }
        //}

        public bool KijkNaarWisselStand
        {
            get => OVIngreepInUitMelding.KijkNaarWisselStand;
            set
            {
                OVIngreepInUitMelding.KijkNaarWisselStand = value;
                RaisePropertyChanged();
            }
        }

        public bool OpvangStoring
        {
            get => OVIngreepInUitMelding.OpvangStoring;
            set
            {
                OVIngreepInUitMelding.OpvangStoring = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasInput2Storing));
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum OpvangStoring2Detectors
        {
            get => OVIngreepInUitMelding.OpvangStoring2Detectors;
            set
            {
                OVIngreepInUitMelding.OpvangStoring2Detectors = value;
                RaisePropertyChanged();
            }
        }

        //public OVIngreepMassaDetectieMeldingType Type
        //{
        //    get => OVIngreepInUitMelding.Type;
        //    set
        //    {
        //        OVIngreepInUitMelding.Type = value;
        //        RaisePropertyChanged<object>(broadcast: true);
        //    }
        //}

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

        //public ObservableCollectionAroundList<OVIngreepInUitMeldingVoorwaardeViewModel, OVIngreepInUitMeldingVoorwaardeModel> Voorwaarden { get; }

        //public ICommand RemoveMeldingCommand => _removeMeldingCommand ?? (_removeMeldingCommand = new RelayCommand(RemoveMeldingCommand_Executed));
        //public ICommand AddVoorwaardeCommand => _addVoorwaardeCommand ?? (_addVoorwaardeCommand = new RelayCommand(AddVoorwaardeCommand_Executed));
        //
        //private void RemoveMeldingCommand_Executed(object prm)
        //{
        //    MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, false, true));
        //}
        //
        //private void AddVoorwaardeCommand_Executed(object prm)
        //{
        //    MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, true, false));
        //}


        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd1 = "";
            var sd2 = "";
            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.Detector)
            {
                sd1 = RelatedInput1;
                sd2 = RelatedInput2;
            }

            var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.Where(x => x.Naam == msg.FaseCyclus).FirstOrDefault();
            if (fc != null)
            {
                Detectoren.Clear();
                foreach (var d in fc.Detectoren)
                {
                    Detectoren.Add(d.Naam);
                }
            }
            
            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.Detector)
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
        }

        private void OnSelectieveDetectorenChanged(SelectieveDetectorenChangedMessage obj)
        {
            var sd = "";
            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector)
            {
                sd = RelatedInput1;
            }

            SelectieveDetectoren.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                SelectieveDetectoren.Add(seld.Naam);
            }

            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector &&
                SelectieveDetectoren.Contains(sd))
            {
                RelatedInput1 = sd;
            }
        }

        public object GetItem()
        {
            return OVIngreepInUitMelding;
        }

        public OVIngreepInUitMeldingViewModel(OVIngreepInUitMeldingModel oVIngreepMassaDetectieMelding)
        {
            OVIngreepInUitMelding = oVIngreepMassaDetectieMelding;

            Detectoren = new ObservableCollection<string>();
            SelectieveDetectoren = new ObservableCollection<string>();

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<SelectieveDetectorenChangedMessage>(this, OnSelectieveDetectorenChanged);
            OnDetectorenChanged(null);
            OnSelectieveDetectorenChanged(null);
            //Voorwaarden = new ObservableCollectionAroundList<OVIngreepInUitMeldingVoorwaardeViewModel, OVIngreepInUitMeldingVoorwaardeModel>(oVIngreepMassaDetectieMelding.Voorwaarden);
        }
    }
}
