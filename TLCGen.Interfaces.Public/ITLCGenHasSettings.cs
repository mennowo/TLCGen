using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins
{
    public interface ITLCGenHasSettings : ITLCGenPlugin
    {
        /// <summary>
        /// Called after creating an instance of the plugin
        /// </summary>
        void LoadSettings();

        /// <summary>
        /// Called before disposing of an instance of the plugin
        /// </summary>
        void SaveSettings();
    }
}
