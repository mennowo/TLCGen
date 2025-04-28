using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;

namespace TLCGen.Helpers
{
    public class WeakReferenceMessengerEx : IMessenger
    {
        private static IMessenger _default;
        public static IMessenger Default => _default ??= WeakReferenceMessenger.Default;

        public static void OverrideDefault(IMessenger messenger)
        {
            _default = messenger;
        }

        public bool IsRegistered<TMessage, TToken>(object recipient, TToken token) where TMessage : class where TToken : IEquatable<TToken>
        {
            return _default.IsRegistered<TMessage, TToken>(recipient, token);
        }

        public void Register<TRecipient, TMessage, TToken>(TRecipient recipient, TToken token, MessageHandler<TRecipient, TMessage> handler) where TRecipient : class where TMessage : class where TToken : IEquatable<TToken>
        {
            _default.Register(recipient, token, handler);
        }

        public void UnregisterAll(object recipient)
        {
            _default.UnregisterAll(recipient);
        }

        public void UnregisterAll<TToken>(object recipient, TToken token) where TToken : IEquatable<TToken>
        {
            _default.UnregisterAll(recipient, token);
        }

        public void Unregister<TMessage, TToken>(object recipient, TToken token) where TMessage : class where TToken : IEquatable<TToken>
        {
            _default.Unregister<TMessage, TToken>(recipient, token);
        }

        public TMessage Send<TMessage, TToken>(TMessage message, TToken token) where TMessage : class where TToken : IEquatable<TToken>
        {
            return _default.Send(message, token);
        }

        public void Cleanup()
        {
            _default.Cleanup();
        }

        public void Reset()
        {
            _default.Reset();
        }
    }

    public class AddRemoveItemsManager<T1, T2, T3> : ObservableObject where T1 : IViewModelWithItem
    {
        private T1 _selectedItem;
        private T3 _selectedItemToAdd;
        private RelayCommand _addItemCommand;
        private RelayCommand _removeItemCommand;

        public ObservableCollectionAroundList<T1, T2> ItemsSource { get; }
        public ObservableCollection<T3> AllSelectableItems { get; } = [];
        public ObservableCollection<T3> SelectableItems { get; } = [];

        public T3 SelectedItemToAdd
        {
            get => _selectedItemToAdd;
            set
            {
                _selectedItemToAdd = value;
                OnPropertyChanged();
                _addItemCommand?.NotifyCanExecuteChanged();
            }
        }

        public T1 SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                _removeItemCommand?.NotifyCanExecuteChanged();
            }
        }

        public Func<T3, T1> GetNewItem { get; }
        
        public Func<T1, T3, bool> SelectableEqualsItem { get; }
        
        public Action OnCollectionChanged { get; }

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
            _addItemCommand?.NotifyCanExecuteChanged();
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
