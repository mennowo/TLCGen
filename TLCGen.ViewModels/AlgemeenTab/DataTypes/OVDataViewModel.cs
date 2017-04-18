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
                if(value == OVIngreepTypeEnum.Uitgebreid)
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Controller.OVData);
                }
                OnMonitoredPropertyChanged(null);
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
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

        [Description("Maximale wachttijd auto")]
        public int MaxWachttijdAuto
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdAuto; }
            set
            {
                _Controller.OVData.MaxWachttijdAuto = value;
                OnMonitoredPropertyChanged("MaxWachttijdAuto");
            }
        }

        [Description("Maximale wachttijd fiets")]
        public int MaxWachttijdFiets
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdFiets; }
            set
            {
                _Controller.OVData.MaxWachttijdFiets = value;
                OnMonitoredPropertyChanged("MaxWachttijdFiets");
            }
        }

        [Description("Maximale wachttijd voetganger")]
        public int MaxWachttijdVoetganger
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdVoetganger; }
            set
            {
                _Controller.OVData.MaxWachttijdVoetganger = value;
                OnMonitoredPropertyChanged("MaxWachttijdVoetganger");
            }
        }

        #endregion // Properties
    }
}
