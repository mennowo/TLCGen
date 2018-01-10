using System;
using System.Collections.Generic;

namespace TLCGen.Models
{
	[Serializable]
	public class HalfstarDataModel
	{
		#region Properties

		public bool IsHalfstar { get; set; }
		public List<SignaalPlanModel> SignaalPlannen { get; set; }
		
		#endregion // Properties

		#region Constructor

		public HalfstarDataModel()
		{
			SignaalPlannen = new List<SignaalPlanModel>();
		}

		#endregion // Constructor
	}
}