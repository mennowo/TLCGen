using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Models.Settings;
using TLCGen.Utilities;

namespace TLCGen.DataAccess
{
    public static class SettingsProvider
    {
        private static TLCGenSettingsModel _Settings;
        public static TLCGenSettingsModel Settings
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
            }
        }

        #region Serialize Methods

        public static void LoadApplicationSettings()
        {
            string settingsfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TLCGenSettings.xml");
            if (File.Exists(settingsfile))
            {
                TLCGen.DataAccess.DeserializeT<TLCGenSettingsModel> deserializer = new TLCGen.DataAccess.DeserializeT<TLCGenSettingsModel>();
                _Settings = deserializer.DeSerialize(settingsfile);
            }
            if (_Settings == null)
                _Settings = new TLCGenSettingsModel();
        }

        public static void SaveApplicationSettings()
        {
            string settingsfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TLCGenSettings.xml");
            TLCGen.DataAccess.SerializeT<TLCGenSettingsModel> serializer = new TLCGen.DataAccess.SerializeT<TLCGenSettingsModel>();
            serializer.Serialize(settingsfile, _Settings);
        }

        #endregion Serialize Methods

        #region Settings Provider Methods

        public static string GetFaseCyclusDefinePrefix()
        {
            return Settings.DefaultControllerSettings.PreFixSettings.FaseCyclusDefinePrefix;
        }

        public static string GetDetectorDefinePrefix()
        {
            return Settings.DefaultControllerSettings.PreFixSettings.DetectorDefinePrefix;
        }

        public static void ApplyDefaultFaseCyclusSettings(FaseCyclusModel fcm, string define)
        {
            fcm.Type = FaseCyclusUtilities.GetFaseTypeFromDefine(define);
            ApplyDefaultFaseCyclusSettings(fcm, fcm.Type);
        }

        public static void ApplyDefaultFaseCyclusSettings(FaseCyclusModel fcm, FaseTypeEnum type)
        {
            int itype = 0;
            switch(type)
            {
                case FaseTypeEnum.Auto:
                    itype = 0;
                    break;
                case FaseTypeEnum.Fiets:
                    itype = 1;
                    break;
                case FaseTypeEnum.Voetganger:
                    itype = 2;
                    break;
                case FaseTypeEnum.OV:
                    itype = 3;
                    break;
            }
            fcm.Kopmax = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].Kopmax;
            fcm.TFG = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TFG;
            fcm.TGG = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TGG;
            fcm.TGG_min = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TGG_min;
            fcm.TGL = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TGL;
            fcm.TGL_min = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TGL_min;
            fcm.TRG = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TRG;
            fcm.TRG_min = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].TRG_min;
            fcm.VasteAanvraag = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].VasteAanvraag;
            fcm.Wachtgroen = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].Wachtgroen;
            fcm.Meeverlengen = Settings.DefaultFaseCyclusSettings.DefaultFasen[itype].Meeverlengen;
        }

        #endregion // Settings Provider Methods

        #region Constructor

        static SettingsProvider()
        {
        }

        #endregion // Constructor
    }
}
