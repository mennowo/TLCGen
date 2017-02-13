using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Models.Settings;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    public class TLCGenSettingsViewModel_old : ViewModelBase
    {
        public TLCGenSettingsModel Settings
        {
            get { return SettingsProvider.Default.Settings; }
        }

        #region Public Methods

        #endregion // Public Methods

        public TLCGenSettingsViewModel_old()
        {
            
        }
    }
}
