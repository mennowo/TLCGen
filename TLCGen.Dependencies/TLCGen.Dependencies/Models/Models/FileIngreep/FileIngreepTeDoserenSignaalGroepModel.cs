using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
    [Serializable]
    public class FileIngreepTeDoserenSignaalGroepModel : IComparable, IEquatable<FileIngreepTeDoserenSignaalGroepModel>
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

        public bool Equals(FileIngreepTeDoserenSignaalGroepModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FaseCyclus == other.FaseCyclus;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileIngreepTeDoserenSignaalGroepModel)obj);
        }

        public override int GetHashCode()
        {
            return (FaseCyclus != null ? FaseCyclus.GetHashCode() : 0);
        }
    }
}
