using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverFileDetectorViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private RoBuGroverFileDetectorModel _FileDetector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get { return _FileDetector.Detector; }
            set
            {
                _FileDetector.Detector = value;
                RaisePropertyChanged<RoBuGroverFileDetectorViewModel>("Detector", broadcast: true);
            }
        }
        public int FileTijd
        {
            get { return _FileDetector.FileTijd; }
            set
            {
                _FileDetector.FileTijd = value;
                RaisePropertyChanged<RoBuGroverFileDetectorViewModel>("FileTijd", broadcast: true);
            }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _FileDetector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public RoBuGroverFileDetectorViewModel(RoBuGroverFileDetectorModel filedetector)
        {
            _FileDetector = filedetector;
        }

        #endregion // Constructor

    }
}
