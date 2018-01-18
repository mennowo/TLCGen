using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Integrity;
using TLCGen.Messaging.Requests;
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

            // Send methods: fake checking for unique elements
            _messenger.
                When(x => x.Send(Arg.Any<IsElementIdentifierUniqueRequest>())).
                Do(c =>
                {
                    c.Arg<IsElementIdentifierUniqueRequest>().Handled = true;
	                if (controller == null)
	                {
		                c.Arg<IsElementIdentifierUniqueRequest>().IsUnique = true;
	                }
	                else
	                {
		                c.Arg<IsElementIdentifierUniqueRequest>().IsUnique =
			                controller.Fasen.All(x =>
				                c.Arg<IsElementIdentifierUniqueRequest>().Type == ElementIdentifierType.Naam &&
				                x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier) &&
			                controller.Fasen.SelectMany(x => x.Detectoren).All(x =>
				                c.Arg<IsElementIdentifierUniqueRequest>().Type == ElementIdentifierType.Naam &&
				                x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier);
	                }
                });

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
