using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace TLCGen.ViewModels
{
    public class ItemsManagerViewModel<T1, T2> : ViewModelBase where T1 : class where T2 : class
    {
        #region Fields

        private Func<T2, T1> _getItemToAdd;
        private Func<T2, T1> _getItemToRemove;
        private Action _afterItemAddedAction;
        private Action _afterItemRemovedAction;
        private Predicate<T2> _isItemSelectable;
        private T1 _selectedItem;
        private T2 _selectedItemToRemove;
        private T2 _selectedItemToAdd;
        private RelayCommand _addItemCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<T1> ItemsInCollection { get; }
        public IEnumerable<T2> AllAvailableItems { get; private set; }
        public ObservableCollection<T2> SelectableItems { get; }
        public ObservableCollection<T2> RemovableItems { get; }

        public T2 SelectedItemToAdd
        {
            get => _selectedItemToAdd;
	        set
            {
                _selectedItemToAdd = value;
                RaisePropertyChanged();
            }
        }
        public T2 SelectedItemToRemove
        {
            get => _selectedItemToRemove;
	        set
            {
                _selectedItemToRemove = value;
                RaisePropertyChanged();
            }
        }

        public T1 SelectedItem
        {
            get => _selectedItem;
	        set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }


        #endregion // Properties

        #region Commands

        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                {
                    _addItemCommand = new RelayCommand(AddItemCommand_Executed, AddItemCommand_CanExecute);
                }
                return _addItemCommand;
            }
        }

        private RelayCommand _removeItemCommand;
        public ICommand RemoveItemCommand
        {
            get
            {
                if (_removeItemCommand == null)
                {
                    _removeItemCommand = new RelayCommand(RemoveItemCommand_Executed, RemoveItemCommand_CanExecute);
                }
                return _removeItemCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddItemCommand_CanExecute()
        {
            return SelectedItemToAdd != null;
        }

        private void AddItemCommand_Executed()
        {
            var d = _getItemToAdd(SelectedItemToAdd);
            ItemsInCollection.Add(d);
            SelectedItem = d;
            Refresh();
            _afterItemAddedAction?.Invoke();
        }

        private bool RemoveItemCommand_CanExecute()
        {
            return SelectedItemToRemove != null || _selectedItem != null;
        }

        private void RemoveItemCommand_Executed()
        {
            if(_selectedItem != null)
            {
                var i = ItemsInCollection.IndexOf(_selectedItem);
                ItemsInCollection.Remove(_selectedItem);
                if(i < (ItemsInCollection.Count - 1))
                {
                    SelectedItem = ItemsInCollection[i];
                }
                else if (ItemsInCollection.Count > 0)
                {
                    SelectedItem = ItemsInCollection[ItemsInCollection.Count - 1];
                }
                else
                {
                    SelectedItem = null;
                }
            }
            else if (SelectedItemToRemove != null && _getItemToRemove != null)
            {
                var d = _getItemToRemove(SelectedItemToRemove);
                ItemsInCollection.Remove(d);
            }
            Refresh();
            _afterItemRemovedAction?.Invoke();
        }

        #endregion // Command functionality
        
        #region Public Methods

        public void Refresh()
        {
            var sdta = SelectedItemToAdd;
            SelectedItemToAdd = null;
            var sdtr = SelectedItemToRemove;
            SelectedItemToRemove = null;

            SelectableItems.Clear();
            RemovableItems.Clear();
            foreach (var d in AllAvailableItems)
            {
                if (_isItemSelectable(d))
                {
                    SelectableItems.Add(d);
                }
                else
                {
                    RemovableItems.Add(d);
                }
            }

            if(sdta != null && SelectableItems.Contains(sdta))
                SelectedItemToAdd = sdta;
            else if (SelectableItems.Count > 0)
                SelectedItemToAdd = SelectableItems[0];

            if (sdtr != null && RemovableItems.Contains(sdtr))
                SelectedItemToRemove = sdtr;
            else if (RemovableItems.Count > 0)
                SelectedItemToRemove = RemovableItems[0];

            if(SelectedItem == null && ItemsInCollection.Any())
            {
                SelectedItem = ItemsInCollection[0];
            }
        }

        public void UpdateAvailableItems(IEnumerable<T2> allAvailableItems)
        {
            AllAvailableItems = allAvailableItems;
            Refresh();
        }

        #endregion // Public Methods

        #region Constructor

        /// <summary>
        /// Wrapper class to support easy management of a list of specific items
        /// belonging to an object. This situation occurs more often, which is why this
        /// class automizes the process of adding and removing items to a given list.
        /// The class is meant to be coupled with an instance of ItemsManagerView.
        /// The class allows for multiple scenarios.
        /// T1 = the type of class of items in the list to be managed
        /// T2 = the type of class of items in list(s) to be displayed to the user, to
        /// allow selection of items to add (and remove if needed).
        /// </summary>
        /// <param name="items">The actual list (T1) that is to be managed.</param>
        /// <param name="allAvaiableItems">A list (T2) holding all items that could be added.</param>
        /// <param name="getItemsToAddFunc">A function that converts T2 to T1 and supplies a new instance of T1.</param>
        /// <param name="isItemSelectablePredicate">Predicate that indicates if an instance of T2 is selectable.
        /// It is presumed that if it is not selectable, it is in the managed list.</param>
        /// <param name="getItemtoRemoveFunc">Meant to be used instead of the property SelectedItem: function to get the detector 
        /// to remove, based on the selected item T2 from the RemovableItems list</param>
        /// <param name="afterItemAddedAction">Things to do after adding an item</param>
        /// <param name="afterItemRemovedAction">Things to do after removing an item</param>
        public ItemsManagerViewModel(
            ObservableCollection<T1> items,
            IEnumerable<T2> allAvaiableItems,
            Func<T2, T1> getItemsToAddFunc, 
            Predicate<T2> isItemSelectablePredicate,
            Func<T2, T1> getItemtoRemoveFunc = null,
            Action afterItemAddedAction = null, 
            Action afterItemRemovedAction = null)
        {
            ItemsInCollection = items;
            AllAvailableItems = allAvaiableItems;
            _getItemToAdd = getItemsToAddFunc;
            _getItemToRemove = getItemtoRemoveFunc;
            _isItemSelectable = isItemSelectablePredicate;
            _afterItemAddedAction = afterItemAddedAction;
            _afterItemRemovedAction = afterItemRemovedAction;

            SelectableItems = new ObservableCollection<T2>();
            RemovableItems = new ObservableCollection<T2>();

            Refresh();
        }

        #endregion // Constructor
    }
}
