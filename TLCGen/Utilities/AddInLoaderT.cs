using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Interfaces.Public;
using TLCGen.Models;

namespace TLCGen.Utilities
{
    public static class AddInLoaderT
    {
        /// <summary>
        /// Loads settings for a given TLCGen addin, such as a generator or importer. The settings are retrieved from
        /// the instance of CustomDataModel parsed, which is searched for the name of the addin.
        /// The settings are applied to the addin by loading the properties of the Type parsed, and calling
        /// SetValue for the properties that have the TLCGenCustomSetting attribute, and for which settings are found.
        /// </summary>
        /// <param name="addin">An instance of ITLCGenAddin</param>
        /// <param name="addintype">And instance of Type, indicating the type of the addin. This is used to read its properties (through reflection).</param>
        /// <param name="customdata">Instance of CustomDataModel to read settings from.</param>
        public static void LoadAddinSettings(ITLCGenAddin addin, Type addintype, CustomDataModel customdata)
        {
            // Cast the addin to ITLCGenAddin so we can read its name
            var iaddin = addin as ITLCGenAddin;

            // Loop the settings data, to see if we have settings for this Generator
            foreach (AddinSettingsModel addindata in customdata.AddinSettings)
            {
                if (addindata.Naam == iaddin.Name)
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

        /// <summary>
        /// Loads all addins of type T1 that have been decorated with attribute T2, that are found in the given folder.
        /// </summary>
        /// <typeparam name="T1">The type of addin to load.</typeparam>
        /// <typeparam name="T2">The attribute with which a class has to been decorated to be considered a addin of type T1.</typeparam>
        /// <param name="AddInFolder">The folder in which to look for .dll files.</param>
        /// <returns></returns>
        public static List<T1> LoadAllAddins<T1, T2>(string AddInFolder)
        {
            List<T1> addins = new List<T1>();
            try
            {
                if (Directory.Exists(AddInFolder))
                {
                    // Find all Generator DLL's
                    foreach (String file in Directory.GetFiles(AddInFolder))
                    {
                        if (Path.GetExtension(file).ToLower() == ".dll")
                        {
                            // Find and loop all types from the Generators
                            Assembly assemblyInstance = Assembly.LoadFrom(file);
                            Type[] types = assemblyInstance.GetTypes();
                            foreach (Type t in types)
                            {
                                // Find TLCGenGenerator attribute, and if found, continue
                                var custattr = Attribute.GetCustomAttribute(t, typeof(T2));
                                if (custattr != null)
                                {
                                    // Cast the Generator to IGenerator so we can read its name
                                    var addin = Activator.CreateInstance(t);
                                    
                                    // Add the generator to our list of available generators
                                    addins.Add((T1)addin);
                                }
                                else
                                {
#if !DEBUG
                                    System.Windows.MessageBox.Show($"Library {file} wordt niet herkend als TLCGen addin.");
#endif
                                }
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
            return addins;
        }
    }
}
