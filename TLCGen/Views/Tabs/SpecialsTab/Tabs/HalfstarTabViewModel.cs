using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.ViewModels
{
	[TLCGenTabItem(index: 5, type: TabItemTypeEnum.SpecialsTab)]
	public class HalfstarTabViewModel : TLCGenTabItemViewModel
	{
		#region Fields
		
		#endregion // Fields

		#region Properties

		public HalfstarDataModel HalfstarData;

		public ObservableCollectionAroundList<SignaalPlanViewModel, SignaalPlanModel> SignaalPlannen { get; private set; }

		public SignaalPlanViewModel SelectedSignaalPlan
		{
			get => _selectedSignaalPlan;
			set
			{
				_selectedSignaalPlan = value; 
				RaisePropertyChanged();
			}
		}

		public bool IsHalfstar
		{
			get => HalfstarData.IsHalfstar;
			set
			{
				HalfstarData.IsHalfstar = value;
				if (value)
				{
					// TODO: can make this generic?
					foreach (var sp in SignaalPlannen)
					{
						foreach (var fc in Controller.Fasen)
						{
							if (sp.Fasen.All(x => fc.Naam != x.FaseCyclus))
							{
								sp.Fasen.Add(new SignaalPlanFaseViewModel(new SignaalPlanFaseModel()
								{
									FaseCyclus = fc.Naam
								}));
							}
						}
						var rems = new List<SignaalPlanFaseViewModel>();
						foreach (var spfc in sp.Fasen)
						{
							if (Controller.Fasen.All(x => x.Naam != spfc.FaseCyclus))
							{
								rems.Add(spfc);
							}
						}

						foreach (var fc in rems)
						{
							sp.Fasen.Remove(fc);
						}
					}
				}
				RaisePropertyChanged();
			}
		}

		#endregion // Properties

		#region Commands

		private RelayCommand _addSignaalPlanCommand;
		public ICommand AddSignaalPlanCommand
		{
			get
			{
				if (_addSignaalPlanCommand == null)
				{
					_addSignaalPlanCommand = new RelayCommand(AddSignaalPlanCommand_Executed, AddSignaalPlanCommand_CanExecute);
				}
				return _addSignaalPlanCommand;
			}
		}

		private RelayCommand _removeSignaalPlanCommand;
		private SignaalPlanViewModel _selectedSignaalPlan;

		public ICommand RemoveSignaalPlanCommand
		{
			get
			{
				if (_removeSignaalPlanCommand == null)
				{
					_removeSignaalPlanCommand = new RelayCommand(RemoveSignaalPlanCommand_Executed, RemoveSignaalPlanCommand_CanExecute);
				}
				return _removeSignaalPlanCommand;
			}
		}

		#endregion // Commands

		#region Command functionality

		private bool AddSignaalPlanCommand_CanExecute(object obj)
		{
			return true;
		}

		private void AddSignaalPlanCommand_Executed(object obj)
		{
			var spl = new SignaalPlanModel()
			{
				Naam = "PL" + (SignaalPlannen.Count + 1)
			};
			foreach (var fc in Controller.Fasen)
			{
				spl.Fasen.Add(new SignaalPlanFaseModel
				{
					FaseCyclus = fc.Naam
				});
			}
			SignaalPlannen.Add(new SignaalPlanViewModel(spl));
		}

		private bool RemoveSignaalPlanCommand_CanExecute(object obj)
		{
			return SelectedSignaalPlan != null;
		}

		private void RemoveSignaalPlanCommand_Executed(object obj)
		{
			var i = SignaalPlannen.IndexOf(SelectedSignaalPlan);
			SignaalPlannen.Remove(SelectedSignaalPlan);
			SelectedSignaalPlan = null;
			if ((i - 1) <= (SignaalPlannen.Count - 1))
			{
				if (i - 1 >= 0)
				{
					SelectedSignaalPlan = SignaalPlannen[i - 1];
				}
				else
				{
					SelectedSignaalPlan = SignaalPlannen[0];
				}
			}

			var j = 1;
			foreach (var pl in SignaalPlannen)
			{
				pl.Naam = "PL" + j;
				++j;
			}
		}

		#endregion // Command functionality

		#region Private methods

		#endregion // Private methods

		#region Public methods

		#endregion // Public methods

		#region TLCGen TabItem overrides

		public override string DisplayName => "Halfstar";

		public override void OnSelected()
		{

		}

		public override ControllerModel Controller
		{
			get => base.Controller;
			set
			{
				base.Controller = value;
				if (base.Controller != null)
				{
					HalfstarData = Controller.HalfstarData;
					SignaalPlannen = new ObservableCollectionAroundList<SignaalPlanViewModel, SignaalPlanModel>(HalfstarData.SignaalPlannen);
				}
				else
				{
					HalfstarData = null;
					SignaalPlannen = null;
				}
			}
		}

		#endregion // TLCGen TabItem overrides

		#region TLCGen Events

		private void OnFasenChanged(FasenChangedMessage message)
		{
			if (message.AddedFasen != null && message.AddedFasen.Count > 0)
			{
				foreach (var fc in message.AddedFasen)
				{
					foreach (var pl in SignaalPlannen)
					{
						pl.Fasen.Add(new SignaalPlanFaseViewModel(new SignaalPlanFaseModel
						{
							FaseCyclus = fc.Naam
						}));
						pl.Fasen.BubbleSort();
					}
				}
			}

			if (message.RemovedFasen != null && message.RemovedFasen.Count > 0)
			{
				foreach (var fc in message.RemovedFasen)
				{
					foreach (var pl in SignaalPlannen)
					{
						var plfc = pl.Fasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
						if (plfc != null)
						{
							pl.Fasen.Remove(plfc);
						}

						pl.Fasen.BubbleSort();
					}
				}
			}
		}

		private void OnNameChanged(NameChangedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
		}

		private void OnFasenSorted(FasenSortedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
		}

		#endregion // TLCGen Events

		#region Constructor

		public HalfstarTabViewModel() : base()
		{
			Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
			Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
			Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
		}

		#endregion // Constructor
	}
}