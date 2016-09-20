using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces.Public
{

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class TLCGenCustomSettingAttribute : System.Attribute
    {
        public enum SettingTypeEnum { Application, Controller };

        public SettingTypeEnum SettingType { get; set; }

        /// <summary>
        /// Property Attribute to indicate a property is meant as setting for a
        /// class that implements IGenerator.
        /// </summary>
        /// <param name="type">The type of setting. Can be "application" for an application wide setting
        /// of "controller" for a setting per controller</param>
        public TLCGenCustomSettingAttribute(SettingTypeEnum type)
        {
            SettingType = type;
        }

        public TLCGenCustomSettingAttribute()
        {
            SettingType = SettingTypeEnum.Application;
        }
    }
}
