using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of GroentijdenSets in the Fasen tab.
    /// </summary>
    [TLCGenTabItem(index: 4, type: TabItemTypeEnum.FasenTab)]
    public class FasenGroentijdenSetsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
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
            var mgsm = new GroentijdenSetModel();
            switch(_Controller.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.VerlengGroentijden:
                    mgsm.Naam = "VG" + (GroentijdenSets.Count + 1).ToString();
                    break;
                default:
                    mgsm.Naam = "MG" + (GroentijdenSets.Count + 1).ToString();
                    break;

            }
            foreach(var fcvm in _Controller.Fasen)
            {
                var mgm = new GroentijdModel();
                mgm.FaseCyclus = fcvm.Naam;
                mgm.Waarde = Settings.Utilities.FaseCyclusUtilities.GetFaseDefaultGroenTijd(fcvm.Type);
                mgsm.Groentijden.Add(mgm);
            }

            // Create ViewModel around the model, add to list
            var mgsvm = new GroentijdenSetViewModel(mgsm);
            GroentijdenSets.Add(mgsvm);

	        if (string.IsNullOrWhiteSpace(_Controller.PeriodenData.DefaultPeriodeGroentijdenSet))
	        {
		        _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = mgsm.Naam;
	        }

            // Rebuild matrix
            BuildGroentijdenMatrix();
        }

        bool AddNewGroentijdenSetCommand_CanExecute(object prm)
        {
            return GroentijdenSets != null;
        }

        void RemoveGroentijdenSetCommand_Executed(object prm)
        {
            var changed = false;
            foreach(var p in _Controller.PeriodenData.Perioden)
            {
                if(p.Type == PeriodeTypeEnum.Groentijden && GroentijdenSets.All(x => p.GroentijdenSet != x.Naam))
                {
                    p.GroentijdenSet = null;
                    changed = true;
                }
            }
            if(_Controller.PeriodenData.DefaultPeriodeGroentijdenSet == SelectedSet.Naam)
            {
                if(_Controller.GroentijdenSets.Count > 0)
                {
                    _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = _Controller.GroentijdenSets[0].Naam;
                }
                else
                {
                    _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = null;
                }
                changed = true;
            }
            if(changed)
            {
                Messenger.Default.Send(new PeriodenChangedMessage());
            }

            GroentijdenSets.Remove(SelectedSet);
            var i = 1;

            foreach(var mgsvm in GroentijdenSets)
            {
                switch (_Controller.Data.TypeGroentijden)
                {
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        mgsvm.Naam = "VG" + i;
                        break;
                    default:
                        mgsvm.Naam = "MG" + i;
                        break;

                }
                i++;
            }
            SelectedSet = null;

	        if (!GroentijdenSets.Any())
	        {
				_Controller.PeriodenData.DefaultPeriodeGroentijdenSet = "";
			}

            BuildGroentijdenMatrix();
        }

        bool RemoveGroentijdenSetCommand_CanExecute(object prm)
        {
            return SelectedSet != null;
        }

        #endregion // Command functionality

        #region Public methods

        #endregion // Public methods

        #region Private methods

        private void BuildGroentijdenMatrix()
        {
            if (GroentijdenSets == null || GroentijdenSets.Count == 0)
            {
                SetNames.Clear();
                FasenNames.Clear();
                GroentijdenMatrix = new GroentijdViewModel[0,0];
				RaisePropertyChanged(nameof(SetNames));
	            RaisePropertyChanged(nameof(FasenNames));
	            RaisePropertyChanged(nameof(GroentijdenMatrix));
			}

            foreach (var mgsvm in GroentijdenSets)
            {
#warning CHECK > why is this needed? It's double, only the first one should be necessary.
                mgsvm.Groentijden.BubbleSort();
                if (!mgsvm.GroentijdenSet.Groentijden.IsSorted())
                {
                    mgsvm.GroentijdenSet.Groentijden.BubbleSort();
                }
            }

            SetNames.Clear();
            FasenNames.Clear();

            var fccount = _Controller.Fasen.Count;

            if (fccount == 0 || GroentijdenSets == null || GroentijdenSets.Count == 0)
                return;

            GroentijdenMatrix = new GroentijdViewModel[GroentijdenSets.Count, fccount];
            int i = 0, j = 0;
            foreach (var mgsvm in GroentijdenSets)
            {
                SetNames.Add(mgsvm.GroentijdenSet.Naam);
                j = 0;
                foreach (var mgvm in mgsvm.Groentijden)
                {
                    // Build fasen list for row headers from first set
                    if (i == 0)
                    {
                        FasenNames.Add(mgvm.FaseCyclus);
                    }

                    // set data in bound matrix
                    if (j < fccount)
                        GroentijdenMatrix[i, j] = mgvm;
                    else
                        throw new IndexOutOfRangeException();
                    j++;
                }
                i++;
            }
            RaisePropertyChanged(nameof(SetNames));
            RaisePropertyChanged(nameof(FasenNames));
            RaisePropertyChanged(nameof(GroentijdenMatrix));
        }

        #endregion // Private methods

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return _Controller?.Data?.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "Maxgroen" : "Verlenggroen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {

        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    GroentijdenSets.CollectionChanged -= GroentijdenSets_CollectionChanged;
                    GroentijdenSets.Clear();
                    foreach (var gsm in base.Controller.GroentijdenSets)
                    {
                        GroentijdenSets.Add(new GroentijdenSetViewModel(gsm));
                    }
                    BuildGroentijdenMatrix();
                    GroentijdenSets.CollectionChanged += GroentijdenSets_CollectionChanged;
                }
                else
                {
                    GroentijdenSets.CollectionChanged -= GroentijdenSets_CollectionChanged;
                    GroentijdenSets.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen events

        public void OnFasenChanged(FasenChangedMessage message)
        {
            if (message.AddedFasen != null)
            {
                foreach (var fcm in message.AddedFasen)
                {
                    foreach (var set in Controller.GroentijdenSets)
                    {
                        var mgm = new GroentijdModel {FaseCyclus = fcm.Naam};
                        DefaultsProvider.Default.SetDefaultsOnModel(mgm, fcm.Type.ToString());
                        set.Groentijden.Add(mgm);
                    }
                }
            }
            if (message.RemovedFasen != null)
            {
                foreach (var fcm in message.RemovedFasen)
                {
                    foreach (var set in Controller.GroentijdenSets)
                    {
                        GroentijdModel mgm = null;
                        foreach (var mgvm in set.Groentijden)
                        {
                            if (mgvm.FaseCyclus == fcm.Naam)
                            {
                                mgm = mgvm;
                            }
                        }
                        if (mgm != null)
                        {
                            set.Groentijden.Remove(mgm);
                        }
                    }
                }
            }

            foreach (var set in GroentijdenSets)
            {
                set.Groentijden.Rebuild();
            }
            BuildGroentijdenMatrix();
        }

        public void OnFasenSorted(FasenSortedMessage message)
        {
            BuildGroentijdenMatrix();
        }

        public void OnNameChanged(NameChangedMessage message)
        {
            foreach(var mgsvm in GroentijdenSets)
            {
                foreach(var mgvm in mgsvm.Groentijden)
                {
                    if(mgvm.FaseCyclus == message.OldName)
                    {
                        mgvm.FaseCyclus = message.NewName;
                    }
                }
            }
            BuildGroentijdenMatrix();
        }

        public void OnGroentijdenTypeChanged(GroentijdenTypeChangedMessage message)
        {
            RaisePropertyChanged(nameof(DisplayName));
            foreach (var setvm in GroentijdenSets)
            {
                setvm.Type = message.Type;
            }
            BuildGroentijdenMatrix();
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
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public FasenGroentijdenSetsTabViewModel()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
            Messenger.Default.Register(this, new Action<GroentijdenTypeChangedMessage>(OnGroentijdenTypeChanged));
        }


        #endregion // Constructor
    }
}
