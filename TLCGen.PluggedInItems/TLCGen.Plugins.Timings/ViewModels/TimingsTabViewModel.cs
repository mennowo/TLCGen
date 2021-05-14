using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.Timings.Models;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System;
using TLCGen.Messaging.Messages;
using System.Linq;
using System.Windows.Media;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.Models;

namespace TLCGen.Plugins.Timings
{
    public class TimingsTabViewModel : ViewModelBase
    {
        #region Fields

        private readonly TimingsPlugin _plugin;
        private TimingsFaseCyclusDataViewModel _selectedTimingsFase;
        private TimingsDataModel _timingsModel;
        private string _betaMsgId;

        #endregion // Fields

        #region Properties

        public TimingsFaseCyclusDataViewModel SelectedTimingsFase
        {
            get => _selectedTimingsFase;
            set
            {
                _selectedTimingsFase = value;
                RaisePropertyChanged();
            }
        }

        public TimingsDataModel TimingsModel
        {
            get => _timingsModel;
            set
            {
                _timingsModel = value;
                TimingsFasen = new ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel>(_timingsModel.TimingsFasen);
            }
        }

        public ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel> TimingsFasen { get; private set; }

        public bool TimingsToepassen
        {
            get => _timingsModel.TimingsToepassen;
            set
            {
                _timingsModel.TimingsToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged(nameof(TimingsToepassenOK));
            }
        }
        
        public bool TimingsUsePredictions
        {
            get => _timingsModel.TimingsUsePredictions;
            set
            {
                _timingsModel.TimingsUsePredictions = value;
                RaisePropertyChanged<object>(broadcast: true);
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

        public bool TimingsToepassenAllowed => _plugin.Controller.Data.CCOLVersie >= TLCGen.Models.Enumerations.CCOLVersieEnum.CCOL9;

        public bool TimingsToepassenOK => TimingsToepassenAllowed && TimingsToepassen;

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
                    var TimingsFc = TimingsFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if(TimingsFc != null)
                    {
                        TimingsFasen.Remove(TimingsFc);
                    }
                }
            }
            if (msg.AddedFasen != null && msg.AddedFasen.Any())
            {
                foreach (var fc in msg.AddedFasen)
                {
                    var Timingsfc = new TimingsFaseCyclusDataViewModel(
                                new TimingsFaseCyclusDataModel { FaseCyclus = fc.Naam });
                    TimingsFasen.Add(Timingsfc);
                }
            }
            TimingsFasen.BubbleSort();
        }

        private void OnNameChanged(NameChangedMessage msg)
        {
            if(msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(TimingsModel, msg.OldName, msg.NewName, TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase);
                TimingsFasen.Rebuild();
                TimingsFasen.BubbleSort();
            }
        }

        #endregion // TLCGen messaging

        #region Private Methods 

        #endregion // Private Methods 

        #region Public Methods

        public void UpdateMessaging()
        {
            MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
            MessengerInstance.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Public Methods

        #region Constructor

        public TimingsTabViewModel(TimingsPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
