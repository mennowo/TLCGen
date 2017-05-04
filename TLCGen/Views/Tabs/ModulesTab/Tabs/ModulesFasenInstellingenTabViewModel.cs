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
                RaisePropertyChanged("SelectedFaseCyclus");
            }
        }
        
        public bool HasAlternatieven
        {
            get { return _Controller.ModuleMolen.LangstWachtendeAlternatief; }
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
            RaisePropertyChanged("HasAlternatieven");
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                if(base.Controller != null)
                {
                    Fasen = new ObservableCollectionAroundList<FaseCyclusModuleDataViewModel, FaseCyclusModuleDataModel>(base.Controller.ModuleMolen.FasenModuleData);
                }
                else
                {
                    Fasen = null;
                }
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

        public ModulesFasenInstellingenTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<FasenSortedMessage>(OnFasenSorted));
        }

        #endregion // Constructor
    }
}
