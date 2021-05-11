using System;
using System.Collections.Generic;
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
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.PrioriteitTab)]
    public class PrioIngrepenTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<PrioIngreepModel>
    {
        #region Fields

        private ObservableCollection<FaseCyclusWithPrioViewModel> _fasen;
        private object _selectedObject;
        private FaseCyclusWithPrioViewModel _selectedFaseCyclus;
        private PrioIngreepMeldingenListViewModel _selectedMeldingenList;
        private PrioIngreepViewModel _selectedIngreep;
        private PrioIngreepInUitMeldingViewModel _selectedMelding;
        private TemplateProviderViewModel<TLCGenTemplateModel<PrioIngreepModel>,PrioIngreepModel> _templatesProviderVm;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusWithPrioViewModel> Fasen => _fasen ??= new ObservableCollection<FaseCyclusWithPrioViewModel>();

        public object SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                switch (SelectedObject)
                {
                    case FaseCyclusWithPrioViewModel fc:
                        SelectedMeldingenList = null;
                        SelectedIngreep = null;
                        SelectedMelding = null;
                        SelectedFaseCyclus = fc;
                        break;
                    case PrioIngreepMeldingenListViewModel list:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMelding = null;
                        SelectedMeldingenList = list;
                        break;
                    case PrioIngreepViewModel ing:
                        SelectedFaseCyclus = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = null;
                        SelectedIngreep = ing;
                        break;
                    case PrioIngreepInUitMeldingViewModel mel:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = mel;
                        break;
                    default:
                        SelectedFaseCyclus = null;
                        SelectedIngreep = null;
                        SelectedMeldingenList = null;
                        SelectedMelding = null;
                        break;
                }
                RaisePropertyChanged();
            }
        }

        public FaseCyclusWithPrioViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value; 
                RaisePropertyChanged();
            }
        }
        
        public PrioIngreepViewModel SelectedIngreep
        {
            get => _selectedIngreep;
            set
            {
                _selectedIngreep = value; 
                RaisePropertyChanged();
            }
        }
        
        public PrioIngreepMeldingenListViewModel SelectedMeldingenList
        {
            get => _selectedMeldingenList;
            set
            {
                _selectedMeldingenList = value; 
                RaisePropertyChanged();
            }
        }

        public PrioIngreepInUitMeldingViewModel SelectedMelding
        {
            get => _selectedMelding;
            set
            {
                _selectedMelding = value; 
                RaisePropertyChanged();
            }
        }
        
        public TemplateProviderViewModel<TLCGenTemplateModel<PrioIngreepModel>, PrioIngreepModel> TemplatesProviderVm => 
            _templatesProviderVm ??= new TemplateProviderViewModel<TLCGenTemplateModel<PrioIngreepModel>, PrioIngreepModel>(this);

        #endregion // Properties
        
        #region Commands

        #endregion // Commands

        #region TabItem Overrides

        public override ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                if (_Controller == null) return;
                RefreshIngrepen();
            }
        }

        public override string DisplayName => "Ingrepen";

        public override bool CanBeEnabled()
        {
            return _Controller?.PrioData?.PrioIngreepType != PrioIngreepTypeEnum.Geen;
        }

        public override void OnSelected()
        {
            
        }

        #endregion // TabItem Overrides

        #region Private Methods

        private void RefreshIngrepen()
        {
            _fasen = null;
            foreach (var fcm in _Controller.Fasen)
            {
                var fcvm = 
                    new FaseCyclusWithPrioViewModel(
                        fcm.Naam, 
                        _Controller.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == fcm.Naam).ToList());
                Fasen.Add(fcvm);
            }
        }
        
        private void OnFasenChanged(FasenChangedMessage obj)
        {
            if (obj.RemovedFasen?.Any() == true)
            {
                foreach (var sg in obj.RemovedFasen)
                {
                    var vm = Fasen.FirstOrDefault(x => x.Naam == sg.Naam);
                    if (vm == null) continue;
                    Fasen.Remove(vm);
                }
            }
            if (obj.AddedFasen?.Any() == true)
            {
                foreach (var sg in obj.AddedFasen)
                {
                    var fcvm = 
                        new FaseCyclusWithPrioViewModel(
                            sg.Naam, 
                            _Controller.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == sg.Naam).ToList());
                    Fasen.Add(fcvm);
                }
            }
            Fasen.BubbleSort();
        }

        private void OnPrioIngreepMassaDetectieObjectNeedsFaseCyclusMessageReceived(PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage obj)
        {
            foreach (var sg in Fasen)
            {
                foreach (var ingreep in sg.Ingrepen)
                {
                    foreach (var prioIngreepInUitMeldingViewModel in ingreep.MeldingenLists.SelectMany(x => x.Meldingen))
                    {
                        if (ReferenceEquals(obj.RequestingObject, prioIngreepInUitMeldingViewModel))
                        {
                            obj.FaseCyclus = sg.Naam;
                            obj.Ingreep = ingreep.Naam;
                            return;
                        }
                    }
                }
            }
        }

        private void OnNameChanged(NameChangedMessage obj)
        {
            if (obj.ObjectType != TLCGenObjectTypeEnum.Fase) return;
            foreach (var faseWithPrio in Fasen)
            {
                if (obj.OldName == faseWithPrio.Naam) faseWithPrio.Naam = obj.NewName;
            }
        }

        #endregion // Private Methods

        #region Constructor

        public PrioIngrepenTabViewModel() : base()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage>(this, OnPrioIngreepMassaDetectieObjectNeedsFaseCyclusMessageReceived);
            MessengerInstance.Register<PrioIngrepenChangedMessage>(this, OnPrioIngrepenChangedMessageReceived);
        }

        private void OnPrioIngrepenChangedMessageReceived(PrioIngrepenChangedMessage obj)
        {
            RefreshIngrepen();
        }

        #endregion // Constructor

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<PrioIngreepModel> items)
        {
            if (_Controller == null)
                return;

            foreach (var prio in items)
            {
                ControllerAccessProvider.Default.Controller.PrioData.PrioIngrepen.Add(prio);
                ControllerAccessProvider.Default.Controller.PrioData.PrioIngrepen.BubbleSort();
                // needed to regulate KAR dummies
                var inM = prio.MeldingenData.Inmeldingen.FirstOrDefault(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding); 
                var uitM = prio.MeldingenData.Uitmeldingen.FirstOrDefault(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding); 
                if (inM != null) MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, inM));
                if (uitM != null) MessengerInstance.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, uitM));
                SelectedFaseCyclus.Ingrepen.Add(new PrioIngreepViewModel(prio, SelectedFaseCyclus));
            }
            
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        public void UpdateAfterApplyTemplate(PrioIngreepModel item)
        {
        }

        #endregion // IAllowTemplates
    }
}
