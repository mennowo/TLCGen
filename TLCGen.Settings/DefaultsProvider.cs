using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;
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

        private TLCGenDefaultsModel _Settings;
        public TLCGenDefaultsModel Defaults
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
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

        private void CopyAllValueProperties<T>(T from, T to)
        {
            var type = from.GetType();
            var props = type.GetProperties();
            foreach (PropertyInfo property in props)
            {
                if (property.PropertyType.IsValueType)
                {
                    object propValue = property.GetValue(from);
                    property.SetValue(to, propValue);
                }
            }
        }

        #endregion // Private Methods

        #region IDefaultsProvider

        public void SetDefaultsOnModel(object model)
        {
            var type = model.GetType();
            var typename = type.Name;
            switch(typename)
            {
                case "FaseCyclusModel":
                    FaseCyclusModel fc = model as FaseCyclusModel;
                    var fromfc = Defaults.Fasen.Where(x => x.Type == fc.Type);
                    if(fromfc != null && fromfc.Count() > 0)
                    {
                        CopyAllValueProperties(fromfc.First(), fc);
                    }
                    break;
                case "DetectorModel":
                    DetectorModel d = model as DetectorModel;
                    var fromd = Defaults.Detectoren.Where(x => x.Type == d.Type);
                    if (fromd != null && fromd.Count() > 0)
                    {
                        CopyAllValueProperties(fromd.First(), d);
                    }
                    break;
                case "RoBuGroverConflictGroepFaseModel":
                    RoBuGroverConflictGroepFaseModel rgvfc = model as RoBuGroverConflictGroepFaseModel;
                    CopyAllValueProperties(Defaults.RoBuGroverFase, rgvfc);
                    break;
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
            Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultssettings.xml"));
#else
            if (File.Exists(setfile))
            {
                Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(setfile);
            }
            else
            {
                Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultssettings.xml"));
            }
#endif
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Defaults\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"tlcgendefaultssettings.xml");
            TLCGenSerialization.Serialize<TLCGenDefaultsModel>(setfile, Defaults);
        }

        #endregion // IDefaultsProvider
    }
}
