using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverConflictGroepFaseViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        #region Fields

        private RoBuGroverConflictGroepFaseModel _Fase;

        #endregion // Fields

        #region Properties

        public string FaseCyclus
        {
            get { return _Fase.FaseCyclus; }
            set
            {
                _Fase.FaseCyclus = value;
                OnMonitoredPropertyChanged("FaseCyclus");
            }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region Overrides

        public override string ToString()
        {
            return FaseCyclus;
        }

        #endregion // Overrides

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Fase;
        }

        #endregion // IViewModelWithItem

        #region IComparable

        public int CompareTo(object obj)
        {
            if(obj is RoBuGroverConflictGroepFaseViewModel)
            {
                return this.FaseCyclus.CompareTo(((RoBuGroverConflictGroepFaseViewModel)obj).FaseCyclus);
            }
            return 0;
        }

        #endregion // IComparable

        #region Constructor

        public RoBuGroverConflictGroepFaseViewModel(RoBuGroverConflictGroepFaseModel fase)
        {
            _Fase = fase;
        }

        #endregion // Constructor
    }
}
