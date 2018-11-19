using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum RoBuGroverMethodeEnum
    {
        [Description("Eigen conflictgroep")]
        EigenConflictGroep = 2,
        [Description("Alle conflictgroepen")]
        AlleConflictGroepen = 1,
        [Description("Ingesteld maximum")]
        IngesteldMaximum = 0
    }
}
