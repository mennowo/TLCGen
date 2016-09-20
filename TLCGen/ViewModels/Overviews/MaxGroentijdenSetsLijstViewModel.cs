using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class MaxGroentijdenSetsLijstViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ObservableCollection<string> _SetNames;

        #endregion // Fields

        #region Properties

        public MaxGroentijdViewModel[,] MaxGroenMatrix { get; set; }

        public MaxGroentijdenSetViewModel SelectedSet { get; set; }

        public ObservableCollection<MaxGroentijdenSetViewModel> MaxGroentijdenSets
        {
            get
            {
                return _ControllerVM.MaxGroentijdenSets;
            }
        }

        public ObservableCollection<string> SetNames
        {
            get
            {
                if (_SetNames == null)
                {
                    _SetNames = new ObservableCollection<string>();
                }
                return _SetNames;
            }
        }

        /// <summary>
        /// Collection of strings used to display row headers
        /// </summary>
        public ObservableCollection<FaseCyclusViewModel> FasenNames
        {
            get
            {
                return _ControllerVM.Fasen;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddGroentijdenSetCommand;
        public ICommand AddGroentijdenSetCommand
        {
            get
            {
                if (_AddGroentijdenSetCommand == null)
                {
                    _AddGroentijdenSetCommand = new RelayCommand(AddNewGroentijdenSetCommand_Executed, AddNewGroentijdenSetCommand_CanExecute);
                }
                return _AddGroentijdenSetCommand;
            }
        }


        RelayCommand _RemoveGroentijdenSetCommand;
        public ICommand RemoveGroentijdenSetCommand
        {
            get
            {
                if (_RemoveGroentijdenSetCommand == null)
                {
                    _RemoveGroentijdenSetCommand = new RelayCommand(RemoveGroentijdenSetCommand_Executed, RemoveGroentijdenSetCommand_CanExecute);
                }
                return _RemoveGroentijdenSetCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        void AddNewGroentijdenSetCommand_Executed(object prm)
        {
            // Build model
            MaxGroentijdenSetModel mgsm = new MaxGroentijdenSetModel();
            mgsm.Naam = "MG" + (MaxGroentijdenSets.Count + 1).ToString();
            foreach(FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
            {
                MaxGroentijdModel mgm = new MaxGroentijdModel();
                mgm.FaseCyclus = fcvm.Define;
                mgm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultMaxGroenTijd(fcvm.Type);
                mgsm.MaxGroentijden.Add(mgm);
            }

            // Create ViewModel around the model, add to list
            MaxGroentijdenSetViewModel mgsvm = new MaxGroentijdenSetViewModel(_ControllerVM, mgsm);
            MaxGroentijdenSets.Add(mgsvm);

            // Rebuild matrix
            BuildMaxGroenMatrix();
        }

        bool AddNewGroentijdenSetCommand_CanExecute(object prm)
        {
            return MaxGroentijdenSets != null;
        }

        void RemoveGroentijdenSetCommand_Executed(object prm)
        {
            MaxGroentijdenSets.Remove(SelectedSet);
            int i = 1;
            foreach(MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
            {
                mgsvm.Naam = "MG" + i.ToString();
                i++;
            }
            BuildMaxGroenMatrix();
        }

        bool RemoveGroentijdenSetCommand_CanExecute(object prm)
        {
            return SelectedSet != null;
        }

        #endregion // Command functionality

        #region Public methods

        public void BuildMaxGroenMatrix()
        {
            foreach (MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
            {
                mgsvm.MaxGroentijdenSetList.BubbleSort();
            }

            _SetNames = new ObservableCollection<string>();

            int fccount = _ControllerVM.Fasen.Count;
            MaxGroenMatrix = new MaxGroentijdViewModel[MaxGroentijdenSets.Count, fccount];
            int i = 0, j = 0;
            foreach(MaxGroentijdenSetViewModel mgsvm in MaxGroentijdenSets)
            {
                SetNames.Add(mgsvm.MaxGroentijdenSet.Naam);
                j = 0;
                foreach (MaxGroentijdViewModel mgvm in mgsvm.MaxGroentijdenSetList)
                {
                    if (j < fccount)
                        MaxGroenMatrix[i, j] = mgvm;
                    else
                        throw new NotImplementedException();
                    j++;
                }
                i++;
            }
            OnPropertyChanged("SetNames");
            OnPropertyChanged("FasenNames");
            OnPropertyChanged("MaxGroenMatrix");
        }

        #endregion // Public methods

        #region Constructor

        public MaxGroentijdenSetsLijstViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
            BuildMaxGroenMatrix();
        }

        #endregion // Constructor
    }
}
