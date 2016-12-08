using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepLijnNummerViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepLijnNummerModel _LijnNummer;

        #endregion // Fields

        #region Properties

        public OVIngreepLijnNummerModel LijnNummer
        {
            get { return _LijnNummer; }
            set
            {
                _LijnNummer = value;
                OnMonitoredPropertyChanged("LijnNummer");
            }
        }

        public string Nummer
        {
            get { return _LijnNummer.Nummer; }
            set
            {
                _LijnNummer.Nummer = value;
                OnMonitoredPropertyChanged("Nummer");
            }
        }

        #endregion // Properties

        #region Constructor

        public OVIngreepLijnNummerViewModel(OVIngreepLijnNummerModel nummer)
        {
            _LijnNummer = nummer;
        }

        #endregion // Constructor
    }
}
