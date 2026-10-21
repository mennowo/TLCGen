using System.Linq;
using NUnit.Framework;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.UnitTests.Prio
{
    [TestFixture]
    public class PrioIngreepModelTests
    {
        private static PrioIngreepModel CreateIngreep(string fc, string naam, PrioIngreepVoertuigTypeEnum type)
        {
            var prio = new PrioIngreepModel { FaseCyclus = fc, Naam = naam, Type = type };
            prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
                { InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding, Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding });
            prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
                { InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding, Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde });
            prio.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
                { InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding, Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding });
            return prio;
        }

        private static DefaultsProvider CreateDefaultsWithEmptyMeldingenPerType()
        {
            var defaults = new TLCGenDefaultsModel();
            foreach (var type in new[] { PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepVoertuigTypeEnum.Hulpdienst })
            {
                defaults.Defaults.Add(new TLCGenDefaultModel
                {
                    DataType = typeof(PrioIngreepModel).FullName + "," + typeof(PrioIngreepModel).Assembly.GetName().Name,
                    Selector1 = type.ToString(),
                    Data = new PrioIngreepModel { Type = type, GroenBewaking = type == PrioIngreepVoertuigTypeEnum.Bus ? 300 : 500 }
                });
            }
            return new DefaultsProvider { Defaults = defaults };
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetDefaultsOnModel_VehicleTypeChanged_MeldingenDataIsKept(bool onlyValues)
        {
            var defaultsProvider = CreateDefaultsWithEmptyMeldingenPerType();
            var prio = CreateIngreep("02", "bus", PrioIngreepVoertuigTypeEnum.Bus);
            var meldingenData = prio.MeldingenData;

            foreach (var type in new[] { PrioIngreepVoertuigTypeEnum.Hulpdienst, PrioIngreepVoertuigTypeEnum.Bus })
            {
                prio.Type = type;
                defaultsProvider.SetDefaultsOnModel(prio, type.ToString(), null, onlyValues);
            }

            Assert.That(prio.MeldingenData, Is.SameAs(meldingenData));
            Assert.That(prio.MeldingenData.Inmeldingen, Has.Count.EqualTo(2));
            Assert.That(prio.MeldingenData.Uitmeldingen, Has.Count.EqualTo(1));
            Assert.That(prio.GroenBewaking, Is.EqualTo(300), "value defaults must still be applied");
        }

        [Test]
        public void SetDefaultsOnModel_TwoIngrepenSameType_DoNotShareMeldingenData()
        {
            var defaultsProvider = CreateDefaultsWithEmptyMeldingenPerType();
            var prio1 = CreateIngreep("02", "bus", PrioIngreepVoertuigTypeEnum.Bus);
            var prio2 = CreateIngreep("05", "bus", PrioIngreepVoertuigTypeEnum.Bus);

            defaultsProvider.SetDefaultsOnModel(prio1, nameof(PrioIngreepVoertuigTypeEnum.Hulpdienst), null, false);
            defaultsProvider.SetDefaultsOnModel(prio2, nameof(PrioIngreepVoertuigTypeEnum.Hulpdienst), null, false);

            Assert.That(prio1.MeldingenData, Is.Not.SameAs(prio2.MeldingenData));
            Assert.That(prio1.LijnNummers, Is.Not.SameAs(prio2.LijnNummers));
        }

        [TestCase(PrioIngreepVoertuigTypeEnum.Bus)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Hulpdienst)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Tram)]
        public void EnsureDummyKARDetectors_AnyVehicleType_EveryKARMeldingHasDummy(PrioIngreepVoertuigTypeEnum type)
        {
            var prio = CreateIngreep("02", "bus", type);

            prio.EnsureDummyKARDetectors();

            var kar = prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen)
                .Where(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding).ToList();
            Assert.That(kar.All(x => x.DummyKARMelding is { Dummy: true }));
            Assert.That(prio.GetDummyInDetectors().Single().Naam, Is.EqualTo("dummykarin02bus"));
            Assert.That(prio.GetDummyUitDetectors().Single().Naam, Is.EqualTo("dummykaruit02bus"));
            Assert.That(prio.MeldingenData.Inmeldingen.Single(x => x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde).DummyKARMelding, Is.Null);
        }

        [Test]
        public void EnsureDummyKARDetectors_ExistingDummy_IsKept()
        {
            var prio = CreateIngreep("02", "bus", PrioIngreepVoertuigTypeEnum.Bus);
            var existing = new DetectorModel { Dummy = true, Naam = "customname" };
            existing.Simulatie.Q1 = 42;
            prio.MeldingenData.Inmeldingen[0].DummyKARMelding = existing;

            prio.EnsureDummyKARDetectors();

            Assert.That(prio.MeldingenData.Inmeldingen[0].DummyKARMelding, Is.SameAs(existing));
            Assert.That(existing.Naam, Is.EqualTo("customname"));
        }

        [Test]
        public void EnsureDummyKARDetectors_MeldingNoLongerKAR_DummyRemoved()
        {
            var prio = CreateIngreep("02", "bus", PrioIngreepVoertuigTypeEnum.Bus);
            prio.EnsureDummyKARDetectors();

            prio.MeldingenData.Inmeldingen[0].Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector;
            prio.EnsureDummyKARDetectors();

            Assert.That(prio.MeldingenData.Inmeldingen[0].DummyKARMelding, Is.Null);
            Assert.That(prio.GetDummyUitDetectors().Count(), Is.EqualTo(1));
        }

        [Test]
        public void RenameDummyKARDetectors_IngreepRenamed_DummiesFollow()
        {
            var prio = CreateIngreep("02", "bus", PrioIngreepVoertuigTypeEnum.Bus);
            prio.EnsureDummyKARDetectors();

            prio.Naam = "hd";
            prio.RenameDummyKARDetectors();

            Assert.That(prio.GetDummyInDetectors().Single().Naam, Is.EqualTo("dummykarin02hd"));
            Assert.That(prio.GetDummyUitDetectors().Single().Naam, Is.EqualTo("dummykaruit02hd"));
        }
    }
}
