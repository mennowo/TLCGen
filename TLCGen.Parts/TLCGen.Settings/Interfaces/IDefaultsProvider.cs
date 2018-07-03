using System;
using TLCGen.Models;

namespace TLCGen.Settings
{
    public interface IDefaultsProvider
    {
        ControllerModel Controller { get; set; }
        TLCGenDefaultsModel Defaults { get; set; }

        event EventHandler DefaultsChanged;

        void LoadSettings();
        void SaveSettings();

        void SetDefaultsOnModel(object model, string selector1 = null, string selector2 = null, bool onlyvalues = true);
    }
}
