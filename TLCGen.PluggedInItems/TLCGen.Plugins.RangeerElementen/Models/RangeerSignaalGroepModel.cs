using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.RangeerElementen.Models
{
    [Serializable]
    public class RangeerSignaalGroepModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string SignaalGroep { get; set; }
        public int RangeerIndex { get; set; }

        public RangeerSignaalGroepModel()
        {
        }
    }
}