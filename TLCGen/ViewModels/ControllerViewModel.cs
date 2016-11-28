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

        private MainWindowViewModel _MainWindowVM;
        private AlgemeenTabViewModel _AlgemeenTabVM;
        private FasenTabViewModel _FasenTabVM;
        private SynchronisatiesTabViewModel _CoordinatiesTabVM;
        private DetectorenTabViewModel _DetectorenTabVM;
        private ModulesTabViewModel _ModulesTabVM;
        private BitmapTabViewModel _BitmapTabVM;
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
        /// ViewModel for generic information and controller settings
        /// </summary>
        public AlgemeenTabViewModel AlgemeenTabVM
        {
            get
            {
                if(_AlgemeenTabVM == null)
                {
                    _AlgemeenTabVM = new AlgemeenTabViewModel(_Controller, _Controller.Data);
                }
                return _AlgemeenTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab with phases
        /// </summary>
        public FasenTabViewModel FasenTabVM
        {
            get
            {
                if(_FasenTabVM == null)
                {
                    _FasenTabVM = new FasenTabViewModel(Controller);
                }
                return _FasenTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab with conflicts and coordinations
        /// </summary>
        public SynchronisatiesTabViewModel CoordinatiesTabVM
        {
            get
            {
                if (_CoordinatiesTabVM == null)
                {
                    _CoordinatiesTabVM = new SynchronisatiesTabViewModel(_Controller);
                }
                return _CoordinatiesTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab with detection
        /// </summary>
        public DetectorenTabViewModel DetectorenTabVM
        {
            get
            {
                if (_DetectorenTabVM == null)
                {
                    _DetectorenTabVM = new DetectorenTabViewModel(_Controller);
                }
                return _DetectorenTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab with modules
        /// </summary>
        public ModulesTabViewModel ModulesTabVM
        {
            get
            {
                if (_ModulesTabVM == null)
                {
                    _ModulesTabVM = new ModulesTabViewModel(_Controller);
                }
                return _ModulesTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab where the bitmap coordinates can be set
        /// </summary>
        public BitmapTabViewModel BitmapTabVM
        {
            get
            {
                if (_BitmapTabVM == null)
                {
                    _BitmapTabVM = new BitmapTabViewModel(_Controller);
                }
                return _BitmapTabVM;
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
                if (_SelectedTab != null && _SelectedTab.DeselectedPreview())
                {
                    _SelectedTab.Deselected();
                }

                // Preview new, set if good
                if (value.SelectedPreview())
                {
                    _SelectedTab = value;
                    _SelectedTab.Selected();
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
                FasenTabVM.SortMaxGroenSetsFasen();
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
            FasenTabVM.SelectedTabIndex = 0;
        }

        /// <summary>
        /// Processes all changes made via the UI to the model.
        /// </summary>
        public void ProcessAllChanges()
        {
            CoordinatiesTabVM.SaveConflictMatrix();
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
                CoordinatiesTabVM.BuildConflictMatrix();
                CoordinatiesTabVM.MatrixChanged = false;
                
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
                        FasenTabVM.SetAllSelectedFasenValue(o as FaseCyclusViewModel, propName);
                        break;
                    case "Detectoren":
                        IsSetting = true;
                        DetectorenTabVM.SetAllSelectedDetectorenValue(o as DetectorViewModel, propName);
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

        /// <summary>
        /// This method is executed when the collection of maxgreentime sets changes
        /// </summary>
        private void MaxGroentijdenSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (GroentijdenSetViewModel mgsvm in e.NewItems)
                {
                    _Controller.GroentijdenSets.Add(mgsvm.GroentijdenSet);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (GroentijdenSetViewModel mgsvm in e.OldItems)
                {
                    _Controller.GroentijdenSets.Remove(mgsvm.GroentijdenSet);
                }
            }
            HasChanged = true;
        }

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

        private void OnDetectorListRequested(GetDetectorListReqeust<List<DetectorModel>, object> message)
        {
            List<DetectorModel> detectors = new List<DetectorModel>();
            foreach(FaseCyclusModel fcm in Controller.Fasen)
            {
                foreach(DetectorModel dm in fcm.Detectoren)
                {
                    detectors.Add(dm);
                }
            }
            foreach (DetectorModel dm in Controller.Detectoren)
            {
                detectors.Add(dm);
            }
            message.Callback.Invoke(detectors);
        }

        #endregion // TLCGen Message handling

        #region Constructor

        public ControllerViewModel(MainWindowViewModel mainwindowvm, ControllerModel controller)
        {
            _MainWindowVM = mainwindowvm;
            _Controller = controller;

            // Add data from the Model to the ViewModel structure
            foreach(FaseCyclusModel fcm in _Controller.Fasen)
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

            MessageManager.Instance.Subscribe(this, new Action<ControllerDataChangedMessage>(OnControllerDataChanged));
            MessageManager.Instance.Subscribe(this, new Action<NameChangedMessage>(OnNameChanged));
            MessageManager.Instance.Subscribe(this, new Action<DefineChangedMessage>(OnDefineChanged));
            MessageManager.Instance.Subscribe(this, new Action<UpdateTabsEnabledMessage>(OnUpdateTabsEnabled));
            MessageManager.Instance.Subscribe(this, new Action<GetDetectorListReqeust<List<DetectorModel>, object>>(OnDetectorListRequested));

            // Connect CollectionChanged event handlers
            MaxGroentijdenSets.CollectionChanged += MaxGroentijdenSets_CollectionChanged;

            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == AlgemeenTabVM.DisplayName).Any())
            {
                TabItems.Add(AlgemeenTabVM as ITLCGenTabItem);
            }
            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == FasenTabVM.DisplayName).Any())
            {
                TabItems.Add(FasenTabVM as ITLCGenTabItem);
            }
            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == DetectorenTabVM.DisplayName).Any())
            {
                TabItems.Add(DetectorenTabVM as ITLCGenTabItem);
            }
            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == CoordinatiesTabVM.DisplayName).Any())
            {
                TabItems.Add(CoordinatiesTabVM as ITLCGenTabItem);
            }
            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == ModulesTabVM.DisplayName).Any())
            {
                TabItems.Add(ModulesTabVM as ITLCGenTabItem);
            }
            if (!_MainWindowVM.TabItems.Where(x => x.GetPluginName() == BitmapTabVM.DisplayName).Any())
            {
                TabItems.Add(BitmapTabVM as ITLCGenTabItem);
            }
            foreach(ITLCGenTabItem item in _MainWindowVM.TabItems)
            {
                if(!TabItems.Contains(item))
                {
                    TabItems.Add(item);
                }
            }

            SelectedTab = AlgemeenTabVM as ITLCGenTabItem;
            SelectedTabIndex = 0;
        }

        #endregion // Constructor
    }
}
