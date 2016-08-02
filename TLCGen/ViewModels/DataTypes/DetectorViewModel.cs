using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
