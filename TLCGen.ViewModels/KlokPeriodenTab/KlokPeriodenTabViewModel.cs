using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 5)]
    public class KlokPeriodenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private PeriodeViewModel _SelectedPeriode;
        private ObservableCollection<PeriodeViewModel> _Periodes;
        private ObservableCollection<string> _GroentijdenSets;

        #endregion // Fields

        #region Properties

        public ObservableCollection<PeriodeViewModel> Periodes
        {
            get
            {
                if (_Periodes == null)
                {
                    _Periodes = new ObservableCollection<PeriodeViewModel>();
                }
                return _Periodes;
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
            get { return _SelectedPeriode; }
            set
            {
                _SelectedPeriode = value;
                OnPropertyChanged("SelectedPeriode");
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

        #region Command functionality

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
            if (index >= 1)
            {
                PeriodeViewModel mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index - 1, mvm);
                SelectedPeriode = mvm;
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
            if (index >= 0 && (index <= (Periodes.Count - 2)))
            {
                PeriodeViewModel mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index + 1, mvm);
                SelectedPeriode = mvm;
            }
        }

        void AddNewPeriodeCommand_Executed(object prm)
        {
            PeriodeModel mm = new PeriodeModel();
            mm.Naam = "Periode" + (Periodes.Count + 1).ToString();
            PeriodeViewModel mvm = new PeriodeViewModel(mm);
            Periodes.Add(mvm);
        }

        bool AddNewPeriodeCommand_CanExecute(object prm)
        {
            return Periodes != null;
        }

        void RemovePeriodeCommand_Executed(object prm)
        {
            Periodes.Remove(SelectedPeriode);
        }

        bool ChangePeriodeCommand_CanExecute(object prm)
        {
            return SelectedPeriode != null;
        }

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Klokperioden";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            GroentijdenSets.Clear();
            foreach (GroentijdenSetModel gsm in _Controller.GroentijdenSets)
            {
                GroentijdenSets.Add(gsm.Naam);
            }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        #endregion // Public Methods

        #region Collection Changed

        private void Periodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PeriodeViewModel pvm in e.NewItems)
                {
                    _Controller.Perioden.Add(pvm.Periode);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PeriodeViewModel pvm in e.OldItems)
                {
                    _Controller.Perioden.Remove(pvm.Periode);
                }
            }
            MessageManager.Instance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public KlokPeriodenTabViewModel(ControllerModel controller) : base(controller)
        {
            foreach(PeriodeModel periode in controller.Perioden)
            {
                Periodes.Add(new PeriodeViewModel(periode));
            }

            Periodes.CollectionChanged += Periodes_CollectionChanged;
        }

        #endregion // Constructor
    }
}
