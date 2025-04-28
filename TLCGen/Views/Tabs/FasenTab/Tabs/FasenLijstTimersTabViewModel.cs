
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    public class TLCGenTabItemWithSelectionViewModel<T> : TLCGenTabItemViewModel where T : ObservableObject
    {
        private T _selectedItem;
        private volatile bool _settingMultiple = false;
        private IList _selectedItems = new ArrayList();

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public IList SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;
                _settingMultiple = false;
                OnPropertyChanged();
            }
        }

        
        #region Event Handling

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_settingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedItems != null && SelectedItems.Count > 1)
            {
                _settingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<FaseCyclusViewModel>(sender, e.PropertyName, SelectedItems);
            }
            _settingMultiple = false;
        }

        #endregion // Event Handling

        public override string DisplayName { get; }

        public void InitCollection(ObservableCollection<T> collection)
        {
            collection.CollectionChanged += CollectionOnCollectionChanged;
            foreach (var i in collection)
            {
                i.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (T item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (T item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }
    }

    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.FasenTab)]
    public class FasenLijstTimersTabViewModel : TLCGenTabItemWithSelectionViewModel<FaseCyclusViewModel>
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ObservableCollection<FaseCyclusViewModel> Fasen => ControllerAccessProvider.Default.AllSignalGroups;

        public bool IsNotInterGreen => !Controller.Data.Intergroen;
        
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

        #region Collection Changed

        #endregion // Collection Changed

        #region Private Methods

        #endregion // Private Methods

        #region Constructor

        public FasenLijstTimersTabViewModel()
        {
            InitCollection(ControllerAccessProvider.Default.AllSignalGroups);
        }

        #endregion // Constructor
    }
}
