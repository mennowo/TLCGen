using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Enumerations;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        #region Fields

        private ModuleModel _Module;
        private ControllerViewModel _ControllerVM;
        private ObservableCollection<FaseCyclusModuleViewModel> _Fasen;

        #endregion // Fields

        #region Properties

        public ModuleModel Module
        {
            get { return _Module; }
        }

        public string Naam
        {
            get { return _Module.Naam; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _Module.Naam = value;
                }
                OnMonitoredPropertyChanged("Naam", _ControllerVM);
            }
        }

        public ObservableCollection<FaseCyclusModuleViewModel> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusModuleViewModel>();
                }
                return _Fasen;
            }
        }

        #endregion // Properties

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusModuleViewModel fcmvm in e.NewItems)
                {
                    _Module.Fasen.Add(fcmvm.Define);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusModuleViewModel fcmvm in e.OldItems)
                {
                    _Module.Fasen.Remove(fcmvm.Define);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Public Methods

        public void RemoveFase(FaseCyclusModuleViewModel fc)
        {
            FaseCyclusModuleViewModel _fc = null;
            foreach (FaseCyclusModuleViewModel fc1 in Fasen)
            {
                if(fc1.Define == fc.Define)
                {
                    _fc = fc1;
                }
            }
            if(_fc != null) Fasen.Remove(_fc);
        }

        #endregion // Public Methods

        #region Constructor

        public ModuleViewModel(ControllerViewModel controllervm, ModuleModel module)
        {
            _ControllerVM = controllervm;
            _Module = module;

            foreach(string fc in module.Fasen)
            {
                foreach(FaseCyclusViewModel fcvm in controllervm.Fasen)
                {
                    if(fcvm.Define == fc)
                    {
                        FaseCyclusModuleViewModel fcmvm = new FaseCyclusModuleViewModel(fcvm, this);
                        Fasen.Add(fcmvm);
                        break;
                    }
                }
            }
            Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
