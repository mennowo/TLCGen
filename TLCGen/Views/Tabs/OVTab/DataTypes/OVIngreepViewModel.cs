using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using System;
using TLCGen.Extensions;

namespace TLCGen.ViewModels
{
    public class OVIngreepViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepModel _OVIngreep;
        private OVIngreepLijnNummerViewModel _SelectedLijnNummer;
        private ObservableCollection<OVIngreepLijnNummerViewModel> _LijnNummers;
        private ObservableCollectionAroundList<OVIngreepMeldingViewModel, OVIngreepMeldingModel> _meldingen;
        private string _NewLijnNummer;

        private ObservableCollection<string> _WisselDetectoren;
        private ObservableCollection<string> _WisselIngangen;

        #endregion // Fields

        #region Properties

        public OVIngreepModel OVIngreep
        {
            get { return _OVIngreep; }
            set
            {
                _OVIngreep = value;
            }
        }
        
        [Browsable(false)]
        public ObservableCollection<OVIngreepMeldingViewModel> Meldingen => _meldingen;

        [Category("Algemene opties")]
        [Description("Type voertuig")]
        public OVIngreepVoertuigTypeEnum Type
        {
            get { return _OVIngreep.Type; }
            set
            {
                _OVIngreep.Type = value;
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        [Description("Versnelde inmelding koplus")]
        [EnabledCondition("HasKoplus")]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => _OVIngreep.VersneldeInmeldingKoplus;
            set
            {
                _OVIngreep.VersneldeInmeldingKoplus = value;
                RaisePropertyChanged<object>(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        //[Description("Min. rijtijd versn. inm.")]
        //[EnabledCondition("VersneldeInmeldingKoplus")]
        //public int MinimaleRijtijdVoorVersneldeInmelding
        //{
        //    get => _OVIngreep.MinimaleRijtijdVoorVersneldeInmelding;
        //    set
        //    {
        //        _OVIngreep.MinimaleRijtijdVoorVersneldeInmelding = value;
        //        RaisePropertyChanged<object>("MinimaleRijtijdVoorVersneldeInmelding", broadcast: true);
        //    }
        //}

        [Browsable(false)]
        public bool HasKoplus
        {
            get
            {
                var fc = TLCGenModelManager.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == _OVIngreep.FaseCyclus);
                return fc != null && fc.Detectoren.Any(x => x.Type == DetectorTypeEnum.Kop);
            }
        }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get { return _OVIngreep.RijTijdOngehinderd; }
            set
            {
                _OVIngreep.RijTijdOngehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get { return _OVIngreep.RijTijdBeperktgehinderd; }
            set
            {
                _OVIngreep.RijTijdBeperktgehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get { return _OVIngreep.RijTijdGehinderd; }
            set
            {
                _OVIngreep.RijTijdGehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get { return _OVIngreep.OnderMaximum; }
            set
            {
                _OVIngreep.OnderMaximum = value;
                RaisePropertyChanged<object>(nameof(OnderMaximum), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get { return _OVIngreep.GroenBewaking; }
            set
            {
                _OVIngreep.GroenBewaking = value;
                RaisePropertyChanged<object>(nameof(GroenBewaking), broadcast: true);
            }
        }

        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten
        {
            get { return _OVIngreep.AfkappenConflicten; }
            set
            {
                _OVIngreep.AfkappenConflicten = value;
                RaisePropertyChanged<object>(nameof(AfkappenConflicten), broadcast: true);
            }
        }

        [Description("Afkappen conflicterend OV")]
        public bool AfkappenConflictenOV
        {
            get { return _OVIngreep.AfkappenConflictenOV; }
            set
            {
                _OVIngreep.AfkappenConflictenOV = value;
                RaisePropertyChanged<object>(nameof(AfkappenConflictenOV), broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get { return _OVIngreep.VasthoudenGroen; }
            set
            {
                _OVIngreep.VasthoudenGroen = value;
                RaisePropertyChanged<object>(nameof(VasthoudenGroen), broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get { return _OVIngreep.TussendoorRealiseren; }
            set
            {
                _OVIngreep.TussendoorRealiseren = value;
                RaisePropertyChanged<object>(nameof(TussendoorRealiseren), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op lijnnummers")]
        public bool CheckLijnNummer
        {
            get { return _OVIngreep.CheckLijnNummer; }
            set
            {
                _OVIngreep.CheckLijnNummer = value;
                RaisePropertyChanged<object>(nameof(CheckLijnNummer), broadcast: true);
            }
        }

        [Browsable(false)]
        [EnabledCondition(nameof(CheckLijnNummer))]
        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen
        {
            get { return _OVIngreep.AlleLijnen; }
            set
            {
                _OVIngreep.AlleLijnen = value;
                RaisePropertyChanged<object>(nameof(AlleLijnen), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get { return _SelectedLijnNummer; }
            set
            {
                _SelectedLijnNummer = value;
                RaisePropertyChanged(nameof(SelectedLijnNummer));
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get { return _NewLijnNummer; }
            set
            {
                _NewLijnNummer = value;
                RaisePropertyChanged(nameof(NewLijnNummer));
            }
        }

        [Browsable(false)]
        public ObservableCollection<OVIngreepLijnNummerViewModel> LijnNummers
        {
            get
            {
                if(_LijnNummers == null)
                {
                    _LijnNummers = new ObservableCollection<OVIngreepLijnNummerViewModel>();
                }
                return _LijnNummers;
            }
        }
        
        [Browsable(false)]
        [Description("Wissel aanwezig")]
        public bool Wissel
        {
            get => _OVIngreep.Wissel;
            set
            {
                _OVIngreep.Wissel = value;
                if (!value)
                {
                    Meldingen.RemoveSome(x => x.Type == OVIngreepMeldingTypeEnum.WisselDetector || x.Type == OVIngreepMeldingTypeEnum.WisselStroomKringDetector);
                }
                else
                {
                    _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                    {
                        FaseCyclus = _OVIngreep.FaseCyclus,
                        Type = OVIngreepMeldingTypeEnum.WisselDetector,
                        Inmelding = false,
                        Uitmelding = false,
                        InmeldingFilterTijd = 15
                    }));
                    _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                    {
                        FaseCyclus = _OVIngreep.FaseCyclus,
                        Type = OVIngreepMeldingTypeEnum.WisselStroomKringDetector,
                        Inmelding = false,
                        Uitmelding = false,
                        InmeldingFilterTijd = 15
                    }));
                }
                RaisePropertyChanged<object>(nameof(Wissel), broadcast: true);
                RaisePropertyChanged(nameof(WisselStandMiddelsDetector));
                RaisePropertyChanged(nameof(WisselStandMiddelsIngang));
            }
    }

        [Browsable(false)]
        public OVIngreepWisselTypeEnum WisselType
        {
            get => _OVIngreep.WisselType;
            set
            {
                if(_OVIngreep.WisselType != value)
                {
                    _OVIngreep.WisselType = value;
                    _OVIngreep.WisselStandInput = null;
                    RaisePropertyChanged<object>(nameof(WisselType), broadcast: true);
                    RaisePropertyChanged(nameof(WisselStandMiddelsDetector));
                    RaisePropertyChanged(nameof(WisselStandMiddelsIngang));
                    RaisePropertyChanged(nameof(WisselStandInput));
                }
            }
        }

        [Browsable(false)]
        public bool WisselStandMiddelsDetector => Wissel && WisselType == OVIngreepWisselTypeEnum.Detector;

        [Browsable(false)]
        public bool WisselStandMiddelsIngang => Wissel && WisselType == OVIngreepWisselTypeEnum.Ingang;

        [Browsable(false)]
        public string WisselStandInput
        {
            get => _OVIngreep.WisselStandInput;
            set
            {
                if (value != null)
                {
                    _OVIngreep.WisselStandInput = value;
                    RaisePropertyChanged<object>(nameof(WisselStandInput), broadcast: true);
                }
            }
        }

        [Browsable(false)]
        public bool WisselStandVoorwaarde
        {
            get => _OVIngreep.WisselStandVoorwaarde;
            set
            {
                _OVIngreep.WisselStandVoorwaarde = value;
                RaisePropertyChanged<object>(nameof(WisselStandVoorwaarde), broadcast: true);
            }
        }

        [Browsable(false)]
        public int UitmeldFilterTijd
        {
            get => _OVIngreep.UitmeldFilterTijd;
            set
            {
                _OVIngreep.UitmeldFilterTijd = value;
                RaisePropertyChanged<object>(nameof(UitmeldFilterTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasKAR => OVIngreep.HasOVIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => OVIngreep.HasOVIngreepVecom();

        public ObservableCollection<string> WisselDetectoren
        {
            get
            {
                if (_WisselDetectoren == null)
                {
                    _WisselDetectoren = new ObservableCollection<string>();
                }
                return _WisselDetectoren;
            }
        }

        public ObservableCollection<string> WisselIngangen
        {
            get
            {
                if (_WisselIngangen == null)
                {
                    _WisselIngangen = new ObservableCollection<string>();
                }
                return _WisselIngangen;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddLijnNummerCommand;
        public ICommand AddLijnNummerCommand
        {
            get
            {
                if (_AddLijnNummerCommand == null)
                {
                    _AddLijnNummerCommand = new RelayCommand(AddLijnNummerCommand_Executed, AddLijnNummerCommand_CanExecute);
                }
                return _AddLijnNummerCommand;
            }
        }

        RelayCommand _Add10LijnNummersCommand;
        public ICommand Add10LijnNummersCommand
        {
            get
            {
                if (_Add10LijnNummersCommand == null)
                {
                    _Add10LijnNummersCommand = new RelayCommand(Add10LijnNummersCommand_Executed, Add10LijnNummersCommand_CanExecute);
                }
                return _Add10LijnNummersCommand;
            }
        }


        RelayCommand _RemoveLijnNummerCommand;
        public ICommand RemoveLijnNummerCommand
        {
            get
            {
                if (_RemoveLijnNummerCommand == null)
                {
                    _RemoveLijnNummerCommand = new RelayCommand(RemoveLijnNummerCommand_Executed, RemoveLijnNummerCommand_CanExecute);
                }
                return _RemoveLijnNummerCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddLijnNummerCommand_Executed(object prm)
        {
            if (!string.IsNullOrWhiteSpace(NewLijnNummer))
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel()
                {
                    Nummer = NewLijnNummer
                };
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            else
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel()
                {
                    Nummer = "0"
                };
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            NewLijnNummer = "";
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        bool AddLijnNummerCommand_CanExecute(object prm)
        {
            return LijnNummers != null;
        }

        void Add10LijnNummersCommand_Executed(object prm)
        {
            for(int i = 0; i < 10; ++i)
            {
                AddLijnNummerCommand.Execute(prm);
            }
        }

        bool Add10LijnNummersCommand_CanExecute(object prm)
        {
            return LijnNummers != null;
        }

        void RemoveLijnNummerCommand_Executed(object prm)
        {
            if (SelectedLijnNummer != null)
            {
                LijnNummers.Remove(SelectedLijnNummer);
                SelectedLijnNummer = null;
            }
            else
            {
                LijnNummers.RemoveAt(LijnNummers.Count - 1);
            }
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        bool RemoveLijnNummerCommand_CanExecute(object prm)
        {
            return LijnNummers != null && LijnNummers.Count > 0;
        }

        #endregion // Command functionality

        #region Collection changed

        private void LijnNummers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (OVIngreepLijnNummerViewModel num in e.NewItems)
                {
                    _OVIngreep.LijnNummers.Add(num.LijnNummer);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (OVIngreepLijnNummerViewModel num in e.OldItems)
                {
                    _OVIngreep.LijnNummers.Remove(num.LijnNummer);
                }
            }
        }

        #endregion // Collection changed

        #region Private Methods

        private void RefreshDetectoren()
        {
            WisselDetectoren.Clear();
            WisselIngangen.Clear();
            if (DataAccess.TLCGenControllerDataProvider.Default.Controller == null) return;

            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                SelectMany(x => x.Detectoren))
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.WisselDetector:
                        WisselDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.WisselIngang:
                        WisselIngangen.Add(d.Naam);
                        break;
                }
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren)
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.WisselDetector:
                        WisselDetectoren.Add(d.Naam);
                        break;
                    case DetectorTypeEnum.WisselIngang:
                        WisselIngangen.Add(d.Naam);
                        break;
                }
            }
        }

        #endregion // Private Methods

        #region TLCGen Messaging

        private void OnNameChanged(NameChangedMessage msg)
        {
            RefreshDetectoren();
            RaisePropertyChanged("");
        }

        private void OnDetectorenChanged(DetectorenChangedMessage msg)
        {
            RefreshDetectoren();
            RaisePropertyChanged("");
        }

        private void OnFaseDetectorTypeChangedChanged(FaseDetectorTypeChangedMessage msg)
        {
            RefreshDetectoren();
            RaisePropertyChanged("");
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public OVIngreepViewModel(OVIngreepModel ovingreep)
        {
            _OVIngreep = ovingreep;

            foreach(OVIngreepLijnNummerModel num in _OVIngreep.LijnNummers)
            {
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(num));
            }

            LijnNummers.CollectionChanged += LijnNummers_CollectionChanged;

            _meldingen = new ObservableCollectionAroundList<OVIngreepMeldingViewModel, OVIngreepMeldingModel>(ovingreep.Meldingen);

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.KAR))
            {
                _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.KAR,
                    Inmelding = true,
                    Uitmelding = true,
                    InmeldingFilterTijd = 15
                }));
            }

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.VECOM))
            {
                var vec = new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.VECOM,
                    Inmelding = false,
                    Uitmelding = false,
                    InmeldingFilterTijd = 15
                };
                _meldingen.Add(new OVIngreepMeldingViewModel(vec));
            }

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.VECOM_io))
            {
                _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.VECOM_io,
                    Inmelding = false,
                    Uitmelding = false,
                    InmeldingFilterTijd = 15
                }));
            }

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.VerlosDetector))
            {
                _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.VerlosDetector,
                    Inmelding = false,
                    Uitmelding = false,
                    InmeldingFilterTijd = 15
                }));
            }

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.MassaPaarIn))
            {
                _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.MassaPaarIn,
                    Inmelding = false,
                    Uitmelding = false,
                    InmeldingFilterTijd = 15
                }));
            }

            if (!_meldingen.Any(x => x.Type == OVIngreepMeldingTypeEnum.MassaPaarUit))
            {
                _meldingen.Add(new OVIngreepMeldingViewModel(new OVIngreepMeldingModel
                {
                    FaseCyclus = ovingreep.FaseCyclus,
                    Type = OVIngreepMeldingTypeEnum.MassaPaarUit,
                    Inmelding = false,
                    Uitmelding = false,
                    InmeldingFilterTijd = 15
                }));
            }

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChangedChanged);

            RefreshDetectoren();
        }

        #endregion // Constructor
    }
}
