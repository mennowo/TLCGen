using GalaSoft.MvvmLight;
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
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.ViewModels;

namespace TLCGen.ViewModels
{
    public class FaseCyclusWithPrioViewModel : ViewModelBase
    {
        public bool HasBus => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Bus);
        public bool HasTram => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Tram);
        public bool HasBicycle => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Fiets);
        public bool HasTruck => Ingrepen.Any(x => x.FaseCyclus == Naam && x.Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen);

        public string Naam { get; }

        public List<PrioIngreepModel> Ingrepen { get; }

        public void UpdateTypes()
        {
            RaisePropertyChanged(nameof(HasBus));
            RaisePropertyChanged(nameof(HasTram));
            RaisePropertyChanged(nameof(HasBicycle));
            RaisePropertyChanged(nameof(HasTruck));
        }

        public FaseCyclusWithPrioViewModel(string naam, List<PrioIngreepModel> ingrepen)
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

        public ObservableCollection<FaseCyclusWithPrioViewModel> Fasen => _fasen ?? (_fasen = new ObservableCollection<FaseCyclusWithPrioViewModel>());

        public ObservableCollection<OVIngreepViewModel> Ingrepen { get; }

        public FaseCyclusWithPrioViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value;
                Ingrepen.Clear();
                if (value != null)
                {
                    foreach (var ig in _Controller.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == _selectedFaseCyclus.Naam))
                    {
                        Ingrepen.Add(new OVIngreepViewModel(ig));
                    }
                    var selType = PrioIngreepVoertuigTypeEnum.Bus;
                    if (SelectedTram) selType = PrioIngreepVoertuigTypeEnum.Tram;
                    if (SelectedTruck) selType = PrioIngreepVoertuigTypeEnum.Vrachtwagen;
                    if (SelectedBicycle) selType = PrioIngreepVoertuigTypeEnum.Fiets;
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == selType);
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
            if (e.PropertyName != "Type") return;
            RaisePropertyChanged(nameof(Prioriteit));
            RaisePropertyChanged(nameof(HasBus));
            RaisePropertyChanged(nameof(HasTram));
            RaisePropertyChanged(nameof(HasBicycle));
            RaisePropertyChanged(nameof(HasTruck));
            SelectedFaseCyclus.UpdateTypes();
        }

        public bool HasBus => Ingrepen.Any(x => x.Type == PrioIngreepVoertuigTypeEnum.Bus);
        public bool HasTram => Ingrepen.Any(x => x.Type == PrioIngreepVoertuigTypeEnum.Tram);
        public bool HasBicycle => Ingrepen.Any(x => x.Type == PrioIngreepVoertuigTypeEnum.Fiets);
        public bool HasTruck => Ingrepen.Any(x => x.Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen);

        public bool Prioriteit
        {
            get => _prioriteit;
            set
            {
                _prioriteit = value;
                var vtgtype = PrioIngreepVoertuigTypeEnum.NG;
                if (SelectedBus) vtgtype = PrioIngreepVoertuigTypeEnum.Bus;
                else if (SelectedTram) vtgtype = PrioIngreepVoertuigTypeEnum.Tram;
                else if (SelectedTruck) vtgtype = PrioIngreepVoertuigTypeEnum.Vrachtwagen;
                else if (SelectedBicycle) vtgtype = PrioIngreepVoertuigTypeEnum.Fiets;
                if (value)
                {
                    if (Ingrepen.All(x => x.Type != vtgtype))
                    {
                        var prio = new PrioIngreepModel
                        {
                            FaseCyclus = SelectedFaseCyclus.Naam,
                            Type = vtgtype
                        };
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio);
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio.MeldingenData);
                        if (prio.Type == PrioIngreepVoertuigTypeEnum.Bus || prio.Type == PrioIngreepVoertuigTypeEnum.Tram)
                        {
                            prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = true,
                                AntiJutterTijd = 15,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                            prio.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel()
                            {
                                AntiJutterTijdToepassen = false,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                        }
                        _Controller.PrioData.PrioIngrepen.Add(prio);
                        _Controller.PrioData.PrioIngrepen.BubbleSort();
                        var prioVm = new OVIngreepViewModel(prio);
                        MessengerInstance.Send(new OVIngreepMeldingChangingMessage(prio, prio.FaseCyclus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
                        Ingrepen.Add(prioVm);
                        SelectedIngreep = prioVm;
                    }
                }
                else
                {
                    var rems = Ingrepen.Where(x => x.Type == vtgtype).ToList();
                    foreach(var r in rems)
                    {
                        _Controller.PrioData.PrioIngrepen.Remove(r.PrioIngreep);
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
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == PrioIngreepVoertuigTypeEnum.Bus);
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
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == PrioIngreepVoertuigTypeEnum.Tram);
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
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == PrioIngreepVoertuigTypeEnum.Vrachtwagen);
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
                    SelectedIngreep = Ingrepen.FirstOrDefault(x => x.Type == PrioIngreepVoertuigTypeEnum.Fiets);
                    SelectedTram = SelectedTruck = SelectedBus = false;
                }
                RaisePropertyChanged();
            }
        }

        public OVIngreepViewModel SelectedIngreep
        {
            get => _selectedIngreep;
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
                _prioriteit = value != null;
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
                        var prio = new PrioIngreepModel
                        {
                            FaseCyclus = SelectedFaseCyclus.Naam,
                            Type = PrioIngreepVoertuigTypeEnum.NG
                        };
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio);
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(prio.MeldingenData);
                        if (prio.Type == PrioIngreepVoertuigTypeEnum.Bus || prio.Type == PrioIngreepVoertuigTypeEnum.Tram)
                        {
                            prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
                            {
                                AntiJutterTijdToepassen = true,
                                AntiJutterTijd = 15,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                            prio.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
                            {
                                AntiJutterTijdToepassen = false,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            });
                        }
                        _Controller.PrioData.PrioIngrepen.Add(prio);
                        _Controller.PrioData.PrioIngrepen.BubbleSort();
                        var prioVm = new OVIngreepViewModel(prio);
                        MessengerInstance.Send(new OVIngreepMeldingChangingMessage(prio, prio.FaseCyclus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
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
                        _Controller.PrioData.PrioIngrepen.Remove(_selectedIngreep.PrioIngreep);
                        Ingrepen.Remove(_selectedIngreep);
                        SelectedIngreep = Ingrepen.FirstOrDefault();
                        _selectedFaseCyclus.UpdateTypes();
                        TLCGenModelManager.Default.SetPrioOutputPerSignalGroup(Controller, Controller.PrioData.PrioUitgangPerFase);
                    },
                    () => _selectedFaseCyclus != null && _selectedIngreep != null));
            }
        }

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName => "Ingrepen";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            var temp = SelectedFaseCyclus;
            Fasen.Clear();
            SelectedFaseCyclus = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusWithPrioViewModel(fcm.Naam, _Controller.PrioData.PrioIngrepen);
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
