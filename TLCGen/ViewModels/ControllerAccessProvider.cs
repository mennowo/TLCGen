using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Dependencies.Messaging.Messages;
using TLCGen.Extensions;
using TLCGen.Helpers;
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
        ObservableCollection<PeriodeViewModel> AllePerioden { get; }
        ObservableCollection<DetectorViewModel> AllDetectors { get; }
        ObservableCollection<DetectorViewModel> AllVecomDetectors { get; }
        ObservableCollection<SelectieveDetectorViewModel> AllSelectiveDetectors { get; }
        ObservableCollection<string> AllSignalGroupStrings { get; }
        ObservableCollection<string> AllDetectorStrings { get; }
        ObservableCollection<string> AllVecomDetectorStrings { get; }
        ObservableCollection<string> AllSelectiveDetectorStrings { get; }
        ObservableCollection<string> OVIngangenStrings { get; }

        ICollectionView GetCollectionView(object type);
        void Setup();
    }

    public class ControllerAccessProvider : IControllerAccessProvider
    {
        private ObservableCollection<IngangViewModel> _allIngangen;
        private ObservableCollection<FaseCyclusViewModel> _allSignalGroups;
        private ObservableCollection<DetectorViewModel> _allDetectors;
        private ObservableCollection<DetectorViewModel> _allVecomDetectors;
        private ObservableCollection<SelectieveDetectorViewModel> _allSelectiveDetectors;
        private ObservableCollection<PeriodeViewModel> _allePerioden;
        private ObservableCollection<string> _allSignalGroupStrings;
        private ObservableCollection<string> _allDetectorStrings;
        private ObservableCollection<string> _allVecomDetectorStrings;
        private ObservableCollection<string> _allSelectiveDetectorStrings;
        private ObservableCollection<string> _ovIngangenStrings;
        private readonly Dictionary<object, ICollectionView> _detectorsCollectionViews = new Dictionary<object, ICollectionView>();
        private bool _fasenChanging;
        private bool _detChanging;
        private static IControllerAccessProvider _default;

        public static IControllerAccessProvider Default => _default ??= new ControllerAccessProvider();

        public static void OverrideDefault(IControllerAccessProvider provider)
        {
            _default = provider;
        }


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
                case PeriodeTypeEnum p:
                    if(_allePerioden != null)
                    {
                        if (!_detectorsCollectionViews.ContainsKey(p))
                        {
                            var view = CollectionViewSource.GetDefaultView(_allIngangen);
                            view.Filter += o => ((PeriodeViewModel)o).Type == p;
                            view.Refresh();
                            _detectorsCollectionViews.Add(p, view);
                        }
                        return _detectorsCollectionViews[p];
                    }
                    break;
            }

            return null;
        }

        public ObservableCollection<IngangViewModel> AllIngangen =>
            _allIngangen ??= new ObservableCollection<IngangViewModel>();

        public ObservableCollection<string> OVIngangenStrings =>
            _ovIngangenStrings ??= new ObservableCollection<string>();

        public ObservableCollection<FaseCyclusViewModel> AllSignalGroups =>
            _allSignalGroups ??= new ObservableCollection<FaseCyclusViewModel>();
        
        public ObservableCollection<string> AllSignalGroupStrings =>
            _allSignalGroupStrings ??= new ObservableCollection<string>();

        public ObservableCollection<DetectorViewModel> AllDetectors =>
            _allDetectors ??= new ObservableCollection<DetectorViewModel>();

        public ObservableCollection<string> AllDetectorStrings =>
            _allDetectorStrings ??= new ObservableCollection<string>();
        
        public ObservableCollection<DetectorViewModel> AllVecomDetectors =>
            _allVecomDetectors ??= new ObservableCollection<DetectorViewModel>();

        public ObservableCollection<string> AllVecomDetectorStrings =>
            _allVecomDetectorStrings ??= new ObservableCollection<string>();

        public ObservableCollection<SelectieveDetectorViewModel> AllSelectiveDetectors =>
            _allSelectiveDetectors ??= new ObservableCollection<SelectieveDetectorViewModel>();

        public ObservableCollection<string> AllSelectiveDetectorStrings =>
            _allSelectiveDetectorStrings ??= new ObservableCollection<string>();

        public ObservableCollection<PeriodeViewModel> AllePerioden =>
            _allePerioden ??= new ObservableCollection<PeriodeViewModel>();

        public ControllerModel Controller { get; private set; }

        public void Setup()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnSignalGroupsChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<SelectieveDetectorenChangedMessage>(this, OnSelectieveDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<ControllerLoadedMessage>(this, OnControllerLoaded);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<IngangenChangedMessage>(this, OnIngangenChanged);
            WeakReferenceMessengerEx.Default.Register<FaseDetectorTypeChangedMessage>(this, OnSignalGroupFaseDetectorTypeChanged);
            WeakReferenceMessengerEx.Default.Register<FaseDetectorVeiligheidsGroenChangedMessage>(this, OnSignalGroupDetectorVeiligheidsGroenChanged);
            WeakReferenceMessengerEx.Default.Register<PeriodenChangedMessage>(this, OnPeriodenChanged);
        }

        private void OnPeriodenChanged(object sender, PeriodenChangedMessage obj)
        {
            foreach (var p in Controller.PeriodenData.Perioden.Where(p => AllePerioden.All(x => x.Naam != p.Naam)))
            {
                AllePerioden.Add(new PeriodeViewModel(p));
            }

            var rem = AllePerioden.Where(x => Controller.PeriodenData.Perioden.All(x2 => x2.Naam != x.Naam)).ToList();
            foreach (var r in rem) AllePerioden.Remove(r);
        }

        private void OnIngangenChanged(object sender, IngangenChangedMessage obj)
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

        private void OnNameChanged(object sender, NameChangedMessage obj)
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
                    OVIngangenStrings.Remove(obj.OldName);
                    OVIngangenStrings.Add(obj.NewName);
                    OVIngangenStrings.BubbleSort();
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

        private void OnSignalGroupsChanged(object sender, FasenChangedMessage obj)
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

        
        private void OnSignalGroupFaseDetectorTypeChanged(object sender, FaseDetectorTypeChangedMessage message)
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

        private void OnSignalGroupDetectorVeiligheidsGroenChanged(object sender, FaseDetectorVeiligheidsGroenChangedMessage message)
        {
            foreach (var fcm in AllSignalGroups)
            {
                fcm.UpdateHasVeiligheidsGroen();
            }
        }

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage dmsg)
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

        private void OnSelectieveDetectorenChanged(object sender, SelectieveDetectorenChangedMessage obj)
        {
            AllSelectiveDetectors.Clear();
            AllSelectiveDetectorStrings.Clear();
            foreach (var seld in DataAccess.TLCGenControllerDataProvider.Default.Controller.SelectieveDetectoren)
            {
                AllSelectiveDetectors.Add(new SelectieveDetectorViewModel(seld));
                AllSelectiveDetectorStrings.Add(seld.Naam);
            }
        }

        private void OnControllerLoaded(object sender, ControllerLoadedMessage obj)
        {
            AllIngangen.Clear();
            OVIngangenStrings.Clear();
            AllSignalGroups.Clear();
            AllSignalGroupStrings.Clear();
            AllDetectors.Clear();
            AllDetectorStrings.Clear();
            AllSelectiveDetectors.Clear();
            AllSelectiveDetectorStrings.Clear();
            AllVecomDetectors.Clear();
            AllVecomDetectorStrings.Clear();
            AllePerioden.Clear();
            
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
            foreach (var ingang in obj.Controller.Ingangen)
            {
                AllIngangen.Add(new IngangViewModel(ingang));
                if (ingang.Type == IngangTypeEnum.OVmelding)
                {
                    OVIngangenStrings.Add(ingang.Naam);
                }
            }

            foreach (var periode in obj.Controller.PeriodenData.Perioden)
            {
                AllePerioden.Add(new PeriodeViewModel(periode));;
            }
        }

        public ControllerAccessProvider()
        {
        }
    }
}
