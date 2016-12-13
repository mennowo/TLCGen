using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private FileIngreepModel _FileIngreep;

        private string _SelectedFaseNaam;
        private string _SelectedDetectorNaam;

        private List<string> _ControllerFasen;
        private List<string> _ControllerDetectoren;
        private ObservableCollection<string> _SelectableFasen;
        private ObservableCollection<string> _SelectableDetectoren;

        private FileIngreepTeDoserenSignaalGroepViewModel _SelectedTeDoserenFase;
        private FileIngreepDetectorViewModel _SelectedFileDetector;

        private RelayCommand _AddFileDetectorCommand;
        private RelayCommand _RemoveFileDetectorCommand;
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

        public ObservableCollection<string> SelectableDetectoren
        {
            get
            {
                if (_SelectableDetectoren == null)
                {
                    _SelectableDetectoren = new ObservableCollection<string>();
                }
                return _SelectableDetectoren;
            }
        }

        public string SelectedFaseNaam
        {
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                OnPropertyChanged("SelectedFaseNaam");
            }
        }

        public string SelectedDetectorNaam
        {
            get { return _SelectedDetectorNaam; }
            set
            {
                _SelectedDetectorNaam = value;
                OnPropertyChanged("SelectedDetectorNaam");
            }
        }

        public FileIngreepTeDoserenSignaalGroepViewModel SelectedTeDoserenFase
        {
            get { return _SelectedTeDoserenFase; }
            set
            {
                _SelectedTeDoserenFase = value;
                OnMonitoredPropertyChanged("SelectedTeDoserenFase");
            }
        }

        public FileIngreepDetectorViewModel SelectedFileDetector
        {
            get { return _SelectedFileDetector; }
            set
            {
                _SelectedFileDetector = value;
                OnMonitoredPropertyChanged("SelectedFileDetector");
            }
        }

        public string Naam
        {
            get { return _FileIngreep.Naam; }
            set
            {
                _FileIngreep.Naam = value;
                OnMonitoredPropertyChanged("Naam");
            }
        }

        public int MinimaalAantalMeldingen
        {
            get { return _FileIngreep.MinimaalAantalMeldingen; }
            set
            {
                _FileIngreep.MinimaalAantalMeldingen = value;
                OnMonitoredPropertyChanged("MinimaalAantalMeldingen");
            }
        }

        public int MinimaalAantalMeldingenMax
        {
            get { return _FileIngreep.FileDetectoren.Count; }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddFileDetectorCommand
        {
            get
            {
                if (_AddFileDetectorCommand == null)
                {
                    _AddFileDetectorCommand = new RelayCommand(AddNewFileDetectorCommand_Executed, AddNewFileDetectorCommand_CanExecute);
                }
                return _AddFileDetectorCommand;
            }
        }

        public ICommand RemoveFileDetectorCommand
        {
            get
            {
                if (_RemoveFileDetectorCommand == null)
                {
                    _RemoveFileDetectorCommand = new RelayCommand(RemoveFileDetectorCommand_Executed, RemoveFileDetectorCommand_CanExecute);
                }
                return _RemoveFileDetectorCommand;
            }
        }

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

        void AddNewFileDetectorCommand_Executed(object prm)
        {
            FileIngreepDetectorModel fidm = new FileIngreepDetectorModel();
            fidm.Detector = SelectedDetectorNaam;
            FileDetectoren.Add(new FileIngreepDetectorViewModel(fidm));
            OnMonitoredPropertyChanged("MinimaalAantalMeldingenMax");
            UpdateSelectables();
        }

        bool AddNewFileDetectorCommand_CanExecute(object prm)
        {
            return !string.IsNullOrWhiteSpace(SelectedDetectorNaam);
        }

        void RemoveFileDetectorCommand_Executed(object prm)
        {
            FileDetectoren.Remove(SelectedFileDetector);
            SelectedFileDetector = null;
            UpdateSelectables();
        }

        bool RemoveFileDetectorCommand_CanExecute(object prm)
        {
            return SelectedFileDetector != null;
        }


        void AddNewTeDoserenSignaalGroepCommand_Executed(object prm)
        {
            FileIngreepTeDoserenSignaalGroepModel dos = new FileIngreepTeDoserenSignaalGroepModel();
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
            SelectableDetectoren.Clear();
            foreach (string s in _ControllerDetectoren)
            {
                if (!FileDetectoren.Where(x => x.Detector == s).Any())
                {
                    SelectableDetectoren.Add(s);
                }
            }
        }

        #endregion // Private methods

        #region Public methods

        public void OnSelected(List<string> controllerfasen, List<string> controllerdetectoren)
        {
            _ControllerFasen = controllerfasen;
            _ControllerDetectoren = controllerdetectoren;
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
