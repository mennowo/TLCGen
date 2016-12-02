using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of GroentijdenSets in the Fasen tab.
    /// </summary>
    public class GroentijdenSetsLijstViewModel : ViewModelBase
    {
        #region Fields

        private ControllerModel _Controller;
        private ObservableCollection<string> _SetNames;
        private ObservableCollection<string> _FasenNames;

        #endregion // Fields

        #region Properties

        public GroentijdViewModel[,] GroentijdenMatrix { get; set; }

        public GroentijdenSetViewModel SelectedSet { get; set; }

        private ObservableCollection<GroentijdenSetViewModel> _GroentijdenSets;
        public ObservableCollection<GroentijdenSetViewModel> GroentijdenSets
        {
            get
            {
                if(_GroentijdenSets == null)
                {
                    _GroentijdenSets = new ObservableCollection<GroentijdenSetViewModel>();
                }
                return _GroentijdenSets;
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
        public ObservableCollection<string> FasenNames
        {
            get
            {
                if(_FasenNames == null)
                {
                    _FasenNames = new ObservableCollection<string>();
                }
                return _FasenNames;
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
            GroentijdenSetModel mgsm = new GroentijdenSetModel();
            switch(_Controller.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.VerlengGroentijden:
                    mgsm.Naam = "VG" + (GroentijdenSets.Count + 1).ToString();
                    break;
                default:
                    mgsm.Naam = "MG" + (GroentijdenSets.Count + 1).ToString();
                    break;

            }
            foreach(FaseCyclusModel fcvm in _Controller.Fasen)
            {
                GroentijdModel mgm = new GroentijdModel();
                mgm.FaseCyclus = fcvm.Define;
                mgm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultGroenTijd(fcvm.Type);
                mgsm.Groentijden.Add(mgm);
            }

            // Create ViewModel around the model, add to list
            GroentijdenSetViewModel mgsvm = new GroentijdenSetViewModel(mgsm);
            GroentijdenSets.Add(mgsvm);

            // Rebuild matrix
            BuildGroentijdenMatrix();
        }

        bool AddNewGroentijdenSetCommand_CanExecute(object prm)
        {
            return GroentijdenSets != null;
        }

        void RemoveGroentijdenSetCommand_Executed(object prm)
        {
            GroentijdenSets.Remove(SelectedSet);
            int i = 1;

            foreach(GroentijdenSetViewModel mgsvm in GroentijdenSets)
            {
                switch (_Controller.Data.TypeGroentijden)
                {
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        mgsvm.Naam = "VG" + i.ToString();
                        break;
                    default:
                        mgsvm.Naam = "MG" + i.ToString();
                        break;

                }
                i++;
            }
            SelectedSet = null;
            BuildGroentijdenMatrix();
        }

        bool RemoveGroentijdenSetCommand_CanExecute(object prm)
        {
            return SelectedSet != null;
        }

        #endregion // Command functionality

        #region Public methods

        public void BuildGroentijdenMatrix()
        {
            if (GroentijdenSets == null || GroentijdenSets.Count == 0)
                return;

            foreach (GroentijdenSetViewModel mgsvm in GroentijdenSets)
            {
#warning CHECK > why is this needed? It's double, only the first one should be necessary.
                mgsvm.GroentijdenSetList.BubbleSort();
                if (!mgsvm.GroentijdenSet.Groentijden.IsSorted())
                {
                    mgsvm.GroentijdenSet.Groentijden.BubbleSort();
                }
            }

            SetNames.Clear();
            FasenNames.Clear();
            
            int fccount = _Controller.Fasen.Count;

            if (fccount == 0 || GroentijdenSets == null || GroentijdenSets.Count == 0)
                return;

            GroentijdenMatrix = new GroentijdViewModel[GroentijdenSets.Count, fccount];
            int i = 0, j = 0;
            foreach(GroentijdenSetViewModel mgsvm in GroentijdenSets)
            {
                SetNames.Add(mgsvm.GroentijdenSet.Naam);
                j = 0;
                foreach (GroentijdViewModel mgvm in mgsvm.GroentijdenSetList)
                {
                    // Build fasen list for row headers from first set
                    if(i == 0)
                    {
                        FasenNames.Add(mgvm.FaseCyclus.Replace(SettingsProvider.Instance.GetFaseCyclusDefinePrefix(), ""));
                    }

                    // set data in bound matrix
                    if (j < fccount)
                        GroentijdenMatrix[i, j] = mgvm;
                    else
                        throw new NotImplementedException();
                    j++;
                }
                i++;
            }
            OnPropertyChanged("SetNames");
            OnPropertyChanged("FasenNames");
            OnPropertyChanged("GroentijdenMatrix");
        }

        #endregion // Public methods

        #region TLCGen events

        void OnFasenChanged(FasenChangedMessage message)
        {
            foreach(FaseCyclusModel fcm in message.AddedFasen)
            {
                foreach (GroentijdenSetViewModel mgsvm in GroentijdenSets)
                {
                    mgsvm.AddFase(fcm.Define, fcm.Naam);
                }
            }
            foreach (FaseCyclusModel fcm in message.RemovedFasen)
            {
                foreach (GroentijdenSetViewModel mgsvm in GroentijdenSets)
                {
                    mgsvm.RemoveFase(fcm.Define);
                }
            }

            BuildGroentijdenMatrix();
        }

        void OnFasenSorted(FasenSortedMessage message)
        {
            BuildGroentijdenMatrix();
        }

        void OnDefineChanged(DefineChangedMessage message)
        {
            foreach(GroentijdenSetViewModel mgsvm in GroentijdenSets)
            {
                foreach(GroentijdViewModel mgvm in mgsvm.GroentijdenSetList)
                {
                    if(mgvm.FaseCyclus == message.OldDefine)
                    {
                        mgvm.FaseCyclus = message.NewDefine;
                    }
                }
            }
        }

        #endregion // TLCGen events

        #region Collection Changed

        /// <summary>
        /// This method is executed when the collection of greentime sets changes
        /// </summary>
        private void GroentijdenSets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
            MessageManager.Instance.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public GroentijdenSetsLijstViewModel(ControllerModel controller)
        {
            _Controller = controller;
            BuildGroentijdenMatrix();

            MessageManager.Instance.Subscribe(this, new Action<FasenChangedMessage>(OnFasenChanged));
            MessageManager.Instance.Subscribe(this, new Action<FasenSortedMessage>(OnFasenSorted));
            MessageManager.Instance.Subscribe(this, new Action<DefineChangedMessage>(OnDefineChanged));

            foreach(GroentijdenSetModel gsm in _Controller.GroentijdenSets)
            {
                GroentijdenSets.Add(new GroentijdenSetViewModel(gsm));
            }

            BuildGroentijdenMatrix();

            GroentijdenSets.CollectionChanged += GroentijdenSets_CollectionChanged;
        }


        #endregion // Constructor
    }
}
