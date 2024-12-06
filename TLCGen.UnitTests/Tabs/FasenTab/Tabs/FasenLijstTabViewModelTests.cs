using System.Linq;
using NUnit.Framework;
using TLCGen.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Models.Enumerations;
using TLCGen.Integrity;
using TLCGen.ModelManagement;

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class FasenLijstTabViewModelTests
    {
        [Test]
        public void AddFaseCommand_Executed_AddsFase()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(new Messenger());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            var vm = new FasenLijstTabViewModel {Controller = model};

            vm.AddFaseCommand.Execute(null);

            Assert.That(1 == model.Fasen.Count);
        }

        [Test]
        public void AddFaseCommand_Executed5Times_Adds5Fasen()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(new Messenger());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            var vm = new FasenLijstTabViewModel {Controller = model};

            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);

            Assert.That(5 == model.Fasen.Count);
        }

        [Test]
        public void AddFaseCommand_Executed5Times_5thFaseCorrectlyNamed()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(new Messenger());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            var vm = new FasenLijstTabViewModel {Controller = model};

            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);
            vm.AddFaseCommand.Execute(null);

            Assert.That("05" == model.Fasen[4].Naam);
        }

        [Test]
        public void RemoveFaseCommand_ExecutedWithFaseSelected_CallsRemoveSignalGroupFromController()
        {
            var model = new ControllerModel();
            var controllermodifiermock = FakesCreator.CreateControllerModifier();
            TLCGenControllerModifier.OverrideDefault(controllermodifiermock);
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;
            vm.SelectedFaseCyclus = vm.Fasen[0];

            vm.RemoveFaseCommand.Execute(null);

            controllermodifiermock.Received().RemoveModelItemFromController("01", TLCGenObjectTypeEnum.Fase);
        }

        [Test]
        public void RemoveFaseCommand_NoFasenPresent_CannotExecute()
        {
            var model = new ControllerModel();
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            var result = vm.RemoveFaseCommand.CanExecute(null);
            
            Assert.False(result);
        }

        [Test]
        public void RemoveFaseCommand_FasenPresentAndSelected_CanExecute()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.SelectedFaseCyclus = vm.Fasen[0];
            var result = vm.RemoveFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_NoFasenPresent_CanExecute()
        {
            var model = new ControllerModel();
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            var result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddFaseCommand_FasenPresent_CanExecute()
        {
            var model = new ControllerModel();
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            var result = vm.AddFaseCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void RenameFase_HigherThanOthers_SortsCorrectlyAfterTabChange()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(FakesCreator.CreateMessenger());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.Fasen[2].Naam = "07";
            vm.OnDeselectedPreview();

            Assert.AreEqual(
                new string[5] { "01", "02", "04", "05", "07" },
                vm.Fasen.Select(x => x.Naam).ToArray());
        }

        [Test]
        public void RenameFase_HigherThanOthers_SortsModelCorrectlyAfterTabChange()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(FakesCreator.CreateMessenger());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.Fasen[2].Naam = "07";
            vm.OnDeselectedPreview();

            Assert.AreEqual(
                new string[5] { "01", "02", "04", "05", "07" },
                model.Fasen.Select(x => x.Naam).ToArray());
        }

        [Test]
        public void RenameFase_LowerThanOthers_SortsCorrectlyAfterTabChange()
        {
            var model = new ControllerModel();
            Messenger.OverrideDefault(new Messenger());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "07" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "09" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.Fasen[4].Naam = "05";
            vm.OnDeselectedPreview();

            Assert.That("05" == vm.Fasen[2].Naam);
            Assert.That("08" == vm.Fasen[4].Naam);
        }

        [Test]
        public void EditFaseInMultipleSelection_VasteAanvraagSetToAltijd_VasteAanvraagAltijdForAllSelectedItems()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;

            vm.SelectedFaseCycli.Add(vm.Fasen[0]);
            vm.SelectedFaseCycli.Add(vm.Fasen[1]);
            vm.SelectedFaseCycli.Add(vm.Fasen[2]);
            vm.SelectedFaseCycli.Add(vm.Fasen[3]);
            vm.SelectedFaseCycli.Add(vm.Fasen[4]);
            vm.Fasen[4].VasteAanvraag = NooitAltijdAanUitEnum.Altijd;

            Assert.That(NooitAltijdAanUitEnum.Altijd == vm.Fasen[0].VasteAanvraag);
            Assert.That(NooitAltijdAanUitEnum.Altijd == vm.Fasen[1].VasteAanvraag);
            Assert.That(NooitAltijdAanUitEnum.Altijd == vm.Fasen[2].VasteAanvraag);
            Assert.That(NooitAltijdAanUitEnum.Altijd == vm.Fasen[3].VasteAanvraag);
            Assert.That(NooitAltijdAanUitEnum.Altijd == vm.Fasen[4].VasteAanvraag);
        }

        [Test]
        public void EditFaseInMultipleSelection_VasteAanvraagSetToAltijd_VasteAanvraagNotAltijdForNonSelectedItems()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTabViewModel();
            vm.Controller = model;
            
            vm.SelectedFaseCycli.Add(vm.Fasen[1]);
            vm.SelectedFaseCycli.Add(vm.Fasen[2]);
            vm.SelectedFaseCycli.Add(vm.Fasen[3]);
            vm.Fasen[3].VasteAanvraag = NooitAltijdAanUitEnum.Altijd;

            Assert.AreNotEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[0].VasteAanvraag);
            Assert.AreNotEqual(NooitAltijdAanUitEnum.Altijd, vm.Fasen[4].VasteAanvraag);
        }
    }
}
