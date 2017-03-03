using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TLCGen.ViewModels
{
    public class DetectorManagerViewModel<T1, T2> : ViewModelBase where T1 : class where T2 : class
    {
        #region Fields

        private Func<T2, T1> _GetDetectorToAdd;
        private Func<T2, T1> _GetDetectorToRemove;
        private Action _AfterDetectorAddedAction;
        private Action _AfterDetectorRemovedAction;
        private Predicate<T2> _IsDetectorSelectable;
        private T1 _SelectedDetector;

        #endregion // Fields

        #region Properties

        public ObservableCollection<T1> DetectorenInCollection { get; private set; }
        public List<T2> AllDetectoren { get; private set; }
        public ObservableCollection<T2> SelectableDetectoren { get; private set; }
        public ObservableCollection<T2> RemovableDetectoren { get; private set; }

        private T2 _SelectedDetectorToAdd;
        public T2 SelectedDetectorToAdd
        {
            get { return _SelectedDetectorToAdd; }
            set { _SelectedDetectorToAdd = value; }
        }
        private T2 _SelectedDetectorToRemove;
        public T2 SelectedDetectorToRemove
        {
            get { return _SelectedDetectorToRemove; }
            set { _SelectedDetectorToRemove = value; }
        }

        public T1 SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
            }
        }


        #endregion // Properties

        #region Commands

        private RelayCommand _AddDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_AddDetectorCommand == null)
                {
                    _AddDetectorCommand = new RelayCommand(AddDetectorCommand_Executed, AddDetectorCommand_CanExecute);
                }
                return _AddDetectorCommand;
            }
        }

        private RelayCommand _RemoveDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_RemoveDetectorCommand == null)
                {
                    _RemoveDetectorCommand = new RelayCommand(RemoveDetectorCommand_Executed, RemoveDetectorCommand_CanExecute);
                }
                return _RemoveDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool AddDetectorCommand_CanExecute()
        {
            return SelectedDetectorToAdd != null;
        }

        private void AddDetectorCommand_Executed()
        {
            T1 d = _GetDetectorToAdd(SelectedDetectorToAdd);
            DetectorenInCollection.Add(d);
            Refresh();
            _AfterDetectorAddedAction?.Invoke();
        }

        private bool RemoveDetectorCommand_CanExecute()
        {
            return SelectedDetectorToRemove != null || _SelectedDetector != null;
        }

        private void RemoveDetectorCommand_Executed()
        {
            if (SelectedDetectorToRemove != null)
            {
                T1 d = _GetDetectorToRemove(SelectedDetectorToRemove);
                DetectorenInCollection.Remove(d);
            }
            else if(_SelectedDetector != null)
            {
                DetectorenInCollection.Remove(_SelectedDetector);
            }
            Refresh();
            _AfterDetectorRemovedAction?.Invoke();
        }

        #endregion // Command functionality

        #region Private Methods

        private void Refresh()
        {
            var sdta = SelectedDetectorToAdd;
            var sdtr = SelectedDetectorToRemove;

            SelectableDetectoren.Clear();
            foreach (var d in AllDetectoren)
            {
                if (_IsDetectorSelectable(d))
                {
                    SelectableDetectoren.Add(d);
                }
            }
            RemovableDetectoren.Clear();
            foreach (var d in AllDetectoren)
            {
                if (!_IsDetectorSelectable(d))
                {
                    RemovableDetectoren.Add(d);
                }
            }

            if(sdta != null && SelectableDetectoren.Contains(sdta))
                SelectedDetectorToAdd = sdta;
            else if (SelectableDetectoren.Count > 0)
                SelectedDetectorToAdd = SelectableDetectoren[0];

            if (_GetDetectorToRemove != null)
            {
                if (sdtr != null && RemovableDetectoren.Contains(sdtr))
                    SelectedDetectorToRemove = sdtr;
                else if (RemovableDetectoren.Count > 0)
                    SelectedDetectorToRemove = RemovableDetectoren[0];
            }
        }

        #endregion // Private Methods

        #region Constructor

        /// <summary>
        /// Wrapper class to support easy management of a list of specific detectors
        /// belonging to an object. This situation occurs more often, which is why this
        /// class automizes the process of adding and removing detectors to a given list.
        /// The class is meant to be coupled with an instance of DetectorManagerView.
        /// The class allows for multiple scenarios.
        /// T1 = the type of class of items in the list to be managed
        /// T2 = the type of class of items in list(s) to be displayed to the user, to
        /// allow selection of detectors to add (and remove if needed).
        /// </summary>
        /// <param name="detectoren">The actual list (T1) that is to be managed.</param>
        /// <param name="alldetectoren">A list (T2) holding all detectors that could be added.</param>
        /// <param name="getdetectortoaddfunc">A function that converts T2 to T1 and supplies a new instance of T1.</param>
        /// <param name="isdetectorselectable">Predicate that indicates if an instance of T2 is selectable.
        /// It is presumed that if it is not selectable, it is in the managed list.</param>
        /// <param name="getdetectortoremovefunc">Meant to be used instead of the property SelectedDetector: function to get the detector 
        /// to remove, based on the selected item T2 from the RemovableDetectors list</param>
        /// <param name="afterdetectoraddedaction">Things to do after adding a detector</param>
        /// <param name="afterdetectorremovedaction">Things to do after removing a detector</param>
        public DetectorManagerViewModel(
            ObservableCollection<T1> detectoren,
            List<T2> alldetectoren,
            Func<T2, T1> getdetectortoaddfunc, 
            Predicate<T2> isdetectorselectable,
            Func<T2, T1> getdetectortoremovefunc = null,
            Action afterdetectoraddedaction = null, 
            Action afterdetectorremovedaction = null)
        {
            DetectorenInCollection = detectoren;
            AllDetectoren = alldetectoren;
            _GetDetectorToAdd = getdetectortoaddfunc;
            _GetDetectorToRemove = getdetectortoremovefunc;
            _IsDetectorSelectable = isdetectorselectable;
            _AfterDetectorAddedAction = afterdetectoraddedaction;
            _AfterDetectorRemovedAction = afterdetectorremovedaction;

            SelectableDetectoren = new ObservableCollection<T2>();
            RemovableDetectoren = new ObservableCollection<T2>();

            Refresh();
        }

        #endregion // Constructor
    }
}
