using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepPeriodeViewModel : ObservableObjectEx, IViewModelWithItem
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

    public class OVIngreepLijnNummerViewModel : ObservableObjectEx, IViewModelWithItem
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
                OnPropertyChanged(nameof(LijnNummer), broadcast: true);
            }
        }

        public string Nummer
        {
            get => _LijnNummer.Nummer;
            set
            {
                _LijnNummer.Nummer = value;
                OnPropertyChanged(nameof(Nummer), broadcast: true);
            }
        }

        public string RitCategorie
        {
            get => _LijnNummer.RitCategorie;
            set
            {
                _LijnNummer.RitCategorie = value;
                OnPropertyChanged(nameof(RitCategorie), broadcast: true);
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
