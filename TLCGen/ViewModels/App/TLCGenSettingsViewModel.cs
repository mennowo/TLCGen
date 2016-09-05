using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class TLCGenSettingsViewModel : ViewModelBase
    {
        private TLCGenSettingsModel _Settings;

        public TLCGenSettingsModel Settings
        {
            get { return _Settings; }
        }

        #region Public Methods

        public void LoadApplicationSettings()
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

        public void SaveApplicationSettings()
        {
            string settingsfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TLCGenSettings.xml");
            TLCGen.DataAccess.SerializeT<TLCGenSettingsModel> serializer = new TLCGen.DataAccess.SerializeT<TLCGenSettingsModel>();
            serializer.Serialize(settingsfile, _Settings);
        }

        #endregion // Public Methods

        public TLCGenSettingsViewModel()
        {
            
        }
    }
}
