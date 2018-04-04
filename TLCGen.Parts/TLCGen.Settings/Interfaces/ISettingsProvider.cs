namespace TLCGen.Settings
{
    public interface ISettingsProvider
    {
        TLCGenSettingsModel Settings { get; set; }

        void LoadApplicationSettings();
        void SaveApplicationSettings();
    }
}
