using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace TLCGen.ViewModels
{
    public class ModuleMolenViewModel : ObservableObjectEx
    {
        #region Fields

        private ModulesDetailsTabViewModel _ModulesTabVM;
        private ModuleMolenModel _ModuleMolen;
        private ObservableCollection<ModuleViewModel> _Modules;
        private ModuleViewModel _SelectedModule;
        private ModuleFaseCyclusViewModel _SelectedModuleFase;
        private volatile bool _reloading;

        #endregion // Fields

        #region Properties

        public string Reeks => _ModuleMolen.Reeks;

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
            get => _SelectedModule;
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

                OnPropertyChanged("SelectedModule");
            }
        }

        public ModuleFaseCyclusViewModel SelectedModuleFase
        {
            get => _SelectedModuleFase;
            set
            {
                _SelectedModuleFase = null;
                OnPropertyChanged("SelectedModuleFase");
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
            get => _ModuleMolen == null ? false : _ModuleMolen.LangstWachtendeAlternatief;
            set
            {
                if (_ModuleMolen != null)
                {
                    _ModuleMolen.LangstWachtendeAlternatief = value;
                    OnPropertyChanged(nameof(LangstWachtendeAlternatief), broadcast: true);
                    OnPropertyChanged("NotLangstWachtendeAlternatief");
                }
            }
        }

        public bool NotLangstWachtendeAlternatief => _ModuleMolen == null ? false : !_ModuleMolen.LangstWachtendeAlternatief;

        public string WachtModule
        {
            get => _ModuleMolen?.WachtModule;
            set
            {
                if (_reloading) return;
                if (_ModuleMolen != null && value != null)
                {
                    _ModuleMolen.WachtModule = value;
                    OnPropertyChanged(nameof(WachtModule), broadcast: true);
                }
                else if (!_Modules.Any())
                {
                    _ModuleMolen.WachtModule = null;
                    OnPropertyChanged(nameof(WachtModule), broadcast: true);
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

        private void MoveModuleUpCommand_Executed()
        {
            var index = -1;
            foreach(var mvm in Modules)
            {
                ++index;
                if(mvm == SelectedModule)
                {
                    break;
                }
            }
            if(index >= 1)
            {
                var mvm = SelectedModule;
                SelectedModule = null;
                Modules.Remove(mvm);
                Modules.Insert(index - 1, mvm);
                SelectedModule = mvm;
            }
        }


        private void MoveModuleDownCommand_Executed()
        {
            var index = -1;
            foreach (var mvm in Modules)
            {
                ++index;
                if (mvm == SelectedModule)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (Modules.Count - 2)))
            {
                var mvm = SelectedModule;
                SelectedModule = null;
                Modules.Remove(mvm);
                Modules.Insert(index + 1, mvm);
                SelectedModule = mvm;
            }
        }

        void AddNewModuleCommand_Executed()
        {
            var mm = new ModuleModel
            {
                Naam = Reeks + (Modules.Count + 1).ToString()
            };
            var mvm = new ModuleViewModel(mm);
            Modules.Add(mvm);
            SelectedModule = mvm;
            WeakReferenceMessenger.Default.Send(new ModulesChangedMessage());
        }

        bool AddNewModuleCommand_CanExecute()
        {
            return Modules != null;
        }

        void RemoveModuleCommand_Executed()
        {
            var index = Modules.IndexOf(SelectedModule);
            Modules.Remove(SelectedModule);
            SelectedModule = null;
            if (Modules.Count > 0)
            {
                if (index >= Modules.Count)
                {
                    SelectedModule = Modules[Modules.Count - 1];
                }
                else
                {
                    SelectedModule = Modules[index];
                }
            }
            WeakReferenceMessenger.Default.Send(new ModulesChangedMessage());
        }

        bool ChangeModuleCommand_CanExecute()
        {
            return SelectedModule != null;
        }

        #endregion // Command functionality

        #region Private Methods

        private void ReloadModules()
        {
            _reloading = true;
            Modules.CollectionChanged -= Modules_CollectionChanged;
            Modules.Clear();
            foreach (var mm in _ModuleMolen.Modules)
            {
                var mvm = new ModuleViewModel(mm);
                Modules.Add(mvm);
            }
            Modules.CollectionChanged += Modules_CollectionChanged;
            _reloading = false;
        }

        #endregion // Private Methods

        #region TLCGen Events

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            ReloadModules();
        }

        private void OnConflictsChanged(object sender, ConflictsChangedMessage message)
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
                foreach (var mvm in Modules)
                {
                    _ModuleMolen.Modules.Add(mvm.Module);
                }

                // Update Module names
                var i = 1;
                foreach (var mvm in Modules)
                {
                    mvm.Naam = Reeks + i.ToString();
                    ++i;
                }
                // Set WachtModule if needed
                if (Modules.Any() &&
                    (Modules.All(x => x.Naam != WachtModule) || string.IsNullOrWhiteSpace(WachtModule)))
                {
                    WachtModule = Modules[0].Naam;
                }
                OnPropertyChanged(nameof(WachtModule));
WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public ModuleMolenViewModel(ModulesDetailsTabViewModel mltab, ModuleMolenModel moduleMolen)
        {
            _ModuleMolen = moduleMolen;
            ReloadModules();
            _ModulesTabVM = mltab;
            WeakReferenceMessenger.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessenger.Default.Register<ConflictsChangedMessage>(this, OnConflictsChanged);
        }

        #endregion // Constructor
    }
}
