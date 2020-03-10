using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 10, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioIngrepenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FaseCyclusWithPrioViewModel> _fasen;
        private RelayCommand _addIngreepCommand;
        private RelayCommand _removeIngreepCommand;
        private object _selectedObject;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusWithPrioViewModel> Fasen => _fasen ?? (_fasen = new ObservableCollection<FaseCyclusWithPrioViewModel>());

        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value; 
                RaisePropertyChanged();
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
                            ///FaseCyclus = SelectedFaseCyclus.Naam,
                            Type = PrioIngreepVoertuigTypeEnum.Bus
                        };
                        var newName = prio.FaseCyclus + DefaultsProvider.Default.GetVehicleTypeAbbreviation(prio.Type);
                        if (!NameSyntaxChecker.IsValidCName(newName))
                        {
                            newName = prio.FaseCyclus + "default";
                        }

                        var iNewName = 0;
                        var tempName = newName;
                        while (!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(
                            DataAccess.TLCGenControllerDataProvider.Default.Controller, tempName,
                            TLCGenObjectTypeEnum.PrioriteitsIngreep))
                        {
                            tempName = newName + ++iNewName;
                        }

                        prio.Naam = DefaultsProvider.Default.GetVehicleTypeAbbreviation(prio.Type) + (iNewName == 0 ? "" : iNewName.ToString());

                        DefaultsProvider.Default.SetDefaultsOnModel(prio);
                        DefaultsProvider.Default.SetDefaultsOnModel(prio.MeldingenData);
                        PrioIngreepInUitMeldingModel inM = null;
                        PrioIngreepInUitMeldingModel uitM = null;
                        if (prio.Type == PrioIngreepVoertuigTypeEnum.Bus)
                        {
                            inM = new PrioIngreepInUitMeldingModel
                            {
                                AntiJutterTijdToepassen = true,
                                AntiJutterTijd = 15,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            };
                            prio.MeldingenData.Inmeldingen.Add(inM);
                            uitM = new PrioIngreepInUitMeldingModel
                            {
                                AntiJutterTijdToepassen = false,
                                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding,
                                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding
                            };
                            prio.MeldingenData.Uitmeldingen.Add(uitM);
                        }
                        _Controller.PrioData.PrioIngrepen.Add(prio);
                        _Controller.PrioData.PrioIngrepen.BubbleSort();
                        var prioVm = new PrioIngreepViewModel(prio);
                        if (inM != null) MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, inM));
                        if (uitM != null) MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, uitM));
                        ///SelectedObject = prioVm;
                        //_selectedFaseCyclus.UpdateTypes();
                    },
                    () => true //_selectedFaseCyclus != null
                           ));
            }
        }
        
        public ICommand RemoveIngreepCommand
        {
            get
            {
                return _removeIngreepCommand ?? (_removeIngreepCommand = new RelayCommand(
                    () =>
                    {
                        ///_Controller.PrioData.PrioIngrepen.Remove(_selectedIngreep.PrioIngreep);
                        ///Ingrepen.Remove(_selectedIngreep);
                        ///SelectedIngreep = Ingrepen.FirstOrDefault();
                        ///_selectedFaseCyclus.UpdateTypes();
                        ///TLCGenModelManager.Default.SetPrioOutputPerSignalGroup(Controller, Controller.PrioData.PrioUitgangPerFase);
                    },
                    () => true ///_selectedFaseCyclus != null && _selectedIngreep != null
                          ));
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
            ///var temp = SelectedFaseCyclus;
            ///Fasen.Clear();
            ///SelectedFaseCyclus = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = new FaseCyclusWithPrioViewModel(fcm.Naam, _Controller.PrioData.PrioIngrepen
                    .Where(x => x.FaseCyclus == fcm.Naam)
                    .Select(x => new PrioIngreepViewModel(x)).ToList());
                Fasen.Add(fcvm);
            ///    if (temp == null || fcvm.Naam != temp.Naam) continue;
            ///    SelectedFaseCyclus = fcvm;
            ///    temp = null;
            }
            ///if (SelectedFaseCyclus == null && Fasen.Count > 0)
            ///{
            ///    SelectedFaseCyclus = Fasen[0];
            ///}
        }

        #endregion // TabItem Overrides

        #region Private Methods
        #endregion // Private Methods

        #region Constructor

        public PrioIngrepenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
