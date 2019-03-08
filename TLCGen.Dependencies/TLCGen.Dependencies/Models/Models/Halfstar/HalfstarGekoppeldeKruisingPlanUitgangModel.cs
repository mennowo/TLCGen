using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
    [IOElement("uit", BitmappedItemTypeEnum.Uitgang, "IOName", "IsSlave")]
	public class HalfstarGekoppeldeKruisingPlanUitgangModel : IOElementModel, IComparable
	{
		public override string Naam { get; set; }
        public override bool Dummy { get; set; }

        [HasDefault(false)]
		public string Plan { get; set; }
        [HasDefault(false)]
        public string Kruising { get; set; }
        public HalfstarGekoppeldTypeEnum Type { get; set; }
		
		public bool IsSlave => Type == HalfstarGekoppeldTypeEnum.Slave;

        public string IOName => Kruising + Plan;

        public int CompareTo(object obj)
		{
			return string.Compare(Plan, ((HalfstarGekoppeldeKruisingPlanUitgangModel) obj).Plan, StringComparison.Ordinal);
		}
	}
}