using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RoBuGroverConflictGroepViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private RoBuGroverConflictGroepModel _ConflictGroep;
        private RoBuGroverConflictGroepFaseViewModel _SelectedFase;

        #endregion // Fields

        #region Properties

        public RoBuGroverConflictGroepModel ConflictGroep
        {
            get { return _ConflictGroep; }
        }

        public ObservableCollectionAroundList<RoBuGroverConflictGroepFaseViewModel, RoBuGroverConflictGroepFaseModel> Fasen
        {
            get;
            private set;
        }

        public RoBuGroverConflictGroepFaseViewModel SelectedFase
        {
            get { return _SelectedFase; }
            set
            {
                _SelectedFase = value;
                OnMonitoredPropertyChanged("SelectedFase");
            }
        }

        #endregion Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public Methods

        #region IViewModelWithItem

        public object GetItem()
        {
            return _ConflictGroep;
        }

        #endregion // IViewModelWithItem

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            Fasen.Rebuild();
        }

        #endregion // TLCGen Events

        #region Constructor

        public RoBuGroverConflictGroepViewModel(RoBuGroverConflictGroepModel conflictgroep)
        {
            _ConflictGroep = conflictgroep;
            Fasen = new ObservableCollectionAroundList<RoBuGroverConflictGroepFaseViewModel, RoBuGroverConflictGroepFaseModel>(_ConflictGroep.Fasen);
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
        }

        #endregion // Constructor

    }
}
