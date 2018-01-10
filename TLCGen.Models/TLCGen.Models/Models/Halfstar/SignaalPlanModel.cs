using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TLCGen.Models
{
	[Serializable]
	public class SignaalPlanModel
	{
		#region Fields
		#endregion // Fields

		#region Properties

		[ModelName]
		[Browsable(false)]
		public string Naam { get; set; }
		public string Commentaar { get; set; }
		public int Cyclustijd { get; set; }
		public int StartMoment { get; set; }
		public int SwitchMoment { get; set; }
		public List<SignaalPlanFaseModel> Fasen { get; set; }

		#endregion // Properties

		#region Constructor

		public SignaalPlanModel()
		{
			Fasen = new List<SignaalPlanFaseModel>();
		}

		#endregion // Constructor
	}
}
