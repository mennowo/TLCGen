using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HDIngreepMeerealiserendeFaseCyclusViewModel : ViewModelBase
    {
        #region Fields

        private HDIngreepMeerealiserendeFaseCyclusModel _FaseCyclus;

        #endregion // Fields

        #region Properties

        public HDIngreepMeerealiserendeFaseCyclusModel FaseCyclus
        {
            get { return _FaseCyclus; }
            set
            {
                _FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        public string Fase
        {
            get { return _FaseCyclus.FaseCyclus; }
            set
            {
                _FaseCyclus.FaseCyclus = value;
                OnMonitoredPropertyChanged("Fase");
            }
        }

        #endregion // Properties

        #region Constructor

        public HDIngreepMeerealiserendeFaseCyclusViewModel(HDIngreepMeerealiserendeFaseCyclusModel fase)
        {
            _FaseCyclus = fase;
        }

        #endregion // Constructor
    }
}
