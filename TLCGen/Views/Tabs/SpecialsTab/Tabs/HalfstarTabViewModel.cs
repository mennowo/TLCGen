using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Dialogs;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Views.Tabs.SpecialsTab.DataTypes;

namespace TLCGen.ViewModels
{
	[TLCGenTabItem(index: 5, type: TabItemTypeEnum.SpecialsTab)]
	public class HalfstarTabViewModel : TLCGenTabItemViewModel
	{
		#region Fields
		
		private SignaalPlanViewModel _selectedSignaalPlan;
		private HalfstarGekoppeldeKruisingViewModel _selectedHalfstarGekoppeldeKruising;
		private string _selectedHoofdRichtingToAdd;
		private HalfstarHoofdrichtingViewModel _selectedHoofdRichtingToRemove;

		#endregion // Fields

		#region Properties

		private HalfstarDataModel HalfstarData;

		public ObservableCollection<string> PTPKruisingenNames { get; } = new ObservableCollection<string>();
		public ObservableCollection<string> SignaalPlannenNames { get; } = new ObservableCollection<string>();
		public ObservableCollection<string> SelectableHoofdRichtingen { get; } = new ObservableCollection<string>();
		public ObservableCollectionAroundList<SignaalPlanViewModel, SignaalPlanModel> SignaalPlannen { get; private set; }
		public ObservableCollectionAroundList<HalfstarPeriodeDataViewModel, HalfstarPeriodeDataModel> HalfstarPeriodenData { get; private set; }
		public ObservableCollectionAroundList<HalfstarGekoppeldeKruisingViewModel, HalfstarGekoppeldeKruisingModel> GekoppeldeKruisingen { get; private set; }
		public ObservableCollectionAroundList<HalfstarHoofdrichtingViewModel, HalfstarHoofdrichtingModel> HoofdRichtingen { get; private set; }
		public ObservableCollectionAroundList<HalfstarFaseCyclusAlternatiefViewModel, HalfstarFaseCyclusAlternatiefModel> Alternatieven { get; private set; }

		public SignaalPlanViewModel SelectedSignaalPlan
		{
			get => _selectedSignaalPlan;
			set
			{
				_selectedSignaalPlan = value; 
				RaisePropertyChanged();
			}
		}

		public string SelectedHoofdRichtingToAdd
		{
			get => _selectedHoofdRichtingToAdd;
			set
			{
				_selectedHoofdRichtingToAdd = value; 
				RaisePropertyChanged();
			}
		}

		public HalfstarHoofdrichtingViewModel SelectedHoofdRichtingToRemove
		{
			get => _selectedHoofdRichtingToRemove;
			set
			{
				_selectedHoofdRichtingToRemove = value;
				RaisePropertyChanged();
			}
		}

		public HalfstarGekoppeldeKruisingViewModel SelectedHalfstarGekoppeldeKruising
		{
			get => _selectedHalfstarGekoppeldeKruising; set
			{
				_selectedHalfstarGekoppeldeKruising = value; 
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

                    UpdateAlternatievenFromController();
                    UpdatePeriodenData();
				}

				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public HalfstarTypeEnum Type
		{
			get => HalfstarData.Type;
			set
			{
				HalfstarData.Type = value;
				switch (value)
				{
					case HalfstarTypeEnum.Master:
						foreach (var k in GekoppeldeKruisingen)
						{
							k.Type = HalfstarGekoppeldTypeEnum.Slave;
						}
						break;
					case HalfstarTypeEnum.FallbackMaster:
						break;
					case HalfstarTypeEnum.Slave:
						foreach (var k in GekoppeldeKruisingen)
						{
							k.Type = HalfstarGekoppeldTypeEnum.Master;
						}
						break;
				}
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public string DefaultSignaalplanText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") plan";
		public string DefaultVARegelenText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") VA regelen";
		public string DefaultAlternatievenVoorHoofdrichtingenText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") hoofdr.alt.";

		public HalfstarVARegelenTypeEnum TypeVARegelen
		{
			get => HalfstarData.TypeVARegelen;
			set
			{
				HalfstarData.TypeVARegelen = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public bool VARegelen
		{
			get => HalfstarData.VARegelen;
			set
			{
				HalfstarData.VARegelen = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public bool OVPrioriteitPL
		{
			get => HalfstarData.OVPrioriteitPL;
			set
			{
				HalfstarData.OVPrioriteitPL = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public bool AlternatievenVoorHoofdrichtingen
		{
			get => HalfstarData.AlternatievenVoorHoofdrichtingen;
			set
			{
				HalfstarData.AlternatievenVoorHoofdrichtingen = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

        public bool PlantijdenInParameters
        {
            get => HalfstarData.PlantijdenInParameters;
            set
            {
                HalfstarData.PlantijdenInParameters = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string DefaultSignaalplan
		{
			get => HalfstarData.DefaultPeriodeSignaalplan; 
			set
			{
				HalfstarData.DefaultPeriodeSignaalplan = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public bool DefaultPeriodeVARegelen
		{
			get => HalfstarData.DefaultPeriodeVARegelen; 
			set
			{
				HalfstarData.DefaultPeriodeVARegelen = value;
				RaisePropertyChanged<object>(broadcast: true);
			}
		}

		public bool DefaultPeriodeAlternatievenVoorHoofdrichtingen
		{
			get => HalfstarData.DefaultPeriodeAlternatievenVoorHoofdrichtingen; 
			set
			{
				HalfstarData.DefaultPeriodeAlternatievenVoorHoofdrichtingen = value;
				RaisePropertyChanged<object>(broadcast: true);
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
		
		private RelayCommand _duplicateSignaalPlanCommand;
		public ICommand DuplicateSignaalPlanCommand
		{
			get
			{
				if (_duplicateSignaalPlanCommand == null)
				{
					_duplicateSignaalPlanCommand = new RelayCommand(DuplicateSignaalPlanCommand_Executed, DuplicateSignaalPlanCommand_CanExecute);
				}
				return _duplicateSignaalPlanCommand;
			}
		}

		private RelayCommand _importSignaalPlanCommand;
		public ICommand ImportSignaalPlanCommand
		{
			get
			{
				if (_importSignaalPlanCommand == null)
				{
					_importSignaalPlanCommand = new RelayCommand(ImportSignaalPlanCommand_Executed, ImportSignaalPlanCommand_CanExecute);
				}
				return _importSignaalPlanCommand;
			}
        }

        private RelayCommand _importManySignaalPlanCommand;
        public ICommand ImportManySignaalPlanCommand
        {
            get
            {
                if (_importManySignaalPlanCommand == null)
                {
                    _importManySignaalPlanCommand = new RelayCommand(ImportManySignaalPlanCommand_Executed, ImportManySignaalPlanCommand_CanExecute);
                }
                return _importManySignaalPlanCommand;
            }
        }

        private RelayCommand _addHoofdRichtingCommand;
		public ICommand AddHoofdRichtingCommand
		{
			get
			{
				if (_addHoofdRichtingCommand == null)
				{
					_addHoofdRichtingCommand = new RelayCommand(AddHoofdRichtingCommand_Executed, AddHoofdRichtingCommand_CanExecute);
				}
				return _addHoofdRichtingCommand;
			}
		}

		private RelayCommand _removeHoofdRichtingCommand;
		public ICommand RemoveHoofdRichtingCommand
		{
			get
			{
				if (_removeHoofdRichtingCommand == null)
				{
					_removeHoofdRichtingCommand = new RelayCommand(RemoveHoofdRichtingCommand_Executed, RemoveHoofdRichtingCommand_CanExecute);
				}
				return _removeHoofdRichtingCommand;
			}
		}

		private RelayCommand _addGekoppeldeKruisingCommand;
		public ICommand AddGekoppeldeKruisingCommand
		{
			get
			{
				if (_addGekoppeldeKruisingCommand == null)
				{
					_addGekoppeldeKruisingCommand = new RelayCommand(AddGekoppeldeKruisingCommand_Executed, AddGekoppeldeKruisingCommand_CanExecute);
				}
				return _addGekoppeldeKruisingCommand;
			}
		}

		private RelayCommand _removeGekoppeldeKruisingCommand;
		public ICommand RemoveGekoppeldeKruisingCommand
		{
			get
			{
				if (_removeGekoppeldeKruisingCommand == null)
				{
					_removeGekoppeldeKruisingCommand = new RelayCommand(RemoveGekoppeldeKruisingCommand_Executed, RemoveGekoppeldeKruisingCommand_CanExecute);
				}
				return _removeGekoppeldeKruisingCommand;
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
			var spl = new SignaalPlanModel
			{
				Naam = "PL" + (SignaalPlannen.Count + 1),
				StartMoment = 1, SwitchMoment = 1
			};
			foreach (var fc in Controller.Fasen)
			{
				spl.Fasen.Add(new SignaalPlanFaseModel
				{
					FaseCyclus = fc.Naam
				});
			}
			SignaalPlannen.Add(new SignaalPlanViewModel(spl));
			foreach (var gk in GekoppeldeKruisingen)
			{
				gk.GekoppeldeKruising.PlanUitgangen.Add(new HalfstarGekoppeldeKruisingPlanUitgangModel
				{
					Kruising = gk.KruisingNaam,
					Plan = spl.Naam,
					Type = gk.Type
				});
				gk.GekoppeldeKruising.PlanUitgangen.BubbleSort();
				gk.GekoppeldeKruising.PlanIngangen.Add(new HalfstarGekoppeldeKruisingPlanIngangModel
				{
					Kruising = gk.KruisingNaam,
					Plan = spl.Naam,
					Type = gk.Type
				});
				gk.GekoppeldeKruising.PlanIngangen.BubbleSort();
			}
            if (SelectedSignaalPlan == null) SelectedSignaalPlan = SignaalPlannen[0];
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
				else if (SignaalPlannen.Any())
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

			foreach (var per in HalfstarPeriodenData)
			{
				if (SignaalPlannen.Count == 0)
				{
					per.Signaalplan = null;
				}

				else
				{
					if (SignaalPlannen.All(x => x.Naam != per.Signaalplan))
					{
						per.Signaalplan = SignaalPlannen[0].Naam;
					}
				}
			}

			foreach (var gk in GekoppeldeKruisingen)
			{
				var plu = gk.GekoppeldeKruising.PlanUitgangen.FirstOrDefault(x => SignaalPlannen.All(x2 => x2.Naam != x.Plan));
				if (plu != null)
				{
					gk.GekoppeldeKruising.PlanUitgangen.Remove(plu);
					gk.GekoppeldeKruising.PlanUitgangen.BubbleSort();
				}
				var pli = gk.GekoppeldeKruising.PlanIngangen.FirstOrDefault(x => SignaalPlannen.All(x2 => x2.Naam != x.Plan));
				if (pli != null)
				{
					gk.GekoppeldeKruising.PlanIngangen.Remove(pli);
					gk.GekoppeldeKruising.PlanIngangen.BubbleSort();
				}
			}
		}
		
		private bool DuplicateSignaalPlanCommand_CanExecute(object obj)
		{
			return SelectedSignaalPlan != null;
		}

		private void DuplicateSignaalPlanCommand_Executed(object obj)
		{
			var newpl = DeepCloner.DeepClone(SelectedSignaalPlan.SignaalPlan);
			newpl.Naam = "PL" + (SignaalPlannen.Count + 1);
			SignaalPlannen.Add(new SignaalPlanViewModel(newpl));
			foreach (var gk in GekoppeldeKruisingen)
			{
				gk.GekoppeldeKruising.PlanUitgangen.Add(new HalfstarGekoppeldeKruisingPlanUitgangModel
				{
					Kruising = gk.KruisingNaam,
					Plan = newpl.Naam,
					Type = gk.Type
				});
				gk.GekoppeldeKruising.PlanUitgangen.BubbleSort();
				gk.GekoppeldeKruising.PlanIngangen.Add(new HalfstarGekoppeldeKruisingPlanIngangModel
				{
					Kruising = gk.KruisingNaam,
					Plan = newpl.Naam,
					Type = gk.Type
				});
				gk.GekoppeldeKruising.PlanIngangen.BubbleSort();
			}
		}
		
		private bool ImportSignaalPlanCommand_CanExecute(object obj)
		{
			return SelectedSignaalPlan != null;
		}

		private void ImportSignaalPlanCommand_Executed(object obj)
		{
            var importWindow = new ImportSignalPlanWindow(SelectedSignaalPlan.SignaalPlan)
            {
                Owner = Application.Current.MainWindow
            };
            importWindow.ShowDialog();
			SelectedSignaalPlan.RaisePropertyChanged("");
			foreach (var fc in SelectedSignaalPlan.Fasen)
			{
				fc.RaisePropertyChanged("");
			}
		}

        private bool ImportManySignaalPlanCommand_CanExecute(object obj)
        {
            return true;
        }

        private void ImportManySignaalPlanCommand_Executed(object obj)
        {
            var importWindow = new ImportManySignalPlanWindow(HalfstarData)
            {
                Owner = Application.Current.MainWindow
            };
            importWindow.ShowDialog();
            SignaalPlannen.Rebuild();
        }

        private bool AddHoofdRichtingCommand_CanExecute(object obj)
		{
			return SelectedHoofdRichtingToAdd != null;
		}

		private void AddHoofdRichtingCommand_Executed(object obj)
		{
			HoofdRichtingen.Add(new HalfstarHoofdrichtingViewModel(new HalfstarHoofdrichtingModel
			{
				FaseCyclus = SelectedHoofdRichtingToAdd
			}));
			HoofdRichtingen.BubbleSort();
			UpdateSelectables();
		}

		private bool RemoveHoofdRichtingCommand_CanExecute(object obj)
		{
			return SelectedHoofdRichtingToRemove != null;
		}

		private void RemoveHoofdRichtingCommand_Executed(object obj)
		{
			HoofdRichtingen.Remove(SelectedHoofdRichtingToRemove);
			HoofdRichtingen.BubbleSort();
			UpdateSelectables();
		}

		private bool AddGekoppeldeKruisingCommand_CanExecute(object obj)
		{
			return Type == HalfstarTypeEnum.Master ||
				   Type == HalfstarTypeEnum.FallbackMaster ||
				   Type == HalfstarTypeEnum.Slave && GekoppeldeKruisingen.Count == 0;
		}

		private void AddGekoppeldeKruisingCommand_Executed(object obj)
		{
			var gkk = new HalfstarGekoppeldeKruisingModel();
			switch (Type)
			{
				case HalfstarTypeEnum.Master:
					gkk.Type = HalfstarGekoppeldTypeEnum.Slave;
					break;
				case HalfstarTypeEnum.FallbackMaster:
					gkk.Type = GekoppeldeKruisingen.Count == 0 ? HalfstarGekoppeldTypeEnum.Master : HalfstarGekoppeldTypeEnum.Slave;
					break;
				case HalfstarTypeEnum.Slave:
					gkk.Type = HalfstarGekoppeldTypeEnum.Master;
					break;
			}

			if (PTPKruisingenNames.Any())
			{
				gkk.PTPKruising = PTPKruisingenNames[0];
			}

			foreach (var pl in SignaalPlannen)
			{
				gkk.PlanUitgangen.Add(new HalfstarGekoppeldeKruisingPlanUitgangModel
				{
					Kruising = gkk.KruisingNaam,
					Plan = pl.Naam,
					Type = gkk.Type
				});
				gkk.PlanIngangen.Add(new HalfstarGekoppeldeKruisingPlanIngangModel
				{
					Kruising = gkk.KruisingNaam,
					Plan = pl.Naam,
					Type = gkk.Type
				});
			}
			GekoppeldeKruisingen.Add(new HalfstarGekoppeldeKruisingViewModel(gkk));
		}

		private bool RemoveGekoppeldeKruisingCommand_CanExecute(object obj)
		{
			return SelectedHalfstarGekoppeldeKruising != null;
		}

		private void RemoveGekoppeldeKruisingCommand_Executed(object obj)
		{
			var i = GekoppeldeKruisingen.IndexOf(SelectedHalfstarGekoppeldeKruising);
			GekoppeldeKruisingen.Remove(SelectedHalfstarGekoppeldeKruising);
			SelectedHalfstarGekoppeldeKruising = null;
			if (i - 1 <= GekoppeldeKruisingen.Count - 1)
			{
				if (i - 1 >= 0)
				{
					SelectedHalfstarGekoppeldeKruising = GekoppeldeKruisingen[i - 1];
				}
				else if (GekoppeldeKruisingen.Any())
				{
					SelectedHalfstarGekoppeldeKruising = GekoppeldeKruisingen[0];
				}
			}

			if (Type == HalfstarTypeEnum.FallbackMaster && GekoppeldeKruisingen.Any())
			{
				if (GekoppeldeKruisingen.All(x => x.Type != HalfstarGekoppeldTypeEnum.Master))
				{
					GekoppeldeKruisingen[0].Type = HalfstarGekoppeldTypeEnum.Master;
				}
			}
		}

		#endregion // Command functionality

		#region Private methods

        private void UpdateAlternatievenFromController()
        {
            foreach (var fc in Controller.Fasen)
            {
                if (Alternatieven.All(x => fc.Naam != x.Model.FaseCyclus))
                {
                    Alternatieven.Add(new HalfstarFaseCyclusAlternatiefViewModel(new HalfstarFaseCyclusAlternatiefModel()
                    {
                        FaseCyclus = fc.Naam
                    }));
                }
            }
        }

		private void UpdateSelectables()
		{
			var s = SelectedHoofdRichtingToAdd;
			SelectableHoofdRichtingen.Clear();
			foreach (var fc in Controller.Fasen)
			{
				if (HoofdRichtingen.All(x => x.FaseCyclus != fc.Naam))
				{
					SelectableHoofdRichtingen.Add(fc.Naam);
				}
			}

			if (s != null && SelectableHoofdRichtingen.Contains(s))
			{
				SelectedHoofdRichtingToAdd = s;
			}
			else if (SelectableHoofdRichtingen.Any())
			{
				SelectedHoofdRichtingToAdd = SelectableHoofdRichtingen[0];
			}
		}

		private void UpdatePeriodenData()
		{
			foreach (var per in Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden))
			{
				if (HalfstarPeriodenData.All(x => x.Periode != per.Naam))
				{
					HalfstarPeriodenData.Add(new HalfstarPeriodeDataViewModel(new HalfstarPeriodeDataModel
					{
						Periode = per.Naam
					}));
				}
			}

			var rems2 = new List<HalfstarPeriodeDataViewModel>();
			foreach (var hstper in HalfstarPeriodenData)
			{
				if (Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden).All(x => x.Naam != hstper.Periode))
				{
					rems2.Add(hstper);
				}
			}

			foreach (var fc in rems2)
			{
				HalfstarPeriodenData.Remove(fc);
			}
		}

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
					HalfstarPeriodenData = new ObservableCollectionAroundList<HalfstarPeriodeDataViewModel, HalfstarPeriodeDataModel>(Controller.HalfstarData.HalfstarPeriodenData);
					GekoppeldeKruisingen = new ObservableCollectionAroundList<HalfstarGekoppeldeKruisingViewModel, HalfstarGekoppeldeKruisingModel>(HalfstarData.GekoppeldeKruisingen);
					HoofdRichtingen = new ObservableCollectionAroundList<HalfstarHoofdrichtingViewModel, HalfstarHoofdrichtingModel>(HalfstarData.Hoofdrichtingen);
                    Alternatieven = new ObservableCollectionAroundList<HalfstarFaseCyclusAlternatiefViewModel, HalfstarFaseCyclusAlternatiefModel>(HalfstarData.Alternatieven);
                    UpdateAlternatievenFromController();

                    SignaalPlannen.CollectionChanged += (o, e) =>
					{
						if (e.OldItems != null && e.OldItems.Count > 0)
						{
							foreach (SignaalPlanViewModel spvm in e.OldItems)
							{
								SignaalPlannenNames.Remove(spvm.Naam);
								foreach (var per in HalfstarPeriodenData)
								{
									if (per.Signaalplan == spvm.Naam)
									{
										per.Signaalplan = SignaalPlannenNames.Any() ? SignaalPlannenNames[0] : null;
									}
								}
							}
						}
						if (e.NewItems != null && e.NewItems.Count > 0)
						{
							foreach (SignaalPlanViewModel spvm in e.NewItems)
							{
								SignaalPlannenNames.Add(spvm.Naam);
							}
						}
					};
					SignaalPlannenNames.Clear();
					foreach (var spvm in SignaalPlannen)
					{
						SignaalPlannenNames.Add(spvm.Naam);
					}
					PTPKruisingenNames.Clear();
					foreach (var kr in Controller.PTPData.PTPKoppelingen)
					{
						PTPKruisingenNames.Add(kr.TeKoppelenKruispunt);
					}
					UpdateSelectables();
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
                    Alternatieven.Add(new HalfstarFaseCyclusAlternatiefViewModel(new HalfstarFaseCyclusAlternatiefModel() { FaseCyclus = fc.Naam }));
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

					var rfc = HoofdRichtingen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
					if (rfc != null) HoofdRichtingen.Remove(rfc);

                    var rafc = Alternatieven.FirstOrDefault(x => x.Model.FaseCyclus == fc.Naam);
                    if (rafc != null) Alternatieven.Remove(rafc);
                }
			}

			UpdateSelectables();
		}

		private void OnNameChanged(NameChangedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
			RaisePropertyChanged(nameof(DefaultSignaalplanText));
			RaisePropertyChanged(nameof(DefaultVARegelenText));
			
			if (PTPKruisingenNames.Contains(msg.OldName))
			{
				PTPKruisingenNames.Add(msg.NewName);
				foreach (var k in GekoppeldeKruisingen)
				{
					if (k.PTPKruising == msg.OldName)
					{
						k.PTPKruising = msg.NewName;
					}
				}
				PTPKruisingenNames.Remove(msg.OldName);
			}

			HoofdRichtingen.BubbleSort();
			UpdateSelectables();
		}

		private void OnFasenSorted(FasenSortedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
			HoofdRichtingen.BubbleSort();
            Alternatieven.BubbleSort();
		}

		private void OnPeriodenChanged(PeriodenChangedMessage msg)
		{
			UpdatePeriodenData();
		}

		private void OnPTPKoppelingenChanged(PTPKoppelingenChangedMessage msg)
		{
			var rems = new List<string>();
			foreach (var ptpk in PTPKruisingenNames)
			{
				if (Controller.PTPData.PTPKoppelingen.All(x => x.TeKoppelenKruispunt != ptpk))
				{
					rems.Add(ptpk);
				}
			}

			foreach (var r in rems)
			{
				PTPKruisingenNames.Remove(r);
				foreach (var k in GekoppeldeKruisingen)
				{
					if (k.PTPKruising == r)
					{
						k.PTPKruising = "onbekend";
					}
				}
			}

			foreach (var ptpkp in Controller.PTPData.PTPKoppelingen)
			{
				if (PTPKruisingenNames.All(x => x != ptpkp.TeKoppelenKruispunt))
				{
					PTPKruisingenNames.Add(ptpkp.TeKoppelenKruispunt);
				}
			}
		}

		#endregion // TLCGen Events

		#region Constructor

		public HalfstarTabViewModel() : base()
		{
			Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
			Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
			Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
			Messenger.Default.Register(this, new Action<PeriodenChangedMessage>(OnPeriodenChanged));
			Messenger.Default.Register(this, new Action<PTPKoppelingenChangedMessage>(OnPTPKoppelingenChanged));

            if (SignaalPlannen?.Any() == true)
            {
                SelectedSignaalPlan = SignaalPlannen[0];
            }
		}

		#endregion // Constructor
	}
}