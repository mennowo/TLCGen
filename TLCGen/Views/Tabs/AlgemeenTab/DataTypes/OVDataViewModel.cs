using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class OVDataViewModel : ViewModelBase
    {
        #region Properties

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            set
            {
                _Controller = value;
                RaisePropertyChanged(null);
            }
        }

        [Browsable(false)]
        [Description("Type OV ingreep")]
        public OVIngreepTypeEnum OVIngreepType
        {
            get { return _Controller == null ? OVIngreepTypeEnum.Geen : _Controller.OVData.OVIngreepType; }
            set
            {
                _Controller.OVData.OVIngreepType = value;
                if(value == OVIngreepTypeEnum.Uitgebreid)
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Controller.OVData);
                }
                RaisePropertyChanged<object>(null, broadcast: true);
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
            }
        }

        [Browsable(false)]
        public bool HasOV
        {
            get { return OVIngreepType != OVIngreepTypeEnum.Geen; }
        }

        [Category("Opties OV")]
        [Description("Regeling heeft DSI")]
        public bool DSI
        {
            get { return _Controller == null ? false : _Controller.OVData.DSI; }
            set
            {
                _Controller.OVData.DSI = value;
                RaisePropertyChanged<object>("DSI", broadcast: true);
            }
        }

        [Description("Check op DSIN")]
        public bool CheckOpDSIN
        {
            get { return _Controller == null ? false : _Controller.OVData.CheckOpDSIN; }
            set
            {
                _Controller.OVData.CheckOpDSIN = value;
                RaisePropertyChanged<object>("CheckOpDSIN", broadcast: true);
            }
        }

        [Description("Maximale wachttijd auto")]
        public int MaxWachttijdAuto
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdAuto; }
            set
            {
                _Controller.OVData.MaxWachttijdAuto = value;
                RaisePropertyChanged<object>("MaxWachttijdAuto", broadcast: true);
            }
        }

        [Description("Maximale wachttijd fiets")]
        public int MaxWachttijdFiets
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdFiets; }
            set
            {
                _Controller.OVData.MaxWachttijdFiets = value;
                RaisePropertyChanged<object>("MaxWachttijdFiets", broadcast: true);
            }
        }

        [Description("Maximale wachttijd voetganger")]
        public int MaxWachttijdVoetganger
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdVoetganger; }
            set
            {
                _Controller.OVData.MaxWachttijdVoetganger = value;
                RaisePropertyChanged<object>("MaxWachttijdVoetganger", broadcast: true);
            }
        }

        #endregion // Properties
    }
}
