using GalaSoft.MvvmLight;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    public class PelotonKoppelingViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _addInkomendeDetectorCommand;
        private RelayCommand _removeInkomendeDetectorCommand;
        private PelotonKoppelingDetectorViewModel _selectedDetector;
        private AddRemoveItemsManager<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel, string> _uitgaandeDetectorenManager;

        #endregion // Fields

        #region Properties

        public PelotonKoppelingModel PelotonKoppeling { get; }

        public string KruisingNaam
        {
            get { return PelotonKoppeling.KruisingNaam; }
            set
            {
                if (NameSyntaxChecker.IsValidName(value))
                {
                    PelotonKoppeling.KruisingNaam = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public Visibility IsInkomend => Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsInkomendDenHaag => Type == PelotonKoppelingType.DenHaag && Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsInkomendRHDHV => Type == PelotonKoppelingType.RHDHV && Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsUitgaand => Richting == PelotonKoppelingRichtingEnum.Uitgaand ? Visibility.Visible : Visibility.Collapsed;

        public string GekoppeldeSignaalGroepNull
        {
            set
            {
                PelotonKoppeling.GekoppeldeSignaalGroep = value;
                RaisePropertyChanged<object>(nameof(GekoppeldeSignaalGroep), broadcast: true);
            }
        }

        public string GekoppeldeSignaalGroep
        {
            get { return PelotonKoppeling.GekoppeldeSignaalGroep; }
            set
            {
                if (value != null)
                {
                    PelotonKoppeling.GekoppeldeSignaalGroep = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
            }
        }

        public int Meetperiode
        {
            get { return PelotonKoppeling.Meetperiode; }
            set
            {
                PelotonKoppeling.Meetperiode = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int Verschuiving
        {
            get { return PelotonKoppeling.Verschuiving; }
            set
            {
                PelotonKoppeling.Verschuiving = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MaximaalHiaat
        {
            get { return PelotonKoppeling.MaximaalHiaat; }
            set
            {
                PelotonKoppeling.MaximaalHiaat = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MinimaalAantalVoertuigen
        {
            get { return PelotonKoppeling.MinimaalAantalVoertuigen; }
            set
            {
                PelotonKoppeling.MinimaalAantalVoertuigen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int TijdTotAanvraag
        {
            get { return PelotonKoppeling.TijdTotAanvraag; }
            set
            {
                PelotonKoppeling.TijdTotAanvraag = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int TijdTotRetourWachtgroen
        {
            get { return PelotonKoppeling.TijdTotRetourWachtgroen; }
            set
            {
                PelotonKoppeling.TijdTotRetourWachtgroen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int TijdRetourWachtgroen
        {
            get { return PelotonKoppeling.TijdRetourWachtgroen; }
            set
            {
                PelotonKoppeling.TijdRetourWachtgroen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int MaxTijdToepassenRetourWachtgroen
        {
            get { return PelotonKoppeling.MaxTijdToepassenRetourWachtgroen; }
            set
            {
                PelotonKoppeling.MaxTijdToepassenRetourWachtgroen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int IngangsSignaalFG
        {
            get { return PelotonKoppeling.IngangsSignaalFG; }
            set
            {
                PelotonKoppeling.IngangsSignaalFG = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum ToepassenAanvraag
        {
            get { return PelotonKoppeling.ToepassenAanvraag; }
            set
            {
                PelotonKoppeling.ToepassenAanvraag = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasAanvraag));
            }
        }

        public NooitAltijdAanUitEnum ToepassenMeetkriterium
        {
            get { return PelotonKoppeling.ToepassenMeetkriterium; }
            set
            {
                PelotonKoppeling.ToepassenMeetkriterium = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum ToepassenRetourWachtgroen
        {
            get { return PelotonKoppeling.ToepassenRetourWachtgroen; }
            set
            {
                PelotonKoppeling.ToepassenRetourWachtgroen = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasRetourWachtgroen));
            }
        }

        public string PTPKruising
        {
            get { return PelotonKoppeling.PTPKruising; }
            set
            {
                if (value != null)
                {
                    PelotonKoppeling.PTPKruising = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
            }
        }

        public HalfstarGekoppeldWijzeEnum KoppelWijze
        {
            get { return PelotonKoppeling.KoppelWijze; }
            set
            {
                PelotonKoppeling.KoppelWijze = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public PelotonKoppelingRichtingEnum Richting
        {
            get { return PelotonKoppeling.Richting; }
            set
            {
                PelotonKoppeling.Richting = value;
                if (value == PelotonKoppelingRichtingEnum.Uitgaand)
                {
                    var rems = Detectoren.Where(x => string.IsNullOrWhiteSpace(x.DetectorNaam)).ToList();
                    foreach (var r in rems)
                    {
                        Detectoren.Remove(r);
                    }
                }
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsInkomend));
                RaisePropertyChanged(nameof(IsInkomendDenHaag));
                RaisePropertyChanged(nameof(IsInkomendRHDHV));
                RaisePropertyChanged(nameof(IsUitgaand));
            }
        }

        public PelotonKoppelingType Type
        {
            get { return PelotonKoppeling.Type; }
            set
            {
                PelotonKoppeling.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(IsInkomend));
                RaisePropertyChanged(nameof(IsInkomendDenHaag));
                RaisePropertyChanged(nameof(IsInkomendRHDHV));
                RaisePropertyChanged(nameof(IsUitgaand));
            }
        }

        public PelotonKoppelingDetectorViewModel SelectedDetector
        {
            get => _selectedDetector;
            set
            {
                _selectedDetector = value;
                _uitgaandeDetectorenManager.SelectedItem = value;
                RaisePropertyChanged();
            }
        }

        public bool HasAanvraag => ToepassenAanvraag != NooitAltijdAanUitEnum.Nooit;

        public bool HasRetourWachtgroen => ToepassenRetourWachtgroen != NooitAltijdAanUitEnum.Nooit;

        public ObservableCollectionAroundList<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel> Detectoren { get; }

        public AddRemoveItemsManager<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel, string> UitgaandeDetectorenManager =>
            _uitgaandeDetectorenManager ??
            (_uitgaandeDetectorenManager = new AddRemoveItemsManager<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel, string>(
                Detectoren,
                x =>
                {
                    var lre = new PelotonKoppelingDetectorViewModel(new PelotonKoppelingDetectorModel()
                    {
                        DetectorNaam = x, KoppelSignaal = 0
                    });
                    return lre;
                },
                (x, y) => x.DetectorNaam == y
                ));


        #endregion // Properties

        #region Commands

        public ICommand AddInkomendeDetectorCommand
        {
            get
            {
                if (_addInkomendeDetectorCommand == null)
                {
                    _addInkomendeDetectorCommand = new RelayCommand(AddInkomendeDetectorCommand_Executed, AddInkomendeDetectorCommand_CanExecute);
                }
                return _addInkomendeDetectorCommand;
            }
        }

        private bool AddInkomendeDetectorCommand_CanExecute()
        {
            return Richting == PelotonKoppelingRichtingEnum.Inkomend;
        }

        private void AddInkomendeDetectorCommand_Executed()
        {
            Detectoren.Add(new PelotonKoppelingDetectorViewModel(new PelotonKoppelingDetectorModel()));
        }

        public ICommand RemoveInkomendeDetectorCommand
        {
            get
            {
                if (_removeInkomendeDetectorCommand == null)
                {
                    _removeInkomendeDetectorCommand = new RelayCommand(RemoveInkomendeDetectorCommand_Executed, RemoveInkomendeDetectorCommand_CanExecute);
                }
                return _removeInkomendeDetectorCommand;
            }
        }

        private bool RemoveInkomendeDetectorCommand_CanExecute()
        {
            return SelectedDetector != null;
        }

        private void RemoveInkomendeDetectorCommand_Executed()
        {
            Detectoren.Remove(SelectedDetector);
            SelectedDetector = Detectoren.Any() ? Detectoren[0] : null;
        }

        #endregion // Commands

        #region Constructor

        public PelotonKoppelingViewModel(PelotonKoppelingModel kop)
        {
            PelotonKoppeling = kop;
            Detectoren = new ObservableCollectionAroundList<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel>(kop.Detectoren);
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Constructor
    }
}
