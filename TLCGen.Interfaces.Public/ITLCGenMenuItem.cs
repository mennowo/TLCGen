using System.Windows.Controls;

namespace TLCGen.Plugins
{
    public interface ITLCGenMenuItem : ITLCGenPlugin
    {
        MenuItem Menu { get; }
    }
}
