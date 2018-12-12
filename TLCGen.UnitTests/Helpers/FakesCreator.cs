using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.DataAccess;
using TLCGen.Dependencies.Providers;
using TLCGen.Integrity;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

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
            var _messenger = Substitute.For<IMessenger>();
            return _messenger;
        }

        public static IDefaultsProvider CreateDefaultsProvider()
        {
            var _defaultprovider = Substitute.For<IDefaultsProvider>();
            _defaultprovider.Defaults = new TLCGenDefaultsModel();
            _defaultprovider.Defaults.Defaults.Add(new TLCGenDefaultModel());
            return _defaultprovider;
        }

        public static ITLCGenControllerModifier CreateControllerModifier()
        {
            var _controllermodifier = Substitute.For<ITLCGenControllerModifier>();
            return _controllermodifier;
        }

        public static ITLCGenControllerDataProvider CreateControllerDataProvider(ControllerModel m)
        {
            var _controllermodifier = Substitute.For<ITLCGenControllerDataProvider>();
            _controllermodifier.Controller.Returns(m);
            return _controllermodifier;
        }

        public static ITemplatesProvider CreateTemplatesProvider()
        {
            var _templatesprovider = Substitute.For<ITemplatesProvider>();
            return _templatesprovider;
        }

        public static ITLCGenModelManager CreateModelManager(ControllerModel m)
        {
            var _modelmanager = Substitute.For<ITLCGenModelManager>();
            _modelmanager.IsElementIdentifierUnique(TLCGenObjectTypeEnum.Fase, "")
                .ReturnsForAnyArgs(x => TLCGenIntegrityChecker.IsElementNaamUnique(m, (string)x[1]));
            return _modelmanager;
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
