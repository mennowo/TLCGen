using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging;
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
            get { return _SelectedTab; }
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
                    RaisePropertyChanged("SelectedTab");
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
            get
            {
                return base.Controller;
            }

            set
            {
                foreach(var tab in TabItems)
                {
                    tab.Controller = value;
                }
                base.Controller = value;
            }
        }

        #endregion // TabItem Overrides

        #region TabItem Method Overrides

        public override void LoadTabs()
        {
            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default.ApplicationPlugins);
            int plugindex = 100;
            foreach (var part in parts)
            {
                if ((part.Item1 & TLCGenPluginElems.TabControl) == TLCGenPluginElems.TabControl)
                {
                    var attr = part.Item2.GetType().GetCustomAttribute<TLCGenTabItemAttribute>();
                    if (attr != null && attr.Type == _TabType)
                    {
                        if (attr.Index == -1)
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
