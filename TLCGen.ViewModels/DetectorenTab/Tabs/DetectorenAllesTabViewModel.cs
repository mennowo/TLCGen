using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list with all detectors that are in the controller.
    /// This class holds an ObservableCollection with all detectors, which must be updated 
    /// before it is used by the View.
    /// </summary>
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.DetectieTab)]
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
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
            }
        }

        public IList SelectedDetectoren
        {
            get { return _SelectedDetectoren; }
            set
            {
                _SelectedDetectoren = value;
                OnPropertyChanged("SelectedDetectoren");
            }
        }

        #endregion // Properties

        #region Public methods

        public void SetDetectorenChanged()
        {
            Detectoren.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    Detectoren.Add(new DetectorViewModel(dm));
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                Detectoren.Add(new DetectorViewModel(dm));
            }
        }

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Alles";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            Detectoren.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    Detectoren.Add(new DetectorViewModel(dm) { FaseCyclus = fcm.Naam });
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                Detectoren.Add(new DetectorViewModel(dm));
            }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public DetectorenAllesTabViewModel(ControllerModel controller) : base(controller)
        {
            SetDetectorenChanged();
        }

        #endregion // Constructor
    }
}
