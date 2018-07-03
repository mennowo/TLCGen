using System.Collections.Generic;

namespace TLCGen.Settings
{
    public interface ITemplatesProvider
    {
        TLCGenTemplatesModel Templates { get; }
        List<TLCGenTemplatesModelWithLocation> LoadedTemplates { get; }

        void LoadSettings();
        void SaveSettings();
    }
}
