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

        public bool RISToepassen
        {
            get => _RISModel.RISToepassen;
            set
            {
                _RISModel.RISToepassen = value;
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
                        risfc.SimulatieVM.Lanes.Add(new RISFaseCyclusLaneSimulatieViewModel(new RISFaseCyclusLaneSimulatieModel()));
                    }
                    RISFasen.Add(risfc);
                }
            }
            RISFasen.BubbleSort();
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
                if(obj.AantalRijstroken > risfc.SimulatieVM.Lanes.Count)
                {
                    var i = obj.AantalRijstroken - risfc.SimulatieVM.Lanes.Count;
                    for (int j = 0; j < i; j++)
                    {
                        risfc.SimulatieVM.Lanes.Add(new RISFaseCyclusLaneSimulatieViewModel(new RISFaseCyclusLaneSimulatieModel()));
                    }
                }
                else if (obj.AantalRijstroken < risfc.SimulatieVM.Lanes.Count)
                {
                    var i = risfc.SimulatieVM.Lanes.Count - obj.AantalRijstroken;
                    for (int j = 0; j < i; j++)
                    {
                        if(risfc.SimulatieVM.Lanes.Any())
                            risfc.SimulatieVM.Lanes.Remove(risfc.SimulatieVM.Lanes.Last());
                    }
                }
            }
        }

        #endregion // TLCGen messaging

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
