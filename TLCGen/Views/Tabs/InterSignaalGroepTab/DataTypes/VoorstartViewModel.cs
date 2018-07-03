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