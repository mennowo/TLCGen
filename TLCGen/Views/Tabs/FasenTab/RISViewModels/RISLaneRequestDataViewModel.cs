using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;


namespace TLCGen.ViewModels
{
    public class RISLaneRequestDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        private RISLaneRequestDataModel _laneData;
        
        public bool RISAanvraag
        {
            get => _laneData.RISAanvraag;
            set
            {
                _laneData.RISAanvraag = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AanvraagStart
        {
            get => _laneData.AanvraagStart;
            set
            {
                _laneData.AanvraagStart = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AanvraagEnd
        {
            get => _laneData.AanvraagEnd;
            set
            {
                _laneData.AanvraagEnd = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public int AanvraagStartSrm0
        {
            get => _laneData.AanvraagStartSrm0;
            set
            {
                _laneData.AanvraagStartSrm0 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int AanvraagEndSrm0
        {
            get => _laneData.AanvraagEndSrm0;
            set
            {
                _laneData.AanvraagEndSrm0 = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string SignalGroupName
        {
            get => _laneData.SignalGroupName;
            set
            {
                _laneData.SignalGroupName = value;
                UpdateRijstroken();
                OnPropertyChanged(broadcast: true);
            }
        }

        public int RijstrookIndex
        {
            get => _laneData.RijstrookIndex;
            set
            {
                _laneData.RijstrookIndex = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        
        public RISStationTypeEnum Type
        {
            get => _laneData.Type;
            set
            {
                _laneData.Type = value;
                OnPropertyChanged(broadcast: true);
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
            OnPropertyChanged(nameof(Rijstroken));
        }

        public object GetItem()
        {
            return _laneData;
        }

        public int CompareTo(object obj)
        {
            var other = obj as RISLaneRequestDataViewModel;
            if(_laneData.SignalGroupName == other.SignalGroupName)
            {
                return RijstrookIndex.CompareTo(other.RijstrookIndex);
            }
            else
            {
                return string.CompareOrdinal(_laneData.SignalGroupName, other.SignalGroupName);
            }
        }

        public RISLaneRequestDataViewModel(RISLaneRequestDataModel laneData)
        {
            _laneData = laneData;
            UpdateRijstroken();
        }
    }
}
