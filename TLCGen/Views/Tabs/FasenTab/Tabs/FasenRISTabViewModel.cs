using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using TLCGen.Messaging.Messages;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Models.Enumerations;
using System.Collections.Generic;
using TLCGen.Dependencies.Messaging.Messages;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 5, type: TabItemTypeEnum.FasenTab)]
    public class FasenRISTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RISFaseCyclusDataViewModel _selectedRISFase;
        private RISSystemITFViewModel _selectedSystemITF;
        private RISDataModel _risModel;
        private AddRemoveItemsManager<RISLaneRequestDataViewModel, RISLaneRequestDataModel, string> _lanesRequestManager;
        private AddRemoveItemsManager<RISLaneExtendDataViewModel, RISLaneExtendDataModel, string> _lanesExtendManager;
        private AddRemoveItemsManager<RISLanePelotonDataViewModel,RISLanePelotonDataModel,string> _lanesPelotonManager;
        private RelayCommand _addDefaultRequestLanesCommand;
        private RelayCommand _addDefaultExtendLanesCommand;
        private RelayCommand _addSystemITFCommand;
        private RelayCommand _removeSystemITFCommand;
        private RelayCommand _copyExtendFromRequestLanesCommand;
        private RelayCommand _copySimulationFromRequestLanesCommand;
        private RelayCommand _generateRISSimulationDataCommand;

        #endregion // Fields

        #region Properties

        public override string DisplayName => "RIS";

        public override ControllerModel Controller
        {
            get => base.Controller; set
            {
                base.Controller = value;
                RISModel = value?.RISData;
            }
        }

        public RISFaseCyclusDataViewModel SelectedRISFase
        {
            get => _selectedRISFase;
            set
            {
                _selectedRISFase = value;
                RaisePropertyChanged();
            }
        }

        public RISDataModel RISModel
        {
            get => _risModel;
            set
            {
                _risModel = value;
                RISFasen = value != null ? new ObservableCollectionAroundList<RISFaseCyclusDataViewModel, RISFaseCyclusDataModel>(_risModel.RISFasen) : null;
                RISRequestLanes = value != null ? new ObservableCollectionAroundList<RISLaneRequestDataViewModel, RISLaneRequestDataModel>(_risModel?.RISRequestLanes) : null;
                RISExtendLanes = value != null ? new ObservableCollectionAroundList<RISLaneExtendDataViewModel, RISLaneExtendDataModel>(_risModel.RISExtendLanes) : null;
                RISPelotonLanes = value != null ? new ObservableCollectionAroundList<RISLanePelotonDataViewModel, RISLanePelotonDataModel>(_risModel.RISPelotonLanes) : null;
                _lanesRequestManager = null;
                _lanesExtendManager = null;
                if (MultiSystemITF != null) MultiSystemITF.CollectionChanged -= MultiSystemITF_CollectionChanged;
                if (value == null) return;
                MultiSystemITF = new ObservableCollectionAroundList<RISSystemITFViewModel, RISSystemITFModel>(_risModel.MultiSystemITF);
                MultiSystemITF.CollectionChanged += MultiSystemITF_CollectionChanged;
                UpdateModel();
            }
        }

        private void MultiSystemITF_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            MessengerInstance.Send(new ControllerDataChangedMessage());
        }

        public ObservableCollectionAroundList<RISFaseCyclusDataViewModel, RISFaseCyclusDataModel> RISFasen { get; private set; }
    
        public ObservableCollectionAroundList<RISLaneRequestDataViewModel, RISLaneRequestDataModel> RISRequestLanes { get; private set; }
        public ObservableCollectionAroundList<RISLaneExtendDataViewModel, RISLaneExtendDataModel> RISExtendLanes { get; private set; }
        public ObservableCollectionAroundList<RISLanePelotonDataViewModel, RISLanePelotonDataModel> RISPelotonLanes { get; private set; }
        public ObservableCollectionAroundList<RISSystemITFViewModel, RISSystemITFModel> MultiSystemITF { get; private set; }
        public ObservableCollection<RISFaseCyclusLaneDataViewModel> RISLanes { get; } = new ObservableCollection<RISFaseCyclusLaneDataViewModel>();

        public RISSystemITFViewModel SelectedSystemITF
        {
            get => _selectedSystemITF;
            set
            {
                _selectedSystemITF = value;
                RaisePropertyChanged();
            }
        }

        public bool RISToepassen
        {
            get => _risModel?.RISToepassen == true;
            set
            {
                _risModel.RISToepassen = value;
                TLCGenModelManager.Default.UpdateControllerAlerts();
                RaisePropertyChanged<object>(broadcast: true);
                if (string.IsNullOrWhiteSpace(SystemITF)) SystemITF = Controller.Data.Naam;
                foreach (var fc in RISFasen)
                {
                    var sg = Controller.Fasen.First(x => x.Naam == fc.FaseCyclus);
                    if (fc.Lanes.Any())
                    {
                        foreach (var l in fc.Lanes)
                        {
                            if (l.SimulatedStations.Any())
                            {
                                switch (sg.Type)
                                {
                                    case FaseTypeEnum.Auto:
                                        if (l.SimulatedStations[0].StationType != RISStationTypeSimEnum.PASSENGERCAR) l.SimulatedStations[0].StationType = RISStationTypeSimEnum.PASSENGERCAR;
                                        break;
                                    case FaseTypeEnum.Fiets:
                                        if (l.SimulatedStations[0].StationType != RISStationTypeSimEnum.CYCLIST) l.SimulatedStations[0].StationType = RISStationTypeSimEnum.CYCLIST;
                                        break;
                                    case FaseTypeEnum.Voetganger:
                                        if (l.SimulatedStations[0].StationType != RISStationTypeSimEnum.PEDESTRIAN) l.SimulatedStations[0].StationType = RISStationTypeSimEnum.PEDESTRIAN;
                                        break;
                                    case FaseTypeEnum.OV:
                                        if (l.SimulatedStations[0].StationType != RISStationTypeSimEnum.BUS) l.SimulatedStations[0].StationType = RISStationTypeSimEnum.BUS;
                                        break;
                                }
                            }
                        }
                    }
                }
                UpdateRISLanes();
                for (var l = 0; l < RISLanes.Count; l++)
                {
                    RISLanes[l].LaneID = l + 1;
                }
            }
        }

        public bool IsCCOL12OrHigher => Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL120;

        public bool HasMultipleSystemITF
        {
            get => _risModel?.HasMultipleSystemITF == true;
            set
            {
                _risModel.HasMultipleSystemITF = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(NoHasMultipleSystemITF));
            }
        }

        public bool NoHasMultipleSystemITF => !HasMultipleSystemITF;

        public string SystemITF
        {
            get => _risModel?.SystemITF;
            set
            {
                _risModel.SystemITF = value;
                if (!HasMultipleSystemITF)
                {
                    foreach (var l in RISLanes)
                    {
                        l.SystemITF = value;
                    }
                }
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public AddRemoveItemsManager<RISLaneRequestDataViewModel, RISLaneRequestDataModel, string> LanesRequestManager =>
            _lanesRequestManager ??= new AddRemoveItemsManager<RISLaneRequestDataViewModel, RISLaneRequestDataModel, string>(
                RISRequestLanes,
                _ =>
                {
                    if (!RISFasen.Any()) return null;
                    var lre = new RISLaneRequestDataViewModel(new RISLaneRequestDataModel()
                    {
                        SignalGroupName = RISRequestLanes.Any() ? RISRequestLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus,
                        RijstrookIndex = 1,
                        Type = GetTypeForFase(null, RISRequestLanes.Any() ? RISRequestLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus)
                    });
                    return lre;
                },
                (_, _) => false,
                () => MessengerInstance.Send(new ControllerDataChangedMessage())
            );

        public AddRemoveItemsManager<RISLaneExtendDataViewModel, RISLaneExtendDataModel, string> LanesExtendManager =>
            _lanesExtendManager ??= new AddRemoveItemsManager<RISLaneExtendDataViewModel, RISLaneExtendDataModel, string>(
                RISExtendLanes,
                _ =>
                {
                    if (!RISFasen.Any()) return null;
                    var lre = new RISLaneExtendDataViewModel(new RISLaneExtendDataModel()
                    {
                        SignalGroupName = RISExtendLanes.Any() ? RISExtendLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus,
                        RijstrookIndex = 1,
                        Type = GetTypeForFase(null, RISExtendLanes.Any() ? RISExtendLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus)
                    });
                    return lre;
                },
                (_, _) => false,
                () => MessengerInstance.Send(new ControllerDataChangedMessage())
            );
        
        public AddRemoveItemsManager<RISLanePelotonDataViewModel, RISLanePelotonDataModel, string> LanesPelotonManager =>
            _lanesPelotonManager ??= new AddRemoveItemsManager<RISLanePelotonDataViewModel, RISLanePelotonDataModel, string>(
                RISPelotonLanes,
                _ =>
                {
                    if (!RISFasen.Any()) return null;
                    var lre = new RISLanePelotonDataViewModel(new RISLanePelotonDataModel()
                    {
                        SignalGroupName = RISPelotonLanes.Any() ? RISPelotonLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus,
                        RijstrookIndex = 1,
                        Type = GetTypeForFase(null, RISPelotonLanes.Any() ? RISPelotonLanes.Last().SignalGroupName : RISFasen.First().FaseCyclus)
                    });
                    return lre;
                },
                (_, _) => false,
                () => MessengerInstance.Send(new ControllerDataChangedMessage())
            );

        #endregion // Properties

        #region ITLCGenTabItem

        public override void OnSelected()
        {
            if (!RISRequestLanes.IsSorted())
            {
                RISRequestLanes.BubbleSort();
            }
            if (!RISExtendLanes.IsSorted())
            {
                RISExtendLanes.BubbleSort();
            }
            if (!RISPelotonLanes.IsSorted())
            {
                RISPelotonLanes.BubbleSort();
            }
        }

        public override bool CanBeEnabled()
        {
            return _Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL110;
        }
        
        #endregion

        #region Commands

        public ICommand AddDefaultRequestLanesCommand => _addDefaultRequestLanesCommand ??= new RelayCommand(AddDefaultRequestLanesCommand_executed);

        private void AddDefaultRequestLanesCommand_executed()
        {
            foreach (var fc in Controller.Fasen)
            {
                var t = GetTypeForFase(fc);
                for (var i = 0; i < fc.AantalRijstroken; i++)
                {
                    if (RISRequestLanes.All(x => x.SignalGroupName != fc.Naam || x.SignalGroupName == fc.Naam && x.RijstrookIndex != i + 1 || x.SignalGroupName == fc.Naam && x.RijstrookIndex == i + 1 && x.Type != t))
                    {
                        RISRequestLanes.Add(new RISLaneRequestDataViewModel(new RISLaneRequestDataModel
                        {
                            SignalGroupName = fc.Naam,
                            RijstrookIndex = i + 1,
                            Type = t
                        }));
                    }
                }
            }
            RISRequestLanes.BubbleSort();
        }

        public ICommand AddDefaultExtendLanesCommand => _addDefaultExtendLanesCommand ??= new RelayCommand(() =>
        {
            foreach (var fc in Controller.Fasen)
            {
                var t = GetTypeForFase(fc);
                for (var i = 0; i < fc.AantalRijstroken; i++)
                {
                    var i1 = i;
                    if (RISExtendLanes.All(x => x.SignalGroupName != fc.Naam || x.SignalGroupName == fc.Naam && x.RijstrookIndex != i1 + 1 || x.SignalGroupName == fc.Naam && x.RijstrookIndex == i1 + 1 && x.Type != t))
                    {
                        RISExtendLanes.Add(new RISLaneExtendDataViewModel(new RISLaneExtendDataModel
                        {
                            SignalGroupName = fc.Naam,
                            RijstrookIndex = i + 1,
                            Type = t
                        }));
                    }
                }
            }

            RISExtendLanes.BubbleSort();
        });

        public ICommand CopyExtendFromRequestsLanesCommand => _copyExtendFromRequestLanesCommand ??= new RelayCommand(() =>
        {
            foreach (var req in RISRequestLanes)
            {
                if (!RISExtendLanes.Any(x => x.SignalGroupName == req.SignalGroupName && x.Type == req.Type && x.RijstrookIndex == req.RijstrookIndex))
                {
                    RISExtendLanes.Add(new RISLaneExtendDataViewModel(new RISLaneExtendDataModel
                    {
                        SignalGroupName = req.SignalGroupName,
                        RijstrookIndex = req.RijstrookIndex,
                        VerlengenStart = req.AanvraagStart,
                        VerlengenEnd = req.AanvraagEnd,
                        Type = req.Type
                    }));
                }
            }
        });

        private bool StationsTypeEqual(RISStationTypeEnum risStationTypeEnum, RISStationTypeSimEnum risStationTypeSimEnum)
        {
            switch (risStationTypeEnum)
            {
                case RISStationTypeEnum.UNKNOWN: return risStationTypeSimEnum == RISStationTypeSimEnum.UNKNOWN;
                case RISStationTypeEnum.PEDESTRIAN: return risStationTypeSimEnum == RISStationTypeSimEnum.PEDESTRIAN;
                case RISStationTypeEnum.CYCLIST: return risStationTypeSimEnum == RISStationTypeSimEnum.CYCLIST;
                case RISStationTypeEnum.MOPED: return risStationTypeSimEnum == RISStationTypeSimEnum.MOPED;
                case RISStationTypeEnum.MOTORCYCLE: return risStationTypeSimEnum == RISStationTypeSimEnum.MOTORCYCLE;
                case RISStationTypeEnum.PASSENGERCAR: return risStationTypeSimEnum == RISStationTypeSimEnum.PASSENGERCAR;
                case RISStationTypeEnum.BUS: return risStationTypeSimEnum == RISStationTypeSimEnum.BUS;
                case RISStationTypeEnum.LIGHTTRUCK: return risStationTypeSimEnum == RISStationTypeSimEnum.LIGHTTRUCK;
                case RISStationTypeEnum.HEAVYTRUCK: return risStationTypeSimEnum == RISStationTypeSimEnum.HEAVYTRUCK;
                case RISStationTypeEnum.TRAILER: return risStationTypeSimEnum == RISStationTypeSimEnum.TRAILER;
                case RISStationTypeEnum.SPECIALVEHICLES: return risStationTypeSimEnum == RISStationTypeSimEnum.SPECIALVEHICLES;
                case RISStationTypeEnum.TRAM: return risStationTypeSimEnum == RISStationTypeSimEnum.TRAM;
                case RISStationTypeEnum.ROADSIDEUNIT: return risStationTypeSimEnum == RISStationTypeSimEnum.ROADSIDEUNIT;
                case RISStationTypeEnum.TRUCKS: return
                        risStationTypeSimEnum == RISStationTypeSimEnum.LIGHTTRUCK ||
                        risStationTypeSimEnum == RISStationTypeSimEnum.HEAVYTRUCK ||
                        risStationTypeSimEnum == RISStationTypeSimEnum.TRAILER;  
                case RISStationTypeEnum.MOTORVEHICLES: return
                        risStationTypeSimEnum != RISStationTypeSimEnum.UNKNOWN &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.CYCLIST &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.MOPED &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.TRAM &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.ROADSIDEUNIT &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.PEDESTRIAN;
                case RISStationTypeEnum.VEHICLES: return
                        risStationTypeSimEnum != RISStationTypeSimEnum.UNKNOWN &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.ROADSIDEUNIT &&
                        risStationTypeSimEnum != RISStationTypeSimEnum.PEDESTRIAN;
                default:
                    throw new ArgumentOutOfRangeException(nameof(risStationTypeEnum), risStationTypeEnum, null);
            }
        }
        
        private RISStationTypeSimEnum ConvertStationTypeToSim(RISStationTypeEnum risStationTypeEnum)
        {
            switch (risStationTypeEnum)
            {
                case RISStationTypeEnum.UNKNOWN: return RISStationTypeSimEnum.UNKNOWN;
                case RISStationTypeEnum.PEDESTRIAN: return RISStationTypeSimEnum.PEDESTRIAN;
                case RISStationTypeEnum.CYCLIST: return RISStationTypeSimEnum.CYCLIST;
                case RISStationTypeEnum.MOPED: return RISStationTypeSimEnum.MOPED;
                case RISStationTypeEnum.MOTORCYCLE: return RISStationTypeSimEnum.MOTORCYCLE;
                case RISStationTypeEnum.PASSENGERCAR: return RISStationTypeSimEnum.PASSENGERCAR;
                case RISStationTypeEnum.BUS: return RISStationTypeSimEnum.BUS;
                case RISStationTypeEnum.LIGHTTRUCK: return RISStationTypeSimEnum.LIGHTTRUCK;
                case RISStationTypeEnum.HEAVYTRUCK: return RISStationTypeSimEnum.HEAVYTRUCK;
                case RISStationTypeEnum.TRAILER: return RISStationTypeSimEnum.TRAILER;
                case RISStationTypeEnum.SPECIALVEHICLES: return RISStationTypeSimEnum.SPECIALVEHICLES;
                case RISStationTypeEnum.TRAM: return RISStationTypeSimEnum.TRAM;
                case RISStationTypeEnum.ROADSIDEUNIT: return RISStationTypeSimEnum.ROADSIDEUNIT;
                case RISStationTypeEnum.TRUCKS: return RISStationTypeSimEnum.LIGHTTRUCK;  
                case RISStationTypeEnum.MOTORVEHICLES: return RISStationTypeSimEnum.PASSENGERCAR;
                case RISStationTypeEnum.VEHICLES: return RISStationTypeSimEnum.PASSENGERCAR;
                default:
                    throw new ArgumentOutOfRangeException(nameof(risStationTypeEnum), risStationTypeEnum, null);
            }
        }

        public ICommand CopySimulationFromRequestsLanesCommand => _copySimulationFromRequestLanesCommand ??= new RelayCommand(() =>
        {
            foreach (var risFc in RISFasen)
            {
                foreach (var risLane in risFc.Lanes)
                {
                    var cfc = _Controller.Fasen.FirstOrDefault(x => x.Naam == risFc.FaseCyclus);
                    if (cfc == null) continue;

                    List<RISStationTypeEnum> types;
                    switch (cfc.Type)
                    {
                        case FaseTypeEnum.Auto:
                            types = new List<RISStationTypeEnum>
                            {
                                RISStationTypeEnum.PASSENGERCAR, RISStationTypeEnum.BUS, RISStationTypeEnum.SPECIALVEHICLES, RISStationTypeEnum.HEAVYTRUCK
                            };
                            break;
                        case FaseTypeEnum.Fiets:
                            types = new List<RISStationTypeEnum> {RISStationTypeEnum.CYCLIST};
                            break;
                        case FaseTypeEnum.Voetganger:
                            types = new List<RISStationTypeEnum> {RISStationTypeEnum.PEDESTRIAN};
                            break;
                        case FaseTypeEnum.OV:
                            types = new List<RISStationTypeEnum> {RISStationTypeEnum.BUS, RISStationTypeEnum.SPECIALVEHICLES};
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (var stType in types)
                    {
                        var sim = risLane.SimulatedStations.FirstOrDefault(x => StationsTypeEqual(stType, x.StationType));
                        if (sim == null)
                        {
                            var simData = new RISFaseCyclusLaneSimulatedStationModel
                            {
                                LaneID = risLane.LaneID,
                                SignalGroupName = risLane.SignalGroupName,
                                ApproachID = risFc.ApproachID,
                                RijstrookIndex = risLane.RijstrookIndex,
                                Type = ConvertStationTypeToSim(stType),
                                SystemITF = risLane.SystemITF,
                                SimulationData =
                                {
                                    FCNr = stType switch
                                    {
                                        RISStationTypeEnum.SPECIALVEHICLES => "NG",
                                        _ => cfc.Naam,
                                    }
                                }
                            };
                            simData.SimulationData.RelatedName = simData.Naam;
                            var (selector1, selector2) = stType switch
                            {
                                RISStationTypeEnum.BUS => (cfc.Type.ToString(), stType.ToString()),
                                RISStationTypeEnum.HEAVYTRUCK => (cfc.Type.ToString(), stType.ToString()),
                                RISStationTypeEnum.SPECIALVEHICLES => (cfc.Type.ToString(), stType.ToString()),
                                RISStationTypeEnum.CYCLIST => (cfc.Type.ToString(), stType.ToString()),
                                RISStationTypeEnum.PEDESTRIAN => (cfc.Type.ToString(), stType.ToString()),
                                _ => (null, null),
                            };
                            DefaultsProvider.Default.SetDefaultsOnModel(simData, selector1, selector2);
                            risLane.SimulatedStations.Add(new RISFaseCyclusLaneSimulatedStationViewModel(simData));
                        }
                    }
                }
            }
        });
        
        public ICommand GenerateRISSimulationDataCommand => _generateRISSimulationDataCommand ??= new RelayCommand(() =>
        {
            var rd = new Random();
            
            foreach (var risFc in RISFasen)
            {
                var numbers = new List<int> { 200, 100, 50 };
                var n = rd.Next(numbers.Count);
                var numbersLow = new List<int> { 2, 4 };
                var q1 = numbers[n];
                var r = numbers[n];
                numbers.Remove(r);
                n = rd.Next(numbers.Count);
                var q2 = numbers[n];
                r = numbers[n];
                numbers.Remove(r);
                var q3 = numbers[0];
                n = rd.Next(2);
                var q4 = numbersLow[n];

                foreach (var sim in risFc.Lanes.SelectMany(x => x.SimulatedStations))
                {
                    sim.Q1 = q1;
                    sim.Q2 = q2;
                    sim.Q3 = q3;
                    sim.Q4 = q4;

                    sim.Stopline = 1800;
                    
                    if (sim.FCNr?.ToUpper() != "NG")
                        sim.FCNr = risFc.FaseCyclus;
                }
            }
        });
        
        public ICommand AddSystemITFCommand => _addSystemITFCommand ??= new RelayCommand(
            () =>
            {
                MultiSystemITF.Add(new RISSystemITFViewModel(new RISSystemITFModel()));
            },
            () => HasMultipleSystemITF);


        public ICommand RemoveSystemITFCommand => _removeSystemITFCommand ??= new RelayCommand(
            () =>
            {
                MultiSystemITF.Remove(SelectedSystemITF);
                SelectedSystemITF = null;
            },
            () => HasMultipleSystemITF && SelectedSystemITF != null);

        #endregion // Commands

        private RISStationTypeEnum GetTypeForFase(FaseCyclusModel fc, string faseName = null)
        {
            fc ??= Controller.Fasen.FirstOrDefault(x => x.Naam == faseName);
            var t = RISStationTypeEnum.UNKNOWN;
            if (fc == null) return t;
            switch (fc.Type)
            {
                case FaseTypeEnum.Auto:
                    t = RISStationTypeEnum.MOTORVEHICLES;
                    break;
                case FaseTypeEnum.Fiets:
                    t = RISStationTypeEnum.CYCLIST;
                    break;
                case FaseTypeEnum.Voetganger:
                    t = RISStationTypeEnum.PEDESTRIAN;
                    break;
                case FaseTypeEnum.OV:
                    t = RISStationTypeEnum.MOTORVEHICLES;
                    break;
            }
            return t;
        }

        #region TLCGen messaging

        private void OnFasenChanged(FasenChangedMessage msg)
        {
            if (msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach (var fc in msg.RemovedFasen)
                {
                    var risFc = RISFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (risFc != null)
                    {
                        RISFasen.Remove(risFc);
                    }
                }
            }
            if (msg.AddedFasen != null && msg.AddedFasen.Any())
            {
                foreach (var fc in msg.AddedFasen)
                {
                    var risfc = new RISFaseCyclusDataViewModel(
                                new RISFaseCyclusDataModel { FaseCyclus = fc.Naam });
                    for (var i = 0; i < fc.AantalRijstroken; i++)
                    {
                        var sitf = SystemITF;
                        if (HasMultipleSystemITF)
                        {
                            sitf = MultiSystemITF.FirstOrDefault()?.SystemITF;
                        }
                        var l = new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i + 1, SystemITF = sitf });
                        risfc.Lanes.Add(l);
                    }
                    RISFasen.Add(risfc);
                    RISRequestLanes.Add(new RISLaneRequestDataViewModel(new RISLaneRequestDataModel
                    {
                        SignalGroupName = fc.Naam,
                        RijstrookIndex = 1,
                        Type = GetTypeForFase(fc)
                    }));
                    RISExtendLanes.Add(new RISLaneExtendDataViewModel(new RISLaneExtendDataModel
                    {
                        SignalGroupName = fc.Naam,
                        RijstrookIndex = 1,
                        Type = GetTypeForFase(fc)
                    }));
                    MessengerInstance.Send(new ControllerDataChangedMessage());
                }
            }
            RISFasen.BubbleSort();
            RISRequestLanes.BubbleSort();
            RISExtendLanes.BubbleSort();
            RISPelotonLanes.BubbleSort();
            UpdateRISLanes();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if (msg.ObjectType == TLCGenObjectTypeEnum.Fase)
            {
                RISFasen.Rebuild();
                RISRequestLanes.Rebuild();
                RISExtendLanes.Rebuild();
                RISLanes.BubbleSort();
                RISFasen.BubbleSort();
                RISRequestLanes.BubbleSort();
                RISExtendLanes.BubbleSort();
                RISPelotonLanes.BubbleSort();
            }
        }

        private void OnAantalRijstrokenChanged(FaseAantalRijstrokenChangedMessage obj)
        {
            var risfc = RISFasen.FirstOrDefault(x => x.FaseCyclus == obj.Fase.Naam);
            if (risfc != null)
            {
                if (obj.AantalRijstroken > risfc.Lanes.Count)
                {
                    var i = risfc.Lanes.Count;
                    for (; i < obj.AantalRijstroken; i++)
                    {
                        var sitf = SystemITF;
                        if (HasMultipleSystemITF)
                        {
                            sitf = MultiSystemITF.FirstOrDefault()?.SystemITF;
                        }
                        var l = new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = obj.Fase.Naam, RijstrookIndex = i + 1, SystemITF = sitf });
                        risfc.Lanes.Add(l);
                    }
                }
                else if (obj.AantalRijstroken < risfc.Lanes.Count)
                {
                    var i = risfc.Lanes.Count - obj.AantalRijstroken;
                    for (var j = 0; j < i; j++)
                    {
                        if (risfc.Lanes.Any())
                        {
                            risfc.Lanes.Remove(risfc.Lanes.Last());
                        }
                    }
                    var rem = RISRequestLanes.Where(x => x.SignalGroupName == obj.Fase.Naam && x.RijstrookIndex >= obj.AantalRijstroken).ToList();
                    foreach (var r in rem) RISRequestLanes.Remove(r);
                    var rem2 = RISExtendLanes.Where(x => x.SignalGroupName == obj.Fase.Naam && x.RijstrookIndex >= obj.AantalRijstroken).ToList();
                    foreach (var r in rem2) RISExtendLanes.Remove(r);
                    var rem3 = RISPelotonLanes.Where(x => x.SignalGroupName == obj.Fase.Naam && x.RijstrookIndex >= obj.AantalRijstroken).ToList();
                    foreach (var r in rem3) RISPelotonLanes.Remove(r);
                }
            }
            foreach (var lre in RISRequestLanes.Where(x => x.SignalGroupName == obj.Fase.Naam)) lre.UpdateRijstroken();
            foreach (var lre in RISExtendLanes.Where(x => x.SignalGroupName == obj.Fase.Naam)) lre.UpdateRijstroken();
            foreach (var lre in RISPelotonLanes.Where(x => x.SignalGroupName == obj.Fase.Naam)) lre.UpdateRijstroken();
            UpdateRISLanes();
        }

        private void OnSystemITFChanged(SystemITFChangedMessage msg)
        {
            foreach (var l in RISLanes)
            {
                if (l.SystemITF == msg.OldSystemITF) l.SystemITF = msg.NewdSystemITF;
            }
        }

        #endregion // TLCGen messaging

        #region Private Methods 

        internal void UpdateRISLanes()
        {
            RISLanes.Clear();
            foreach (var fc in RISFasen)
            {
                foreach (var l in fc.Lanes)
                {
                    RISLanes.Add(l);
                }
            }
        }

        internal void UpdateModel()
        {
            if (Controller != null && _risModel != null)
            {
                var sitf = SystemITF;
                if (HasMultipleSystemITF)
                {
                    var msitf = MultiSystemITF.FirstOrDefault();
                    if (msitf != null)
                    {
                        sitf = msitf.SystemITF;
                    }
                }
                foreach (var fc in Controller.Fasen)
                {
                    if (RISFasen.All(x => x.FaseCyclus != fc.Naam))
                    {
                        var risfc = new RISFaseCyclusDataViewModel(
                                new RISFaseCyclusDataModel { FaseCyclus = fc.Naam });
                        for (var i = 0; i < fc.AantalRijstroken; i++)
                        {
                            risfc.Lanes.Add(new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i + 1, SystemITF = sitf }));
                        }
                        RISFasen.Add(risfc);
                    }
                    else
                    {
                        var risfc = RISFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                        if (risfc != null)
                        {
                            if (fc.AantalRijstroken > risfc.Lanes.Count)
                            {
                                var i = risfc.Lanes.Count;
                                for (; i < fc.AantalRijstroken; i++)
                                {
                                    risfc.Lanes.Add(new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i + 1, SystemITF = sitf }));
                                }
                            }
                            else if (fc.AantalRijstroken < risfc.Lanes.Count)
                            {
                                var i = risfc.Lanes.Count - fc.AantalRijstroken;
                                for (var j = 0; j < i; j++)
                                {
                                    if (risfc.Lanes.Any())
                                        risfc.Lanes.Remove(risfc.Lanes.Last());
                                }
                                var rem = _risModel.RISRequestLanes.Where(x => x.SignalGroupName == fc.Naam && x.RijstrookIndex >= fc.AantalRijstroken).ToList();
                                foreach (var r in rem) _risModel.RISRequestLanes.Remove(r);
                                var rem2 = _risModel.RISExtendLanes.Where(x => x.SignalGroupName == fc.Naam && x.RijstrookIndex >= fc.AantalRijstroken).ToList();
                                foreach (var r in rem2) _risModel.RISExtendLanes.Remove(r);
                                var rem3 = _risModel.RISPelotonLanes.Where(x => x.SignalGroupName == fc.Naam && x.RijstrookIndex >= fc.AantalRijstroken).ToList();
                                foreach (var r in rem3) _risModel.RISPelotonLanes.Remove(r);
                            }
                        }
                    }
                }
                var rems = RISFasen.Where(x => Controller.Fasen.All(x2 => x2.Naam != x.FaseCyclus)).ToList(); 
                foreach (var sg in rems)
                {
                    RISFasen.Remove(sg);
                }
                RISFasen.BubbleSort();
                foreach (var lre in RISRequestLanes) lre.UpdateRijstroken();
                foreach (var lre in RISExtendLanes) lre.UpdateRijstroken();
                foreach (var lre in RISPelotonLanes) lre.UpdateRijstroken();
                RISRequestLanes.BubbleSort();
                RISExtendLanes.BubbleSort();
                RISPelotonLanes.BubbleSort();
                UpdateRISLanes();
                RaisePropertyChanged("");
            }
        }

        internal static RISFaseCyclusLaneSimulatedStationViewModel GetNewStationForSignalGroup(FaseCyclusModel sg, int laneId, int rijstrookIndex, string systemITF)
        {
            var st = new RISFaseCyclusLaneSimulatedStationViewModel(new RISFaseCyclusLaneSimulatedStationModel())
            {
                StationData =
                {
                    SignalGroupName = sg.Naam,
                    RijstrookIndex = rijstrookIndex,
                    LaneID = laneId,
                    SystemITF = systemITF
                }
            };
            switch (sg.Type)
            {
                case FaseTypeEnum.Auto:
                    st.StationType = RISStationTypeSimEnum.PASSENGERCAR;
                    st.Flow = 200;
                    st.Snelheid = 50;
                    break;
                case FaseTypeEnum.Fiets:
                    st.StationType = RISStationTypeSimEnum.CYCLIST;
                    st.Flow = 20;
                    st.Snelheid = 15;
                    break;
                case FaseTypeEnum.Voetganger:
                    st.StationType = RISStationTypeSimEnum.PEDESTRIAN;
                    st.Flow = 20;
                    st.Snelheid = 5;
                    break;
                case FaseTypeEnum.OV:
                    st.StationType = RISStationTypeSimEnum.BUS;
                    st.Flow = 10;
                    st.Snelheid = 45;
                    break;
            }
            st.StationData.SimulationData.RelatedName = st.StationData.Naam;
            st.StationData.SimulationData.FCNr = sg.Naam;
            return st;
        }

        #endregion // Private Methods 

        #region Public Methods

        #endregion // Public Methods

        #region Constructor

        public FasenRISTabViewModel()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<FaseAantalRijstrokenChangedMessage>(this, OnAantalRijstrokenChanged);
            MessengerInstance.Register<SystemITFChangedMessage>(this, OnSystemITFChanged);
        }

        #endregion // Constructor
    }
}
