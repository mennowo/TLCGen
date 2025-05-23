﻿using System.Collections.ObjectModel;
using System.Linq;
using TLCGen.Models;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.FasenTab)]
    public class FasenDetailsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private FaseCyclusViewModel _selectedFaseCyclus;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen => ControllerAccessProvider.Default.AllSignalGroups;

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
            set
            {
                _selectedFaseCyclus = value;
                OnPropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Details";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            SelectedFaseCyclus ??= Fasen.FirstOrDefault();
        }

        public override ControllerModel Controller
        {
            get => base.Controller; 
            set
            {
                base.Controller = value;
                SelectedFaseCyclus = null;
            }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public FasenDetailsTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
