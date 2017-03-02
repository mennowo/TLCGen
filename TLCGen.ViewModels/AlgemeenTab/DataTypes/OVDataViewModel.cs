using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

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
                OnPropertyChanged(null);
            }
        }

        [Category("Opties OV")]
        [Description("Type OV ingreep")]
        public OVIngreepTypeEnum OVIngreepType
        {
            get { return _Controller == null ? OVIngreepTypeEnum.Geen : _Controller.OVData.OVIngreepType; }
            set
            {
                _Controller.OVData.OVIngreepType = value;
                OnMonitoredPropertyChanged("OVIngreepType");
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
            }
        }

        [Description("Regeling heeft DSI")]
        public bool DSI
        {
            get { return _Controller == null ? false : _Controller.OVData.DSI; }
            set
            {
                _Controller.OVData.DSI = value;
                OnMonitoredPropertyChanged("DSI");
            }
        }

        [Description("Check op DSIN")]
        public bool CheckOpDSIN
        {
            get { return _Controller == null ? false : _Controller.OVData.CheckOpDSIN; }
            set
            {
                _Controller.OVData.CheckOpDSIN = value;
                OnMonitoredPropertyChanged("CheckOpDSIN");
            }
        }

        #endregion // Properties
    }
}
