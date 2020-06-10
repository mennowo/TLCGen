using System;
using System.Collections.Generic;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	[IOElement("", BitmappedItemTypeEnum.Uitgang, "Naam")]
	public class SignaalPlanModel : IOElementModel, IHaveName, IComparable<SignaalPlanModel>
    {
		#region Fields
		#endregion // Fields

		#region Properties

        [RefersTo(TLCGenObjectTypeEnum.SignaalPlan)]
		[ModelName(TLCGenObjectTypeEnum.SignaalPlan)]
        [Browsable(false)]
		public override string Naam { get; set; }
        public override bool Dummy { get; set; }
        [HasDefault(false)]
        public string Commentaar { get; set; }
		public int Cyclustijd { get; set; }
		public int StartMoment { get; set; }
		public int SwitchMoment { get; set; }
		public List<SignaalPlanFaseModel> Fasen { get; set; }

        [Browsable(false)] [HasDefault(false)] public TLCGenObjectTypeEnum ObjectType => TLCGenObjectTypeEnum.SignaalPlan;

        #endregion // Properties

        #region IComparable<SignaalPlanModel>

        public int CompareTo(SignaalPlanModel other)
        {
            return this.Naam.CompareTo(other.Naam);
        }

        #endregion // IComparable<SignaalPlanModel>

        #region Constructor

        public SignaalPlanModel()
		{
			Fasen = new List<SignaalPlanFaseModel>();
		}

        #endregion // Constructor
    }
}
