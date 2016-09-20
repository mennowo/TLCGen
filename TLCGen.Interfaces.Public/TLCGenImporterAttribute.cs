using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces.Public
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenImporterAttribute : System.Attribute
    {
        //public enum ImporterTypeEnum { New, Existing }

        //public ImporterTypeEnum ImporterType { get; set; }

        public TLCGenImporterAttribute()
        {
        //    ImporterType = ImporterTypeEnum.New;
        }

        //public TLCGenImporterAttribute(ImporterTypeEnum e)
        //{
        //    ImporterType = e;
        //}
    }
}
