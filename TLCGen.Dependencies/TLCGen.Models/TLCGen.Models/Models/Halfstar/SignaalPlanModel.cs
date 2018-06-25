using System;
using System.Collections.Generic;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	[IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
	public class SignaalPlanModel : IOElementModel, IHaveName
    {
		#region Fields
		#endregion // Fields

		#region Properties

		[ModelName(TLCGenObjectTypeEnum.SignaalPlan)]
		[Browsable(false)]
		public override string Naam { get; set; }
        [HasDefault(false)]
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
