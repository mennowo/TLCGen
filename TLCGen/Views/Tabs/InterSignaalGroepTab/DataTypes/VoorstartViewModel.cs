using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class VoorstartViewModel : ObservableObjectEx
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
                OnPropertyChanged(broadcast: true);
            }
		}

		public int VoorstartOntruimingstijd
		{
			get => _voorstart.VoorstartOntruimingstijd;
			set
			{
				_voorstart.VoorstartOntruimingstijd = value;
                OnPropertyChanged(broadcast: true);
            }
		}

        public string Comment1 =>
            $"Voorstart tijd van {_voorstart.FaseVan} op {_voorstart.FaseNaar}";

        public string Comment2 =>
            $"Voorstart {(TLCGenControllerDataProvider.Default.Controller.Data.Intergroen ? "intergroentijd" : "ontruimingstijd")} van {_voorstart.FaseNaar} naar {_voorstart.FaseVan}";

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