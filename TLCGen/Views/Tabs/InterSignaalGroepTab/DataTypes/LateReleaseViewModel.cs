using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Models;


namespace TLCGen.ViewModels
{
    public class LateReleaseViewModel : ObservableObjectEx
    {
        #region Fields

        private LateReleaseModel _lateRelease;

        #endregion // Fields

        #region Properties

        public int LateReleaseTijd
        {
            get => _lateRelease.LateReleaseTijd;
            set
            {
                _lateRelease.LateReleaseTijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int LateReleaseOntruimingstijd
        {
            get => _lateRelease.LateReleaseOntruimingstijd;
            set
            {
                _lateRelease.LateReleaseOntruimingstijd = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Comment1 =>
            $"Late release tijd van {_lateRelease.FaseVan} na {_lateRelease.FaseNaar}";

        public string Comment2 =>
            $"Late release {(TLCGenControllerDataProvider.Default.Controller.Data.Intergroen ? "intergroentijd" : "ontruimingstijd")} " +
            $"van {_lateRelease.FaseNaar} naar {_lateRelease.FaseVan}";

        #endregion // Properties

        #region Private methods

        #endregion // Private methods

        #region Collection changed

        #endregion // Collection changed

        #region TLCGen Events

        #endregion // TLCGen Events

        #region Constructor

        public LateReleaseViewModel(LateReleaseModel rl)
        {
            _lateRelease = rl;
        }

        #endregion // Constructor
    }
}
