using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        ObservableCollection<DetectorViewModel> AllDetectors { get; }
        ObservableCollection<DetectorViewModel> AllVecomDetectors { get; }
        ObservableCollection<SelectieveDetectorViewModel> AllSelectiveDetectors { get; }
        ObservableCollection<string> AllSignalGroupStrings { get; }
        ObservableCollection<string> AllDetectorStrings { get; }
        ObservableCollection<string> AllVecomDetectorStrings { get; }
        ObservableCollection<string> AllSelectiveDetectorStrings { get; }
        void Setup();
    }

    public class ControllerAccessProvider : IControllerAccessProvider
    {
        private readonly IMessenger MessengerInstance;
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
            MessengerInstance.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnSignalGroupFaseDetectorTypeChanged));
            MessengerInstance.Register(this, new Action<FaseDetectorVeiligheidsGroenChangedMessage>(OnSignalGroupDetectorVeiligheidsGroenChanged));
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
                case TLCGenObjectTypeEnum.SelectieveDetector:
                    AllSelectiveDetectorStrings.Remove(obj.OldName);
                    AllSelectiveDetectorStrings.Add(obj.NewName);
                    AllSelectiveDetectorStrings.BubbleSort();
                    break;
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

        private void OnSignalGroupsChanged(FasenChangedMessage obj)
        {
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
        }

        
        private void OnSignalGroupFaseDetectorTypeChanged(FaseDetectorTypeChangedMessage message)
        {
            foreach (var fcm in AllSignalGroups)
            {
                fcm.UpdateHasKopmax();
            }
        }

        private void OnSignalGroupDetectorVeiligheidsGroenChanged(FaseDetectorVeiligheidsGroenChangedMessage message)
        {
            foreach (var fcm in AllSignalGroups)
            {
                fcm.UpdateHasVeiligheidsGroen();
            }
        }

        private void OnDetectorenChanged(DetectorenChangedMessage dmsg)
        {
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

            RefreshVecomDetectors();
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
            _allSignalGroups = null;
            _allSignalGroupStrings = null;
            _allDetectors = null;
            _allDetectorStrings = null;
            _allSelectiveDetectors = null;
            _allSelectiveDetectorStrings = null;
            _allVecomDetectors = null;
            _allVecomDetectorStrings = null;

            Controller = obj.Controller;

            if (obj.Controller == null) return;

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
        }

        public ControllerAccessProvider(IMessenger messenger)
        {
            MessengerInstance = messenger;
        }
    }
}
