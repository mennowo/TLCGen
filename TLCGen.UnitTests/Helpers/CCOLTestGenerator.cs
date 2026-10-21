using TLCGen.Generators.CCOL;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;

namespace TLCGen.UnitTests
{
    /// <summary>
    /// Shared headless CCOL generator for all tests that generate code.
    /// </summary>
    internal static class CCOLTestGenerator
    {
        private static CCOLCodeGeneratorPlugin _plugin;

        // CCOLGenerator can only be constructed once per process (its piece generators are static),
        // so every test must use this one plugin instance.
        public static CCOLCodeGeneratorPlugin Plugin => _plugin ??= new CCOLCodeGeneratorPlugin(true);

        /// <summary>
        /// Loads the embedded default generator settings. Unlike CCOLCodeGeneratorPlugin.LoadSettings(), this
        /// ignores the user's settings in %APPDATA%, so generated code does not depend on the machine.
        /// </summary>
        public static void LoadDefaultSettings()
        {
            CCOLGeneratorSettingsProvider.Default.Settings = TLCGenSerialization.DeSerializeData<CCOLGeneratorSettingsModel>(
                ResourceReader.GetResourceTextFile("TLCGen.Generators.CCOL.Settings.ccolgendefaults.xml", Plugin, typeof(CCOLGeneratorSettingsProvider)));
            CCOLGeneratorSettingsProvider.Default.Reset();
            Plugin.Generator.LoadSettings();
        }
    }
}
