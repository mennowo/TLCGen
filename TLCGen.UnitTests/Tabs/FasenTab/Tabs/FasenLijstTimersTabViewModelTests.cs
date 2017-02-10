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
using System.Collections.Generic;

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class FasenLijstTimersTabViewModelTests
    {
        IMessenger messenger;
        ISettingsProvider settingsprovider;
        ControllerModel model;

        [SetUp]
        public void FasenTimersTabSetup()
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
        public void FasenTimersTabSelected_ControllerHas5Fasen_TabAlsoExposes5Fasen()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.OnSelected();

            Assert.AreEqual(5, vm.Fasen.Count);
        }

        [Test]
        public void FasenTimersTabSelectedFase_TabDeselectedAndSelected_SelectedFaseEqual()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[3];
            vm.OnDeselected();
            vm.OnSelected();

            Assert.True(object.ReferenceEquals(vm.SelectedFaseCyclus, vm.Fasen[3]));
        }

        [Test]
        public void FasenTimersTabMultipleSelectionEdit_TGLChangedOnOnePhase_ChangesAllSelected()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05", TGL = 30 });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.OnSelected();
            vm.SelectedFaseCycli = new List<FaseCyclusViewModel>() { vm.Fasen[1], vm.Fasen[2], vm.Fasen[3] };
            vm.Fasen[3].TGL = 50;

            Assert.AreEqual(30, vm.Fasen[0].TGL);
            Assert.AreEqual(50, vm.Fasen[1].TGL);
            Assert.AreEqual(50, vm.Fasen[2].TGL);
            Assert.AreEqual(50, vm.Fasen[3].TGL);
            Assert.AreEqual(30, vm.Fasen[4].TGL);
        }
    }
}
