using System.Windows.Controls;

namespace TLCGen.Plugins
{
    public interface ITLCGenGenerator : ITLCGenPlugin
    {
        UserControl GeneratorView { get; }

        bool CanGenerateController();
        void GenerateController();

        string GetGeneratorName();
        string GetGeneratorVersion();

    }
}