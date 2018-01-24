using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
    [IOElement("uit", BitmappedItemTypeEnum.Uitgang, "IOName", "IsSlave")]
	public class HalfstarGekoppeldeKruisingPlanUitgangModel : IOElementModel, IComparable
	{
		public override string Naam { get; set; }
		
		public string Plan { get; set; }
		public string Kruising { get; set; }
		public HalfstarGekoppeldTypeEnum Type { get; set; }
		
		public bool IsSlave => Type == HalfstarGekoppeldTypeEnum.Master;

		public string IOName => Kruising + Plan;

		public int CompareTo(object obj)
		{
			return string.Compare(((HalfstarGekoppeldeKruisingPlanUitgangModel) obj).Plan, Plan, StringComparison.Ordinal);
		}
	}
}