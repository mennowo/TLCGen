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
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModulesTabViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private FaseCyclusModuleViewModel _SelectedFaseCyclus;
        private ModuleViewModel _SelectedModule;
        private ObservableCollection<FaseCyclusModuleViewModel> _Fasen;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusModuleViewModel> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusModuleViewModel>();
                }
                return _Fasen;
            }
        }

        public FaseCyclusModuleViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        public ModuleViewModel SelectedModule
        {
            get { return _SelectedModule; }
            set
            {
                _SelectedModule = value;
                foreach(FaseCyclusModuleViewModel fcmvm in Fasen)
                {
                    fcmvm.ModuleVM = value;
                    fcmvm.UpdateModuleInfo();
                }
                OnPropertyChanged("SelectedModule");
            }
        }

        public ObservableCollection<ModuleViewModel> Modules
        {
            get
            {   
                return _ControllerVM?.Modules;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddModuleCommand;
        public ICommand AddModuleCommand
        {
            get
            {
                if (_AddModuleCommand == null)
                {
                    _AddModuleCommand = new RelayCommand(AddNewModuleCommand_Executed, AddNewModuleCommand_CanExecute);
                }
                return _AddModuleCommand;
            }
        }


        RelayCommand _RemoveModuleCommand;
        public ICommand RemoveModuleCommand
        {
            get
            {
                if (_RemoveModuleCommand == null)
                {
                    _RemoveModuleCommand = new RelayCommand(RemoveModuleCommand_Executed, RemoveModuleCommand_CanExecute);
                }
                return _RemoveModuleCommand;
            }
        }

        RelayCommand _AddRemoveFaseCommand;
        public ICommand AddRemoveFaseCommand
        {
            get
            {
                if (_AddRemoveFaseCommand == null)
                {
                    _AddRemoveFaseCommand = new RelayCommand(AddRemoveFaseCommand_Executed, AddRemoveFaseCommand_CanExecute);
                }
                return _AddRemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewModuleCommand_Executed(object prm)
        {
            ModuleModel mm = new ModuleModel();
            mm.Naam = (Modules.Count + 1).ToString();
            ModuleViewModel mvm = new ModuleViewModel(_ControllerVM, mm);
            Modules.Add(mvm);
        }

        bool AddNewModuleCommand_CanExecute(object prm)
        {
            return Modules != null;
        }

        void RemoveModuleCommand_Executed(object prm)
        {
            Modules.Remove(SelectedModule);
        }

        bool RemoveModuleCommand_CanExecute(object prm)
        {
            return SelectedModule != null;
        }

        void AddRemoveFaseCommand_Executed(object prm)
        {
            FaseCyclusModuleViewModel fcmvm = prm as FaseCyclusModuleViewModel;
            SelectedFaseCyclus = fcmvm;
            if (fcmvm.CanBeAddedToModule && !fcmvm.IsInModule)
            {
                SelectedModule.Fasen.Add(fcmvm);
            }
            else if(fcmvm.IsInModule)
            {
                // Use custom method instead of Remove method:
                // it removes based on Define instead of reference
                SelectedModule.RemoveFase(fcmvm);
            }
            foreach(FaseCyclusModuleViewModel _fcmvm in Fasen)
            {
                _fcmvm.UpdateModuleInfo();
            }
        }

        bool AddRemoveFaseCommand_CanExecute(object prm)
        {
            return SelectedModule != null;
        }

        #endregion // Command functionality

        #region Public Methods

        #endregion // Public Methods

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    Fasen.Add(new FaseCyclusModuleViewModel(fcvm, null));
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    FaseCyclusModuleViewModel _fcmvm = null;
                    foreach (FaseCyclusModuleViewModel fcmvm in Fasen)
                    {
                        if (fcmvm.Define == fcvm.Define)
                        {
                            _fcmvm = fcmvm;
                            break;
                        }
                    }
                    if (_fcmvm != null)
                        Fasen.Remove(_fcmvm);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public ModulesTabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
            foreach(FaseCyclusViewModel fcvm in controllervm.Fasen)
            {
                Fasen.Add(new FaseCyclusModuleViewModel(fcvm, null));
            }
            _ControllerVM.Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
