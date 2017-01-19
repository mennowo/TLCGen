using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel meant for displaying a list of phases in the Modules tab.
    /// Also handles clicks on the list, adding and removing phases from modules accordingly.
    /// </summary>
    public class ModulesTabFasenLijstViewModel : ViewModelBase
    {
        #region Fields
        
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
                mfcm.FaseCyclus = fcmvm.Naam;
                ModuleFaseCyclusViewModel mfcvm = new ModuleFaseCyclusViewModel(mfcm);
                SelectedModule.Fasen.Add(mfcvm);
                SelectedModule.Fasen.BubbleSort();
            }
            else if (fcmvm.IsInModule)
            {
                // Use custom method instead of Remove method:
                // it removes based on Define instead of reference
                SelectedModule.RemoveFase(fcmvm.Naam);
                SelectedModule.Fasen.BubbleSort();
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
        
        #region Message handling

        private void OnFasenChanged(FasenChangedMessage message)
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in message.Fasen)
            {
                Fasen.Add(new FaseCyclusModuleViewModel(fcm, null));
            }
        }

        #endregion // Message handling

        #region Constructor

        public ModulesTabFasenLijstViewModel(ModulesTabViewModel modulestabvm)
        {   
            foreach(FaseCyclusModel fcm in modulestabvm.Controller.Fasen)
            {
                Fasen.Add(new FaseCyclusModuleViewModel(fcm, null));
            }

            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
        }

        #endregion // Constructor
    }
}
