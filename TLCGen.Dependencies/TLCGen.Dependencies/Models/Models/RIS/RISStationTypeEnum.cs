using System.ComponentModel;

namespace TLCGen.Models
{
    public enum RISStationTypeEnum
    {
        [Description("unknown")]
        UNKNOWN = 0x0001,
        [Description("vtg")]
        PEDESTRIAN = 0x0002,
        [Description("fts")]
        CYCLIST = 0x0004,
        [Description("moped")]
        MOPED = 0x0008,
        [Description("motor")]
        MOTORCYCLE = 0x0010,
        [Description("auto")]
        PASSENGERCAR = 0x0020,
        [Description("bus")]
        BUS = 0x0040,
        [Description("ltruck")]
        LIGHTTRUCK = 0x0080,
        [Description("htruck")]
        HEAVYTRUCK = 0x0100,
        [Description("trailer")]
        TRAILER = 0x0200,
        [Description("special")]
        SPECIALVEHICLES = 0x0400,
        [Description("tram")]
        TRAM = 0x0800,
        [Description("rsu")]
        ROADSIDEUNIT = 0x8000,
        [Description("trucks")]
        TRUCKS = 0x0380,
        [Description("mveh")]
        MOTORVEHICLES = 0x7FF0,
        [Description("vehicles")]
        VEHICLES = 0x7FFC
    }
}
