using System.Collections.Generic;
using System.Windows.Controls;
using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenGenerator : ITLCGenPlugin
    {
        UserControl GeneratorView { get; }

        bool CanGenerateController();
        void GenerateController();

        List<IOElementModel> GetAllIOElements(ControllerModel c);

        string GetGeneratorName();
        string GetGeneratorVersion();

    }
}