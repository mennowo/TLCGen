using GalaSoft.MvvmLight;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class GelijkstartViewModel : ViewModelBase
	{
        #region Fields

		private readonly GelijkstartModel _gelijkstart;

        #endregion // Fields

        #region Properties

        public GelijkstartViewModel MirroredViewModel;

		public bool DeelConflict
		{
			get => _gelijkstart.DeelConflict;
			set
			{
				_gelijkstart.DeelConflict = value;
                if (MirroredViewModel != null)
                {
                    MirroredViewModel.DeelConflictNoMessaging = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
			}
		}

        public bool DeelConflictNoMessaging
        {
            get => _gelijkstart.DeelConflict;
            set => _gelijkstart.DeelConflict = value;
        }

        public int OntruimingstijdFaseVan
		{
			get => _gelijkstart.GelijkstartOntruimingstijdFaseVan;
			set
			{
				_gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
                if (MirroredViewModel != null)
                {
                    MirroredViewModel.OntruimingstijdFaseNaarNoMessaging = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

        public int OntruimingstijdFaseVanNoMessaging
        {
            get => _gelijkstart.GelijkstartOntruimingstijdFaseVan;
            set => _gelijkstart.GelijkstartOntruimingstijdFaseVan = value;
        }

        public int OntruimingstijdFaseNaar
		{
			get => _gelijkstart.GelijkstartOntruimingstijdFaseNaar;
			set
			{
				_gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
                if (MirroredViewModel != null)
                {
                    MirroredViewModel.OntruimingstijdFaseVanNoMessaging = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

        public int OntruimingstijdFaseNaarNoMessaging
        {
            get => _gelijkstart.GelijkstartOntruimingstijdFaseNaar;
            set => _gelijkstart.GelijkstartOntruimingstijdFaseNaar = value;
        }

        public AltijdAanUitEnum Schakelbaar
        {
            get => _gelijkstart.Schakelbaar;
            set
            {
                _gelijkstart.Schakelbaar = value;
                if (MirroredViewModel != null)
                {
                    MirroredViewModel.SchakelbaarNoMessaging = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public AltijdAanUitEnum SchakelbaarNoMessaging
        {
            get => _gelijkstart.Schakelbaar;
            set => _gelijkstart.Schakelbaar = value;
        }

        public string Comment1 =>
			$"Fictive {(TLCGenControllerDataProvider.Default.Controller.Data.Intergroen ? "intergroentijd" : "ontruimingstijd")} " +
			$"van {_gelijkstart.FaseVan} naar {_gelijkstart.FaseNaar}";

		public string Comment2 =>
			$"Fictive {(TLCGenControllerDataProvider.Default.Controller.Data.Intergroen ? "intergroentijd" : "ontruimingstijd")} " +
			$"van {_gelijkstart.FaseNaar} naar {_gelijkstart.FaseVan}";

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Collection changed

        #endregion // Collection changed

        #region TLCGen Events
      
        #endregion // TLCGen Events

        #region Constructor

        public GelijkstartViewModel(GelijkstartModel gs)
        {
            _gelijkstart = gs;
        }

        #endregion // Constructor
	}
}