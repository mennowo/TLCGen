﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesDetailsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ModuleMolenViewModel _ModuleMolenVM;
        private ModulesTabFasenLijstViewModel _FasenLijstVM;
        private string _selectedModuleReeks;

        #endregion // Fields

        #region Properties

        public ModuleMolenViewModel ModuleMolenVM
        {
            get => _ModuleMolenVM;
            set
            {
                _ModuleMolenVM = value;
                OnPropertyChanged();
            }
        }

        public ModulesTabFasenLijstViewModel FasenLijstVM => _FasenLijstVM;

        public string SelectedModuleReeks
        {
            get => _selectedModuleReeks;
            set
            {
                if (value == null || _Controller == null) return;
                _selectedModuleReeks = value;             
                if (value != "ML" && !_Controller.MultiModuleMolens.Any(x => x.Reeks == value))
                {
                    _Controller.MultiModuleMolens.Add(new ModuleMolenModel() { Reeks = value });
                }
                if(value == "ML")
                {
                    ModuleMolenVM = new ModuleMolenViewModel(this, _Controller.ModuleMolen);
                }
                else
                {
                    ModuleMolenVM = new ModuleMolenViewModel(this, _Controller.MultiModuleMolens.FirstOrDefault(x => x.Reeks == value));
                }
                ModuleMolenVM.SelectedModule = ModuleMolenVM.Modules.FirstOrDefault();
                if (ModuleMolenVM.Modules.Count > 0)
                {
                    ModuleMolenVM.SelectedModule = ModuleMolenVM.Modules[0];
                    FasenLijstVM.SelectedModule = ModuleMolenVM.Modules[0];
                }
                OnPropertyChanged();
            }
        }

        public List<string> ModuleReeks { get; } = new List<string>
        {
            "MLA",
            "MLB",
            "MLC",
            "MLD",
            "MLE"
        };

        public bool ModulenInParameters
        {
            get => _Controller?.Data == null ? false : _Controller.Data.ModulenInParameters;
            set
            {
                if (_Controller?.Data != null)
                {
                    _Controller.Data.ModulenInParameters = value;
                    OnPropertyChanged(nameof(ModulenInParameters), broadcast: true);
                }
            }
        }

        public bool LangstWachtendeAlternatief
        {
            get => _Controller?.ModuleMolen == null ? false : _Controller.ModuleMolen.LangstWachtendeAlternatief;
            set
            {
                if (_Controller?.ModuleMolen != null)
                {
                    _Controller.ModuleMolen.LangstWachtendeAlternatief = value;
                    OnPropertyChanged(nameof(LangstWachtendeAlternatief), broadcast: true);
                    WeakReferenceMessengerEx.Default.Send(new UpdateTabsEnabledMessage());
                    _ModuleMolenVM.LangstWachtendeAlternatief = value;
                }
            }
        }

        public Visibility HasMultiML => _Controller.Data.MultiModuleReeksen ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Modules";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            OnPropertyChanged(nameof(HasMultiML));
            if (_Controller.Data.MultiModuleReeksen && SelectedModuleReeks == "ML")
            {
                SelectedModuleReeks = "MLA";
            }
            else if (!_Controller.Data.MultiModuleReeksen && SelectedModuleReeks != "ML")
            {
                SelectedModuleReeks = "ML";
            }
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                FasenLijstVM.Controller = value;
                if (value != null)
                {
                    if (value.Data.MultiModuleReeksen)
                    {
                        SelectedModuleReeks = "MLA";
                    }
                    else
                    {
                        SelectedModuleReeks = "ML";
                    }
                }
                if (FasenLijstVM.Fasen.Count > 0)
                {
                    FasenLijstVM.SelectedFaseCyclus = FasenLijstVM.Fasen[0];
                }
            }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        public void SetSelectedModule(ModuleViewModel mvm)
        {
            _FasenLijstVM.SelectedModule = mvm;
        }

        public void SetSelectedModuleFase(ModuleFaseCyclusViewModel mvm)
        {
            _FasenLijstVM.SelectedModuleFase = mvm;
        }

        #endregion // Public Methods

        #region TLCGen Message Handling

        #endregion // TLCGen Message Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesDetailsTabViewModel() : base()
        {
            _FasenLijstVM = new ModulesTabFasenLijstViewModel();
            SelectedModuleReeks = "ML";
        }

        #endregion // Constructor
    }
}
