﻿using System.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Models.Enumerations
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum IngangTypeEnum
    {
        [Description("Wisselcontact")]
        WisselContact,
        [Description("Overig")]
        Overig,
        [Description("OV in/uitmelding")]
        OVmelding,
    }
}