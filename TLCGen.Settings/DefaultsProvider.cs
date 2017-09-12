using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings.Utilities;

namespace TLCGen.Settings
{
    public class DefaultsProvider : IDefaultsProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static IDefaultsProvider _Default;

        #endregion // Fields

        #region Properties

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
            }
        }

        private TLCGenDefaultsModel _Defaults;
        public TLCGenDefaultsModel Defaults
        {
            get { return _Defaults; }
            set
            {
                _Defaults = value;
            }
        }

        public static IDefaultsProvider Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new DefaultsProvider();
                        }
                    }
                }
                return _Default;
            }
        }

        #endregion // Properties

        #region Public Methods

        public static void OverrideDefault(IDefaultsProvider provider)
        {
            _Default = provider;
        }

        #endregion // Public Methods

        #region Private Methods

        private void CopyAllProperties<T>(T from, T to, bool onlyvalues = true)
        {
            var type = from.GetType();
            var props = type.GetProperties();
            foreach (PropertyInfo property in props)
            {
                var att = (HasDefaultAttribute)property.GetCustomAttribute(typeof(HasDefaultAttribute));
                if (att == null || att.HasDefault == true)
                {
                    if (property.PropertyType.IsValueType ||
                        !onlyvalues)
                    {
                        object propValue = property.GetValue(from);
                        property.SetValue(to, propValue);
                    }
                }
            }
        }

        private FaseTypeEnum GetFaseCyclusTypeFromName(string name)
        {
            if (_Controller == null)
                return FaseTypeEnum.Auto;

            foreach(var fc in _Controller.Fasen)
            {
                if(fc.Naam == name)
                {
                    return fc.Type;
                }
            }
            return FaseTypeEnum.Auto;
        }

        private DetectorTypeEnum GetDetectorTypeFromName(string name)
        {
            if (_Controller == null)
                return DetectorTypeEnum.Kop;

            foreach (var fc in _Controller.Fasen)
            {
                foreach (var d in fc.Detectoren)
                {
                    if (d.Naam == name)
                    {
                        return d.Type;
                    }
                }
            }
            foreach (var d in _Controller.Detectoren)
            {
                if (d.Naam == name)
                {
                    return d.Type;
                }
            }
            return DetectorTypeEnum.Kop;
        }

        #endregion // Private Methods

        #region IDefaultsProvider

        public void SetDefaultsOnModel(object model, string selector1 = null, string selector2 = null, bool onlyvalues = true)
        {
            var type = model.GetType();
            var typename = type.FullName + "," + type.Assembly.GetName().Name;

            var defs = Defaults.Defaults.Where(x => x.DataType == typename);

            if (defs == null || !defs.Any())
                return;

            if(defs.Count() == 1)
            {
                CopyAllProperties(defs.First().Data, model, onlyvalues);
            }
            else if(defs.Count() > 1)
            {
                bool found = false;
                if (selector1 == null && selector2 == null)
                {
                    CopyAllProperties(defs.First().Data, model, onlyvalues);
                    found = true;
                    //MessageBox.Show("Fout bij toepassen default instellingen voor " + type.Name + ":\nGeen selector bekend bij meerdere beschikbare defaults.", "Fout bij toepassen defaults");
                }
                else
                {
                    foreach (var def in defs)
                    {
                        if ((selector1 == null || selector1 == def.Selector1) &&
                            (selector2 == null || def.Selector2 == null || selector2 == def.Selector2))
                        {
                            CopyAllProperties(def.Data, model, onlyvalues);
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    //CopyAllValueProperties(defs.First().Data, model);
                    //MessageBox.Show("Fout bij toepassen default instellingen voor " + type.Name + ":\nGeen passende default gevonden bij: " + selector1 + "/" + selector2 + ".", "Fout bij toepassen defaults");
                }
            }
        }

        public void LoadSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Defaults\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");
#if DEBUG
            setfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultdefaults.xml");
#else
            if (!File.Exists(setfile))
            {
                setfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultdefaults.xml");
            }
            if (!File.Exists(setfile))
            {
                MessageBox.Show("Could not find defaults for default settings. None loaded.", "Error loading defaults");
                setfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultdefaults.xml");
                Defaults = new TLCGenDefaultsModel();
                return;
            }
#endif
            Defaults = new TLCGenDefaultsModel();
            try
            {
                var doc = new XmlDocument();
                XmlReader reader =
                    XmlReader.Create(
                        setfile,
                        new XmlReaderSettings() { IgnoreComments = true });
                doc.Load(reader);
                var defs = doc.DocumentElement.SelectSingleNode("Defaults");
                foreach (XmlNode def in defs.ChildNodes)
                {
                    XmlNode x = def.SelectSingleNode("DataType");
                    string t = x.InnerText;
                    var type = Type.GetType(t);
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "Data";
                    xRoot.IsNullable = true;
                    System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(type, xRoot);
                    // http://stackoverflow.com/questions/1563473/xmlnode-to-objects
                    object o = ser.Deserialize(new XmlNodeReader(def.SelectSingleNode("Data")));
                    var item = new TLCGenDefaultModel();
                    item.DefaultName = def.SelectSingleNode("DefaultName").InnerText;
                    item.DataType = def.SelectSingleNode("DataType").InnerText;
                    item.Category = def.SelectSingleNode("Category").InnerText;
                    XmlNode n1 = def.SelectSingleNode("Selector1");
                    if (n1 != null)
                    {
                        item.Selector1 = n1.InnerText;
                    }
                    XmlNode n2 = def.SelectSingleNode("Selector2");
                    if (n2 != null)
                    {
                        item.Selector2 = n2.InnerText;
                    }
                    item.Data = o;
                    Defaults.Defaults.Add(item);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured while loading the defaults:\n " + e.ToString() + "\nPlease report this.", "Error while loading defaults");
            }
        }

        private object ConvertNode(XmlNode node, Type t)
        {
            MemoryStream stm = new MemoryStream();
            StreamWriter stw = new StreamWriter(stm);
            stw.Write(node.OuterXml);
            stw.Flush();
            stm.Position = 0;
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(t);
            var result = ser.Deserialize(stm);
            return result;
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Defaults\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");
            using (FileStream fs = new FileStream(setfile, FileMode.Create, FileAccess.Write))
            {
                List<Type> et = new List<Type>();
                foreach(var def in Defaults.Defaults)
                {
                    if(!et.Contains(def.Data.GetType()))
                    {
                        et.Add(def.Data.GetType());
                    }
                }

                var serializer = new XmlSerializer(typeof(TLCGenDefaultsModel), et.ToArray());
                serializer.Serialize(fs, Defaults);
                fs.Close();
            }
        }

        #endregion // IDefaultsProvider
    }
}
