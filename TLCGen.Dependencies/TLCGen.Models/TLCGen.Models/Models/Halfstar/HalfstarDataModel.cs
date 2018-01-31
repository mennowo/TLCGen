using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		[Browsable(false)]
		[IOElement("mlact", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel MlActUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("plact", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel PlActUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("kpact", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel KpActUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("mlpl", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel MlPlUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("txtimer", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel TxTimerUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("klok", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel KlokUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("hand", BitmappedItemTypeEnum.Uitgang, conditionprop:"IsHalfstar")]
		public BitmapCoordinatenDataModel HandUitgang { get; set; }
		
		[Browsable(false)]
		[IOElement("master", BitmappedItemTypeEnum.Uitgang, conditionprop: "IsNotMaster")]
		public BitmapCoordinatenDataModel MasterUitgang { get; set; }

		[Browsable(false)]
		public bool IsNotMaster => IsHalfstar && Type != HalfstarTypeEnum.Master;
		
		#endregion // Properties

		#region Constructor

		public HalfstarDataModel()
		{
			SignaalPlannen = new List<SignaalPlanModel>();
			HalfstarPeriodenData = new List<HalfstarPeriodeDataModel>();
			GekoppeldeKruisingen = new List<HalfstarGekoppeldeKruisingModel>();
			Hoofdrichtingen = new List<HalfstarHoofdrichtingModel>();
			
			MlActUitgang = new BitmapCoordinatenDataModel();
			PlActUitgang = new BitmapCoordinatenDataModel();
			KpActUitgang = new BitmapCoordinatenDataModel();
			MlPlUitgang = new BitmapCoordinatenDataModel();
			TxTimerUitgang = new BitmapCoordinatenDataModel();
			KlokUitgang = new BitmapCoordinatenDataModel();
			HandUitgang = new BitmapCoordinatenDataModel();
			MasterUitgang = new BitmapCoordinatenDataModel();
		}

		#endregion // Constructor
	}
}