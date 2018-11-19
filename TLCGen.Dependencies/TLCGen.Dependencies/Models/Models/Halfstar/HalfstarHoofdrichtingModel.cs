using System;

namespace TLCGen.Models
{
	[Serializable]
	[RefersTo(nameof(FaseCyclus))]
	public class HalfstarHoofdrichtingModel
	{
		#region Properties

        [HasDefault(false)]
		public string FaseCyclus { get; set; }
		
		#endregion // Properties
	}
}