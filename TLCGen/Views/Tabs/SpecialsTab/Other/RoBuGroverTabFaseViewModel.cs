
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverTabFaseViewModel : ObservableObject
    {
        #region Fields

        private string _FaseCyclusNaam;
        private RoBuGroverConflictGroepModel _SelectedConflictGroep;
        private bool _CheckConflicts;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// The name of the PhaseCyclus
        /// </summary>
        public string FaseCyclusNaam => _FaseCyclusNaam;

        /// <summary>
        /// Indicates if this phase can or cannot be added to the Module referenced by property ModuleVM
        /// </summary>
        public bool CanBeAddedToConflictGroep
        {
            get
            {
                if (_SelectedConflictGroep != null)
                {
                    if (!_CheckConflicts)
                        return true;

                    foreach (var fc in _SelectedConflictGroep.Fasen)
                    {
                        var request = new IsFasenConflictingRequest(FaseCyclusNaam, fc.FaseCyclus);
WeakReferenceMessengerEx.Default.Send(request);
                        if (request.Handled && !request.IsConflicting)
                            return false;
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Indicates if this phase is or is not in the Module referenced by property ModuleVM
        /// </summary>
        public bool IsInConflictGroep
        {
            get
            {
                if (_SelectedConflictGroep != null)
                {
                    foreach (var fc in _SelectedConflictGroep.Fasen)
                    {
                        if (fc.FaseCyclus == FaseCyclusNaam)
                            return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Indicates that the property ModuleVM has no value
        /// </summary>
        public bool NoConflictGroepAvailable => _SelectedConflictGroep == null;

        #endregion // Properties

        #region Collection Changed

        #endregion // Collection Changed

        #region Overrides

        public override string ToString()
        {
            return FaseCyclusNaam;
        }

        #endregion // Overrides

        #region Public methods

        /// <summary>
        /// Instructs the view that info has changed
        /// </summary>
        public void UpdateConflictGroepInfo()
        {
            OnPropertyChanged("CanBeAddedToConflictGroep");
            OnPropertyChanged("IsInConflictGroep");
            OnPropertyChanged("NoConflictGroepAvailable");
        }

        #endregion // Public methods

        #region TLCGen Message Handling

        private void OnSelectedConflictGroepChanged(object sender, SelectedConflictGroepChangedMessage message)
        {
            _SelectedConflictGroep = message.NewGroep;
            _CheckConflicts = message.NewGroupCheckConflicts;
            UpdateConflictGroepInfo();
        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public RoBuGroverTabFaseViewModel(string fasenaam)
        {
            _FaseCyclusNaam = fasenaam;

            WeakReferenceMessengerEx.Default.Register<SelectedConflictGroepChangedMessage>(this, OnSelectedConflictGroepChanged);
        }

        #endregion // Constructor
    }
}
