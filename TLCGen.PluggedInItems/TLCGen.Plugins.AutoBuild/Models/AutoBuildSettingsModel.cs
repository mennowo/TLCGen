using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Plugins.AutoBuild
{
	[Serializable]
	public class AutoBuildSettingsModel
	{
		public string MSBuildPath { get; set; }
		public bool ToolBarVisibility { get; set; }
		public bool TabVisibility { get; set; }
	}
}
