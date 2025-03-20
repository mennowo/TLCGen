
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;


namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel meant for displaying a list of phases in the Modules tab.
    /// Also handles clicks on the list, adding and removing phases from modules accordingly.
    /// </summary>
    public class ModulesTabFasenLijstViewModel : ObservableObject
    {
        #region Fields
        
        private FaseCyclusModuleViewModel _SelectedFaseCyclus;
        private ModuleViewModel _SelectedModule;
        private ModuleFaseCyclusViewModel _SelectedModuleFase;
        private ObservableCollection<FaseCyclusModuleViewModel> _Fasen;
        private ControllerModel _Controller;

        #endregion // Fields

        #region Properties

        public ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                Fasen.Clear();
                if (_Controller != null)
                {
                    foreach (var fcm in Controller.Fasen)
                    {
                        Fasen.Add(new FaseCyclusModuleViewModel(fcm, null));
                    }
                }
            }
        }

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
            get => _SelectedFaseCyclus;
            set
            {
                _SelectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }
        
        public ModuleViewModel SelectedModule
        {
            get => _SelectedModule;
            set
            {
                _SelectedModule = value;
                _SelectedModuleFase = null;
                foreach (var fcmvm in Fasen)
                {
                    fcmvm.ModuleVM = value;
                    fcmvm.ModuleFaseVM = null;
                    if (!_Controller.Data.MultiModuleReeksen && _Controller.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == fcmvm.Naam)) ||
                        _Controller.MultiModuleMolens.Any(y => y.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == fcmvm.Naam))))
                    {
                        fcmvm.HasModule = true;
                    }
                    else
                    {
                        fcmvm.HasModule = false;
                    }
                    fcmvm.UpdateModuleInfo();
                }
                OnPropertyChanged("SelectedModule");
            }
        }

        public ModuleFaseCyclusViewModel SelectedModuleFase
        {
            get => _SelectedModuleFase;
            set
            {
                _SelectedModuleFase = value;
                _SelectedModule = null;
                foreach (var fcmvm in Fasen)
                {
                    fcmvm.ModuleVM = null;
                    fcmvm.ModuleFaseVM = value;
                    if (!_Controller.Data.MultiModuleReeksen && _Controller.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == fcmvm.Naam)) ||
                        _Controller.MultiModuleMolens.Any(y => y.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == fcmvm.Naam))))
                    {
                        fcmvm.HasModule = true;
                    }
                    else
                    {
                        fcmvm.HasModule = false;
                    }
                    fcmvm.UpdateModuleInfo();
                }
                OnPropertyChanged("SelectedModuleFase");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand<object> _AddRemoveFaseCommand;
        public ICommand AddRemoveFaseCommand
        {
            get
            {
                if (_AddRemoveFaseCommand == null)
                {
                    _AddRemoveFaseCommand = new RelayCommand<object>(AddRemoveFaseCommand_Executed, AddRemoveFaseCommand_CanExecute);
                }
                return _AddRemoveFaseCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddRemoveFaseCommand_Executed(object prm)
        {
            var fcmvm = prm as FaseCyclusModuleViewModel;
            SelectedFaseCyclus = fcmvm;
            if (SelectedModule != null)
            {
                if (fcmvm.CanBeAdded && !fcmvm.IsIn)
                {
                    var mfcm = new ModuleFaseCyclusModel();
                    mfcm.FaseCyclus = fcmvm.Naam;
                    var mfcvm = new ModuleFaseCyclusViewModel(mfcm);
                    SelectedModule.Fasen.Add(mfcvm);
                    SelectedModule.Fasen.BubbleSort();
                }
                else if (fcmvm.IsIn)
                {
                    // Use custom method instead of Remove method:
                    // it removes based on Define instead of reference
                    SelectedModule.RemoveFase(fcmvm.Naam);
                    SelectedModule.Fasen.BubbleSort();
                }
            }
            else if (SelectedModuleFase != null)
            {
                if (fcmvm.CanBeAdded && !fcmvm.IsIn)
                {
                    var afcm = new ModuleFaseCyclusAlternatiefModel();
                    afcm.FaseCyclus = fcmvm.Naam;
                    if (SelectedModuleFase.Alternatieven.Any())
                    {
                        afcm.AlternatieveGroenTijd = SelectedModuleFase.Alternatieven.First().AlternatieveGroenTijd;
                    }
                    SelectedModuleFase.Alternatieven.Add(new ModuleFaseCyclusAlternatiefViewModel(afcm));
                    SelectedModuleFase.Alternatieven.BubbleSort();
                }
                else if (fcmvm.IsIn)
                {
                    // Use custom method instead of Remove method:
                    // it removes based on Define instead of reference
                    var rafcm = SelectedModuleFase.Alternatieven.FirstOrDefault(x => x.FaseCyclus == fcmvm.Naam);
                    if (rafcm != null) SelectedModuleFase.Alternatieven.Remove(rafcm);
                    SelectedModuleFase.Alternatieven.BubbleSort();
                }
            }
            foreach (var _fcmvm in Fasen)
            {
                if (!_Controller.Data.MultiModuleReeksen && _Controller.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == _fcmvm.Naam)) ||
                    _Controller.MultiModuleMolens.Any(y => y.Modules.Any(x => x.Fasen.Any(x2 => x2.FaseCyclus == _fcmvm.Naam))))
                {
                    _fcmvm.HasModule = true;
                }
                else
                {
                    _fcmvm.HasModule = false; 
                }
                _fcmvm.UpdateModuleInfo();
            }
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        bool AddRemoveFaseCommand_CanExecute(object prm)
        {
            return SelectedModule != null || SelectedModuleFase != null;
        }

        #endregion // Command functionality

        #region Public Methods

        #endregion // Public Methods
        
        #region Message handling

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            var sfc = SelectedFaseCyclus;
            Fasen.Clear();
            foreach (var fcm in _Controller.Fasen)
            {
                Fasen.Add(new FaseCyclusModuleViewModel(fcm, null));
            }
            if(sfc != null && Fasen.Count > 0)
            {
                foreach(var fc in Fasen)
                {
                    if(fc.Naam == sfc.Naam)
                    {
                        SelectedFaseCyclus = sfc;
                    }
                }
            }
            else if(Fasen.Count > 0)
            {
                SelectedFaseCyclus = Fasen[0];
            }
            Fasen.BubbleSort();
        }

        private void OnNameChanged(object sender, NameChangedMessage obj)
        {
            Fasen.BubbleSort();
        }

        private void OnFasenSortedChanged(object sender, FasenSortedMessage obj)
        {
            Fasen.BubbleSort();
        }

        #endregion // Message handling

        #region Constructor

        public ModulesTabFasenLijstViewModel()
        {   
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSortedChanged);
        }

        #endregion // Constructor
    }
}
