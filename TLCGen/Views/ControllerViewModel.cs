using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class ControllerViewModel : ObservableObject
    {
        #region Fields
        
        private ControllerModel _Controller;

        private ObservableCollection<ITLCGenTabItem> _TabItems;
        private ITLCGenTabItem _SelectedTab;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Readonly reference to the data model
        /// </summary>
        public ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                foreach (var tab in _TabItems)
                {
                    tab.Controller = value;
                }
                SelectedTab = TabItems?.Count > 0 ? TabItems[0] : null;
                OnPropertyChanged(string.Empty);
            }
        }

        public ObservableCollection<ITLCGenTabItem> TabItems
        {
            get
            {
                if (_TabItems == null)
                {
                    _TabItems = new ObservableCollection<ITLCGenTabItem>();
                }
                return _TabItems;
            }
        }

        /// <summary>
        /// Reference to the selected tab in the Controller View
        /// </summary>
        public ITLCGenTabItem SelectedTab
        {
            get => _SelectedTab;
            set
            {
                // Take actions for current 
                if (_SelectedTab != value && _SelectedTab != null)
                {
                    if(_SelectedTab.OnDeselectedPreview())
                    {
                        _SelectedTab.OnDeselected();
                    }
                    else
                    {
                        return;
                    }
                }

                // Preview new, set if good
                if (value.OnSelectedPreview())
                {
                    _SelectedTab = value;
                    _SelectedTab.OnSelected();
                    OnPropertyChanged("SelectedTab");
                }
            }
        }

        #endregion // Properties

        #region Private methods

        #endregion

        #region Public methods

        /// <summary>
        /// Soft reload of the current controller: updates bound values and updates phases.
        /// </summary>
        public void ReloadController()
        {
            OnPropertyChanged("");
            SelectedTab = TabItems?.Count > 0 ? TabItems[0] : null;
        }

        /// <summary>
        /// Helper method to set the value of multiple elements at the same time. This means the user
        /// can select multiple elements in a DataGrid, edit the last one selected, and the value that
        /// is input will be propagated to all selected elements
        /// </summary>
        private bool IsSetting;
        public void SetAllSelectedElementsValue(object o, string propName)
        {
            if (!IsSetting)
            {
                switch(SelectedTab.DisplayName)
                {
                    case "Fasen":
                        IsSetting = true;
                        break;
                    case "Detectoren":
                        IsSetting = true;
                        break;
                }
                IsSetting = false;
            }
        }

        #endregion // Public methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message handling

        private void OnUpdateTabsEnabled(object sender, UpdateTabsEnabledMessage message)
        {
            foreach(var item in TabItems)
            {
                if(item.CanBeEnabled())
                {
                    item.IsEnabled = true;
                    if(item is TLCGenMainTabItemViewModel)
                    {
                        var tab = item as TLCGenMainTabItemViewModel;
                        foreach(var item2 in tab.TabItems)
                        {
                            if(item2.CanBeEnabled())
                            {
                                item2.IsEnabled = true;
                            }
                            else
                            {
                                item2.IsEnabled = false;
                            }
                        }
                    }
                }
                else
                {
                    item.IsEnabled = false;
                }
            }
        }

        private void OnIsFasenConflictRequestReceived(object sender, IsFasenConflictingRequest request)
        {
            if (request.Handled == false)
            {
                request.Handled = true;
                request.IsConflicting = TLCGenIntegrityChecker.IsFasenConflicting(_Controller, request.Define1, request.Define2);
            }
        }

        #endregion // TLCGen Message handling

        #region Constructor

        public ControllerViewModel()
        {
            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default.ApplicationPlugins);
            var plugindex = 100;
            foreach(var part in parts)
            {
                if((part.Item1 & TLCGenPluginElems.TabControl) == TLCGenPluginElems.TabControl)
                {
                    var attr = part.Item2.GetType().GetCustomAttribute<TLCGenTabItemAttribute>();
                    if(attr != null && attr.Type == TabItemTypeEnum.MainWindow)
                    {
                        if(attr.Index == -1)
                        {
                            tabs.Add(plugindex++, part.Item2 as ITLCGenTabItem);
                        }
                        else
                        {
                            tabs.Add(attr.Index, part.Item2 as ITLCGenTabItem);
                        }
                    }
                }
            }

            foreach(var tab in tabs)
            {
                TabItems.Add(tab.Value);
                var attr = tab.Value.GetType().GetCustomAttribute<TLCGenTabItemAttribute>();
                if((attr.Type & TabItemTypeEnum.MainWindow) == TabItemTypeEnum.MainWindow)
                {
                    tab.Value.LoadTabs();
                }
            }
            
            WeakReferenceMessengerEx.Default.Register<UpdateTabsEnabledMessage>(this, OnUpdateTabsEnabled);
            WeakReferenceMessengerEx.Default.Register<IsFasenConflictingRequest>(this, OnIsFasenConflictRequestReceived);

            SelectedTab = TabItems?.Count > 0 ? TabItems[0] : null;
        }

        #endregion // Constructor
    }
}
