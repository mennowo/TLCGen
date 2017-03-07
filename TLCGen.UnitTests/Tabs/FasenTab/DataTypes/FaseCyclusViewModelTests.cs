using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.DataTypes
{
    [TestFixture]
    public class FaseCyclusViewModelTests
    {
        [Test]
        public void FaseCyclusViewModel_NameChanged_NameChangedMessageSent()
        {
            var messengermock = FakesCreator.CreateMessenger();
            Messenger.OverrideDefault(messengermock);
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FaseCyclusViewModel(model.Fasen[2]);

            vm.Naam = "07";

            messengermock.Received().Send(Arg.Is<NameChangedMessage>(x => x.OldName == "03" && x.NewName == "07"));
        }

        [Test]
        public void FaseCyclusViewModel_TypeChanged_SetDefaultsOnModelCalled()
        {
            var defaultsprovidermock = FakesCreator.CreateDefaultsProvider();
            DefaultsProvider.OverrideDefault(defaultsprovidermock);
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Type = Models.Enumerations.FaseTypeEnum.Auto });
            var vm = new FaseCyclusViewModel(model.Fasen[0]);

            vm.Type = Models.Enumerations.FaseTypeEnum.Fiets;

            defaultsprovidermock.Received().SetDefaultsOnModel(Arg.Is<FaseCyclusModel>(x => x.Naam == "01"), Arg.Is<string>("Fiets"));
        }
    }
}
