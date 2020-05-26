using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    public class PrioIngreepViewModel : PrioItemViewModel
    {
        #region Fields

        private OVIngreepLijnNummerViewModel _selectedLijnNummer;
        private OVIngreepRitCategorieViewModel _selectedRitCategorie;
        private string _newLijnNummer;
        private string _newRitCategorie;
        private RelayCommand _addLijnNummerCommand;
        private RelayCommand _add10LijnNummersCommand;
        private RelayCommand _removeLijnNummerCommand;
        private RelayCommand _removeIngreepCommand;
        private readonly FaseCyclusWithPrioViewModel _parentIngreep;
        private ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> _lijnNummers;
        private PrioIngreepWisselDataViewModel _wisselData;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public PrioIngreepModel PrioIngreep { get; set; }

        [Browsable(false)]
        public string FaseCyclus => PrioIngreep.FaseCyclus;

        private ObservableCollection<PrioIngreepMeldingenListViewModel> _meldingenLists;
        public ObservableCollection<PrioIngreepMeldingenListViewModel> MeldingenLists => _meldingenLists ?? (_meldingenLists = new ObservableCollection<PrioIngreepMeldingenListViewModel>());

        [Category("Algemene opties")]
        [Description("Type voertuig")]
        public PrioIngreepVoertuigTypeEnum Type
        {
            get => PrioIngreep.Type;
            set
            {
                PrioIngreep.Type = value;

                foreach (var m in PrioIngreep.MeldingenData.Inmeldingen.Where(m => m.DummyKARMelding != null)) m.DummyKARMelding.Naam = $"dummykarin{PrioIngreep.FaseCyclus}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(value)}";
                foreach (var m in PrioIngreep.MeldingenData.Uitmeldingen.Where(m => m.DummyKARMelding != null)) m.DummyKARMelding.Naam = $"dummykaruit{PrioIngreep.FaseCyclus}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(value)}";

                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
                RaisePropertyChanged(nameof(IsTypeBus));
                RaisePropertyChanged(nameof(IsTypeBicycle));
                RaisePropertyChanged(nameof(IsTypeTram));
                RaisePropertyChanged(nameof(IsTypeTruck));
            }
        }

        [Browsable(false)]
        public bool IsTypeBus => Type == PrioIngreepVoertuigTypeEnum.Bus;
        [Browsable(false)]
        public bool IsTypeBicycle => Type == PrioIngreepVoertuigTypeEnum.Fiets;
        [Browsable(false)]
        public bool IsTypeTram => Type == PrioIngreepVoertuigTypeEnum.Tram;
        [Browsable(false)]
        public bool IsTypeTruck => Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen;

        [Browsable(false)]
        public string DisplayName => PrioIngreep.DisplayName;

        public string Naam
        {
            get => PrioIngreep.Naam;
            set
            {
                var oldName = PrioIngreep.Naam;
                var newName = PrioIngreep.FaseCyclus + value;
                if ((value == "" || NameSyntaxChecker.IsValidCName(value)) &&
                    Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(
                    DataAccess.TLCGenControllerDataProvider.Default.Controller, newName,
                    TLCGenObjectTypeEnum.PrioriteitsIngreep))
                {
                    PrioIngreep.Naam = value;
                    foreach (var melding in PrioIngreep.MeldingenData.Inmeldingen.Where(x =>
                        x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                    {
                        melding.DummyKARMelding.Naam = $"dummykarin{PrioIngreep.FaseCyclus}{value}";
                    }
                    foreach (var melding in PrioIngreep.MeldingenData.Uitmeldingen.Where(x =>
                        x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding))
                    {
                        melding.DummyKARMelding.Naam = $"dummykaruit{PrioIngreep.FaseCyclus}{value}";
                    }
                    RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
                }
                else
                {
                    PrioIngreep.Naam = oldName;
                }
                RaisePropertyChanged(nameof(DisplayName));
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => PrioIngreep.VersneldeInmeldingKoplus;
            set
            {
                PrioIngreep.VersneldeInmeldingKoplus = value;
                RaisePropertyChanged<object>(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public string Koplus
        {
            get => PrioIngreep.Koplus;
            set
            {
                PrioIngreep.Koplus = value;
                if (value == null) PrioIngreep.Koplus = "NG";
                RaisePropertyChanged<object>(nameof(Koplus), broadcast: true);
                RaisePropertyChanged(nameof(HasKoplus));
                RaisePropertyChanged(nameof(HasWisselstand));
            }
        }

        [Browsable(false)]
        public bool NoodaanvraagKoplus
        {
            get => PrioIngreep.NoodaanvraagKoplus;
            set
            {
                PrioIngreep.NoodaanvraagKoplus = value;
                RaisePropertyChanged<object>(nameof(NoodaanvraagKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool KoplusKijkNaarWisselstand
        {
            get => PrioIngreep.KoplusKijkNaarWisselstand;
            set
            {
                PrioIngreep.KoplusKijkNaarWisselstand = value;
                RaisePropertyChanged<object>(nameof(KoplusKijkNaarWisselstand), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasKoplus => !string.IsNullOrWhiteSpace(Koplus) && Koplus != "NG";

        [Browsable(false)]
        public bool HasWisselstand => HasKoplus && PrioIngreep.MeldingenData.Wissel1 || PrioIngreep.MeldingenData.Wissel2;
        
        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get => PrioIngreep.RijTijdOngehinderd;
            set
            {
                PrioIngreep.RijTijdOngehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get => PrioIngreep.RijTijdBeperktgehinderd;
            set
            {
                PrioIngreep.RijTijdBeperktgehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get => PrioIngreep.RijTijdGehinderd;
            set
            {
                PrioIngreep.RijTijdGehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get => PrioIngreep.OnderMaximum;
            set
            {
                PrioIngreep.OnderMaximum = value;
                RaisePropertyChanged<object>(nameof(OnderMaximum), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get => PrioIngreep.GroenBewaking;
            set
            {
                PrioIngreep.GroenBewaking = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Blokkeren prio na ingreep")]
        public int BlokkeertijdNaPrioIngreep
        {
            get => PrioIngreep.BlokkeertijdNaPrioIngreep;
            set
            {
                PrioIngreep.BlokkeertijdNaPrioIngreep = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Bezettijd prio gehinderd")]
        public int BezettijdPrioGehinderd
        {
            get => PrioIngreep.BezettijdPrioGehinderd;
            set
            {
                PrioIngreep.BezettijdPrioGehinderd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Minimale roodtijd\n(t.b.v. inmelden)")]
        public int MinimaleRoodtijd
        {
            get => PrioIngreep.MinimaleRoodtijd;
            set
            {
                PrioIngreep.MinimaleRoodtijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten
        {
            get => PrioIngreep.AfkappenConflicten;
            set
            {
                PrioIngreep.AfkappenConflicten = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Afkappen conflicten prio")]
        public bool AfkappenConflictenPrio
        {
            get => PrioIngreep.AfkappenConflictenPrio;
            set
            {
                PrioIngreep.AfkappenConflictenPrio = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get => PrioIngreep.VasthoudenGroen;
            set
            {
                PrioIngreep.VasthoudenGroen = value;
                RaisePropertyChanged<object>(nameof(VasthoudenGroen), broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get => PrioIngreep.TussendoorRealiseren;
            set
            {
                PrioIngreep.TussendoorRealiseren = value;
                RaisePropertyChanged<object>(nameof(TussendoorRealiseren), broadcast: true);
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit
        {
            get => PrioIngreep.GeconditioneerdePrioriteit;
            set
            {
                PrioIngreep.GeconditioneerdePrioriteit = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioriteit), broadcast: true);
                RaisePropertyChanged(nameof(HasGeconditioneerdePrioriteit));
            }
        }
        
        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [Browsable(false)]
        public int GeconditioneerdePrioTeVroeg
        {
            get => PrioIngreep.GeconditioneerdePrioTeVroeg;
            set
            {
                PrioIngreep.GeconditioneerdePrioTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeVroeg), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioOpTijd
        {
            get => PrioIngreep.GeconditioneerdePrioOpTijd;
            set
            {
                PrioIngreep.GeconditioneerdePrioOpTijd = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioOpTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioTeLaat
        {
            get => PrioIngreep.GeconditioneerdePrioTeLaat;
            set
            {
                PrioIngreep.GeconditioneerdePrioTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeLaat), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op lijnnummers")]
        public bool CheckLijnNummer
        {
            get => PrioIngreep.CheckLijnNummer;
            set
            {
                PrioIngreep.CheckLijnNummer = value;
				if(value && !LijnNummers.Any())
				{
					Add10LijnNummersCommand.Execute(null);
				}
                RaisePropertyChanged<object>(nameof(CheckLijnNummer), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op wagennummers")]
        public bool CheckWagenNummer
        {
            get => PrioIngreep.CheckWagenNummer;
            set
            {
                PrioIngreep.CheckWagenNummer = value;
                RaisePropertyChanged<object>(nameof(CheckWagenNummer), broadcast: true);
                if (!value) return;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleInmeldingen = false;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleUitmeldingen = false;
                foreach (var m in PrioIngreep.MeldingenData.Inmeldingen) m.AntiJutterTijdToepassen = false;
                foreach (var m in PrioIngreep.MeldingenData.Uitmeldingen) m.AntiJutterTijdToepassen = false;
            }
        }

        [Browsable(false)]
        [Description("Check op ritcategorie")]
        public bool CheckRitCategorie
        {
            get => PrioIngreep.CheckRitCategorie;
            set
            {
                PrioIngreep.CheckRitCategorie = value;
                RaisePropertyChanged<object>(nameof(CheckRitCategorie), broadcast: true);
            }
        }

        [Browsable(false)]
        [EnabledCondition(nameof(CheckLijnNummer))]
        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen
        {
            get => PrioIngreep.AlleLijnen;
            set
            {
                PrioIngreep.AlleLijnen = value;
                RaisePropertyChanged<object>(nameof(AlleLijnen), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get => _selectedLijnNummer;
            set
            {
                _selectedLijnNummer = value;
                RaisePropertyChanged(nameof(SelectedLijnNummer));
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get => _newLijnNummer;
            set
            {
                _newLijnNummer = value;
                RaisePropertyChanged(nameof(NewLijnNummer));
            }
        }
        
        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> LijnNummers =>
            _lijnNummers ?? (_lijnNummers = new ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel>(PrioIngreep.LijnNummers));

        [Browsable(false)]
        public bool HasKAR => PrioIngreep.HasPrioIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => PrioIngreep.HasOVIngreepVecom();

        [Browsable(false)]
        public ObservableCollection<string> Detectoren { get; }

        public string Description => PrioIngreep.DisplayName;

        public PrioIngreepWisselDataViewModel WisselData =>
            _wisselData ?? (_wisselData = new PrioIngreepWisselDataViewModel(PrioIngreep.MeldingenData));
        
        #endregion // Properties

        #region Commands

        public ICommand AddLijnNummerCommand
        {
            get
            {
                return _addLijnNummerCommand ?? (_addLijnNummerCommand =
                           new RelayCommand(() => 
                           {
                               if (!string.IsNullOrWhiteSpace(NewLijnNummer))
                               {
                                   var nummer = new OVIngreepLijnNummerModel()
                                   {
                                       Nummer = NewLijnNummer, RitCategorie = "999"
                                   };
                                   LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
                               }
                               else
                               {
                                   var nummer = new OVIngreepLijnNummerModel()
                                   {
                                       Nummer = "0", RitCategorie = "999"
                                   };
                                   LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
                               }
                               NewLijnNummer = "";
                               GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());

                           }, () => LijnNummers != null));
            }
        }

        public ICommand Add10LijnNummersCommand
        {
            get
            {
                if (_add10LijnNummersCommand == null)
                {
                    _add10LijnNummersCommand = new RelayCommand(
                        () =>
                        {
                            for (var i = 0; i < 10; ++i) AddLijnNummerCommand.Execute(null);
                        }, () => LijnNummers != null);
                }
                return _add10LijnNummersCommand;
            }
        }



        public ICommand RemoveLijnNummerCommand
        {
            get
            {
                if (_removeLijnNummerCommand == null)
                {
                    _removeLijnNummerCommand = new RelayCommand(() =>
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
                    }, () => LijnNummers != null && LijnNummers.Count > 0);
                }
                return _removeLijnNummerCommand;
            }
        }

        public ICommand RemoveIngreepCommand
        {
            get
            {
                return _removeIngreepCommand ?? (_removeIngreepCommand =
                           new RelayCommand(() => { _parentIngreep.Ingrepen.Remove(this); }));
            }
        }
        
        #endregion // Commands

        #region Private Methods

        #endregion // Private Methods

        #region TLCGen Messaging

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            var sd1 = Koplus;

            Detectoren.Clear();
            Detectoren.Add("NG");
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.GetAllDetectors(x => x.Type == DetectorTypeEnum.Kop))
            {
                Detectoren.Add(d.Naam);
            }

            if (!string.IsNullOrWhiteSpace(sd1) && Detectoren.Contains(sd1))
            {
                PrioIngreep.Koplus = sd1;
                RaisePropertyChanged(nameof(Koplus));
            }
            else
            {
                PrioIngreep.Koplus = "NG";
                RaisePropertyChanged(nameof(Koplus));
            }
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public PrioIngreepViewModel(PrioIngreepModel ovingreep, FaseCyclusWithPrioViewModel parentIngreep)
        {
            PrioIngreep = ovingreep;
            _parentIngreep = parentIngreep;
            
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null);

            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Inmeldingen", PrioIngreepInUitMeldingTypeEnum.Inmelding, ovingreep.MeldingenData));
            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Uitmeldingen", PrioIngreepInUitMeldingTypeEnum.Uitmelding, ovingreep.MeldingenData));
        }

        #endregion // Constructor
    }
}
