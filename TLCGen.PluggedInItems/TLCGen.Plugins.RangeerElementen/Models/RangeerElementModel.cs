using System;
using TLCGen.Models;

namespace TLCGen.Plugins.RangeerElementen.Models
{
    [Serializable]
    public class RangeerElementModel
    {
        [RefersTo]
        public string Element { get; set; }
        public int RangeerIndex { get; set; }

        public RangeerElementModel()
        {
        }
    }
}
