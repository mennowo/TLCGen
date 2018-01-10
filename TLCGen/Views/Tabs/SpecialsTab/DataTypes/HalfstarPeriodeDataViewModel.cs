using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Views.Tabs.SpecialsTab.DataTypes
{
	public class HalfstarPeriodeDataViewModel : ViewModelBase, IViewModelWithItem
	{

		#region Properties

		public HalfstarPeriodeDataModel PeriodeData { get; }

		public string Periode
		{
			get => PeriodeData.Periode;
		}

		#endregion // Properties

		#region IViewModelWithItem

		public object GetItem()
		{
			return PeriodeData;
		}

		#endregion // IViewModelWithItem

		#region Constructor

		public HalfstarPeriodeDataViewModel(HalfstarPeriodeDataModel periodeData)
		{
			PeriodeData = periodeData;
		}

		#endregion // Constructor
	}
}