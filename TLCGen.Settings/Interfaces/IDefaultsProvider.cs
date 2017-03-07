using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public interface IDefaultsProvider
    {
        ControllerModel Controller { get; set; }
        TLCGenDefaultsModel Defaults { get; set; }

        void LoadSettings();
        void SaveSettings();

        void SetDefaultsOnModel(object model, string selector1 = null, string selector2 = null);
    }
}
