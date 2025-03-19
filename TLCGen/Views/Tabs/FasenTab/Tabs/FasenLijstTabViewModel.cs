
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<FaseCyclusModel>
    {
        #region Fields

        private FaseCyclusViewModel _selectedFaseCyclus;
        private IList _selectedFaseCycli = new ArrayList();
        private TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> _templatesProviderVm;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen => ControllerAccessProvider.Default.AllSignalGroups;

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
                if (value != null) TemplatesProviderVm.SetSelectedApplyToItem(value.FaseCyclus);
            }
        }

        public IList SelectedFaseCycli
        {
            get => _selectedFaseCycli;
            set
            {
                _selectedFaseCycli = value;
                _SettingMultiple = false;
                OnPropertyChanged("SelectedFaseCycli");
                if (value != null)
                {
                    var sl = new List<FaseCyclusModel>();
                    foreach(var s in value)
                    {
                        sl.Add((s as FaseCyclusViewModel).FaseCyclus);
                    }
                    TemplatesProviderVm.SetSelectedApplyToItems(sl);
                }
            }
        }

        public TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel> TemplatesProviderVm => _templatesProviderVm ??= new TemplateProviderViewModel<TLCGenTemplateModel<FaseCyclusModel>, FaseCyclusModel>(this);

        #endregion // Properties

        #region Commands

        RelayCommand _AddFaseCommand;
        public ICommand AddFaseCommand
        {
            get
            {
                if (_AddFaseCommand == null)
                {
                    _AddFaseCommand = new RelayCommand(AddNewFaseCommand_Executed, AddNewFaseCommand_CanExecute);
                }
                return _AddFaseCommand;
            }
        }


        RelayCommand _RemoveFaseCommand;
        public ICommand RemoveFaseCommand
        {
            get
            {
                if (_RemoveFaseCommand == null)
                {
                    _RemoveFaseCommand = new RelayCommand(RemoveFaseCommand_Executed, RemoveFaseCommand_CanExecute);
                }
                return _RemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewFaseCommand_Executed()
        {
            var fcm = new FaseCyclusModel();

            string newname;
            var inext = 0;
            foreach (var fcvm in Fasen)
            {
                if (int.TryParse(fcvm.Naam, out var inewname))
                {
                    inext = inewname > inext ? inewname : inext;
                }
            }
            do
            {
                inext++;
                newname = (inext < 10 ? "0" : "") + inext;
            }
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, newname));

            fcm.Naam = newname;
            fcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fcm.Naam);
            DefaultsProvider.Default.SetDefaultsOnModel(fcm, fcm.Type.ToString());
            
            // This will cause the model to be updated
WeakReferenceMessenger.Default.Send(new FasenChangingMessage(new List<FaseCyclusModel>{fcm}, null));
        }

        bool AddNewFaseCommand_CanExecute() => Fasen != null;

        void RemoveFaseCommand_Executed()
        {
            var changed = false;
            var remfcs = new List<FaseCyclusModel>();
            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 0)
            {
                changed = true;
                foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
                {
                    TLCGenControllerModifier.Default.RemoveModelItemFromController(fcvm.Naam, TLCGenObjectTypeEnum.Fase);
                    remfcs.Add(fcvm.FaseCyclus);
                }

                SelectedFaseCycli = null;
            }
            else if (SelectedFaseCyclus != null)
            {
                changed = true;
                remfcs.Add(SelectedFaseCyclus.FaseCyclus);
                TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedFaseCyclus.Naam, TLCGenObjectTypeEnum.Fase);
                SelectedFaseCyclus = null;
            }

            if(changed)
            {
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                foreach (var fc in _Controller.Fasen)
                {
                    Fasen.Add(new FaseCyclusViewModel(fc));
                }
                Fasen.CollectionChanged += Fasen_CollectionChanged;
WeakReferenceMessenger.Default.Send(new FasenChangingMessage(null, remfcs));
            }

        }

        bool RemoveFaseCommand_CanExecute()
        {
            return Fasen != null &&
                (SelectedFaseCyclus != null ||
                 SelectedFaseCycli != null && SelectedFaseCycli.Count > 0);
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName => "Overzicht";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
        }

        public override bool OnDeselectedPreview()
        {
            if (!Fasen.IsSorted())
            {
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.BubbleSort();
                _Controller.Fasen.Clear();
                foreach(var fcvm in Fasen)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                }
WeakReferenceMessenger.Default.Send(new FasenSortedMessage(_Controller.Fasen));
                Fasen.CollectionChanged += Fasen_CollectionChanged;
            }
            return true;
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                // TODO these kind of actions should happen elsewhere
                Fasen.CollectionChanged -= Fasen_CollectionChanged;
                Fasen.Clear();
                if (base.Controller != null)
                {
                    foreach (var fcm in base.Controller.Fasen)
                    {
                        var fcvm = new FaseCyclusViewModel(fcm);
                        fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
                        Fasen.Add(fcvm);
                    }
                    Fasen.CollectionChanged += Fasen_CollectionChanged;
                }
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Event handling
        #endregion // TLCGen Event handling

        #region Event handling

        private bool _SettingMultiple = false;
        private void FaseCyclus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedFaseCycli);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    fcvm.PropertyChanged += FaseCyclus_PropertyChanged;
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    fcvm.PropertyChanged -= FaseCyclus_PropertyChanged;
                }
            }
        }

        #endregion // Collection Changed

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<FaseCyclusModel> items)
        {
            if (_Controller == null)
                return;

            foreach (var fc in items)
            {
                if (!(TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, fc.Naam, TLCGenObjectTypeEnum.Fase) &&
                     (fc.Detectoren.Count == 0 || fc.Detectoren.All(x => TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, x.Naam, TLCGenObjectTypeEnum.Detector) != false))))
                {
                    MessageBox.Show("Error bij toevoegen van fase met naam " + fc.Naam + ".\nFase en/of bijbehorende detectie is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
WeakReferenceMessenger.Default.Send(new FasenChangingMessage(items, null));
        }

        public void UpdateAfterApplyTemplate(FaseCyclusModel item)
        {
            var fc = Fasen.First(x => x.FaseCyclus == item);
            fc.OnPropertyChanged("");
WeakReferenceMessenger.Default.Send(new DetectorenChangedMessage(_Controller, null, null));
        }

        #endregion // IAllowTemplates

        #region Constructor

        public FasenLijstTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
