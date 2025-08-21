﻿
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Controls;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


namespace TLCGen.ViewModels
{
    public class HDIngreepViewModel : ObservableObjectEx
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
                OnPropertyChanged(nameof(KAR), broadcast: true);
WeakReferenceMessengerEx.Default.Send(new PrioIngrepenChangedMessage());
            }
        }

        [Description("Toepassen anti-jutter tijden KAR")]
        public bool KARToepassenFilterTijden
        {
            get => _HDIngreep.KARToepassenFilterTijden;
            set
            {
                _HDIngreep.KARToepassenFilterTijden = value;
                OnPropertyChanged(nameof(KARToepassenFilterTijden), broadcast: true);
            }
        }

        [Description("Inmelding anti-jutter tijd KAR")]
        [BrowsableCondition(nameof(KARToepassenFilterTijden))]
        public int? KARInmeldingFilterTijd
        {
            get => _HDIngreep.KARInmeldingFilterTijd;
            set
            {
                _HDIngreep.KARInmeldingFilterTijd = value;
                OnPropertyChanged(nameof(KARInmeldingFilterTijd), broadcast: true);
            }
        }

        [Description("Uitmelding anti-jutter tijd KAR")]
        [BrowsableCondition(nameof(KARToepassenFilterTijden))]
        public int? KARUitmeldingFilterTijd
        {
            get => _HDIngreep.KARUitmeldingFilterTijd;
            set
            {
                _HDIngreep.KARUitmeldingFilterTijd = value;
                OnPropertyChanged(nameof(KARUitmeldingFilterTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool RIS
        {
            get => _HDIngreep.RIS;
            set
            {
                _HDIngreep.RIS = value;
                OnPropertyChanged(nameof(RIS), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("RIS start (dichtbij ss)")]
        [BrowsableCondition(nameof(RIS))]
        public int RisStart
        {
            get => _HDIngreep.RisStart;
            set
            {
                _HDIngreep.RisStart = value;
                OnPropertyChanged(nameof(RisStart), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("RIS end (verweg ss)")]
        [BrowsableCondition(nameof(RIS))]
        public int RisEnd
        {
            get => _HDIngreep.RisEnd;
            set
            {
                _HDIngreep.RisEnd = value;
                OnPropertyChanged(nameof(RisEnd), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("RIS eta")]
        [BrowsableCondition(nameof(RIS))]
        public int? RisEta
        {
            get => _HDIngreep.RisEta;
            set
            {
                _HDIngreep.RisEta = value;
                OnPropertyChanged(nameof(RisEta), broadcast: true);
            }
        }

        [Description("Check op sirene")]
        public bool Sirene
        {
            get => _HDIngreep.Sirene;
            set
            {
                _HDIngreep.Sirene = value;
                OnPropertyChanged(nameof(Sirene), broadcast: true);
            }
        }

        [Browsable(false)]
        public static bool KARSignaalGroepNummersInParameters =>
            ControllerAccessProvider.Default.Controller.PrioData.KARSignaalGroepNummersInParameters;

        [Browsable(false)]
        public static bool VerlaagHogeSG =>
            ControllerAccessProvider.Default.Controller.PrioData.VerlaagHogeSignaalGroepNummers;

        [Description("KAR richtingnummer HD")]
        [BrowsableCondition(nameof(KARSignaalGroepNummersInParameters))]
        public int KARSignaalGroepNummerHD
        {
            get
            {
                if ((_HDIngreep.KARSignaalGroepNummerHD == 0) && (int.TryParse(_HDIngreep.FaseCyclus, out var iFc)))
                    if ((iFc > 200) && VerlaagHogeSG)
                        return iFc - 200;
                    else
                        return iFc;
                else if ((_HDIngreep.KARSignaalGroepNummerHD > 0) && (_HDIngreep.KARSignaalGroepNummerHD <= 200))
                    return _HDIngreep.KARSignaalGroepNummerHD;
                else if ((_HDIngreep.KARSignaalGroepNummerHD > 200) && (_HDIngreep.KARSignaalGroepNummerHD <= 400))
                    return HDIngreep.KARSignaalGroepNummerHD - 200;
                else
                    return 0;
            }
            set
            {
                if (VerlaagHogeSG && (value > 200) && (value <= 400)) value = value - 200;
                _HDIngreep.KARSignaalGroepNummerHD = value;
                OnPropertyChanged(nameof(KARSignaalGroepNummerHD), broadcast: true);
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
                OnPropertyChanged(nameof(Opticom), broadcast: true);
                OnPropertyChanged(nameof(OpticomOn));
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
                    OnPropertyChanged(nameof(OpticomRelatedInput), broadcast: true);
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
                    OnPropertyChanged(nameof(OpticomInmeldingFilterTijd), broadcast: true);
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
                OnPropertyChanged(nameof(InmeldingOokDoorToepassen), broadcast: true);
            }
        }

        [Browsable(false)]
        public int InmeldingOokDoorFase
        {
            get => _HDIngreep.InmeldingOokDoorFase;
            set
            {
                _HDIngreep.InmeldingOokDoorFase = value;
                OnPropertyChanged(nameof(InmeldingOokDoorFase), broadcast: true);
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
                OnPropertyChanged(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get => _HDIngreep.RijTijdBeperktgehinderd;
            set
            {
                _HDIngreep.RijTijdBeperktgehinderd = value;
                OnPropertyChanged(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get => _HDIngreep.RijTijdGehinderd;
            set
            {
                _HDIngreep.RijTijdGehinderd = value;
                OnPropertyChanged(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get => _HDIngreep.GroenBewaking;
            set
            {
                _HDIngreep.GroenBewaking = value;
                OnPropertyChanged(nameof(GroenBewaking), broadcast: true);
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
                OnPropertyChanged("SelectedFase");
            }
        }

        [Browsable(false)]
        public HDIngreepMeerealiserendeFaseCyclusViewModel SelectedMeerealiserendeFase
        {
            get => _SelectedMeerealiserendeFase;
            set
            {
                _SelectedMeerealiserendeFase = value;
                OnPropertyChanged("SelectedMeerealiserendeFase");
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

        public ObservableCollection<RISVehicleImportanceViewModel> AvailableImportances { get; } = new();

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

        void AddNewMeerealiserendeFaseCommand_Executed()
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

            OnPropertyChanged(broadcast: true);
        }

        bool AddNewMeerealiserendeFaseCommand_CanExecute()
        {
            return MeerealiserendeFasen != null && SelectedFase != null;
        }

        void RemoveMeerealiserendeFaseCommand_Executed()
        {
            MeerealiserendeFasen.Remove(SelectedMeerealiserendeFase);

            BuildFasenList();

            _HDIngreep.MeerealiserendeFaseCycli.BubbleSort();
            MeerealiserendeFasen.Rebuild();

            if (MeerealiserendeFasen.Count > 0)
                SelectedMeerealiserendeFase = MeerealiserendeFasen[MeerealiserendeFasen.Count - 1];
            else
                SelectedMeerealiserendeFase = null;

            OnPropertyChanged(broadcast: true);
        }

        bool RemoveMeerealiserendeFaseCommand_CanExecute()
        {
            return SelectedMeerealiserendeFase != null && MeerealiserendeFasen != null && MeerealiserendeFasen.Count > 0;
        }

        #endregion // Command functionality

        #region Private Methods

        private void IvmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            HDIngreep.RisImportance = 0;
            foreach (var impt in AvailableImportances)
            {
                if (impt.IsSelected) HDIngreep.RisImportance |= impt.Importance;
            }
            OnPropertyChanged(broadcast: true);
        }

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

        private void OnNameChanged(object sender, NameChangedMessage msg)
        {
            RefreshDetectoren();
            OnPropertyChanged("");
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage msg)
        {
            RefreshDetectoren();
            OnPropertyChanged("");
        }

        private void OnFaseDetectorTypeChangedChanged(object sender, FaseDetectorTypeChangedMessage msg)
        {
            RefreshDetectoren();
            OnPropertyChanged("");
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

            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChangedChanged);

            foreach (RISVehicleImportance importance in Enum.GetValues(typeof(RISVehicleImportance)))
            {
                var ivm = new RISVehicleImportanceViewModel
                {
                    Importance = importance,
                    IsSelected = HDIngreep.RisImportance.HasFlag(importance)
                };
                ivm.PropertyChanged += IvmOnPropertyChanged;
                AvailableImportances.Add(ivm);
            }
        }

        #endregion // Constructor
    }
}
