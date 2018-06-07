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
                RaisePropertyChanged();
            }
        }

        public int VAOntrMax
        {
            get { return _VAOntruimenFase.VAOntrMax; }
            set
            {
                _VAOntruimenFase.VAOntrMax = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool KijkNaarWisselstand
        {
            get => _VAOntruimenFase.KijkNaarWisselstand;
            set
            {
                _VAOntruimenFase.KijkNaarWisselstand = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel1));
                RaisePropertyChanged(nameof(HasWissel2));
                RaisePropertyChanged(nameof(HasWissel1Voorwaarde));
                RaisePropertyChanged(nameof(HasWissel2Voorwaarde));
                RaisePropertyChanged(nameof(IsWissel1Detector));
                RaisePropertyChanged(nameof(IsWissel1Ingang));
            }
        }

        public OVIngreepInUitDataWisselTypeEnum Wissel1Type
        {
            get => _VAOntruimenFase.Wissel1Type;
            set
            {
                _VAOntruimenFase.Wissel1Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel1Voorwaarde));
                RaisePropertyChanged(nameof(IsWissel1Detector));
                RaisePropertyChanged(nameof(IsWissel1Ingang));
            }
        }

        public string Wissel1Input
        {
            get => _VAOntruimenFase.Wissel1Input;
            set
            {
                if(value != null)
                {
                    _VAOntruimenFase.Wissel1Input = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public string Wissel1Detector
        {
            get => _VAOntruimenFase.Wissel1Detector;
            set
            {
                if (value != null)
                {
                    _VAOntruimenFase.Wissel1Detector = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public bool Wissel1Voorwaarde
        {
            get => _VAOntruimenFase.Wissel1InputVoorwaarde;
            set
            {
                _VAOntruimenFase.Wissel1InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool Wissel2
        {
            get => _VAOntruimenFase.Wissel2;
            set
            {
                _VAOntruimenFase.Wissel2 = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel2));
                RaisePropertyChanged(nameof(IsWissel2Detector));
                RaisePropertyChanged(nameof(IsWissel2Ingang));
                RaisePropertyChanged(nameof(HasWissel2Voorwaarde));
            }
        }

        public OVIngreepInUitDataWisselTypeEnum Wissel2Type
        {
            get => _VAOntruimenFase.Wissel2Type;
            set
            {
                _VAOntruimenFase.Wissel2Type = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasWissel2Voorwaarde));
                RaisePropertyChanged(nameof(IsWissel2Detector));
                RaisePropertyChanged(nameof(IsWissel2Ingang));
            }
        }

        public string Wissel2Input
        {
            get => _VAOntruimenFase.Wissel2Input;
            set
            {
                if (value != null)
                {
                    _VAOntruimenFase.Wissel2Input = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public string Wissel2Detector
        {
            get => _VAOntruimenFase.Wissel2Detector;
            set
            {
                if (value != null)
                {
                    _VAOntruimenFase.Wissel2Detector = value;
                    RaisePropertyChanged<object>(broadcast: true);
                }
                else
                {
                    RaisePropertyChanged();
                }
            }
        }

        public bool Wissel2Voorwaarde
        {
            get => _VAOntruimenFase.Wissel2InputVoorwaarde;
            set
            {
                _VAOntruimenFase.Wissel2InputVoorwaarde = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public bool HasWissel1 => KijkNaarWisselstand;
        public bool IsWissel1Ingang => KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang;
        public bool IsWissel1Detector => KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector;
        public bool HasWissel1Voorwaarde => KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang;
        public bool HasWissel2 => KijkNaarWisselstand && Wissel2;
        public bool IsWissel2Ingang => HasWissel2 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang;
        public bool IsWissel2Detector => HasWissel2 && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector;
        public bool HasWissel2Voorwaarde => HasWissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang;

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

        private ItemsManagerViewModel<VAOntruimenDetectorViewModel, string> _DetectorManager;

        public ItemsManagerViewModel<VAOntruimenDetectorViewModel, string> DetectorManager
        {
            get
            {
                if (_DetectorManager == null)
                {
                    List<string> dets =
                        DataAccess.TLCGenControllerDataProvider.Default.Controller.
                            GetAllDetectors().
                            Select(x => x.Naam).
                            ToList();
                    _DetectorManager = new ItemsManagerViewModel<VAOntruimenDetectorViewModel, string>(
                        VAOntruimenDetectoren,
                        dets,
                        (x) =>
                        {
                            var vad = new VAOntruimenDetectorModel()
                            {
                                Detector = x
                            };
                            foreach (var kv in ConflicterendeFasen)
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
                            foreach (var d in VAOntruimenDetectoren)
                            {
                                if (d.Detector == x)
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

        public ObservableCollection<string> WisselDetectoren { get; }

        public ObservableCollection<string> WisselInputs { get; }
        
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
                RaisePropertyChanged("VAOntruimenMatrix");
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
            RaisePropertyChanged("VAOntruimenMatrix");
        }

        #endregion // Private methods

        #region Public methods

        public void Refresh()
        {
            VAOntruimenDetectoren.CollectionChanged -= VAOntruimenDetectoren_CollectionChanged;
            VAOntruimenDetectoren.Rebuild();
            VAOntruimenDetectoren.CollectionChanged += VAOntruimenDetectoren_CollectionChanged;
            RebuildVAOnruimenMatrix();
            DetectorManager = null;
            RaisePropertyChanged("DetectorManager");
        }

        #endregion // Public Methods

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            _DetectorManager = null;
            RaisePropertyChanged("DetectorManager");

            //var sd1 = "";
            //var sd2 = "";
            //if (KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector)
            //{
            //    sd1 = Wissel1Detector;
            //}
            //if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector)
            //{
            //    sd2 = Wissel2Detector;
            //}

            WisselDetectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselDetector)))
            {
                WisselDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselDetector))
            {
                WisselDetectoren.Add(d.Naam);
            }

            //if (KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd1))
            //{
            //    Wissel1Detector = sd1;
            //}
            //if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Detector && WisselDetectoren.Contains(sd2))
            //{
            //    Wissel2Detector = sd2;
            //}
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            //var sd1 = "";
            //var sd2 = "";
            //if (KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
            //{
            //    sd1 = Wissel1Input;
            //}
            //if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
            //{
            //    sd2 = Wissel2Input;
            //}

            WisselInputs.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            {
                WisselInputs.Add(seld.Naam);
            }

            //if (KijkNaarWisselstand && Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd1))
            //{
            //    Wissel1Input = sd1;
            //}
            //if (Wissel2 && Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang && WisselInputs.Contains(sd2))
            //{
            //    Wissel2Input = sd2;
            //}
        }

        #endregion // TLCGen Events

        #region IViewModelWithItem

        public object GetItem()
        {
            return _VAOntruimenFase;
        }

        #endregion // IViewModelWithItem

        #region Collection changed
        
        private void VAOntruimenDetectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection changed
        
        #region Constructor

        public VAOntruimenFaseViewModel(VAOntruimenFaseModel vaontruimenfase)
        {
            _VAOntruimenFase = vaontruimenfase;
            VAOntruimenDetectoren = new ObservableCollectionAroundList<VAOntruimenDetectorViewModel, VAOntruimenDetectorModel>(vaontruimenfase.VADetectoren);
            VAOntruimenDetectoren.CollectionChanged += VAOntruimenDetectoren_CollectionChanged;

            WisselDetectoren = new ObservableCollection<string>();
            WisselInputs = new ObservableCollection<string>();
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            OnDetectorenChanged(null);
            OnIngangenChanged(null);

            Refresh();
        }


        #endregion // Constructor
    }
}
