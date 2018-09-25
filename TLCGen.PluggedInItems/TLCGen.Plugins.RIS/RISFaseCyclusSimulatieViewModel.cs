using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public class RISFaseCyclusSimulatieViewModel : ViewModelBase
    {
        private RISFaseCyclusSimulatieModel _simulatieModel;
        private RISFaseCyclusLaneSimulatieViewModel _selectedLane;

        public ObservableCollectionAroundList<RISFaseCyclusLaneSimulatieViewModel, RISFaseCyclusLaneSimulatieModel> Lanes { get; }

        public RISFaseCyclusLaneSimulatieViewModel SelectedLane
        {
            get => _selectedLane;
            set
            {
                _selectedLane = value;
                RaisePropertyChanged();
            }
        }

        public RISFaseCyclusSimulatieViewModel(RISFaseCyclusSimulatieModel simulatieModel)
        {
            _simulatieModel = simulatieModel;
            Lanes = new ObservableCollectionAroundList<RISFaseCyclusLaneSimulatieViewModel, RISFaseCyclusLaneSimulatieModel>(simulatieModel.LaneData);
        }
    }
}
