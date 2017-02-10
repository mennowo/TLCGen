using System;
using System.Linq;
using NUnit.Framework;
using TLCGen.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests
{
    public class FasenDetailsTabViewModelTests
    {
        IMessenger messenger;
        ISettingsProvider settingsprovider;
        ControllerModel model;

        [SetUp]
        public void FasenDetailsTabSetup()
        {
            model = new ControllerModel();
            messenger = Substitute.For<IMessenger>();
            settingsprovider = Substitute.For<ISettingsProvider>();
            messenger.
                When(x => x.Send(Arg.Any<IsElementIdentifierUniqueRequest>())).
                Do(c =>
                {
                    c.Arg<IsElementIdentifierUniqueRequest>().Handled = true;
                    c.Arg<IsElementIdentifierUniqueRequest>().IsUnique = model.Fasen.All(x =>
                    {
                        return c.Arg<IsElementIdentifierUniqueRequest>().Type == ElementIdentifierType.Naam && x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier;
                    });
                });
            Messenger.OverrideDefault(messenger);
            SettingsProvider.OverrideDefault(settingsprovider);
        }

        [Test]
        public void FasenDetailsTabSelected_ControllerHas5Fasen_TabAlsoExposes5Fasen()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenDetailsTabViewModel();
            vm.Controller = model;

            vm.OnSelected();

            Assert.AreEqual(5, vm.Fasen.Count);
        }

        [Test]
        public void FasenDetailsTabSelectedFase_TabDeselectedAndSelected_SelectedFaseEqual()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenDetailsTabViewModel();
            vm.Controller = model;

            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[3];
            vm.OnDeselected();
            vm.OnSelected();

            Assert.True(object.ReferenceEquals(vm.SelectedFaseCyclus, vm.Fasen[3]));
        }
    }
}
