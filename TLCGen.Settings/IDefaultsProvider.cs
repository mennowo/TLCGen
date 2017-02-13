using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Settings
{
    public interface IDefaultsProvider
    {
        TLCGenDefaultsModel Defaults { get; }

        void LoadSettings();
        void SaveSettings();

        void SetDefaultsOnModel(object model);
    }
}
