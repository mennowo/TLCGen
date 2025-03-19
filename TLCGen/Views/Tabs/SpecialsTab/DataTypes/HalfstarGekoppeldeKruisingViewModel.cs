using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Views.Tabs.SpecialsTab.DataTypes
{
    public class HalfstarGekoppeldeKruisingViewModel : ObservableObjectEx, IViewModelWithItem
	{
		#region Fields

		private readonly HalfstarGekoppeldeKruisingModel _gekoppeldeKruising;

        #endregion // Fields

        #region Properties

        public event EventHandler TypeChanged;

		public HalfstarGekoppeldeKruisingModel GekoppeldeKruising => _gekoppeldeKruising;

		public string KruisingNaam
		{
			get => _gekoppeldeKruising.KruisingNaam;
			set
			{
				_gekoppeldeKruising.KruisingNaam = value;
				foreach (var u in _gekoppeldeKruising.PlanUitgangen)
				{
					u.Kruising = value;
				}
                OnPropertyChanged(nameof(KruisingNaam), broadcast: true);
			}
		}
		
		public HalfstarGekoppeldTypeEnum Type
		{
			get => _gekoppeldeKruising.Type;
			set
			{
				_gekoppeldeKruising.Type = value;
                TypeChanged?.Invoke(this, EventArgs.Empty);
				foreach (var u in _gekoppeldeKruising.PlanUitgangen)
				{
					u.Type = value;
				}
                OnPropertyChanged(nameof(Type), broadcast: true);
            }
        }

		public HalfstarGekoppeldWijzeEnum KoppelWijze
		{
			get => _gekoppeldeKruising.KoppelWijze;
			set
			{
				_gekoppeldeKruising.KoppelWijze = value;
                OnPropertyChanged(nameof(KoppelWijze), broadcast: true);
                OnPropertyChanged(nameof(ShowPTPOptions));
				OnPropertyChanged(nameof(ShowKoppelsignalenOpties));
			}
		}

		public string PTPKruising
		{
			get => _gekoppeldeKruising.PTPKruising;
			set
			{
				if (value != null)
				{
					_gekoppeldeKruising.PTPKruising = value;
                    OnPropertyChanged(nameof(PTPKruising), broadcast: true);
                }
            }
		}

		public bool ShowPTPOptions => _gekoppeldeKruising.KoppelWijze == HalfstarGekoppeldWijzeEnum.PTP;

		public bool ShowKoppelsignalenOpties => _gekoppeldeKruising.KoppelWijze == HalfstarGekoppeldWijzeEnum.Koppelsignalen;

		#endregion // Properties
		
		#region Commands

		#endregion // Commands

		#region Command functionality

		#endregion // Command functionality

		#region Private methods

		#endregion // Private methods

		#region Public methods

		public object GetItem()
		{
			return _gekoppeldeKruising;
		}

		#endregion // Public Methods

		#region Constructor

		public HalfstarGekoppeldeKruisingViewModel(HalfstarGekoppeldeKruisingModel gekoppeldekruising)
		{
			_gekoppeldeKruising = gekoppeldekruising;
		}

		#endregion // Constructor

	}
}