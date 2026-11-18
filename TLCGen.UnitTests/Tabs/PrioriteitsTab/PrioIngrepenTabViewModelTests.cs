using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests
{
    [TestFixture]
    [ResetGlobalState]
    public class PrioIngrepenTabViewModelTests
    {
        private static ControllerModel CreateController()
        {
            var controller = ControllerCreator.GetSmallControllerWithoutDetection();
            controller.PrioData.PrioIngreepType = PrioIngreepTypeEnum.GeneriekePrioriteit;
            ControllerAccessProvider.OverrideDefault(FakesCreator.CreateControllerAccessProvider(controller));
            TemplatesProvider.OverrideDefault(FakesCreator.CreateTemplatesProvider());
            return controller;
        }

        /// <summary>
        /// A template item can have no FaseCyclus (the template editor creates its first item without one and
        /// a Replace placeholder in a null FaseCyclus cannot be replaced). Inserting it under a fase must still
        /// make it belong to that fase, otherwise the tree, the overview and the generator never show it after reload.
        /// </summary>
        [Test]
        public void InsertItemsFromTemplate_ItemWithoutFaseCyclus_BelongsToSelectedFaseAfterReload()
        {
            var controller = CreateController();
            var vm = new PrioIngrepenTabViewModel { Controller = controller };
            var fase = vm.Fasen.Single(x => x.Naam == "02");
            vm.SelectedObject = fase;

            // shape of a clone from a template with Replace "FC" applied to fase "02"
            var item = new PrioIngreepModel
            {
                FaseCyclus = null,
                Naam = "FC_bus",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };
            ModelStringSetter.ReplaceStringInModel(item, "FC", "02");

            vm.InsertItemsFromTemplate(new List<PrioIngreepModel> { item });

            Assert.That(item.FaseCyclus, Is.EqualTo("02"));
            Assert.That(controller.PrioData.PrioIngrepen, Has.Count.EqualTo(1));
            Assert.That(fase.Ingrepen, Has.Count.EqualTo(1), "shown right after adding");

            var reloaded = TLCGenSerialization.SerializeFromXmlDocument<ControllerModel>(
                TLCGenSerialization.SerializeToXmlDocument(controller));
            var reloadedVm = new PrioIngrepenTabViewModel { Controller = reloaded };

            Assert.That(reloadedVm.Fasen.Single(x => x.Naam == "02").Ingrepen, Has.Count.EqualTo(1),
                "still shown under the fase after reloading the file");
        }

        [Test]
        public void InsertItemsFromTemplate_ItemWithOtherFaseCyclus_IsMovedToSelectedFase()
        {
            var controller = CreateController();
            var vm = new PrioIngrepenTabViewModel { Controller = controller };
            var fase = vm.Fasen.Single(x => x.Naam == "02");
            vm.SelectedObject = fase;

            // eg. a fixed template (no Replace) holding the fase it was authored on
            var item = new PrioIngreepModel
            {
                FaseCyclus = "01",
                Naam = "bus",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };

            vm.InsertItemsFromTemplate(new List<PrioIngreepModel> { item });

            Assert.That(item.FaseCyclus, Is.EqualTo("02"));
            Assert.That(controller.PrioData.PrioIngrepen.Single().FaseCyclus, Is.EqualTo("02"));
        }

        [Test]
        public void SelectedObject_IngreepWithoutTemplateProviderInUse_DoesNotThrowAndSetsApplyToItem()
        {
            var controller = CreateController();
            controller.PrioData.PrioIngrepen.Add(new PrioIngreepModel
            {
                FaseCyclus = "02",
                Naam = "bus",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            });
            var vm = new PrioIngrepenTabViewModel { Controller = controller };
            var ingreep = vm.Fasen.Single(x => x.Naam == "02").Ingrepen.Single();

            Assert.DoesNotThrow(() => vm.SelectedObject = ingreep);
            Assert.That(vm.TemplatesProviderVm.ApplyToItem, Is.SameAs(ingreep.PrioIngreep));
        }
    }

    [TestFixture]
    [ResetGlobalState]
    public class PrioIngreepTemplateViewModelTests
    {
        [Test]
        public void Replace_Set_ItemsWithoutFaseCyclusGetTheReplacePlaceholder()
        {
            var template = new TLCGenTemplateModel<PrioIngreepModel>
            {
                Naam = "test",
                Items = { new PrioIngreepModel { Naam = "FC_bus", Type = PrioIngreepVoertuigTypeEnum.Bus } }
            };
            var vm = new PrioIngreepTemplateViewModel(template);

            vm.Replace = "FC";

            Assert.That(template.Items.Single().FaseCyclus, Is.EqualTo("FC"));
        }

        [Test]
        public void Replace_Changed_ItemsWithOldPlaceholderFollowTheNewOne()
        {
            var template = new TLCGenTemplateModel<PrioIngreepModel>
            {
                Naam = "test",
                Replace = "FC",
                Items = { new PrioIngreepModel { FaseCyclus = "FC", Naam = "FC_bus", Type = PrioIngreepVoertuigTypeEnum.Bus } }
            };
            var vm = new PrioIngreepTemplateViewModel(template);

            vm.Replace = "XX";

            Assert.That(template.Items.Single().FaseCyclus, Is.EqualTo("XX"));
        }
    }
}
