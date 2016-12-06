using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        #endregion // TabItem Overrides

        #region Constructor

        public TLCGenMainTabItemViewModel(ControllerModel controller) : base(controller)
        {
        }

        #endregion // Constructor
    }
}
