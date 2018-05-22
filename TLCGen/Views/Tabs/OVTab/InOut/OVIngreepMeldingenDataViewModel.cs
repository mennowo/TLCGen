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
    public class OVIngreepMassaDetectieDataViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addInmeldingCommand;
        private RelayCommand _removeInmeldingCommand;
        private RelayCommand _addUitmeldingCommand;
        private RelayCommand _removeUitmeldingCommand;
        //private RelayCommand _addVoorwaardeCommand;
        private object _selectedObject;
        private OVIngreepInUitMeldingViewModel _selectedInmelding;
        private OVIngreepInUitMeldingViewModel _selectedUitmelding;

        #endregion // Fields

        #region Properties

        public OVIngreepMeldingenDataModel OVIngreepMassaDetectieData { get; }

        public ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel> Inmeldingen { get; }
        public ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel> Uitmeldingen { get; }

        public ObservableCollection<string> WisselDetectoren { get; }
        public ObservableCollection<string> WisselInputs { get; }

        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                RaisePropertyChanged();
                //RaisePropertyChanged(nameof(SelectedObjectDescription));
            }
        }

        public OVIngreepInUitMeldingViewModel SelectedInmelding
        {
            get => _selectedInmelding;
            set
            {
                _selectedInmelding = value;
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitMeldingViewModel SelectedUitmelding
        {
            get => _selectedUitmelding;
            set
            {
                _selectedUitmelding = value;
                RaisePropertyChanged();
            }
        }

        public bool Wissel1
        {
            get => OVIngreepMassaDetectieData.Wissel1;
            set
            {
                OVIngreepMassaDetectieData.Wissel1 = value;
                RaisePropertyChanged();
            }
        }

        public OVIngreepInUitDataWisselType Wissel1Type
        {
            get => OVIngreepMassaDetectieData.Wissel1Type;
            set
            {
                OVIngreepMassaDetectieData.Wissel1Type = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsWissel1Ingang));
                RaisePropertyChanged(nameof(IsWissel1Detector));
            }
        }

        public bool IsWissel1Ingang => Wissel1Type == OVIngreepInUitDataWisselType.Ingang;
        public bool IsWissel2Ingang => Wissel2Type == OVIngreepInUitDataWisselType.Ingang;
        public bool IsWissel1Detector => Wissel1Type == OVIngreepInUitDataWisselType.Detector;
        public bool IsWissel2Detector => Wissel2Type == OVIngreepInUitDataWisselType.Detector;

        public OVIngreepInUitDataWisselType Wissel2Type
        {
            get => OVIngreepMassaDetectieData.Wissel2Type;
            set
            {
                OVIngreepMassaDetectieData.Wissel2Type = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsWissel2Ingang));
                RaisePropertyChanged(nameof(IsWissel2Detector));
            }
        }

        public bool Wissel1Voorwaarde
        {
            get => OVIngreepMassaDetectieData.Wissel1;
            set
            {
                OVIngreepMassaDetectieData.Wissel1 = value;
                RaisePropertyChanged();
            }
        }

        public string Wissel1Input
        {
            get => OVIngreepMassaDetectieData.Wissel1Input;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieData.Wissel1Input = value;
                }
                RaisePropertyChanged();
            }
        }

        public string Wissel1Detector
        {
            get => OVIngreepMassaDetectieData.Wissel1Detector;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieData.Wissel1Detector = value;
                }
                RaisePropertyChanged();
            }
        }

        public bool Wissel2
        {
            get => OVIngreepMassaDetectieData.Wissel2;
            set
            {
                OVIngreepMassaDetectieData.Wissel2 = value;
                RaisePropertyChanged();
            }
        }

        public bool Wissel2Voorwaarde
        {
            get => OVIngreepMassaDetectieData.Wissel2;
            set
            {
                OVIngreepMassaDetectieData.Wissel2 = value;
                RaisePropertyChanged();
            }
        }

        public string Wissel2Input
        {
            get => OVIngreepMassaDetectieData.Wissel2Input;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieData.Wissel2Input = value;
                }
                RaisePropertyChanged();
            }
        }

        public string Wissel2Detector
        {
            get => OVIngreepMassaDetectieData.Wissel2Detector;
            set
            {
                if (value != null)
                {
                    OVIngreepMassaDetectieData.Wissel2Detector = value;
                }
                RaisePropertyChanged();
            }
        }

        //public string SelectedObjectDescription
        //{
        //    get
        //    {
        //        switch (SelectedObject)
        //        {
        //            case OVIngreepInUitMeldingViewModel melding:
        //                return "Melding data";
        //            case OVIngreepInUitMeldingVoorwaardeViewModel voorwaarde:
        //                return "Voorwaarde data";
        //        }
        //        return "Geen selectie";
        //    }
        //}

        #endregion // Properties

        #region Commands

        public ICommand AddInmeldingCommand => _addInmeldingCommand ?? (_addInmeldingCommand = new RelayCommand(AddInmeldingCommand_Executed));
        public ICommand RemoveInmeldingCommand => _removeInmeldingCommand ?? (_removeInmeldingCommand = new RelayCommand(RemoveInmeldingCommand_Executed));
        public ICommand AddUitmeldingCommand => _addUitmeldingCommand ?? (_addUitmeldingCommand = new RelayCommand(AddUitmeldingCommand_Executed));
        public ICommand RemoveUitmeldingCommand => _removeUitmeldingCommand ?? (_removeUitmeldingCommand = new RelayCommand(RemoveUitmeldingCommand_Executed));
        //public ICommand AddVoorwaardeCommand => _addVoorwaardeCommand ?? (_addVoorwaardeCommand = new RelayCommand(AddVoorwaardeCommand_Executed));

        #endregion // Commands

        #region Command functionality

        private void AddInmeldingCommand_Executed(object prm)
        {
            Inmeldingen.Add(new OVIngreepInUitMeldingViewModel(new OVIngreepInUitMeldingModel
            {
                InUit = OVIngreepInUitMeldingType.Inmelding
            }));
        }

        private void AddUitmeldingCommand_Executed(object prm)
        {
            Uitmeldingen.Add(new OVIngreepInUitMeldingViewModel(new OVIngreepInUitMeldingModel
            {
                InUit = OVIngreepInUitMeldingType.Uitmelding
            }));
        }



        private void RemoveInmeldingCommand_Executed(object prm)
        {
            if(SelectedInmelding != null)
            {
                Inmeldingen.Remove(SelectedInmelding);
                SelectedInmelding = null;
            }
        }

        private void RemoveUitmeldingCommand_Executed(object prm)
        {
            if (SelectedUitmelding != null)
            {
                Uitmeldingen.Remove(SelectedUitmelding);
                SelectedUitmelding = null;
            }
        }

        //private void AddVoorwaardeCommand_Executed(object obj)
        //{
        //    SelectedMelding.Voorwaarden.Add(new OVIngreepInUitMeldingVoorwaardeViewModel(new OVIngreepInUitMeldingVoorwaardeModel()));
        //}

        private bool AddVoorwaardeCommand_CanExecute(object prm)
        {
            return SelectedObject != null && SelectedObject is OVIngreepInUitMeldingModel;
        }

        #endregion // Command functionality

        // private void OnObjectAction(OVIngreepMassaDetectieObjectActionMessage msg)
        // {
        //     RaisePropertyChanged<object>(broadcast: true);
        //     switch (msg.Object)
        //     {
        //         case OVIngreepInUitMeldingViewModel melding:
        //             if (msg.Add)
        //             {
        //                 melding.Voorwaarden.Add(new OVIngreepInUitMeldingVoorwaardeViewModel(new OVIngreepInUitMeldingVoorwaardeModel()));
        //             }
        //             if (msg.Remove)
        //             {
        //                 Inmeldingen.Remove(melding);
        //                 if (SelectedObject == melding) SelectedObject = null;
        //             }
        //             break;
        //         case OVIngreepInUitMeldingVoorwaardeViewModel voorwaarde:
        //             if (msg.Remove)
        //             {
        //                 foreach (var m in Inmeldingen)
        //                 {
        //                     if (m.Voorwaarden.Contains(voorwaarde))
        //                     {
        //                         m.Voorwaarden.Remove(voorwaarde);
        //                         if (SelectedObject == voorwaarde) SelectedObject = null;
        //                         return;
        //                     }
        //                 }
        //             }
        //             break;
        //     }
        // }


        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd1 = "";
            var sd2 = "";
            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselType.Detector)
            {
                sd1 = Wissel1Detector;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselType.Detector)
            {
                sd2 = Wissel2Detector;
            }
            

            WisselDetectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselDetector)))
            {
                WisselDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselDetector))
            {
                WisselDetectoren.Add(d.Naam);
            }

            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselType.Detector && WisselDetectoren.Contains(sd1))
            {
                Wissel1Detector = sd1;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselType.Detector && WisselDetectoren.Contains(sd2))
            {
                Wissel2Detector = sd2;
            }
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            var sd1 = "";
            var sd2 = "";
            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselType.Ingang)
            {
                sd1 = Wissel1Input;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselType.Ingang)
            {
                sd2 = Wissel2Input;
            }

            WisselInputs.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            {
                WisselInputs.Add(seld.Naam);
            }

            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselType.Ingang && WisselInputs.Contains(sd1))
            {
                Wissel1Input = sd1;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselType.Ingang && WisselInputs.Contains(sd2))
            {
                Wissel2Input = sd2;
            }
        }

        #region Constructor

        public OVIngreepMassaDetectieDataViewModel(OVIngreepMeldingenDataModel ovIngreepMassaDetectieData)
        {
            OVIngreepMassaDetectieData = ovIngreepMassaDetectieData;
            Inmeldingen = new ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel>(ovIngreepMassaDetectieData.Inmeldingen);
            Uitmeldingen = new ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel>(ovIngreepMassaDetectieData.Uitmeldingen);
            WisselInputs = new ObservableCollection<string>();
            WisselDetectoren = new ObservableCollection<string>();
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            OnDetectorenChanged(null);
            OnIngangenChanged(null);
            // MessengerInstance.Register<OVIngreepMassaDetectieObjectActionMessage>(this, OnObjectAction);
        }

        #endregion // Constructor
    }
}
