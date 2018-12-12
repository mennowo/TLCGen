using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Dependencies.Providers
{
    public interface ITLCGenFileAccessProvider
    {
        string[] ReadAllLines(string filename);
    }

    public class TLCGenFileAccessProvider: ITLCGenFileAccessProvider
    {
        private static ITLCGenFileAccessProvider _default;

        public static ITLCGenFileAccessProvider Default
        {
            get => _default ?? (_default = new TLCGenFileAccessProvider());
            set
            {
                _default = value;
            }
        }

        public string[] ReadAllLines(string filename)
        {
            return File.ReadAllLines(filename);
        }
    }
}
