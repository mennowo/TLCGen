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
		public string DefaultSignaalplan { get; set; }
		public bool DefaultVARegelen { get; set; }
		public List<HalfstarPeriodeDataModel> HalfstarPeriodenData { get; set; }
		public List<SignaalPlanModel> SignaalPlannen { get; set; }
		public List<HalfstarGekoppeldeKruisingModel> GekoppeldeKruisingen { get; set; }
		
		#endregion // Properties

		#region Constructor

		public HalfstarDataModel()
		{
			SignaalPlannen = new List<SignaalPlanModel>();
			HalfstarPeriodenData = new List<HalfstarPeriodeDataModel>();
			GekoppeldeKruisingen = new List<HalfstarGekoppeldeKruisingModel>();
		}

		#endregion // Constructor
	}
}