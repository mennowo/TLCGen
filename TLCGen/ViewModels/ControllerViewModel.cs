using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ControllerViewModel : ViewModelBase
    {
        #region Fields

        private MainWindowViewModel _MainWindowVM;
        private AlgemeenTabViewModel _AlgemeenTabVM;
        private FasenTabViewModel _FasenTabVM;
        private CoordinatiesTabViewModel _CoordinatiesTabVM;
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
        private ObservableCollection<MaxGroentijdenSetViewModel> _MaxGroentijdenSets;
        private TabItem _SelectedTab;
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

        /// <summary>
        /// ViewModel for generic information and controller settings
        /// </summary>
        public AlgemeenTabViewModel AlgemeenTabVM
        {
            get
            {
                if(_AlgemeenTabVM == null)
                {
                    _AlgemeenTabVM = new AlgemeenTabViewModel(this, _Controller.Data);
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
                    _FasenTabVM = new FasenTabViewModel(this);
                }
                return _FasenTabVM;
            }
        }

        /// <summary>
        /// ViewModel for tab with conflicts and coordinations
        /// </summary>
        public CoordinatiesTabViewModel CoordinatiesTabVM
        {
            get
            {
                if (_CoordinatiesTabVM == null)
                {
                    _CoordinatiesTabVM = new CoordinatiesTabViewModel(this);
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
                    _DetectorenTabVM = new DetectorenTabViewModel(this);
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
                    _ModulesTabVM = new ModulesTabViewModel(this);
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
                    _BitmapTabVM = new BitmapTabViewModel(this);
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
        public ObservableCollection<MaxGroentijdenSetViewModel> MaxGroentijdenSets
        {
            get
            {
                if (_MaxGroentijdenSets == null)
                {
                    _MaxGroentijdenSets = new ObservableCollection<MaxGroentijdenSetViewModel>();
                }
                return _MaxGroentijdenSets;
            }
        }

        /// <summary>
        /// Reference to the selected tab in the Controller View
        /// </summary>
        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                // Take actions for current 
                if (_SelectedTab != null)
                {
                    // Save the conflict matrix if needed
                    if (_SelectedTab.Name == "ConflictenTab" &&
                        _CoordinatiesTabVM.MatrixChanged)
                    {
                        string s = _CoordinatiesTabVM.IsMatrixSymmetrical();
                        if (!string.IsNullOrEmpty(s))
                        {
                            System.Windows.MessageBox.Show(s, "Error: Conflict matrix niet symmetrisch.");
                            return;
                        }
                        _CoordinatiesTabVM.SaveConflictMatrix();
                    }
                    // Update Fasen
                    if (_SelectedTab.Name == "FasenTab")
                    {
                        DoUpdateFasen();
                    }
                }

                // Set new value
                _SelectedTab = value;

                // Take actions as needed
                if (_SelectedTab != null)
                {
                    if (_SelectedTab.Name == "BitmapTab")
                    {
                        // Collect all IO to be displayed in the lists
                        BitmapTabVM.CollectAllIO();

                        // Set the bitmap
                        if(DataProvider.FileName != null)
                            BitmapTabVM.BitmapFileName =
                                System.IO.Path.Combine(
                                    System.IO.Path.GetDirectoryName(DataProvider.FileName), 
                                    AlgemeenTabVM.BitmapNaam.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) ? 
                                    AlgemeenTabVM.BitmapNaam :
                                    AlgemeenTabVM.BitmapNaam + ".bmp"
                                );
                    }
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

        /// <summary>
        /// Indicates whether or not the 'modules' tab should be enabled
        /// </summary>
        public bool ModulesTabEnabled
        {
            get
            {
                return Fasen?.Count > 0;
            }
        }

        /// <summary>
        /// Indicates whether or not the 'bitmap' tab should be enabled
        /// </summary>
        public bool BitmapTabEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(_AlgemeenTabVM.BitmapNaam) && 
                       !string.IsNullOrWhiteSpace(DataProvider.FileName);
            }
        }

        #endregion // Properties

        #region Private methods

        /// <summary>
        /// Method to instruct the view to check if tabs should be or not be enabled
        /// </summary>
        public void UpdateTabsEnabled()
        {
            OnPropertyChanged("ModulesTabEnabled");
            OnPropertyChanged("BitmapTabEnabled");
        }

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
                // Temporarily don't watch collection changes: the collection doesn't really change, just reorders itself
                Fasen.CollectionChanged -= Fasen_CollectionChanged;

                // Sort, update
                SortFasen();
                HasChangedFasen = false;
                _CoordinatiesTabVM.BuildConflictMatrix();
                _CoordinatiesTabVM.MatrixChanged = false;

                // Watch collection changes again
                Fasen.CollectionChanged += Fasen_CollectionChanged;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes the Define property for an instance of FaseCyclusViewModel, and propagates that change throughout the model.
        /// </summary>
        /// <param name="fcvm">The instance of FaseCyclusViewModel whose Define property will be changed</param>
        /// <param name="olddefine">The old value of Define, which needs to be replaced elsewhere</param>
        public void ChangeFaseDefine(FaseCyclusViewModel fcvm, string olddefine)
        {
            foreach(DetectorViewModel dvm in fcvm.Detectoren)
            {
                if(!string.IsNullOrEmpty(dvm.FCNr))
                {
                    dvm.FCNr = fcvm.Define;
                }
            }
            foreach(FaseCyclusViewModel fcvm2 in Fasen)
            {
                foreach(ConflictViewModel cvm in fcvm2.Conflicten)
                {
                    if (cvm.FaseNaar == olddefine)
                        cvm.FaseNaar = fcvm.Define;
                }
            }
            foreach(MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
            {
                foreach(MaxGroentijdViewModel mgvm in mgsvm.MaxGroentijdenSetList)
                {
                    if(mgvm.FaseCyclus == olddefine)
                    {
                        mgvm.FaseCyclus = fcvm.Define;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if an element's Name property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Name property to check</param>
        /// <returns>True if unique, false if not</returns>
        public bool IsElementNaamUnique(string naam)
        {
            // Check fasen
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (fcvm.Naam == naam)
                    return false;
            }

            // Check detectie
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                foreach(DetectorViewModel dvm in fcvm.Detectoren)
                {
                    if (dvm.Naam == naam)
                        return false;
                }
            }
            foreach (DetectorViewModel dvm in Detectoren)
            {
                if (dvm.Naam == naam)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if an element's Define property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Define property to check</param>
        /// <returns>True if unique, false if not</returns>
        public bool IsElementDefineUnique(string define)
        {
            // Fasen
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (fcvm.Define == define)
                    return false;
            }

            // Detectie
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                foreach (DetectorViewModel dvm in fcvm.Detectoren)
                {
                    if (dvm.Define == define)
                        return false;
                }
            }
            foreach (DetectorViewModel dvm in Detectoren)
            {
                if (dvm.Define == define)
                    return false;
            }
            return true;
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
                switch(SelectedTab.Name)
                {
                    case "FasenTab":
                        IsSetting = true;
                        FasenTabVM.SetAllSelectedFasenValue(o as FaseCyclusViewModel, propName);
                        break;
                    case "DetectorenTab":
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
        /// This method is executed when the collection of phases changes
        /// </summary>
        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.NewItems)
                {
                    _Controller.Fasen.Add(fcvm.FaseCyclus);
                    foreach(MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
                    {
                        mgsvm.AddFase(fcvm.Define);
                    }
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    _Controller.Fasen.Remove(fcvm.FaseCyclus);
                    foreach (MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
                    {
                        mgsvm.RemoveFase(fcvm.Define);
                    }
                }
            }
            HasChanged = true;
            HasChangedFasen = true;
            UpdateTabsEnabled();
        }

        /// <summary>
        /// This method is executed when the collection of 'extra' detectors changes
        /// </summary>
        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.NewItems)
                {
                    _Controller.Detectoren.Add(dvm.Detector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.OldItems)
                {
                    _Controller.Detectoren.Remove(dvm.Detector);
                }
            }
            HasChanged = true;
            HasChangedFasen = true;
        }

        /// <summary>
        /// This method is executed when the collection of maxgreentime sets changes
        /// </summary>
        private void MaxGroentijdenSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (MaxGroentijdenSetViewModel mgsvm in e.NewItems)
                {
                    _Controller.MaxGroentijdenSets.Add(mgsvm.MaxGroentijdenSet);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (MaxGroentijdenSetViewModel mgsvm in e.OldItems)
                {
                    _Controller.MaxGroentijdenSets.Remove(mgsvm.MaxGroentijdenSet);
                }
            }
            HasChanged = true;
        }

        #endregion // Collection Changed

        #region Constructor

        public ControllerViewModel(MainWindowViewModel mainwindowvm, ControllerModel controller)
        {
            _MainWindowVM = mainwindowvm;
            _Controller = controller;

            // Add data from the Model to the ViewModel structure
            foreach(FaseCyclusModel fcm in _Controller.Fasen)
            {
                FaseCyclusViewModel fcvm = new FaseCyclusViewModel(this, fcm);
                Fasen.Add(fcvm);
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                DetectorViewModel dvm = new DetectorViewModel(this, dm);
                Detectoren.Add(dvm);
            }
            foreach (MaxGroentijdenSetModel mgm in _Controller.MaxGroentijdenSets)
            {
                MaxGroentijdenSetViewModel mgvm = new MaxGroentijdenSetViewModel(this, mgm);
                MaxGroentijdenSets.Add(mgvm);
            }

            // Connect CollectionChanged event handlers
            Fasen.CollectionChanged += Fasen_CollectionChanged;
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            MaxGroentijdenSets.CollectionChanged += MaxGroentijdenSets_CollectionChanged;
        }

        #endregion // Constructor
    }
}
