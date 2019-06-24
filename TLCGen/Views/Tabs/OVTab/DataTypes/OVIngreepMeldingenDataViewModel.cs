using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVIngreepInUitMeldingenDataViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addInmeldingCommand;
        private RelayCommand _removeInmeldingCommand;
        private RelayCommand _addUitmeldingCommand;
        private RelayCommand _removeUitmeldingCommand;
        private OVIngreepInUitMeldingViewModel _selectedInmelding;
        private OVIngreepInUitMeldingViewModel _selectedUitmelding;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public OVIngreepMeldingenDataModel OVIngreepMeldingenData { get; }

        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel> Inmeldingen { get; }
        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepInUitMeldingViewModel, OVIngreepInUitMeldingModel> Uitmeldingen { get; }

        [Browsable(false)]
        public ObservableCollection<string> WisselDetectoren { get; }
        [Browsable(false)]
        public ObservableCollection<string> WisselInputs { get; }

        [Browsable(false)]
        public OVIngreepInUitMeldingViewModel SelectedInmelding
        {
            get => _selectedInmelding;
            set
            {
                _selectedInmelding = value;
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public OVIngreepInUitMeldingViewModel SelectedUitmelding
        {
            get => _selectedUitmelding;
            set
            {
                _selectedUitmelding = value;
                RaisePropertyChanged();
            }
        }

        [Category("Anti jutter uitmelding")]
        [Description("Toepassen")]
        public bool AntiJutterVoorAlleUitmeldingen
        {
            get => OVIngreepMeldingenData.AntiJutterVoorAlleUitmeldingen;
            set
            {
                OVIngreepMeldingenData.AntiJutterVoorAlleUitmeldingen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Anti jutter tijd")]
        public int AntiJutterTijdVoorAlleUitmeldingen
        {
            get => OVIngreepMeldingenData.AntiJutterTijdVoorAlleUitmeldingen;
            set
            {
                OVIngreepMeldingenData.AntiJutterTijdVoorAlleUitmeldingen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Browsable(false)]
        public bool IsWissel1Ingang => Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang;
        [Browsable(false)]
        public bool IsWissel2Ingang => Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang;
        [Browsable(false)]
        public bool IsWissel1Detector => Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector;
        [Browsable(false)]
        public bool IsWissel2Detector => Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector;

        [Category("Wissels")]
        [Description("Wissel 1")]
        public bool Wissel1
        {
            get => OVIngreepMeldingenData.Wissel1;
            set
            {
                OVIngreepMeldingenData.Wissel1 = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel));
            }
        }

        [Description("Wissel 1 type")]
        public OVIngreepInUitDataWisselTypeEnum Wissel1Type
        {
            get => OVIngreepMeldingenData.Wissel1Type;
            set
            {
                OVIngreepMeldingenData.Wissel1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsWissel1Ingang));
                RaisePropertyChanged(nameof(IsWissel1Detector));
            }
        }

        [Description("Wissel 1 input")]
        [BrowsableCondition(nameof(IsWissel1Ingang))]
        public string Wissel1Input
        {
            get => OVIngreepMeldingenData.Wissel1Input;
            set
            {
                if (value != null)
                {
                    OVIngreepMeldingenData.Wissel1Input = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 1 voorwaarde")]
        [BrowsableCondition(nameof(IsWissel1Ingang))]
        public bool Wissel1Voorwaarde
        {
            get => OVIngreepMeldingenData.Wissel1InputVoorwaarde;
            set
            {
                OVIngreepMeldingenData.Wissel1InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 1 detector")]
        [BrowsableCondition(nameof(IsWissel1Detector))]
        public string Wissel1Detector
        {
            get => OVIngreepMeldingenData.Wissel1Detector;
            set
            {
                if (value != null)
                {
                    OVIngreepMeldingenData.Wissel1Detector = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2")]
        public bool Wissel2
        {
            get => OVIngreepMeldingenData.Wissel2;
            set
            {
                OVIngreepMeldingenData.Wissel2 = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel));
            }
        }

        [Description("Wissel 2 type")]
        public OVIngreepInUitDataWisselTypeEnum Wissel2Type
        {
            get => OVIngreepMeldingenData.Wissel2Type;
            set
            {
                OVIngreepMeldingenData.Wissel2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsWissel2Ingang));
                RaisePropertyChanged(nameof(IsWissel2Detector));
            }
        }

        [Description("Wissel 2 input")]
        [BrowsableCondition(nameof(IsWissel2Ingang))]
        public string Wissel2Input
        {
            get => OVIngreepMeldingenData.Wissel2Input;
            set
            {
                if (value != null)
                {
                    OVIngreepMeldingenData.Wissel2Input = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2 detector")]
        [BrowsableCondition(nameof(IsWissel2Detector))]
        public string Wissel2Detector
        {
            get => OVIngreepMeldingenData.Wissel2Detector;
            set
            {
                if (value != null)
                {
                    OVIngreepMeldingenData.Wissel2Detector = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Wissel 2 voorwaarde")]
        [BrowsableCondition(nameof(IsWissel2Detector))]
        public bool Wissel2Voorwaarde
        {
            get => OVIngreepMeldingenData.Wissel2InputVoorwaarde;
            set
            {
                OVIngreepMeldingenData.Wissel2InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasWissel => Wissel1 || Wissel2;

        #endregion // Properties

        #region Commands

        public ICommand AddInmeldingCommand => _addInmeldingCommand ?? (_addInmeldingCommand = new RelayCommand(AddInmeldingCommand_Executed));
        public ICommand RemoveInmeldingCommand => _removeInmeldingCommand ?? (_removeInmeldingCommand = new RelayCommand(RemoveInmeldingCommand_Executed, RemoveInmeldingCommand_CanExecute));
        public ICommand AddUitmeldingCommand => _addUitmeldingCommand ?? (_addUitmeldingCommand = new RelayCommand(AddUitmeldingCommand_Executed));
        public ICommand RemoveUitmeldingCommand => _removeUitmeldingCommand ?? (_removeUitmeldingCommand = new RelayCommand(RemoveUitmeldingCommand_Executed, RemoveUitmeldingCommand_CanExecute));
        //public ICommand AddVoorwaardeCommand => _addVoorwaardeCommand ?? (_addVoorwaardeCommand = new RelayCommand(AddVoorwaardeCommand_Executed));

        #endregion // Commands

        #region Command functionality

        private void AddInmeldingCommand_Executed(object prm)
        {
            Inmeldingen.Add(new OVIngreepInUitMeldingViewModel(new OVIngreepInUitMeldingModel
            {
                InUit = OVIngreepInUitMeldingTypeEnum.Inmelding,
                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
            }));

            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;
            MessengerInstance.Send(new OVIngreepMeldingChangedMessage(msg.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
        }

        private void AddUitmeldingCommand_Executed(object prm)
        {
            Uitmeldingen.Add(new OVIngreepInUitMeldingViewModel(new OVIngreepInUitMeldingModel
            {
                InUit = OVIngreepInUitMeldingTypeEnum.Uitmelding,
                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
            }));

            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;
            MessengerInstance.Send(new OVIngreepMeldingChangedMessage(msg.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
        }

        private void RemoveInmeldingCommand_Executed(object prm)
        {
            if(SelectedInmelding != null)
            {
                Inmeldingen.Remove(SelectedInmelding);
				var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
				MessengerInstance.Send(msg);
				if (msg.FaseCyclus == null) return;
				MessengerInstance.Send(new OVIngreepMeldingChangedMessage(msg.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
				SelectedInmelding = null;
            }
        }

        private bool RemoveInmeldingCommand_CanExecute(object prm)
        {
            return SelectedInmelding != null;
        }

        private void RemoveUitmeldingCommand_Executed(object prm)
        {
            if (SelectedUitmelding != null)
            {
                Uitmeldingen.Remove(SelectedUitmelding);
				var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
				MessengerInstance.Send(msg);
				if (msg.FaseCyclus == null) return;
				MessengerInstance.Send(new OVIngreepMeldingChangedMessage(msg.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
				SelectedUitmelding = null;
            }
        }

        private bool RemoveUitmeldingCommand_CanExecute(object prm)
        {
            return SelectedUitmelding != null;
        }

        #endregion // Command functionality

        #region TLCGen events

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var msg = new OVIngreepMassaDetectieObjectNeedsFaseCyclusMessage(this);
            MessengerInstance.Send(msg);
            if (msg.FaseCyclus == null) return;

            var sd1 = "";
            var sd2 = "";
            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector)
            {
                sd1 = Wissel1Detector;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector)
            {
                sd2 = Wissel2Detector;
            }
            

            WisselDetectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselStandDetector)))
            {
                WisselDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselStandDetector))
            {
                WisselDetectoren.Add(d.Naam);
            }

            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd1))
            {
                Wissel1Detector = sd1;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd2))
            {
                Wissel2Detector = sd2;
            }
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            var sd1 = "";
            var sd2 = "";
            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
            {
                sd1 = Wissel1Input;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
            {
                sd2 = Wissel2Input;
            }

            WisselInputs.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            {
                WisselInputs.Add(seld.Naam);
            }

            if (Wissel1 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd1))
            {
                Wissel1Input = sd1;
            }
            if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd2))
            {
                Wissel2Input = sd2;
            }
        }

        #endregion // TLCGen events

        #region Constructor

        public OVIngreepInUitMeldingenDataViewModel(OVIngreepMeldingenDataModel ovIngreepMassaDetectieData)
        {
            OVIngreepMeldingenData = ovIngreepMassaDetectieData;
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
