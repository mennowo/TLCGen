using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class PTPKoppelingViewModel : ViewModelBase
    {
        #region Fields

        private PTPKoppelingModel _PTPKoppeling;

        #endregion // Fields

        #region Properties

        public PTPKoppelingModel PTPKoppeling
        {
            get { return _PTPKoppeling; }
        }

        public string TeKoppelenKruispunt
        {
            get { return _PTPKoppeling.TeKoppelenKruispunt; }
            set
            {
                _PTPKoppeling.TeKoppelenKruispunt = value;
                OnMonitoredPropertyChanged("TeKoppelenKruispunt");
            }
        }
        public int AantalsignalenIn
        {
            get { return _PTPKoppeling.AantalsignalenIn; }
            set
            {
                _PTPKoppeling.AantalsignalenIn = value;
                OnMonitoredPropertyChanged("AantalsignalenIn");
            }
        }

        public int AantalsignalenUit
        {
            get { return _PTPKoppeling.AantalsignalenUit; }
            set
            {
                _PTPKoppeling.AantalsignalenUit = value;
                OnMonitoredPropertyChanged("AantalsignalenUit");
            }
        }

        public int PortnummerSimuatieOmgeving
        {
            get { return _PTPKoppeling.PortnummerSimuatieOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerSimuatieOmgeving = value;
                OnMonitoredPropertyChanged("PortnummerSimuatieOmgeving");
            }
        }

        public int PortnummerAutomaatOmgeving
        {
            get { return _PTPKoppeling.PortnummerAutomaatOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerAutomaatOmgeving = value;
                OnMonitoredPropertyChanged("PortnummerAutomaatOmgeving");
            }
        }

        public int NummerSource
        {
            get { return _PTPKoppeling.NummerSource; }
            set
            {
                _PTPKoppeling.NummerSource = value;
                OnMonitoredPropertyChanged("NummerSource");
            }
        }

        public int NummerDestination
        {
            get { return _PTPKoppeling.NummerDestination; }
            set
            {
                _PTPKoppeling.NummerDestination = value;
                OnMonitoredPropertyChanged("NummerDestination");
            }
        }


        #endregion // Properties

        #region Constructor

        public PTPKoppelingViewModel(PTPKoppelingModel kop)
        {
            _PTPKoppeling = kop;
        }

        #endregion // Constructor
    }
}
