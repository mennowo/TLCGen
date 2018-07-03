using GalaSoft.MvvmLight.Messaging;
using System;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Requests;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// Used to disclose relevant information about a PhaseCyclus to the of the application
    /// that deals with modules.
    /// </summary>
    public class FaseCyclusModuleViewModel : ViewModelBase, IComparable
    {
        #region Fields

        private FaseCyclusModel _FaseCyclus;
        private ModuleViewModel _ModuleVM;
        private ModuleFaseCyclusViewModel _ModuleFaseVM;

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

        public ModuleFaseCyclusViewModel ModuleFaseVM
        {
            get { return _ModuleFaseVM; }
            set
            {
                _ModuleFaseVM = value;
                RaisePropertyChanged("ModuleFaseVM");
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
        public bool CanBeAdded
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
                if (_ModuleFaseVM != null)
                {
                    if (_ModuleFaseVM.FaseCyclusNaam == Naam) return false;
                    IsFasenConflictingRequest request = new IsFasenConflictingRequest(this.Naam, _ModuleFaseVM.FaseCyclusNaam);
                    Messenger.Default.Send(request);
                    if (request.Handled && request.IsConflicting)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Indicates if this phase is or is not in the Module referenced by property ModuleVM
        /// </summary>
        public bool IsIn
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
                    return false;
                }
                if (_ModuleFaseVM != null)
                {
                    foreach (var afcvm in _ModuleFaseVM.Alternatieven)
                    {
                        if (afcvm.FaseCyclus == this.Naam)
                            return true;
                    }
                    return false;
                }
                return false;
            }
        }
        
        public bool HasModule { get; set; }

        /// <summary>
        /// Indicates that the property ModuleVM has no value
        /// </summary>
        public bool NothingAvailable => _ModuleVM == null && _ModuleFaseVM == null;

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Overrides

        public override string ToString()
        {
            return Naam;
        }

        public int CompareTo(object obj)
        {
            return string.Compare(Naam, ((FaseCyclusModuleViewModel) obj).Naam, StringComparison.Ordinal);
        }

        #endregion // Overrides

        #region Public methods

        /// <summary>
        /// Instructs the view that info has changed
        /// </summary>
        public void UpdateModuleInfo()
        {
            RaisePropertyChanged("CanBeAdded");
            RaisePropertyChanged("IsIn");
            RaisePropertyChanged("NothingAvailable"); 
            RaisePropertyChanged("HasModule");
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
