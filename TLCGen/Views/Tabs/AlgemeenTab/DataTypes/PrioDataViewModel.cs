using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.Controls;
using TLCGen.ModelManagement;

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
            get => _Controller?.PrioData.PrioIngreepType ?? PrioIngreepTypeEnum.Geen;
            set
            {
                if (_Controller.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen && value != PrioIngreepTypeEnum.Geen)
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(_Controller.PrioData);
                }
                _Controller.PrioData.PrioIngreepType = value;
                Messenger.Default.Send(new ControllerHasOVChangedMessage(value));
                Messenger.Default.Send(new UpdateTabsEnabledMessage());
                RaisePropertyChanged<object>(nameof(PrioIngreepType), broadcast: true);
                RaisePropertyChanged(nameof(HasPrio));
            }
        }

        [Description("Enkele uitgang prio actief per fase")]
        public bool PrioUitgangPerFase
        {
            get => _Controller?.PrioData.PrioUitgangPerFase ?? false;
            set
            {
                _Controller.PrioData.PrioUitgangPerFase = value;
                RaisePropertyChanged<object>(nameof(PrioUitgangPerFase), broadcast: true);

                TLCGenModelManager.Default.SetSpecialIOPerSignalGroup(_Controller);
            }
        }

        [Browsable(false)]
        public bool HasPrio => PrioIngreepType != PrioIngreepTypeEnum.Geen;

        [Category("Opties prioriteit")]
        [Description("Check type op DSI bericht bij VECOM")]
        public bool CheckOpDSIN
        {
            get => _Controller?.PrioData.CheckOpDSIN ?? false;
            set
            {
                _Controller.PrioData.CheckOpDSIN = value;
                RaisePropertyChanged<object>(nameof(CheckOpDSIN), broadcast: true);
            }
        }

        [Description("Maximale wachttijd auto")]
        public int MaxWachttijdAuto
        {
            get => _Controller?.PrioData.MaxWachttijdAuto ?? 0;
            set
            {
                _Controller.PrioData.MaxWachttijdAuto = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdAuto), broadcast: true);
            }
        }

        [Description("Maximale wachttijd fiets")]
        public int MaxWachttijdFiets
        {
            get => _Controller?.PrioData.MaxWachttijdFiets ?? 0;
            set
            {
                _Controller.PrioData.MaxWachttijdFiets = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdFiets), broadcast: true);
            }
        }

        [Description("Maximale wachttijd voetganger")]
        public int MaxWachttijdVoetganger
        {
            get => _Controller?.PrioData.MaxWachttijdVoetganger ?? 0;
            set
            {
                _Controller.PrioData.MaxWachttijdVoetganger = value;
                RaisePropertyChanged<object>(nameof(MaxWachttijdVoetganger), broadcast: true);
            }
        }

        [Description("Grens te vroeg tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeVroeg
        {
            get => _Controller?.PrioData.GeconditioneerdePrioGrensTeVroeg ?? 0;
            set
            {
                _Controller.PrioData.GeconditioneerdePrioGrensTeVroeg = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeVroeg), broadcast: true);
            }
        }

        [Description("Grens te laat tbv geconditioneerde prio")]
        public int GeconditioneerdePrioGrensTeLaat
        {
            get => _Controller?.PrioData.GeconditioneerdePrioGrensTeLaat ?? 0;
            set
            {
                _Controller.PrioData.GeconditioneerdePrioGrensTeLaat = value;
                RaisePropertyChanged<object>(nameof(GeconditioneerdePrioGrensTeLaat), broadcast: true);
            }
        }

        [Description("Blokkeren niet-conflicten tijdens HD ingreep")]
        public bool BlokkeerNietConflictenBijHDIngreep
        {
            get => _Controller?.PrioData.BlokkeerNietConflictenBijHDIngreep ?? false;
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
            get => _Controller?.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer ?? false;
            set
            {
                _Controller.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer = value;
                RaisePropertyChanged<object>(nameof(BlokkeerNietConflictenAlleenLangzaamVerkeer), broadcast: true);
            }
        }

        [Description("Verklikken wijziging prio teller via UBER")]
        public NooitAltijdAanUitEnum VerklikkenPrioTellerUber
        {
            get => _Controller?.PrioData.VerklikkenPrioTellerUber ?? NooitAltijdAanUitEnum.Nooit;
            set
            {
                _Controller.PrioData.VerklikkenPrioTellerUber = value;
                RaisePropertyChanged<object>(nameof(VerklikkenPrioTellerUber), broadcast: true);
            }
        }

        [Description("Check signaalgroepnmrs hoger dan 2## als ## in DSI")]
        public bool VerlaagHogeSignaalGroepNummers
        {
            get => _Controller?.PrioData.VerlaagHogeSignaalGroepNummers ?? false;
            set
            {
                _Controller.PrioData.VerlaagHogeSignaalGroepNummers = value;
                RaisePropertyChanged<object>(nameof(VerlaagHogeSignaalGroepNummers), broadcast: true);
            }
        }

        [Description("KAR signaalgroep nummers in PRMs")]
        public bool KARSignaalGroepNummersInParameters
        {
            get => _Controller?.PrioData.KARSignaalGroepNummersInParameters ?? false;
            set
            {
                _Controller.PrioData.KARSignaalGroepNummersInParameters = value;
                RaisePropertyChanged<object>(nameof(KARSignaalGroepNummersInParameters), broadcast: true);
            }
        }

        #endregion // Properties
    }
}
