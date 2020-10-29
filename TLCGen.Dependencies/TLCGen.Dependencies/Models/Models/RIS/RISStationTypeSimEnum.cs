using System;

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
}
