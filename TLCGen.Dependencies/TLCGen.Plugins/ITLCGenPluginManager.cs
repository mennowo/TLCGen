using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins
{
    public interface ITLCGenPluginManager
    {
        List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationPlugins { get; }
        List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationParts { get; }
        void LoadPlugins(string pluginpath);
        void LoadApplicationParts(List<Type> types);
    }
}
