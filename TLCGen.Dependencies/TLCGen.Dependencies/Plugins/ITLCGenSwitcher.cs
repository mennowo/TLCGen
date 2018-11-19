using System;
using TLCGen.Models;

namespace TLCGen.Plugins
{
	public interface ITLCGenSwitcher : ITLCGenPlugin
	{
		event EventHandler<ControllerModel> ControllerSet;
		event EventHandler<string> FileNameSet;
	}
}
