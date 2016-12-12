using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class OVIngreepSignaalGroepParametersViewModel : ViewModelBase
    {
        #region Fields

        private OVIngreepSignaalGroepParametersModel _Parameters;

        #endregion // Fields

        #region Properties

        public OVIngreepSignaalGroepParametersModel Parameters
        {
            get { return _Parameters; }
        }

        public string FaseCyclus
        {
            get { return _Parameters.FaseCyclus; }
            set
            {
                _Parameters.FaseCyclus = value;
                OnPropertyChanged("FaseCyclus");
            }
        }

        public int AantalKerenNietAfkappen
        {
            get { return _Parameters.AantalKerenNietAfkappen; }
            set
            {
                _Parameters.AantalKerenNietAfkappen = value;
                OnPropertyChanged("FaseCyclus");
            }
        }
        public int MinimumGroentijdConflictOVRealisatie
        {
            get { return _Parameters.MinimumGroentijdConflictOVRealisatie; }
            set
            {
                _Parameters.MinimumGroentijdConflictOVRealisatie = value;
                OnPropertyChanged("FaseCyclus");
            }
        }

        public int PercMaxGroentijdConflictOVRealisatie
        {
            get { return _Parameters.PercMaxGroentijdConflictOVRealisatie; }
            set
            {
                _Parameters.PercMaxGroentijdConflictOVRealisatie = value;
                OnPropertyChanged("FaseCyclus");
            }
        }

        public int PercMaxGroentijdVoorTerugkomen
        {
            get { return _Parameters.PercMaxGroentijdVoorTerugkomen; }
            set
            {
                _Parameters.PercMaxGroentijdVoorTerugkomen = value;
                OnPropertyChanged("PercMaxGroentijdVoorTerugkomen");
            }
        }

        public int OndergrensNaTerugkomen
        {
            get { return _Parameters.OndergrensNaTerugkomen; }
            set
            {
                _Parameters.OndergrensNaTerugkomen = value;
                OnPropertyChanged("OndergrensNaTerugkomen");
            }
        }

        public int OphoogpercentageNaAfkappen
        {
            get { return _Parameters.OphoogpercentageNaAfkappen; }
            set
            {
                _Parameters.OphoogpercentageNaAfkappen = value;
                OnPropertyChanged("OphoogpercentageNaAfkappen");
            }
        }

        public int BlokkeertijdNaOVIngreep
        {
            get { return _Parameters.BlokkeertijdNaOVIngreep; }
            set
            {
                _Parameters.BlokkeertijdNaOVIngreep = value;
                OnPropertyChanged("BlokkeertijdNaOVIngreep");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region Constructor

        public OVIngreepSignaalGroepParametersViewModel(OVIngreepSignaalGroepParametersModel prms)
        {
            _Parameters = prms;
        }

        #endregion // Constructor
    }
}
