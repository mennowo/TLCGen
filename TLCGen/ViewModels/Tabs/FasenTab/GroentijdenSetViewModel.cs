using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class GroentijdenSetViewModel : ViewModelBase
    {
        #region Fields
        
        private ObservableCollection<GroentijdViewModel> _GroentijdenSetList;
        private GroentijdenSetModel _GroentijdenSet;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get
            {
                return _GroentijdenSet.Naam;
            }
            set
            {
                _GroentijdenSet.Naam = value;
                OnPropertyChanged("Naam");
            }
        }

        public GroentijdenTypeEnum Type
        {
            get
            {
                return _GroentijdenSet.Type;
            }
            set
            {
                _GroentijdenSet.Type = value;
                switch (value)
                {
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        this.Naam = "VG" + this.Naam.Substring(2);
                        break;
                    default:
                        this.Naam = "MG" + this.Naam.Substring(2); 
                        break;

                }
            }
        }

        public GroentijdenSetModel GroentijdenSet
        {
            get
            {
                return _GroentijdenSet;
            }
        }

        public ObservableCollection<GroentijdViewModel> GroentijdenSetList
        {
            get
            {
                if (_GroentijdenSetList == null)
                {
                    _GroentijdenSetList = new ObservableCollection<GroentijdViewModel>();
                }
                return _GroentijdenSetList;
            }
        }

        #endregion // Properties

        #region Collection Changed

        private void GroentijdenSetList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (GroentijdViewModel mgvm in e.NewItems)
                {
                    _GroentijdenSet.Groentijden.Add(mgvm.Groentijd);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (GroentijdViewModel mgvm in e.OldItems)
                {
                    _GroentijdenSet.Groentijden.Remove(mgvm.Groentijd);
                }
            }
        }

        #endregion // Collection Changed

        #region Public methods

        public void AddFase(string fasedefine, string fasename)
        {
            GroentijdModel mgm = new GroentijdModel();
            mgm.FaseCyclus = fasedefine;
            mgm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultGroenTijd(fasedefine);
            GroentijdenSetList.Add(new GroentijdViewModel(mgm));
        }

        public void RemoveFase(string fasedefine)
        {
            GroentijdViewModel _mgvm = null;
            foreach (GroentijdViewModel mgvm in GroentijdenSetList)
            {
                if(mgvm.FaseCyclus == fasedefine)
                {
                    _mgvm = mgvm;
                }
            }
            if(_mgvm != null)
            {
                GroentijdenSetList.Remove(_mgvm);
            }
        }

        #endregion // Public methods

        #region Constructor

        public GroentijdenSetViewModel(GroentijdenSetModel mgsm)
        {
            _GroentijdenSet = mgsm;

            foreach(GroentijdModel mgm in mgsm.Groentijden)
            {
                GroentijdViewModel mgvm = new GroentijdViewModel(mgm);
                GroentijdenSetList.Add(mgvm);
            }

            GroentijdenSetList.CollectionChanged += GroentijdenSetList_CollectionChanged;
        }

        #endregion // Constructor
    }
}
