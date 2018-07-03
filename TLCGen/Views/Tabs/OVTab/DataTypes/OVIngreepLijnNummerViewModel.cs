using GalaSoft.MvvmLight;
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
                RaisePropertyChanged<object>("LijnNummer", broadcast: true);
            }
        }

        public string Nummer
        {
            get { return _LijnNummer.Nummer; }
            set
            {
                _LijnNummer.Nummer = value;
                RaisePropertyChanged<object>("Nummer", broadcast: true);
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
