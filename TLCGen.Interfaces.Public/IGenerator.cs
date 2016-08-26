using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Interfaces.Public
{
    public interface IGenerator
    {
        string GenerateSourceFiles(ControllerModel controller, string sourcefilepath);
        string GenerateProjectFiles(ControllerModel controller, string projectfilepath);
        string GenerateSpecification(ControllerModel controller, string specificationfilepath);

        string GetGeneratorName();
        string GetGeneratorVersion();
    }
}
