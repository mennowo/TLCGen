using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleMolenViewModel : ViewModelBase
    {
        private ControllerModel _Controller;
        private ModulesTabViewModel _ModulesTabVM;
        private ModuleMolenModel _ModuleMolen;
        private ObservableCollection<ModuleViewModel> _Modules;
        private ModuleViewModel _SelectedModule;
        private ModuleFaseCyclusViewModel _SelectedModuleFase;

        public ObservableCollection<ModuleViewModel> Modules
        {
            get
            {
                if (_Modules == null)
                {
                    _Modules = new ObservableCollection<ModuleViewModel>();
                }
                return _Modules;
            }
        }
        public ModuleViewModel SelectedModule
        {
            get { return _SelectedModule; }
            set
            {
                _SelectedModule = value;
                _ModulesTabVM.SetSelectedModule(value);
                if(value != null)
                    SelectedModuleFase = null;
                OnPropertyChanged("SelectedModule");
            }
        }

        public ModuleFaseCyclusViewModel SelectedModuleFase
        {
            get { return _SelectedModuleFase; }
            set
            {
                _SelectedModuleFase = value;
                if(value != null)
                    SelectedModule = null;
                OnPropertyChanged("SelectedModuleFase");
            }
        }

        public bool LangstWachtendeAlternatief
        {
            get { return _ModuleMolen.LangstWachtendeAlternatief; }
            set
            {
                _ModuleMolen.LangstWachtendeAlternatief = value;
                OnMonitoredPropertyChanged("LangstWachtendeAlternatief");
            }
        }

        public string WachtModule
        {
            get { return _ModuleMolen.WachtModule; }
            set
            {
                _ModuleMolen.WachtModule = value;
                OnMonitoredPropertyChanged("WachtModule");
            }
        }

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
                    _RemoveModuleCommand = new RelayCommand(RemoveModuleCommand_Executed, ChangeModuleCommand_CanExecute);
                }
                return _RemoveModuleCommand;
            }
        }

        RelayCommand _MoveModuleUpCommand;
        public ICommand MoveModuleUpCommand
        {
            get
            {
                if (_MoveModuleUpCommand == null)
                {
                    _MoveModuleUpCommand = new RelayCommand(MoveModuleUpCommand_Executed, ChangeModuleCommand_CanExecute);
                }
                return _MoveModuleUpCommand;
            }
        }

        RelayCommand _MoveModuleDownCommand;
        public ICommand MoveModuleDownCommand
        {
            get
            {
                if (_MoveModuleDownCommand == null)
                {
                    _MoveModuleDownCommand = new RelayCommand(MoveModuleDownCommand_Executed, ChangeModuleCommand_CanExecute);
                }
                return _MoveModuleDownCommand;
            }
        }
        #endregion // Commands

        #region Command functionality

        private void MoveModuleUpCommand_Executed(object obj)
        {
            int index = -1;
            foreach(ModuleViewModel mvm in Modules)
            {
                ++index;
                if(mvm == SelectedModule)
                {
                    break;
                }
            }
            if(index >= 1)
            {
                ModuleViewModel mvm = SelectedModule;
                SelectedModule = null;
                Modules.Remove(mvm);
                Modules.Insert(index - 1, mvm);
                SelectedModule = mvm;
            }
        }


        private void MoveModuleDownCommand_Executed(object obj)
        {
            int index = -1;
            foreach (ModuleViewModel mvm in Modules)
            {
                ++index;
                if (mvm == SelectedModule)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (Modules.Count - 2)))
            {
                ModuleViewModel mvm = SelectedModule;
                SelectedModule = null;
                Modules.Remove(mvm);
                Modules.Insert(index + 1, mvm);
                SelectedModule = mvm;
            }
        }

        void AddNewModuleCommand_Executed(object prm)
        {
            ModuleModel mm = new ModuleModel();
            mm.Naam = "ML" + (Modules.Count + 1).ToString();
            ModuleViewModel mvm = new ModuleViewModel(_Controller, mm);
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

        bool ChangeModuleCommand_CanExecute(object prm)
        {
            return SelectedModule != null;
        }

        #endregion // Command functionality

        private void Modules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0 ||
                e.OldItems != null && e.OldItems.Count > 0)
            {
                // Rebuild the mill in the model: needed to allow reordering
                _ModuleMolen.Modules.Clear();
                foreach (ModuleViewModel mvm in Modules)
                {
                    _ModuleMolen.Modules.Add(mvm.Module);
                }

                // Update Module names
                int i = 1;
                foreach (ModuleViewModel mvm in Modules)
                {
                    mvm.Naam = "ML" + i.ToString();
                    ++i;
                }
                // Set WachtModule if needed
                if (Modules.Count == 1)
                {
                    WachtModule = Modules[0].Naam;
                }
                Messenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        public ModuleMolenViewModel(ControllerModel controller, ModulesTabViewModel modulestabvm)
        {
            _Controller = controller;
            _ModulesTabVM = modulestabvm;
            _ModuleMolen = _Controller.ModuleMolen;

            foreach (ModuleModel mm in _ModuleMolen.Modules)
            {
                ModuleViewModel mvm = new ModuleViewModel(_Controller, mm);
                Modules.Add(mvm);
            }

            Modules.CollectionChanged += Modules_CollectionChanged;
        }
    }
}
