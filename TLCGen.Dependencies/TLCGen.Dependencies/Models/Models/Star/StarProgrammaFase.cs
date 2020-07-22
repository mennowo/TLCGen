using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class StarProgrammaFase
    {
        #region Properties

        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int Start1 { get; set; }
        public int Eind1 { get; set; }
        public int? Start2 { get; set; }
        public int? Eind2 { get; set; }

        #endregion // Properties
    }
}