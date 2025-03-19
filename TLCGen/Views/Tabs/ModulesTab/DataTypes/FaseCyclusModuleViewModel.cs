
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Requests;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// Used to disclose relevant information about a PhaseCyclus to the of the application
    /// that deals with modules.
    /// </summary>
    public class FaseCyclusModuleViewModel : ObservableObjectEx, IComparable
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
            get => _ModuleVM;
            set
            {
                _ModuleVM = value;
                OnPropertyChanged("ModuleVM");
                UpdateModuleInfo();
            }
        }

        public ModuleFaseCyclusViewModel ModuleFaseVM
        {
            get => _ModuleFaseVM;
            set
            {
                _ModuleFaseVM = value;
                OnPropertyChanged("ModuleFaseVM");
                UpdateModuleInfo();
            }
        }

        /// <summary>
        /// The name of the PhaseCyclus
        /// </summary>
        public string Naam => _FaseCyclus.Naam;

        /// <summary>
        /// Indicates if this phase can or cannot be added to the Module referenced by property ModuleVM
        /// </summary>
        public bool CanBeAdded
        {
            get
            {
                if (_ModuleVM != null)
                {
                    foreach (var mfcvm in _ModuleVM.Fasen)
                    {
                        var request = new IsFasenConflictingRequest(this.Naam, mfcvm.FaseCyclusNaam);
WeakReferenceMessenger.Default.Send(request);
                        if (request.Handled && request.IsConflicting)
                            return false;
                    }
                }
                if (_ModuleFaseVM != null)
                {
                    if (_ModuleFaseVM.FaseCyclusNaam == Naam) return false;
                    var request = new IsFasenConflictingRequest(this.Naam, _ModuleFaseVM.FaseCyclusNaam);
WeakReferenceMessenger.Default.Send(request);
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
                    foreach (var mfcvm in _ModuleVM.Fasen)
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
            OnPropertyChanged("CanBeAdded");
            OnPropertyChanged("IsIn");
            OnPropertyChanged("NothingAvailable"); 
            OnPropertyChanged("HasModule");
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
