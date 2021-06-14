using GalaSoft.MvvmLight;

namespace TLCGen.Plugins.AutoBuild
{
	public class AutoBuildSettingsViewModel : ViewModelBase
	{
		public AutoBuildSettingsModel Settings { get; }

		public string MSBuildPath
		{
			get => Settings?.MSBuildPath;
			set
			{
				if(Settings != null)
				{
					Settings.MSBuildPath = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool ToolBarVisibility
		{
			get => Settings?.ToolBarVisibility == true;
			set
			{
				if (Settings != null)
				{
					Settings.ToolBarVisibility = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool TabVisibility
		{
			get => Settings?.TabVisibility == true;
			set
			{
				if (Settings != null)
				{
					Settings.TabVisibility = value;
					RaisePropertyChanged();
				}
			}
		}

		public AutoBuildSettingsViewModel(AutoBuildSettingsModel settings)
		{
			Settings = settings;
		}
	}
}
