﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public class TLCGenPluginManager : ITLCGenPluginManager
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenPluginManager _Default;
        private List<Tuple<TLCGenPluginElems,Type>> _ApplicationParts;
        private List<Tuple<TLCGenPluginElems,Type>> _Plugins;
        private List<ITLCGenPlugin> _LoadedPlugins;
        private List<ITLCGenPlugin> _ApplicationPlugins;

        #endregion // Fields

        #region Properties

        public static ITLCGenPluginManager Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TLCGenPluginManager();
                        }
                    }
                }
                return _Default;
            }
        }

        public List<Tuple<TLCGenPluginElems, Type>> ApplicationParts
        {
            get
            {
                if (_ApplicationParts == null)
                {
                    _ApplicationParts = new List<Tuple<TLCGenPluginElems, Type>>();
                }
                return _ApplicationParts;
            }
        }

        public List<Tuple<TLCGenPluginElems,Type>> Plugins
        {
            get
            {
                if(_Plugins == null)
                {
                    _Plugins = new List<Tuple<TLCGenPluginElems, Type>>();
                }
                return _Plugins;
            }
        }

        public List<ITLCGenPlugin> LoadedPlugins
        {
            get
            {
                if (_LoadedPlugins == null)
                {
                    _LoadedPlugins = new List<ITLCGenPlugin>();
                }
                return _LoadedPlugins;
            }
        }

        public List<ITLCGenPlugin> ApplicationPlugins
        {
            get
            {
                if (_ApplicationPlugins == null)
                {
                    _ApplicationPlugins = new List<ITLCGenPlugin>();
                }
                return _ApplicationPlugins;
            }
        }

        #endregion // Properties

        #region Plublic methods

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

        public void LoadPlugins(string pluginpath)
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
                                    Plugins.Add(new Tuple<TLCGenPluginElems, Type>(attr.PluginElements, t));
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

        #endregion // Plublic methods

        #region Constructor

        public TLCGenPluginManager()
        {

        }

        #endregion // Constructor
    }
}
