using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepPeriodeViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields
        #endregion // Fields

        #region Properties

        public OVIngreepPeriodeModel Periode { get; }

        public string PeriodeNaam => Periode.Periode;

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return Periode;
        }

        #endregion // IViewModelWithItem

        #region Constructor
        
        public OVIngreepPeriodeViewModel(OVIngreepPeriodeModel periode)
        {
            Periode = periode;
        }
        
        #endregion // Constructor
    }

    public class OVIngreepLijnNummerViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private OVIngreepLijnNummerModel _LijnNummer;

        #endregion // Fields

        #region Properties

        public OVIngreepLijnNummerModel LijnNummer
        {
            get => _LijnNummer;
            set
            {
                _LijnNummer = value;
                RaisePropertyChanged<object>(nameof(LijnNummer), broadcast: true);
            }
        }

        public string Nummer
        {
            get => _LijnNummer.Nummer;
            set
            {
                _LijnNummer.Nummer = value;
                RaisePropertyChanged<object>(nameof(Nummer), broadcast: true);
            }
        }

        public string RitCategorie
        {
            get => _LijnNummer.RitCategorie;
            set
            {
                _LijnNummer.RitCategorie = value;
                RaisePropertyChanged<object>(nameof(RitCategorie), broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
        {
            return _LijnNummer;
        }

        #endregion // IViewModelWithItem

        #region Constructor

        public OVIngreepLijnNummerViewModel(OVIngreepLijnNummerModel nummer)
        {
            _LijnNummer = nummer;
        }

        #endregion // Constructor
    }
}
