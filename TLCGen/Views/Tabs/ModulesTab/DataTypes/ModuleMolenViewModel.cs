using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModuleMolenViewModel : ViewModelBase
    {
        #region Fields

        private ControllerModel _Controller;
        private ModulesDetailsTabViewModel _ModulesTabVM;
        private ModuleMolenModel _ModuleMolen;
        private ObservableCollection<ModuleViewModel> _Modules;
        private ModuleViewModel _SelectedModule;
        private ModuleFaseCyclusViewModel _SelectedModuleFase;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if (_Controller != null)
                {
                    _ModuleMolen = _Controller.ModuleMolen;
                    Modules.CollectionChanged -= Modules_CollectionChanged;
                    Modules.Clear();
                    foreach (ModuleModel mm in _ModuleMolen.Modules)
                    {
                        ModuleViewModel mvm = new ModuleViewModel(mm);
                        Modules.Add(mvm);
                    }
                    Modules.CollectionChanged += Modules_CollectionChanged;
                }
                else
                {
                    _ModuleMolen = null;
                    Modules.CollectionChanged -= Modules_CollectionChanged;
                    Modules.Clear();
                }
            }
        }

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
                if (_ModulesTabVM != null)
                {
                    if (value != null)
                    {
                        SelectedModuleFase = null;
                        _ModulesTabVM.SetSelectedModuleFase(null);
                    }
                    _ModulesTabVM.SetSelectedModule(value);
                }

                RaisePropertyChanged("SelectedModule");
            }
        }

        public ModuleFaseCyclusViewModel SelectedModuleFase
        {
            get { return _SelectedModuleFase; }
            set
            {
                _SelectedModuleFase = null;
                RaisePropertyChanged("SelectedModuleFase");
                _SelectedModuleFase = value;
                if (_ModulesTabVM != null)
                {
                    if (value != null)
                    {
                        _ModulesTabVM.SetSelectedModule(Modules.First(x => x.Fasen.Contains(value)));
                        SelectedModule = null;
                    }
                    _ModulesTabVM.SetSelectedModuleFase(value);
                }
            }
        }

        public bool LangstWachtendeAlternatief
        {
            get { return _ModuleMolen == null ? false : _ModuleMolen.LangstWachtendeAlternatief; }
            set
            {
                if (_ModuleMolen != null)
                {
                    _ModuleMolen.LangstWachtendeAlternatief = value;
                    RaisePropertyChanged<object>("LangstWachtendeAlternatief", broadcast: true);
                    RaisePropertyChanged("NotLangstWachtendeAlternatief");
                    MessengerInstance.Send(new UpdateTabsEnabledMessage());
                }
            }
        }

        public bool NotLangstWachtendeAlternatief
        {
            get => _ModuleMolen == null ? false : !_ModuleMolen.LangstWachtendeAlternatief;
        }

        public string WachtModule
        {
            get { return _ModuleMolen?.WachtModule; }
            set
            {
                if (_ModuleMolen != null)
                {
                    _ModuleMolen.WachtModule = value;
                    RaisePropertyChanged<object>(nameof(WachtModule), broadcast: true);
                }
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
            ModuleViewModel mvm = new ModuleViewModel(mm);
            Modules.Add(mvm);
            SelectedModule = mvm;
            MessengerInstance.Send(new ModulesChangedMessage());
        }

        bool AddNewModuleCommand_CanExecute(object prm)
        {
            return Modules != null;
        }

        void RemoveModuleCommand_Executed(object prm)
        {
            int index = Modules.IndexOf(SelectedModule);
            Modules.Remove(SelectedModule);
            if(Modules.Count > 0)
            {
                if(index >= Modules.Count)
                {
                    SelectedModule = Modules[Modules.Count - 1];
                }
                else
                {
                    SelectedModule = Modules[index];
                }
            }
            MessengerInstance.Send(new ModulesChangedMessage());
        }

        bool ChangeModuleCommand_CanExecute(object prm)
        {
            return SelectedModule != null;
        }

        #endregion // Command functionality

        #region Private Methods

        private void ReloadModules()
        {
            Modules.CollectionChanged -= Modules_CollectionChanged;
            Modules.Clear();
            foreach (ModuleModel mm in _ModuleMolen.Modules)
            {
                ModuleViewModel mvm = new ModuleViewModel(mm);
                Modules.Add(mvm);
            }
            Modules.CollectionChanged += Modules_CollectionChanged;
        }

        #endregion // Private Methods

        #region TLCGen Events

        private void OnFasenChanged(FasenChangedMessage message)
        {
            ReloadModules();
        }

        private void OnConflictsChanged(ConflictsChangedMessage message)
        {
            ReloadModules();
        }

        #endregion // TLCGen Events

        #region Collection Changed

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
                if (Modules.Any() &&
                    (Modules.All(x => x.Naam != WachtModule) || string.IsNullOrWhiteSpace(WachtModule)))
                {
                    WachtModule = Modules[0].Naam;
                }
                RaisePropertyChanged(nameof(WachtModule));
                Messenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public ModuleMolenViewModel(ModulesDetailsTabViewModel mltab)
        {
            _ModulesTabVM = mltab;
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<ConflictsChangedMessage>(OnConflictsChanged));
        }

        #endregion // Constructor
    }
}
