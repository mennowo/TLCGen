using GalaSoft.MvvmLight;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VoorstartViewModel : ViewModelBase
	{
        #region Fields

        private VoorstartModel _voorstart;
        
        #endregion // Fields

        #region Properties

		public int VoorstartTijd
		{
			get => _voorstart.VoorstartTijd;
			set
			{
				_voorstart.VoorstartTijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

		public int VoorstartOntruimingstijd
		{
			get => _voorstart.VoorstartOntruimingstijd;
			set
			{
				_voorstart.VoorstartOntruimingstijd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

        public string Comment1 =>
            $"Voorstart tijd van {_voorstart.FaseVan} naar {_voorstart.FaseNaar}";

        public string Comment2 =>
            $"Voorstart ontruimingstijd van {_voorstart.FaseNaar} naar {_voorstart.FaseVan}";

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Collection changed

        #endregion // Collection changed

        #region TLCGen Events

        #endregion // TLCGen Events

        #region Constructor

        public VoorstartViewModel(VoorstartModel vs)
        {
            _voorstart = vs;
        }

        #endregion // Constructor
	}
}