using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepInUitMeldingVoorwaardeViewModel : ViewModelBase, IViewModelWithItem
    {
        private RelayCommand _removeVoorwaardeCommand;
        
        public OVIngreepInUitMeldingVoorwaardeModel OVIngreepMassaDetectieMeldingVoorwaarde { get; }

        public ObservableCollection<string> Detectoren { get; }
        public ObservableCollection<string> SelectieveDetectoren { get; }
        public ObservableCollection<string> WisselDetectoren { get; }
        public ObservableCollection<string> WisselIngangen { get; }

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

        public bool HasInput => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding;

        public bool CanHave2Inputs => Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.KARMelding &&
            Type != OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector;

        public bool TweedeInput
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.TweedeInput;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.TweedeInput = value;
                RaisePropertyChanged();
            }
        }

        public bool OpvangStoring
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.OpvangStoring;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.OpvangStoring = value;
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputOmgangMetStoringTypeEnum OpvangStoring2Detectors
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.OpvangStoring2Detectors;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.OpvangStoring2Detectors = value;
                RaisePropertyChanged();
            }
        }

        public string RelatedInput1
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput1;
            set
            {
                if(value != null)
                {
                    OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput1 = value;
                }
                RaisePropertyChanged();
            }
        }

        public string RelatedInput2
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput2;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput2 = value;
                }
                RaisePropertyChanged();
            }
        }


        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput1Type
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput1Type;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput1Type = value;
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitMeldingVoorwaardeInputTypeEnum RelatedInput2Type
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput2Type;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.RelatedInput2Type = value;
                RaisePropertyChanged();
            }
        }

        public string Omschrijving
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.Omschrijving;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.Omschrijving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum Type
        {
            get => OVIngreepMassaDetectieMeldingVoorwaarde.Type;
            set
            {
                OVIngreepMassaDetectieMeldingVoorwaarde.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(AvailableInputs));
            }
        }
        
        public ICommand RemoveVoorwaardeCommand => _removeVoorwaardeCommand ?? (_removeVoorwaardeCommand = new RelayCommand(RemoveVoorwaardeCommand_Executed));

        private void RemoveVoorwaardeCommand_Executed(object prm)
        {
            MessengerInstance.Send(new OVIngreepMassaDetectieObjectActionMessage(this, false, true));
        }

        public object GetItem()
        {
            return OVIngreepMassaDetectieMeldingVoorwaarde;
        }

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
                foreach(var d in fc.Detectoren)
                {
                    Detectoren.Add(d.Naam);
                }
            }

            WisselDetectoren.Clear();
            foreach(var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselDetector)))
            {
                WisselDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselDetector))
            {
                WisselDetectoren.Add(d.Naam);
            }

            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.Detector)
            {
                if(Detectoren.Contains(sd1))
                {
                    RelatedInput1 = sd1;
                }
                if (Detectoren.Contains(sd2))
                {
                    RelatedInput2 = sd2;
                }
            }
            //else if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.WisselDetector)
            //{
            //    if (WisselDetectoren.Contains(sd1))
            //    {
            //        RelatedInput1 = sd1;
            //    }
            //    if (WisselDetectoren.Contains(sd2))
            //    {
            //        RelatedInput2 = sd2;
            //    }
            //}
        }

        private void OnSelectieveDetectorenChanged(SelectieveDetectorenChangedMessage obj)
        {
            var sd = "";
            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector)
            {
                sd = RelatedInput1;
            }

            SelectieveDetectoren.Clear();
            foreach(var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                SelectieveDetectoren.Add(seld.Naam);
            }

            if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.SelectieveDetector &&
                SelectieveDetectoren.Contains(sd))
            {
                RelatedInput1 = sd;
            }
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            var sd = "";
            //if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.WisselIngang)
            //{
            //    sd = RelatedInput1;
            //}

            WisselIngangen.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            {
                WisselIngangen.Add(seld.Naam);
            }

            //if (Type == OVIngreepMassaDetectieMeldingVoorwaardeTypeEnum.WisselIngang &&
            //    SelectieveDetectoren.Contains(sd))
            //{
            //    RelatedInput1 = sd;
            //}
        }

        public OVIngreepInUitMeldingVoorwaardeViewModel(OVIngreepInUitMeldingVoorwaardeModel ovIngreepMassaDetectieMeldingVoorwaarde)
        {
            OVIngreepMassaDetectieMeldingVoorwaarde = ovIngreepMassaDetectieMeldingVoorwaarde;
            Detectoren = new ObservableCollection<string>();
            WisselIngangen = new ObservableCollection<string>();
            WisselDetectoren = new ObservableCollection<string>();
            SelectieveDetectoren = new ObservableCollection<string>();
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            MessengerInstance.Register<SelectieveDetectorenChangedMessage>(this, OnSelectieveDetectorenChanged);
            OnDetectorenChanged(null);
            OnIngangenChanged(null);
            OnSelectieveDetectorenChanged(null);
        }
    }
}
