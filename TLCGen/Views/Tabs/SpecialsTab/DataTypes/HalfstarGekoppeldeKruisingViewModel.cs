using System;
using System.Web.UI.WebControls;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Views.Tabs.SpecialsTab.DataTypes
{
	public class HalfstarGekoppeldeKruisingViewModel : ViewModelBase, IViewModelWithItem
	{
		#region Fields

		private HalfstarGekoppeldeKruisingModel _gekoppeldeKruising;
		private bool _showPTPOptions;
		private bool _showKoppelsignalenOpties;

		#endregion // Fields
		
		#region Properties

		public string KruisingNaam
		{
			get { return _gekoppeldeKruising.KruisingNaam; }
			set
			{
				_gekoppeldeKruising.KruisingNaam = value;
				RaisePropertyChanged();
			}
		}
		
		public HalfstarGekoppeldTypeEnum Type
		{
			get { return _gekoppeldeKruising.Type; }
			set
			{
				_gekoppeldeKruising.Type = value;
				RaisePropertyChanged();
			}
		}

		public HalfstarGekoppeldWijzeEnum KoppelWijze
		{
			get { return _gekoppeldeKruising.KoppelWijze; }
			set
			{
				_gekoppeldeKruising.KoppelWijze = value;
				RaisePropertyChanged();
				RaisePropertyChanged(nameof(ShowPTPOptions));
				RaisePropertyChanged(nameof(ShowKoppelsignalenOpties));
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
					RaisePropertyChanged();
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