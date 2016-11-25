using TLCGen.Models;

namespace TLCGen.Plugins
{
    public interface ITLCGenGenerator : ITLCGenPlugin
    {
        string GenerateSourceFiles(ControllerModel controller, string sourcefilepath);
        string GenerateProjectFiles(ControllerModel controller, string projectfilepath);
        string GenerateSpecification(ControllerModel controller, string specificationfilepath);

        string GetGeneratorName();
        string GetGeneratorVersion();
    }
}