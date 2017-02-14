using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class RoBuGroverSignaalGroepInstellingenViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private RoBuGroverFaseCyclusInstellingenModel _SignaalGroepInstellingen;

        private RoBuGroverFileDetectorViewModel _SelectedFileDetector;
        private RoBuGroverHiaatDetectorViewModel _SelectedHiaatDetector;
        private string _SelectedFileDetectorToAdd;
        private string _SelectedHiaatDetectorToAdd;
        private List<string> _FaseFileDetectoren;
        private List<string> _FaseHiaatDetectoren;
        private ObservableCollection<string> _SelectableFileDetectoren;
        private ObservableCollection<string> _SelectableHiaatDetectoren;

        #endregion // Fields

        #region Properties

        public RoBuGroverFileDetectorViewModel SelectedFileDetector
        {
            get { return _SelectedFileDetector; }
            set
            {
                _SelectedFileDetector = value;
                OnMonitoredPropertyChanged("SelectedFileDetector");
            }
        }

        public RoBuGroverHiaatDetectorViewModel SelectedHiaatDetector
        {
            get { return _SelectedHiaatDetector; }
            set
            {
                _SelectedHiaatDetector = value;
                OnMonitoredPropertyChanged("SelectedHiaatDetector");
            }
        }

        public string SelectedFileDetectorToAdd
        {
            get { return _SelectedFileDetectorToAdd; }
            set
            {
                _SelectedFileDetectorToAdd = value;
                OnMonitoredPropertyChanged("SelectedFileDetectorToAdd");
            }
        }

        public string SelectedHiaatDetectorToAdd
        {
            get { return _SelectedHiaatDetectorToAdd; }
            set
            {
                _SelectedHiaatDetectorToAdd = value;
                OnMonitoredPropertyChanged("SelectedHiaatDetectorToAdd");
            }
        }

        public ObservableCollection<string> SelectableFileDetectoren
        {
            get
            {
                if (_SelectableFileDetectoren == null)
                {
                    _SelectableFileDetectoren = new ObservableCollection<string>();
                }
                return _SelectableFileDetectoren;
            }
        }

        public ObservableCollection<string> SelectableHiaatDetectoren
        {
            get
            {
                if (_SelectableHiaatDetectoren == null)
                {
                    _SelectableHiaatDetectoren = new ObservableCollection<string>();
                }
                return _SelectableHiaatDetectoren;
            }
        }

        public string FaseCyclus
        {
            get { return _SignaalGroepInstellingen.FaseCyclus; }
            set
            {
                _SignaalGroepInstellingen.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }
        public int MinGroenTijd
        {
            get { return _SignaalGroepInstellingen.MinGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MinGroenTijd = value;
                OnMonitoredPropertyChanged("MinGroenTijd");
            }
        }
        public int MaxGroenTijd
        {
            get { return _SignaalGroepInstellingen.MaxGroenTijd; }
            set
            {
                _SignaalGroepInstellingen.MaxGroenTijd = value;
                OnMonitoredPropertyChanged("MaxGroenTijd");
            }
        }

        public ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel> FileDetectoren
        {
            get;
            private set;
        }

        public ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel> HiaatDetectoren
        {
            get;
            private set;
        }

        #endregion Properties

        #region Commands

        RelayCommand _AddFileDetectorCommand;
        public ICommand AddFileDetectorCommand
        {
            get
            {
                if (_AddFileDetectorCommand == null)
                {
                    _AddFileDetectorCommand = new RelayCommand(AddFileDetectorCommand_Executed, AddFileDetectorCommand_CanExecute);
                }
                return _AddFileDetectorCommand;
            }
        }

        RelayCommand _RemoveFileDetectorCommand;
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

        RelayCommand _AddHiaatDetectorCommand;
        public ICommand AddHiaatDetectorCommand
        {
            get
            {
                if (_AddHiaatDetectorCommand == null)
                {
                    _AddHiaatDetectorCommand = new RelayCommand(AddHiaatDetectorCommand_Executed, AddHiaatDetectorCommand_CanExecute);
                }
                return _AddHiaatDetectorCommand;
            }
        }

        RelayCommand _RemoveHiaatDetectorCommand;
        public ICommand RemoveHiaatDetectorCommand
        {
            get
            {
                if (_RemoveHiaatDetectorCommand == null)
                {
                    _RemoveHiaatDetectorCommand = new RelayCommand(RemoveHiaatDetectorCommand_Executed, RemoveHiaatDetectorCommand_CanExecute);
                }
                return _RemoveHiaatDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddFileDetectorCommand_CanExecute(object obj)
        {
            return true;
        }

        private void AddFileDetectorCommand_Executed(object obj)
        {
            RoBuGroverFileDetectorModel d = new RoBuGroverFileDetectorModel();
            d.Detector = SelectedFileDetectorToAdd;
            DefaultsProvider.Default.SetDefaultsOnModel(d);
            FileDetectoren.Add(new RoBuGroverFileDetectorViewModel(d));
            UpdateSelectables();
            if (SelectableFileDetectoren.Count > 0)
                SelectedFileDetectorToAdd = SelectableFileDetectoren[0];
        }

        private bool RemoveFileDetectorCommand_CanExecute(object obj)
        {
            return SelectedFileDetector != null;
        }

        private void RemoveFileDetectorCommand_Executed(object obj)
        {
            FileDetectoren.Remove(SelectedFileDetector);
            SelectedFileDetector = null;
            UpdateSelectables();
            if (SelectableFileDetectoren.Count > 0)
                SelectedFileDetectorToAdd = SelectableFileDetectoren[0];
        }

        private bool AddHiaatDetectorCommand_CanExecute(object obj)
        {
            return true;
        }

        private void AddHiaatDetectorCommand_Executed(object obj)
        {
            RoBuGroverHiaatDetectorModel d = new RoBuGroverHiaatDetectorModel();
            d.Detector = SelectedHiaatDetectorToAdd;
            DefaultsProvider.Default.SetDefaultsOnModel(d);
            HiaatDetectoren.Add(new RoBuGroverHiaatDetectorViewModel(d));
            UpdateSelectables();
        }

        private bool RemoveHiaatDetectorCommand_CanExecute(object obj)
        {
            return SelectedHiaatDetector != null;
        }

        private void RemoveHiaatDetectorCommand_Executed(object obj)
        {
            HiaatDetectoren.Remove(SelectedHiaatDetector);
            SelectedHiaatDetector = null;
            UpdateSelectables();
        }

        #endregion // Command functionality

        #region Private methods

        private void UpdateSelectables()
        {
            SelectableFileDetectoren.Clear();
            foreach (string s in _FaseFileDetectoren)
            {
                if (!FileDetectoren.Where(x => x.Detector == s).Any())
                {
                    SelectableFileDetectoren.Add(s);
                }
            }
            if (SelectableFileDetectoren.Count > 0)
                SelectedFileDetectorToAdd = SelectableFileDetectoren[0];
            SelectableHiaatDetectoren.Clear();
            foreach (string s in _FaseHiaatDetectoren)
            {
                if (!HiaatDetectoren.Where(x => x.Detector == s).Any())
                {
                    SelectableHiaatDetectoren.Add(s);
                }
            }
            if (SelectableHiaatDetectoren.Count > 0)
                SelectedHiaatDetectorToAdd = SelectableHiaatDetectoren[0];
        }

        #endregion // Private methods

        #region Public methods

        public void OnSelected(List<string> fasefildetectoren, List<string> fasehiaatdetectoren)
        {
            _FaseFileDetectoren = fasefildetectoren;
            _FaseHiaatDetectoren = fasehiaatdetectoren;
            UpdateSelectables();
        }

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _SignaalGroepInstellingen;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is RoBuGroverSignaalGroepInstellingenViewModel)
            {
                return this.FaseCyclus.CompareTo(((RoBuGroverSignaalGroepInstellingenViewModel)obj).FaseCyclus);
            }
            return 0;
        }
        
        #endregion // IComparable

        #region Constructor

        public RoBuGroverSignaalGroepInstellingenViewModel(RoBuGroverFaseCyclusInstellingenModel signaalgroepinstellingen)
        {
            _SignaalGroepInstellingen = signaalgroepinstellingen;

            FileDetectoren = new ObservableCollectionAroundList<RoBuGroverFileDetectorViewModel, RoBuGroverFileDetectorModel>(_SignaalGroepInstellingen.FileDetectoren);
            HiaatDetectoren = new ObservableCollectionAroundList<RoBuGroverHiaatDetectorViewModel, RoBuGroverHiaatDetectorModel>(_SignaalGroepInstellingen.HiaatDetectoren);
        }

        #endregion // Constructor

    }
}
