﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Plugins.DynamischHiaat.Models;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.Plugins.DynamischHiaat.ViewModels
{
    internal class DynamischHiaatPluginTabViewModel : ObservableObjectEx
    {
        #region Fields

        private DynamischHiaatModel _model;
        private DynamischHiaatPlugin _plugin;
        private ControllerModel _controller;
        private ObservableCollectionAroundList<DynamischHiaatSignalGroupViewModel, DynamischHiaatSignalGroupModel> _DynamischHiaatSignalGroups;
        private DynamischHiaatSignalGroupViewModel _selectedDynamischHiaatSignalGroup;

        #endregion // Fields

        #region Properties

        public string TypeDynamischHiaat
        {
            get => Model.TypeDynamischHiaat;
            set
            {
                Model.TypeDynamischHiaat = value;
                OnPropertyChanged(broadcast: true);
                foreach(var msg in DynamischHiaatSignalGroups)
                {
                    msg.SelectedDefault = _plugin.MyDefaults.Defaults.FirstOrDefault(x => x.Name == TypeDynamischHiaat);
                    if(string.IsNullOrEmpty(msg.Snelheid) || !msg.SelectedDefault.Snelheden.Any(x => x.Name == msg.Snelheid))
                    {
                        msg.Snelheid = msg.SelectedDefault.DefaultSnelheid;
                    }
                }
            }
        }

        public DynamischHiaatSignalGroupViewModel SelectedDynamischHiaatSignalGroup
        {
            get => _selectedDynamischHiaatSignalGroup;
            set
            {
                _selectedDynamischHiaatSignalGroup = value;
                if (value != null)
                {
                    var fc = _controller.Fasen.FirstOrDefault(x => x.Naam == _selectedDynamischHiaatSignalGroup.SignalGroupName);
                    if(fc != null)
                    {
                        _selectedDynamischHiaatSignalGroup.UpdateSelectableDetectoren(fc.Detectoren.Select(x => x.Naam));
                    }
                }
                OnPropertyChanged();
            }
        }

        public List<DynamischHiaatDefaultModel> Defaults => _plugin.MyDefaults.Defaults;

        public ObservableCollectionAroundList<DynamischHiaatSignalGroupViewModel, DynamischHiaatSignalGroupModel> DynamischHiaatSignalGroups
        {
            get
            {
                if(_DynamischHiaatSignalGroups == null)
                {
                     _DynamischHiaatSignalGroups = new ObservableCollectionAroundList<DynamischHiaatSignalGroupViewModel, DynamischHiaatSignalGroupModel>(_model.SignaalGroepenMetDynamischHiaat);
                    foreach(var msg in _DynamischHiaatSignalGroups)
                    {
                        msg.SelectedDefault = _plugin.MyDefaults.Defaults.FirstOrDefault(x => x.Name == TypeDynamischHiaat);
                    }
                }
                return _DynamischHiaatSignalGroups;
            }
        }

        public ControllerModel Controller { get => _controller; set => _controller = value; }

        public DynamischHiaatModel Model
        {
            get => _model;
            set
            {
                _model = value;
                if (value != null)
                {
                    _DynamischHiaatSignalGroups = null;
                }
                OnPropertyChanged("");
            }
        }

        #endregion // Properties

        #region Public Methods

        public void UpdateTLCGenMessaging()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<FaseDetectorTypeChangedMessage>(this, OnFaseDetectorTypeChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessengerEx.Default.Register<FaseTypeChangedMessage>(this, OnFaseTypeChanged);
        }

        private void OnFaseTypeChanged(object sender, FaseTypeChangedMessage obj)
        {
            if (obj.NewType == TLCGen.Models.Enumerations.FaseTypeEnum.Auto)
            {
                var mfc = DynamischHiaatSignalGroups.FirstOrDefault(x => x.SignalGroupName == obj.Fase.Naam);
                if(mfc == null)
                {
                    var vm = new DynamischHiaatSignalGroupViewModel(new DynamischHiaatSignalGroupModel { SignalGroupName = obj.Fase.Naam });
                    vm.SelectedDefault = _plugin.MyDefaults.Defaults.FirstOrDefault(x => x.Name == _model.TypeDynamischHiaat);
                    if (vm.SelectedDefault != null &&
                        (string.IsNullOrEmpty(vm.Snelheid) || !vm.SelectedDefault.Snelheden.Any(x => x.Name == vm.Snelheid)))
                    {
                        vm.Snelheid = vm.SelectedDefault.DefaultSnelheid;
                    }
                    DynamischHiaatSignalGroups.Add(vm);
                }
            }
            if (obj.NewType != TLCGen.Models.Enumerations.FaseTypeEnum.Auto)
            {
                var mfc = DynamischHiaatSignalGroups.FirstOrDefault(x => x.SignalGroupName == obj.Fase.Naam);
                DynamischHiaatSignalGroups.Remove(mfc);
            }
        }

        private void OnFaseDetectorTypeChanged(object sender, FaseDetectorTypeChangedMessage obj)
        {
            var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Detectoren.Any(x2 => x2.Naam == obj.DetectorDefine));
            if (fc != null)
            {
                var d = fc.Detectoren.First(x => x.Naam == obj.DetectorDefine);
                var mfc = DynamischHiaatSignalGroups.FirstOrDefault(x => x.SignalGroupName == fc.Naam);
                if (mfc != null && mfc.HasDynamischHiaat && 
                    (d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Kop ||
                     d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Lang ||
                     d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Verweg))
                {
                    if (!mfc.DynamischHiaatDetectoren.Any(x => x.DetectorName == d.Naam))
                    {
                        mfc.DynamischHiaatDetectoren.Add(new DynamischHiaatDetectorViewModel(new DynamischHiaatDetectorModel
                        {
                            DetectorName = d.Naam,
                            SignalGroupName = fc.Naam
                        }));
                    }
                    mfc.DynamischHiaatDetectoren.BubbleSort();
                }
                else if (mfc != null && mfc.DynamischHiaatDetectoren.Any(x => x.DetectorName == d.Naam))
                {
                    var r = mfc.DynamischHiaatDetectoren.First(x => x.DetectorName == d.Naam);
                    mfc.DynamischHiaatDetectoren.Remove(r);
                    mfc.DynamischHiaatDetectoren.BubbleSort();
                }
                if (mfc != null) mfc.DynamischHiaatDetectorenManager.UpdateSelectables(fc.Detectoren.Select(x => x.Naam));
            }
        }

        #endregion

        #region TLCGen Events

        private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            if(message.AddedDetectoren == null && message.RemovedDetectoren == null)
            {
                foreach(var mfc in DynamischHiaatSignalGroups)
                {
                    mfc.DynamischHiaatDetectoren.BubbleSort();
                }
            }
            if (message.AddedDetectoren?.Count > 0)
            {
                foreach(var d in message.AddedDetectoren)
                {
                    var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Detectoren.Any(x2 => x2.Naam == d.Naam));
                    if(fc != null)
                    {
                        var mfc = DynamischHiaatSignalGroups.FirstOrDefault(x => x.SignalGroupName == fc.Naam);
                        if (mfc != null && mfc.HasDynamischHiaat)
                        {
                            if (!mfc.DynamischHiaatDetectoren.Any(x => x.DetectorName == d.Naam) &&
                                (d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Kop ||
                                 d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Lang ||
                                 d.Type == TLCGen.Models.Enumerations.DetectorTypeEnum.Verweg))
                            {
                                mfc.DynamischHiaatDetectoren.Add(new DynamischHiaatDetectorViewModel(new DynamischHiaatDetectorModel
                                {
                                    DetectorName = d.Naam,
                                    SignalGroupName = fc.Naam
                                }));
                            }
                            mfc.DynamischHiaatDetectoren.BubbleSort();
                            mfc.DynamischHiaatDetectorenManager.UpdateSelectables(fc.Detectoren.Select(x => x.Naam));
                        }
                    }
                }
            }
            if (message.RemovedDetectoren?.Count > 0)
            {
                foreach(var mfc in DynamischHiaatSignalGroups)
                {
                    var rem = mfc.DynamischHiaatDetectoren.Where(x => message.RemovedDetectoren.Any(x2 => x2.Naam == x.DetectorName)).ToList();
                    foreach(var r in rem)
                    {
                        mfc.DynamischHiaatDetectoren.Remove(r);
                        mfc.DynamischHiaatDetectorenManager.SelectableItems.Remove(r.DetectorName);
                    }
                }
            }
        }

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            if (message.AddedFasen?.Count > 0)
            {
                foreach (var fc in message.AddedFasen.Where(x => x.Type == TLCGen.Models.Enumerations.FaseTypeEnum.Auto))
                {
                    var sn = _plugin.MyDefaults.Defaults.FirstOrDefault(x => x.Name == TypeDynamischHiaat);
                    var msg = new DynamischHiaatSignalGroupViewModel(new DynamischHiaatSignalGroupModel
                    {
                        SignalGroupName = fc.Naam,
                        Snelheid = (sn == null ? "" : sn.DefaultSnelheid)
                    });
                    msg.SelectedDefault = _plugin.MyDefaults.Defaults.FirstOrDefault(x => x.Name == TypeDynamischHiaat);
                    DynamischHiaatSignalGroups.Add(msg);
                }
                DynamischHiaatSignalGroups.BubbleSort();
            }
            if (message.RemovedFasen?.Count > 0)
            {
                var rems = new List<DynamischHiaatSignalGroupViewModel>();
                foreach (var fc in message.RemovedFasen)
                {
                    var r = DynamischHiaatSignalGroups.FirstOrDefault(x => x.SignalGroupName == fc.Naam);
                    if (r != null) rems.Add(r);
                }
                foreach (var r in rems)
                {
                    DynamischHiaatSignalGroups.Remove(r);
                }
            }
        }

        private void OnNameChanged(object sender, NameChangedMessage message)
        {
            ModelManagement.TLCGenModelManager.Default.ChangeNameOnObject(_model, message.OldName, message.NewName, message.ObjectType);
            foreach (var mfc in DynamischHiaatSignalGroups) mfc.DynamischHiaatDetectoren.BubbleSort();
            OnPropertyChanged("");
        }

        private void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            DynamischHiaatSignalGroups.BubbleSort();
        }

        #endregion // TLCGen Events

        #region Constructor

        public DynamischHiaatPluginTabViewModel(DynamischHiaatPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
