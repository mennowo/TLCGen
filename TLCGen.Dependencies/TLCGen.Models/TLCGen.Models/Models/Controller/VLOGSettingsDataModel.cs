using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class VLOGSettingsDataModel
    {
        public bool VLOGToepassen { get; set; }

        public VLOGVersieEnum VLOGVersie { get; set; }

        public int MONTYPE_def { get; set; }
        public int MONDP_def { get; set; }
        public int MONIS_def { get; set; }
        public int MONFC_def { get; set; }
        public int MONUS_def { get; set; }
        public int MONDS_def { get; set; }
        public int LOGTYPE_def { get; set; }

        public int LOGTYPE_DATI { get; set; }
        public int LOGTYPE_DP { get; set; }
        public int LOGTYPE_IS { get; set; }
        public int LOGTYPE_FC { get; set; }
        public int LOGTYPE_US { get; set; }
        public int LOGTYPE_PS { get; set; }
        public int LOGTYPE_DS { get; set; }
        public int LOGTYPE_MLX { get; set; }
        public int LOGTYPE_OMG { get; set; }
        public int LOGTYPE_CRC { get; set; }
        public int LOGTYPE_CFG { get; set; }
        public int LOGPRM_EVENT { get; set; }

        public int MONTYPE_DATI { get; set; }
        public int MONTYPE_DP { get; set; }
        public int MONTYPE_IS { get; set; }
        public int MONTYPE_FC { get; set; }
        public int MONTYPE_US { get; set; }
        public int MONTYPE_PS { get; set; }
        public int MONTYPE_DS { get; set; }
        public int MONTYPE_MLX { get; set; }
        public int MONTYPE_OMG { get; set; }
        public int MONTYPE_CRC { get; set; }
        public int MONTYPE_CFG { get; set; }
        public int MONPRM_EVENT { get; set; }

        public VLOGLogModeEnum LOGPRM_VLOGMODE { get; set; }
        
        public VLOGMonModeEnum MONPRM_VLOGMODE { get; set; }

    }
}
