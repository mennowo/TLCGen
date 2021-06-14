using GalaSoft.MvvmLight;
using TLCGen.Plugins.Additor;

namespace TLCGen.Plugins.Additor
{
	public class AdditorSettingsViewModel : ViewModelBase
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
					RaisePropertyChanged();
				}
			}
		}

		public AdditorSettingsViewModel(AdditorSettingsModel settings)
		{
			Settings = settings;
		}
	}
}
