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
                    if (!Fasen.IsSorted() || HasChangedFasen)
                    {
                        SortFasen();
                        HasChangedFasen = false;
                        _ConflictMatrixVM.BuildConflictMatrix();
                    }
                    _ConflictMatrixVM.MatrixChanged = false;
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
            IsSortingFasen = true;
            Fasen.BubbleSort();
            IsSortingFasen = false;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Checks if a given string is unique in the list of Fasen. Uniqueness is determined by
        /// comparing the given argument to all instances of FaseCyclusViewModel in the Fasen collection.
        /// </summary>
        /// <param name="name">The "Naam" string of the FaseCyclusViewModel instance</param>
        /// <returns></returns>
        public bool IsFaseNaamUnique(string naam)
        {
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (fcvm.Naam == naam)
                    return false;
            }
            return true;
        }

        public bool IsFaseDefineUnique(string define)
        {
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (fcvm.Define == define)
                    return false;
            }
            return true;
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
        }

        public bool IsDetectorNaamUnique(string naam)
        {
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

        public bool IsDetectorDefineUnique(string define)
        {
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
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (FaseCyclusViewModel fcvm in e.OldItems)
                {
                    _Controller.Fasen.Remove(fcvm.FaseCyclus);
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

            // Connect CollectionChanged event handlers
            Fasen.CollectionChanged += Fasen_CollectionChanged;
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
        }

        #endregion // Constructor
    }
}
