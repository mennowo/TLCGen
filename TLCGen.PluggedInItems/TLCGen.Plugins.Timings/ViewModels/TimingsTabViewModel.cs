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
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.Plugins.Timings
{
    public class TimingsTabViewModel : ViewModelBase
    {
        #region Fields

        private TimingsPlugin _plugin;

        #endregion // Fields

        #region Properties

        private TimingsFaseCyclusDataViewModel _selectedTimingsFase;
        public TimingsFaseCyclusDataViewModel SelectedTimingsFase
        {
            get => _selectedTimingsFase;
            set
            {
                _selectedTimingsFase = value;
                RaisePropertyChanged();
            }
        }

        private TimingsDataModel _TimingsModel;
        public TimingsDataModel TimingsModel
        {
            get => _TimingsModel;
            set
            {
                _TimingsModel = value;
                TimingsFasen = new ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel>(_TimingsModel.TimingsFasen);
            }
        }

        public ObservableCollectionAroundList<TimingsFaseCyclusDataViewModel, TimingsFaseCyclusDataModel> TimingsFasen { get; private set; }

        public bool TimingsToepassen
        {
            get => _TimingsModel.TimingsToepassen;
            set
            {
                _TimingsModel.TimingsToepassen = value;
                RaisePropertyChanged<object>(broadcast: true);
                RaisePropertyChanged("TimingsToepassenOK");
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
