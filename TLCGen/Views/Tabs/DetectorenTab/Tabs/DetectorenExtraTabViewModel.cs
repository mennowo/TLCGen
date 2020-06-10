using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
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

        #endregion // Fields

        #region Properties

        private ObservableCollection<DetectorViewModel> _Detectoren;
        public ObservableCollection<DetectorViewModel> Detectoren
        {
            get
            {
                if(_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<DetectorViewModel>();
                }
                return _Detectoren;
            }
        }

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
                RaisePropertyChanged("SelectedDetector");
                if (value != null) TemplatesProviderVM.SetSelectedApplyToItem(value.Detector);
            }
        }

        public IList SelectedDetectoren
        {
            get => _SelectedDetectoren;
            set
            {
                _SelectedDetectoren = value;
                RaisePropertyChanged("SelectedDetectoren");
                if (value != null)
                {
                    var sl = new List<DetectorModel>();
                    foreach (var s in value)
                    {
                        sl.Add((s as DetectorViewModel).Detector);
                    }
                    TemplatesProviderVM.SetSelectedApplyToItems(sl);
                }
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

        RelayCommand _AddDetectorCommand;
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


        RelayCommand _RemoveDetectorCommand;
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

        void AddDetectorCommand_Executed(object prm)
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
            dm.AanvraagDirect = false; // Not possible / allowed on loose detector
            var dvm1 = new DetectorViewModel(dm);
            Detectoren.Add(dvm1);
            Messenger.Default.Send(new DetectorenChangedMessage(_Controller, new List<DetectorModel> { dm }, null));
            Detectoren.BubbleSort();
        }

        bool AddDetectorCommand_CanExecute(object prm)
        {
            return Detectoren != null;
        }

        void RemoveDetectorCommand_Executed(object prm)
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
            MessengerInstance.Send(new ControllerDataChangedMessage());

            if (changed)
            {
                Messenger.Default.Send(new DetectorenChangedMessage(_Controller, null, remDets));
                Detectoren.BubbleSort();
            }
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
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
            RaisePropertyChanged("");
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
            d.RaisePropertyChanged("");
			Messenger.Default.Send(new DetectorenChangedMessage(_Controller, new List<DetectorModel> { item }, null));
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
            Messenger.Default.Send(new DetectorenExtraListChangedMessage(_Controller.Detectoren));
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorenExtraTabViewModel() : base()
        {
            MessengerInstance.Register<Messaging.Requests.PrepareForGenerationRequest>(this, (msg) =>
            {
                Detectoren.BubbleSort();
            });
            MessengerInstance.Register<Messaging.Messages.NameChangedMessage>(this, (msg) =>
            {
                if(msg.ObjectType == TLCGenObjectTypeEnum.Detector)
                    Detectoren.BubbleSort();
            });
        }

        #endregion // Constructor
    }
}
