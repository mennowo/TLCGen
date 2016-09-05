using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;
using TLCGen.Models;
using TLCGen.Extensions;

namespace TLCGen.ViewModels
{
    public class ModuleViewModel : ViewModelBase
    {
        #region Fields

        private ModuleModel _Module;
        private ControllerViewModel _ControllerVM;
        private ObservableCollection<ModuleFaseCyclusViewModel> _Fasen;

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

        public ObservableCollection<ModuleFaseCyclusViewModel> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<ModuleFaseCyclusViewModel>();
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
                foreach (ModuleFaseCyclusViewModel mfcvm in e.NewItems)
                {
                    _Module.Fasen.Add(mfcvm.ModuleFaseCyclus);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (ModuleFaseCyclusViewModel mfcvm in e.OldItems)
                {
                    _Module.Fasen.Remove(mfcvm.ModuleFaseCyclus);
                }
            }
            _ControllerVM.HasChanged = true;
        }

        #endregion // Collection Changed

        #region Public Methods

        public void RemoveFase(ModuleFaseCyclusViewModel fc)
        {
            ModuleFaseCyclusViewModel _fc = null;
            foreach (ModuleFaseCyclusViewModel fc1 in Fasen)
            {
                if(fc1.FaseCyclusDefine == fc.FaseCyclusDefine)
                {
                    _fc = fc1;
                }
            }
            if(_fc != null) Fasen.Remove(_fc);
        }

        public void RemoveFase(string fcdefine)
        {
            ModuleFaseCyclusViewModel _fc = null;
            foreach (ModuleFaseCyclusViewModel fc1 in Fasen)
            {
                if (fc1.FaseCyclusDefine == fcdefine)
                {
                    _fc = fc1;
                }
            }
            if (_fc != null) Fasen.Remove(_fc);
        }

        #endregion // Public Methods

        #region Constructor

        public ModuleViewModel(ControllerViewModel controllervm, ModuleModel module)
        {
            _ControllerVM = controllervm;
            _Module = module;

            foreach(ModuleFaseCyclusModel mfcm in module.Fasen)
            {
                // Create ViewModel
                ModuleFaseCyclusViewModel mfcvm = new ModuleFaseCyclusViewModel(_ControllerVM, mfcm);
                
                // Find the friendly name of the FaseCyclus
                foreach(FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
                {
                    if(fcvm.Define == mfcvm.FaseCyclusDefine)
                    {
                        mfcvm.FaseCyclusNaam = fcvm.Naam;
                        break;
                    }
                }

                // Add to list
                Fasen.Add(mfcvm);
            }
            Fasen.BubbleSort();
            Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
