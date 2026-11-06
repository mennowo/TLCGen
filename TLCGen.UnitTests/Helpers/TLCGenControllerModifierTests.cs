using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void RemoveModelItemFromController_RemoveFaseCyclusFromController_RemovesManualVAOntruimenConflict()
        {
            var controller = new ControllerModel();
            controller.Fasen.Add(new FaseCyclusModel { Naam = "33" });
            var va = new VAOntruimenFaseModel { FaseCyclus = "05" };
            va.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 7 } }
            });
            controller.VAOntruimenFasen.Add(va);
            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier { Controller = controller });

            TLCGenControllerModifier.Default.RemoveModelItemFromController("33", TLCGenObjectTypeEnum.Fase);

            Assert.That(va.VADetectoren[0].ManualConflicts, Is.Empty);
        }

        [Test]
        public void CorrectModel_AlteredConflicts_KeepsManualVAOntruimenConflictThatIsNotAHardConflict()
        {
            var controller = CreateControllerWithVAOntruimen(manualConflictWaarde: -2);

            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier { Controller = controller });
            TLCGenControllerModifier.Default.CorrectModel_AlteredConflicts();

            var manualConflicts = controller.VAOntruimenFasen[0].VADetectoren[0].ManualConflicts;
            Assert.That(manualConflicts.Count, Is.EqualTo(1));
            Assert.That(manualConflicts[0].FaseCyclus, Is.EqualTo("33"));
            Assert.That(manualConflicts[0].VAOntruimingsTijd, Is.EqualTo(7));
        }

        [Test]
        public void CorrectModel_AlteredConflicts_RemovesManualConflictThatBecameAHardConflict()
        {
            var controller = CreateControllerWithVAOntruimen(manualConflictWaarde: 30);

            TLCGenControllerModifier.OverrideDefault(new TLCGenControllerModifier { Controller = controller });
            TLCGenControllerModifier.Default.CorrectModel_AlteredConflicts();

            var detector = controller.VAOntruimenFasen[0].VADetectoren[0];
            Assert.That(detector.ManualConflicts, Is.Empty);
            Assert.That(detector.ConflicterendeFasen.Any(x => x.FaseCyclus == "33"), Is.True,
                "the conflict is now maintained automatically");
        }

        private static ControllerModel CreateControllerWithVAOntruimen(int manualConflictWaarde)
        {
            var controller = new ControllerModel();
            controller.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            controller.Fasen.Add(new FaseCyclusModel { Naam = "26" });
            controller.Fasen.Add(new FaseCyclusModel { Naam = "33" });

            var va = new VAOntruimenFaseModel { FaseCyclus = "05" };
            va.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 4 } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 7 } }
            });
            controller.VAOntruimenFasen.Add(va);

            controller.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "05", FaseNaar = "26", Waarde = 30 });
            controller.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "05", FaseNaar = "33", Waarde = manualConflictWaarde });
            return controller;
        }
    }
}
