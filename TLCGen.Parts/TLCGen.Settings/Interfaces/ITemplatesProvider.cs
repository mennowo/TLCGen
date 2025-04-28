using System;
using System.Collections.Generic;

namespace TLCGen.Settings
{
    public interface ITemplatesProvider
    {
        TLCGenTemplatesModel Templates { get; }
        List<TLCGenTemplatesModelWithLocation> LoadedTemplates { get; }
        event EventHandler LoadedTemplatesChanged;

        void LoadSettings();
        void SaveSettings();
    }
}
