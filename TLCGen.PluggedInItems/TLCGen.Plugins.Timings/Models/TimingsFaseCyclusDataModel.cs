﻿using System;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins.Timings.Models
{
    [Serializable]
    public class TimingsFaseCyclusDataModel
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        public string FaseCyclus { get; set; }

        public TimingsFaseCyclusTypeEnum ConflictType { get; set; }

        public TimingsFaseCyclusDataModel()
        {
        }
    }
}
