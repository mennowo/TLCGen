using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;

namespace TLCGen.SpecialsRotterdam
{
    internal class SpecialsRotterdamViewModel : ViewModelBase
    {
        #region Fields

        SpecialsRotterdamModel _Specials;

        #endregion // Fields

        #region Properties

        public SpecialsRotterdamModel Specials
        {
            get { return _Specials; }
            set
            {
                _Specials = value;
                RaisePropertyChanged(null);
            }
        }

        public bool ToepassenAFM
        {
            get { return _Specials.ToepassenAFM; }
            set
            {
                _Specials.ToepassenAFM = value;
                RaisePropertyChanged("ToepassenAFM");
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command Functionality
        
        #endregion // Command Functionality

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
        }

        #endregion // Public Methods

        #region Private Methods

        #endregion // Private Methods

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {

        }

        #endregion // TLCGen Events

        #region Constructor
        #endregion // Constructor
    }
}
