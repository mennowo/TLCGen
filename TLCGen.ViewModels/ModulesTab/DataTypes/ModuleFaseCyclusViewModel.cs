using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ObservableCollection<ModuleFaseCyclusAlternatiefModel> Alternatieven
        {
            get
            {
                if (_Alternatieven == null)
                {
                    _Alternatieven = new ObservableCollection<ModuleFaseCyclusAlternatiefModel>();
                }
                return _Alternatieven;
            }
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

        #region Collection Changed

        private void Alternatieven_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (ModuleFaseCyclusAlternatiefModel s in e.NewItems)
                {
                    _ModuleFaseCyclus.Alternatieven.Add(s);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (ModuleFaseCyclusAlternatiefModel s in e.OldItems)
                {
                    _ModuleFaseCyclus.Alternatieven.Remove(s);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

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
            foreach(ModuleFaseCyclusAlternatiefModel s in _ModuleFaseCyclus.Alternatieven)
            {
                Alternatieven.Add(s);
            }
            Alternatieven.CollectionChanged += Alternatieven_CollectionChanged;
        }

        #endregion // Constructor
    }
}
