using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleFaseCyclusViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private ModuleFaseCyclusModel _ModuleFaseCyclus;
        private ControllerViewModel _ControllerVM;
        private ObservableCollection<string> _Alternatieven;
        private string _FaseCyclusNaam;

        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusModel ModuleFaseCyclus
        {
            get { return _ModuleFaseCyclus; }
        }

        public string FaseCyclusDefine
        {
            get { return _ModuleFaseCyclus.FaseCyclus; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _ModuleFaseCyclus.FaseCyclus = value;
                }
                OnPropertyChanged("FaseCyclus");
            }
        }

        public string FaseCyclusNaam
        {
            get { return _FaseCyclusNaam; }
            set
            {
                _FaseCyclusNaam = value;
                OnPropertyChanged("FaseCyclusNaam");
            }
        }

        public ObservableCollection<string> Alternatieven
        {
            get
            {
                if (_Alternatieven == null)
                {
                    _Alternatieven = new ObservableCollection<string>();
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
                string myName = FaseCyclusDefine;
                string hisName = fcvm.FaseCyclusDefine;
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
                foreach (string s in e.NewItems)
                {
                    _ModuleFaseCyclus.Alternatieven.Add(s);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (string s in e.OldItems)
                {
                    _ModuleFaseCyclus.Alternatieven.Remove(s);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Public Methods

        #endregion // Public Methods

        public override string ToString()
        {
            return FaseCyclusNaam;
        }

        #region Constructor

        public ModuleFaseCyclusViewModel(ControllerViewModel controllervm, ModuleFaseCyclusModel mfcm)
        {
            _ControllerVM = controllervm;
            _ModuleFaseCyclus = mfcm;
            foreach(string s in _ModuleFaseCyclus.Alternatieven)
            {
                Alternatieven.Add(s);
            }
            Alternatieven.CollectionChanged += Alternatieven_CollectionChanged;
        }

        #endregion // Constructor
    }
}
