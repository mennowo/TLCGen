using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Enumerations;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class DetectorViewModel : ViewModelBase
    {
        #region Fields

        private DetectorModel _Detector;
        private ControllerViewModel _ControllerVM;

        #endregion // Fields

        #region Properties

        public DetectorModel Detector
        {
            get { return _Detector; }
        }

        public long ID
        {
            get { return _Detector.ID; }
        }

        public string Naam
        {
            get { return _Detector.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && _ControllerVM.IsDetectorNaamUnique(value))
                {
                    string oldname = _Detector.Naam;
                    _Detector.Naam = value;
                    _Detector.Define = _ControllerVM.ControllerDataVM.PrefixSettings.DetectorDefinePrefix + value;
                }
                OnMonitoredPropertyChanged("Naam", _ControllerVM);
            }
        }

        public DetectorTypeEnum Type
        {
            get { return _Detector.Type; }
            set
            {
                _Detector.Type = value;
                OnMonitoredPropertyChanged("Type", _ControllerVM);
            }
        }

        public int TDB
        {
            get { return _Detector.TDB; }
            set
            {
                if (value >= 0)
                    _Detector.TDB = value;
                OnMonitoredPropertyChanged("TDB", _ControllerVM);
            }
        }

        public int TDH
        {
            get { return _Detector.TDH; }
            set
            {
                if (value >= 0)
                    _Detector.TDH = value;
                OnMonitoredPropertyChanged("TDH", _ControllerVM);
            }
        }

        public int TOG
        {
            get { return _Detector.TOG; }
            set
            {
                if (value >= 0)
                    _Detector.TOG = value;
                OnMonitoredPropertyChanged("TOG", _ControllerVM);
            }
        }

        public int TBG
        {
            get { return _Detector.TBG; }
            set
            {
                if (value >= 0)
                    _Detector.TBG = value;
                OnMonitoredPropertyChanged("TBG", _ControllerVM);
            }
        }

        public DetectorAanvraagTypeEnum Aanvraag
        {
            get { return _Detector.Aanvraag; }
            set
            {
                _Detector.Aanvraag = value;
                OnMonitoredPropertyChanged("Aanvraag", _ControllerVM);
            }
        }

        public DetectorVerlengenTypeEnum Verlengen
        {
            get { return _Detector.Verlengen; }
            set
            {
                _Detector.Verlengen = value;
                OnMonitoredPropertyChanged("Verlengen", _ControllerVM);
            }
        }

        #endregion // Properties

        #region Constructor

        public DetectorViewModel(ControllerViewModel controllervm, DetectorModel detector)
        {
            _ControllerVM = controllervm;
            _Detector = detector;

        }

        #endregion // Constructor
    }
}
