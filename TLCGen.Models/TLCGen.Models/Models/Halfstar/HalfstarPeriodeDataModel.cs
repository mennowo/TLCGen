using System;

namespace TLCGen.Models
{
	[Serializable]
	[RefersTo("Periode")]
	public class HalfstarPeriodeDataModel
	{
		#region Properties

		public string Periode { get; set; }
		public string Signaalplan { get; set; }
		public bool VARegelen { get; set; }

		#endregion // Properties
	}
}