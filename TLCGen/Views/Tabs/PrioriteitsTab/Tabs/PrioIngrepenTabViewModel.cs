﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;


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
                OnPropertyChanged();
            }
        }

        public FaseCyclusWithPrioViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value; 
                OnPropertyChanged();
            }
        }
        
        public PrioIngreepViewModel SelectedIngreep
        {
            get => _selectedIngreep;
            set
            {
                _selectedIngreep = value; 
                OnPropertyChanged();
            }
        }
        
        public PrioIngreepMeldingenListViewModel SelectedMeldingenList
        {
            get => _selectedMeldingenList;
            set
            {
                _selectedMeldingenList = value; 
                OnPropertyChanged();
            }
        }

        public PrioIngreepInUitMeldingViewModel SelectedMelding
        {
            get => _selectedMelding;
            set
            {
                _selectedMelding = value; 
                OnPropertyChanged();
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
        
        private void OnFasenChanged(object sender, FasenChangedMessage obj)
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

        private void OnPrioIngreepMassaDetectieObjectNeedsFaseCyclusMessageReceived(object sender, PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage obj)
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

        private void OnNameChanged(object sender, NameChangedMessage obj)
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
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<PrioIngreepMeldingNeedsFaseCyclusAndIngreepMessage>(this, OnPrioIngreepMassaDetectieObjectNeedsFaseCyclusMessageReceived);
            WeakReferenceMessengerEx.Default.Register<PrioIngrepenChangedMessage>(this, OnPrioIngrepenChangedMessageReceived);
        }

        private void OnPrioIngrepenChangedMessageReceived(object sender, PrioIngrepenChangedMessage obj)
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
                if (inM != null) WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, inM));
                if (uitM != null) WeakReferenceMessengerEx.Default.Send(new PrioIngreepMeldingChangedMessage(prio.FaseCyclus, uitM));
                SelectedFaseCyclus.Ingrepen.Add(new PrioIngreepViewModel(prio, SelectedFaseCyclus));
            }
            
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        public void UpdateAfterApplyTemplate(PrioIngreepModel item)
        {
        }

        #endregion // IAllowTemplates
    }
}
