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

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class FasenLijstTabViewModelTests
    {
        IMessenger messenger;
        ISettingsProvider settingsprovider;
        ControllerModel model;

        [SetUp]
        public void FasenTabSetup()
        {
            model = new ControllerModel();
            messenger = Substitute.For<IMessenger>();
            settingsprovider = Substitute.For<ISettingsProvider>();
            messenger.
                When(x => x.Send(Arg.Any<IsElementIdentifierUniqueRequest>())).
                Do(c =>
                {
                    c.Arg<IsElementIdentifierUniqueRequest>().Handled = true;
                    c.Arg<IsElementIdentifierUniqueRequest>().IsUnique = model.Fasen.All(x => x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier);
                });
            Messenger.OverrideDefault(messenger);
            SettingsProvider.OverrideDefault(settingsprovider);
        }

        [Test]
        public void AddFaseCommand_Executed_AddsFase()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.AddFaseCommand.Execute(null);

            Assert.True(model.Fasen.Count == 1);
        }

        [Test]
        public void AddFaseCommand_Executed5Times_Adds5Fasen()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);

            Assert.True(model.Fasen.Count == 5);
        }

        [Test]
        public void AddFaseCommand_Executed5Times_5thFaseCorrectlyNamed()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);

            Assert.AreEqual("05", model.Fasen[4].Naam);
        }

        [Test]
        public void RemoveFaseCommand_ExecutedWithFaseSelected_RemovesFase()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);
            vm.SelectedFaseCyclus = vm.Fasen[0];

            vm.RemoveFaseCommand.Execute(null);

            Assert.True(model.Fasen.Count == 0);
        }

        [Test]
        public void RemoveFaseCommand_Executed5TimesWithFaseSelected_Removes5Fasen()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.SelectedFaseCyclus = vm.Fasen[0];
            vm.RemoveFaseCommand.Execute(null);
            vm.SelectedFaseCyclus = vm.Fasen[0];
            vm.RemoveFaseCommand.Execute(null);
            vm.SelectedFaseCyclus = vm.Fasen[0];
            vm.RemoveFaseCommand.Execute(null);
            vm.SelectedFaseCyclus = vm.Fasen[0];
            vm.RemoveFaseCommand.Execute(null);
            vm.SelectedFaseCyclus = vm.Fasen[0];
            vm.RemoveFaseCommand.Execute(null);

            Assert.True(model.Fasen.Count == 0);
        }

        [Test]
        public void RemoveFaseCommand_CanExecute_NoFasenPresent_CannotExecute()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.RemoveFaseCommand.CanExecute(null);
            
            Assert.False(result);
        }

        [Test]
        public void RemoveFaseCommand_CanExecute_FasenPresentAndSelected_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);
            vm.SelectedFaseCyclus = vm.Fasen[0];

            bool result = vm.RemoveFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_CanExecute_NoFasenPresent_CanExecute()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_CanExecute_FasenPresent_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void RenameFase_HigherThanOthers_SortsCorrectlyAfterTabChange()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", Define = "fc01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02", Define = "fc02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", Define = "fc03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04", Define = "fc04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05", Define = "fc05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.Fasen[2].Naam = "07";
            vm.OnDeselected();

            Assert.AreEqual("04", vm.Fasen[2].Naam);
            Assert.AreEqual("07", vm.Fasen[4].Naam);
        }

        [Test]
        public void RenameFase_LowerThanOthers_SortsCorrectlyAfterTabChange()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", Define = "fc01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04", Define = "fc02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "07", Define = "fc03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08", Define = "fc04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "09", Define = "fc05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.Fasen[4].Naam = "05";
            vm.OnDeselected();

            Assert.AreEqual("05", vm.Fasen[2].Naam);
            Assert.AreEqual("08", vm.Fasen[4].Naam);
        }
    }
}
