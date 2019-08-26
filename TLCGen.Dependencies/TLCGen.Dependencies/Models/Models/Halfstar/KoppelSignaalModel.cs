using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class KoppelSignaalModel
    {
        public int Count { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [RefersTo(TLCGenObjectTypeEnum.PTPKruising)]
        public string Koppeling { get; set; }
        public KoppelSignaalRichtingEnum Richting { get; set; }
    }
}