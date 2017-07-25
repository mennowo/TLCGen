using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleFaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private ModuleFaseCyclusModel _ModuleFaseCyclus;
        private ObservableCollection<ModuleFaseCyclusAlternatiefModel> _Alternatieven;
        
        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusModel ModuleFaseCyclus
        {
            get { return _ModuleFaseCyclus; }
        }

        public string FaseCyclusNaam
        {
            get { return _ModuleFaseCyclus.FaseCyclus; }
        }

        public ObservableCollectionAroundList<ModuleFaseCyclusAlternatiefViewModel, ModuleFaseCyclusAlternatiefModel> Alternatieven
        {
            get;
        }

        #endregion // Properties

        #region IComparable

        public int CompareTo(object obj)
        {
            ModuleFaseCyclusViewModel fcvm = obj as ModuleFaseCyclusViewModel;
            if (fcvm == null)
                throw new NotImplementedException();
            else
            {
                string myName = FaseCyclusNaam;
                string hisName = fcvm.FaseCyclusNaam;
                if (myName.Length < hisName.Length) myName = myName.PadLeft(hisName.Length, '0');
                else if (hisName.Length < myName.Length) hisName = hisName.PadLeft(myName.Length, '0');
                return myName.CompareTo(hisName);
            }
        }

        #endregion // IComparable

        #region Public Methods

        #endregion // Public Methods

        public override string ToString()
        {
            return FaseCyclusNaam;
        }

        #region Constructor

        public ModuleFaseCyclusViewModel(ModuleFaseCyclusModel mfcm)
        {
            _ModuleFaseCyclus = mfcm;
            Alternatieven = new ObservableCollectionAroundList<ModuleFaseCyclusAlternatiefViewModel, ModuleFaseCyclusAlternatiefModel>(mfcm.Alternatieven);
        }

        #endregion // Constructor
    }
}
