namespace TLCGen.Plugins
{
    [System.Flags]
    public enum TLCGenPluginElems
    {
        Generator = 0x1,
        Importer = 0x2,
        TabControl = 0x4,
        MenuControl = 0x8,
        ToolBarControl = 0x10,
        IOElementProvider = 0x20,
        XMLNodeWriter = 0x40,
        HasSettings = 0x80,
        PlugMessaging = 0x100,
		Switcher = 0x200
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenPluginAttribute : System.Attribute
    {
        public TLCGenPluginElems PluginElements { get; set; }

        public TLCGenPluginAttribute(TLCGenPluginElems pluginelements)
        {
            PluginElements = pluginelements;
        }
    }
}
