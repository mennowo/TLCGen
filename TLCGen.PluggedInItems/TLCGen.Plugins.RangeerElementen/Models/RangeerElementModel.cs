using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.RangeerElementen.Models
{
    [Serializable]
    public class RangeerElementModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Detector)]
        public string Element { get; set; }
        public int RangeerIndex { get; set; }

        public RangeerElementModel()
        {
        }
    }
}
