using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.AlgemeenTab)]
    public class VersieBeheerTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private VersieViewModel _SelectedVersie;

        #endregion // Fields

        #region Properties

        public VersieViewModel SelectedVersie
        {
            get { return _SelectedVersie; }
            set
            {
                _SelectedVersie = value;
                OnPropertyChanged("SelectedVersie");
            }
        }

        private ObservableCollection<VersieViewModel> _Versies;
        public ObservableCollection<VersieViewModel> Versies
        {
            get
            {
                if (_Versies == null)
                {
                    _Versies = new ObservableCollection<VersieViewModel>();
                }
                return _Versies;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddVersieCommand;
        public ICommand AddVersieCommand
        {
            get
            {
                if (_AddVersieCommand == null)
                {
                    _AddVersieCommand = new RelayCommand(AddVersieCommand_Executed, AddVersieCommand_CanExecute);
                }
                return _AddVersieCommand;
            }
        }

        RelayCommand _RemoveVersieCommand;
        public ICommand RemoveVersieCommand
        {
            get
            {
                if (_RemoveVersieCommand == null)
                {
                    _RemoveVersieCommand = new RelayCommand(RemoveVersieCommand_Executed, RemoveVersieCommand_CanExecute);
                }
                return _RemoveVersieCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        void AddVersieCommand_Executed()
        {
            VersieModel vm = new VersieModel();
            vm.Datum = DateTime.Now;
            string nextver = null;
            if (Versies != null && Versies.Count > 0)
            {
                Match m = Regex.Match(Versies[Versies.Count - 1].Versie, @"([0-9]+)\.([0-9]+)\.([0-9]+)");
                if (m.Groups.Count == 4)
                {
                    string midver = m.Groups[2].Value;
                    int nextmidver;
                    if (Int32.TryParse(midver, out nextmidver))
                    {
                        nextver = m.Groups[1].Value + "." + (nextmidver + 1).ToString() + ".0";
                    }
                }
            }
            vm.Versie = nextver == null ? "1.0.0" : nextver;
            vm.Ontwerper = Environment.UserName;
            VersieViewModel vvm = new VersieViewModel(vm);
            Versies.Add(vvm);
        }

        bool AddVersieCommand_CanExecute()
        {
            return Versies != null;
        }

        void RemoveVersieCommand_Executed()
        {
            Versies.Remove(SelectedVersie);
            SelectedVersie = null;
        }

        bool RemoveVersieCommand_CanExecute()
        {
            return Versies != null && Versies.Count > 0 && SelectedVersie != null;
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Versiebeheer";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
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
                if (base.Controller != null)
                {
                    Versies.CollectionChanged -= Versies_CollectionChanged;
                    Versies.Clear();
                    foreach (VersieModel vm in _Controller.Data.Versies)
                    {
                        VersieViewModel vvm = new VersieViewModel(vm);
                        Versies.Add(vvm);
                    }
                    Versies.CollectionChanged += Versies_CollectionChanged;
                    OnPropertyChanged(null);
                }
                else
                {
                    Versies.CollectionChanged -= Versies_CollectionChanged;
                    Versies.Clear();
                    OnPropertyChanged(null);
                }
            }
        }

        #endregion // TabItem Overrides

        #region Collection Changed

        private void Versies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.NewItems)
                {
                    _Controller.Data.Versies.Add(vvm.VersieEntry);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (VersieViewModel vvm in e.OldItems)
                {
                    _Controller.Data.Versies.Remove(vvm.VersieEntry);
                }
            }
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public VersieBeheerTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
