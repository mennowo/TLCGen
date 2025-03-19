using System.ComponentModel;
using TLCGen.Controls;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class VLOGSettingsDataViewModel : ObservableObjectEx
    {
        public readonly VLOGSettingsDataModel VLOGSettingsData;

        [Description("VLOG toepassen")]
        public bool VLOGToepassen
        {
            get => VLOGSettingsData.VLOGToepassen;
            set
            {
                if (value) Settings.DefaultsProvider.Default.SetDefaultsOnModel(VLOGSettingsData);
                VLOGSettingsData.VLOGToepassen = value;
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged("");
            }
        }


        [Category("Modes")]
        [Description("Logging mode")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public VLOGLogModeEnum LOGPRM_VLOGMODE
        {
            get => VLOGSettingsData.LOGPRM_VLOGMODE;
            set
            {
                VLOGSettingsData.LOGPRM_VLOGMODE = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Monitoring mode")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public VLOGMonModeEnum MONPRM_VLOGMODE
        {
            get => VLOGSettingsData.MONPRM_VLOGMODE;
            set
            {
                VLOGSettingsData.MONPRM_VLOGMODE = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Category("Defaults")]
        [Description("Monitoring type default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_def
        {
            get => VLOGSettingsData.MONTYPE_def;
            set
            {
                VLOGSettingsData.MONTYPE_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Logging type default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_def
        {
            get => VLOGSettingsData.LOGTYPE_def;
            set
            {
                VLOGSettingsData.LOGTYPE_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MONDP default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONDP_def
        {
            get => VLOGSettingsData.MONDP_def;
            set
            {
                VLOGSettingsData.MONDP_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MONIS default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONIS_def
        {
            get => VLOGSettingsData.MONIS_def;
            set
            {
                VLOGSettingsData.MONIS_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MONFC default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONFC_def
        {
            get => VLOGSettingsData.MONFC_def;
            set
            {
                VLOGSettingsData.MONFC_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MONUS default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONUS_def
        {
            get => VLOGSettingsData.MONUS_def;
            set
            {
                VLOGSettingsData.MONUS_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MONDS default")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONDS_def
        {
            get => VLOGSettingsData.MONDS_def;
            set
            {
                VLOGSettingsData.MONDS_def = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Category("Logging")]
        [Description("DATI")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_DATI
        {
            get => VLOGSettingsData.LOGTYPE_DATI;
            set
            {
                VLOGSettingsData.LOGTYPE_DATI = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Detectoren")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_DP
        {
            get => VLOGSettingsData.LOGTYPE_DP;
            set
            {
                VLOGSettingsData.LOGTYPE_DP = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Ingangen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_IS
        {
            get => VLOGSettingsData.LOGTYPE_IS;
            set
            {
                VLOGSettingsData.LOGTYPE_IS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Fasen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_FC
        {
            get => VLOGSettingsData.LOGTYPE_FC;
            set
            {
                VLOGSettingsData.LOGTYPE_FC = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Uitgangen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_US
        {
            get => VLOGSettingsData.LOGTYPE_US;
            set
            {
                VLOGSettingsData.LOGTYPE_US = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("PS")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_PS
        {
            get => VLOGSettingsData.LOGTYPE_PS;
            set
            {
                VLOGSettingsData.LOGTYPE_PS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Selectieve detectoren")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_DS
        {
            get => VLOGSettingsData.LOGTYPE_DS;
            set
            {
                VLOGSettingsData.LOGTYPE_DS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MLX")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_MLX
        {
            get => VLOGSettingsData.LOGTYPE_MLX;
            set
            {
                VLOGSettingsData.LOGTYPE_MLX = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("OMG")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_OMG
        {
            get => VLOGSettingsData.LOGTYPE_OMG;
            set
            {
                VLOGSettingsData.LOGTYPE_OMG = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("CRC")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_CRC
        {
            get => VLOGSettingsData.LOGTYPE_CRC;
            set
            {
                VLOGSettingsData.LOGTYPE_CRC = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("CFG")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int LOGTYPE_CFG
        {
            get => VLOGSettingsData.LOGTYPE_CFG;
            set
            {
                VLOGSettingsData.LOGTYPE_CFG = value;
                OnPropertyChanged(broadcast: true);
            }
        }


        [Category("Monitoring (realtime)")]
        [Description("DATI")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_DATI
        {
            get => VLOGSettingsData.MONTYPE_DATI;
            set
            {
                VLOGSettingsData.MONTYPE_DATI = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Detectoren")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_DP
        {
            get => VLOGSettingsData.MONTYPE_DP;
            set
            {
                VLOGSettingsData.MONTYPE_DP = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Ingangen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_IS
        {
            get => VLOGSettingsData.MONTYPE_IS;
            set
            {
                VLOGSettingsData.MONTYPE_IS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Fasen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_FC
        {
            get => VLOGSettingsData.MONTYPE_FC;
            set
            {
                VLOGSettingsData.MONTYPE_FC = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Uitgangen")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_US
        {
            get => VLOGSettingsData.MONTYPE_US;
            set
            {
                VLOGSettingsData.MONTYPE_US = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("PS")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_PS
        {
            get => VLOGSettingsData.MONTYPE_PS;
            set
            {
                VLOGSettingsData.MONTYPE_PS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("Selectieve detetoren")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_DS
        {
            get => VLOGSettingsData.MONTYPE_DS;
            set
            {
                VLOGSettingsData.MONTYPE_DS = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("MLX")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_MLX
        {
            get => VLOGSettingsData.MONTYPE_MLX;
            set
            {
                VLOGSettingsData.MONTYPE_MLX = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("OMG")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_OMG
        {
            get => VLOGSettingsData.MONTYPE_OMG;
            set
            {
                VLOGSettingsData.MONTYPE_OMG = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("CRC")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_CRC
        {
            get => VLOGSettingsData.MONTYPE_CRC;
            set
            {
                VLOGSettingsData.MONTYPE_CRC = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("CFG")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONTYPE_CFG
        {
            get => VLOGSettingsData.MONTYPE_CFG;
            set
            {
                VLOGSettingsData.MONTYPE_CFG = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        [Description("EVENT")]
        [EnabledCondition(nameof(VLOGToepassen))]
        public int MONPRM_EVENT
        {
            get => VLOGSettingsData.MONPRM_EVENT;
            set
            {
                VLOGSettingsData.MONPRM_EVENT = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #region Constructor

        public VLOGSettingsDataViewModel(VLOGSettingsDataModel vLOGSettingsData)
        {
            VLOGSettingsData = vLOGSettingsData;
        }

        #endregion // Constructor
    }
}
