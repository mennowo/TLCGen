using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
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
                _FileIngreep.Naam = value;
                RaisePropertyChanged<object>("Naam", broadcast: true);
            }
        }

        public int MinimaalAantalMeldingen
        {
            get { return _FileIngreep.MinimaalAantalMeldingen; }
            set
            {
                _FileIngreep.MinimaalAantalMeldingen = value;
                RaisePropertyChanged<object>("MinimaalAantalMeldingen", broadcast: true);
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
            foreach (string s in _ControllerFasen)
            {
                if (!TeDoserenSignaalGroepen.Where(x => x.FaseCyclus == s).Any())
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
        }

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FileIngreep;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public FileIngreepViewModel(FileIngreepModel fileingreep)
        {
            _FileIngreep = fileingreep;

            FileDetectoren = new ObservableCollectionAroundList<FileIngreepDetectorViewModel, FileIngreepDetectorModel>(_FileIngreep.FileDetectoren);
            TeDoserenSignaalGroepen = new ObservableCollectionAroundList<FileIngreepTeDoserenSignaalGroepViewModel, FileIngreepTeDoserenSignaalGroepModel>(_FileIngreep.TeDoserenSignaalGroepen);
        }

        #endregion // Constructor
    }
}
