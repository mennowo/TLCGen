using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins
{
    public interface ITLCGenPluginManager
    {
        List<Tuple<TLCGenPluginElems, Type>> Plugins { get; }
        List<Tuple<TLCGenPluginElems, Type>> ApplicationParts { get; }
        void LoadPlugins(string pluginpath);
    }
}
