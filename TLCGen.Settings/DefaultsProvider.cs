using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
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

        public void SetDefaultsOnModel(object model)
        {
            var type = model.GetType();
            var typename = type.Name;
            var props = type.GetProperties();
#warning TODO: change this, so it uses the RefersToSignalGroup and RefersToDetector attributes to find the name; that attr is there already!
            if (props.Where(x => x.Name == "FaseCyclus").Any() || 
                typename.StartsWith("FaseCyclus") && props.Where(x => x.Name == "Naam").Any())
            {
                PropertyInfo prop;
                var _props = props.Where(x => x.Name == "FaseCyclus");
                if(_props == null || _props.Count() == 0)
                {
                    prop = props.Where(x => x.Name == "Naam").First();
                }
                else
                {
                    prop = props.Where(x => x.Name == "FaseCyclus").First();
                }
                var fctype = GetFaseCyclusTypeFromName((string)prop.GetValue(model));
                var fromfc = Defaults.Fasen.Where(x => x.Type == fctype);
                var frommodel = fromfc.First().GetModel(model.GetType().Name);
                if(frommodel != null)
                {
                    CopyAllValueProperties(frommodel, model);
                }
            }
            if (props.Where(x => x.Name == "Detector").Any() ||
                typename.StartsWith("Detector") && props.Where(x => x.Name == "Naam").Any())
            {
                PropertyInfo prop;
                var _props = props.Where(x => x.Name == "Detector");
                if (_props == null || _props.Count() == 0)
                {
                    prop = props.Where(x => x.Name == "Naam").First();
                }
                else
                {
                    prop = props.Where(x => x.Name == "Detector").First();
                }
                var dtype = GetDetectorTypeFromName((string)prop.GetValue(model));
                var fromd = Defaults.Detectoren.Where(x => x.Type == dtype);
                var frommodel = fromd.First().GetModel(model.GetType().Name);
                if (frommodel != null)
                {
                    CopyAllValueProperties(frommodel, model);
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
            Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultdefaults.xml"));
#else
            if (File.Exists(setfile))
            {
                Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(setfile);
            }
            else
            {
                Defaults = TLCGenSerialization.DeSerialize<TLCGenDefaultsModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultdefaults.xml"));
            }
#endif
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Defaults\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");
            TLCGenSerialization.Serialize<TLCGenDefaultsModel>(setfile, Defaults);
        }

        #endregion // IDefaultsProvider
    }
}
