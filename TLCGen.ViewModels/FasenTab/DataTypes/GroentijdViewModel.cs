using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class GroentijdViewModel : ViewModelBase, IComparable
    {
        #region Fields
        
        private GroentijdModel _Groentijd;

        #endregion // Fields

        #region Properties

        public GroentijdModel Groentijd
        {
            get { return _Groentijd; }
        }

        public string FaseCyclus
        {
            get
            {
                return _Groentijd.FaseCyclus;
            }
            set
            {
                _Groentijd.FaseCyclus = value;
            }
        }

        public int? Waarde
        {
            get
            {
                return _Groentijd.Waarde;
            }
            set
            {
                _Groentijd.Waarde = value;
                OnMonitoredPropertyChanged("Waarde");
            }
        }

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Public methods

        #endregion // Public methods

        #region IComparable

        public int CompareTo(object obj)
        {
            GroentijdViewModel mgvm = obj as GroentijdViewModel;
            if (mgvm == null)
                throw new NotImplementedException();
            else
            {
                string myFase = FaseCyclus;
                string hisFase = mgvm.FaseCyclus;
                if (myFase.Length < hisFase.Length) myFase = myFase.PadLeft(hisFase.Length, '0');
                else if (hisFase.Length < myFase.Length) hisFase = hisFase.PadLeft(myFase.Length, '0');
                int i = myFase.CompareTo(hisFase);
                return i;
            }
        }

        #endregion // IComparable

        #region Constructor

        public GroentijdViewModel(GroentijdModel mgm)
        {
            _Groentijd = mgm;
        }

        #endregion // Constructor
    }
}