using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class DetectorenDefaultsTabViewModel : ViewModelBase
    {
        #region Fields

        private object _SelectedDetector;
        private DetectorTypeEnum _SelectedDetectorType;
        private string _SelectedDetectorFunctionality;

        private List<string> _DetectorFunctionalities;

        #endregion // Fields

        #region Properties
        
        public List<string> DetectorFunctionalities
        {
            get { return _DetectorFunctionalities; }
        }

        public string SelectedDetectorFunctionality
        {
            get { return _SelectedDetectorFunctionality; }
            set
            {
                _SelectedDetectorFunctionality = value;
                SetSelectedDetectorDefaultsObject(SelectedDetectorType);
                RaisePropertyChanged("SelectedDetectorFunctionality");
            }
        }

        public object SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                RaisePropertyChanged("SelectedDetector");
            }
        }

        public DetectorTypeEnum SelectedDetectorType
        {
            get { return _SelectedDetectorType; }
            set
            {
                _SelectedDetectorType = value;
                SetSelectedDetectorDefaultsObject(SelectedDetectorType);
                RaisePropertyChanged("SelectedDetectorType");
            }
        }

        public List<DetectorDefaultsModel> Detectoren { get; private set; }

        #endregion // Properties

        #region Private Methods

        private void SetSelectedDetectorDefaultsObject(DetectorTypeEnum type)
        {
            if (!string.IsNullOrEmpty(SelectedDetectorFunctionality) && Detectoren.Where(x => x.Type == type).Any())
            {
                switch (SelectedDetectorFunctionality)
                {
                    case "Basis":
                        SelectedDetector = Detectoren.Where(x => x.Type == type).First().Detector;
                        break;
                    case "RoBuGrover":
                        SelectedDetector = Detectoren.Where(x => x.Type == type).First().RoBuGroverFileDetector;
                        if (SelectedDetector == null)
                        {
                            SelectedDetector = Detectoren.Where(x => x.Type == type).First().RoBuGroverHiaatDetector;
                        }
                        break;
                }
            }
            else
            {
                SelectedDetector = null;
            }
        }

        #endregion // Private Methods

        #region Constructor

        public DetectorenDefaultsTabViewModel()
        {

            _DetectorFunctionalities = new List<string>();
            _DetectorFunctionalities.Add("Basis");
            _DetectorFunctionalities.Add("RoBuGrover");
            _SelectedDetectorFunctionality = "Basis";
            Detectoren = new List<DetectorDefaultsModel>();
            foreach (var d in DefaultsProvider.Default.Defaults.Detectoren)
            {
                Detectoren.Add(d);
            }
            SelectedDetectorType = DetectorTypeEnum.Kop;
        }

        #endregion // Constructor
    }
}
