using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ModulesTabFasenLijstViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ModulesTabViewModel _ModulesTabVM;
        private FaseCyclusModuleViewModel _SelectedFaseCyclus;
        private ModuleViewModel _SelectedModule;
        private ObservableCollection<FaseCyclusModuleViewModel> _Fasen;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusModuleViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
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
                foreach (FaseCyclusModuleViewModel fcmvm in Fasen)
                {
                    fcmvm.ModuleVM = value;
                    fcmvm.UpdateModuleInfo();
                }
                OnPropertyChanged("SelectedModule");
            }
        }

        #endregion // Properties

        #region Commands

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

        void AddRemoveFaseCommand_Executed(object prm)
        {
            FaseCyclusModuleViewModel fcmvm = prm as FaseCyclusModuleViewModel;
            SelectedFaseCyclus = fcmvm;
            if (fcmvm.CanBeAddedToModule && !fcmvm.IsInModule)
            {
                ModuleFaseCyclusModel mfcm = new ModuleFaseCyclusModel();
                mfcm.FaseCyclus = fcmvm.Define;
                ModuleFaseCyclusViewModel mfcvm = new ModuleFaseCyclusViewModel(_ControllerVM, mfcm);
                mfcvm.FaseCyclusNaam = fcmvm.Naam;
                SelectedModule.Fasen.Add(mfcvm);
            }
            else if (fcmvm.IsInModule)
            {
                // Use custom method instead of Remove method:
                // it removes based on Define instead of reference
                SelectedModule.RemoveFase(fcmvm.Define);
            }
            foreach (FaseCyclusModuleViewModel _fcmvm in Fasen)
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
            if (e.NewItems != null && e.NewItems.Count > 0 || e.OldItems != null && e.OldItems.Count > 0)
            {
                Fasen.Clear();
                foreach (FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
                {
                    Fasen.Add(new FaseCyclusModuleViewModel(fcvm, null));
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public ModulesTabFasenLijstViewModel(ControllerViewModel controllervm, ModulesTabViewModel modulestabvm)
        {
            _ControllerVM = controllervm;
            _ModulesTabVM = modulestabvm;
            
            foreach (FaseCyclusViewModel fcvm in controllervm.Fasen)
            {
                Fasen.Add(new FaseCyclusModuleViewModel(fcvm, null));
            }
            _ControllerVM.Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
