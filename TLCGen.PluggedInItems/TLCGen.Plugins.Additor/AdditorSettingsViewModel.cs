using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Plugins.Additor;

namespace TLCGen.Plugins.Additor
{
	public class AdditorSettingsViewModel : ObservableObject
	{
		public AdditorSettingsModel Settings { get; }

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

		public AdditorSettingsViewModel(AdditorSettingsModel settings)
		{
			Settings = settings;
		}
	}
}
