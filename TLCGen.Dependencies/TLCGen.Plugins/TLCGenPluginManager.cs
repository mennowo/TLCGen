using Microsoft.Win32;
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
    public class TLCGenPluginManager : ITLCGenPluginManager
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenPluginManager _Default;
        private static List<string> _lookupDepsPaths;
        private List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> _ApplicationParts;
        private List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> _Plugins;

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

        public List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationParts
        {
            get
            {
                if (_ApplicationParts == null)
                {
                    _ApplicationParts = new List<Tuple<TLCGenPluginElems, ITLCGenPlugin>>();
                }
                return _ApplicationParts;
            }
        }

        public List<Tuple<TLCGenPluginElems, ITLCGenPlugin>> ApplicationPlugins
        {
            get
            {
                if(_Plugins == null)
                {
                    _Plugins = new List<Tuple<TLCGenPluginElems, ITLCGenPlugin>>();
                }
                return _Plugins;
            }
        }

        #endregion // Properties

        #region Public methods

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
                                                throw new InvalidCastException("False IGenerator property type: " + type);
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

        public void LoadApplicationParts(List<Type> types)
        {
            foreach (var type in types)
            {
                // Load parts
                var attr = (TLCGenPluginAttribute)type.GetCustomAttribute(typeof(TLCGenPluginAttribute));
                if(attr != null)
                {
                    ApplicationParts.Add(
                        new Tuple<TLCGenPluginElems, ITLCGenPlugin>(
                            attr.PluginElements,
                            (ITLCGenPlugin)Activator.CreateInstance(type)));
                }
                else
                {
                    // load tabs
                    var attrt = (TLCGenTabItemAttribute)type.GetCustomAttribute(typeof(TLCGenTabItemAttribute));
                    if (attrt != null)
                    {
                        ApplicationParts.Add(
                            new Tuple<TLCGenPluginElems, ITLCGenPlugin>(
                                TLCGenPluginElems.TabControl,
                                (ITLCGenPlugin)Activator.CreateInstance(type)));
                    }
                }
            }

        }

        public void LoadPlugins(string pluginpath)
        {
            // load local plugins
            try
            {
                if (Directory.Exists(pluginpath))
                {
                    LoadPluginsFromPath(pluginpath);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            // load remote plugins
            try
            {
                var key = Registry.CurrentUser.OpenSubKey("Software\\CodingConnected e.U.\\TLCGen\\Plugins");
                if (key != null)
                {
                    foreach (var v in key.GetValueNames())
                    {
                        var paths = (string[])key.GetValue(v);
                        if (paths.Length == 1)
                        {
                            LoadPluginsFromPath(paths[0]);
                        }
                        else if (paths.Length == 2)
                        {
                            if (!_lookupDepsPaths.Contains(paths[1]))
                            {
                                _lookupDepsPaths.Add(paths[1]);
                            }
                            LoadPluginsFromPath(paths[0]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion // Public methods

        #region Private Methods

        private void LoadPluginsFromPath(string pluginpath)
        {
            // Find all plugin DLL's
            foreach (String file in Directory.GetFiles(pluginpath))
            {
                string _file = file;
                if (Path.GetExtension(_file).ToLower() == ".dll")
                {
                    // Find and loop all types from the assembly
                    Assembly assemblyInstance = null;
                    assemblyInstance = Assembly.LoadFrom(_file);
                    var types = assemblyInstance.GetTypes();
                    var bFound = false;
                    foreach (Type type in types)
                    {
                        // Find TLCGenPluginAttribute attribute, and if found, continue
                        var attr = (TLCGenPluginAttribute)type.GetCustomAttribute(typeof(TLCGenPluginAttribute));
                        if (attr != null)
                        {
                            ApplicationPlugins.Add(
                                new Tuple<TLCGenPluginElems, ITLCGenPlugin>(
                                    attr.PluginElements,
                                    (ITLCGenPlugin)Activator.CreateInstance(type)));
                            bFound = true;
                        }
                    }
                    if (!bFound)
                    {
                        System.Windows.MessageBox.Show($"Library {_file} wordt niet herkend als TLCGen addin.");
                    }
                }
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyname = new AssemblyName(args.Name).Name;
            foreach(var p in _lookupDepsPaths)
            {
                var assemblyFileName = Path.Combine(p, assemblyname + ".dll");
                if(File.Exists(assemblyFileName))
                {
                    var assembly = Assembly.LoadFrom(assemblyFileName);
                    return assembly;
                }
            }
            return null;
        }

        #endregion // Private Methods

        #region Constructor

        public TLCGenPluginManager()
        {
            _lookupDepsPaths = new List<string>();
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        #endregion // Constructor
    }
}
