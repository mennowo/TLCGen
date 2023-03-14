using System;
using System.ComponentModel;

namespace TLCGen.Models
{
    [Flags]
    public enum RISStationTypeSimEnum
    {
        UNKNOWN = 0x0001,
        PEDESTRIAN = 0x0002,
        CYCLIST = 0x0004,
        MOPED = 0x0008,
        MOTORCYCLE = 0x0010,
        PASSENGERCAR = 0x0020,
        BUS = 0x0040,
        LIGHTTRUCK = 0x0080,
        HEAVYTRUCK = 0x0100,
        TRAILER = 0x0200,
        SPECIALVEHICLES = 0x0400,
        TRAM = 0x0800,
        ROADSIDEUNIT = 0x8000
    }

    [Flags]
    public enum RISVehicleRole
    {
        DEFAULT = 0x0001,
        PUBLICTRANSPORT = 0x0002,
        SPECIALTRANSPORT = 0x0004,
        DANGEROUSGOODS = 0x0008,
        ROADWORK = 0x0010,
        RESCUE = 0x0020,
        EMERGENCY = 0x0040,
        SAFETYCAR = 0x0080,
        AGRICULTURE = 0x0100,
        COMMERCIAL = 0x0200,
        MILITARY = 0x0400,
        ROADOPERATOR = 0x0800,
        TAXI = 0x1000
    }

    [Flags]
    public enum RISVehicleSubrole
    {
        UNKNOWN = 0x0001,
        BUS = 0x0002,
        TRAM = 0x0004,
        METRO = 0x0008,
        TRAIN = 0x0010,
        EMERGENCY = 0x0020,
        SMOOTH = 0x0040,
        TIMETABLE = 0x0080,
        INTERVAL = 0x0100,
        EXPRESSTRANSIT = 0x0200,
        NOSERVICE = 0x0400,
        PLATOON = 0x0800,
        ECODRIVING = 0x1000
    }

    [Flags]
    public enum RISVehicleImportance
    {
        [Description("1")]
        I01 = 0x0001,
        [Description("2")]
        I02 = 0x0002,
        [Description("3")]
        I03 = 0x0004,
        [Description("4")]
        I04 = 0x0008,
        [Description("5")]
        I05 = 0x0010,
        [Description("6")]
        I06 = 0x0020,
        [Description("7")]
        I07 = 0x0040,
        [Description("8")]
        I08 = 0x0080,
        [Description("9")]
        I09 = 0x0100,
        [Description("10")]
        I10 = 0x0200,
        [Description("11")]
        I11 = 0x0400,
        [Description("12")]
        I12 = 0x0800,
        [Description("13")]
        I13 = 0x1000,
        [Description("14")]
        I14 = 0x2000
    }
}
