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
        EigenConflictGroep,
        [Description("Alle conflictgroepen")]
        AlleConflictGroepen,
        [Description("Ingesteld maximum")]
        IngesteldMaximum
    }
}
