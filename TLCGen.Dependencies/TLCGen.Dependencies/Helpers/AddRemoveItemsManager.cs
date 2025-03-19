using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Extensions;

namespace TLCGen.Helpers
{
    public class AddRemoveItemsManager<T1, T2, T3> : ObservableObject where T1 : IViewModelWithItem
    {
        public ObservableCollectionAroundList<T1, T2> ItemsSource { get; }
        public ObservableCollection<T3> AllSelectableItems { get; } = new();
        public ObservableCollection<T3> SelectableItems { get; } = new();
        
        private T3 _selectedItemToAdd;
        public T3 SelectedItemToAdd
        {
            get => _selectedItemToAdd;
            set
            {
                _selectedItemToAdd = value;
                OnPropertyChanged();
            }
        }

        private T1 _selectedItem;
        public T1 SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public Func<T3, T1> GetNewItem { get; }
        public Func<T1, T3, bool> SelectableEqualsItem { get; }
        public Action OnCollectionChanged { get; }

        RelayCommand _addItemCommand;
        public ICommand AddItemCommand => _addItemCommand ??= new RelayCommand(AddItemCommand_executed, AddItemCommand_canExecute);

        private bool AddItemCommand_canExecute()
        {
            return SelectedItemToAdd != null || !AllSelectableItems.Any();
        }

        private void AddItemCommand_executed()
        {
            var i = SelectableItems.IndexOf(SelectedItemToAdd);
            ItemsSource.Add(GetNewItem(SelectedItemToAdd));
            ItemsSource.BubbleSort();
            OnCollectionChanged?.Invoke();
            UpdateSelectables(null);
            if (SelectableItems.Any())
            {
                if (i >= 0 && SelectableItems.Count > i) SelectedItemToAdd = SelectableItems[i];
                else SelectedItemToAdd = SelectableItems.Last();
            }
            else
            {
                SelectedItemToAdd = default(T3);
            }
        }

        RelayCommand _removeItemCommand;
        public ICommand RemoveItemCommand => _removeItemCommand ??= new RelayCommand(RemoveItemCommand_executed, RemoveItemCommand_canExecute);

        private bool RemoveItemCommand_canExecute()
        {
            return SelectedItem != null;
        }

        private void RemoveItemCommand_executed()
        {
            var iSelectedItem = ItemsSource.IndexOf(SelectedItem);
            var removedSelToAddItem = AllSelectableItems.FirstOrDefault(x => SelectableEqualsItem(SelectedItem, x));
            var iSelectedToAdd = SelectableItems.IndexOf(SelectedItemToAdd);
            ItemsSource.Remove(SelectedItem);
            OnCollectionChanged?.Invoke();
            if (ItemsSource.Count > 0)
            {
                if (iSelectedItem >= 0 && ItemsSource.Count > iSelectedItem) SelectedItem = ItemsSource[iSelectedItem];
                else SelectedItem = ItemsSource.Last();
            }
            else
            {
                SelectedItem = default(T1);
            }
            UpdateSelectables(null);
            if (SelectableItems.Any())
            {
                if(removedSelToAddItem != null && SelectableItems.Any(x => x.Equals(removedSelToAddItem)))
                {
                    SelectedItemToAdd = removedSelToAddItem;
                }
                else
                {
                    if (iSelectedToAdd >= 0 && iSelectedToAdd < SelectableItems.Count) SelectedItemToAdd = SelectableItems[iSelectedToAdd];
                    else SelectedItemToAdd = SelectableItems.Last();
                }
            }
            else
            {
                SelectedItemToAdd = default(T3);
            }
        }

        public void UpdateSelectables(IEnumerable<T3> allItems)
        {
            if (allItems != null)
            {
                AllSelectableItems.Clear();
                foreach (var i in allItems)
                {
                    AllSelectableItems.Add(i);
                }
            }
            SelectableItems.Clear();
            foreach (var i in AllSelectableItems)
            {
                if (!ItemsSource.Any(x => SelectableEqualsItem(x, i)))
                {
                    SelectableItems.Add(i);
                }
            }
            if (SelectableItems.Any()) SelectedItemToAdd = SelectableItems[0];
        }

        public AddRemoveItemsManager(ObservableCollectionAroundList<T1, T2> itemsSource, Func<T3, T1> getNewItem, Func<T1, T3, bool> selectableEqualsItem, Action onCollectionChanged = null)
        {
            ItemsSource = itemsSource;
            GetNewItem = getNewItem;
            SelectableEqualsItem = selectableEqualsItem;
            OnCollectionChanged = onCollectionChanged;
        }
    }
}
