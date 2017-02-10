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

        private bool _HasChanged;
        private ControllerModel _Controller;

        private List<ITLCGenPlugin> _LoadedPlugins;

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
                foreach(var pl in _LoadedPlugins)
                {
                    pl.Controller = value;
                }
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
            _LoadedPlugins = new List<ITLCGenPlugin>();

#warning This must be moved: to plugin manager

            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            foreach(var v in TLCGenPluginManager.Default.ApplicationParts)
            {
                if(v.Item1.HasFlag(TLCGenPluginElems.TabControl))
                {
                    var attr = (TLCGenTabItemAttribute)Attribute.GetCustomAttribute(v.Item2, typeof(TLCGenTabItemAttribute));
                    if (attr != null && attr.Type == TabItemTypeEnum.MainWindow)
                    {
                        tabs.Add(attr.Index, (ITLCGenTabItem)Activator.CreateInstance(v.Item2));
                    }
                }
            }
            foreach (var v in TLCGenPluginManager.Default.Plugins)
            {
                if (v.Item1.HasFlag(TLCGenPluginElems.TabControl) &&
                    !v.Item1.HasFlag(TLCGenPluginElems.Generator) &&
                    !v.Item1.HasFlag(TLCGenPluginElems.Importer) &&
                    !v.Item1.HasFlag(TLCGenPluginElems.ToolBarControl))
                {
                    int i = tabs.Count;
                    var tab = (ITLCGenTabItem)Activator.CreateInstance(v.Item2);
                    tabs.Add(i, tab);
                    _LoadedPlugins.Add(tab as ITLCGenPlugin);
                }
            }
            foreach(var tab in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                var tabpl = tab as ITLCGenTabItem;
                if(tabpl != null)
                {
                    int i = tabs.Count;
                    tabs.Add(i, tabpl);
                    _LoadedPlugins.Add(tab as ITLCGenPlugin);
                }
            }
            foreach(var tab in tabs)
            {
                TabItems.Add(tab.Value);
            }

            TLCGenPluginManager.Default.LoadedPlugins.Clear();
            foreach(var pl in _LoadedPlugins)
            {
                TLCGenPluginManager.Default.LoadedPlugins.Add(pl);

                var messpl = pl as ITLCGenPlugMessaging;
                if(messpl != null)
                {
                    messpl.UpdateTLCGenMessaging();
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
