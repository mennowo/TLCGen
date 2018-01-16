using System;
using System.Collections.Generic;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	public class HalfstarDataModel
	{
		#region Properties

		public bool IsHalfstar { get; set; }
		public HalfstarTypeEnum Type { get; set; }
		public string DefaultPeriodeSignaalplan { get; set; }
		public HalfstarVARegelenTypeEnum TypeVARegelen { get; set; }
		public bool DefaultPeriodeVARegelen { get; set; }
		public bool DefaultPeriodeAlternatievenVoorHoofdrichtingen { get; set; }
		public bool OVPrioriteitPL { get; set; }
		public bool VARegelen { get; set; }
		public bool AlternatievenVoorHoofdrichtingen { get; set; }
		public List<HalfstarPeriodeDataModel> HalfstarPeriodenData { get; set; }
		public List<SignaalPlanModel> SignaalPlannen { get; set; }
		public List<HalfstarGekoppeldeKruisingModel> GekoppeldeKruisingen { get; set; }
		public List<HalfstarHoofdrichtingModel> Hoofdrichtingen { get; set; }
		
		#endregion // Properties

		#region Constructor

		public HalfstarDataModel()
		{
			SignaalPlannen = new List<SignaalPlanModel>();
			HalfstarPeriodenData = new List<HalfstarPeriodeDataModel>();
			GekoppeldeKruisingen = new List<HalfstarGekoppeldeKruisingModel>();
			Hoofdrichtingen = new List<HalfstarHoofdrichtingModel>();
		}

		#endregion // Constructor
	}
}