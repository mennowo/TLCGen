
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.PeriodenTab)]
    public class PeriodenGroentijdenTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<PeriodeModel>
    {
        #region Fields
        
        private PeriodeViewModel _SelectedPeriode;
        private ObservableCollection<string> _GroentijdenSets;

        private RelayCommand _AddPeriodeCommand;
        private RelayCommand _RemovePeriodeCommand;
        private RelayCommand _MovePeriodeUpCommand;
        private RelayCommand _MovePeriodeDownCommand;

        private ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> _Periodes;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> Periodes
        {
            get => _Periodes;
            set
            {
                _Periodes = value;
                OnPropertyChanged();
                _AddPeriodeCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GroentijdenSets { get; } = [];

        public PeriodeViewModel SelectedPeriode
        {
            get => _SelectedPeriode;
            set
            {
                _SelectedPeriode = value;
                OnPropertyChanged("SelectedPeriode");
                TemplatesProviderVM.SetSelectedApplyToItem(value?.Periode);
                _RemovePeriodeCommand?.NotifyCanExecuteChanged();
                _MovePeriodeUpCommand?.NotifyCanExecuteChanged();
                _MovePeriodeDownCommand?.NotifyCanExecuteChanged();
            }
        }

        public string DefaultPeriodeGroentijdenSet
        {
            get => _Controller?.PeriodenData.DefaultPeriodeGroentijdenSet;
            set
            {
                if (value != null)
                {
                    _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = value;
                    // TODO Check OK ? OnPropertyChanged(nameof(DefaultPeriodeGroentijdenSet), null, null, true);
                    OnPropertyChanged(broadcast: true);
                }
            }
        }

        public string DefaultPeriodeNaam
        {
            get => _Controller?.PeriodenData.DefaultPeriodeNaam;
            set
            {
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, value))
                {
		            var oldName = _Controller.PeriodenData.DefaultPeriodeNaam;
					_Controller.PeriodenData.DefaultPeriodeNaam = value;
		            WeakReferenceMessengerEx.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.Periode, oldName, value));
	            }
                // TODO Check OK ? OnPropertyChanged(nameof(DefaultPeriodeNaam), null, null, true);
                OnPropertyChanged(broadcast: true);
            }
        }

        private TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel> _TemplatesProviderVM;
        public TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel> TemplatesProviderVM
        {
            get
            {
                if (_TemplatesProviderVM == null)
                {
                    _TemplatesProviderVM = new TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel>(this);
                }
                return _TemplatesProviderVM;
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddPeriodeCommand => _AddPeriodeCommand ??= new RelayCommand(() =>
        {
            var mm = new PeriodeModel();
            mm.Type = PeriodeTypeEnum.Groentijden;
            mm.DagCode = PeriodeDagCodeEnum.AlleDagen;
            var inewname = Periodes.Count;
            do
            {
                inewname++;
                mm.Naam = "periode" + (inewname < 10 ? "0" : "") + inewname;
            }
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, mm.Naam));
            var mvm = new PeriodeViewModel(mm);
            var mgset = _Controller.GroentijdenSets.FirstOrDefault();
            if (mgset != null) mvm.GroentijdenSet = mgset.Naam;

            if (Periodes.Any(x => x.Type == PeriodeTypeEnum.Groentijden))
            {
                var index = Periodes.Count(x => x.Type == PeriodeTypeEnum.Groentijden);
                Periodes.Insert(index, mvm);
            }
            else
            {
                Periodes.Insert(0, mvm);
            }
            WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
        }, () => Periodes != null && GroentijdenSets?.Count > 0);

        public ICommand RemovePeriodeCommand => _RemovePeriodeCommand ??= new RelayCommand(() =>
        {
            TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedPeriode.Naam, TLCGenObjectTypeEnum.Periode);
            Periodes.Remove(SelectedPeriode);
            SelectedPeriode = null;
            WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
        }, () => SelectedPeriode != null);

        public ICommand MovePeriodeUpCommand => _MovePeriodeUpCommand ??= new RelayCommand(() =>
        {
            var index = Periodes.IndexOf(SelectedPeriode);
            if (index >= 1)
            {
                var repeat = true;
                while(repeat)
                {
                    var mvm = SelectedPeriode;
                    SelectedPeriode = null;
                    Periodes.Remove(mvm);
                    Periodes.Insert(index - 1, mvm);
                    SelectedPeriode = mvm;
                    Periodes.RebuildList();
                    WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
                    index = Periodes.IndexOf(SelectedPeriode);

                    if (index == 0 || index + 1 < Periodes.Count && Periodes[index + 1].Type == PeriodeTypeEnum.Groentijden)
                    {
                        repeat = false;
                    }
                }   
            }
        }, () => SelectedPeriode != null);

        public ICommand MovePeriodeDownCommand => _MovePeriodeDownCommand ??= new RelayCommand(() =>
        {
            var index = Periodes.IndexOf(SelectedPeriode);
            if (index - 1 < Periodes.Count)
            {
                var repeat = true;
                while(repeat)
                {
                    var mvm = SelectedPeriode;
                    SelectedPeriode = null;
                    Periodes.Remove(mvm);
                    if (index >= Periodes.Count - 1)
                    {
                        Periodes.Add(mvm);
                    }
                    else
                    {
                        Periodes.Insert(index + 1, mvm);   
                    }
                    SelectedPeriode = mvm;
                    Periodes.RebuildList();
                    WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
                    index = Periodes.IndexOf(SelectedPeriode);

                    if (index == Periodes.Count - 1 || index - 1 >= 0 && Periodes[index - 1].Type == PeriodeTypeEnum.Groentijden)
                    {
                        repeat = false;
                    }
                }   
            }
        }, () => SelectedPeriode != null);

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName => "Groentijden";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            var v = _Controller.PeriodenData.DefaultPeriodeGroentijdenSet;
            GroentijdenSets.Clear();
            foreach (var gsm in _Controller.GroentijdenSets)
            {
                GroentijdenSets.Add(gsm.Naam);
            }
            _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = v;
            OnPropertyChanged("DefaultPeriodeGroentijdenSet");
            _AddPeriodeCommand?.NotifyCanExecuteChanged();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (Periodes != null)
                {
                    Periodes.CollectionChanged -= Periodes_CollectionChanged;
                }
                if (base.Controller != null)
                {
                    Periodes = new ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel>(base.Controller.PeriodenData.Perioden);
                    Periodes.CollectionChanged += Periodes_CollectionChanged;
                    var view = CollectionViewSource.GetDefaultView(Periodes);
                    view.Filter = FilterPerioden;
                }
                else
                {
                    Periodes = null;
                }
                OnPropertyChanged("Periodes");
            }
        }

        #endregion // TabItem Overrides

        #region Private Methods

        private bool FilterPerioden(object o)
        {
            var per = (PeriodeViewModel)o;
            return per.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden;
        }

        #endregion // Private Methods

        #region Collection Changed

        private void Periodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<PeriodeModel> items)
        {
            if (_Controller == null)
                return;

            foreach (var per in items)
            {
                if (!(TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, per.Naam, TLCGenObjectTypeEnum.Periode)))
                {
                    MessageBox.Show("Error bij toevoegen van periode met naam " + per.Naam + ".\nNaam van de periode is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
                if(_Controller.GroentijdenSets.All(x => x.Naam != per.GroentijdenSet))
                {
                    MessageBox.Show("Error bij toevoegen van periode verwijzend naar groentijdenset " + per.GroentijdenSet + ".\nDeze groentijdenset ontbreekt in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
            foreach (var per in items)
            {
                Periodes.Add(new PeriodeViewModel(per));
            }
WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
        }

        public void UpdateAfterApplyTemplate(PeriodeModel item)
        {
            var p = Periodes.First(x => x.Periode == item);
            p.OnPropertyChanged("");
        }

        #endregion // IAllowTemplates

        #region TLCGen Events

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage message)
        {
            var sel = SelectedPeriode;
            Periodes.CollectionChanged -= Periodes_CollectionChanged;
            Periodes.Rebuild();
            Periodes.CollectionChanged += Periodes_CollectionChanged;
            if (sel != null)
            {
                foreach (var p in Periodes)
                {
                    if (sel.Naam == p.Naam)
                    {
                        SelectedPeriode = p;
                    }
                }
            }
        }

        #endregion // TLCGen Events

        #region Constructor

        public PeriodenGroentijdenTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
        }

        #endregion // Constructor
    }
}
