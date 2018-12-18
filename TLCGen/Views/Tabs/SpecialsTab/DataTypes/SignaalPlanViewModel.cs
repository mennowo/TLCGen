using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
	public class SignaalPlanViewModel : ViewModelBase, IViewModelWithItem
	{
		#region Properties

		public SignaalPlanModel SignaalPlan { get; }

		public string Naam
		{
			get => SignaalPlan.Naam;
			set
			{
				SignaalPlan.Naam = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
		}

		public string Commentaar
		{
			get => SignaalPlan.Commentaar;
			set
			{
				SignaalPlan.Commentaar = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

		public int Cyclustijd
		{
			get => SignaalPlan.Cyclustijd;
			set
			{
                if (value > 0)
                {
                    SignaalPlan.Cyclustijd = value;
                    if (StartMoment > value)
                    {
                        StartMoment = value;
                    }
                    if (SwitchMoment > value)
                    {
                        SwitchMoment = value;
                    }
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

		public int StartMoment
		{
			get => SignaalPlan.StartMoment;
			set
			{
                if (value > 0 && value <= Cyclustijd)
                {
				    SignaalPlan.StartMoment = value;
                }
			    RaisePropertyChanged<object>(broadcast: true);
            }
        }

		public int SwitchMoment
		{
			get => SignaalPlan.SwitchMoment;
			set
			{
                if (value > 0 && value <= Cyclustijd)
                {
                    SignaalPlan.SwitchMoment = value;
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
		}
		
		public ObservableCollectionAroundList<SignaalPlanFaseViewModel, SignaalPlanFaseModel> Fasen { get; private set; }

		#endregion // Properties

		#region IViewModelWithItem

		public object GetItem()
		{
			return SignaalPlan;
		}
		
		#endregion // IViewModelWithItem

		#region Constructor

		public SignaalPlanViewModel(SignaalPlanModel signaalPlan)
		{
			SignaalPlan = signaalPlan;
			Fasen = new ObservableCollectionAroundList<SignaalPlanFaseViewModel, SignaalPlanFaseModel>(SignaalPlan.Fasen);
		}

		#endregion // Constructor
	}
}