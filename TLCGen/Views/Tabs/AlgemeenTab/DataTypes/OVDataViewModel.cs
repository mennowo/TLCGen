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
    public class PrioDataViewModel : ViewModelBase
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
        public PrioIngreepTypeEnum PrioIngreepType
        {
            get { return _Controller == null ? PrioIngreepTypeEnum.Geen : _Controller.PrioData.PrioIngreepType; }
            set
            {
                if (_Controller.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen && value != PrioIngreepTypeEnum.Geen)
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Controller.PrioData);
                }
                _Controller.PrioData.PrioIngreepType = value;
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged("");
            }
        }

        [Browsable(false)]
        public bool HasPrio
        {
            get { return PrioIngreepType != PrioIngreepTypeEnum.Geen; }
        }

        [Category("Opties prioriteit")]
        [Description("Check type op DSI bericht bij VECOM")]
        public bool CheckOpDSIN
        {
            get { return _Controller == null ? false : _Controller.PrioData.CheckOpDSIN; }
            set
            {
                _Controller.PrioData.CheckOpDSIN = value;
                RaisePropertyChanged<object>(nameof(CheckOpDSIN), broadcast: true);
            }
        }

        [Description("Maximale wachttijd auto")]
        public int MaxWachttijdAuto
        {
            get { return _Controller == null ? 0 : _Controller.PrioData.MaxWachttijdAuto; }
            set
            {
                _Controller.PrioData.MaxWachttijdAuto = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdAuto), broadcast: true);
            }
        }

        [Description("Maximale wachttijd fiets")]
        public int MaxWachttijdFiets
        {
            get { return _Controller == null ? 0 : _Controller.PrioData.MaxWachttijdFiets; }
            set
            {
                _Controller.PrioData.MaxWachttijdFiets = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdFiets), broadcast: true);
            }
        }

        [Description("Maximale wachttijd voetganger")]
        public int MaxWachttijdVoetganger
        {
            get { return _Controller == null ? 0 : _Controller.PrioData.MaxWachttijdVoetganger; }
            set
            {
                _Controller.PrioData.MaxWachttijdVoetganger = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdVoetganger), broadcast: true);
            }
        }

        [Description("Grens te vroeg tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeVroeg
        {
            get { return _Controller == null ? 0 : _Controller.PrioData.GeconditioneerdePrioGrensTeVroeg; }
            set
            {
                _Controller.PrioData.GeconditioneerdePrioGrensTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeVroeg), broadcast: true);
            }
        }

        [Description("Grens te laat tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeLaat
        {
            get { return _Controller == null ? 0 : _Controller.PrioData.GeconditioneerdePrioGrensTeLaat; }
            set
            {
                _Controller.PrioData.GeconditioneerdePrioGrensTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeLaat), broadcast: true);
            }
        }

        [Description("Blokkeren niet-conflicten tijdens HD ingreep")]
        public bool BlokkeerNietConflictenBijHDIngreep
        {
            get { return _Controller == null ? false : _Controller.PrioData.BlokkeerNietConflictenBijHDIngreep; }
            set
            {
                _Controller.PrioData.BlokkeerNietConflictenBijHDIngreep = value;
                RaisePropertyChanged<object>(nameof(BlokkeerNietConflictenBijHDIngreep), broadcast: true);
            }
        }

        [Description("Blokkeren niet-conflicten geldt alleen voor langzaam verkeer")]
        [EnabledCondition(nameof(BlokkeerNietConflictenBijHDIngreep))]
        public bool BlokkeerNietConflictenAlleenLangzaamVerkeer
        {
            get { return _Controller == null ? false : _Controller.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer; }
            set
            {
                _Controller.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer = value;
                RaisePropertyChanged<object>(nameof(BlokkeerNietConflictenAlleenLangzaamVerkeer), broadcast: true);
            }
        }

        [Description("Verklikken wijziging prio teller via UBER")]
        public NooitAltijdAanUitEnum VerklikkenPrioTellerUber
        {
            get { return _Controller == null ? NooitAltijdAanUitEnum.Nooit : _Controller.PrioData.VerklikkenPrioTellerUber; }
            set
            {
                _Controller.PrioData.VerklikkenPrioTellerUber = value;
                RaisePropertyChanged<object>(nameof(VerklikkenPrioTellerUber), broadcast: true);
            }
        }

        [Description("Check signaalgroepnmrs hoger dan 2## als ## in DSI")]
        public bool VerlaagHogeSignaalGroepNummers
        {
            get { return _Controller == null ? false : _Controller.PrioData.VerlaagHogeSignaalGroepNummers; }
            set
            {
                _Controller.PrioData.VerlaagHogeSignaalGroepNummers = value;
                RaisePropertyChanged<object>(nameof(VerlaagHogeSignaalGroepNummers), broadcast: true);
            }
        }

        #endregion // Properties
    }
}
