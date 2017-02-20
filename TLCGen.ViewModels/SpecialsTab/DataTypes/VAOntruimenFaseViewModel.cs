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
    public class VAOntruimenFaseViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenFaseModel _VAOntruimenFase;
        private ObservableCollection<string> _FaseDetectoren;
        private Dictionary<string, int> _ConflicterendeFasen;
        private ObservableCollection<string> _SelectableDetectoren;
        private ObservableCollection<string> _RemovableDetectoren;

        private ObservableCollection<string> _VAOntruimenMatrixColumnHeaders;
        private ObservableCollection<string> _VAOntruimenMatrixRowHeaders;

        private string _SelectedDetectorToAdd;
        private string _SelectedDetectorToRemove;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _VAOntruimenFase.FaseCyclus; }
            set
            {
                _VAOntruimenFase.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }
        public int VAOntrTijdensRood
        {
            get { return _VAOntruimenFase.VAOntrTijdensRood; }
            set
            {
                _VAOntruimenFase.VAOntrTijdensRood = value;
                OnMonitoredPropertyChanged("VAOntrTijdensRood");
            }
        }

        public ObservableCollectionAroundList<VAOntruimenDetectorViewModel, VAOntruimenDetectorModel> VAOntruimenDetectoren
        {
            get;
            private set;
        }


        public ObservableCollection<string> FaseDetectoren
        {
            get
            {
                if (_FaseDetectoren == null)
                {
                    _FaseDetectoren = new ObservableCollection<string>();
                }
                return _FaseDetectoren;
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

        public string SelectedDetectorToAdd
        {
            get { return _SelectedDetectorToAdd; }
            set
            {
                _SelectedDetectorToAdd = value;
                OnPropertyChanged("SelectedDetectorToAdd");
            }
        }

        public ObservableCollection<string> RemovableDetectoren
        {
            get
            {
                if (_RemovableDetectoren == null)
                {
                    _RemovableDetectoren = new ObservableCollection<string>();
                }
                return _RemovableDetectoren;
            }
        }

        public string SelectedDetectorToRemove
        {
            get { return _SelectedDetectorToRemove; }
            set
            {
                _SelectedDetectorToRemove = value;
                OnPropertyChanged("SelectedDetectorToRemove");
            }
        }

        public Dictionary<string, int> ConflicterendeFasen
        {
            get
            {
                if (_ConflicterendeFasen == null)
                {
                    _ConflicterendeFasen = new Dictionary<string, int>();
                }
                return _ConflicterendeFasen;
            }
        }
        
        public VAOntruimenNaarFaseViewModel[,] VAOntruimenMatrix
        {
            get;
            set;
        }

        public ObservableCollection<string> VAOntruimenMatrixColumnHeaders
        {
            get
            {
                if (_VAOntruimenMatrixColumnHeaders == null)
                {
                    _VAOntruimenMatrixColumnHeaders = new ObservableCollection<string>();
                }
                return _VAOntruimenMatrixColumnHeaders;
            }
        }

        public ObservableCollection<string> VAOntruimenMatrixRowHeaders
        {
            get
            {
                if (_VAOntruimenMatrixRowHeaders == null)
                {
                    _VAOntruimenMatrixRowHeaders = new ObservableCollection<string>();
                }
                return _VAOntruimenMatrixRowHeaders;
            }
        }

        #endregion Properties

        #region Commands

        private RelayCommand _AddDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_AddDetectorCommand == null)
                {
                    _AddDetectorCommand = new RelayCommand(AddDetectorCommand_Executed, AddDetectorCommand_CanExecute);
                }
                return _AddDetectorCommand;
            }
        }

        private RelayCommand _RemoveDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_RemoveDetectorCommand == null)
                {
                    _RemoveDetectorCommand = new RelayCommand(RemoveDetectorCommand_Executed, RemoveDetectorCommand_CanExecute);
                }
                return _RemoveDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddDetectorCommand_CanExecute(object obj)
        {
            return !string.IsNullOrEmpty(SelectedDetectorToAdd);
        }

        private void AddDetectorCommand_Executed(object obj)
        {
            var vad = new VAOntruimenDetectorModel();
            vad.Detector = SelectedDetectorToAdd;
            foreach(var kv in ConflicterendeFasen)
            {
                vad.ConflicterendeFasen.Add(new VAOntruimenNaarFaseModel() { FaseCyclus = kv.Key });
            }
            VAOntruimenDetectoren.Add(new VAOntruimenDetectorViewModel(vad));

            SetSelectableDetectoren();
            RebuildVAOnruimenMatrix();
        }

        private bool RemoveDetectorCommand_CanExecute(object obj)
        {
            return !string.IsNullOrEmpty(SelectedDetectorToRemove);
        }

        private void RemoveDetectorCommand_Executed(object obj)
        {
            VAOntruimenDetectorViewModel dvm = null;
            foreach(var d in VAOntruimenDetectoren)
            {
                if(d.Detector == SelectedDetectorToRemove)
                {
                    dvm = d;
                    break;
                }
            }
            if(dvm != null)
            {
                VAOntruimenDetectoren.Remove(dvm);
            }

            Refresh();
        }

        #endregion // Command functionality

        #region Private methods



        private void SetSelectableDetectoren()
        {
            SelectableDetectoren.Clear();
            foreach (var s in FaseDetectoren)
            {
                if (!this.VAOntruimenDetectoren.Where(x => x.Detector == s).Any())
                {
                    SelectableDetectoren.Add(s);
                }
            }

            RemovableDetectoren.Clear();
            foreach (var d in VAOntruimenDetectoren)
            {
                RemovableDetectoren.Add(d.Detector);
            }
        }

        private void RebuildVAOnruimenMatrix()
        {
            if (ConflicterendeFasen.Count == 0 || VAOntruimenDetectoren.Count == 0)
            {
                return;
            }

            VAOntruimenMatrixColumnHeaders.Clear();
            VAOntruimenMatrixRowHeaders.Clear();
            VAOntruimenMatrix = new VAOntruimenNaarFaseViewModel[ConflicterendeFasen.Count, VAOntruimenDetectoren.Count];

            for (int d = 0; d < VAOntruimenDetectoren.Count; ++d)
            {
                VAOntruimenMatrixColumnHeaders.Add(VAOntruimenDetectoren[d].Detector);
                for (int cfc = 0; cfc < VAOntruimenDetectoren[d].ConflicterendeFasen.Count; ++cfc)
                {
                    VAOntruimenMatrixRowHeaders.Add(VAOntruimenDetectoren[d].ConflicterendeFasen[cfc].FaseCyclus);
                    VAOntruimenMatrix[cfc, d] = VAOntruimenDetectoren[d].ConflicterendeFasen[cfc];
                }
            }
            OnPropertyChanged("VAOntruimenMatrix");
        }

        #endregion // Private methods

        #region Public methods

        public void Refresh()
        {
            SetSelectableDetectoren();
            VAOntruimenDetectoren.Rebuild();
            RebuildVAOnruimenMatrix();
        }

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _VAOntruimenFase;
        }
        
        #endregion // IViewModelWithItem

        #region Constructor

        public VAOntruimenFaseViewModel(VAOntruimenFaseModel vaontruimenfase)
        {
            _VAOntruimenFase = vaontruimenfase;
            VAOntruimenDetectoren = new ObservableCollectionAroundList<VAOntruimenDetectorViewModel, VAOntruimenDetectorModel>(vaontruimenfase.VADetectoren);

            RebuildVAOnruimenMatrix();
        }

        #endregion // Constructor
    }
}
