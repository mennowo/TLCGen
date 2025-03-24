
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of extra detectors, not belonging to a PhaseCyclus
    /// </summary>
    [TLCGenTabItem(index: 60, type: TabItemTypeEnum.DetectieTab)]
    public class SelectieveDetectorenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields
        
        private SelectieveDetectorViewModel _SelectedSelectieveDetector;
        private IList _SelectedSelectieveDetectoren = new ArrayList();
        private RelayCommand _AddSelectieveDetectorCommand;
        private RelayCommand _RemoveSelectieveDetectorCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<SelectieveDetectorViewModel> SelectieveDetectoren { get; } = [];

        public SelectieveDetectorViewModel SelectedSelectieveDetector
        {
            get => _SelectedSelectieveDetector;
            set
            {
                _SelectedSelectieveDetector = value;
                OnPropertyChanged();
                _RemoveSelectieveDetectorCommand?.NotifyCanExecuteChanged();
            }
        }

        public IList SelectedSelectieveDetectoren
        {
            get => _SelectedSelectieveDetectoren;
            set
            {
                _SelectedSelectieveDetectoren = value;
                OnPropertyChanged();
                if (value != null)
                {
                    var sl = new List<SelectieveDetectorModel>();
                    foreach (var s in value)
                    {
                        sl.Add((s as SelectieveDetectorViewModel).SelectieveDetector);
                    }
                }
                _RemoveSelectieveDetectorCommand?.NotifyCanExecuteChanged();
            }
        }
        
        #endregion // Properties

        #region Commands

        public ICommand AddSelectieveDetectorCommand => _AddSelectieveDetectorCommand ??= new RelayCommand(() =>
            {
                var dm = new SelectieveDetectorModel();
                var inewname = SelectieveDetectoren.Count + 1;
                var newname = "s" + inewname.ToString("000");
                while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.SelectieveDetector, newname))
                {
                    inewname++;
                    newname = "s" + inewname.ToString("000");
                }
                dm.Naam = newname;
                var dvm1 = new SelectieveDetectorViewModel(dm);
                SelectieveDetectoren.Add(dvm1);
                WeakReferenceMessengerEx.Default.Send(new SelectieveDetectorenChangedMessage(new List<SelectieveDetectorModel>
                {
                    dm
                }, null));
                SelectedSelectieveDetectoren.BubbleSort();
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
            });

        public ICommand RemoveSelectieveDetectorCommand => _RemoveSelectieveDetectorCommand ??= new RelayCommand(() =>
            {
                var changed = false;
                var removed = new List<SelectieveDetectorModel>();
                if (SelectedSelectieveDetectoren != null && SelectedSelectieveDetectoren.Count > 0)
                {
                    changed = true;
                    foreach (SelectieveDetectorViewModel ivm in SelectedSelectieveDetectoren)
                    {
                        removed.Add(ivm.SelectieveDetector);
                        Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(ivm.Naam, TLCGenObjectTypeEnum.SelectieveDetector);
                    }
                }
                else if (SelectedSelectieveDetector != null)
                {
                    changed = true;
                    removed.Add(SelectedSelectieveDetector.SelectieveDetector);
                    Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedSelectieveDetector.Naam, TLCGenObjectTypeEnum.SelectieveDetector);
                }
                RebuildSelectieveDetectorenList();

                if (changed)
                {
                    WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                    WeakReferenceMessengerEx.Default.Send(new SelectieveDetectorenChangedMessage(null, removed));
                }
            }, 
            () => SelectedSelectieveDetector != null || SelectedSelectieveDetectoren is { Count: > 0 });

        #endregion // Commands

        #region Command functionality

        void AddSelectieveDetectorCommand_Executed()
        {
            var dm = new SelectieveDetectorModel();
            var inewname = SelectieveDetectoren.Count + 1;
            var newname = "s" + inewname.ToString("000");
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.SelectieveDetector, newname))
            {
                inewname++;
                newname = "s" + inewname.ToString("000");
            }
            dm.Naam = newname;
            var dvm1 = new SelectieveDetectorViewModel(dm);
            SelectieveDetectoren.Add(dvm1);
            WeakReferenceMessengerEx.Default.Send(new SelectieveDetectorenChangedMessage(new List<SelectieveDetectorModel>{dm}, null));
            SelectedSelectieveDetectoren.BubbleSort();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        bool AddSelectieveDetectorCommand_CanExecute()
        {
            return SelectieveDetectoren != null;
        }

        void RemoveSelectieveDetectorCommand_Executed()
        {
            var changed = false;
            var removed = new List<SelectieveDetectorModel>();
            if (SelectedSelectieveDetectoren != null && SelectedSelectieveDetectoren.Count > 0)
            {
                changed = true;
                foreach (SelectieveDetectorViewModel ivm in SelectedSelectieveDetectoren)
                {
                    removed.Add(ivm.SelectieveDetector);
                    Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(ivm.Naam, TLCGenObjectTypeEnum.SelectieveDetector);
                }
            }
            else if (SelectedSelectieveDetector != null)
            {
                changed = true;
                removed.Add(SelectedSelectieveDetector.SelectieveDetector);
                Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedSelectieveDetector.Naam, TLCGenObjectTypeEnum.SelectieveDetector);
            }
            RebuildSelectieveDetectorenList();

            if (changed)
            {
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
                WeakReferenceMessengerEx.Default.Send(new SelectieveDetectorenChangedMessage(null, removed));
            }
        }

        bool RemoveSelectieveDetectorCommand_CanExecute()
        {
            return SelectieveDetectoren != null &&
                (SelectedSelectieveDetector != null ||
                 SelectedSelectieveDetectoren != null && SelectedSelectieveDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region Private Methods

        private void RebuildSelectieveDetectorenList()
        {
            SelectieveDetectoren.CollectionChanged -= SelectieveDetectoren_CollectionChanged;
            SelectieveDetectoren.Clear();
            foreach (var dm in base.Controller.SelectieveDetectoren)
            {
                var dvm = new SelectieveDetectorViewModel(dm);
                dvm.PropertyChanged += SelectieveDetector_PropertyChanged;
                SelectieveDetectoren.Add(dvm);
            }
            SelectieveDetectoren.CollectionChanged += SelectieveDetectoren_CollectionChanged;
            OnPropertyChanged("");
        }

        #endregion // Private Methods

        #region TabItem Overrides

        public override string DisplayName => "Selectieve\ndetectie";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
        }

        public override void OnDeselected()
        {
            this.SelectieveDetectoren.BubbleSort();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    RebuildSelectieveDetectorenList();
                }
                else
                {
                    SelectieveDetectoren.CollectionChanged -= SelectieveDetectoren_CollectionChanged;
                    SelectieveDetectoren.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        #region Event handling

        private bool _SettingMultiple = false;
        private void SelectieveDetector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedSelectieveDetectoren != null && SelectedSelectieveDetectoren.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<SelectieveDetectorViewModel>(sender, e.PropertyName, SelectedSelectieveDetectoren);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void SelectieveDetectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (SelectieveDetectorViewModel dvm in e.NewItems)
                {
                    dvm.PropertyChanged += SelectieveDetector_PropertyChanged;
                    _Controller.SelectieveDetectoren.Add(dvm.SelectieveDetector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (SelectieveDetectorViewModel dvm in e.OldItems)
                {
                    _Controller.SelectieveDetectoren.Remove(dvm.SelectieveDetector);
                }
            };
        }

        #endregion // Collection Changed

        #region Constructor

        public SelectieveDetectorenTabViewModel() : base()
        {
        }

        #endregion // Constructor
    }
}
