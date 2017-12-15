using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class FileIngreepViewModel : ViewModelBase, IViewModelWithItem
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
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                RaisePropertyChanged("SelectedFaseNaam");
            }
        }

        public FileIngreepTeDoserenSignaalGroepViewModel SelectedTeDoserenFase
        {
            get { return _SelectedTeDoserenFase; }
            set
            {
                _SelectedTeDoserenFase = value;
                RaisePropertyChanged("SelectedTeDoserenFase");
            }
        }

        public FileIngreepDetectorViewModel SelectedFileDetector
        {
            get { return DetectorManager.SelectedDetector; }
            set
            {
                DetectorManager.SelectedDetector = value;
                RaisePropertyChanged("SelectedFileDetector");
            }
        }

        public string Naam
        {
            get { return _FileIngreep.Naam; }
            set
            {
	            if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidName(value))
	            {
		            var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
		            Messenger.Default.Send(message);
		            if (message.Handled && message.IsUnique)
		            {
                        string oldname = _FileIngreep.Naam;
			            _FileIngreep.Naam = value;

						// Notify the messenger
						Messenger.Default.Send(new NameChangedMessage(oldname, value));
						RaisePropertyChanged<object>(nameof(Naam), broadcast: true);
		            }
				}
            }
        }

        public bool MetingPerLus
        {
            get => _FileIngreep.MetingPerLus;
            set
            {
                _FileIngreep.MetingPerLus = value;
                RaisePropertyChanged<object>("MetingPerLus", broadcast: true);
            }
        }

        public bool MetingPerLusAvailable
        {
            get => _FileIngreep.FileDetectoren.Count > 1;
        }

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
                RaisePropertyChanged<object>("MetingPerStrook", broadcast: true);
            }
        }

        public int AfvalVertraging
        {
            get { return _FileIngreep.AfvalVertraging; }
            set
            {
                _FileIngreep.AfvalVertraging = value;
                RaisePropertyChanged<object>("AfvalVertraging", broadcast: true);
            }
        }

        public bool EerlijkDoseren
        {
            get { return _FileIngreep.EerlijkDoseren; }
            set
            {
                _FileIngreep.EerlijkDoseren = value;
                if(value)
                {
                    if(TeDoserenSignaalGroepen.Count > 1)
                    {
                        int dos = TeDoserenSignaalGroepen[0].DoseerPercentage;
                        foreach(var tdsg in TeDoserenSignaalGroepen)
                        {
                            tdsg.DoseerPercentageNoMessaging = dos;
                        }
                    }
                }
                RaisePropertyChanged<object>("EerlijkDoseren", broadcast: true);
            }
        }

        public int MinimaalAantalMeldingenMax
        {
            get { return _FileIngreep.FileDetectoren.Count; }
        }

        private DetectorManagerViewModel<FileIngreepDetectorViewModel, string> _DetectorManager;
        public DetectorManagerViewModel<FileIngreepDetectorViewModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null)
                {
                    var dets1 =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen
                            .SelectMany(x => x.Detectoren)
                            .Where(x => x.Type == DetectorTypeEnum.File)
                            .Select(x => x.Naam);
                    var dets2 =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren
                            .Where(x => x.Type == DetectorTypeEnum.File)
                            .Select(x => x.Naam);
                    var dets = dets1.Concat(dets2).ToList();
                    _DetectorManager = new DetectorManagerViewModel<FileIngreepDetectorViewModel, string>(
                        FileDetectoren as ObservableCollection<FileIngreepDetectorViewModel>,
                        dets,
                        (x) => { var fd = new FileIngreepDetectorViewModel(new FileIngreepDetectorModel { Detector = x }); return fd; },
                        (x) => { return !FileDetectoren.Where(y => y.Detector == x).Any(); },
                        null,
                        () => { RaisePropertyChanged<object>("SelectedFileDetector", broadcast: true); },
                        () => { RaisePropertyChanged<object>("SelectedFileDetector", broadcast: true); }
                        );
                }
                return _DetectorManager;
            }
        }

        #endregion // Properties

        #region Commands
        
        public ICommand AddTeDoserenSignaalGroepCommand
        {
            get
            {
                if (_AddTeDoserenSignaalGroepCommand == null)
                {
                    _AddTeDoserenSignaalGroepCommand = new RelayCommand(AddNewTeDoserenSignaalGroepCommand_Executed, AddNewTeDoserenSignaalGroepCommand_CanExecute);
                }
                return _AddTeDoserenSignaalGroepCommand;
            }
        }

        public ICommand RemoveTeDoserenSignaalGroepCommand
        {
            get
            {
                if (_RemoveTeDoserenSignaalGroepCommand == null)
                {
                    _RemoveTeDoserenSignaalGroepCommand = new RelayCommand(RemoveTeDoserenSignaalGroepCommand_Executed, RemoveTeDoserenSignaalGroepCommand_CanExecute);
                }
                return _RemoveTeDoserenSignaalGroepCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality
        
        void AddNewTeDoserenSignaalGroepCommand_Executed(object prm)
        {
            FileIngreepTeDoserenSignaalGroepModel dos = new FileIngreepTeDoserenSignaalGroepModel();
            DefaultsProvider.Default.SetDefaultsOnModel(dos);
            dos.FaseCyclus = SelectedFaseNaam;
            TeDoserenSignaalGroepen.Add(new FileIngreepTeDoserenSignaalGroepViewModel(dos));
            UpdateSelectables();
        }

        bool AddNewTeDoserenSignaalGroepCommand_CanExecute(object prm)
        {
            return !string.IsNullOrWhiteSpace(SelectedFaseNaam);
        }

        void RemoveTeDoserenSignaalGroepCommand_Executed(object prm)
        {
            TeDoserenSignaalGroepen.Remove(SelectedTeDoserenFase);
            SelectedTeDoserenFase = null;
            UpdateSelectables();
        }

        bool RemoveTeDoserenSignaalGroepCommand_CanExecute(object prm)
        {
            return SelectedTeDoserenFase != null;
        }

        #endregion // Command Functionality

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
            RaisePropertyChanged("MetingPerStrookAvailable");
        }

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FileIngreep;
        }

        #endregion // IViewModelWithItem

        #region TLCGen Messenging

        public void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            _DetectorManager = null;
        }

        public void OnFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            _DetectorManager = null;
            if (message.Type != DetectorTypeEnum.File)
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

            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChanged);

            FileDetectoren = new ObservableCollectionAroundList<FileIngreepDetectorViewModel, FileIngreepDetectorModel>(_FileIngreep.FileDetectoren);
            FileDetectoren.CollectionChanged += (o, e) =>
            {
                RaisePropertyChanged("MetingPerLusAvailable");
            };
            TeDoserenSignaalGroepen = new ObservableCollectionAroundList<FileIngreepTeDoserenSignaalGroepViewModel, FileIngreepTeDoserenSignaalGroepModel>(_FileIngreep.TeDoserenSignaalGroepen);
        }

        #endregion // Constructor
    }
}
