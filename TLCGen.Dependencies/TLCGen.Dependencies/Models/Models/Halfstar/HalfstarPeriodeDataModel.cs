using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	public class HalfstarPeriodeDataModel
	{
		#region Properties

	    [RefersTo(TLCGenObjectTypeEnum.Periode)]
        [HasDefault(false)]
		public string Periode { get; set; }
        [HasDefault(false)]
		public string Signaalplan { get; set; }
		public bool VARegelen { get; set; }
		public bool AlternatievenVoorHoofdrichtingen { get; set; }

		#endregion // Properties
	}
}