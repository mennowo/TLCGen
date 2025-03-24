using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;


namespace TLCGen.ViewModels
{
    public class VAOntruimenFaseViewModel : ObservableObjectEx, IViewModelWithItem
    {
        #region Fields

        private VAOntruimenFaseModel _VAOntruimenFase;
        private Dictionary<string, int> _ConflicterendeFasen;
        private ItemsManagerViewModel<VAOntruimenDetectorViewModel, string> _detectorManager;
        private ObservableCollection<string> _VAOntruimenMatrixColumnHeaders;
        private ObservableCollection<string> _VAOntruimenMatrixRowHeaders;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get => _VAOntruimenFase.FaseCyclus;
            set
            {
                _VAOntruimenFase.FaseCyclus = value;
                OnPropertyChanged();
            }
        }

        public int VAOntrMax
        {
            get => _VAOntruimenFase.VAOntrMax;
            set
            {
                _VAOntruimenFase.VAOntrMax = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool KijkNaarWisselstand
        {
            get => _VAOntruimenFase.KijkNaarWisselstand;
            set
            {
                _VAOntruimenFase.KijkNaarWisselstand = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasWissel1));
                OnPropertyChanged(nameof(HasWissel2));
                OnPropertyChanged(nameof(HasWissel1Voorwaarde));
                OnPropertyChanged(nameof(HasWissel2Voorwaarde));
                OnPropertyChanged(nameof(IsWissel1Detector));
                OnPropertyChanged(nameof(IsWissel1Ingang));
            }
        }

        public PrioIngreepInUitDataWisselTypeEnum Wissel1Type
        {
            get => _VAOntruimenFase.Wissel1Type;
            set
            {
                _VAOntruimenFase.Wissel1Type = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasWissel1Voorwaarde));
                OnPropertyChanged(nameof(IsWissel1Detector));
                OnPropertyChanged(nameof(IsWissel1Ingang));
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
                    OnPropertyChanged(broadcast: true);
                }
                else
                {
                    OnPropertyChanged();
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
                    OnPropertyChanged(broadcast: true);
                }
                else
                {
                    OnPropertyChanged();
                }
            }
        }

        public bool Wissel1Voorwaarde
        {
            get => _VAOntruimenFase.Wissel1InputVoorwaarde;
            set
            {
                _VAOntruimenFase.Wissel1InputVoorwaarde = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool Wissel2
        {
            get => _VAOntruimenFase.Wissel2;
            set
            {
                _VAOntruimenFase.Wissel2 = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasWissel2));
                OnPropertyChanged(nameof(IsWissel2Detector));
                OnPropertyChanged(nameof(IsWissel2Ingang));
                OnPropertyChanged(nameof(HasWissel2Voorwaarde));
            }
        }

        public PrioIngreepInUitDataWisselTypeEnum Wissel2Type
        {
            get => _VAOntruimenFase.Wissel2Type;
            set
            {
                _VAOntruimenFase.Wissel2Type = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(HasWissel2Voorwaarde));
                OnPropertyChanged(nameof(IsWissel2Detector));
                OnPropertyChanged(nameof(IsWissel2Ingang));
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
                    OnPropertyChanged(broadcast: true);
                }
                else
                {
                    OnPropertyChanged();
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
                    OnPropertyChanged(broadcast: true);
                }
                else
                {
                    OnPropertyChanged();
                }
            }
        }

        public bool Wissel2Voorwaarde
        {
            get => _VAOntruimenFase.Wissel2InputVoorwaarde;
            set
            {
                _VAOntruimenFase.Wissel2InputVoorwaarde = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool HasWissel1 => KijkNaarWisselstand;
        public bool IsWissel1Ingang => KijkNaarWisselstand && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
        public bool IsWissel1Detector => KijkNaarWisselstand && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector;
        public bool HasWissel1Voorwaarde => KijkNaarWisselstand && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
        public bool HasWissel2 => KijkNaarWisselstand && Wissel2;
        public bool IsWissel2Ingang => HasWissel2 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;
        public bool IsWissel2Detector => HasWissel2 && Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Detector;
        public bool HasWissel2Voorwaarde => HasWissel2 && Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang;

        public ObservableCollectionAroundList<VAOntruimenDetectorViewModel, VAOntruimenDetectorModel> VAOntruimenDetectoren
        {
            get;
        }

        public Dictionary<string, int> ConflicterendeFasen => _ConflicterendeFasen ??= new Dictionary<string, int>();

        public VAOntruimenNaarFaseViewModel[,] VAOntruimenMatrix
        {
            get;
            set;
        }

        public ObservableCollection<string> VAOntruimenMatrixColumnHeaders =>
            _VAOntruimenMatrixColumnHeaders ??= new ObservableCollection<string>();

        public ObservableCollection<string> VAOntruimenMatrixRowHeaders =>
            _VAOntruimenMatrixRowHeaders ??= new ObservableCollection<string>();

        public ItemsManagerViewModel<VAOntruimenDetectorViewModel, string> DetectorManager
        {
            get
            {
                if (_detectorManager == null)
                {
                    _detectorManager = new ItemsManagerViewModel<VAOntruimenDetectorViewModel, string>(
                        VAOntruimenDetectoren,
                        ControllerAccessProvider.Default.AllDetectorStrings,
                        x =>
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
                        x => VAOntruimenDetectoren.All(y => y.Detector != x),
                        x => VAOntruimenDetectoren.FirstOrDefault(d => d.Detector == x),
                        Refresh,
                        Refresh);
                }
                return _detectorManager;
            }
            private set => _detectorManager = value;
        }

        public ObservableCollection<string> WisselDetectoren { get; }

        public ObservableCollection<string> WisselInputs { get; }
        
        #endregion Properties

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

            for (var d = 0; d < VAOntruimenDetectoren.Count; ++d)
            {
                VAOntruimenMatrixColumnHeaders.Add(VAOntruimenDetectoren[d].Detector);
                for (var cfc = 0; cfc < VAOntruimenDetectoren[d].ConflicterendeFasen.Count; ++cfc)
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
            VAOntruimenDetectoren.CollectionChanged -= VAOntruimenDetectoren_CollectionChanged;
            VAOntruimenDetectoren.Rebuild();
            VAOntruimenDetectoren.CollectionChanged += VAOntruimenDetectoren_CollectionChanged;
            RebuildVAOnruimenMatrix();
            DetectorManager = null;
            OnPropertyChanged("DetectorManager");
        }

        #endregion // Public Methods

        #region TLCGen Events

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            _detectorManager?.Refresh();

            WisselDetectoren.Clear();
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.SelectMany(x => x.Detectoren.Where(x2 => x2.Type == DetectorTypeEnum.WisselStandDetector)))
            {
                WisselDetectoren.Add(d.Naam);
            }
            foreach (var d in DataAccess.TLCGenControllerDataProvider.Default.Controller.Detectoren.Where(x => x.Type == DetectorTypeEnum.WisselStandDetector))
            {
                WisselDetectoren.Add(d.Naam);
            }
        }

        private void OnIngangenChanged(object sender, IngangenChangedMessage obj)
        {
            WisselInputs.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
            {
                WisselInputs.Add(seld.Naam);
            }
        }

        private void OnNameChanged(object sender, NameChangedMessage obj)
        {
            if (obj.ObjectType != TLCGenObjectTypeEnum.Detector) return;
            _detectorManager?.Refresh();
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
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
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
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            OnDetectorenChanged(null, null);
            OnIngangenChanged(null, null);

            Refresh();
        }

        #endregion // Constructor
    }
}
