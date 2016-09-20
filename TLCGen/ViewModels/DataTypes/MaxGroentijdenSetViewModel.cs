using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class MaxGroentijdenSetViewModel : ViewModelBase
    {
        #region Fields
        
        private ObservableCollection<MaxGroentijdViewModel> _MaxGroentijdenSetList;
        private MaxGroentijdenSetModel _MaxGroentijdenSet;
        private ControllerViewModel _ControllerVM;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get
            {
                return _MaxGroentijdenSet.Naam;
            }
            set
            {
                _MaxGroentijdenSet.Naam = value;
                OnPropertyChanged("Naam");
            }
        }

        public MaxGroentijdenSetModel MaxGroentijdenSet
        {
            get
            {
                return _MaxGroentijdenSet;
            }
        }

        public ObservableCollection<MaxGroentijdViewModel> MaxGroentijdenSetList
        {
            get
            {
                if (_MaxGroentijdenSetList == null)
                {
                    _MaxGroentijdenSetList = new ObservableCollection<MaxGroentijdViewModel>();
                }
                return _MaxGroentijdenSetList;
            }
        }

        #endregion // Properties

        #region Collection Changed

        private void _MaxGroentijdenSetList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MaxGroentijdViewModel mgvm in e.NewItems)
                {
                    _MaxGroentijdenSet.MaxGroentijden.Add(mgvm.MaxGroentijd);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MaxGroentijdViewModel mgvm in e.OldItems)
                {
                    _MaxGroentijdenSet.MaxGroentijden.Remove(mgvm.MaxGroentijd);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Public methods

        public void AddFase(string fasedefine)
        {
            MaxGroentijdModel mgm = new MaxGroentijdModel();
            mgm.FaseCyclus = fasedefine;
            mgm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultMaxGroenTijd(fasedefine);
            MaxGroentijdenSetList.Add(new MaxGroentijdViewModel(_ControllerVM, mgm));
        }

        public void RemoveFase(string fasedefine)
        {
            MaxGroentijdViewModel _mgvm = null;
            foreach (MaxGroentijdViewModel mgvm in MaxGroentijdenSetList)
            {
                if(mgvm.FaseCyclus == fasedefine)
                {
                    _mgvm = mgvm;
                }
            }
            if(_mgvm != null)
            {
                MaxGroentijdenSetList.Remove(_mgvm);
            }
        }

        #endregion // Public methods

        #region Constructor

        public MaxGroentijdenSetViewModel(ControllerViewModel controllervm, MaxGroentijdenSetModel mgsm)
        {
            _ControllerVM = controllervm;
            _MaxGroentijdenSet = mgsm;

            foreach(MaxGroentijdModel mgm in mgsm.MaxGroentijden)
            {
                MaxGroentijdViewModel mgvm = new MaxGroentijdViewModel(_ControllerVM, mgm);
                MaxGroentijdenSetList.Add(mgvm);
            }

            MaxGroentijdenSetList.CollectionChanged += _MaxGroentijdenSetList_CollectionChanged;
        }

        #endregion // Constructor
    }
}
