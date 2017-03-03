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
        private Dictionary<string, int> _ConflicterendeFasen;

        private ObservableCollection<string> _VAOntruimenMatrixColumnHeaders;
        private ObservableCollection<string> _VAOntruimenMatrixRowHeaders;

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

        private DetectorManagerViewModel<VAOntruimenDetectorViewModel, string> _DetectorManager;
        public DetectorManagerViewModel<VAOntruimenDetectorViewModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.
                            Where(x => x.Naam == FaseCyclus).
                            First().
                            Detectoren.
                            Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new DetectorManagerViewModel<VAOntruimenDetectorViewModel, string>(
                        VAOntruimenDetectoren,
                        dets,
                        (x) => 
                        {
                            var vad = new VAOntruimenDetectorModel();
                            vad.Detector = x;
                            foreach(var kv in ConflicterendeFasen)
                            {
                                vad.ConflicterendeFasen.Add(new VAOntruimenNaarFaseModel() { FaseCyclus = kv.Key });
                            }
                            return new VAOntruimenDetectorViewModel(vad);
                        },
                        (x) => 
                        {
                            return !VAOntruimenDetectoren.Where(y => y.Detector == x).Any();
                        },
                        (x) =>
                        {
                            VAOntruimenDetectorViewModel dvm = null;
                            foreach(var d in VAOntruimenDetectoren)
                            {
                                if(d.Detector == x)
                                {
                                    dvm = d;
                                    break;
                                }
                            }
                            return dvm;
                        },
                        () => { Refresh(); },
                        () => { Refresh(); }
                        );
                }
                return _DetectorManager;
            }
            private set { _DetectorManager = value; }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        private void RebuildVAOnruimenMatrix()
        {
            VAOntruimenMatrixColumnHeaders.Clear();
            VAOntruimenMatrixRowHeaders.Clear();

            if (ConflicterendeFasen.Count == 0 || VAOntruimenDetectoren.Count == 0)
            {
                VAOntruimenMatrix = null;
                OnPropertyChanged("VAOntruimenMatrix");
                return;
            }

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
            VAOntruimenDetectoren.Rebuild();
            RebuildVAOnruimenMatrix();
            DetectorManager = null;
            OnPropertyChanged("DetectorManager");
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

            Refresh();
        }

        #endregion // Constructor
    }
}
