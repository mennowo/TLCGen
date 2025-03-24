using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;


namespace TLCGen.ViewModels
{
    public class FileIngreepViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private FileIngreepModel _FileIngreep;

        private string _SelectedFaseNaam;
        
        private List<string> _ControllerFasen;
        private ObservableCollection<string> _SelectableFasen;
        
        private FileIngreepTeDoserenSignaalGroepViewModel _SelectedTeDoserenFase;
        
        private RelayCommand _AddTeDoserenSignaalGroepCommand;
        private RelayCommand _RemoveTeDoserenSignaalGroepCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<FileIngreepDetectorViewModel, FileIngreepDetectorModel> FileDetectoren
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<FileIngreepTeDoserenSignaalGroepViewModel, FileIngreepTeDoserenSignaalGroepModel> TeDoserenSignaalGroepen
        {
            get;
            private set;
        }

        public ObservableCollection<string> SelectableFasen
        {
            get
            {
                if (_SelectableFasen == null)
                {
                    _SelectableFasen = new ObservableCollection<string>();
                }
                return _SelectableFasen;
            }
        }
        
        [Browsable(false)]
        public string SelectedFaseNaam
        {
            get => _SelectedFaseNaam;
            set
            {
                _SelectedFaseNaam = value;
                OnPropertyChanged("SelectedFaseNaam");
                _AddTeDoserenSignaalGroepCommand?.NotifyCanExecuteChanged();
            }
        }

        public FileIngreepTeDoserenSignaalGroepViewModel SelectedTeDoserenFase
        {
            get => _SelectedTeDoserenFase;
            set
            {
                _SelectedTeDoserenFase = value;
                OnPropertyChanged("SelectedTeDoserenFase");
                _RemoveTeDoserenSignaalGroepCommand?.NotifyCanExecuteChanged();
            }
        }

        public FileIngreepDetectorViewModel SelectedFileDetector
        {
            get => DetectorManager.SelectedItem;
            set
            {
                DetectorManager.SelectedItem = value;
                OnPropertyChanged("SelectedFileDetector");
            }
        }

        public string Naam
        {
            get => _FileIngreep.Naam;
            set
            {
	            if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
	            {
                    if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.FileIngreep, value))
                    {
                        var oldname = _FileIngreep.Naam;
			            _FileIngreep.Naam = value;

						// Notify the messenger
WeakReferenceMessengerEx.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.FileIngreep, oldname, value));
						OnPropertyChanged(nameof(Naam), broadcast: true);
		            }
				}
            }
        }
        
        public bool AanUit
        {
            get => _FileIngreep.AanUit;
            set
            {
                _FileIngreep.AanUit = value;
                OnPropertyChanged("AanUit");
            }
        }

        public bool MetingPerLus
        {
            get => _FileIngreep.MetingPerLus;
            set
            {
                _FileIngreep.MetingPerLus = value;
                OnPropertyChanged(nameof(MetingPerLus), broadcast: true);
            }
        }

        public bool MetingPerLusAvailable => _FileIngreep.FileDetectoren.Count > 1;

        public bool MetingPerStrookAvailable
        {
            get
            {
                var detectorDict = new Dictionary<int, List<string>>();
                foreach (var fmd in FileDetectoren)
                {
                    var d = TLCGenModelManager.Default.Controller.Fasen.SelectMany(x => x.Detectoren).FirstOrDefault(x => x.Naam == fmd.Detector) ??
                            TLCGenModelManager.Default.Controller.Detectoren.FirstOrDefault(x => x.Naam == fmd.Detector);
                    if (d?.Rijstrook == null) continue;
                    if (!detectorDict.ContainsKey(d.Rijstrook.Value))
                    {
                        detectorDict.Add(d.Rijstrook.Value, new List<string> { fmd.Detector });
                    }
                    else
                    {
                        detectorDict[d.Rijstrook.Value].Add(fmd.Detector);
                    }
                }
                return detectorDict.Count > 1;
            } 
        }

        public bool MetingPerStrook
        {
            get => _FileIngreep.MetingPerStrook;
            set
            {
                _FileIngreep.MetingPerStrook = value;
                OnPropertyChanged(nameof(MetingPerStrook), broadcast: true);
            }
        }

        public int AfvalVertraging
        {
            get => _FileIngreep.AfvalVertraging;
            set
            {
                _FileIngreep.AfvalVertraging = value;
                OnPropertyChanged(nameof(AfvalVertraging), broadcast: true);
            }
        }

        public bool EerlijkDoseren
        {
            get => _FileIngreep.EerlijkDoseren;
            set
            {
                _FileIngreep.EerlijkDoseren = value;
                if(value)
                {
                    if(TeDoserenSignaalGroepen.Count > 1)
                    {
                        var dos = TeDoserenSignaalGroepen[0].DoseerPercentage;
                        foreach(var tdsg in TeDoserenSignaalGroepen)
                        {
                            tdsg.DoseerPercentageNoMessaging = dos;
                        }
                    }
                }
                OnPropertyChanged(nameof(EerlijkDoseren), broadcast: true);
            }
        }

        public NooitAltijdAanUitEnum ToepassenDoseren
        {
            get => _FileIngreep.ToepassenDoseren;
            set
            {
                _FileIngreep.ToepassenDoseren = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasToepassenDoseren));
            }
        }

        public bool HasToepassenDoseren => ToepassenDoseren != NooitAltijdAanUitEnum.Nooit;

        public NooitAltijdAanUitEnum ToepassenAlternatieveGroentijdenSet
        {
            get => _FileIngreep.ToepassenAlternatieveGroentijdenSet;
            set
            {
                _FileIngreep.ToepassenAlternatieveGroentijdenSet = value;
                if(value != NooitAltijdAanUitEnum.Nooit && string.IsNullOrWhiteSpace(_FileIngreep.AlternatieveGroentijdenSet))
                {
                    _FileIngreep.AlternatieveGroentijdenSet = "NG";
                }
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasToepassenAlternatieveGroentijdenSet));
            }
        }

        public bool HasToepassenAlternatieveGroentijdenSet => ToepassenAlternatieveGroentijdenSet != NooitAltijdAanUitEnum.Nooit;

        public string AlternatieveGroentijdenSet
        {
            get => _FileIngreep.AlternatieveGroentijdenSet;
            set
            {
                _FileIngreep.AlternatieveGroentijdenSet = value ?? "NG";
                OnPropertyChanged(broadcast: true);
            }
        }

        public FileMetingLocatieEnum FileMetingLocatie
        {
            get => _FileIngreep.FileMetingLocatie;
            set
            {
                _FileIngreep.FileMetingLocatie = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int MinimaalAantalMeldingenMax => _FileIngreep.FileDetectoren.Count;

        private ItemsManagerViewModel<FileIngreepDetectorViewModel, string> _detectorManager;
        public ItemsManagerViewModel<FileIngreepDetectorViewModel, string> DetectorManager
        {
            get
            {
                if (_detectorManager != null) return _detectorManager;
                _detectorManager = new ItemsManagerViewModel<FileIngreepDetectorViewModel, string>(
                    FileDetectoren,
                    ControllerAccessProvider.Default.AllDetectors.Where(x => x.Type == DetectorTypeEnum.File).Select(x => x.Naam),
                    x => 
                    {
                        var fd = new FileIngreepDetectorModel { Detector = x };
                        DefaultsProvider.Default.SetDefaultsOnModel(fd);
                        var fdvm = new FileIngreepDetectorViewModel(fd); return fdvm;
                    },
                    x => FileDetectoren.All(y => y.Detector != x),
                    null,
                    () => OnPropertyChanged(nameof(SelectedFileDetector), broadcast: true),
                    () => OnPropertyChanged(nameof(SelectedFileDetector), broadcast: true));
                return _detectorManager;
            }
        }

        #endregion // Properties

        #region Commands
        
        public ICommand AddTeDoserenSignaalGroepCommand => _AddTeDoserenSignaalGroepCommand ??= new RelayCommand(() =>
            {
                var dos = new FileIngreepTeDoserenSignaalGroepModel();
                DefaultsProvider.Default.SetDefaultsOnModel(dos);
                dos.FaseCyclus = SelectedFaseNaam;
                TeDoserenSignaalGroepen.Add(new FileIngreepTeDoserenSignaalGroepViewModel(dos));
                UpdateSelectables();
            },
            () => !string.IsNullOrWhiteSpace(SelectedFaseNaam));

        public ICommand RemoveTeDoserenSignaalGroepCommand => _RemoveTeDoserenSignaalGroepCommand ??= new RelayCommand(() =>
            {
                TeDoserenSignaalGroepen.Remove(SelectedTeDoserenFase);
                SelectedTeDoserenFase = null;
                UpdateSelectables();
            },
            () => SelectedTeDoserenFase != null);

        #endregion // Commands

        #region Private methods

        private void UpdateSelectables()
        {
            SelectableFasen.Clear();
            foreach (var s in _ControllerFasen)
            {
                if (TeDoserenSignaalGroepen.All(x => x.FaseCyclus != s))
                {
                    SelectableFasen.Add(s);
                }
            }
        }

        #endregion // Private methods

        #region Public methods

        public void OnSelected(List<string> controllerfasen)
        {
            _ControllerFasen = controllerfasen;
            UpdateSelectables();
            OnPropertyChanged("MetingPerStrookAvailable");
        }

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FileIngreep;
        }

        #endregion // IViewModelWithItem

        #region TLCGen Messenging

        public void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            if (message.RemovedDetectoren?.Any(x => x.Type == DetectorTypeEnum.File) == true ||
                message.AddedDetectoren?.Any(x => x.Type == DetectorTypeEnum.File) == true)
            {
                _detectorManager?.Refresh();
            }
        }

        public void OnNameChanged(object sender, NameChangedMessage message)
        {
            if (message.ObjectType != TLCGenObjectTypeEnum.Detector) return;
            _detectorManager?.Refresh();
        }

        public void OnFaseDetectorTypeChanged(object sender, FaseDetectorTypeChangedMessage message)
        {
            if (message.OldType == DetectorTypeEnum.File || message.NewType == DetectorTypeEnum.File)
            {
                _detectorManager?.Refresh();
            }
            if (message.NewType != DetectorTypeEnum.File)
            {
                var d = FileDetectoren.FirstOrDefault(x => x.Detector == message.DetectorDefine);
                if (d != null)
                {
                    FileDetectoren.Remove(d);
                }
            }
        }

        #endregion // TLCGen MEssenging

        #region Constructor

        public FileIngreepViewModel(FileIngreepModel fileingreep)
        {
            _FileIngreep = fileingreep;

            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChanged);

            FileDetectoren = new ObservableCollectionAroundList<FileIngreepDetectorViewModel, FileIngreepDetectorModel>(_FileIngreep.FileDetectoren);
            FileDetectoren.CollectionChanged += (o, e) =>
            {
                OnPropertyChanged("MetingPerLusAvailable");
            };
            TeDoserenSignaalGroepen = new ObservableCollectionAroundList<FileIngreepTeDoserenSignaalGroepViewModel, FileIngreepTeDoserenSignaalGroepModel>(_FileIngreep.TeDoserenSignaalGroepen);
        }

        #endregion // Constructor
    }
}
