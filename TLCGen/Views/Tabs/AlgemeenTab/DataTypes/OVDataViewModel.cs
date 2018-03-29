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
                RaisePropertyChanged("");
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
                RaisePropertyChanged<object>(nameof(OVIngreepType), broadcast: true);
                RaisePropertyChanged("");
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
        [Description("Check type op DSI bericht bij VECOM")]
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
