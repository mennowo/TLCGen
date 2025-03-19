using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;


namespace TLCGen.ViewModels
{
    public class RISLanePelotonDataViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {
        private RISLanePelotonDataModel _laneData;

        public bool RISPelotonBepaling
        {
            get => _laneData.RISPelotonBepaling;
            set
            {
                _laneData.RISPelotonBepaling = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int PelotonBepalingStart
        {
            get => _laneData.PelotonBepalingStart;
            set
            {
                _laneData.PelotonBepalingStart = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public int PelotonBepalingEnd
        {
            get => _laneData.PelotonBepalingEnd;
            set
            {
                _laneData.PelotonBepalingEnd = value;
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
            if (obj is RISLanePelotonDataViewModel other)
            {
                if (_laneData.SignalGroupName == other.SignalGroupName)
                {
                    return RijstrookIndex.CompareTo(other.RijstrookIndex);
                }
                else
                {
                    return string.CompareOrdinal(_laneData.SignalGroupName, other.SignalGroupName);
                }
            }

            return 0;
        }

        public RISLanePelotonDataViewModel(RISLanePelotonDataModel laneData)
        {
            _laneData = laneData;
            UpdateRijstroken();
        }
    }
}