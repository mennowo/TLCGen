using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
	public class HalfstarPeriodeDataViewModel : ObservableObjectEx, IViewModelWithItem
	{
		#region Properties

		public HalfstarPeriodeDataModel PeriodeData { get; }

		public string Periode => PeriodeData.Periode;

        public string Signaalplan
		{
			get => PeriodeData.Signaalplan;
			set
			{
				PeriodeData.Signaalplan = value;
                OnPropertyChanged(nameof(Signaalplan), broadcast: true);
            }
        }

		public bool VARegelen
		{
			get => PeriodeData.VARegelen;
			set
			{
				PeriodeData.VARegelen = value;
                OnPropertyChanged(nameof(VARegelen), broadcast: true);
            }
        }

        public bool AlternatievenVoorHoofdrichtingen
        {
            get => PeriodeData.AlternatievenVoorHoofdrichtingen;
            set
            {
                PeriodeData.AlternatievenVoorHoofdrichtingen = value;
                OnPropertyChanged(nameof(AlternatievenVoorHoofdrichtingen), broadcast: true);
            }
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