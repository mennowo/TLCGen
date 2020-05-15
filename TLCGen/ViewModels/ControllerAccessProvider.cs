using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Dependencies.Messaging.Messages;
using TLCGen.Extensions;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public interface IControllerAccessProvider
    {
        ControllerModel Controller { get; }
        ObservableCollection<FaseCyclusViewModel> AllSignalGroups { get; }
        ObservableCollection<IngangViewModel> AllIngangen { get; }
        ObservableCollection<DetectorViewModel> AllDetectors { get; }
        ObservableCollection<DetectorViewModel> AllVecomDetectors { get; }
        ObservableCollection<SelectieveDetectorViewModel> AllSelectiveDetectors { get; }
        ObservableCollection<string> AllSignalGroupStrings { get; }
        ObservableCollection<string> AllDetectorStrings { get; }
        ObservableCollection<string> AllVecomDetectorStrings { get; }
        ObservableCollection<string> AllSelectiveDetectorStrings { get; }

        ICollectionView GetCollectionView(object type);
        void Setup();
    }

    public class ControllerAccessProvider : IControllerAccessProvider
    {
        private readonly IMessenger MessengerInstance;
        private ObservableCollection<IngangViewModel> _allIngangen;
        private ObservableCollection<FaseCyclusViewModel> _allSignalGroups;
        private ObservableCollection<DetectorViewModel> _allDetectors;
        private ObservableCollection<DetectorViewModel> _allVecomDetectors;
        private ObservableCollection<SelectieveDetectorViewModel> _allSelectiveDetectors;
        private ObservableCollection<string> _allSignalGroupStrings;
        private ObservableCollection<string> _allDetectorStrings;
        private ObservableCollection<string> _allVecomDetectorStrings;
        private ObservableCollection<string> _allSelectiveDetectorStrings;

        public static IControllerAccessProvider _default;

        public static IControllerAccessProvider Default => _default ?? (_default = new ControllerAccessProvider(Messenger.Default));

        private Dictionary<object, ICollectionView> _detectorsCollectionViews;

        public ICollectionView GetCollectionView(object type)
        {
            switch (type)
            {
                case DetectorTypeEnum d:
                    if(_allDetectors != null)
                    {
                        if (!_detectorsCollectionViews.ContainsKey(d))
                        {
                            var view = CollectionViewSource.GetDefaultView(_allDetectors);
                            view.Filter += o => ((DetectorViewModel) o).Type == d;
                            view.Refresh();
                            _detectorsCollectionViews.Add(d, view);
                        }
                        return _detectorsCollectionViews[d];
                    }
                    break;
                case SelectieveDetectorTypeEnum sd:
                    if(_allSelectiveDetectors != null)
                    {
                        if (!_detectorsCollectionViews.ContainsKey(sd))
                        {
                            var view = CollectionViewSource.GetDefaultView(_allSelectiveDetectors);
                            view.Filter += o => ((SelectieveDetectorViewModel) o).SdType == sd;
                            view.Refresh();
                            _detectorsCollectionViews.Add(sd, view);
                        }
                        return _detectorsCollectionViews[sd];
                    }
                    break;
                case IngangTypeEnum i:
                    if(_allIngangen != null)
                    {
                        if (!_detectorsCollectionViews.ContainsKey(i))
                        {
                            var view = CollectionViewSource.GetDefaultView(_allIngangen);
                            view.Filter += o => ((IngangViewModel)o).Type == i;
                            view.Refresh();
                            _detectorsCollectionViews.Add(i, view);
                        }
                        return _detectorsCollectionViews[i];
                    }
                    break;
            }

            return null;
        }

        public ObservableCollection<IngangViewModel> AllIngangen =>
            _allIngangen ?? (_allIngangen= new ObservableCollection<IngangViewModel>());

        public ObservableCollection<FaseCyclusViewModel> AllSignalGroups =>
            _allSignalGroups ?? (_allSignalGroups = new ObservableCollection<FaseCyclusViewModel>());
        
        public ObservableCollection<string> AllSignalGroupStrings =>
            _allSignalGroupStrings ?? (_allSignalGroupStrings = new ObservableCollection<string>());

        public ObservableCollection<DetectorViewModel> AllDetectors =>
            _allDetectors ?? (_allDetectors = new ObservableCollection<DetectorViewModel>());

        public ObservableCollection<string> AllDetectorStrings =>
            _allDetectorStrings ?? (_allDetectorStrings = new ObservableCollection<string>());
        
        public ObservableCollection<DetectorViewModel> AllVecomDetectors =>
            _allVecomDetectors ?? (_allVecomDetectors = new ObservableCollection<DetectorViewModel>());

        public ObservableCollection<string> AllVecomDetectorStrings =>
            _allVecomDetectorStrings ?? (_allVecomDetectorStrings = new ObservableCollection<string>());

        public ObservableCollection<SelectieveDetectorViewModel> AllSelectiveDetectors =>
            _allSelectiveDetectors ?? (_allSelectiveDetectors = new ObservableCollection<SelectieveDetectorViewModel>());

        public ObservableCollection<string> AllSelectiveDetectorStrings =>
            _allSelectiveDetectorStrings ?? (_allSelectiveDetectorStrings = new ObservableCollection<string>());

        public ControllerModel Controller { get; private set; }

        public void Setup()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnSignalGroupsChanged);
            MessengerInstance.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            MessengerInstance.Register<SelectieveDetectorenChangedMessage>(this, OnSelectieveDetectorenChanged);
            MessengerInstance.Register<ControllerLoadedMessage>(this, OnControllerLoaded);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            MessengerInstance.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnSignalGroupFaseDetectorTypeChanged));
            MessengerInstance.Register(this, new Action<FaseDetectorVeiligheidsGroenChangedMessage>(OnSignalGroupDetectorVeiligheidsGroenChanged));
        }

        private void OnIngangenChanged(IngangenChangedMessage obj)
        {
            if (obj.RemovedIngangen?.Any() == true)
            {
                foreach (var sg in obj.RemovedIngangen)
                {
                    var vm = AllIngangen.FirstOrDefault(x => x.Naam == sg.Naam);
                    if (vm == null) continue;
                    AllIngangen.Remove(vm);
                }
            }
            if (obj.AddedIngangen?.Any() == true)
            {
                foreach (var sg in obj.AddedIngangen)
                {
                    AllIngangen.Add(new IngangViewModel(sg));
                }
            }
            AllIngangen.BubbleSort();
        }

        private void OnNameChanged(NameChangedMessage obj)
        {
            switch (obj.ObjectType)
            {
                case TLCGenObjectTypeEnum.Fase:
                    AllSignalGroupStrings.Remove(obj.OldName);
                    AllSignalGroupStrings.Add(obj.NewName);
                    AllSignalGroups.BubbleSort();
                    AllSignalGroupStrings.BubbleSort();
                    break;
                case TLCGenObjectTypeEnum.Detector:
                    AllDetectorStrings.Remove(obj.OldName);
                    AllDetectorStrings.Add(obj.NewName);
                    AllDetectors.BubbleSort();
                    AllDetectorStrings.BubbleSort();
                    RefreshVecomDetectors();
                    break;
                case TLCGenObjectTypeEnum.Input:
                    AllIngangen.BubbleSort();
                    break;
                case TLCGenObjectTypeEnum.SelectieveDetector:
                    AllSelectiveDetectorStrings.Remove(obj.OldName);
                    AllSelectiveDetectorStrings.Add(obj.NewName);
                    AllSelectiveDetectorStrings.BubbleSort();
                    break;
            }
            foreach (var detectorsCollectionView in _detectorsCollectionViews)
            {
                detectorsCollectionView.Value.Refresh();
            }
        }

        private void RefreshVecomDetectors()
        {
            AllVecomDetectors.Clear();
            foreach (var d in AllDetectors.Where(x2 => x2.Type == DetectorTypeEnum.VecomDetector))
            {
                AllVecomDetectors.Add(d);
            }
        }

        private bool _fasenChanging = false;
        private void OnSignalGroupsChanged(FasenChangedMessage obj)
        {
            if (_fasenChanging) return;
            
            _fasenChanging = true;

            if (obj.RemovedFasen?.Any() == true)
            {
                foreach (var sg in obj.RemovedFasen)
                {
                    var vm = AllSignalGroups.FirstOrDefault(x => x.Naam == sg.Naam);
                    if (vm == null) continue;
                    AllSignalGroups.Remove(vm);
                    AllSignalGroupStrings.Remove(vm.Naam);
                }
            }
            if (obj.AddedFasen?.Any() == true)
            {
                foreach (var sg in obj.AddedFasen)
                {
                    AllSignalGroups.Add(new FaseCyclusViewModel(sg));
                    AllSignalGroupStrings.Add(sg.Naam);
                }
            }
            AllSignalGroups.BubbleSort();
            AllSignalGroupStrings.BubbleSort();

            _fasenChanging = false;
        }

        
        private void OnSignalGroupFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            foreach (var fcm in AllSignalGroups)
            {
                fcm.UpdateHasKopmax();
            }
            foreach (var detectorsCollectionView in _detectorsCollectionViews)
            {
                detectorsCollectionView.Value.Refresh();
            }
        }

        private void OnSignalGroupDetectorVeiligheidsGroenChanged(FaseDetectorVeiligheidsGroenChangedMessage message)
        {
            foreach (var fcm in AllSignalGroups)
            {
                fcm.UpdateHasVeiligheidsGroen();
            }
        }

        private bool _detChanging = false;
        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
            if (_detChanging) return;

            _detChanging = true;

            if (dmsg.RemovedDetectoren?.Any() == true)
            {
                foreach (var d in dmsg.RemovedDetectoren)
                {
                    var vm = AllDetectors.FirstOrDefault(x => x.Naam == d.Naam);
                    if (vm == null) continue;
                    AllDetectors.Remove(vm);
                    AllDetectorStrings.Remove(vm.Naam);
                }
            }
            if (dmsg.AddedDetectoren?.Any() == true)
            {
                foreach (var d in dmsg.AddedDetectoren)
                {
                    AllDetectors.Add(new DetectorViewModel(d));
                    AllDetectorStrings.Add(d.Naam);
                }
            }
            AllDetectors.BubbleSort();
            AllDetectorStrings.BubbleSort<string>(TLCGenIntegrityChecker.CompareDetectors);
            foreach (var detectorsCollectionView in _detectorsCollectionViews)
            {
                detectorsCollectionView.Value.Refresh();
            }
            RefreshVecomDetectors();

            _detChanging = false;
        }

        private void OnSelectieveDetectorenChanged(SelectieveDetectorenChangedMessage obj)
        {
            AllSelectiveDetectors.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                AllSelectiveDetectors.Add(new SelectieveDetectorViewModel(seld));
            }
        }

        private void OnControllerLoaded(ControllerLoadedMessage obj)
        {
            _allIngangen = null;
            _allSignalGroups = null;
            _allSignalGroupStrings = null;
            _allDetectors = null;
            _allDetectorStrings = null;
            _allSelectiveDetectors = null;
            _allSelectiveDetectorStrings = null;
            _allVecomDetectors = null;
            _allVecomDetectorStrings = null;
            _detectorsCollectionViews = null;

            Controller = obj.Controller;

            if (obj.Controller == null) return;

            _detectorsCollectionViews = new Dictionary<object, ICollectionView>();

            foreach (var sg in obj.Controller.Fasen)
            {
                AllSignalGroups.Add(new FaseCyclusViewModel(sg));
                AllSignalGroupStrings.Add(sg.Naam);
                foreach (var d in sg.Detectoren)
                {
                    AllDetectors.Add(new DetectorViewModel(d) {FaseCyclus = sg.Naam});
                    AllDetectorStrings.Add(d.Naam);
                }
            }

            foreach (var d in obj.Controller.Detectoren)
            {
                AllDetectors.Add(new DetectorViewModel(d));
                AllDetectorStrings.Add(d.Naam);
            }
            foreach (var seld in obj.Controller.SelectieveDetectoren)
            {
                AllSelectiveDetectors.Add(new SelectieveDetectorViewModel(seld));
                AllSelectiveDetectorStrings.Add(seld.Naam);
            }
            foreach (var ingang in obj.Controller.Ingangen)
            {
                AllIngangen.Add(new IngangViewModel(ingang));
            }
        }

        public ControllerAccessProvider(IMessenger messenger)
        {
            MessengerInstance = messenger;
        }
    }
}
