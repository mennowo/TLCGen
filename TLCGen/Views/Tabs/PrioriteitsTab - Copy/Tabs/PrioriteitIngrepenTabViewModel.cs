﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.ViewModels;

namespace TLCGen.ViewModels
{
    public class FaseCyclusWithPrioViewModel : ViewModelBase
    {
        public bool HasBus => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == OVIngreepVoertuigTypeEnum.Bus);
        public bool HasTram => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == OVIngreepVoertuigTypeEnum.Tram);
        public bool HasBicycle => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == OVIngreepVoertuigTypeEnum.Fiets);
        public bool HasTruck => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == OVIngreepVoertuigTypeEnum.Vrachtwagen);

        public string Naam { get; }

        public List<OVIngreepModel> Ingrepen { get; }

        public void UpdateTypes()
        {
            RaisePropertyChanged(nameof(HasBus));
            RaisePropertyChanged(nameof(HasTram));
            RaisePropertyChanged(nameof(HasBicycle));
            RaisePropertyChanged(nameof(HasTruck));
        }

        public FaseCyclusWithPrioViewModel(string naam, List<OVIngreepModel> ingrepen)
        {
            Naam = naam;
            Ingrepen = ingrepen;
        }
    }

    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioriteitIngrepenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RelayCommand _addIngreepCommand;
        private RelayCommand _removeIngreepCommand;
        private OVIngreepViewModel _selectedIngreep;
        private FaseCyclusWithPrioViewModel _selectedFaseCyclus;
        private ObservableCollection<FaseCyclusWithPrioViewModel> _fasen;
        private bool _selectedBus;
        private bool _selectedTram;
        private bool _selectedTruck;
        private bool _selectedBicycle;
        private bool _prioriteit;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusWithPrioViewModel> Fasen
        {
            get
            {
                if (_fasen == null)
                {
                    _fasen = new ObservableCollection<FaseCyclusWithPrioViewModel>();
                }
                return _fasen;
            }
        }

        public ObservableCollection<OVIngreepViewModel> Ingrepen { get; }

        public FaseCyclusWithPrioViewModel SelectedFaseCyclus
        {
            get { return _selectedFaseCyclus; }
            set
            {
                _selectedFaseCyclus = value;
                Ingrepen.Clear();
                if (value != null)
                {
                    foreach (var ig in _Controller.OVData.OVIngrepen.Where(x => x.FaseCyclus == _selectedFaseCyclus.Naam))
                    {
                        Ingrepen.Add(new OVIngreepViewModel(ig));
                    }
                    var selType = OVIngreepVoertuigTypeEnum.Bus;
                    if (SelectedTram) selType = OVIngreepVoertuigTypeEnum.Tram;
                    if (SelectedTruck) selType = OVIngreepVoertuigTypeEnum.Vrachtwagen;
                    if (SelectedBicycle) selType = OVIngreepVoertuigTypeEnum.Fiets;
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == selType);
                    //RefreshAvailableTypes();
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Prioriteit));
                RaisePropertyChanged(nameof(HasBus));
                RaisePropertyChanged(nameof(HasTram));
                RaisePropertyChanged(nameof(HasBicycle));
                RaisePropertyChanged(nameof(HasTruck));
            }
        }

        private void SelectedIngreep_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
            {
                RaisePropertyChanged(nameof(Prioriteit));
                RaisePropertyChanged(nameof(HasBus));
                RaisePropertyChanged(nameof(HasTram));
                RaisePropertyChanged(nameof(HasBicycle));
                RaisePropertyChanged(nameof(HasTruck));
                SelectedFaseCyclus.UpdateTypes();
            }
        }

        public bool HasBus => Ingrepen.Any(x => x.Type == OVIngreepVoertuigTypeEnum.Bus);
        public bool HasTram => Ingrepen.Any(x => x.Type == OVIngreepVoertuigTypeEnum.Tram);
        public bool HasBicycle => Ingrepen.Any(x => x.Type == OVIngreepVoertuigTypeEnum.Fiets);
        public bool HasTruck => Ingrepen.Any(x => x.Type == OVIngreepVoertuigTypeEnum.Vrachtwagen);

        public bool Prioriteit
        {
            get => _prioriteit;
            set
            {
                _prioriteit = value;
                var vtgtype = OVIngreepVoertuigTypeEnum.NG;
                if (SelectedBus) vtgtype = OVIngreepVoertuigTypeEnum.Bus;
                else if (SelectedTram) vtgtype = OVIngreepVoertuigTypeEnum.Tram;
                else if (SelectedTruck) vtgtype = OVIngreepVoertuigTypeEnum.Vrachtwagen;
                else if (SelectedBicycle) vtgtype = OVIngreepVoertuigTypeEnum.Fiets;
                if (value)
                {
                    if (!Ingrepen.Any(x => x.Type == vtgtype))
                    {
                        var prio = new OVIngreepModel
                        {
                            FaseCyclus = SelectedFaseCyclus.Naam,
                            Type = vtgtype
                        };
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio);
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio.MeldingenData);
                        if (prio.Type == OVIngreepVoertuigTypeEnum.Bus || prio.Type == OVIngreepVoertuigTypeEnum.Tram)
                        {
                            prio.MeldingenData.Inmeldingen.Add(new OVIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = true,
                                AntiJutterTijd = 15,
                                InUit = OVIngreepInUitMeldingTypeEnum.Inmelding,
                                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                            prio.MeldingenData.Uitmeldingen.Add(new OVIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = false,
                                InUit = OVIngreepInUitMeldingTypeEnum.Uitmelding,
                                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                        }
                        _Controller.OVData.OVIngrepen.Add(prio);
                        _Controller.OVData.OVIngrepen.BubbleSort();
                        var prioVm = new OVIngreepViewModel(prio);
                        MessengerInstance.Send(new OVIngreepMeldingChangingMessage(prio, prio.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                        Ingrepen.Add(prioVm);
                        SelectedIngreep = prioVm;
                    }
                }
                else
                {
                    var rems = Ingrepen.Where(x => x.Type == vtgtype).ToList();
                    foreach(var r in rems)
                    {
                        _Controller.OVData.OVIngrepen.Remove(r.OVIngreep);
                        Ingrepen.Remove(r);
                    }
                    SelectedIngreep = null;
                }
                foreach (var f in Fasen) f.UpdateTypes();
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(HasBus));
                RaisePropertyChanged(nameof(HasTram));
                RaisePropertyChanged(nameof(HasBicycle));
                RaisePropertyChanged(nameof(HasTruck));
            }
        }

        public bool SelectedBus
        {
            get => _selectedBus;
            set
            {
                _selectedBus = value;
                if (value)
                {
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == OVIngreepVoertuigTypeEnum.Bus);
                    SelectedTram = SelectedTruck = SelectedBicycle = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool SelectedTram
        {
            get => _selectedTram;
            set
            {
                _selectedTram = value;
                if (value)
                {
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == OVIngreepVoertuigTypeEnum.Tram);
                    SelectedBus = SelectedTruck = SelectedBicycle = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool SelectedTruck
        {
            get => _selectedTruck;
            set
            {
                _selectedTruck = value;
                if (value)
                {
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == OVIngreepVoertuigTypeEnum.Vrachtwagen);
                    SelectedTram = SelectedBus = SelectedBicycle = false;
                }
                RaisePropertyChanged();
            }
        }

        public bool SelectedBicycle
        {
            get => _selectedBicycle;
            set
            {
                _selectedBicycle = value;
                if (value)
                {
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == OVIngreepVoertuigTypeEnum.Fiets);
                    SelectedTram = SelectedTruck = SelectedBus = false;
                }
                RaisePropertyChanged();
            }
        }

        public OVIngreepViewModel SelectedIngreep
        {
            get { return _selectedIngreep; }
            set
            {
                if (_selectedIngreep != null)
                {
                    _selectedIngreep.PropertyChanged -= SelectedIngreep_PropertyChanged;
                }
                _selectedIngreep = value;
                if (_selectedIngreep != null)
                {
                    _selectedIngreep.PropertyChanged += SelectedIngreep_PropertyChanged;
                }
                if (value == null)
                {
                    _prioriteit = false;
                }
                else
                {
                    _prioriteit = true;
                }
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Prioriteit));
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddIngreepCommand
        {
            get
            {
                return _addIngreepCommand ?? (_addIngreepCommand = new RelayCommand(
                    () =>
                    {
                        var prio = new OVIngreepModel
                        {
                            FaseCyclus = SelectedFaseCyclus.Naam,
                            Type = OVIngreepVoertuigTypeEnum.NG
                        };
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio);
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio.MeldingenData);
                        if (prio.Type == OVIngreepVoertuigTypeEnum.Bus || prio.Type == OVIngreepVoertuigTypeEnum.Tram)
                        {
                            prio.MeldingenData.Inmeldingen.Add(new OVIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = true,
                                AntiJutterTijd = 15,
                                InUit = OVIngreepInUitMeldingTypeEnum.Inmelding,
                                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                            prio.MeldingenData.Uitmeldingen.Add(new OVIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = false,
                                InUit = OVIngreepInUitMeldingTypeEnum.Uitmelding,
                                Type = OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                        }
                        _Controller.OVData.OVIngrepen.Add(prio);
                        _Controller.OVData.OVIngrepen.BubbleSort();
                        var prioVm = new OVIngreepViewModel(prio);
                        MessengerInstance.Send(new OVIngreepMeldingChangingMessage(prio, prio.FaseCyclus, OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                        Ingrepen.Add(prioVm);
                        SelectedIngreep = prioVm;
                    },
                    () => _selectedFaseCyclus != null));
            }
        }
        
        public ICommand RemoveIngreepCommand
        {
            get
            {
                return _removeIngreepCommand ?? (_removeIngreepCommand = new RelayCommand(
                    () =>
                    {
                        _Controller.OVData.OVIngrepen.Remove(_selectedIngreep.OVIngreep);
                        Ingrepen.Remove(_selectedIngreep);
                        SelectedIngreep = Ingrepen.FirstOrDefault();
                    },
                    () => _selectedFaseCyclus != null && _selectedIngreep != null));
            }
        }

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Ingrepen";
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller?.OVData?.OVIngreepType != Models.Enumerations.OVIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            var temp = SelectedFaseCyclus;
            Fasen.Clear();
            SelectedFaseCyclus = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusWithPrioViewModel(fcm.Naam, _Controller.OVData.OVIngrepen);
                Fasen.Add(fcvm);
                if (temp == null || fcvm.Naam != temp.Naam) continue;
                SelectedFaseCyclus = fcvm;
                temp = null;
            }
            if (SelectedFaseCyclus == null && Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
        }

        #endregion // TabItem Overrides

        #region Private Methods

        //private void RefreshAvailableTypes()
        //{
        //    _availableTypes.Clear();
        //    foreach (OVIngreepVoertuigTypeEnum prioVtgType in Enum.GetValues(typeof(OVIngreepVoertuigTypeEnum)))
        //    {
        //        if (prioVtgType == OVIngreepVoertuigTypeEnum.NG) continue;
        //        if (!Ingrepen.Any(x => x.Type == prioVtgType)) _availableTypes.Add(prioVtgType);
        //    }
        //}

        #endregion // Private Methods

        #region Constructor

        public PrioriteitIngrepenTabViewModel() : base()
        {
            Ingrepen = new ObservableCollection<OVIngreepViewModel>();
            SelectedBus = true;
        }

        #endregion // Constructor
    }
}
