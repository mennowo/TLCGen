using System;
using System.Collections.Generic;
using System.ComponentModel;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	public class HalfstarGekoppeldeKruisingModel
	{
		#region Properties

		public string KruisingNaam { get; set; }
		public HalfstarGekoppeldTypeEnum Type { get; set; }
		public HalfstarGekoppeldWijzeEnum KoppelWijze { get; set; }
		public string PTPKruising { get; set; }

		[Browsable(false)]
		public bool IsSlave => Type == HalfstarGekoppeldTypeEnum.Slave;

		[Browsable(false)]
		public bool IsMaster => Type == HalfstarGekoppeldTypeEnum.Master;

		[Browsable(false)]
		[IOElement("leven", BitmappedItemTypeEnum.Uitgang, "KruisingNaam")]
		public BitmapCoordinatenDataModel UitLeven { get; set; }

		[Browsable(false)]
		[IOElement("kp", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
		public BitmapCoordinatenDataModel UitKoppelpuls { get; set; }

		[Browsable(false)]
		[IOElement("pervar", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
		public BitmapCoordinatenDataModel UitPeriodeVARegelen { get; set; }

		[Browsable(false)]
		[IOElement("perarh", BitmappedItemTypeEnum.Uitgang, "KruisingNaam", "IsSlave")]
		public BitmapCoordinatenDataModel UitPeriodenAlternatievenHoofdrichtingen { get; set; }

		[Browsable(false)]
		public List<HalfstarGekoppeldeKruisingPlanUitgangModel> PlanUitgangen { get; set; }

		[Browsable(false)]
		[IOElement("leven", BitmappedItemTypeEnum.Ingang, "KruisingNaam")]
		public BitmapCoordinatenDataModel InLeven { get; set; }

		[Browsable(false)]
		[IOElement("syncok", BitmappedItemTypeEnum.Ingang, "KruisingNaam", "IsSlave")]
		public BitmapCoordinatenDataModel InSynchronisatieOk { get; set; }

		[Browsable(false)]
		[IOElement("txsok", BitmappedItemTypeEnum.Ingang, "KruisingNaam", "IsSlave")]
		public BitmapCoordinatenDataModel InTxsOk { get; set; }

		#endregion // Properties

		public HalfstarGekoppeldeKruisingModel()
		{
			UitLeven = new BitmapCoordinatenDataModel();
			UitKoppelpuls = new BitmapCoordinatenDataModel();
			UitPeriodeVARegelen = new BitmapCoordinatenDataModel();
			UitPeriodenAlternatievenHoofdrichtingen = new BitmapCoordinatenDataModel();
			PlanUitgangen = new List<HalfstarGekoppeldeKruisingPlanUitgangModel>();
			InLeven = new BitmapCoordinatenDataModel();
			InSynchronisatieOk = new BitmapCoordinatenDataModel();
			InTxsOk = new BitmapCoordinatenDataModel();
		}
	}
}