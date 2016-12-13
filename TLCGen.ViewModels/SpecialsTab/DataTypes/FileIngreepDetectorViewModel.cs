using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class FileIngreepDetectorViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private FileIngreepDetectorModel _Detector;

        #endregion // Fields

        #region Properties

        public string Detector
        {
            get { return _Detector.Detector; }
            set
            {
                _Detector.Detector = value;
                OnPropertyChanged("Detector");
            }
        }

        public int BezetTijd
        {
            get { return _Detector.BezetTijd; }
            set
            {
                _Detector.BezetTijd = value;
                OnMonitoredPropertyChanged("BezetTijd");
            }
        }

        public int RijTijd
        {
            get { return _Detector.RijTijd; }
            set
            {
                _Detector.RijTijd = value;
                OnMonitoredPropertyChanged("RijTijd");
            }
        }

        public int AfvalVertraging
        {
            get { return _Detector.AfvalVertraging; }
            set
            {
                _Detector.AfvalVertraging = value;
                OnMonitoredPropertyChanged("AfvalVertraging");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Detector;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public FileIngreepDetectorViewModel(FileIngreepDetectorModel detector)
        {
            _Detector = detector;
        }

        #endregion // Constructor
    }
}
