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
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.Extensions;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesLangstwachtendeInstellingenTabViewModel : TLCGenTabItemViewModel
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
            get => _SelectedFaseCyclus;
            set
            {
                _SelectedFaseCyclus = value;
                RaisePropertyChanged("SelectedFaseCyclus");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Langstw.alt.";

        public override bool IsEnabled
        {
            get { return _Controller.ModuleMolen.LangstWachtendeAlternatief; }
            set { }
        }

        public override void OnSelected()
        {
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                Fasen = base.Controller != null ? new ObservableCollectionAroundList<FaseCyclusModuleDataViewModel, FaseCyclusModuleDataModel>(base.Controller.ModuleMolen.FasenModuleData) : null;
                RaisePropertyChanged("Fasen");
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Message Handling

        private void OnFasenChanged(FasenChangedMessage message)
        {
            Fasen.Rebuild();
        }

        private void OnFasenSorted(FasenSortedMessage message)
        {
            _Controller.ModuleMolen.FasenModuleData.BubbleSort();
            Fasen.Rebuild();
        }

        #endregion // TLCGen Message Handling

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesLangstwachtendeInstellingenTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
            MessengerInstance.Register(this, new Action<UpdateTabsEnabledMessage>(x => RaisePropertyChanged(string.Empty)));
        }

        #endregion // Constructor
    }
}
