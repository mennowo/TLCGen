using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class HDIngreepViewModel : ViewModelBase
    {
        #region Fields

        private HDIngreepModel _HDIngreep;
        private HDIngreepMeerealiserendeFaseCyclusViewModel _SelectedMeerealiserendeFase;
        private ObservableCollection<string> _Fasen;
        private string _SelectedFase;
        private ControllerModel _Controller;
        private ObservableCollection<string> _OpticomIngangen;

        #endregion // Fields

        #region Properties

        public HDIngreepModel HDIngreep
        {
            get => _HDIngreep;
            set => _HDIngreep = value;
        }

        [Category("Opties")]
        public bool KAR
        {
            get => _HDIngreep.KAR;
            set
            {
                _HDIngreep.KAR = value;
                if (value)
                {
                    _HDIngreep.DummyKARInmelding = new DetectorModel { Dummy = true };
                    _HDIngreep.DummyKARUitmelding = new DetectorModel { Dummy = true };
                    _HDIngreep.DummyKARInmelding.Naam = "dummyhdkarin" + _HDIngreep.FaseCyclus;
                    _HDIngreep.DummyKARUitmelding.Naam = "dummyhdkaruit" + _HDIngreep.FaseCyclus;
                }
                else
                {
                    _HDIngreep.DummyKARInmelding = null;
                    _HDIngreep.DummyKARUitmelding = null;
                }
                RaisePropertyChanged<object>(nameof(KAR), broadcast: true);
                Messenger.Default.Send(new PrioIngrepenChangedMessage());
            }
        }

        [Description("Inmelding filtertijd KAR")]
        public int? KARInmeldingFilterTijd
        {
            get => _HDIngreep.KARInmeldingFilterTijd;
            set
            {
                _HDIngreep.KARInmeldingFilterTijd = value;
                RaisePropertyChanged<object>(nameof(KARInmeldingFilterTijd), broadcast: true);
            }
        }

        [Description("Uitmelding filtertijd KAR")]
        public int? KARUitmeldingFilterTijd
        {
            get => _HDIngreep.KARUitmeldingFilterTijd;
            set
            {
                _HDIngreep.KARUitmeldingFilterTijd = value;
                RaisePropertyChanged<object>(nameof(KARUitmeldingFilterTijd), broadcast: true);
            }
        }

        public bool RIS
        {
            get => _HDIngreep.RIS;
            set
            {
                _HDIngreep.RIS = value;
                RaisePropertyChanged<object>(nameof(RIS), broadcast: true);
            }
        }

        [Description("RIS start (dichtbij ss)")]
        [BrowsableCondition(nameof(RIS))]
        public int RisStart
        {
            get => _HDIngreep.RisStart;
            set
            {
                _HDIngreep.RisStart = value;
                RaisePropertyChanged<object>(nameof(RisStart), broadcast: true);
            }
        }

        [Description("RIS end (verweg ss)")]
        [BrowsableCondition(nameof(RIS))]
        public int RisEnd
        {
            get => _HDIngreep.RisEnd;
            set
            {
                _HDIngreep.RisEnd = value;
                RaisePropertyChanged<object>(nameof(RisEnd), broadcast: true);
            }
        }

        [Description("RIS eta")]
        [BrowsableCondition(nameof(RIS))]
        public int? RisEta
        {
            get => _HDIngreep.RisEta;
            set
            {
                _HDIngreep.RisEta = value;
                RaisePropertyChanged<object>(nameof(RisEta), broadcast: true);
            }
        }

        [Description("Check op sirene")]
        public bool Sirene
        {
            get => _HDIngreep.Sirene;
            set
            {
                _HDIngreep.Sirene = value;
                RaisePropertyChanged<object>(nameof(Sirene), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool OpticomAvailable => OpticomIngangen != null && OpticomIngangen.Any();

        [Browsable(false)]
        public bool OpticomOn => Opticom && OpticomIngangen != null && OpticomIngangen.Any();

        [Browsable(false)]
        public bool Opticom
        {
            get => _HDIngreep.Opticom;
            set
            {
                _HDIngreep.Opticom = value;
                RaisePropertyChanged<object>(nameof(Opticom), broadcast: true);
                RaisePropertyChanged(nameof(OpticomOn));
            }
        }

        [Browsable(false)]
        public string OpticomRelatedInput
        {
            get => _HDIngreep.OpticomRelatedInput;
            set
            {
                if(value != null)
                {
                    _HDIngreep.OpticomRelatedInput = value;
                    RaisePropertyChanged<object>(nameof(OpticomRelatedInput), broadcast: true);
                }
            }
        }

        [Browsable(false)]
        public int? OpticomInmeldingFilterTijd
        {
            get => _HDIngreep.OpticomInmeldingFilterTijd;
            set
            {
                if (value != null)
                {
                    _HDIngreep.OpticomInmeldingFilterTijd = value;
                    RaisePropertyChanged<object>(nameof(OpticomInmeldingFilterTijd), broadcast: true);
                }
            }
        }

        [Browsable(false)]
        public bool InmeldingOokDoorToepassen
        {
            get => _HDIngreep.InmeldingOokDoorToepassen;
            set
            {
                _HDIngreep.InmeldingOokDoorToepassen = value;
                if (value)
                {
                    if(InmeldingOokDoorFase == 0)
                    {
                        if (int.TryParse(_HDIngreep.FaseCyclus, out var i))
                        {
                            if (_HDIngreep.FaseCyclus.EndsWith("1") || _HDIngreep.FaseCyclus.EndsWith("4") || _HDIngreep.FaseCyclus.EndsWith("7") || _HDIngreep.FaseCyclus.EndsWith("10"))
                            {
                                InmeldingOokDoorFase = i + 1;
                            }
                            else if (_HDIngreep.FaseCyclus.EndsWith("3") || _HDIngreep.FaseCyclus.EndsWith("6") || _HDIngreep.FaseCyclus.EndsWith("9") || _HDIngreep.FaseCyclus.EndsWith("12"))
                            {
                                InmeldingOokDoorFase = i - 1;
                            }
                            else
                            {
                                InmeldingOokDoorFase = i;
                            }
                        }
                    }
                }
                RaisePropertyChanged<object>(nameof(InmeldingOokDoorToepassen), broadcast: true);
            }
        }

        [Browsable(false)]
        public int InmeldingOokDoorFase
        {
            get => _HDIngreep.InmeldingOokDoorFase;
            set
            {
                _HDIngreep.InmeldingOokDoorFase = value;
                RaisePropertyChanged<object>(nameof(InmeldingOokDoorFase), broadcast: true);
            }
        }

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get => _HDIngreep.RijTijdOngehinderd;
            set
            {
                _HDIngreep.RijTijdOngehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get => _HDIngreep.RijTijdBeperktgehinderd;
            set
            {
                _HDIngreep.RijTijdBeperktgehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get => _HDIngreep.RijTijdGehinderd;
            set
            {
                _HDIngreep.RijTijdGehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get => _HDIngreep.GroenBewaking;
            set
            {
                _HDIngreep.GroenBewaking = value;
                RaisePropertyChanged<object>(nameof(GroenBewaking), broadcast: true);
            }
        }

        [Browsable(false)]
        public ObservableCollection<string> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        [Browsable(false)]
        public string SelectedFase
        {
            get => _SelectedFase;
            set
            {
                _SelectedFase = value;
                RaisePropertyChanged("SelectedFase");
            }
        }

        [Browsable(false)]
        public HDIngreepMeerealiserendeFaseCyclusViewModel SelectedMeerealiserendeFase
        {
            get => _SelectedMeerealiserendeFase;
            set
            {
                _SelectedMeerealiserendeFase = value;
                RaisePropertyChanged("SelectedMeerealiserendeFase");
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<HDIngreepMeerealiserendeFaseCyclusViewModel, HDIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFasen
        {
            get;
            private set;
        }

        public ObservableCollection<string> OpticomIngangen
        {
            get
            {
                if (_OpticomIngangen == null)
                {
                    _OpticomIngangen = new ObservableCollection<string>();
                }
                return _OpticomIngangen;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddMeerealiserendeFaseCommand;
        public ICommand AddMeerealiserendeFaseCommand
        {
            get
            {
                if (_AddMeerealiserendeFaseCommand == null)
                {
                    _AddMeerealiserendeFaseCommand = new RelayCommand(AddNewMeerealiserendeFaseCommand_Executed, AddNewMeerealiserendeFaseCommand_CanExecute);
                }
                return _AddMeerealiserendeFaseCommand;
            }
        }


        RelayCommand _RemoveMeerealiserendeFaseCommand;
        public ICommand RemoveMeerealiserendeFaseCommand
        {
            get
            {
                if (_RemoveMeerealiserendeFaseCommand == null)
                {
                    _RemoveMeerealiserendeFaseCommand = new RelayCommand(RemoveMeerealiserendeFaseCommand_Executed, RemoveMeerealiserendeFaseCommand_CanExecute);
                }
                return _RemoveMeerealiserendeFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewMeerealiserendeFaseCommand_Executed(object prm)
        {
            if (!(MeerealiserendeFasen.Where(x => x.FaseCyclus.FaseCyclus == SelectedFase).Count() > 0))
            {
                MeerealiserendeFasen.Add(
                    new HDIngreepMeerealiserendeFaseCyclusViewModel(
                        new HDIngreepMeerealiserendeFaseCyclusModel() { FaseCyclus = SelectedFase }));
            }

            BuildFasenList();

            _HDIngreep.MeerealiserendeFaseCycli.BubbleSort();
            MeerealiserendeFasen.Rebuild();

            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];

            RaisePropertyChanged<object>(broadcast: true);
        }

        bool AddNewMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return MeerealiserendeFasen != null && SelectedFase != null;
        }

        void RemoveMeerealiserendeFaseCommand_Executed(object prm)
        {
            MeerealiserendeFasen.Remove(SelectedMeerealiserendeFase);

            BuildFasenList();

            _HDIngreep.MeerealiserendeFaseCycli.BubbleSort();
            MeerealiserendeFasen.Rebuild();

            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];
            else
                SelectedMeerealiserendeFase = null;

            RaisePropertyChanged<object>(broadcast: true);
        }

        bool RemoveMeerealiserendeFaseCommand_CanExecute(object prm)
        {
            return SelectedMeerealiserendeFase != null && MeerealiserendeFasen != null && MeerealiserendeFasen.Count > 0;
        }

        #endregion // Command functionality

        #region Private Methods
        
        private void BuildFasenList()
        {
            Fasen.Clear();
            foreach (var m in _Controller.Fasen)
            {
                if (m.Naam != _HDIngreep.FaseCyclus && 
                    m.HDIngreep &&
                    !(_HDIngreep.MeerealiserendeFaseCycli.Where(x => x.FaseCyclus == m.Naam).Count() > 0))
                    Fasen.Add(m.Naam);
            }
            if (Fasen.Count > 0)
            {
                SelectedFase = Fasen[0];
            }
        }

        private void RefreshDetectoren()
        {
            OpticomIngangen.Clear();
            if (DataAccess.TLCGenControllerDataProvider.Default.Controller == null) return;

            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                SelectMany(x => x.Detectoren))
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.OpticomIngang:
                        OpticomIngangen.Add(d.Naam);
                        break;
                }
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren)
            {
                switch (d.Type)
                {
                    case DetectorTypeEnum.OpticomIngang:
                        OpticomIngangen.Add(d.Naam);
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

        public HDIngreepViewModel(ControllerModel controller, HDIngreepModel hdingreep)
        {
            _HDIngreep = hdingreep;
            _Controller = controller;

            BuildFasenList();

            MeerealiserendeFasen = new ObservableCollectionAroundList<HDIngreepMeerealiserendeFaseCyclusViewModel, HDIngreepMeerealiserendeFaseCyclusModel>(hdingreep.MeerealiserendeFaseCycli);

            RefreshDetectoren();

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChangedChanged);
        }

        #endregion // Constructor
    }
}
