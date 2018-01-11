using System;
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

		#endregion // Properties
	}
}