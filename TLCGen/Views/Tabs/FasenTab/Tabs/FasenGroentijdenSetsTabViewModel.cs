﻿
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;


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
        private RelayCommand _addGroentijdenSetCommand;
        private RelayCommand _removeGroentijdenSetCommand;
        private RelayCommand _importGroentijdenDataCommand;
        private GroentijdenSetViewModel _selectedSet;

        #endregion // Fields

        #region Properties

        public GroentijdViewModel[,] GroentijdenMatrix { get; set; }

        public GroentijdenSetViewModel SelectedSet
        {
            get => _selectedSet;
            set
            {
                _selectedSet = value; 
                OnPropertyChanged(); 
                _removeGroentijdenSetCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<GroentijdenSetViewModel> GroentijdenSets { get; } = [];

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

        public ICommand AddGroentijdenSetCommand => _addGroentijdenSetCommand ??= new RelayCommand(() =>
            {
                // Build model
                var mgsm = new GroentijdenSetModel();
                switch(_Controller.Data.TypeGroentijden)
                {
                    case GroentijdenTypeEnum.VerlengGroentijden:
                        mgsm.Naam = "VG" + (GroentijdenSets.Count + 1);
                        break;
                    default:
                        mgsm.Naam = "MG" + (GroentijdenSets.Count + 1);
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
                CheckGroentijdenSetsWithDefaultPeriode();
            });

        public ICommand RemoveGroentijdenSetCommand => _removeGroentijdenSetCommand ??= new RelayCommand(() =>
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
                    WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
                }

                GroentijdenSets.Remove(SelectedSet);
                var i = 1;

                foreach(var mgsvm in GroentijdenSets)
                {
                    if (Regex.IsMatch(mgsvm.Naam, @"(M|V)G[0-9]+"))
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
                    }
                    i++;
                }
                SelectedSet = null;

                if (!GroentijdenSets.Any())
                {
                    _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = "";
                }

                BuildGroentijdenMatrix();
                CheckGroentijdenSetsWithDefaultPeriode();
            }, 
            () => SelectedSet != null);

        public ICommand ImportGroentijdenDataCommand => _importGroentijdenDataCommand ??= new RelayCommand(() =>
            {
                var importWindow = new Dialogs.ImportGroentijdenDataWindow(_Controller)
                {
                    Owner = Application.Current.MainWindow
                };
                var result = importWindow.ShowDialog();
                if (result == true)
                {
                    OnPropertyChanged(broadcast: true);
                    BuildGroentijdenMatrix();
                }
            });

        #endregion // Commands

        #region Command functionality

        void AddNewGroentijdenSetCommand_Executed()
        {
            // Build model
            var mgsm = new GroentijdenSetModel();
            switch(_Controller.Data.TypeGroentijden)
            {
                case GroentijdenTypeEnum.VerlengGroentijden:
                    mgsm.Naam = "VG" + (GroentijdenSets.Count + 1);
                    break;
                default:
                    mgsm.Naam = "MG" + (GroentijdenSets.Count + 1);
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
            CheckGroentijdenSetsWithDefaultPeriode();
        }

        bool AddNewGroentijdenSetCommand_CanExecute()
        {
            return GroentijdenSets != null;
        }

        void RemoveGroentijdenSetCommand_Executed()
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
WeakReferenceMessengerEx.Default.Send(new PeriodenChangedMessage());
            }

            GroentijdenSets.Remove(SelectedSet);
            var i = 1;

            foreach(var mgsvm in GroentijdenSets)
            {
                if (Regex.IsMatch(mgsvm.Naam, @"(M|V)G[0-9]+"))
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
                }
                i++;
            }
            SelectedSet = null;

	        if (!GroentijdenSets.Any())
	        {
				_Controller.PeriodenData.DefaultPeriodeGroentijdenSet = "";
			}

            BuildGroentijdenMatrix();
            CheckGroentijdenSetsWithDefaultPeriode();
        }

        bool RemoveGroentijdenSetCommand_CanExecute()
        {
            return SelectedSet != null;
        }

        void ImportGroentijdenDataCommand_Executed()
        {
            var importWindow = new Dialogs.ImportGroentijdenDataWindow(_Controller)
            {
                Owner = Application.Current.MainWindow
            };
            var result = importWindow.ShowDialog();
            if (result == true)
            {
                OnPropertyChanged(broadcast: true);
                BuildGroentijdenMatrix();
            }
        }

        bool ImportGroentijdenDataCommand_CanExecute()
        {
            return true;
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
				OnPropertyChanged(nameof(SetNames));
	            OnPropertyChanged(nameof(FasenNames));
	            OnPropertyChanged(nameof(GroentijdenMatrix));
			}

            foreach (var mgsvm in GroentijdenSets)
            {
                mgsvm.Groentijden.BubbleSort();
                mgsvm.GroentijdenSet.Groentijden.BubbleSort();
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
            OnPropertyChanged(nameof(SetNames));
            OnPropertyChanged(nameof(FasenNames));
            OnPropertyChanged(nameof(GroentijdenMatrix));
        }

        private void CheckGroentijdenSetsWithDefaultPeriode()
        {
            if (FasenNames.Count == 0 || GroentijdenSets.Count == 0) return;

            // check if any fase has no greentime for some set and does have them for others
            var ok = true;
            foreach (var sg in FasenNames)
            {
                // check if the fase has any greentime
                if (GroentijdenSets.Any(x => x.Groentijden.Any(x2 => x2.FaseCyclus == sg && x2.Waarde.HasValue)))
                {
                    // now check if there is a set that does not have this fase
                    foreach (var set in GroentijdenSets)
                    {
                        if (set.Groentijden.Any(x => x.FaseCyclus == sg && !x.Waarde.HasValue))
                        {
                            ok = false;
                            break;
                        }
                    }
                }
                if (!ok) break;
            }

            if (!ok)
            {
                var okgts = new List<GroentijdenSetViewModel>();
                foreach (var set in GroentijdenSets)
                {
                    var setok = true;
                    foreach (var gt in set.Groentijden)
                    {
                        if (!gt.Groentijd.Waarde.HasValue)
                        {
                            foreach (var set2 in GroentijdenSets)
                            {
                                if (ReferenceEquals(set, set2)) continue;

                                if (set2.Groentijden.Any(x => x.FaseCyclus == gt.FaseCyclus && x.Waarde.HasValue))
                                {
                                    setok = false;
                                    break;
                                }
                            }
                        }
                        if (setok == false) break;
                    }
                    if (setok) okgts.Add(set);
                }
                if (okgts.All(x => x.Naam != Controller.PeriodenData.DefaultPeriodeGroentijdenSet))
                {
                    Controller.PeriodenData.DefaultPeriodeGroentijdenSet = okgts.FirstOrDefault()?.Naam;
                    if (Controller.PeriodenData.DefaultPeriodeGroentijdenSet != null)
                    {
                        MessageBox.Show("Groentijdenset voor default periode aangepast: " + Controller.PeriodenData.DefaultPeriodeGroentijdenSet, "Groentijdenset default periode aangepast");
                    }
                    else
                    {
                        MessageBox.Show("Groentijdenset voor default periode kon niet op een geldige waarde worden ingesteld.\nControleer de groentijdensets!", "Groentijdenset default periode aangepast");
                    }
                }
            }
        }

        #endregion // Private methods

        #region TabItem Overrides

        public override string DisplayName => _Controller?.Data?.TypeGroentijden == GroentijdenTypeEnum.MaxGroentijden ? "Maxgroen" : "Verlenggroen";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {

        }

        public override ControllerModel Controller
        {
            get => base.Controller;

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

        public void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            foreach (var set in GroentijdenSets)
            {
                set.Groentijden.Rebuild();
            }
            BuildGroentijdenMatrix();
        }

        public void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            BuildGroentijdenMatrix();
        }

        public void OnNameChanged(object sender, NameChangedMessage message)
        {
            BuildGroentijdenMatrix();
        }

        public void OnGroentijdenTypeChanged(object sender, GroentijdenTypeChangedMessage message)
        {
            OnPropertyChanged(nameof(DisplayName));
            var isdef = false;
            foreach (var setvm in GroentijdenSets)
            {
                if(_Controller.PeriodenData.DefaultPeriodeGroentijdenSet == setvm.Naam)
                {
                    isdef = true;
                }
                setvm.Type = message.Type;
                if (isdef)
                {
                    _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = setvm.Naam;
                    isdef = false;
                }
            }
            BuildGroentijdenMatrix();
        }

        private void OnGroentijdChanged(object sender, GroentijdChangedMessage msg)
        {
            CheckGroentijdenSetsWithDefaultPeriode();
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
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public FasenGroentijdenSetsTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<GroentijdenTypeChangedMessage>(this, OnGroentijdenTypeChanged);
            WeakReferenceMessengerEx.Default.Register<GroentijdChangedMessage>(this, OnGroentijdChanged);
        }


        #endregion // Constructor
    }
}
