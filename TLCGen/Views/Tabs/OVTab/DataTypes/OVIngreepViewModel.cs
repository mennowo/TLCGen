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

        private OVIngreepModel _OVIngreep;
        private OVIngreepLijnNummerViewModel _SelectedLijnNummer;
        private OVIngreepRitCategorieViewModel _SelectedRitCategorie;
        private ObservableCollection<OVIngreepLijnNummerViewModel> _LijnNummers;
        private ObservableCollection<OVIngreepRitCategorieViewModel> _RitCategorien;
        private string _NewLijnNummer;
        private string _NewRitCategorie;

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

        [Browsable(false)]
        public NooitAltijdAanUitEnum VersneldeInmeldingKoplus
        {
            get => _OVIngreep.VersneldeInmeldingKoplus;
            set
            {
                _OVIngreep.VersneldeInmeldingKoplus = value;
                RaisePropertyChanged<object>(nameof(VersneldeInmeldingKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public string Koplus
        {
            get => _OVIngreep.Koplus;
            set
            {
                if(value != null)
                {
                    _OVIngreep.Koplus = value;
                }
                RaisePropertyChanged<object>(nameof(Koplus), broadcast: true);
                RaisePropertyChanged(nameof(HasKoplus));
                RaisePropertyChanged(nameof(HasWisselstand));
            }
        }

        [Browsable(false)]
        public bool NoodaanvraagKoplus
        {
            get => _OVIngreep.NoodaanvraagKoplus;
            set
            {
                _OVIngreep.NoodaanvraagKoplus = value;
                RaisePropertyChanged<object>(nameof(NoodaanvraagKoplus), broadcast: true);
            }
        }

        [Browsable(false)]
        public bool KoplusKijkNaarWisselstand
        {
            get => _OVIngreep.KoplusKijkNaarWisselstand;
            set
            {
                _OVIngreep.KoplusKijkNaarWisselstand = value;
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
        public NooitAltijdAanUitEnum GeconditioneerdePrioriteit
        {
            get { return _OVIngreep.GeconditioneerdePrioriteit; }
            set
            {
                _OVIngreep.GeconditioneerdePrioriteit = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioriteit), broadcast: true);
                RaisePropertyChanged(nameof(HasGeconditioneerdePrioriteit));
            }
        }
        
        [Browsable(false)]
        public bool HasGeconditioneerdePrioriteit => GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit;

        [Browsable(false)]
        public int GeconditioneerdePrioTeVroeg
        {
            get { return _OVIngreep.GeconditioneerdePrioTeVroeg; }
            set
            {
                _OVIngreep.GeconditioneerdePrioTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeVroeg), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioOpTijd
        {
            get { return _OVIngreep.GeconditioneerdePrioOpTijd; }
            set
            {
                _OVIngreep.GeconditioneerdePrioOpTijd = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioOpTijd), broadcast: true);
            }
        }

        [Browsable(false)]
        public int GeconditioneerdePrioTeLaat
        {
            get { return _OVIngreep.GeconditioneerdePrioTeLaat; }
            set
            {
                _OVIngreep.GeconditioneerdePrioTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioTeLaat), broadcast: true);
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
				if(value && !LijnNummers.Any())
				{
					Add10LijnNummersCommand.Execute(null);
				}
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
        [Description("Check op ritcategorie")]
        public bool CheckRitCategorie
        {
            get { return _OVIngreep.CheckRitCategorie; }
            set
            {
                _OVIngreep.CheckRitCategorie = value;
                if (value && !RitCategorien.Any())
                {
                    Add4RitCategoriesCommand.Execute(null);
                }
                RaisePropertyChanged<object>(nameof(CheckRitCategorie), broadcast: true);
            }
        }

        [Browsable(false)]
        [EnabledCondition(nameof(CheckRitCategorie))]
        [Description("Prioriteit voor alle ritcategoriën")]
        public bool AlleRitCategorien
        {
            get { return _OVIngreep.AlleRitCategorien; }
            set
            {
                _OVIngreep.AlleRitCategorien = value;
                RaisePropertyChanged<object>(nameof(AlleRitCategorien), broadcast: true);
            }
        }

        [Browsable(false)]
        public OVIngreepRitCategorieViewModel SelectedRitCategorie
        {
            get { return _SelectedRitCategorie; }
            set
            {
                _SelectedRitCategorie = value;
                RaisePropertyChanged(nameof(SelectedRitCategorie));
            }
        }

        [Browsable(false)]
        public string NewRitCategorie
        {
            get { return _NewRitCategorie; }
            set
            {
                _NewRitCategorie = value;
                RaisePropertyChanged(nameof(NewRitCategorie));
            }
        }

        [Browsable(false)]
        public ObservableCollection<OVIngreepRitCategorieViewModel> RitCategorien
        {
            get
            {
                if (_RitCategorien == null)
                {
                    _RitCategorien = new ObservableCollection<OVIngreepRitCategorieViewModel>();
                }
                return _RitCategorien;
            }
        }

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

        RelayCommand _AddRitCategorieCommand;
        public ICommand AddRitCategorieCommand
        {
            get
            {
                if (_AddRitCategorieCommand == null)
                {
                    _AddRitCategorieCommand = new RelayCommand(AddRitCategorieCommand_Executed, AddRitCategorieCommand_CanExecute);
                }
                return _AddRitCategorieCommand;
            }
        }

        RelayCommand _Add4RitCategoriesCommand;
        public ICommand Add4RitCategoriesCommand
        {
            get
            {
                if (_Add4RitCategoriesCommand == null)
                {
                    _Add4RitCategoriesCommand = new RelayCommand(Add4RitCategorienCommand_Executed, Add4RitCategorienCommand_CanExecute);
                }
                return _Add4RitCategoriesCommand;
            }
        }


        RelayCommand _RemoveRitCategorieCommand;
        public ICommand RemoveRitCategorieCommand
        {
            get
            {
                if (_RemoveRitCategorieCommand == null)
                {
                    _RemoveRitCategorieCommand = new RelayCommand(RemoveRitCategorieCommand_Executed, RemoveRitCategorieCommand_CanExecute);
                }
                return _RemoveRitCategorieCommand;
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

        void AddRitCategorieCommand_Executed(object prm)
        {
            if (!string.IsNullOrWhiteSpace(NewRitCategorie))
            {
                OVIngreepRitCategorieModel nummer = new OVIngreepRitCategorieModel()
                {
                    Nummer = NewRitCategorie
                };
                RitCategorien.Add(new OVIngreepRitCategorieViewModel(nummer));
            }
            else
            {
                OVIngreepRitCategorieModel nummer = new OVIngreepRitCategorieModel()
                {
                    Nummer = "0"
                };
                RitCategorien.Add(new OVIngreepRitCategorieViewModel(nummer));
            }
            NewRitCategorie = "";
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        bool AddRitCategorieCommand_CanExecute(object prm)
        {
            return RitCategorien != null;
        }

        void Add4RitCategorienCommand_Executed(object prm)
        {
            for (int i = 0; i < 4; ++i)
            {
                AddRitCategorieCommand.Execute(prm);
            }
        }

        bool Add4RitCategorienCommand_CanExecute(object prm)
        {
            return RitCategorien != null;
        }

        void RemoveRitCategorieCommand_Executed(object prm)
        {
            if (SelectedRitCategorie != null)
            {
                RitCategorien.Remove(SelectedRitCategorie);
                SelectedRitCategorie = null;
            }
            else
            {
                RitCategorien.RemoveAt(RitCategorien.Count - 1);
            }
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        bool RemoveRitCategorieCommand_CanExecute(object prm)
        {
            return RitCategorien != null && RitCategorien.Count > 0;
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
                _OVIngreep.Koplus = sd1;
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
            _OVIngreep = ovingreep;
            
            foreach(OVIngreepLijnNummerModel num in _OVIngreep.LijnNummers)
            {
                LijnNummers.Add(new OVIngreepLijnNummerViewModel(num));
            }
           
            LijnNummers.CollectionChanged += LijnNummers_CollectionChanged;

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            Detectoren = new ObservableCollection<string>();
            OnDetectorenChanged(null);
        }

        #endregion // Constructor
    }
}
