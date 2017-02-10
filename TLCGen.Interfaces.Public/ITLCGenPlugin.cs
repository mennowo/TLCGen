using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenPlugin
    {
        ControllerModel Controller { get; set; }
        string GetPluginName();
    }
}
