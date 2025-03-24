using System;
using System.Collections;
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
using TLCGen.Messaging.Requests;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// ViewModel for the list of extra detectors, not belonging to a PhaseCyclus
    /// </summary>
    [TLCGenTabItem(index: 10, type: TabItemTypeEnum.DetectieTab)]
    public class DetectorenExtraTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<DetectorModel>
    {
        #region Fields
        
        private ObservableCollection<string> _Templates;
        private DetectorViewModel _SelectedDetector;
        private IList _SelectedDetectoren = new ArrayList();
        private RelayCommand _AddDetectorCommand;
        private RelayCommand _RemoveDetectorCommand;

        #endregion // Fields

        #region Properties

        public ObservableCollection<DetectorViewModel> Detectoren { get; } = [];

        public ObservableCollection<string> Templates
        {
            get
            {
                if (_Templates == null)
                {
                    _Templates = new ObservableCollection<string>();
                    _Templates.Add("Template placeholder");
                }
                return _Templates;
            }
        }

        public DetectorViewModel SelectedDetector
        {
            get => _SelectedDetector;
            set
            {
                _SelectedDetector = value;
                OnPropertyChanged("SelectedDetector");
                if (value != null) TemplatesProviderVM.SetSelectedApplyToItem(value.Detector);
                _RemoveDetectorCommand?.NotifyCanExecuteChanged();
            }
        }

        public IList SelectedDetectoren
        {
            get => _SelectedDetectoren;
            set
            {
                _SelectedDetectoren = value;
                OnPropertyChanged("SelectedDetectoren");
                if (value != null)
                {
                    var sl = new List<DetectorModel>();
                    foreach (var s in value)
                    {
                        sl.Add((s as DetectorViewModel).Detector);
                    }
                    TemplatesProviderVM.SetSelectedApplyToItems(sl);
                }
                _RemoveDetectorCommand?.NotifyCanExecuteChanged();
            }
        }

        private TemplateProviderViewModel<TLCGenTemplateModel<DetectorModel>, DetectorModel> _TemplatesProviderVM;
        public TemplateProviderViewModel<TLCGenTemplateModel<DetectorModel>, DetectorModel> TemplatesProviderVM
        {
            get
            {
                if (_TemplatesProviderVM == null)
                {
                    _TemplatesProviderVM = new TemplateProviderViewModel<TLCGenTemplateModel<DetectorModel>, DetectorModel>(this, false);
                }
                return _TemplatesProviderVM;
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand AddDetectorCommand => _AddDetectorCommand ??= new RelayCommand(() =>
            {
                var dm = new DetectorModel();
                var newname = "001";
                var inewname = 1;
                foreach (var dvm in Detectoren)
                {
                    if (Regex.IsMatch(dvm.Naam, @"[0-9]+"))
                    {
                        var m = Regex.Match(dvm.Naam, @"[0-9]+");
                        var next = m.Value;
                        if (Int32.TryParse(next, out inewname))
                        {
                            newname = inewname.ToString("000");
                            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, newname))
                            {
                                inewname++;
                                newname = inewname.ToString("000");
                            }
                        }
                    }
                }
                dm.Naam = newname;
                DefaultsProvider.Default.SetDefaultsOnModel(dm, dm.Type.ToString());
                dm.AanvraagDirectSch = NooitAltijdAanUitEnum.Nooit; // Not possible / allowed on loose detector
                var dvm1 = new DetectorViewModel(dm);
                Detectoren.Add(dvm1);
                WeakReferenceMessengerEx.Default.Send(new DetectorenChangedMessage(_Controller, new List<DetectorModel>
                {
                    dm
                }, null));
                Detectoren.BubbleSort();
            });

        public ICommand RemoveDetectorCommand => _RemoveDetectorCommand ??= new RelayCommand(() =>
            {
                var changed = false;
                var remDets = new List<DetectorModel>();
                if (SelectedDetectoren != null && SelectedDetectoren.Count > 0)
                {
                    changed = true;
                    foreach (DetectorViewModel dvm in SelectedDetectoren)
                    {
                        remDets.Add(dvm.Detector);
                        Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(dvm.Naam, TLCGenObjectTypeEnum.Detector);
                    }
                }
                else if (SelectedDetector != null)
                {
                    changed = true;
                    remDets.Add(SelectedDetector.Detector);
                    Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedDetector.Naam, TLCGenObjectTypeEnum.Detector);
                }
                RebuildDetectorenList();
                WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());

                if (changed)
                {
                    WeakReferenceMessengerEx.Default.Send(new DetectorenChangedMessage(_Controller, null, remDets));
                    Detectoren.BubbleSort();
                }
            }, 
            () => SelectedDetector != null || SelectedDetectoren != null && SelectedDetectoren.Count > 0);

        #endregion // Commands

        #region Command functionality

        void AddDetectorCommand_Executed()
        {
            var dm = new DetectorModel();
            var newname = "001";
            var inewname = 1;
            foreach (var dvm in Detectoren)
            {
                if (Regex.IsMatch(dvm.Naam, @"[0-9]+"))
                {
                    var m = Regex.Match(dvm.Naam, @"[0-9]+");
                    var next = m.Value;
                    if (Int32.TryParse(next, out inewname))
                    {
                        newname = inewname.ToString("000");
                        while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Detector, newname))
                        {
                            inewname++;
                            newname = inewname.ToString("000");
                        }
                    }
                }
            }
            dm.Naam = newname;
            DefaultsProvider.Default.SetDefaultsOnModel(dm, dm.Type.ToString());
            dm.AanvraagDirectSch = NooitAltijdAanUitEnum.Nooit; // Not possible / allowed on loose detector
            var dvm1 = new DetectorViewModel(dm);
            Detectoren.Add(dvm1);
            WeakReferenceMessengerEx.Default.Send(new DetectorenChangedMessage(_Controller, new List<DetectorModel> { dm }, null));
            Detectoren.BubbleSort();
        }

        bool AddDetectorCommand_CanExecute()
        {
            return Detectoren != null;
        }

        void RemoveDetectorCommand_Executed()
        {
            var changed = false;
            var remDets = new List<DetectorModel>();
            if (SelectedDetectoren != null && SelectedDetectoren.Count > 0)
            {
                changed = true;
                foreach (DetectorViewModel dvm in SelectedDetectoren)
                {
                    remDets.Add(dvm.Detector);
                    Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(dvm.Naam, TLCGenObjectTypeEnum.Detector);
                }
            }
            else if (SelectedDetector != null)
            {
                changed = true;
                remDets.Add(SelectedDetector.Detector);
                Integrity.TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedDetector.Naam, TLCGenObjectTypeEnum.Detector);
            }
            RebuildDetectorenList();
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());

            if (changed)
            {
                WeakReferenceMessengerEx.Default.Send(new DetectorenChangedMessage(_Controller, null, remDets));
                Detectoren.BubbleSort();
            }
        }

        bool RemoveDetectorCommand_CanExecute()
        {
            return Detectoren != null &&
                (SelectedDetector != null ||
                 SelectedDetectoren != null && SelectedDetectoren.Count > 0);
        }

        #endregion // Command functionality

        #region Private Methods

        private void RebuildDetectorenList()
        {
            Detectoren.CollectionChanged -= Detectoren_CollectionChanged;
            Detectoren.Clear();
            foreach (var dm in base.Controller.Detectoren)
            {
                var dvm = new DetectorViewModel(dm);
                dvm.PropertyChanged += Detector_PropertyChanged;
                Detectoren.Add(dvm);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;
            OnPropertyChanged("");
        }

        #endregion // Private Methods

        #region TabItem Overrides

        public override string DisplayName => "Extra detectie";

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
            this.Detectoren.BubbleSort();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    RebuildDetectorenList();
                }
                else
                {
                    Detectoren.CollectionChanged -= Detectoren_CollectionChanged;
                    Detectoren.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<DetectorModel> items)
        {
            foreach (var d in items)
            {
                if (!Integrity.TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, d.Naam, TLCGenObjectTypeEnum.Detector))
                {
                    MessageBox.Show("Error bij toevoegen van detector met naam " + d.Naam + ".\nDe detector naam is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
            foreach (var d in items)
            {
                Detectoren.Add(new DetectorViewModel(d));
            }
        }

        public void UpdateAfterApplyTemplate(DetectorModel item)
        {
            var d = Detectoren.First(x => x.Detector == item);
            d.OnPropertyChanged("");
			WeakReferenceMessengerEx.Default.Send(new DetectorenChangedMessage(_Controller, new List<DetectorModel> { item }, null));
        }

        #endregion // IAllowTemplates

        #region Event handling

        private bool _SettingMultiple = false;
        private void Detector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_SettingMultiple || string.IsNullOrEmpty(e.PropertyName))
                return;

            if (SelectedDetectoren != null && SelectedDetectoren.Count > 1)
            {
                _SettingMultiple = true;
                MultiPropertySetter.SetPropertyForAllItems<DetectorViewModel>(sender, e.PropertyName, SelectedDetectoren);
            }
            _SettingMultiple = false;
        }

        #endregion // Event handling

        #region Collection Changed

        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.NewItems)
                {
                    dvm.PropertyChanged += Detector_PropertyChanged;
                    _Controller.Detectoren.Add(dvm.Detector);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorViewModel dvm in e.OldItems)
                {
                    _Controller.Detectoren.Remove(dvm.Detector);
                }
            };
WeakReferenceMessengerEx.Default.Send(new DetectorenExtraListChangedMessage(_Controller.Detectoren));
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorenExtraTabViewModel() : base()
        {
            WeakReferenceMessengerEx.Default.Register<Messaging.Requests.PrepareForGenerationRequest>(this, OnPrepareForGenerationRequest);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChangedMessage);
        }

        private void OnPrepareForGenerationRequest(object recipient, PrepareForGenerationRequest message)
        {
             Detectoren.BubbleSort();
        }

        private void OnNameChangedMessage(object recipient, NameChangedMessage message)
        {
            if (message.ObjectType == TLCGenObjectTypeEnum.Detector) Detectoren.BubbleSort();
        }

        #endregion // Constructor
    }
}
