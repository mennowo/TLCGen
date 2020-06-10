using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    public interface IHaveName
    {
        string Naam { get; set; }
        TLCGenObjectTypeEnum ObjectType { get; }
    }
}
