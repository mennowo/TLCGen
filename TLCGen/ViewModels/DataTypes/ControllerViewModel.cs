using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class ControllerViewModel : ViewModelBase
    {
        #region Fields

        private ControllerDataViewModel _ControllerDataVM;
        private FasenTabViewModel _FasenTabVM;
        private ConflictMatrixViewModel _ConflictMatrixVM;
        private DetectorenTabViewModel _DetectorenTabVM;

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

        public ControllerDataViewModel ControllerDataVM
        {
            get
            {
                if(_ControllerDataVM == null)
                {
                    _ControllerDataVM = new ControllerDataViewModel(this, _Controller.Data);
                }
                return _ControllerDataVM;
            }
        }

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

        public ConflictMatrixViewModel ConflictMatrixVM
        {
            get
            {
                if (_ConflictMatrixVM == null)
                {
                    _ConflictMatrixVM = new ConflictMatrixViewModel(this);
                }
                return _ConflictMatrixVM;
            }
        }

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

        public bool HasChanged
        {
            get { return _HasChanged; }
            set
            {
                _HasChanged = value;
                OnPropertyChanged("HasChanged");
            }
        }

        public bool IsSortingFasen
        {
            get { return _IsSortingFasen; }
            set
            {
                _IsSortingFasen = value;
                OnPropertyChanged("IsSortingFasen");
            }
        }

        public bool HasChangedFasen
        {
            get { return _HasChangedFasen; }
            set
            {
                _HasChangedFasen = value;
                OnPropertyChanged("HasChanged");
            }
        }

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

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                // Save the conflict matrix if needed
                if (_SelectedTab != null && 
                    _SelectedTab.Header.ToString() == "Conflicten" &&
                    _ConflictMatrixVM.MatrixChanged)
                {
                    string s = _ConflictMatrixVM.IsMatrixSymmetrical();
                    if(!string.IsNullOrEmpty(s))
                    {
                        System.Windows.MessageBox.Show(s, "Error: Conflict matrix niet symmetrisch.");
                        return;
                    }
                    _ConflictMatrixVM.SaveConflictMatrix();
                }
                if(_SelectedTab != null && 
                   _SelectedTab.Header.ToString() == "Fasen")
                {
                    DoUpdateFasen();
                }
                _SelectedTab = value;
            }
        }

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

        public bool DoUpdateFasen()
        {
            if (!Fasen.IsSorted() || HasChangedFasen)
            {
                // Temporarily don't watch collection changes: the collection doesn't really change, just reorders itself
                Fasen.CollectionChanged -= Fasen_CollectionChanged;

                // Sort, update
                SortFasen();
                HasChangedFasen = false;
                _ConflictMatrixVM.BuildConflictMatrix();
                _ConflictMatrixVM.MatrixChanged = false;

                // Watch collection changes again
                Fasen.CollectionChanged += Fasen_CollectionChanged;
                return true;
            }
            return false;
        }

        public void ChangeFaseDefine(FaseCyclusViewModel fcvm, string olddefine)
        {
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

        #endregion // Public methods

        #region Collection Changed

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
        }

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.NewItems)
                {
                    _Controller.Dectectoren.Add(dvm.Detector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.OldItems)
                {
                    _Controller.Dectectoren.Remove(dvm.Detector);
                }
            }
            HasChanged = true;
            HasChangedFasen = true;
        }

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

        public ControllerViewModel(ControllerModel controller)
        {
            _Controller = controller;

            // Add data from the Model to the ViewModel structure
            foreach(FaseCyclusModel fcm in _Controller.Fasen)
            {
                FaseCyclusViewModel fcvm = new FaseCyclusViewModel(this, fcm);
                Fasen.Add(fcvm);
            }
            foreach (DetectorModel dm in _Controller.Dectectoren)
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
