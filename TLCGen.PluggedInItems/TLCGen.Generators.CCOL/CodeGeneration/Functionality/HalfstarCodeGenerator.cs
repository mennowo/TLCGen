using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
	public class HalfstarCodeGenerator : CCOLCodePieceGeneratorBase
	{
		private List<CCOLElement> _myElements;

#pragma warning disable 0649
		private string _usmlact;
		private string _usplact;
		private string _uskpact;
		private string _usmlpl;
		private string _ustxtimer;
		private string _usmaster;
		private string _usslave;
		private string _usklok;
		private string _ushand;
		private string _usinleven;
		private string _usinsyncok;
		private string _usintxsok;
		private string _usuitkpuls;
		private string _usuitpervar;
		private string _usuitperarh;
		private string _usuitpl;

		private string _mklok;
		private string _mhand;
		private string _hkpact;
		private string _hplact;
		private string _hmlact;
		private string _schvar;
		private string _schvarper;
		private string _hvarper;
		private string _scharhper;
		private string _harhper;
		private string _schvarstreng;
		private string _schvaml;
		private string _hplhulpdienst;
		private string _schovpriople;
		private string _prmplxper;
		private string _tin;
		private string _hxpl;
		private string _schinst;
#pragma warning restore 0649

		public override void CollectCCOLElements(ControllerModel c)
		{
			_myElements = new List<CCOLElement>();

			if (c.HalfstarData.IsHalfstar)
			{

			}
			
		}

		public override bool HasCCOLElements()
		{
			return true;
		}

		public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
		{
			return _myElements.Where(x => x.Type == type);
		}

		public override int HasCode(CCOLCodeTypeEnum type)
		{
			switch (type)
			{
				default:
					return 0;
			}
		}

		public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
		{
			StringBuilder sb = new StringBuilder();

			switch (type)
			{
				default:
					return null;
			}
		}
	}
}
