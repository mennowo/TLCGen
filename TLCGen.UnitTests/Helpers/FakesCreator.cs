using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.Integrity;
using TLCGen.Models;
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

        public static ITemplatesProvider CreateTemplatesProvider()
        {
            var _templatesprovider = Substitute.For<ITemplatesProvider>();
            return _templatesprovider;
        }
    }
}
