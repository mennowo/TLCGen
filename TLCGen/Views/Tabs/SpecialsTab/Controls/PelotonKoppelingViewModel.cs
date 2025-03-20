using CommunityToolkit.Mvvm.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PelotonKoppelingViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private RelayCommand _addInkomendeDetectorCommand;
        private RelayCommand _removeInkomendeDetectorCommand;
        private PelotonKoppelingDetectorViewModel _selectedDetector;
        private AddRemoveItemsManager<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel, string> _uitgaandeDetectorenManager;

        #endregion // Fields

        #region Properties

        public PelotonKoppelingModel PelotonKoppeling { get; }

        public string KoppelingNaam
        {
            get => PelotonKoppeling.KoppelingNaam;
            set
            {
                if (NameSyntaxChecker.IsValidCName(value) && TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.PelotonKoppeling, value))
                {
                    var oldname = PelotonKoppeling.KoppelingNaam;
                    PelotonKoppeling.KoppelingNaam = value;

                    // Notify the messenger
                    WeakReferenceMessengerEx.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.PelotonKoppeling, oldname, PelotonKoppeling.KoppelingNaam));
                    OnPropertyChanged(broadcast: true);
                }
                else
                {
                    OnPropertyChanged();
                }
            }
        }

        public Visibility IsInkomend => Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsInkomendDenHaag => Type == PelotonKoppelingTypeEnum.DenHaag && Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsInkomendRHDHV => Type == PelotonKoppelingTypeEnum.RHDHV && Richting == PelotonKoppelingRichtingEnum.Inkomend ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsUitgaand => Richting == PelotonKoppelingRichtingEnum.Uitgaand ? Visibility.Visible : Visibility.Collapsed;

        public string GekoppeldeSignaalGroepNull
        {
            set
            {
                PelotonKoppeling.GekoppeldeSignaalGroep = value;
                OnPropertyChanged(nameof(GekoppeldeSignaalGroep), broadcast: true);
            }
        }

        public string GekoppeldeSignaalGroep
        {
            get => PelotonKoppeling.GekoppeldeSignaalGroep;
            set
            {
                if (value != null)
                {
                    PelotonKoppeling.GekoppeldeSignaalGroep = value;
                    OnPropertyChanged(broadcast: true);
                }
            }
        }

        public int Meetperiode
        {
            get => PelotonKoppeling.Meetperiode;
            set
            {
                PelotonKoppeling.Meetperiode = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int Verschuiving
        {
            get => PelotonKoppeling.Verschuiving;
            set
            {
                PelotonKoppeling.Verschuiving = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MaximaalHiaat
        {
            get => PelotonKoppeling.MaximaalHiaat;
            set
            {
                PelotonKoppeling.MaximaalHiaat = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MinimaalAantalVoertuigen
        {
            get => PelotonKoppeling.MinimaalAantalVoertuigen;
            set
            {
                PelotonKoppeling.MinimaalAantalVoertuigen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int TijdTotAanvraag
        {
            get => PelotonKoppeling.TijdTotAanvraag;
            set
            {
                PelotonKoppeling.TijdTotAanvraag = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int TijdTotRetourWachtgroen
        {
            get => PelotonKoppeling.TijdTotRetourWachtgroen;
            set
            {
                PelotonKoppeling.TijdTotRetourWachtgroen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int TijdRetourWachtgroen
        {
            get => PelotonKoppeling.TijdRetourWachtgroen;
            set
            {
                PelotonKoppeling.TijdRetourWachtgroen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MaxTijdToepassenRetourWachtgroen
        {
            get => PelotonKoppeling.MaxTijdToepassenRetourWachtgroen;
            set
            {
                PelotonKoppeling.MaxTijdToepassenRetourWachtgroen = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public NooitAltijdAanUitEnum ToepassenAanvraag
        {
            get => PelotonKoppeling.ToepassenAanvraag;
            set
            {
                PelotonKoppeling.ToepassenAanvraag = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasAanvraag));
            }
        }

        public NooitAltijdAanUitEnum ToepassenMeetkriterium
        {
            get => PelotonKoppeling.ToepassenMeetkriterium;
            set
            {
                PelotonKoppeling.ToepassenMeetkriterium = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum ToepassenRetourWachtgroen
        {
            get => PelotonKoppeling.ToepassenRetourWachtgroen;
            set
            {
                PelotonKoppeling.ToepassenRetourWachtgroen = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasRetourWachtgroen));
            }
        }

        public bool IsIntern
        {
            get => PelotonKoppeling.IsIntern;
            set
            {
                PelotonKoppeling.IsIntern = value;
                if (value)
                {
                    PelotonKoppeling.PTPKruising = "INTERN";
                }
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(PTPKruising));
                OnPropertyChanged(nameof(IsNotIntern));
                OnPropertyChanged(nameof(IsInternIn));
                OnPropertyChanged(nameof(IsInternUit));
            }
        }

        public bool IsInternIn => IsIntern && Richting == PelotonKoppelingRichtingEnum.Inkomend;
        public bool IsInternUit => IsIntern && Richting == PelotonKoppelingRichtingEnum.Uitgaand;
        public bool IsNotIntern => !IsIntern;

        public string GerelateerdePelotonKoppeling
        {
            get => PelotonKoppeling.GerelateerdePelotonKoppeling;
            set
            {
                if (value != null)
                {
                    PelotonKoppeling.GerelateerdePelotonKoppeling = value;
                    OnPropertyChanged(broadcast: true);
                }
            }
        }

        public string PTPKruising
        {
            get => PelotonKoppeling.PTPKruising;
            set
            {
                if (value != null)
                {
                    PelotonKoppeling.PTPKruising = value;
                    OnPropertyChanged(broadcast: true);
                    if (value == "INTERN") IsIntern = true;
                }
            }
        }

        public HalfstarGekoppeldWijzeEnum KoppelWijze
        {
            get => PelotonKoppeling.KoppelWijze;
            set
            {
                PelotonKoppeling.KoppelWijze = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool HasRichting => !(Richting == PelotonKoppelingRichtingEnum.Uitgaand && Type == PelotonKoppelingTypeEnum.RHDHV);
        
        public PelotonKoppelingRichtingEnum Richting
        {
            get => PelotonKoppeling.Richting;
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
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(IsInkomend));
                OnPropertyChanged(nameof(IsInkomendDenHaag));
                OnPropertyChanged(nameof(IsInkomendRHDHV));
                OnPropertyChanged(nameof(IsUitgaand));
                OnPropertyChanged(nameof(HasRichting));
            }
        }

        public PelotonKoppelingTypeEnum Type
        {
            get => PelotonKoppeling.Type;
            set
            {
                PelotonKoppeling.Type = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(IsInkomend));
                OnPropertyChanged(nameof(IsInkomendDenHaag));
                OnPropertyChanged(nameof(IsInkomendRHDHV));
                OnPropertyChanged(nameof(IsUitgaand));
                OnPropertyChanged(nameof(HasRichting));
            }
        }

        public PelotonKoppelingDetectorViewModel SelectedDetector
        {
            get => _selectedDetector;
            set
            {
                _selectedDetector = value;
                _uitgaandeDetectorenManager.SelectedItem = value;
                OnPropertyChanged();
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
                        DetectorNaam = x
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

        #region IViewModelWithItem

        public object GetItem()
        {
            return PelotonKoppeling;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public PelotonKoppelingViewModel(PelotonKoppelingModel kop)
        {
            PelotonKoppeling = kop;
            Detectoren = new ObservableCollectionAroundList<PelotonKoppelingDetectorViewModel, PelotonKoppelingDetectorModel>(kop.Detectoren);
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Constructor
    }
}
