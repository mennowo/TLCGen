using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class MaxGroentijdViewModel : ViewModelBase, IComparable
    {
        #region Fields
        
        private MaxGroentijdModel _MaxGroentijd;

        #endregion // Fields

        #region Properties

        public MaxGroentijdModel MaxGroentijd
        {
            get { return _MaxGroentijd; }
        }

        public string FaseCyclus
        {
            get
            {
                return _MaxGroentijd.FaseCyclus;
            }
            set
            {
                _MaxGroentijd.FaseCyclus = value;
            }
        }

        public int? Waarde
        {
            get
            {
                return _MaxGroentijd.Waarde;
            }
            set
            {
                _MaxGroentijd.Waarde = value;
                OnPropertyChanged("Waarde");
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
            MaxGroentijdViewModel mgvm = obj as MaxGroentijdViewModel;
            if (mgvm == null)
                throw new NotImplementedException();
            else
            {
                string myFase = FaseCyclus.Replace(SettingsProvider.AppSettings.PrefixSettings.FaseCyclusDefinePrefix.Setting, "");
                string hisFase = mgvm.FaseCyclus.Replace(SettingsProvider.AppSettings.PrefixSettings.FaseCyclusDefinePrefix.Setting, "");
                if (myFase.Length < hisFase.Length) myFase = myFase.PadLeft(hisFase.Length, '0');
                else if (hisFase.Length < myFase.Length) hisFase = hisFase.PadLeft(myFase.Length, '0');
                return myFase.CompareTo(hisFase);
            }
        }

        #endregion // IComparable

        #region Constructor

        public MaxGroentijdViewModel(MaxGroentijdModel mgm)
        {
            _MaxGroentijd = mgm;
        }

        #endregion // Constructor
    }
}