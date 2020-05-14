using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTimersTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private FaseCyclusViewModel _selectedFaseCyclus;
        private IList _selectedFaseCycli = new ArrayList();
        private volatile bool _settingMultiple = false;

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen => ControllerAccessProvider.Default.AllSignalGroups;

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get => _selectedFaseCyclus;
	        set
            {
                _selectedFaseCyclus = value;
                RaisePropertyChanged();
            }
        }

        public IList SelectedFaseCycli
        {
            get => _selectedFaseCycli;
	        set
            {
                _selectedFaseCycli = value;
                _settingMultiple = false;
                RaisePropertyChanged();
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Tijden";

	    public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
        }

        #endregion // TabItem Overrides

        #region TLCGen Event handling
        #endregion // TLCGen Event handling

        #region Event Handling

        private void FaseCyclus_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedFaseCycli != null && SelectedFaseCycli.Count > 1)
            {
                _settingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedFaseCycli);
            }
            _settingMultiple = false;
        }

        #endregion // Event Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public FasenLijstTimersTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
