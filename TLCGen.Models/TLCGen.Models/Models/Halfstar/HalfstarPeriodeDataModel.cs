using System;

namespace TLCGen.Models
{
	[Serializable]
	[RefersTo("Periode")]
	public class HalfstarPeriodeDataModel
	{
		#region Properties

		public string Periode { get; set; }

		#endregion // Properties
	}
}