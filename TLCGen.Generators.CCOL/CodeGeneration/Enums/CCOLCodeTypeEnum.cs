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
        KlokPerioden,
        Aanvragen,
        Maxgroen,
        Wachtgroen,
        Meetkriterium,
        FileVerwerking,
        Meeverlengen,
        RealisatieAfhandelingModules,
        RealisatieAfhandeling,
        InitApplication,
        Application,
        PreSystemApplication,
        SystemApplication,
        PostSystemApplication,
        DumpApplication
    };
}
