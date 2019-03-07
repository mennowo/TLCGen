using System;
using System.IO;
using System.Windows;
using TLCGen.Helpers;

namespace TLCGen.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ISettingsProvider _Default;

        #endregion // Fields

        #region Properties

        private TLCGenSettingsModel _Settings;
        public TLCGenSettingsModel Settings
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
            }
        }

        public static ISettingsProvider Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new SettingsProvider();
                        }
                    }
                }
                return _Default;
            }
        }

        #endregion // Properties

        #region Public Methods

        public static void OverrideDefault(ISettingsProvider provider)
        {
            _Default = provider;
        }

        #endregion // Public Methods

        #region Serialize Methods

        /// <summary>
        /// Loads the application settings from a file called TLCGenSettings.xml if it exists in the folder where the application runs
        /// </summary>
        public void LoadApplicationSettings()
        {
            try
            {
                string settingsfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TLCGen", "settings.xml");
                if (File.Exists(settingsfile))
                {
                    _Settings = TLCGenSerialization.DeSerialize<TLCGenSettingsModel>(settingsfile);
                }
                if (_Settings == null)
                    _Settings = new TLCGenSettingsModel();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading application settings:\n\n" + e.ToString() + "\n\nReverting to defaults.", "Error loading application settings");
                _Settings = new TLCGenSettingsModel();
            }
        }

        /// <summary>
        /// Saves the application settings to a file called TLCGenSettings.xml in the folder where the application runs
        /// </summary>
        public void SaveApplicationSettings()
        {
            try
            {
                string settingsfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TLCGen", "settings.xml");
                TLCGenSerialization.Serialize(settingsfile, _Settings);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error saving application settings:\n\n" + e.ToString() + "\n\nReverting to defaults.", "Error saving application settings");
                _Settings = new TLCGenSettingsModel();
            }
        }

        #endregion Serialize Methods
    }
}
