using System;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenDefaultString
    {
        public string Name { get; set; }
        public string Default { get; set; }
        public string Setting { get; set; }
    }
}