using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    [TLCGenTabItem(index: 3, type: TabItemTypeEnum.ModulesTab)]
    public class ModulesAlternatievenCVInstellingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<string> _ModuleNames;
        private ObservableCollection<string> _FasenNames;

        #endregion // Fields

        #region Properties

        public ModuleFaseCyclusAlternatiefViewModel[,] InstellingenMatrix { get; set; }

        public ObservableCollectionAroundList<FaseCyclusModuleDataViewModel, FaseCyclusModuleDataModel> Fasen
        {
            get; private set;
        }

        public ObservableCollection<string> FasenNames
        {
            get
            {
                if (_FasenNames == null)
                {
                    _FasenNames = new ObservableCollection<string>();
                }
                return _FasenNames;
            }
        }

        public ObservableCollection<string> ModuleNames
        {
            get
            {
                if (_ModuleNames == null)
                {
                    _ModuleNames = new ObservableCollection<string>();
                }
                return _ModuleNames;
            }
        }

        #endregion // Properties

        #region TabItem Overrides

        public override string DisplayName => "Alternatieven\nonder CV";

        public override bool IsEnabled
        {
            get => !_Controller.ModuleMolen.LangstWachtendeAlternatief;
            set { }
        }

        public override void OnSelected()
        {
            RebuildMatrix();
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
            RebuildMatrix();
        }

        private void OnFasenSorted(object sender, FasenSortedMessage message)
        {
            _Controller.ModuleMolen.FasenModuleData.BubbleSort();
            Fasen.Rebuild();
            RebuildMatrix();
        }

        private void OnNameChanged(object sender, NameChangedMessage message)
        {
            _Controller.ModuleMolen.FasenModuleData.BubbleSort();
            Fasen.Rebuild();
        }

        #endregion // TLCGen Message Handling

        #region Private Methods

        private void RebuildMatrix()
        {
            if(Fasen.Count == 0 || Controller.ModuleMolen.Modules.Count == 0) return;

            FasenNames.Clear();
            ModuleNames.Clear();
            OnPropertyChanged("ModuleNames");
            OnPropertyChanged("FasenNames");
            OnPropertyChanged("InstellingenMatrix");

            foreach (var fc in Fasen)
            {
                FasenNames.Add(fc.FaseCyclus);
            }
            foreach (var ml in Controller.ModuleMolen.Modules)
            {
                ModuleNames.Add(ml.Naam);
            }

            InstellingenMatrix = new ModuleFaseCyclusAlternatiefViewModel[Controller.ModuleMolen.Modules.Count,Fasen.Count];

            var iml = 0;
            foreach (var ml in Controller.ModuleMolen.Modules)
            {
                var ifc = 0;
                foreach (var fc in Fasen)
                {
                    foreach (var mlfc in ml.Fasen)
                    {
                        foreach (var amlfc in mlfc.Alternatieven.Where(x => x.FaseCyclus == fc.FaseCyclus))
                        {
                            if (InstellingenMatrix[iml, ifc] == null)
                            {
                                InstellingenMatrix[iml, ifc] =
                                    new ModuleFaseCyclusAlternatiefViewModel(amlfc)
                                    {
                                        Others = new List<ModuleFaseCyclusAlternatiefModel>()
                                    };
                            }
                            else
                            {
                                InstellingenMatrix[iml, ifc].Others.Add(amlfc);
                            }
                        }
                    }
                    ifc++;
                }
                ++iml;
            }
            OnPropertyChanged("ModuleNames");
            OnPropertyChanged("FasenNames");
            OnPropertyChanged("InstellingenMatrix");
        }

        #endregion // Private Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region Constructor

        public ModulesAlternatievenCVInstellingenTabViewModel()
        {
            WeakReferenceMessenger.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessenger.Default.Register<FasenSortedMessage>(this, OnFasenSorted);
            WeakReferenceMessenger.Default.Register<NameChangedMessage>(this, OnNameChanged);
            WeakReferenceMessenger.Default.Register<UpdateTabsEnabledMessage>(this, (o, x) => OnPropertyChanged(string.Empty));
        }

        #endregion // Constructor
    }
}
