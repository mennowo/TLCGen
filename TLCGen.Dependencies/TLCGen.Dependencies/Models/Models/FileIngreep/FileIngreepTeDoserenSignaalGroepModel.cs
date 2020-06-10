using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FileIngreepTeDoserenSignaalGroepModel : IComparable
    {
        [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
        public string FaseCyclus { get; set; }
        public int DoseerPercentage { get; set; }
        public bool AfkappenOpStartFile { get; set; }
        public int AfkappenOpStartFileMinGroentijd { get; set; }
        public bool MinimaleRoodtijd { get; set; }
        public int MinimaleRoodtijdTijd { get; set; }
        public bool MaximaleGroentijd { get; set; }
        public int MaximaleGroentijdTijd { get; set; }

        public int CompareTo(object obj)
        {
            return FaseCyclus.CompareTo(((FileIngreepTeDoserenSignaalGroepModel)obj).FaseCyclus);
        }
    }
}
