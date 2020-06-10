using System;
using TLCGen.Models.Enumerations;

namespace TLCGen.Models
{
	[Serializable]
	public class HalfstarHoofdrichtingModel
	{
		#region Properties

	    [RefersTo(TLCGenObjectTypeEnum.Fase)]
        [HasDefault(false)]
		public string FaseCyclus { get; set; }

        public bool Tegenhouden { get; set; }
        public bool AfkappenWG { get; set; }
        public bool AfkappenVG { get; set; }
		
		#endregion // Properties
	}
}