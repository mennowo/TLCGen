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
        private RelayCommand _AddModuleCommand;
        private RelayCommand _RemoveModuleCommand;
        private RelayCommand _MoveModuleUpCommand;
        private RelayCommand _MoveModuleDownCommand;

        #endregion // Fields

        #region Properties

        public string Reeks => _ModuleMolen.Reeks;

        public ObservableCollection<ModuleViewModel> Modules { get; } = [];
        
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

                OnPropertyChanged();
                _RemoveModuleCommand?.NotifyCanExecuteChanged();
                _MoveModuleUpCommand?.NotifyCanExecuteChanged();
                _MoveModuleDownCommand?.NotifyCanExecuteChanged();
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
        public ICommand AddModuleCommand => _AddModuleCommand ??= new RelayCommand(() =>
        {
            var mm = new ModuleModel
            {
                Naam = Reeks + (Modules.Count + 1).ToString()
            };
            var mvm = new ModuleViewModel(mm);
            Modules.Add(mvm);
            SelectedModule = mvm;
            WeakReferenceMessengerEx.Default.Send(new ModulesChangedMessage());
        });

        public ICommand RemoveModuleCommand => _RemoveModuleCommand ??= new RelayCommand(() =>
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
            WeakReferenceMessengerEx.Default.Send(new ModulesChangedMessage());
        }, () => SelectedModule != null);

        public ICommand MoveModuleUpCommand => _MoveModuleUpCommand ??= new RelayCommand(() =>
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
        }, () => SelectedModule != null);

        public ICommand MoveModuleDownCommand => _MoveModuleDownCommand ??= new RelayCommand(() =>
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
        }, () => SelectedModule != null);

        #endregion // Commands

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
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public ModuleMolenViewModel(ModulesDetailsTabViewModel mltab, ModuleMolenModel moduleMolen)
        {
            _ModuleMolen = moduleMolen;
            ReloadModules();
            _ModulesTabVM = mltab;
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<ConflictsChangedMessage>(this, OnConflictsChanged);
        }

        #endregion // Constructor
    }
}
