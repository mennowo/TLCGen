using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces.Public
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenGeneratorAttribute : System.Attribute
    {
        public TLCGenGeneratorAttribute()
        {
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class TLCGenGeneratorSettingAttribute : System.Attribute
    {
        public string SettingType { get; set; }

        /// <summary>
        /// Property Attribute to indicate a property is meant as setting for a
        /// class that implements IGenerator.
        /// </summary>
        /// <param name="type">The type of setting. Can be "application" for an application wide setting
        /// of "controller" for a setting per controller</param>
        public TLCGenGeneratorSettingAttribute(string type)
        {
            SettingType = type;
        }
    }
}
