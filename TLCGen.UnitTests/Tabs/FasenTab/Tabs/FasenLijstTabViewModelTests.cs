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
    [TestFixture]
    public class FasenLijstTabViewModelTests
    {
        IMessenger messengerstub;
        ISettingsProvider settingsproviderstub;
        ControllerModel model;

        [SetUp]
        public void FasenTabSetup()
        {
            model = new ControllerModel();
            messengerstub = Substitute.For<IMessenger>();
            settingsproviderstub = Substitute.For<ISettingsProvider>();
            messengerstub.
                When(x => x.Send(Arg.Any<IsElementIdentifierUniqueRequest>())).
                Do(c =>
                {
                    c.Arg<IsElementIdentifierUniqueRequest>().Handled = true;
                    c.Arg<IsElementIdentifierUniqueRequest>().IsUnique = model.Fasen.All(x =>
                    {
                        return c.Arg<IsElementIdentifierUniqueRequest>().Type == ElementIdentifierType.Naam && x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier;
                    });
                });
            Messenger.OverrideDefault(messengerstub);
            SettingsProvider.OverrideDefault(settingsproviderstub);
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
        public void RemoveFaseCommand_NoFasenPresent_CannotExecute()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.RemoveFaseCommand.CanExecute(null);
            
            Assert.False(result);
        }

        [Test]
        public void RemoveFaseCommand_FasenPresentAndSelected_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.SelectedFaseCyclus = vm.Fasen[0];
            bool result = vm.RemoveFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_NoFasenPresent_CanExecute()
        {
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_FasenPresent_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            bool result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void RenameFase_HigherThanOthers_SortsCorrectlyAfterTabChange()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.Fasen[2].Naam = "07";
            vm.OnDeselected();

            Assert.AreEqual("04", vm.Fasen[2].Naam);
            Assert.AreEqual("07", vm.Fasen[4].Naam);
        }

        [Test]
        public void RenameFase_LowerThanOthers_SortsCorrectlyAfterTabChange()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "07" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "09" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.Fasen[4].Naam = "05";
            vm.OnDeselected();

            Assert.AreEqual("05", vm.Fasen[2].Naam);
            Assert.AreEqual("08", vm.Fasen[4].Naam);
        }

        [Test]
        public void EditFaseInMultipleSelection_VasteAanvraagSetToAltijd_VasteAanvraagAltijdForAllSelectedItems()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);

            vm.SelectedFaseCycli.Add(vm.Fasen[0]);
            vm.SelectedFaseCycli.Add(vm.Fasen[1]);
            vm.SelectedFaseCycli.Add(vm.Fasen[2]);
            vm.SelectedFaseCycli.Add(vm.Fasen[3]);
            vm.SelectedFaseCycli.Add(vm.Fasen[4]);
            vm.Fasen[4].VasteAanvraag = NooitAltijdAanUitEnum.Altijd;

            Assert.AreEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[0].VasteAanvraag);
            Assert.AreEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[1].VasteAanvraag);
            Assert.AreEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[2].VasteAanvraag);
            Assert.AreEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[3].VasteAanvraag);
            Assert.AreEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[4].VasteAanvraag);
        }

        [Test]
        public void EditFaseInMultipleSelection_VasteAanvraagSetToAltijd_VasteAanvraagNotAltijdForNonSelectedItems()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            FasenLijstTabViewModel vm = new FasenLijstTabViewModel(model);
            
            vm.SelectedFaseCycli.Add(vm.Fasen[1]);
            vm.SelectedFaseCycli.Add(vm.Fasen[2]);
            vm.SelectedFaseCycli.Add(vm.Fasen[3]);
            vm.Fasen[3].VasteAanvraag = NooitAltijdAanUitEnum.Altijd;

            Assert.AreNotEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[0].VasteAanvraag);
            Assert.AreNotEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[4].VasteAanvraag);
        }
    }
}
