using System;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.Views.Tabs.SpecialsTab.DataTypes
{
	public class HalfstarHoofdrichtingViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
	{
		#region Properties

		public HalfstarHoofdrichtingModel Hoofdrichting { get; }

		public string FaseCyclus
		{
			get => Hoofdrichting.FaseCyclus;
			set
			{
				Hoofdrichting.FaseCyclus = value;
                OnPropertyChanged(nameof(FaseCyclus), broadcast: true);
            }
        }

        public bool Tegenhouden
        {
            get => Hoofdrichting.Tegenhouden;
            set
            {
                Hoofdrichting.Tegenhouden = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool AfkappenWG
        {
            get => Hoofdrichting.AfkappenWG;
            set
            {
                Hoofdrichting.AfkappenWG = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public bool AfkappenVG
        {
            get => Hoofdrichting.AfkappenVG;
            set
            {
                Hoofdrichting.AfkappenVG = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        #endregion // Properties

        #region IViewModelWithItem

        public object GetItem()
		{
			return Hoofdrichting;
		}

		#endregion // IViewModelWithItem

		#region IComparable

		public int CompareTo(object obj)
		{
			return string.Compare(FaseCyclus, ((HalfstarHoofdrichtingViewModel) obj).FaseCyclus, StringComparison.Ordinal);
		}

		#endregion // IComparable

		#region Constructor

		public HalfstarHoofdrichtingViewModel(HalfstarHoofdrichtingModel hoofdrichting)
		{
			Hoofdrichting = hoofdrichting;
		}

		#endregion // Constructor
	}
}