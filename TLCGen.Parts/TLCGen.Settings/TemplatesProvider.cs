using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using TLCGen.Helpers;

namespace TLCGen.Settings
{
    public class TemplatesProvider : ITemplatesProvider
    {
        #region Fields

        private static readonly object Locker = new object();
        private static ITemplatesProvider _default;
        private List<TLCGenTemplatesModelWithLocation> _loadedTemplates;

        #endregion // Fields

        #region Properties

        public List<TLCGenTemplatesModelWithLocation> LoadedTemplates => _loadedTemplates;

        public TLCGenTemplatesModel Templates { get; set; }

        public static ITemplatesProvider Default
        {
            get
            {
                if (_default == null)
                {
                    lock (Locker)
                    {
                        if (_default == null)
                        {
                            _default = new TemplatesProvider();
                        }
                    }
                }
                return _default;
            }
        }

        #endregion // Properties

        #region ITemplatesProvider

        public void LoadSettings()
        {
            try
            {
                Templates = new TLCGenTemplatesModel();
                _loadedTemplates = new List<TLCGenTemplatesModelWithLocation>();
                if (SettingsProvider.Default.Settings.UseFolderForTemplates)
                {
                    if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                        Directory.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                    {
                        var files = Directory.EnumerateFiles(SettingsProvider.Default.Settings.TemplatesLocation, "*.xml");
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
                                _loadedTemplates.Add(twl);
                                if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                                if (twl.Templates.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                                if (twl.Templates.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                                if (twl.Templates.PrioIngreepTemplates != null) foreach (var tprio in t.PrioIngreepTemplates) Templates.PrioIngreepTemplates.Add(tprio);
                            }
                            catch
                            {
                                // ignored (file not right)
                            }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) ||
                        !string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                        !Directory.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                    {
                        if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                            !Directory.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                            MessageBox.Show("De ingestelde map met templates is niet gevonden\n\n" +
                                           $"{SettingsProvider.Default.Settings.TemplatesLocation}\n\n" +
                                           $"De default templates worden geladen", "Map met templates niet gevonden");

                        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")))
                        {
                            var t = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml"));
                            var twl = new TLCGenTemplatesModelWithLocation
                            {
                                Location = Path.GetFileNameWithoutExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")),
                                Editable = false,
                                Templates = t
                            };
                            _loadedTemplates.Add(twl);
                            if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                            if (t.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                            if (t.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                            if (t.PrioIngreepTemplates != null) foreach (var tprio in t.PrioIngreepTemplates) Templates.PrioIngreepTemplates.Add(tprio);
                        }
                        else
                        {
                            MessageBox.Show("Could not find defaults for default settings. None loaded.", "Error loading default template");
                            Templates = new TLCGenTemplatesModel();
                        }
                    }
                }
                else
                {
                    if (SettingsProvider.Default.Settings.TemplatesLocation != null &&
                        File.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                    {
                        try
                        {
                            var t = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(SettingsProvider.Default.Settings.TemplatesLocation);
                            var twl = new TLCGenTemplatesModelWithLocation
                            {
                                Location = Path.GetFileNameWithoutExtension(SettingsProvider.Default.Settings.TemplatesLocation),
                                Editable = true,
                                Templates = t
                            };
                            _loadedTemplates.Add(twl);
                            if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                            if (t.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                            if (t.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                            if (t.PrioIngreepTemplates != null) foreach (var tprio in t.PrioIngreepTemplates) Templates.PrioIngreepTemplates.Add(tprio);
                        }
                        catch
                        {
                            MessageBox.Show("Het ingestelde bestand voor templates heeft niet het juiste formaat:\n\n" +
                                           $"{SettingsProvider.Default.Settings.TemplatesLocation}\n\n" +
                                           $"De default templates worden geladen", "Verkeerd formaat");
                        }
                    }
                    if (string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) ||
                        !string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                        !File.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                    {
                        if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                            !File.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                            MessageBox.Show("Het ingestelde bestand voor templates is niet gevonden\n\n" +
                                           $"{SettingsProvider.Default.Settings.TemplatesLocation}\n\n" +
                                           $"De default templates worden geladen", "Bestand met templates niet gevonden");

                        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")))
                        {
                            var t = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml"));
                            var twl = new TLCGenTemplatesModelWithLocation
                            {
                                Location = Path.GetFileNameWithoutExtension(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml")),
                                Editable = false,
                                Templates = t
                            };
                            _loadedTemplates.Add(twl);
                            if (t.FasenTemplates != null) foreach (var tfc in t.FasenTemplates) Templates.FasenTemplates.Add(tfc);
                            if (twl.Templates.DetectorenTemplates != null) foreach (var td in t.DetectorenTemplates) Templates.DetectorenTemplates.Add(td);
                            if (twl.Templates.PeriodenTemplates != null) foreach (var tp in t.PeriodenTemplates) Templates.PeriodenTemplates.Add(tp);
                            if (twl.Templates.PrioIngreepTemplates != null) foreach (var tprio in t.PrioIngreepTemplates) Templates.PrioIngreepTemplates.Add(tprio);
                        }
                        else
                        {
                            MessageBox.Show("Could not find defaults for templates. None loaded.", "Error loading default template");
                            Templates = new TLCGenTemplatesModel();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while loading templates: " + e, "Error while loading templates");
            }
        }

        public void SaveSettings()
        {
            if (SettingsProvider.Default.Settings.UseFolderForTemplates)
            {
                if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                    Directory.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                {
                    foreach (var t in _loadedTemplates)
                    {
                        var fn = Path.Combine(SettingsProvider.Default.Settings.TemplatesLocation, t.Location + ".xml");
                        if (File.Exists(fn))
                        {
                            File.Delete(fn);
                        }
                        TLCGenSerialization.Serialize(fn, t.Templates);
                    }
                }
            }
            else if(_loadedTemplates.Any(x => x.Editable))
            {
                if (!string.IsNullOrWhiteSpace(SettingsProvider.Default.Settings.TemplatesLocation) &&
                    File.Exists(SettingsProvider.Default.Settings.TemplatesLocation))
                {
                    File.Delete(SettingsProvider.Default.Settings.TemplatesLocation);
                }
                TLCGenSerialization.Serialize(SettingsProvider.Default.Settings.TemplatesLocation, _loadedTemplates.First(x => x.Editable).Templates);
            }
        }

        #endregion // ITemplatesProvider

        #region Public Methods

        public static void OverrideDefault(ITemplatesProvider provider)
        {
            _default = provider;
        }

        #endregion // Public Methods
    }
}
