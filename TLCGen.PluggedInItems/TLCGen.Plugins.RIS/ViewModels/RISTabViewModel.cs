using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System;
using TLCGen.Messaging.Messages;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.Plugins.RIS
{
    public class RISTabViewModel : ViewModelBase
    {
        #region Fields

        private RISPlugin _plugin;

        #endregion // Fields

        #region Properties

        private RISFaseCyclusDataViewModel _selectedRISFase;
        public RISFaseCyclusDataViewModel SelectedRISFase
        {
            get => _selectedRISFase;
            set
            {
                _selectedRISFase = value;
                RaisePropertyChanged();
            }
        }

        private RISDataModel _RISModel;
        public RISDataModel RISModel
        {
            get => _RISModel;
            set
            {
                _RISModel = value;
                RISFasen = new ObservableCollectionAroundList<RISFaseCyclusDataViewModel, RISFaseCyclusDataModel>(_RISModel.RISFasen);
            }
        }

        public ObservableCollectionAroundList<RISFaseCyclusDataViewModel, RISFaseCyclusDataModel> RISFasen { get; private set; }

        public ObservableCollection<RISFaseCyclusLaneDataViewModel> RISLanes { get; } = new ObservableCollection<RISFaseCyclusLaneDataViewModel>();

        public bool RISToepassen
        {
            get => _RISModel.RISToepassen;
            set
            {
                _RISModel.RISToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
                if (string.IsNullOrWhiteSpace(SystemITF)) SystemITF = _plugin.Controller.Data.Naam;
                foreach (var fc in RISFasen)
                {
                    var sg = _plugin.Controller.Fasen.First(x => x.Naam == fc.FaseCyclus);
                    if (sg != null && fc.Lanes.Any())
                    {
                        foreach (var l in fc.Lanes)
                        {
                            if (l.SimulatedStations.Any())
                            {

                                switch (sg.Type)
                                {
                                    case TLCGen.Models.Enumerations.FaseTypeEnum.Auto:
                                        if (l.SimulatedStations[0].Type != RISStationTypeEnum.PASSENGERCAR) l.SimulatedStations[0].Type = RISStationTypeEnum.PASSENGERCAR;
                                        break;
                                    case TLCGen.Models.Enumerations.FaseTypeEnum.Fiets:
                                        if (l.SimulatedStations[0].Type != RISStationTypeEnum.CYCLIST) l.SimulatedStations[0].Type = RISStationTypeEnum.CYCLIST;
                                        break;
                                    case TLCGen.Models.Enumerations.FaseTypeEnum.Voetganger:
                                        if (l.SimulatedStations[0].Type != RISStationTypeEnum.PEDESTRIAN) l.SimulatedStations[0].Type = RISStationTypeEnum.PEDESTRIAN;
                                        break;
                                    case TLCGen.Models.Enumerations.FaseTypeEnum.OV:
                                        if (l.SimulatedStations[0].Type != RISStationTypeEnum.BUS) l.SimulatedStations[0].Type = RISStationTypeEnum.BUS;
                                        break;
                                }
                            }
                            else
                            {
                                l.SimulatedStations.Add(RISPlugin.GetNewStationForSignalGroup(sg));
                            }
                        }
                    }
                }
                UpdateRISLanes();
                for (int l = 0; l < RISLanes.Count; l++)
                {
                    RISLanes[l].LaneID = l;
                }
            }
        }

        public string SystemITF
        {
            get => _RISModel.SystemITF;
            set
            {
                _RISModel.SystemITF = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region TLCGen messaging

        private void OnFasenChanged(FasenChangedMessage msg)
        {
            if(msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach(var fc in msg.RemovedFasen)
                {
                    var RISFc = RISFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if(RISFc != null)
                    {
                        RISFasen.Remove(RISFc);
                    }
                }
            }
            if (msg.AddedFasen != null && msg.AddedFasen.Any())
            {
                foreach (var fc in msg.AddedFasen)
                {
                    var risfc = new RISFaseCyclusDataViewModel(
                                new RISFaseCyclusDataModel { FaseCyclus = fc.Naam });
                    for (int i = 0; i < fc.AantalRijstroken; i++)
                    {
                        var l = new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = fc.Naam, RijstrookIndex = i });
                        l.SimulatedStations.Add(RISPlugin.GetNewStationForSignalGroup(fc));
                        risfc.Lanes.Add(l);
                    }
                    RISFasen.Add(risfc);
                }
            }
            RISFasen.BubbleSort();
            UpdateRISLanes();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if(msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(RISModel, msg.OldName, msg.NewName);
                RISFasen.Rebuild();
                RISFasen.BubbleSort();
            }
        }

        private void OnAantalRijstrokenChanged(FaseAantalRijstrokenChangedMessage obj)
        {
            var risfc = RISFasen.FirstOrDefault(x => x.FaseCyclus == obj.Fase.Naam);
            if (risfc != null)
            {
                if(obj.AantalRijstroken > risfc.Lanes.Count)
                {
                    var i = risfc.Lanes.Count;
                    for (; i < obj.AantalRijstroken; i++)
                    {
                        var l = new RISFaseCyclusLaneDataViewModel(new RISFaseCyclusLaneDataModel() { SignalGroupName = obj.Fase.Naam, RijstrookIndex = i });
                        l.SimulatedStations.Add(RISPlugin.GetNewStationForSignalGroup(obj.Fase));
                        risfc.Lanes.Add(l);
                    }
                }
                else if (obj.AantalRijstroken < risfc.Lanes.Count)
                {
                    var i = risfc.Lanes.Count - obj.AantalRijstroken;
                    for (int j = 0; j < i; j++)
                    {
                        if (risfc.Lanes.Any())
                        {
                            risfc.Lanes.Remove(risfc.Lanes.Last());
                        }
                    }
                }
            }
            UpdateRISLanes();
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

        #endregion // Private Methods 

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
            MessengerInstance.Register<FaseAantalRijstrokenChangedMessage>(this, OnAantalRijstrokenChanged);
        }

        #endregion // Public Methods

        #region Constructor

        public RISTabViewModel(RISPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
