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
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class OVIngreepViewModel : ViewModelBase
    public class OVIngreepViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepLijnNummerViewModel _SelectedLijnNummer;
        private OVIngreepRitCategorieViewModel _SelectedRitCategorie;
        private string _NewLijnNummer;
        private string _NewRitCategorie;

        #endregion // Fields

        #region Properties

        public PrioIngreepModel PrioIngreep { get; set; }

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
            }
        }

        public string DisplayName => PrioIngreep.DisplayName;

        public string Naam
        {
            get => PrioIngreep.Naam;
            set
            {
                var oldName = PrioIngreep.Naam;
                var newName = PrioIngreep.FaseCyclus + value;
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
                }
                else
                {
                    PrioIngreep.Naam = oldName;
                }
                RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
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
            get => _SelectedLijnNummer;
            set
            {
                _SelectedLijnNummer = value;
                RaisePropertyChanged(nameof(SelectedLijnNummer));
            }
        }

        [Browsable(false)]
        public string NewLijnNummer
        {
            get => _NewLijnNummer;
            set
            {
                _NewLijnNummer = value;
                RaisePropertyChanged(nameof(NewLijnNummer));
            }
        }

        [Browsable(false)]
        private ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> _lijnNummers;
        public ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> LijnNummers =>
            _lijnNummers ?? (_lijnNummers = new ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel>(PrioIngreep.LijnNummers));

        [Browsable(false)]
        public bool HasKAR => PrioIngreep.HasPrioIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => PrioIngreep.HasOVIngreepVecom();

        [Browsable(false)]
        public ObservableCollection<string> Detectoren { get; }

        public string Description => PrioIngreep.DisplayName;

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
                    Nummer = NewLijnNummer, RitCategorie = "999"
                };
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(nummer));
            }
            else
            {
                OVIngreepLijnNummerModel nummer = new OVIngreepLijnNummerModel()
                {
                    Nummer = "0", RitCategorie = "999"
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

        public OVIngreepViewModel(PrioIngreepModel ovingreep)
        {
            PrioIngreep = ovingreep;
            
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null);
        }

        #endregion // Constructor
    }
}
