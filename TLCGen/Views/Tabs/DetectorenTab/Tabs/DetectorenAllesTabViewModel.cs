using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list with all detectors that are in the controller.
    /// This class holds an ObservableCollection with all detectors, which must be updated 
    /// before it is used by the View.
    /// </summary>
    [TLCGenTabItem(index: 30, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenAllesTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();
        private ObservableCollection<DetectorViewModel> _Detectoren;

        #endregion // Fields

        #region Properties

        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                return _Detectoren;
            }
        }
        
        public DetectorViewModel SelectedDetector
        {
            get => _SelectedDetector;
            set
            {
                _SelectedDetector = value;
                RaisePropertyChanged("SelectedDetector");
            }
        }

        public IList SelectedDetectoren
        {
            get => _SelectedDetectoren;
            set
            {
                _SelectedDetectoren = value;
                RaisePropertyChanged("SelectedDetectoren");
            }
        }

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        #region Private Methods

        private void UpdateDetectoren()
        {
            foreach (var d in Detectoren) d.PropertyChanged -= Detector_PropertyChanged;
            Detectoren.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                foreach (var dm in fcm.Detectoren)
                {
                    var dvm = new DetectorViewModel(dm) { FaseCyclus = fcm.Naam };
                    dvm.PropertyChanged += Detector_PropertyChanged;
                    Detectoren.Add(dvm);
                }
            }
			Detectoren.BubbleSort();
            foreach (var dm in _Controller.Detectoren)
            {
                var dvm = new DetectorViewModel(dm);
                dvm.PropertyChanged += Detector_PropertyChanged;
                Detectoren.Add(dvm);
            }
        }

        #endregion // Private Methods

        #region TabItem Overrides

        public override string DisplayName => "Alle detectie";

	    public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            UpdateDetectoren();
        }

        #endregion // TabItem Overrides

        #region TLCGen Events

        private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            UpdateDetectoren();
        }

        #endregion // TLCGen Events

        #region Event handling

        private bool _SettingMultiple = false;
        private void Detector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedDetectoren != null && SelectedDetectoren.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<DetectorViewModel>(sender, e.PropertyName, SelectedDetectoren);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Constructor

        public DetectorenAllesTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
        }

        #endregion // Constructor
    }
}
