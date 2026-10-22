using System.Linq;
using NUnit.Framework;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.PrioriteitsTab
{
    [TestFixture]
    [ResetGlobalState]
    public class PrioIngreepInUitMeldingViewModelTests
    {
        private static PrioIngreepViewModel CreateIngreep(PrioIngreepVoertuigTypeEnum type, params PrioIngreepInUitMeldingVoorwaardeTypeEnum[] inmeldingen)
        {
            var controller = TLCGenControllerDataProvider.Default.Controller;
            controller.Data.CCOLVersie = CCOLVersieEnum.CCOL110;
            var prio = new PrioIngreepModel { FaseCyclus = "02", Naam = "bus", Type = type };
            foreach (var t in inmeldingen)
            {
                prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel { InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding, Type = t });
            }
            controller.PrioData.PrioIngrepen.Add(prio);
            return new PrioIngreepViewModel(prio, null);
        }

        private static PrioIngreepInUitMeldingViewModel GetInmelding(PrioIngreepViewModel ingreep, int i) =>
            ingreep.MeldingenLists.First(x => x.Meldingen.Any() && x.Meldingen[0].InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding).Meldingen[i];

        [Test]
        public void Constructor_SelectsTypeOfModel()
        {
            var ingreep = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector);

            var melding = GetInmelding(ingreep, 0);

            Assert.That(melding.Type?.Value, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector));
            Assert.That(melding.MeldingenTypes.Select(x => x.Value),
                Is.EqualTo(PrioMeldingTypes.GetAllowed(PrioIngreepVoertuigTypeEnum.Bus, CCOLVersieEnum.CCOL110)));
        }

        [Test]
        public void Type_SetToNull_IsIgnored()
        {
            var ingreep = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
            var melding = GetInmelding(ingreep, 0);

            melding.Type = null;

            Assert.That(melding.Type?.Value, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            Assert.That(melding.PrioIngreepInUitMelding.Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
        }

        [Test]
        public void VehicleTypeChanged_TypesListReplacedNotCleared_SelectionFollowsModel()
        {
            var ingreep = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus,
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);
            var kar = GetInmelding(ingreep, 0);
            var oldTypes = kar.MeldingenTypes;
            var oldCount = oldTypes.Count;

            ingreep.Type = PrioIngreepVoertuigTypeEnum.Fiets;

            Assert.That(kar.MeldingenTypes, Is.Not.SameAs(oldTypes));
            Assert.That(oldTypes, Has.Count.EqualTo(oldCount), "the bound list must not be cleared");
            Assert.That(kar.MeldingenTypes.Select(x => x.Value),
                Is.EqualTo(PrioMeldingTypes.GetAllowed(PrioIngreepVoertuigTypeEnum.Fiets, CCOLVersieEnum.CCOL110)));
            Assert.That(kar.Type?.Value, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
            Assert.That(kar.PrioIngreepInUitMelding.Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
            Assert.That(kar.PrioIngreepInUitMelding.DummyKARMelding, Is.Null);
            Assert.That(GetInmelding(ingreep, 1).Type?.Value, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
        }

        [Test]
        public void VehicleTypeChanged_BusToHulpdienstAndBack_KarKept()
        {
            var ingreep = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
            var kar = GetInmelding(ingreep, 0);

            ingreep.Type = PrioIngreepVoertuigTypeEnum.Hulpdienst;
            ingreep.Type = PrioIngreepVoertuigTypeEnum.Bus;

            Assert.That(kar.Type?.Value, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            Assert.That(kar.PrioIngreepInUitMelding.DummyKARMelding, Is.Not.Null);
        }
    }
}
