using CommunityToolkit.Mvvm.ComponentModel;

namespace TLCGen.Plugins.AutoBuild
{
	public class AutoBuildSettingsViewModel : ObservableObject
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
					OnPropertyChanged();
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
					OnPropertyChanged();
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
					OnPropertyChanged();
				}
			}
		}

		public AutoBuildSettingsViewModel(AutoBuildSettingsModel settings)
		{
			Settings = settings;
		}
	}
}
