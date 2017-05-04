using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenImporter : ITLCGenPlugin
    {
        bool ImportsIntoExisting { get; }

        ControllerModel ImportController(ControllerModel c = null);
    }
}
