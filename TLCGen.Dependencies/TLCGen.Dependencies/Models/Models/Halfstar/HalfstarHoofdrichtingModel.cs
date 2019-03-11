using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	[RefersTo(TLCGenObjectTypeEnum.Fase, nameof(FaseCyclus))]
	public class HalfstarHoofdrichtingModel
	{
		#region Properties

        [HasDefault(false)]
		public string FaseCyclus { get; set; }
		
		#endregion // Properties
	}
}