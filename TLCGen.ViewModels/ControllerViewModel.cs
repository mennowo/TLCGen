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
        private bool _IsSortingFasen;
        private bool _HasChangedFasen;
        private ControllerModel _Controller;
        private ObservableCollection<FaseCyclusViewModel> _Fasen;
        private ObservableCollection<DetectorViewModel> _Detectoren;
        private ObservableCollection<GroentijdenSetViewModel> _MaxGroentijdenSets;

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

        public SortedDictionary<int,Type> TabItemTypes
        {
            set
            {
                if (value == null)
                {
                    throw new NotImplementedException();
                }
                TabItems.Clear();
                foreach(var tab in value)
                {
                    var v = Activator.CreateInstance(tab.Value, _Controller);
                    TabItems.Add(v as ITLCGenTabItem);
                }
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
        /// A property meant to make sorting phases thread safe.
        /// </summary>
        public bool IsSortingFasen
        {
            get { return _IsSortingFasen; }
            set
            {
                _IsSortingFasen = value;
                OnPropertyChanged("IsSortingFasen");
            }
        }

        /// <summary>
        /// Indicates if changes have been made to the list of phases. This value is used to update
        /// the model accordingly, so phases can be sorted, etc.
        /// </summary>
        public bool HasChangedFasen
        {
            get { return _HasChangedFasen; }
            set
            {
                _HasChangedFasen = value;
                OnPropertyChanged("HasChanged");
            }
        }

        /// <summary>
        /// ObservableCollection of phases that belong to the controller
        /// </summary>
        public ObservableCollection<FaseCyclusViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<FaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        /// <summary>
        /// ObservableCollection of detectors that are 'extra', in that they do not belong to any phase
        /// </summary>
        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                }
                return _Detectoren;
            }
        }

        /// <summary>
        /// ObservableCollection of sets of maxgreen times for phases
        /// </summary>
        public ObservableCollection<GroentijdenSetViewModel> MaxGroentijdenSets
        {
            get
            {
                if (_MaxGroentijdenSets == null)
                {
                    _MaxGroentijdenSets = new ObservableCollection<GroentijdenSetViewModel>();
                }
                return _MaxGroentijdenSets;
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

        /// <summary>
        /// Sorts property ObservableCollection<FaseCyclusViewModel> Fasen. 
        /// For this to not disrupt the model data, we temporarily disconnect method Fasen_CollectionChanged
        /// from the CollectionChanged event.
        /// </summary>
        public void SortFasen()
        {
            if (!IsSortingFasen)
            {
                IsSortingFasen = true;
                Fasen.BubbleSort();
                //FasenTabVM.SortMaxGroenSetsFasen();
#warning TODO: check above.
                IsSortingFasen = false;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Soft reload of the current controller: updates bound values and updates phases.
        /// </summary>
        public void ReloadController()
        {
            OnPropertyChanged(null);
            HasChangedFasen = true;
            DoUpdateFasen();
            SelectedTabIndex = 0;
        }

        /// <summary>
        /// Updates the phases collection: sorting, rebuilding conflict matrix, etc.
        /// Only does something if phases have changed or are not sorted.
        /// </summary>
        /// <returns>True if it took action, false if not.</returns>
        public bool DoUpdateFasen()
        {
            if (!Fasen.IsSorted() || HasChangedFasen)
            {
                // Sort, update
                SortFasen();
                HasChangedFasen = false;
                
                return true;
            }
            return false;
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

            // Add data from the Model to the ViewModel structure
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                FaseCyclusViewModel fcvm = new FaseCyclusViewModel(fcm);
                Fasen.Add(fcvm);
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                DetectorViewModel dvm = new DetectorViewModel(dm);
                Detectoren.Add(dvm);
            }
            foreach (GroentijdenSetModel mgm in _Controller.GroentijdenSets)
            {
                GroentijdenSetViewModel mgvm = new GroentijdenSetViewModel(mgm);
                MaxGroentijdenSets.Add(mgvm);
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
