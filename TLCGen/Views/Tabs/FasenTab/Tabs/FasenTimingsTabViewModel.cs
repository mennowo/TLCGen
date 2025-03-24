using TLCGen.Helpers;
using System;
using TLCGen.Messaging.Messages;
using System.Linq;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Plugins;
using TimingsFaseCyclusDataModel = TLCGen.Models.TimingsFaseCyclusDataModel;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: -1, type: TabItemTypeEnum.FasenTab)]
    public class FasenTimingsTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private TimingsFaseCyclusDataViewModel _selectedTimingsFase;
        private string _betaMsgId;
        private ControllerModel _controller;

        #endregion // Fields

        #region Properties

        public override string DisplayName => "Timings";
        
        public ImageSource Icon => null;
        
        public TimingsFaseCyclusDataViewModel SelectedTimingsFase
        {
            get => _selectedTimingsFase;
            set
            {
                _selectedTimingsFase = value;
                OnPropertyChanged();
            }
        }

        public override ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                if (_controller != null)
                {
                    TimingsFasen = new ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel>(_controller.TimingsData.TimingsFasen);
                    UpdateTimingsFasen();
                }
                OnPropertyChanged();
            }
        }

        public ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel> TimingsFasen { get; private set; }

        private void UpdateTimingsFasen()
        {
            var changed = false;
            foreach (var fc in _controller.Fasen)
            {
                if (TimingsFasen.All(x => x.FaseCyclus != fc.Naam))
                {
                    var model = new TimingsFaseCyclusDataModel { FaseCyclus = fc.Naam };
                    TimingsFasen.Add(new TimingsFaseCyclusDataViewModel(model));
                    changed = true;
                }
            }
            var remove = TimingsFasen.Where(x => _controller.Fasen.All(x2 => x2.Naam != x.FaseCyclus)).ToList();
            if (remove.Any()) changed = true;
            foreach (var r in remove)
            {
                TimingsFasen.Remove(r);
                _controller.TimingsData.TimingsFasen.Remove(r.TimingsFase);
            }
                
            if (changed) TimingsFasen.BubbleSort();
        }

        public bool TimingsToepassen
        {
            get => _controller.TimingsData.TimingsToepassen;
            set
            {
                _controller.TimingsData.TimingsToepassen = value;
                if (value)
                {
                    UpdateTimingsFasen();
                }
                OnPropertyChanged(broadcast: true);
                OnPropertyChanged(nameof(TimingsToepassenOK));
            }
        }
        
        public bool TimingsUsePredictions
        {
            get => _controller.TimingsData.TimingsUsePredictions;
            set
            {
                _controller.TimingsData.TimingsUsePredictions = value;
                OnPropertyChanged(broadcast: true);
                if (value)
                {
                    _betaMsgId = Guid.NewGuid().ToString();
                    var msg = new ControllerAlertMessage(_betaMsgId)
                    {
                        Background = Brushes.Lavender,
                        Shown = true,
                        Message = "***Let op!*** Timings voorspellingen functiontionaliteit bevindt zich in de bèta test fase.",
                        Type = ControllerAlertType.FromPlugin
                    };
                    TLCGenModelManager.Default.AddControllerAlert(msg);
                }
                else
                {
                    TLCGenModelManager.Default.RemoveControllerAlert(_betaMsgId);
                }
            }
        }

        public bool TimingsToepassenAllowed => _controller.Data.CCOLVersie >= TLCGen.Models.Enumerations.CCOLVersieEnum.CCOL9;
        
        public bool TimingsPredictionsToepassenAllowed => _controller.TimingsData.TimingsToepassen && _controller.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL110;

        public bool TimingsToepassenOK => TimingsToepassenAllowed && TimingsToepassen;

        #endregion // Properties

        #region TLCGen messaging

        private void OnFasenChanged(object sender, FasenChangedMessage msg)
        {
            if(msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach(var fc in msg.RemovedFasen)
                {
                    var timingsFc = TimingsFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if(timingsFc != null)
                    {
                        TimingsFasen.Remove(timingsFc);
                    }
                }
            }
            if (msg.AddedFasen != null && msg.AddedFasen.Any())
            {
                foreach (var fc in msg.AddedFasen)
                {
                    var timingsfc = new TimingsFaseCyclusDataViewModel(
                                new TimingsFaseCyclusDataModel { FaseCyclus = fc.Naam });
                    TimingsFasen.Add(timingsfc);
                }
            }
            TimingsFasen.BubbleSort();
        }

        #endregion // TLCGen messaging

        #region Private Methods 

        #endregion // Private Methods 

        #region Constructor

        public FasenTimingsTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
        }

        #endregion // Constructor
    }
}
