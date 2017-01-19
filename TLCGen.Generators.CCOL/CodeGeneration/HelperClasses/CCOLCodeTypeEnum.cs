using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public enum CCOLCodeType
    {
        Includes,
        KlokPerioden,
        Aanvragen,
        Maxgroen,
        Wachtgroen,
        Meetkriterium,
        Meeverlengen,
        RealisatieAfhandeling,
        InitApplication,
        Application,
        SystemApplication,
        DumpApplication
    };
}
