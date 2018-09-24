using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
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
                RaisePropertyChanged("");
            }
        }

        public bool ToevoegenOVM
        {
            get { return _Specials.ToevoegenOVM; }
            set
            {
                _Specials.ToevoegenOVM = value;
                MessengerInstance.Send(new ControllerDataChangedMessage());
                RaisePropertyChanged<object>("ToevoegenOVM", null, null, true);
            }
        }

        public bool PrmLoggingTfbMax
        {
            get { return _Specials.PrmLoggingTfbMax; }
            set
            {
                _Specials.PrmLoggingTfbMax = value;
                MessengerInstance.Send(new ControllerDataChangedMessage());
                RaisePropertyChanged<object>("PrmLoggingTfbMax", null, null, true);
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
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
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

        public SpecialsRotterdamViewModel()
        {
            
        }

        public SpecialsRotterdamViewModel(IMessenger messenger = null) : base(messenger)
        {

        }

        #endregion // Constructor
    }
}
