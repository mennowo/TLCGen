using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class ControllerViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Fields

        private TLCGenStatusBarViewModel _StatusBarVM;
        
        private ControllerModel _Controller;

        private ObservableCollection<ITLCGenTabItem> _TabItems;
        private ITLCGenTabItem _SelectedTab;
        private int _SelectedTabIndex;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Readonly reference to the data model
        /// </summary>
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                foreach (var tab in _TabItems)
                {
                    tab.Controller = value;
                }
                RaisePropertyChanged();
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
        /// ViewModel for status bar
        /// </summary>
        public TLCGenStatusBarViewModel StatusBarVM
        {
            get
            {
                if (_StatusBarVM == null)
                    _StatusBarVM = new TLCGenStatusBarViewModel();
                return _StatusBarVM;
            }
        }

        /// <summary>
        /// Reference to the selected tab in the Controller View
        /// </summary>
        public ITLCGenTabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                // Take actions for current 
                if (_SelectedTab != null)
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
                    RaisePropertyChanged("SelectedTab");
                }
            }
        }

        /// <summary>
        /// Index of the selected tab in the Controller View
        /// </summary>
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                RaisePropertyChanged("SelectedTabIndex");
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
            RaisePropertyChanged(null);
            SelectedTabIndex = 0;
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
                        //FasenTabVM.SetAllSelectedFasenValue(o as FaseCyclusViewModel, propName);
                        break;
                    case "Detectoren":
                        IsSetting = true;
                        //DetectorenTabVM.SetAllSelectedDetectorenValue(o as DetectorViewModel, propName);
#warning TODO
                        break;
                }
                IsSetting = false;
            }
        }

        /// <summary>
        /// Method to set the text of the status bar
        /// </summary>
        /// <param name="statustext">The text to set</param>
        public void SetStatusText(string statustext)
        {
            StatusBarVM.StatusText = DateTime.Now.ToLongTimeString() + " -> " + statustext;
        }

        #endregion // Public methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message handling

        private void OnUpdateTabsEnabled(UpdateTabsEnabledMessage message)
        {
            foreach(ITLCGenTabItem item in TabItems)
            {
                if(item.CanBeEnabled())
                {
                    item.IsEnabled = true;
                    if(item is TLCGenMainTabItemViewModel)
                    {
                        TLCGenMainTabItemViewModel tab = item as TLCGenMainTabItemViewModel;
                        foreach(ITLCGenTabItem item2 in tab.TabItems)
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

        private void OnNameChanged(NameChangedMessage message)
        {
            ModelStringSetter.SetStringInModel(_Controller, message.OldName, message.NewName);
        }

        private void OnIsElementIdentifierUniqueRequestReceived(IsElementIdentifierUniqueRequest request)
        {
            if (request.Handled == false)
            {
                switch(request.Type)
                {
                    case ElementIdentifierType.Naam:
                        request.IsUnique = IntegrityChecker.IsElementNaamUnique(_Controller, request.Identifier);
                        request.Handled = true;
                        break;
                    case ElementIdentifierType.VissimNaam:
                        request.IsUnique = IntegrityChecker.IsElementVissimNaamUnique(_Controller, request.Identifier);
                        request.Handled = true;
                        break;
                }
            }
        }

        private void OnIsFasenConflictRequestReceived(IsFasenConflictingRequest request)
        {
            if (request.Handled == false)
            {
                request.Handled = true;
                request.IsConflicting = IntegrityChecker.IsFasenConflicting(_Controller, request.Define1, request.Define2);
            }
        }

        #endregion // TLCGen Message handling

        #region Constructor

        public ControllerViewModel()
        {
            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            var parts = TLCGenPluginManager.Default.ApplicationParts.Concat(TLCGenPluginManager.Default.ApplicationPlugins);
            int plugindex = 100;
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
            
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            Messenger.Default.Register(this, new Action<UpdateTabsEnabledMessage>(OnUpdateTabsEnabled));
            Messenger.Default.Register(this, new Action<IsElementIdentifierUniqueRequest>(OnIsElementIdentifierUniqueRequestReceived));
            Messenger.Default.Register(this, new Action<IsFasenConflictingRequest>(OnIsFasenConflictRequestReceived));

            SelectedTabIndex = 0;
        }

        #endregion // Constructor
    }
}
