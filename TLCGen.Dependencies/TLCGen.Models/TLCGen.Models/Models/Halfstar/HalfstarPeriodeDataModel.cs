using System;

namespace TLCGen.Models
{
	[Serializable]
	[RefersTo("Periode")]
	public class HalfstarPeriodeDataModel
	{
		#region Properties

        [HasDefault(false)]
		public string Periode { get; set; }
        [HasDefault(false)]
		public string Signaalplan { get; set; }
		public bool VARegelen { get; set; }
		public bool AlternatievenVoorHoofdrichtingen { get; set; }

		#endregion // Properties
	}
}