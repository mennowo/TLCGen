namespace TLCGen.Models
{
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

    public enum RISVehicleRole
    {
        DEFAULT = 0,
        PUBLICTRANSPORT = 1,
        SPECIALTRANSPORT = 2,
        DANGEROUSGOODS = 3,
        ROADWORK = 4,
        RESCUE = 5,
        EMERGENCY = 6,
        SAFETYCAR = 7,
        AGRICULTURE = 8,
        COMMERCIAL = 9,
        MILITARY = 10,
        ROADOPERATOR = 11,
        TAXI = 12
    }

    public enum RISVehicleSubrole
    {
        UNKNOWN = 0,
        BUS = 1,
        TRAM = 2,
        METRO = 3,
        TRAIN = 4,
        EMERGENCY = 5,
        SMOOTH = 6,
        TIMETABLE = 7,
        INTERVAL = 8,
        EXPRESSTRANSIT = 9,
        NOSERVICE = 10,
        PLATOON = 11,
        ECODRIVING = 12
    }
}
