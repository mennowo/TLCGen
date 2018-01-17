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
				RaisePropertyChanged();
			}
		}

		public string Commentaar
		{
			get => SignaalPlan.Commentaar;
			set
			{
				SignaalPlan.Commentaar = value;
				RaisePropertyChanged();
			}
		}

		public int Cyclustijd
		{
			get => SignaalPlan.Cyclustijd;
			set
			{
				SignaalPlan.Cyclustijd = value;
				RaisePropertyChanged();
			}
		}

		public int StartMoment
		{
			get => SignaalPlan.StartMoment;
			set
			{
				SignaalPlan.StartMoment = value;
				RaisePropertyChanged();
			}
		}

		public int SwitchMoment
		{
			get => SignaalPlan.SwitchMoment;
			set
			{
				SignaalPlan.SwitchMoment = value;
				RaisePropertyChanged();
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