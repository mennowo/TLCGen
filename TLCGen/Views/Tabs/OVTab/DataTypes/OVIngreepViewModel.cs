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

namespace TLCGen.ViewModels
{
    public class OVIngreepViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepLijnNummerViewModel _SelectedLijnNummer;
        private OVIngreepRitCategorieViewModel _SelectedRitCategorie;
        private string _NewLijnNummer;
        private string _NewRitCategorie;

        #endregion // Fields

        #region Properties

        public OVIngreepModel OVIngreep { get; set; }

        [Category("Algemene opties")]
        [Description("Type voertuig")]
        public OVIngreepVoertuigTypeEnum Type
        {
            get { return OVIngreep.Type; }
            set
            {
                OVIngreep.Type = value;
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => OVIngreep.VersneldeInmeldingKoplus;
            set
            {
                OVIngreep.VersneldeInmeldingKoplus = value;
                RaisePropertyChanged<object>(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public string Koplus
        {
            get => OVIngreep.Koplus;
            set
            {
                if(value != null)
                {
                    OVIngreep.Koplus = value;
                }
                RaisePropertyChanged<object>(nameof(Koplus), broadcast: true);
                RaisePropertyChanged(nameof(HasKoplus));
                RaisePropertyChanged(nameof(HasWisselstand));
            }
        }

        [Browsable(false)]
        public bool NoodaanvraagKoplus
        {
            get => OVIngreep.NoodaanvraagKoplus;
            set
            {
                OVIngreep.NoodaanvraagKoplus = value;
                RaisePropertyChanged<object>(nameof(NoodaanvraagKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool KoplusKijkNaarWisselstand
        {
            get => OVIngreep.KoplusKijkNaarWisselstand;
            set
            {
                OVIngreep.KoplusKijkNaarWisselstand = value;
                RaisePropertyChanged<object>(nameof(KoplusKijkNaarWisselstand), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool HasKoplus => !string.IsNullOrWhiteSpace(Koplus) && Koplus != "NG";

        [Browsable(false)]
        public bool HasWisselstand => HasKoplus && OVIngreep.MeldingenData.Wissel1 || OVIngreep.MeldingenData.Wissel2;

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

        [Category("Tijden")]
        [Description("Rijtijd ongehinderd")]
        public int RijTijdOngehinderd
        {
            get { return OVIngreep.RijTijdOngehinderd; }
            set
            {
                OVIngreep.RijTijdOngehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdOngehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd beperkt gehinderd")]
        public int RijTijdBeperktgehinderd
        {
            get { return OVIngreep.RijTijdBeperktgehinderd; }
            set
            {
                OVIngreep.RijTijdBeperktgehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdBeperktgehinderd), broadcast: true);
            }
        }

        [Description("Rijtijd gehinderd")]
        public int RijTijdGehinderd
        {
            get { return OVIngreep.RijTijdGehinderd; }
            set
            {
                OVIngreep.RijTijdGehinderd = value;
                RaisePropertyChanged<object>(nameof(RijTijdGehinderd), broadcast: true);
            }
        }

        [Description("Ondermaximum")]
        public int OnderMaximum
        {
            get { return OVIngreep.OnderMaximum; }
            set
            {
                OVIngreep.OnderMaximum = value;
                RaisePropertyChanged<object>(nameof(OnderMaximum), broadcast: true);
            }
        }

        [Description("Groenbewaking")]
        public int GroenBewaking
        {
            get { return OVIngreep.GroenBewaking; }
            set
            {
                OVIngreep.GroenBewaking = value;
                RaisePropertyChanged<object>(nameof(GroenBewaking), broadcast: true);
            }
        }

        [Description("Blokkeren prio na ingreep")]
        public int BlokkeertijdNaOVIngreep
        {
            get { return OVIngreep.BlokkeertijdNaOVIngreep; }
            set
            {
                OVIngreep.BlokkeertijdNaOVIngreep = value;
                RaisePropertyChanged<object>("BlokkeertijdNaOVIngreep", broadcast: true);
            }
        }

        [Description("Bezettijd OV gehinderd")]
        public int BezettijdOVGehinderd
        {
            get { return OVIngreep.BezettijdOVGehinderd; }
            set
            {
                OVIngreep.BezettijdOVGehinderd = value;
                RaisePropertyChanged<object>("BezettijdOVGehinderd", broadcast: true);
            }
        }

        [Category("Prioriteitsopties")]
        [Description("Afkappen conflicten")]
        public bool AfkappenConflicten
        {
            get { return OVIngreep.AfkappenConflicten; }
            set
            {
                OVIngreep.AfkappenConflicten = value;
                RaisePropertyChanged<object>(nameof(AfkappenConflicten), broadcast: true);
            }
        }

        [Description("Afkappen conflicterend OV")]
        public bool AfkappenConflictenOV
        {
            get { return OVIngreep.AfkappenConflictenOV; }
            set
            {
                OVIngreep.AfkappenConflictenOV = value;
                RaisePropertyChanged<object>(nameof(AfkappenConflictenOV), broadcast: true);
            }
        }

        [Description("Vasthouden groen")]
        public bool VasthoudenGroen
        {
            get { return OVIngreep.VasthoudenGroen; }
            set
            {
                OVIngreep.VasthoudenGroen = value;
                RaisePropertyChanged<object>(nameof(VasthoudenGroen), broadcast: true);
            }
        }

        [Description("Tussendoor realiseren")]
        public bool TussendoorRealiseren
        {
            get { return OVIngreep.TussendoorRealiseren; }
            set
            {
                OVIngreep.TussendoorRealiseren = value;
                RaisePropertyChanged<object>(nameof(TussendoorRealiseren), broadcast: true);
            }
        }

        [Browsable(false)]
        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit
        {
            get { return OVIngreep.GeconditioneerdePrioriteit; }
            set
            {
                OVIngreep.GeconditioneerdePrioriteit = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioriteit), broadcast: true);
                RaisePropertyChanged(nameof(HasGeconditioneerdePrioriteit));
            }
        }
        
        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [Browsable(false)]
        public int GeconditioneerdePrioTeVroeg
        {
            get { return OVIngreep.GeconditioneerdePrioTeVroeg; }
            set
            {
                OVIngreep.GeconditioneerdePrioTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeVroeg), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioOpTijd
        {
            get { return OVIngreep.GeconditioneerdePrioOpTijd; }
            set
            {
                OVIngreep.GeconditioneerdePrioOpTijd = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioOpTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioTeLaat
        {
            get { return OVIngreep.GeconditioneerdePrioTeLaat; }
            set
            {
                OVIngreep.GeconditioneerdePrioTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeLaat), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op lijnnummers")]
        public bool CheckLijnNummer
        {
            get { return OVIngreep.CheckLijnNummer; }
            set
            {
                OVIngreep.CheckLijnNummer = value;
				if(value && !LijnNummers.Any())
				{
					Add10LijnNummersCommand.Execute(null);
				}
                RaisePropertyChanged<object>(nameof(CheckLijnNummer), broadcast: true);
            }
        }

        [Browsable(false)]
        [Description("Check op ritcategorie")]
        public bool CheckRitCategorie
        {
            get { return OVIngreep.CheckRitCategorie; }
            set
            {
                OVIngreep.CheckRitCategorie = value;
                RaisePropertyChanged<object>(nameof(CheckRitCategorie), broadcast: true);
            }
        }

        [Browsable(false)]
        [EnabledCondition(nameof(CheckLijnNummer))]
        [Description("Prioriteit voor alle lijnen")]
        public bool AlleLijnen
        {
            get { return OVIngreep.AlleLijnen; }
            set
            {
                OVIngreep.AlleLijnen = value;
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
        private ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> _lijnNummers;
        public ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel> LijnNummers =>
            _lijnNummers ?? (_lijnNummers = new ObservableCollectionAroundList<OVIngreepLijnNummerViewModel, OVIngreepLijnNummerModel>(OVIngreep.LijnNummers));

        [Browsable(false)]
        public bool HasKAR => OVIngreep.HasOVIngreepKAR();

        [Browsable(false)]
        public bool HasVecom => OVIngreep.HasOVIngreepVecom();

        [Browsable(false)]
        public ObservableCollection<string> Detectoren { get; }

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
                OVIngreep.Koplus = sd1;
                RaisePropertyChanged(nameof(Koplus));
            }
            else
            {
                RaisePropertyChanged(nameof(Koplus));
            }
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public OVIngreepViewModel(OVIngreepModel ovingreep)
        {
            OVIngreep = ovingreep;
            
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null);
        }

        #endregion // Constructor
    }
}
