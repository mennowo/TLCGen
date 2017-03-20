using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public enum CCOLRegCCodeTypeEnum
    {
        Includes,
        Top,
        KwcApplication,
        PreApplication,
        KlokPerioden,
        Aanvragen,
        Verlenggroen,
        Maxgroen,
        Wachtgroen,
        Meetkriterium,
        FileVerwerking,
        DetectieStoring,
        Meeverlengen,
        RealisatieAfhandelingModules,
        RealisatieAfhandelingNaModules,
        RealisatieAfhandeling,
        Alternatieven,
        Synchronisaties,
        InitApplication,
        Application,
        PostApplication,
        PreSystemApplication,
        SystemApplication,
        PostSystemApplication,
        DumpApplication
    };
}
