using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.SpecialsDenHaag.OVMModule
{
	public class DVMModuleDenHaag
	{
		#region Fields
		#endregion // Fields

		#region Properties
		#endregion // Properties

		#region Public methods

		public List<CCOLElement> GetCCOLElements(ControllerModel c)
		{
			var elems = new List<CCOLElement>();

			foreach (var fc in c.Fasen)
			{
				if (fc.Type == FaseTypeEnum.Auto || fc.Type == FaseTypeEnum.OV)
				{
					
				}
			}

			return elems;
		}

		#endregion // Public methods

		#region Constructor

		public DVMModuleDenHaag()
		{
			
		}

		#endregion // Constructor
	}
}
