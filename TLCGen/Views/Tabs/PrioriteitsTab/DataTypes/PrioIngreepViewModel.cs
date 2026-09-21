using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TLCGen.Controls;
using TLCGen.Dependencies.Providers;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class PrioIngreepViewModel : PrioItemViewModel
    {
        #region Fields

        private OVIngreepLijnNummerViewModel _selectedLijnNummer;
        private string _newLijnNummer;
        private RelayCommand _addLijnNummerCommand;
        private RelayCommand _add10LijnNummersCommand;
        private RelayCommand _removeLijnNummerCommand;
        private RelayCommand _removeIngreepCommand;
        private readonly FaseCyclusWithPrioViewModel _parentIngreep;
        private ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> _lijnNummers;
        private PrioIngreepWisselDataViewModel _wisselData;
        private PrioIngreepMeerealiserendeFaseCyclusViewModel _SelectedMeerealiserendeFase;
        private ObservableCollection<string> _Fasen;
        private string _SelectedFase;
        
        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public PrioIngreepModel PrioIngreep { get; set; }

        [Browsable(false)]
        public string FaseCyclus => PrioIngreep.FaseCyclus;

        private ObservableCollection<PrioIngreepMeldingenListViewModel> _meldingenLists;
        private ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel> _gerelateerdePerioden;
        private OVIngreepPeriodeViewModel _selectedPeriode;
        private ItemsManagerViewModel<OVIngreepPeriodeViewModel, string> _gerelateerdePeriodenManager;
        public ObservableCollection<PrioIngreepMeldingenListViewModel> MeldingenLists => _meldingenLists ??= new ObservableCollection<PrioIngreepMeldingenListViewModel>();

        [Category("Algemene opties")]
        [Description("Type voertuig")]
        public PrioIngreepVoertuigTypeEnum Type
        {
            get => PrioIngreep.Type;
            set
            {
                PrioIngreep.Type = value;
                
                foreach (var m in MeldingenLists.SelectMany(x => x.Meldingen)) m.RefreshAvailableTypes();

                foreach (var m in PrioIngreep.MeldingenData.Inmeldingen.Where(m => m.DummyKARMelding != null)) m.DummyKARMelding.Naam = $"dummykarin{PrioIngreep.FaseCyclus}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(value)}";
                foreach (var m in PrioIngreep.MeldingenData.Uitmeldingen.Where(m => m.DummyKARMelding != null)) m.DummyKARMelding.Naam = $"dummykaruit{PrioIngreep.FaseCyclus}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(value)}";

                var meldingen = MeldingenLists.SelectMany(x => x.Meldingen);
                foreach (var melding in meldingen)
                {
                    SetRisRoles(melding);
                }
                
                OnPropertyChanged(nameof(Type), broadcast: true);
                OnPropertyChanged(nameof(IsTypeBus));
                OnPropertyChanged(nameof(IsTypeBicycle));
                OnPropertyChanged(nameof(IsTypeTram));
                OnPropertyChanged(nameof(IsTypeTruck));
                OnPropertyChanged(nameof(IsTypeHulpdienst));
                OnPropertyChanged(nameof(IsNotTypeHulpdienst));
                OnPropertyChanged(nameof(ShowLijnnummer));

                DefaultsProvider.Default.SetDefaultsOnModel(PrioIngreep, PrioIngreep.Type.ToString(), null, false);
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
        public bool IsTypeHulpdienst => Type == PrioIngreepVoertuigTypeEnum.Hulpdienst;
        [Browsable(false)]
        public bool IsNotTypeHulpdienst => Type != PrioIngreepVoertuigTypeEnum.Hulpdienst;
        [Browsable(false)]
        public bool ShowLijnnummer => Type != PrioIngreepVoertuigTypeEnum.Hulpdienst &&
                                      Type != PrioIngreepVoertuigTypeEnum.Fiets &&
                                      Type != PrioIngreepVoertuigTypeEnum.Vrachtwagen;

        [Browsable(false)]
        public string DisplayName => PrioIngreep.DisplayName;

        public string Naam
        {
            get => PrioIngreep.Naam;
            set
            {
                var oldName = PrioIngreep.Naam;
                var newName = PrioIngreep.FaseCyclus + value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    PrioIngreep.Naam = oldName;
                    OnPropertyChanged(nameof(Naam), broadcast: true);
                    OnPropertyChanged(nameof(DisplayName));
                    return;
                }
                if (NameSyntaxChecker.IsValidCName(value) &&
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
                    OnPropertyChanged(nameof(Naam), broadcast: true);
                }
                else
                {
                    PrioIngreep.Naam = oldName;
                }
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => PrioIngreep.VersneldeInmeldingKoplus;
            set
            {
                PrioIngreep.VersneldeInmeldingKoplus = value;
                OnPropertyChanged(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public string Koplus
        {
            get => PrioIngreep.Koplus;
            set
            {
                // Do not set if value is NULL but the existing value is valid
                if (value == null && 
                    PrioIngreep.Koplus != null &&
                    DataAccess.TLCGenControllerDataProvider.Default.Controller.GetAllDetectors(x => x.Type == DetectorTypeEnum.Kop).Any(x => x.Naam == PrioIngreep.Koplus))
                {
                    return;
                }

                PrioIngreep.Koplus = value;
                if (value == null) PrioIngreep.Koplus = "NG";
                OnPropertyChanged(nameof(Koplus), broadcast: true);
                OnPropertyChanged(nameof(HasKoplus));
                OnPropertyChanged(nameof(HasWisselstand));
            }
        }

        [Browsable(false)]
        public bool NoodaanvraagKoplus
        {
            get => PrioIngreep.NoodaanvraagKoplus;
            set
            {
                PrioIngreep.NoodaanvraagKoplus = value;
                OnPropertyChanged(nameof(NoodaanvraagKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool KoplusKijkNaarWisselstand
        {
            get => PrioIngreep.KoplusKijkNaarWisselstand;
            set
            {
                PrioIngreep.KoplusKijkNaarWisselstand = value;
                OnPropertyChanged(nameof(KoplusKijkNaarWisselstand), broadcast: true);
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
                OnPropertyChanged(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get => PrioIngreep.RijTijdBeperktgehinderd;
            set
            {
                PrioIngreep.RijTijdBeperktgehinderd = value;
                OnPropertyChanged(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get => PrioIngreep.RijTijdGehinderd;
            set
            {
                PrioIngreep.RijTijdGehinderd = value;
                OnPropertyChanged(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get => PrioIngreep.OnderMaximum;
            set
            {
                PrioIngreep.OnderMaximum = value;
                OnPropertyChanged(nameof(OnderMaximum), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get => PrioIngreep.GroenBewaking;
            set
            {
                PrioIngreep.GroenBewaking = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Blokkeren prio na ingreep")]
        public int BlokkeertijdNaPrioIngreep
        {
            get => PrioIngreep.BlokkeertijdNaPrioIngreep;
            set
            {
                PrioIngreep.BlokkeertijdNaPrioIngreep = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Bezettijd prio gehinderd")]
        public int BezettijdPrioGehinderd
        {
            get => PrioIngreep.BezettijdPrioGehinderd;
            set
            {
                PrioIngreep.BezettijdPrioGehinderd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Minimale roodtijd\n(t.b.v. inmelden)")]
        public int MinimaleRoodtijd
        {
            get => PrioIngreep.MinimaleRoodtijd;
            set
            {
                PrioIngreep.MinimaleRoodtijd = value;
                OnPropertyChanged(broadcast: true);
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
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Afkappen conflicten prio")]
        public bool AfkappenConflictenPrio
        {
            get => PrioIngreep.AfkappenConflictenPrio;
            set
            {
                PrioIngreep.AfkappenConflictenPrio = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get => PrioIngreep.VasthoudenGroen;
            set
            {
                PrioIngreep.VasthoudenGroen = value;
                OnPropertyChanged(nameof(VasthoudenGroen), broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get => PrioIngreep.TussendoorRealiseren;
            set
            {
                PrioIngreep.TussendoorRealiseren = value;
                OnPropertyChanged(nameof(TussendoorRealiseren), broadcast: true);
            }
        }

        [Description("Prioriteitsniveau")]
        public string PrioriteitsNiveau
        {
            get => PrioIngreep.PrioriteitsNiveau.ToString();
            set
            {
                PrioIngreep.PrioriteitsNiveau = int.Parse(value);
                OnPropertyChanged(nameof(PrioriteitsNiveau), broadcast: true);
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit
        {
            get => PrioIngreep.GeconditioneerdePrioriteit;
            set
            {
                PrioIngreep.GeconditioneerdePrioriteit = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioriteit), broadcast: true);
                OnPropertyChanged(nameof(HasGeconditioneerdePrioriteit));
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
                OnPropertyChanged(nameof(GeconditioneerdePrioTeVroeg), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioOpTijd
        {
            get => PrioIngreep.GeconditioneerdePrioOpTijd;
            set
            {
                PrioIngreep.GeconditioneerdePrioOpTijd = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioOpTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioTeLaat
        {
            get => PrioIngreep.GeconditioneerdePrioTeLaat;
            set
            {
                PrioIngreep.GeconditioneerdePrioTeLaat = value;
                OnPropertyChanged(nameof(GeconditioneerdePrioTeLaat), broadcast: true);
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
                OnPropertyChanged(nameof(CheckLijnNummer), broadcast: true);
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
                OnPropertyChanged(nameof(CheckWagenNummer), broadcast: true);
                if (!value) return;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleInmeldingen = false;
                PrioIngreep.MeldingenData.AntiJutterVoorAlleUitmeldingen = false;
                foreach (var m in PrioIngreep.MeldingenData.Inmeldingen) m.AntiJutterTijdToepassen = false;
                foreach (var m in PrioIngreep.MeldingenData.Uitmeldingen) m.AntiJutterTijdToepassen = false;
            }
        }

        [Browsable(false)]
        [Description("Check op sirene")]
        public bool CheckOpSirene
        {
            get => PrioIngreep.CheckOpSirene;
            set
            {
                PrioIngreep.CheckOpSirene = value;
                OnPropertyChanged(nameof(CheckOpSirene), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool InmeldingOokDoorToepassen
        {
            get => PrioIngreep.InmeldingOokDoorToepassen;
            set
            {
                PrioIngreep.InmeldingOokDoorToepassen = value;
                if (value)
                {
                    if(InmeldingOokDoorFase == 0)
                    {
                        if (int.TryParse(PrioIngreep.FaseCyclus, out var i))
                        {
                            if (PrioIngreep.FaseCyclus.EndsWith("1") || PrioIngreep.FaseCyclus.EndsWith("4") || PrioIngreep.FaseCyclus.EndsWith("7") || PrioIngreep.FaseCyclus.EndsWith("10"))
                            {
                                InmeldingOokDoorFase = i + 1;
                            }
                            else if (PrioIngreep.FaseCyclus.EndsWith("3") || PrioIngreep.FaseCyclus.EndsWith("6") || PrioIngreep.FaseCyclus.EndsWith("9") || PrioIngreep.FaseCyclus.EndsWith("12"))
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
            get => PrioIngreep.InmeldingOokDoorFase;
            set
            {
                PrioIngreep.InmeldingOokDoorFase = value;
                OnPropertyChanged(nameof(InmeldingOokDoorFase), broadcast: true);
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
                _AddMeerealiserendeFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        [Browsable(false)]
        public PrioIngreepMeerealiserendeFaseCyclusViewModel SelectedMeerealiserendeFase
        {
            get => _SelectedMeerealiserendeFase;
            set
            {
                _SelectedMeerealiserendeFase = value;
                OnPropertyChanged("SelectedMeerealiserendeFase");
                _RemoveMeerealiserendeFaseCommand?.NotifyCanExecuteChanged();
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<PrioIngreepMeerealiserendeFaseCyclusViewModel, PrioIngreepMeerealiserendeFaseCyclusModel> MeerealiserendeFasen
        {
            get;
            private set;
        }

        [Browsable(false)]
        [Description("Check op ritcategorie")]
        public bool CheckRitCategorie
        {
            get => PrioIngreep.CheckRitCategorie;
            set
            {
                PrioIngreep.CheckRitCategorie = value;
                OnPropertyChanged(nameof(CheckRitCategorie), broadcast: true);
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
                OnPropertyChanged(nameof(AlleLijnen), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepLijnNummerViewModel SelectedLijnNummer
        {
            get => _selectedLijnNummer;
            set
            {
                _selectedLijnNummer = value;
                OnPropertyChanged(nameof(SelectedLijnNummer));
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get => _newLijnNummer;
            set
            {
                _newLijnNummer = value;
                OnPropertyChanged(nameof(NewLijnNummer));
            }
        }
        
        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> LijnNummers =>
            _lijnNummers ??= new ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel>(PrioIngreep.LijnNummers);

        [Browsable(false)]
        [Description("Check op periode actief")]
        public bool CheckPeriode
        {
            get => PrioIngreep.CheckPeriode;
            set
            {
                PrioIngreep.CheckPeriode = value;
                OnPropertyChanged(nameof(CheckPeriode), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepPeriodeViewModel SelectedPeriode
        {
            get => _selectedPeriode;
            set
            {
                _selectedPeriode = value;
                OnPropertyChanged(nameof(SelectedPeriode));
            }
        }

        [Browsable(false)]
        public ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel> GerelateerdePerioden =>
            _gerelateerdePerioden ??= new ObservableCollectionAroundList<OVIngreepPeriodeViewModel, OVIngreepPeriodeModel>(PrioIngreep.GerelateerdePerioden);

        [Browsable(false)]
        public ItemsManagerViewModel<OVIngreepPeriodeViewModel, string> GerelateerdePeriodenManager => _gerelateerdePeriodenManager ??=
            new ItemsManagerViewModel<OVIngreepPeriodeViewModel, string>(
                GerelateerdePerioden,
                ControllerAccessProvider.Default.AllePerioden.Where(x => x.Type == PeriodeTypeEnum.Overig).Select(x => x.Naam), 
                model => new OVIngreepPeriodeViewModel(new OVIngreepPeriodeModel {Periode = model}),
                model => GerelateerdePerioden.All(x => x.PeriodeNaam != model));

        [Browsable(false)]
        public bool HasKAR => PrioIngreep.HasPrioIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => PrioIngreep.HasOVIngreepVecom();

        [Browsable(false)]
        public ObservableCollection<string> Detectoren { get; }

        [Browsable(false)]
        public ICollectionView OverigePerioden =>
            ControllerAccessProvider.Default.GetCollectionView(PeriodeTypeEnum.Overig);

        public string Description => PrioIngreep.DisplayName;

        public PrioIngreepWisselDataViewModel WisselData =>
            _wisselData ??= new PrioIngreepWisselDataViewModel(PrioIngreep.MeldingenData);
        
        #endregion // Properties

        #region Commands

        public ICommand AddLijnNummerCommand => _addLijnNummerCommand ??= new RelayCommand(() => 
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
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                _removeLijnNummerCommand?.NotifyCanExecuteChanged();
            });

        public ICommand Add10LijnNummersCommand => _add10LijnNummersCommand ??= new RelayCommand(
            () =>
            {
                for (var i = 0; i < 10; ++i) AddLijnNummerCommand.Execute(null);
            }, () => LijnNummers != null);


        public ICommand RemoveLijnNummerCommand => _removeLijnNummerCommand ??= new RelayCommand(() =>
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

                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                _removeLijnNummerCommand?.NotifyCanExecuteChanged();
            }, () => LijnNummers is { Count: > 0 });

        public ICommand RemoveIngreepCommand => _removeIngreepCommand ??= new RelayCommand(() =>
            {
                _parentIngreep.Ingrepen.Remove(this);
                WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(PrioIngreep.FaseCyclus, null, true));
            });

        #region Commands

        RelayCommand _AddMeerealiserendeFaseCommand;
        RelayCommand _RemoveMeerealiserendeFaseCommand;
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
                    new PrioIngreepMeerealiserendeFaseCyclusViewModel(
                        new PrioIngreepMeerealiserendeFaseCyclusModel() { FaseCyclus = SelectedFase }));
            }

            BuildFasenList();

            PrioIngreep.MeerealiserendeFaseCycli.BubbleSort();
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

            PrioIngreep.MeerealiserendeFaseCycli.BubbleSort();
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

        #endregion // Commands

        #region Public Methods

        public void SetRisRoles(PrioIngreepInUitMeldingViewModel ingreepMelding)
        {
            switch (Type)
            {
                case PrioIngreepVoertuigTypeEnum.Tram:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.PUBLICTRANSPORT;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.TRAM;
                    break;
                case PrioIngreepVoertuigTypeEnum.Bus:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.PUBLICTRANSPORT;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.BUS;
                    break;
                case PrioIngreepVoertuigTypeEnum.Fiets:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.DEFAULT;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Vrachtwagen:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.COMMERCIAL | RISVehicleRole.SPECIALTRANSPORT;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Auto:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.DEFAULT;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.UNKNOWN;
                    break;
                case PrioIngreepVoertuigTypeEnum.Hulpdienst:
                    ingreepMelding.PrioIngreepInUitMelding.RisRole = RISVehicleRole.EMERGENCY;
                    ingreepMelding.PrioIngreepInUitMelding.RisSubrole = RISVehicleSubrole.EMERGENCY;
                    break;
                case PrioIngreepVoertuigTypeEnum.NG:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            ingreepMelding.RISViewModel.UpdateRoles();
        }

        #endregion // Public Methods
        
        #region TLCGen Messaging

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage dmsg)
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
                OnPropertyChanged(nameof(Koplus));
            }
            else
            {
                PrioIngreep.Koplus = "NG";
                OnPropertyChanged(nameof(Koplus));
            }
        }

        private void OnFasenChanged(object sender, FasenChangedMessage fmsg)
        {
            BuildFasenList();
        }

        private void OnNameChanged(object sender, NameChangedMessage nmsg)
        {
            OnPropertyChanged(nameof(Naam));
        }

        private void BuildFasenList()
        {
            Fasen.Clear();
            foreach (var f in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen)
            {
                Fasen.Add(f.Naam);
            }
        }
        
        #endregion // TLCGen Messaging

        #region Constructor

        public PrioIngreepViewModel(PrioIngreepModel ovingreep, FaseCyclusWithPrioViewModel parentIngreep)
        {
            PrioIngreep = ovingreep;
            _parentIngreep = parentIngreep;

            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
            WeakReferenceMessengerEx.Default.Register<CCOLVersionChangedMessage>(this, OnCCOLVersionChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null, null);

            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Inmeldingen", PrioIngreepInUitMeldingTypeEnum.Inmelding, ovingreep.MeldingenData, this));
            MeldingenLists.Add(new PrioIngreepMeldingenListViewModel("Uitmeldingen", PrioIngreepInUitMeldingTypeEnum.Uitmelding, ovingreep.MeldingenData, this));

            MeerealiserendeFasen = new ObservableCollectionAroundList<PrioIngreepMeerealiserendeFaseCyclusViewModel, PrioIngreepMeerealiserendeFaseCyclusModel>(ovingreep.MeerealiserendeFaseCycli);

            BuildFasenList();
        }

        private void OnCCOLVersionChanged(object sender, CCOLVersionChangedMessage obj)
        {
            foreach (var m in MeldingenLists.SelectMany(x => x.Meldingen)) m.RefreshAvailableTypes();
        }

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage obj)
        {
            GerelateerdePerioden.Rebuild();
            _gerelateerdePeriodenManager?.Refresh();
        }

        #endregion // Constructor
    }
}
