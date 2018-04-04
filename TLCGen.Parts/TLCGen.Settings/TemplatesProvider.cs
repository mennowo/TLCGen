using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.Helpers;

namespace TLCGen.Settings
{
    public class TemplatesProvider : ITemplatesProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITemplatesProvider _Default;
        private List<TLCGenTemplatesModelWithLocation> _LoadedTemplates;

        #endregion // Fields

        #region Properties

        public List<TLCGenTemplatesModelWithLocation> LoadedTemplates => _LoadedTemplates;

        private TLCGenTemplatesModel _Templates;
        public TLCGenTemplatesModel Templates
        {
            get { return _Templates; }
            set
            {
                _Templates = value;
            }
        }

        public static ITemplatesProvider Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TemplatesProvider();
                        }
                    }
                }
                return _Default;
            }
        }

        #endregion // Properties

        #region ITemplatesProvider

        public void LoadSettings()
        {
            try
            {
                Templates = new TLCGenTemplatesModel();
                _LoadedTemplates = new List<TLCGenTemplatesModelWithLocation>();
                if (SettingsProvider.Default.Settings.TemplatesFolderLocation != null &&
                    Directory.Exists(SettingsProvider.Default.Settings.TemplatesFolderLocation))
                {
                    var files = Directory.EnumerateFiles(SettingsProvider.Default.Settings.TemplatesFolderLocation, "*.xml");
                    foreach (var f in files)
                    {
                        try
                        {
                            var t = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(f);
                            var twl = new TLCGenTemplatesModelWithLocation
                            {
                                Location = Path.GetFileNameWithoutExtension(f),
                                Editable = true,
                                Templates = t
                            };
                            _LoadedTemplates.Add(twl);
                            if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                            if (twl.Templates.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                            if (twl.Templates.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                        }
                        catch
                        {
                            // ignored (file not right)
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesFolderLocation) ||
                    !string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesFolderLocation) &&
                    !Directory.Exists(SettingsProvider.Default.Settings.TemplatesFolderLocation))
                {
                    if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesFolderLocation) &&
                        !Directory.Exists(SettingsProvider.Default.Settings.TemplatesFolderLocation))
                        MessageBox.Show("De ingestelde map met templates is niet gevonden\n\n" +
                                       $"{SettingsProvider.Default.Settings.TemplatesFolderLocation}\n\n" +
                                       $"De default templates worden geladen", "Map met templates gevonden");
                
                    if (File.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")))
                    {
                        var t = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml"));
                        var twl = new TLCGenTemplatesModelWithLocation
                        {
                            Location = Path.GetFileNameWithoutExtension(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")),
                            Editable = false,
                            Templates = t
                        };
                        _LoadedTemplates.Add(twl);
                        if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                        if (twl.Templates.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                        if (twl.Templates.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                    }
                    else
                    {
                        MessageBox.Show("Could not find defaults for default settings. None loaded.", "Error loading default template");
                        Templates = new TLCGenTemplatesModel();
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading templates: " + e.ToString(), "Error while loading templates");
            }
        }

        public void SaveSettings()
        {
            if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesFolderLocation) &&
                Directory.Exists(SettingsProvider.Default.Settings.TemplatesFolderLocation))
            {
                foreach(var t in _LoadedTemplates)
                {
                    var fn = Path.Combine(SettingsProvider.Default.Settings.TemplatesFolderLocation, t.Location + ".xml");
                    TLCGenSerialization.Serialize<TLCGenTemplatesModel>(fn, t.Templates);
                }
            }
        }

        #endregion // ITemplatesProvider

        #region Public Methods

        public static void OverrideDefault(ITemplatesProvider provider)
        {
            _Default = provider;
        }

        #endregion // Public Methods
    }
}
