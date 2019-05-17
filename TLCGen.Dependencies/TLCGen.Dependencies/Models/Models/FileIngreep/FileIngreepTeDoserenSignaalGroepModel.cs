using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    [RefersTo(TLCGenObjectTypeEnum.Fase, "FaseCyclus")]
    public class FileIngreepTeDoserenSignaalGroepModel : IComparable
    {
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int DoseerPercentage { get; set; }
        public bool AfkappenOpStartFile { get; set; }
        public int AfkappenOpStartFileMinGroentijd { get; set; }
        public bool MinimaleRoodtijd { get; set; }
        public int MinimaleRoodtijdTijd { get; set; }

        public int CompareTo(object obj)
        {
            return FaseCyclus.CompareTo(((FileIngreepTeDoserenSignaalGroepModel)obj).FaseCyclus);
        }
    }
}
