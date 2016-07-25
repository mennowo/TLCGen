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
        private FasenListViewModel _FasenListVM;
        private ConflictMatrixViewModel _ConflictMatrixVM;

        private bool _HasChanged;
        private bool _IsSortingFasen;
        private bool _HasChangedFasen;
        private ControllerModel _Controller;
        private ObservableCollection<FaseCyclusViewModel> _Fasen;
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

        public FasenListViewModel FasenListVM
        {
            get
            {
                if(_FasenListVM == null)
                {
                    _FasenListVM = new FasenListViewModel(this);
                }
                return _FasenListVM;
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
        /// <param name="define">The "define" string of the FaseCyclusViewModel instance</param>
        /// <returns></returns>
        public bool IsFaseDefineUnique(string define)
        {
            foreach (FaseCyclusViewModel fcvm in Fasen)
            {
                if (fcvm.Define == define)
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

            // Connect CollectionChanged event handlers
            Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
