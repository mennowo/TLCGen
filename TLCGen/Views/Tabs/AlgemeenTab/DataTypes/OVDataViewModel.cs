using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.Controls;

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
        [Description("Type prioriteit")]
        public OVIngreepTypeEnum OVIngreepType
        {
            get { return _Controller == null ? OVIngreepTypeEnum.Geen : _Controller.OVData.OVIngreepType; }
            set
            {
                if (_Controller.OVData.OVIngreepType == OVIngreepTypeEnum.Geen && value != OVIngreepTypeEnum.Geen)
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Controller.OVData);
                }
                _Controller.OVData.OVIngreepType = value;
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged("");
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
                RaisePropertyChanged<object>(nameof(CheckOpDSIN), broadcast: true);
            }
        }

        [Description("Maximale wachttijd auto")]
        public int MaxWachttijdAuto
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdAuto; }
            set
            {
                _Controller.OVData.MaxWachttijdAuto = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdAuto), broadcast: true);
            }
        }

        [Description("Maximale wachttijd fiets")]
        public int MaxWachttijdFiets
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdFiets; }
            set
            {
                _Controller.OVData.MaxWachttijdFiets = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdFiets), broadcast: true);
            }
        }

        [Description("Maximale wachttijd voetganger")]
        public int MaxWachttijdVoetganger
        {
            get { return _Controller == null ? 0 : _Controller.OVData.MaxWachttijdVoetganger; }
            set
            {
                _Controller.OVData.MaxWachttijdVoetganger = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdVoetganger), broadcast: true);
            }
        }

        [Description("Grens te vroeg tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeVroeg
        {
            get { return _Controller == null ? 0 : _Controller.OVData.GeconditioneerdePrioGrensTeVroeg; }
            set
            {
                _Controller.OVData.GeconditioneerdePrioGrensTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeVroeg), broadcast: true);
            }
        }

        [Description("Grens te laat tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeLaat
        {
            get { return _Controller == null ? 0 : _Controller.OVData.GeconditioneerdePrioGrensTeLaat; }
            set
            {
                _Controller.OVData.GeconditioneerdePrioGrensTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeLaat), broadcast: true);
            }
        }

        [Description("Blokkeren niet-conflicten tijdens HD ingreep")]
        public bool BlokkeerNietConflictenBijHDIngreep
        {
            get { return _Controller == null ? false : _Controller.OVData.BlokkeerNietConflictenBijHDIngreep; }
            set
            {
                _Controller.OVData.BlokkeerNietConflictenBijHDIngreep = value;
                RaisePropertyChanged<object>(nameof(BlokkeerNietConflictenBijHDIngreep), broadcast: true);
            }
        }

        [Description("Blokkeren niet-conflicten geldt alleen voor langzaam verkeer")]
        [EnabledCondition(nameof(BlokkeerNietConflictenBijHDIngreep))]
        public bool BlokkeerNietConflictenAlleenLangzaamVerkeer
        {
            get { return _Controller == null ? false : _Controller.OVData.BlokkeerNietConflictenAlleenLangzaamVerkeer; }
            set
            {
                _Controller.OVData.BlokkeerNietConflictenAlleenLangzaamVerkeer = value;
                RaisePropertyChanged<object>(nameof(BlokkeerNietConflictenAlleenLangzaamVerkeer), broadcast: true);
            }
        }

        [Description("Verklikken wijzing OV teller via UBER")]
        public NooitAltijdAanUitEnum VerklikkenOVTellerUber
        {
            get { return _Controller == null ? NooitAltijdAanUitEnum.Nooit : _Controller.OVData.VerklikkenOVTellerUber; }
            set
            {
                _Controller.OVData.VerklikkenOVTellerUber = value;
                RaisePropertyChanged<object>(nameof(VerklikkenOVTellerUber), broadcast: true);
            }
        }

        [Description("Check signaalgroepnmrs hoger dan 2## als ## in DSI")]
        public bool VerlaagHogeSignaalGroepNummers
        {
            get { return _Controller == null ? false : _Controller.OVData.VerlaagHogeSignaalGroepNummers; }
            set
            {
                _Controller.OVData.VerlaagHogeSignaalGroepNummers = value;
                RaisePropertyChanged<object>(nameof(VerlaagHogeSignaalGroepNummers), broadcast: true);
            }
        }

        #endregion // Properties
    }
}
