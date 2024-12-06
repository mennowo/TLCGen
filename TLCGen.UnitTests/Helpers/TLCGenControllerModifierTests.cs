using System.Collections.Generic;
using NUnit.Framework;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests.Helpers
{
    [TestFixture]
    public class TLCGenControllerModifierTests
    {
        [Test]
        public void RemoveModelItemFromController_RemoveFaseCyclusFromController_RelatedTeDoserenSignaalGroepRemovedFromFileIngreep()
        {
            var controller = new ControllerModel();
            controller.Fasen.Add(new FaseCyclusModel{Naam = "01", Detectoren = new List<DetectorModel>{new DetectorModel{Naam = "011"}}});
            controller.FileIngrepen.Add(new FileIngreepModel{TeDoserenSignaalGroepen = new List<FileIngreepTeDoserenSignaalGroepModel>{new FileIngreepTeDoserenSignaalGroepModel{FaseCyclus = "01"}}});
            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier{Controller = controller});

            TLCGenControllerModifier.Default.RemoveModelItemFromController("01", TLCGenObjectTypeEnum.Fase);

            Assert.That(0 == controller.Fasen.Count);
            Assert.That(0 == controller.FileIngrepen[0].TeDoserenSignaalGroepen.Count);
        }

        [Test]
        public void RemoveModelItemFromController_RemoveFaseCyclusFromController_RelatedModuleDataRemoved()
        {
            var controller = new ControllerModel();
            controller.Fasen.Add(new FaseCyclusModel{Naam = "01", Detectoren = new List<DetectorModel>{new DetectorModel{Naam = "011"}}});
            controller.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel{FaseCyclus = "01"});
            controller.ModuleMolen.Modules.Add(new ModuleModel{Fasen = new List<ModuleFaseCyclusModel>{new ModuleFaseCyclusModel{FaseCyclus = "01"}}});
            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier{Controller = controller});

            TLCGenControllerModifier.Default.RemoveModelItemFromController("01", TLCGenObjectTypeEnum.Fase);

            Assert.That(0 == controller.Fasen.Count);
            Assert.That(0 == controller.ModuleMolen.FasenModuleData.Count);
            Assert.That(0 == controller.ModuleMolen.Modules[0].Fasen.Count);
        }

        [Test]
        public void RemoveModelItemFromController_RemoveSelectieveDetectorFromController_RelatedPrioMeldingAlseChanged()
        {
            var controller = new ControllerModel();
            controller.Fasen.Add(new FaseCyclusModel{Naam = "01", Detectoren = new List<DetectorModel>{new DetectorModel{Naam = "011"}}});
            controller.SelectieveDetectoren.Add(new SelectieveDetectorModel{Naam = "s011"});
            controller.PrioData.PrioIngrepen.Add(new PrioIngreepModel
            {
                FaseCyclus = "01",
                MeldingenData = new PrioIngreepMeldingenDataModel
                {
                    Inmeldingen = new List<PrioIngreepInUitMeldingModel>
                    {
                        new PrioIngreepInUitMeldingModel
                        {
                            Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector,
                            RelatedInput1 = "s011"
                        }
                    }
                }
            });
            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier{Controller = controller});

            TLCGenControllerModifier.Default.RemoveModelItemFromController("s011", TLCGenObjectTypeEnum.SelectieveDetector);

            Assert.That(0 == controller.SelectieveDetectoren.Count);
            Assert.That(0 == controller.PrioData.PrioIngrepen[0].MeldingenData.Inmeldingen.Count);
        }
    }
}
