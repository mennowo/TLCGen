using System;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Models;
using TLCGen.Helpers;

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
                OnPropertyChanged("SelectedFaseCyclus");
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Langstw.alt.";

        public override bool IsEnabled
        {
            get => _Controller?.ModuleMolen?.LangstWachtendeAlternatief == true;
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

	            if (Fasen != null)
	            {
		            foreach (var fc in Controller.Fasen)
		            {
			            if (Fasen.All(x => x.FaseCyclus != fc.Naam))
			            {
				            var data = new FaseCyclusModuleDataModel();
				            DefaultsProvider.Default.SetDefaultsOnModel(data);
				            data.FaseCyclus = fc.Naam;
				            Fasen.Add(new FaseCyclusModuleDataViewModel(data));
			            }
		            }
	            }

	            OnPropertyChanged("Fasen");
            }
        }

        #endregion // TabItem Overrides

        #region TLCGen Message Handling

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            Fasen.Rebuild();
        }

        private void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            _Controller.ModuleMolen.FasenModuleData.BubbleSort();
            Fasen.Rebuild();
        }

        private void OnNameChanged(object sender, NameChangedMessage message)
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
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessengerEx.Default.Register<UpdateTabsEnabledMessage>(this, (o, x) => OnPropertyChanged(string.Empty));
        }

        #endregion // Constructor
    }
}
