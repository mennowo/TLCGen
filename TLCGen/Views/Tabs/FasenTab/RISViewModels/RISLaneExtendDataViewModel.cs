using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RISLaneExtendDataViewModel : ViewModelBase, IViewModelWithItem, IComparable
    {
        private RISLaneExtendDataModel _laneData;
        
        public bool RISVerlengen
        {
            get => _laneData.RISVerlengen;
            set
            {
                _laneData.RISVerlengen = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenStart
        {
            get => _laneData.VerlengenStart;
            set
            {
                _laneData.VerlengenStart = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenEnd
        {
            get => _laneData.VerlengenEnd;
            set
            {
                _laneData.VerlengenEnd = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenStartSrm0
        {
            get => _laneData.VerlengenStartSrm0;
            set
            {
                _laneData.VerlengenStartSrm0 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int VerlengenEndSrm0
        {
            get => _laneData.VerlengenEndSrm0;
            set
            {
                _laneData.VerlengenEndSrm0 = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }
        
        public string SignalGroupName
        {
            get => _laneData.SignalGroupName;
            set
            {
                _laneData.SignalGroupName = value;
                UpdateRijstroken();
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public int RijstrookIndex
        {
            get => _laneData.RijstrookIndex;
            set
            {
                _laneData.RijstrookIndex = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public RISStationTypeEnum Type
        {
            get => _laneData.Type;
            set
            {
                _laneData.Type = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public List<int> Rijstroken { get; private set; }

        public void UpdateRijstroken()
        {
            if (DataAccess.TLCGenControllerDataProvider.Default.Controller == null) return;
            var fc = DataAccess.TLCGenControllerDataProvider.Default.Controller.Fasen.FirstOrDefault(x => x.Naam == SignalGroupName);
            if (fc == null) return;
            Rijstroken = new List<int>();
            for (var i = 0; i < fc.AantalRijstroken; ++i)
            {
                Rijstroken.Add(i + 1);
            }
            if (!Rijstroken.Contains(RijstrookIndex)) RijstrookIndex = 1;
            RaisePropertyChanged(nameof(Rijstroken));
        }

        public object GetItem()
        {
            return _laneData;
        }

        public int CompareTo(object obj)
        {
            var other = obj as RISLaneExtendDataViewModel;
            if(_laneData.SignalGroupName == other.SignalGroupName)
            {
                return RijstrookIndex.CompareTo(other.RijstrookIndex);
            }
            else
            {
                return string.CompareOrdinal(_laneData.SignalGroupName, other.SignalGroupName);
            }
        }

        public RISLaneExtendDataViewModel(RISLaneExtendDataModel laneData)
        {
            _laneData = laneData;
            UpdateRijstroken();
        }
    }
}
