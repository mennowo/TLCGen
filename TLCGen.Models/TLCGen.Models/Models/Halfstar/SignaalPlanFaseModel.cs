namespace TLCGen.Models
{
	[RefersToSignalGroup("FaseCyclus")]
	public class SignaalPlanFaseModel
	{
		#region Properties

		public string FaseCyclus { get; set; }
		public int? A1 { get; set; }
		public int B1 { get; set; }
		public int? C1 { get; set; }
		public int D1 { get; set; }
		public int? E1 { get; set; }
		public int? A2 { get; set; }
		public int? B2 { get; set; }
		public int? C2 { get; set; }
		public int? D2 { get; set; }
		public int? E2 { get; set; }

		#endregion // Properties
	}
}