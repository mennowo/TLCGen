using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
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

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> Periodes
        {
            get; private set;
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
            get { return _SelectedPeriode; }
            set
            {
                _SelectedPeriode = value;
                RaisePropertyChanged("SelectedPeriode");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddPeriodeCommand;
        public ICommand AddPeriodeCommand
        {
            get
            {
                if (_AddPeriodeCommand == null)
                {
                    _AddPeriodeCommand = new RelayCommand(AddNewPeriodeCommand_Executed, AddNewPeriodeCommand_CanExecute);
                }
                return _AddPeriodeCommand;
            }
        }


        RelayCommand _RemovePeriodeCommand;
        public ICommand RemovePeriodeCommand
        {
            get
            {
                if (_RemovePeriodeCommand == null)
                {
                    _RemovePeriodeCommand = new RelayCommand(RemovePeriodeCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _RemovePeriodeCommand;
            }
        }

        RelayCommand _MovePeriodeUpCommand;
        public ICommand MovePeriodeUpCommand
        {
            get
            {
                if (_MovePeriodeUpCommand == null)
                {
                    _MovePeriodeUpCommand = new RelayCommand(MovePeriodeUpCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _MovePeriodeUpCommand;
            }
        }

        RelayCommand _MovePeriodeDownCommand;
        public ICommand MovePeriodeDownCommand
        {
            get
            {
                if (_MovePeriodeDownCommand == null)
                {
                    _MovePeriodeDownCommand = new RelayCommand(MovePeriodeDownCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _MovePeriodeDownCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void MovePeriodeUpCommand_Executed(object obj)
        {
            int index = -1;
            foreach (PeriodeViewModel mvm in Periodes)
            {
                ++index;
                if (mvm == SelectedPeriode)
                {
                    break;
                }
            }
            if (index >= 1 && Periodes[index - 1].Type != Models.Enumerations.PeriodeTypeEnum.Groentijden)
            {
                PeriodeViewModel mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index - 1, mvm);
                SelectedPeriode = mvm;
                Periodes.RebuildList();
                Messenger.Default.Send(new PeriodenChangedMessage());
            }

        }


        private void MovePeriodeDownCommand_Executed(object obj)
        {
            int index = -1;
            foreach (PeriodeViewModel mvm in Periodes)
            {
                ++index;
                if (mvm == SelectedPeriode)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (Periodes.Count - 2)) && Periodes[index + 1].Type != Models.Enumerations.PeriodeTypeEnum.Groentijden)
            {
                PeriodeViewModel mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index + 1, mvm);
                SelectedPeriode = mvm;
                Periodes.RebuildList();
                Messenger.Default.Send(new PeriodenChangedMessage());
            }

        }

        void AddNewPeriodeCommand_Executed(object prm)
        {
            PeriodeModel mm = new PeriodeModel();
            mm.Type = PeriodeTypeEnum.Overig;
            mm.DagCode = PeriodeDagCodeEnum.AlleDagen;
			var inewname = Periodes.Count;
	        do
	        {
		        inewname++;
		        mm.Naam = "periode" + (inewname < 10 ? "0" : "") + inewname;
	        }
	        while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Periode, mm.Naam));

			PeriodeViewModel mvm = new PeriodeViewModel(mm);

            if(Periodes.Any(x => x.Type != PeriodeTypeEnum.Groentijden))
            {
                int index = Periodes.Count(x => x.Type != PeriodeTypeEnum.Groentijden);
                Periodes.Insert(index, mvm);
            }
            else
            {
                Periodes.Insert(0, mvm);
            }

            Periodes.RebuildList();
	        Messenger.Default.Send(new PeriodenChangedMessage());
		}

		bool AddNewPeriodeCommand_CanExecute(object prm)
        {
            return Periodes != null;
        }

        void RemovePeriodeCommand_Executed(object prm)
        {
			TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedPeriode.Naam);
	        Periodes.Remove(SelectedPeriode);
	        SelectedPeriode = null;
	        Messenger.Default.Send(new PeriodenChangedMessage());
		}

        bool ChangePeriodeCommand_CanExecute(object prm)
        {
            return SelectedPeriode != null;
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Overig";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
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
            get
            {
                return base.Controller;
            }

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
                    ICollectionView view = CollectionViewSource.GetDefaultView(Periodes);
                    view.Filter = FilterPerioden;
                }
                else
                {
                    Periodes = null;
                }
                RaisePropertyChanged("Periodes");
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
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region TLCGen Events

        private void OnPeriodenChanged(PeriodenChangedMessage message)
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

        private List<string> _PeriodeTypeOpties;
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
            Messenger.Default.Register(this, new Action<PeriodenChangedMessage>(OnPeriodenChanged));

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
