using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
		private RelayCommand _addSignaalPlanCommand;
		private RelayCommand _removeSignaalPlanCommand;
		private RelayCommand _duplicateSignaalPlanCommand;
		private RelayCommand _importSignaalPlanCommand;
        private RelayCommand _importManySignaalPlanCommand;
        private RelayCommand _TLCImportSignaalPlannenCommand;
        private RelayCommand _TLCExportSignaalPlannenCommand;
        private RelayCommand _copyPlanToPlanCommand;
        private RelayCommand _removeGekoppeldeKruisingCommand;
        private RelayCommand _addHoofdRichtingCommand;
        private RelayCommand _removeHoofdRichtingCommand;
        private RelayCommand _addGekoppeldeKruisingCommand;

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
		public ObservableCollectionAroundList<HalfstarFaseCyclusInstellingenViewModel, HalfstarFaseCyclusInstellingenModel> Alternatieven { get; private set; }
        public ObservableCollection<HalfstarOVIngreepViewModel> OVIngrepenHalfstar { get; } = new ObservableCollection<HalfstarOVIngreepViewModel>();

        public bool ShowHalfstarAlert
        {
            get
            {
                return Type != HalfstarTypeEnum.Master && GekoppeldeKruisingen.All(x => x.Type != HalfstarGekoppeldTypeEnum.Master);
            }
        }

        public string HalfstarAlert => "Let op! Gezien het type halfstarre regelaar, moet er een master regelaar worden opgegeven.";

        public SignaalPlanViewModel SelectedSignaalPlan
		{
			get => _selectedSignaalPlan;
			set
			{
				_selectedSignaalPlan = value; 
				OnPropertyChanged();
				_removeSignaalPlanCommand?.NotifyCanExecuteChanged();
                _duplicateSignaalPlanCommand?.NotifyCanExecuteChanged();
                _importSignaalPlanCommand?.NotifyCanExecuteChanged();
                _copyPlanToPlanCommand?.NotifyCanExecuteChanged();
            }
		}

		public string SelectedHoofdRichtingToAdd
		{
			get => _selectedHoofdRichtingToAdd;
			set
			{
				_selectedHoofdRichtingToAdd = value; 
				OnPropertyChanged();
                _addHoofdRichtingCommand?.NotifyCanExecuteChanged();

            }
		}

		public HalfstarHoofdrichtingViewModel SelectedHoofdRichtingToRemove
		{
			get => _selectedHoofdRichtingToRemove;
			set
			{
				_selectedHoofdRichtingToRemove = value;
				OnPropertyChanged();
                _removeHoofdRichtingCommand?.NotifyCanExecuteChanged();
            }
		}

		public HalfstarGekoppeldeKruisingViewModel SelectedHalfstarGekoppeldeKruising
		{
			get => _selectedHalfstarGekoppeldeKruising; set
			{
				_selectedHalfstarGekoppeldeKruising = value; 
				OnPropertyChanged();
                _removeGekoppeldeKruisingCommand?.NotifyCanExecuteChanged();
            }
		}

        public bool IsHalfstarWithAltenatieven => IsHalfstar && _Controller?.ModuleMolen.LangstWachtendeAlternatief == true;

        public bool IsHalfstar
		{
			get => HalfstarData?.IsHalfstar == true;
			set
			{
				HalfstarData.IsHalfstar = value;
                if (value && Controller != null)
				{
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
				else if (!value && Controller != null)
                {
                    ClearHalfstar();
                }

				OnPropertyChanged(broadcast: true);
				OnPropertyChanged(nameof(IsHalfstarWithAltenatieven));
			}
		}

        private void ClearHalfstar()
        {
            SignaalPlannen.RemoveAll();
			GekoppeldeKruisingen.RemoveAll();
            HoofdRichtingen.RemoveAll();
        }

        public HalfstarTypeEnum Type
		{
			get => HalfstarData?.Type ?? HalfstarTypeEnum.FallbackMaster;
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
				OnPropertyChanged(broadcast: true);
                _addGekoppeldeKruisingCommand?.NotifyCanExecuteChanged();
            }
		}

		public string DefaultSignaalplanText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") plan";
		public string DefaultVARegelenText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") VA regelen";
		public string DefaultAlternatievenVoorHoofdrichtingenText => "Default (" + Controller?.PeriodenData.DefaultPeriodeNaam + ") hoofdr.alt.";

		public HalfstarVARegelenTypeEnum TypeVARegelen
		{
			get => HalfstarData?.TypeVARegelen ?? HalfstarVARegelenTypeEnum.ML;
			set
			{
				HalfstarData.TypeVARegelen = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		public bool VARegelen
		{
			get => HalfstarData?.VARegelen == true;
			set
			{
				HalfstarData.VARegelen = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		public bool OVPrioriteitPL
		{
			get => HalfstarData?.OVPrioriteitPL == true;
			set
			{
				HalfstarData.OVPrioriteitPL = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		public bool AlternatievenVoorHoofdrichtingen
		{
			get => HalfstarData?.AlternatievenVoorHoofdrichtingen == true;
			set
			{
				HalfstarData.AlternatievenVoorHoofdrichtingen = value;
				OnPropertyChanged(broadcast: true);
			}
		}

        public bool PlantijdenInParameters
        {
            get => HalfstarData?.PlantijdenInParameters == true;
            set
            {
                HalfstarData.PlantijdenInParameters = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string DefaultSignaalplan
		{
			get => HalfstarData?.DefaultPeriodeSignaalplan; 
			set
			{
				HalfstarData.DefaultPeriodeSignaalplan = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		public bool DefaultPeriodeVARegelen
		{
			get => HalfstarData?.DefaultPeriodeVARegelen == true; 
			set
			{
				HalfstarData.DefaultPeriodeVARegelen = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		public bool DefaultPeriodeAlternatievenVoorHoofdrichtingen
		{
			get => HalfstarData?.DefaultPeriodeAlternatievenVoorHoofdrichtingen == true;
			set
			{
				HalfstarData.DefaultPeriodeAlternatievenVoorHoofdrichtingen = value;
				OnPropertyChanged(broadcast: true);
			}
		}

		#endregion // Properties

		#region Commands

		public ICommand AddSignaalPlanCommand => _addSignaalPlanCommand ??= new RelayCommand(() =>
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
        });

        public ICommand RemoveSignaalPlanCommand => _removeSignaalPlanCommand ??= new RelayCommand(() =>
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
        }, () => SelectedSignaalPlan != null);

        public ICommand DuplicateSignaalPlanCommand => _duplicateSignaalPlanCommand ??= new RelayCommand(() =>
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
        }, () => SelectedSignaalPlan != null);

        public ICommand ImportSignaalPlanCommand => _importSignaalPlanCommand ??= new RelayCommand(() =>
        {
            var importWindow = new ImportSignalPlanWindow(SelectedSignaalPlan.SignaalPlan)
            {
                Owner = Application.Current.MainWindow
            };
            importWindow.ShowDialog();
            SelectedSignaalPlan.OnPropertyChanged("");
            foreach (var fc in SelectedSignaalPlan.Fasen)
            {
                fc.OnPropertyChanged("");
            }
        }, () => SelectedSignaalPlan != null);

        public ICommand ImportManySignaalPlanCommand => _importManySignaalPlanCommand ??= new RelayCommand(() =>
        {
            var importWindow = new ImportManySignalPlanWindow(HalfstarData)
            {
                Owner = Application.Current.MainWindow
            };
            importWindow.ShowDialog();
            SignaalPlannen.Rebuild();
        });

        public ICommand TLCImportSignaalPlannenCommand => _TLCImportSignaalPlannenCommand ??= new RelayCommand(() =>
            {
                var importWindow = new TLCImportSignalPlannenWindow(HalfstarData, true)
                {
                    Owner = Application.Current.MainWindow
                };
                importWindow.ShowDialog();
                SignaalPlannen.Rebuild();
                foreach(var pl in SignaalPlannen)
                {
                    pl.OnPropertyChanged("");
                    foreach(var fc in pl.Fasen)
                    {
                        fc.OnPropertyChanged("");
                    }
                }
            },
                () => SignaalPlannen?.Any() == true);

        public ICommand TLCExportSignaalPlannenCommand => _TLCExportSignaalPlannenCommand ??= new RelayCommand(() =>
            {
                var importWindow = new TLCImportSignalPlannenWindow(HalfstarData, false)
                {
                    Owner = Application.Current.MainWindow
                };
                importWindow.ShowDialog();
                SignaalPlannen.Rebuild();
            }, () => SignaalPlannen?.Any() == true);

        public ICommand CopyPlanToPlanCommand => _copyPlanToPlanCommand ??= new RelayCommand(
            () =>
            {
                var dlg = new InputBoxWindow
                {
                    Owner = Application.Current.MainWindow,
                    Title = "Kopieëren naar plan naar ander plan",
                    Explanation = $"Geef de naam van het plan op om de tijden " +
                                  $"van {SelectedSignaalPlan.Naam} naar toe te kopieëren:"
                };
                dlg.ShowDialog();
                if (dlg.DialogResult == true)
                {
                    var pl = SignaalPlannen.FirstOrDefault(x2 => x2.Naam == dlg.Text);
                    if (pl != null)
                    {
                        pl.Commentaar = SelectedSignaalPlan.Commentaar;
                        pl.Cyclustijd = SelectedSignaalPlan.Cyclustijd;
                        pl.StartMoment = SelectedSignaalPlan.StartMoment;
                        pl.SwitchMoment = SelectedSignaalPlan.SwitchMoment;
                        foreach (var fc in pl.Fasen)
                        {
                            var fc2 = SelectedSignaalPlan.Fasen.FirstOrDefault(x2 =>
                                x2.FaseCyclus == fc.FaseCyclus);
                            if (fc2 != null)
                            {
                                fc.A1 = fc2.A1;
                                fc.B1 = fc2.B1;
                                fc.C1 = fc2.C1;
                                fc.D1 = fc2.D1;
                                fc.E1 = fc2.E1;
                                fc.A2 = fc2.A2;
                                fc.B2 = fc2.B2;
                                fc.C2 = fc2.C2;
                                fc.D2 = fc2.D2;
                                fc.E2 = fc2.E2;
                            }
                        }
                    }
                }
            },
            () => SelectedSignaalPlan != null);

        public ICommand AddHoofdRichtingCommand => _addHoofdRichtingCommand ??= new RelayCommand(() =>
        {
            HoofdRichtingen.Add(new HalfstarHoofdrichtingViewModel(new HalfstarHoofdrichtingModel
            {
                FaseCyclus = SelectedHoofdRichtingToAdd
            }));
            HoofdRichtingen.BubbleSort();
            UpdateSelectables();
            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }, () => SelectedHoofdRichtingToAdd != null);

        public ICommand RemoveHoofdRichtingCommand => _removeHoofdRichtingCommand ??= new RelayCommand(() =>
        {
            HoofdRichtingen.Remove(SelectedHoofdRichtingToRemove);
            HoofdRichtingen.BubbleSort();
            UpdateSelectables();
            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }, () => SelectedHoofdRichtingToRemove != null);

        public ICommand AddGekoppeldeKruisingCommand => _addGekoppeldeKruisingCommand ??= new RelayCommand(() =>
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
            WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }, () => Type == HalfstarTypeEnum.Master ||
                 Type == HalfstarTypeEnum.FallbackMaster ||
                 Type == HalfstarTypeEnum.Slave && GekoppeldeKruisingen.Count == 0);

        public ICommand RemoveGekoppeldeKruisingCommand => _removeGekoppeldeKruisingCommand ??= new RelayCommand(() =>
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
                WeakReferenceMessengerEx.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
            },
                () => SelectedHalfstarGekoppeldeKruising != null);

        #endregion // Commands

        #region Private methods

        private void UpdateAlternatievenFromController()
        {
            foreach (var fc in Controller.Fasen)
            {
                if (Alternatieven.All(x => fc.Naam != x.Model.FaseCyclus))
                {
                    Alternatieven.Add(new HalfstarFaseCyclusInstellingenViewModel(new HalfstarFaseCyclusInstellingenModel()
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
			var groentijdPerioden = Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.Groentijden).ToList();

            foreach (var per in groentijdPerioden)
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
				if (groentijdPerioden.All(x => x.Naam != hstper.Periode))
				{
					rems2.Add(hstper);
				}
			}

			foreach (var fc in rems2)
			{
				HalfstarPeriodenData.Remove(fc);
			}

			var pers = new Dictionary<string, int>();
			for (var i = 0; i < groentijdPerioden.Count; ++i)
			{
				pers.Add(groentijdPerioden[i].Naam, i);
			}

            HalfstarPeriodenData.BubbleSort<HalfstarPeriodeDataViewModel>((x, y) => pers[x.Periode].CompareTo(pers[y.Periode]));
			HalfstarPeriodenData.RebuildList();
		}

		#endregion // Private methods

		#region Public methods

		#endregion // Public methods

		#region TLCGen TabItem overrides

		public override string DisplayName => "Halfstar";

		public override void OnSelected()
		{
            OnPropertyChanged(nameof(IsHalfstarWithAltenatieven));
        }

        public override ControllerModel Controller
		{
			get => base.Controller;
			set
			{
				base.Controller = value;
                if (SignaalPlannen != null) SignaalPlannen.CollectionChanged -= SignaalPlannenOnCollectionChanged;
				if (base.Controller != null)
				{
					HalfstarData = Controller.HalfstarData;
					SignaalPlannen = new ObservableCollectionAroundList<SignaalPlanViewModel, SignaalPlanModel>(HalfstarData.SignaalPlannen);
					SignaalPlannen.CollectionChanged += SignaalPlannenOnCollectionChanged;
					HalfstarPeriodenData = new ObservableCollectionAroundList<HalfstarPeriodeDataViewModel, HalfstarPeriodeDataModel>(Controller.HalfstarData.HalfstarPeriodenData);
					UpdatePeriodenData();
                    if (GekoppeldeKruisingen != null)
                    {
                        foreach (var k in GekoppeldeKruisingen)
                        {
                            k.TypeChanged += GekoppeldeKruising_TypeChanged;
                        }
                        GekoppeldeKruisingen.CollectionChanged -= GekoppeldeKruisingen_CollectionChanged;
                    }
					GekoppeldeKruisingen = new ObservableCollectionAroundList<HalfstarGekoppeldeKruisingViewModel, HalfstarGekoppeldeKruisingModel>(HalfstarData.GekoppeldeKruisingen);
                    GekoppeldeKruisingen.CollectionChanged += GekoppeldeKruisingen_CollectionChanged;
                    foreach(var k in GekoppeldeKruisingen)
                    {
                        k.TypeChanged += GekoppeldeKruising_TypeChanged;
                    }
                    HoofdRichtingen = new ObservableCollectionAroundList<HalfstarHoofdrichtingViewModel, HalfstarHoofdrichtingModel>(HalfstarData.Hoofdrichtingen);
                    Alternatieven = new ObservableCollectionAroundList<HalfstarFaseCyclusInstellingenViewModel, HalfstarFaseCyclusInstellingenModel>(HalfstarData.FaseCyclusInstellingen);
                    OVIngrepenHalfstar.Clear();
                    foreach(var ov in Controller.PrioData.PrioIngrepen)
                    {
                        OVIngrepenHalfstar.Add(new HalfstarOVIngreepViewModel(ov.HalfstarIngreepData) { BelongsToOVIngreep = ov });
                    }
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

					if (!HalfstarData.IsHalfstar) ClearHalfstar();

					UpdateSelectables();
				}
				else
				{
					HalfstarData = null;
					SignaalPlannen = null;
				}
                _TLCImportSignaalPlannenCommand?.NotifyCanExecuteChanged();
                _TLCExportSignaalPlannenCommand?.NotifyCanExecuteChanged();
            }
		}

        private void SignaalPlannenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _TLCImportSignaalPlannenCommand?.NotifyCanExecuteChanged();
            _TLCExportSignaalPlannenCommand?.NotifyCanExecuteChanged();
        }

        private void GekoppeldeKruising_TypeChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ShowHalfstarAlert));
        }

        private void GekoppeldeKruisingen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (HalfstarGekoppeldeKruisingViewModel k in e.OldItems)
                {
                    k.TypeChanged -= GekoppeldeKruising_TypeChanged;
                }
            }
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (HalfstarGekoppeldeKruisingViewModel k in e.NewItems)
                {
                    k.TypeChanged += GekoppeldeKruising_TypeChanged;
                }
            }
            OnPropertyChanged(nameof(ShowHalfstarAlert));
        }

        #endregion // TLCGen TabItem overrides

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
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
                    Alternatieven.Add(new HalfstarFaseCyclusInstellingenViewModel(new HalfstarFaseCyclusInstellingenModel() { FaseCyclus = fc.Naam }));
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

            Alternatieven.BubbleSort();

            UpdateSelectables();
		}

		private void OnNameChanged(object sender, NameChangedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
			OnPropertyChanged(nameof(DefaultSignaalplanText));
			OnPropertyChanged(nameof(DefaultVARegelenText));
			
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
            OVIngrepenHalfstar.BubbleSort();
            Alternatieven.BubbleSort();
            UpdateSelectables();
		}

		private void OnFasenSorted(object sender, FasenSortedMessage msg)
		{
			foreach (var pl in SignaalPlannen)
			{
				pl.Fasen.BubbleSort();
			}
			HoofdRichtingen.BubbleSort();
            Alternatieven.BubbleSort();
		}

		private void OnPeriodenChanged(object sender, PeriodenChangedMessage msg)
		{
			UpdatePeriodenData();
		}

		private void OnPTPKoppelingenChanged(object sender, PTPKoppelingenChangedMessage msg)
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

        private void OnOVIngrepenChanged(object sender, PrioIngrepenChangedMessage msg)
        {
            var rems = OVIngrepenHalfstar.Where(x => Controller.PrioData.PrioIngrepen.All(x2 => !ReferenceEquals(x.OvIngreep, x2.HalfstarIngreepData))).ToList();
            foreach(var rem in rems)
            {
                OVIngrepenHalfstar.Remove(rem);
            }
            foreach(var ovi in Controller.PrioData.PrioIngrepen)
            {
                if (OVIngrepenHalfstar.All(x => !ReferenceEquals(x.OvIngreep, ovi.HalfstarIngreepData)))
                {
                    OVIngrepenHalfstar.Add(new HalfstarOVIngreepViewModel(ovi.HalfstarIngreepData) { BelongsToOVIngreep = ovi });
                }
            }
            OVIngrepenHalfstar.BubbleSort();
        }

		#endregion // TLCGen Events

		#region Constructor

		public HalfstarTabViewModel() : base()
		{
			WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
			WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
			WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
			WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
			WeakReferenceMessengerEx.Default.Register<PTPKoppelingenChangedMessage>(this, OnPTPKoppelingenChanged);
			WeakReferenceMessengerEx.Default.Register<PrioIngrepenChangedMessage>(this, OnOVIngrepenChanged);

            if (SignaalPlannen?.Any() == true)
            {
                SelectedSignaalPlan = SignaalPlannen[0];
            }
		}

        #endregion // Constructor
    }
}