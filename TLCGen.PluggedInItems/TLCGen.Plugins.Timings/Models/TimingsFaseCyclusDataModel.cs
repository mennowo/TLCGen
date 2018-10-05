using System;
using System.Collections.Generic;
using TLCGen.Models;

namespace TLCGen.Plugins.Timings.Models
{
    [Serializable]
    public class TimingsFaseCyclusDataModel
    {
        [RefersTo]
        public string FaseCyclus { get; set; }

        public TimingsFaseCyclusTypeEnum ConflictType { get; set; }

        public TimingsFaseCyclusDataModel()
        {
        }
    }
}
