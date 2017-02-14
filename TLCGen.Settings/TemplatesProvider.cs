using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;

namespace TLCGen.Settings
{
    public class TemplatesProvider : ITemplatesProvider
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITemplatesProvider _Default;

        #endregion // Fields

        #region Properties

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
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Templates\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"settings.xml");
#if DEBUG
            Templates = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaulttemplates.xml"));
#else
            if (File.Exists(setfile))
            {
                Templates = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(setfile);
            }
            else
            {
                Templates = TLCGenSerialization.DeSerialize<TLCGenTemplatesModel>(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Settings\\tlcgendefaultssettings.xml"));
            }
#endif
        }

        public void SaveSettings()
        {
            var appdatpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var setpath = Path.Combine(appdatpath, @"TLCGen\Templates\");
            if (!Directory.Exists(setpath))
                Directory.CreateDirectory(setpath);
            var setfile = Path.Combine(setpath, @"tlcgentemplates.xml");
            TLCGenSerialization.Serialize<TLCGenTemplatesModel>(setfile, Templates);
        }

        #endregion // ITemplatesProvider
    }
}
