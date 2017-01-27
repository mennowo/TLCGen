using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.Helpers;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Messaging.Messages;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesFasenInstellingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private FaseCyclusViewModel _SelectedFaseCyclus;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<FaseCyclusModuleDataViewModel, FaseCyclusModuleDataModel> Fasen
        {
            get; private set;
        }

        public FaseCyclusViewModel SelectedFaseCyclus
        {
            get { return _SelectedFaseCyclus; }
            set
            {
                _SelectedFaseCyclus = value;
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Fasen";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            
        }

        #endregion // TabItem Overrides

        #region TLCGen Message Handling

        private void OnFasenChanged(FasenChangedMessage message)
        {
            if(message.AddedFasen != null && message.AddedFasen.Count > 0)
            {
                foreach(var f in message.AddedFasen)
                {
                    Fasen.Add(new FaseCyclusModuleDataViewModel(new FaseCyclusModuleDataModel() { FaseCyclus = f.Naam }));
                }
            }

            if(message.RemovedFasen != null && message.RemovedFasen.Count > 0)
            {
                foreach(var f in message.RemovedFasen)
                {
                    FaseCyclusModuleDataViewModel fcvm = null;
                    foreach(var _f in Fasen)
                    {
                        if(f.Naam == _f.FaseCyclus)
                        {
                            fcvm = _f;
                        }
                    }
                    if (fcvm != null)
                    {
                        Fasen.Remove(fcvm);
                    }
                }
            }
        }

        #endregion // TLCGen Message Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesFasenInstellingenTabViewModel(ControllerModel controller) : base(controller)
        {
            Fasen = new ObservableCollectionAroundList<FaseCyclusModuleDataViewModel, FaseCyclusModuleDataModel>(controller.ModuleMolen.FasenModuleData);

            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
        }

        #endregion // Constructor
    }
}
