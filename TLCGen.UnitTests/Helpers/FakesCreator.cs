using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.DataAccess;
using TLCGen.Dependencies.Providers;
using TLCGen.Integrity;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests
{
    public static class FakesCreator
    {
		public static ISettingsProvider CreateSettingsProvider()
        {
            var _settingsprovider = Substitute.For<ISettingsProvider>();
            return _settingsprovider;
        }

        public static IMessenger CreateMessenger(ControllerModel controller = null)
        {
            return Substitute.For<IMessenger>();
        }

        public static IDefaultsProvider CreateDefaultsProvider()
        {
            var defaultsProvider = Substitute.For<IDefaultsProvider>();
            defaultsProvider.Defaults = new TLCGenDefaultsModel();
            defaultsProvider.Defaults.Defaults.Add(new TLCGenDefaultModel());
            return defaultsProvider;
        }

        public static ITLCGenControllerModifier CreateControllerModifier()
        {
            var controllerModifier = Substitute.For<ITLCGenControllerModifier>();
            return controllerModifier;
        }

        public static ITLCGenControllerDataProvider CreateControllerDataProvider(ControllerModel m)
        {
            var controllerDataProvider = Substitute.For<ITLCGenControllerDataProvider>();
            controllerDataProvider.Controller.Returns(m);
            return controllerDataProvider;
        }

        public static IControllerAccessProvider CreateControllerAccessProvider(ControllerModel m)
        {
            var allSgs = new ObservableCollection<FaseCyclusViewModel>(m.Fasen.Select(x => new FaseCyclusViewModel(x)));
            var controllerAccessProvider = Substitute.For<IControllerAccessProvider>();
            controllerAccessProvider.Controller.Returns(m);
            controllerAccessProvider.AllSignalGroups.Returns(allSgs);
            return controllerAccessProvider;
        }

        public static ITemplatesProvider CreateTemplatesProvider()
        {
            var templatesProvider = Substitute.For<ITemplatesProvider>();
            return templatesProvider;
        }

        public static ITLCGenModelManager CreateModelManager(ControllerModel m)
        {
            var modelManager = Substitute.For<ITLCGenModelManager>();
            modelManager.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, "")
                .ReturnsForAnyArgs(x => TLCGenIntegrityChecker.IsElementNaamUnique(m, (string)x[1], (TLCGenObjectTypeEnum)x[0]));
            return modelManager;
        }

        public static ITLCGenDialogProvider CreateDialogProvider()
        {
            return Substitute.For<ITLCGenDialogProvider>();
        }

        public static ITLCGenFileAccessProvider CreateFileAccessProvider()
        {
            return Substitute.For<ITLCGenFileAccessProvider>();
        }
    }
}
