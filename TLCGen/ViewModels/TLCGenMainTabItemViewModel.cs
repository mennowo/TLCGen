using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using TLCGen.Dependencies.Providers;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public abstract class TLCGenMainTabItemViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        protected ObservableCollection<ITLCGenTabItem> _TabItems;
        protected ITLCGenTabItem _SelectedTab;
        protected TabItemTypeEnum _TabType;

        #endregion // Fields

        #region Properties

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

        public ITLCGenTabItem SelectedTab
        {
            get => _SelectedTab;
            set
            {
                // Take actions for current 
                if (_SelectedTab != null)
                {
                    if (_SelectedTab.OnDeselectedPreview())
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

        #region TabItem Overrides

        public override bool OnSelectedPreview()
        {
            if (SelectedTab == null)
                return true;
            else
                return SelectedTab.OnSelectedPreview();
        }

        public override void OnSelected()
        {
			if(SelectedTab == null && TabItems.Any())
			{
				SelectedTab = TabItems.First();
			}
            SelectedTab?.OnSelected();
        }

        public override bool OnDeselectedPreview()
        {
            if (SelectedTab == null)
                return true;
            else
                return SelectedTab.OnDeselectedPreview();
        }

        public override void OnDeselected()
        {
            SelectedTab?.OnDeselected();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            //foreach (var tab in TabItems)
            //{
            //    tab.Controller = value;
            //}
            set => base.Controller = value;
        }

        #endregion // TabItem Overrides

        #region TabItem Method Overrides

        public override void LoadTabs()
        {
            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default.ApplicationPlugins);
            var plugindex = 100;
            foreach (var part in parts)
            {
                if ((part.Item1 & TLCGenPluginElems.TabControl) == TLCGenPluginElems.TabControl)
                {
                    var attr = part.Item2.GetType().GetCustomAttribute<TLCGenTabItemAttribute>();
                    if (attr != null && attr.Type == _TabType)
                    {
                        try
                        {
                            tabs.Add(attr.Index == -1 ? plugindex++ : attr.Index, part.Item2 as ITLCGenTabItem);
                        }
                        catch (Exception e)
                        {
                            TLCGenDialogProvider.Default.ShowMessageBox(
                                $"Cannot add tab with ID {(attr.Index == -1 ? plugindex : attr.Index)} to tab with type {_TabType}:\n{e.ToString()}",
                                "Error adding tab",
                                MessageBoxButton.OK);
                        }
                    }
                }
            }

            foreach (var tab in tabs)
            {
                TabItems.Add(tab.Value);
            }
        }

        #endregion // Public Methods

        #region Constructor

        public TLCGenMainTabItemViewModel(TabItemTypeEnum type) : base()
        {
            _TabType = type;
        }

        #endregion // Constructor
    }
}