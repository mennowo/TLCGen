using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public class TLCGenPluginManager
    {
        private List<ITLCGenPlugin> _AllPlugins;
        public List<ITLCGenPlugin> AllPlugins
        {
            get
            {
                if(_AllPlugins == null)
                {
                    _AllPlugins = new List<ITLCGenPlugin>();
                }
                return _AllPlugins;
            }
        }

        private List<ITLCGenGenerator> _Generators;
        public List<ITLCGenGenerator> Generators
        {
            get
            {
                if (_Generators == null)
                {
                    _Generators = new List<ITLCGenGenerator>();
                }
                return _Generators;
            }
        }

        private List<ITLCGenImporter> _Importers;
        public List<ITLCGenImporter> Importers
        {
            get
            {
                if (_Importers == null)
                {
                    _Importers = new List<ITLCGenImporter>();
                }
                return _Importers;
            }
        }

        private List<ITLCGenMenuItem> _MenuItems;
        public List<ITLCGenMenuItem> MenuItems
        {
            get
            {
                if (_MenuItems == null)
                {
                    _MenuItems = new List<ITLCGenMenuItem>();
                }
                return _MenuItems;
            }
        }

        private List<ITLCGenTabItem> _TabItems;
        public List<ITLCGenTabItem> TabItems
        {
            get
            {
                if (_TabItems == null)
                {
                    _TabItems = new List<ITLCGenTabItem>();
                }
                return _TabItems;
            }
        }

        private List<ITLCGenToolBar> _Toolbars;
        public List<ITLCGenToolBar> Toolbars
        {
            get
            {
                if (_Toolbars == null)
                {
                    _Toolbars = new List<ITLCGenToolBar>();
                }
                return _Toolbars;
            }
        }

        /// <summary>
        /// Loads settings for a given TLCGen addin, such as a generator or importer. The settings are retrieved from
        /// the instance of CustomDataModel parsed, which is searched for the name of the addin.
        /// The settings are applied to the addin by loading the properties of the Type parsed, and calling
        /// SetValue for the properties that have the TLCGenCustomSetting attribute, and for which settings are found.
        /// </summary>
        /// <param name="addin">An instance of ITLCGenAddin</param>
        /// <param name="addintype">And instance of Type, indicating the type of the addin. This is used to read its properties (through reflection).</param>
        /// <param name="customdata">Instance of CustomDataModel to read settings from.</param>
        public static void LoadAddinSettings(ITLCGenPlugin addin, Type addintype, CustomDataModel customdata)
        {
            // Cast the addin to ITLCGenAddin so we can read its name
            var iaddin = addin as ITLCGenPlugin;

            // Loop the settings data, to see if we have settings for this Generator
            foreach (AddinSettingsModel addindata in customdata.AddinSettings)
            {
                if (addindata.Naam == iaddin.GetPluginName())
                {
                    // From the Generator, real all properties attributed with [TLCGenGeneratorSetting]
                    var dllprops = addintype.GetProperties().Where(
                        prop => Attribute.IsDefined(prop, typeof(TLCGenCustomSettingAttribute)));
                    // Loop the saved settings, and load if applicable
                    foreach (AddinSettingsPropertyModel dataprop in addindata.Properties)
                    {
                        foreach (var propinfo in dllprops)
                        {
                            // Only load here, if it is a controller specific setting
                            TLCGenCustomSettingAttribute propattr = (TLCGenCustomSettingAttribute)Attribute.GetCustomAttribute(propinfo, typeof(TLCGenCustomSettingAttribute));
                            if (propinfo.Name == dataprop.Naam)
                            {
                                if (propattr != null && propattr.SettingType == TLCGenCustomSettingAttribute.SettingTypeEnum.Application)
                                {
                                    try
                                    {
                                        string type = propinfo.PropertyType.ToString();
                                        switch (type)
                                        {
                                            case "System.Double":
                                                double d;
                                                if (Double.TryParse(dataprop.Setting, out d))
                                                    propinfo.SetValue(addin, d);
                                                break;
                                            case "System.Int32":
                                                int i32;
                                                if (Int32.TryParse(dataprop.Setting, out i32))
                                                    propinfo.SetValue(addin, i32);
                                                break;
                                            case "System.String":
                                                propinfo.SetValue(addin, dataprop.Setting);
                                                break;
                                            default:
                                                throw new NotImplementedException("False IGenerator property type: " + type);
                                        }
                                    }
                                    catch
                                    {
                                        System.Windows.MessageBox.Show("Error load generator settings.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public TLCGenPluginManager(string pluginpath)
        {
            try
            {
                if (Directory.Exists(pluginpath))
                {
                    // Find all Generator DLL's
                    foreach (String file in Directory.GetFiles(pluginpath))
                    {
                        if (Path.GetExtension(file).ToLower() == ".dll")
                        {
                            // Find and loop all types from the assembly
                            var assemblyInstance = Assembly.LoadFrom(file);
                            var types = assemblyInstance.GetTypes();
                            var bFound = false;
                            foreach (Type t in types)
                            {
                                // Find TLCGenPluginAttribute attribute, and if found, continue
                                var attr = (TLCGenPluginAttribute)Attribute.GetCustomAttribute(t, typeof(TLCGenPluginAttribute));
                                if (attr != null)
                                {
                                    var plugin = Activator.CreateInstance(t);

                                    if ((attr.PluginElements & TLCGenPluginElems.Generator) == TLCGenPluginElems.Generator)
                                    {
                                        Generators.Add(plugin as ITLCGenGenerator);
                                    }

                                    if ((attr.PluginElements & TLCGenPluginElems.Importer) == TLCGenPluginElems.Importer)
                                    {
                                        Importers.Add(plugin as ITLCGenImporter);
                                    }

                                    if ((attr.PluginElements & TLCGenPluginElems.MenuControl) == TLCGenPluginElems.MenuControl)
                                    {
                                        MenuItems.Add(plugin as ITLCGenMenuItem);
                                    }

                                    if ((attr.PluginElements & TLCGenPluginElems.TabControl) == TLCGenPluginElems.TabControl)
                                    {
                                        TabItems.Add(plugin as ITLCGenTabItem);
                                    }

                                    if ((attr.PluginElements & TLCGenPluginElems.ToolBarControl) == TLCGenPluginElems.ToolBarControl)
                                    {
                                        TabItems.Add(plugin as ITLCGenTabItem);
                                    }

                                    bFound = true;
                                }
                            }
                            if (!bFound)
                            {
                                //#if !DEBUG
                                System.Windows.MessageBox.Show($"Library {file} wordt niet herkend als TLCGen addin.");
                                //#endif
                            }
                        }
                    }
                }
                else
                {
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
    }
}
