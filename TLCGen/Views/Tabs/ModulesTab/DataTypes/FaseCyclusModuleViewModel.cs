using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// Used to disclose relevant information about a PhaseCyclus to the of the application
    /// that deals with modules.
    /// </summary>
    public class FaseCyclusModuleViewModel : ViewModelBase
    {
        #region Fields

        private FaseCyclusModel _FaseCyclus;
        private ModuleViewModel _ModuleVM;
        
        #endregion // Fields

        #region Properties

        /// <summary>
        /// A reference to the currently selected module. When this value is set, the method
        /// UpdateModuleInfo causes the view to update the information of this class, thus
        /// determining if the class is in the module, or can be added.
        /// </summary>
        public ModuleViewModel ModuleVM
        {
            get { return _ModuleVM; }
            set
            {
                _ModuleVM = value;
                RaisePropertyChanged("ModuleVM");
                UpdateModuleInfo();
            }
        }

        /// <summary>
        /// The name of the PhaseCyclus
        /// </summary>
        public string Naam
        {
            get { return _FaseCyclus.Naam; }
        }

        /// <summary>
        /// Indicates if this phase can or cannot be added to the Module referenced by property ModuleVM
        /// </summary>
        public bool CanBeAddedToModule
        {
            get
            {
                if (_ModuleVM != null)
                {
                    foreach (ModuleFaseCyclusViewModel mfcvm in _ModuleVM.Fasen)
                    {
                        IsFasenConflictingRequest request = new IsFasenConflictingRequest(this.Naam, mfcvm.FaseCyclusNaam);
                        Messenger.Default.Send(request);
                        if (request.Handled && request.IsConflicting)
                            return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Indicates if this phase is or is not in the Module referenced by property ModuleVM
        /// </summary>
        public bool IsInModule
        {
            get
            {
                if (_ModuleVM != null)
                {
                    foreach (ModuleFaseCyclusViewModel mfcvm in _ModuleVM.Fasen)
                    {
                        if (mfcvm.FaseCyclusNaam == this.Naam)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Indicates that the property ModuleVM has no value
        /// </summary>
        public bool NoModuleAvailable
        {
            get { return _ModuleVM == null; }
        }

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Overrides

        public override string ToString()
        {
            return Naam;
        }

        #endregion // Overrides

        #region Public methods

        /// <summary>
        /// Instructs the view that info has changed
        /// </summary>
        public void UpdateModuleInfo()
        {
            RaisePropertyChanged("CanBeAddedToModule");
            RaisePropertyChanged("IsInModule");
            RaisePropertyChanged("NoModuleAvailable"); 
        }

        #endregion // Public methods

        #region Constructor

        public FaseCyclusModuleViewModel(FaseCyclusModel fcm, ModuleViewModel mvm)
        {
            _FaseCyclus = fcm;
            _ModuleVM = mvm;
        }

        #endregion // Constructor
    }
}
