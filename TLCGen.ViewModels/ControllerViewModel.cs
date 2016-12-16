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
using TLCGen.Integrity;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public class ControllerViewModel : ViewModelBase
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
            set { _Controller = value; }
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
        /// Indicates whether the model has ben changed by the user. This property is used to determine if
        /// the user needs to be shown a dialog, asking to save changes, when closing the controller.
        /// </summary>
        public bool HasChanged
        {
            get { return _HasChanged; }
            set
            {
                _HasChanged = value;
                OnPropertyChanged("HasChanged");
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
                    OnPropertyChanged("SelectedTab");
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
                OnPropertyChanged("SelectedTabIndex");
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
            OnPropertyChanged(null);
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

        public XmlDocument GetControllerXmlData()
        {
            var doc = TLCGenSerialization.SerializeToXmlDocument(_Controller);
            foreach (var v in _LoadedPlugins)
            {
                if (v is ITLCGenXMLNodeWriter)
                {
                    var writer = (ITLCGenXMLNodeWriter)v;
                    writer.SetXmlInDocument(doc);
                }
            }
            return doc;
        }

        public void LoadPluginDataFromXmlDocument(XmlDocument document)
        {
            if (document == null)
                return;

            foreach (var v in _LoadedPlugins)
            {
                if (v is ITLCGenXMLNodeWriter)
                {
                    var writer = (ITLCGenXMLNodeWriter)v;
                    writer.GetXmlFromDocument(document);
                }
            }
        }

        #endregion // Public methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message handling

        private void OnControllerDataChanged(ControllerDataChangedMessage message)
        {
            this.HasChanged = true;
        }

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

        public void SetStringInModel(object obj, string oldstring, string newstring)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj);
                if (property.PropertyType == typeof(string))
                {
                    string propString = (string)propValue;
                    if (propString == oldstring)
                    {
                        property.SetValue(obj, newstring);
                    }
                }
                else if (!objType.IsValueType)
                {
                    var elems = propValue as IList;
                    if (elems != null)
                    {
                        foreach (var item in elems)
                        {
                            SetStringInModel(item, oldstring, newstring);
                        }
                    }
                    else
                    {
                        SetStringInModel(propValue, oldstring, newstring);
                    }
                }
            }
        }

        private void OnNameChanged(NameChangedMessage message)
        {
            SetStringInModel(_Controller, message.OldName, message.NewName);
        }

        private void OnDefineChanged(DefineChangedMessage message)
        {
            SetStringInModel(_Controller, message.OldDefine, message.NewDefine);
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
                    case ElementIdentifierType.Define:
                        request.IsUnique = IntegrityChecker.IsElementDefineUnique(_Controller, request.Identifier);
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

        public ControllerViewModel(ControllerModel controller)
        {
            _Controller = controller;
            _LoadedPlugins = new List<ITLCGenPlugin>();

            var tabs = new SortedDictionary<int, ITLCGenTabItem>();
            foreach(var v in TLCGenPluginManager.Default.ApplicationParts)
            {
                if(v.Item1.HasFlag(TLCGenPluginElems.TabControl))
                {
                    var attr = (TLCGenTabItemAttribute)Attribute.GetCustomAttribute(v.Item2, typeof(TLCGenTabItemAttribute));
                    if (attr != null && attr.Type == TabItemTypeEnum.MainWindow)
                    {
                        tabs.Add(attr.Index, (ITLCGenTabItem)Activator.CreateInstance(v.Item2, _Controller));
                    }
                }
            }
            foreach (var v in TLCGenPluginManager.Default.Plugins)
            {
                if (v.Item1.HasFlag(TLCGenPluginElems.TabControl))
                {
                    int i = tabs.Count;
                    var tab = (ITLCGenTabItem)Activator.CreateInstance(v.Item2);
                    tab.Controller = _Controller;
                    tabs.Add(i, tab);
                    _LoadedPlugins.Add(tab as ITLCGenPlugin);
                }
            }
            foreach(var tab in tabs)
            {
                TabItems.Add(tab.Value);   
            }

            Messenger.Default.Register(this, new Action<ControllerDataChangedMessage>(OnControllerDataChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            Messenger.Default.Register(this, new Action<DefineChangedMessage>(OnDefineChanged));
            Messenger.Default.Register(this, new Action<UpdateTabsEnabledMessage>(OnUpdateTabsEnabled));
            Messenger.Default.Register(this, new Action<IsElementIdentifierUniqueRequest>(OnIsElementIdentifierUniqueRequestReceived));
            Messenger.Default.Register(this, new Action<IsFasenConflictingRequest>(OnIsFasenConflictRequestReceived));

            SelectedTabIndex = 0;
        }

        #endregion // Constructor
    }
}
