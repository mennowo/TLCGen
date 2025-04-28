
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
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


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.PeriodenTab)]
    public class PeriodenOverigTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private PeriodeViewModel _SelectedPeriode;
        private ObservableCollection<string> _GroentijdenSets;
        private RelayCommand _AddPeriodeCommand;
        private RelayCommand _RemovePeriodeCommand;
        private RelayCommand _MovePeriodeUpCommand;
        private RelayCommand _MovePeriodeDownCommand;
        private List<string> _PeriodeTypeOpties;
        private ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> _periodes;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> Periodes
        {
            get => _periodes;
            private set
            {
                _periodes = value;
                OnPropertyChanged();
                _AddPeriodeCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GroentijdenSets
        {
            get
            {
                if (_GroentijdenSets == null)
                {
                    _GroentijdenSets = new ObservableCollection<string>();
                }
                return _GroentijdenSets;
            }
        }

        public PeriodeViewModel SelectedPeriode
        {
            get => _SelectedPeriode;
            set
            {
                _SelectedPeriode = value;
                OnPropertyChanged();
                _RemovePeriodeCommand?.NotifyCanExecuteChanged();
                _MovePeriodeUpCommand?.NotifyCanExecuteChanged();
                _MovePeriodeDownCommand?.NotifyCanExecuteChanged();
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddPeriodeCommand => _AddPeriodeCommand ??= new RelayCommand(() =>
        {
            var mm = new PeriodeModel();
            mm.Type = PeriodeTypeEnum.Overig;
            mm.DagCode = PeriodeDagCodeEnum.AlleDagen;
            var inewname = Periodes.Count;
            do
            {
                inewname++;
                mm.Naam = "periode" + (inewname < 10 ? "0" : "") + inewname;
            }
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, mm.Naam));

            var mvm = new PeriodeViewModel(mm);

            if(Periodes.Any(x => x.Type != PeriodeTypeEnum.Groentijden))
            {
                var index = Periodes.Count(x => x.Type != PeriodeTypeEnum.Groentijden);
                Periodes.Insert(index, mvm);
            }
            else
            {
                Periodes.Insert(0, mvm);
            }

            Periodes.RebuildList();
            WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
        }, () => Periodes != null);

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

                    if (index == 0 || index + 1 < Periodes.Count && Periodes[index + 1].Type != PeriodeTypeEnum.Groentijden)
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

                    if (index == Periodes.Count - 1 || index - 1 >= 0 && Periodes[index - 1].Type != PeriodeTypeEnum.Groentijden)
                    {
                        repeat = false;
                    }
                }   
            }
        }, () => SelectedPeriode != null);

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName => "Overig";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
	        if (_Controller.PeriodenData.Perioden.Count != Periodes.Count)
	        {
		        Periodes.Rebuild();
	        }
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
            return per.Type != Models.Enumerations.PeriodeTypeEnum.Groentijden;
        }

        #endregion // Private Methods

        #region Collection Changed

        private void Periodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region TLCGen Events

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage message)
        {
            var sel = SelectedPeriode;
            Periodes.Rebuild();
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

        public List<string> PeriodeTypeOpties
        {
            get
            {
                if (_PeriodeTypeOpties == null)
                {
                    _PeriodeTypeOpties = new List<string>();
                }
                return _PeriodeTypeOpties;
            }
        }

        public PeriodenOverigTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);

            PeriodeTypeOpties.Clear();
            var descs = Enum.GetValues(typeof(PeriodeTypeEnum));
            foreach (PeriodeTypeEnum d in descs)
            {
                if (d != PeriodeTypeEnum.Groentijden)
                {
                    PeriodeTypeOpties.Add(d.GetDescription());
                }
            }
        }

        #endregion // Constructor
    }
}
