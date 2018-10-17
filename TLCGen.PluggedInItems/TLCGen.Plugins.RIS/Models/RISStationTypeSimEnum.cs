namespace TLCGen.Plugins.RIS.Models
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
}
