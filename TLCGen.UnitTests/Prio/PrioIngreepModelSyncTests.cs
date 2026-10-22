using System.Linq;
using NUnit.Framework;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests.Prio
{
    [TestFixture]
    public class PrioIngreepModelSyncTests
    {
        private static PrioIngreepModel CreateIngreep(PrioIngreepVoertuigTypeEnum type, params PrioIngreepInUitMeldingVoorwaardeTypeEnum[] inmeldingen)
        {
            var prio = new PrioIngreepModel { FaseCyclus = "02", Naam = "bus", Type = type };
            foreach (var t in inmeldingen)
            {
                prio.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel { InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding, Type = t });
            }
            prio.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
                { InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding, Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding });
            return prio;
        }

        [Test]
        public void GetAllowed_EveryVehicleType_IsNotEmpty()
        {
            foreach (var type in System.Enum.GetValues(typeof(PrioIngreepVoertuigTypeEnum)).Cast<PrioIngreepVoertuigTypeEnum>())
            {
                Assert.That(PrioMeldingTypes.GetAllowed(type, CCOLVersieEnum.CCOL9), Is.Not.Empty, type.ToString());
            }
        }

        [TestCase(PrioIngreepVoertuigTypeEnum.Bus)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Fiets)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Vrachtwagen)]
        public void GetAllowed_RisOnlyFromCcol11(PrioIngreepVoertuigTypeEnum type)
        {
            Assert.That(PrioMeldingTypes.GetAllowed(type, CCOLVersieEnum.CCOL9), Does.Not.Contain(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
            Assert.That(PrioMeldingTypes.GetAllowed(type, CCOLVersieEnum.CCOL110), Does.Contain(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
        }

        [Test]
        public void GetAllowed_Hulpdienst_RisRegardlessOfCcolVersion()
        {
            Assert.That(PrioMeldingTypes.GetAllowed(PrioIngreepVoertuigTypeEnum.Hulpdienst, CCOLVersieEnum.CCOL9),
                Does.Contain(PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
        }

        [TestCase(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, true)]
        [TestCase(PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, true)]
        [TestCase(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, false)]
        [TestCase(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, false)]
        [TestCase(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, false)]
        public void HasValidOpvangStoring_OnlyDetectorOrVecomWithDetectorFallback(
            PrioIngreepInUitMeldingVoorwaardeTypeEnum type, PrioIngreepInUitMeldingVoorwaardeTypeEnum fallbackType, bool expected)
        {
            var melding = new PrioIngreepInUitMeldingModel
            {
                Type = type, OpvangStoring = true, MeldingBijstoring = new PrioIngreepInUitMeldingModel { Type = fallbackType }
            };

            Assert.That(PrioMeldingTypes.HasValidOpvangStoring(melding), Is.EqualTo(expected));
        }

        [Test]
        public void ApplyVehicleTypeChange_TypeWithoutOpvangStoring_DropsMeldingBijStoring()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus,
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.Ingang, PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector);
            foreach (var m in prio.MeldingenData.Inmeldingen)
            {
                m.OpvangStoring = true;
                m.MeldingBijstoring = new PrioIngreepInUitMeldingModel { Type = PrioMeldingTypes.MeldingBijStoringType };
            }

            // hulpdienst: Ingang is not allowed (-> KAR, no opvang storing), Vecom is
            PrioIngreepModelSync.ApplyVehicleTypeChange(prio, PrioIngreepVoertuigTypeEnum.Hulpdienst, CCOLVersieEnum.CCOL110);

            var ingang = prio.MeldingenData.Inmeldingen[0];
            var vecom = prio.MeldingenData.Inmeldingen[1];
            Assert.That(ingang.Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            Assert.That(ingang.OpvangStoring, Is.False);
            Assert.That(ingang.MeldingBijstoring, Is.Null);
            Assert.That(vecom.OpvangStoring, Is.True);
            Assert.That(vecom.MeldingBijstoring, Is.Not.Null);
        }

        [Test]
        public void ApplyVehicleTypeChange_BusToFiets_DisallowedMeldingGetsFirstAllowedType()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus,
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);

            PrioIngreepModelSync.ApplyVehicleTypeChange(prio, PrioIngreepVoertuigTypeEnum.Fiets, CCOLVersieEnum.CCOL110);

            Assert.That(prio.Type, Is.EqualTo(PrioIngreepVoertuigTypeEnum.Fiets));
            Assert.That(prio.MeldingenData.Inmeldingen.Select(x => x.Type), Is.EqualTo(new[]
            {
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector, // KAR not allowed for fiets
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector
            }));
            Assert.That(prio.MeldingenData.Uitmeldingen[0].Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
        }

        [Test]
        public void ApplyVehicleTypeChange_KarMeldingReplaced_DummyRemoved()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
            prio.EnsureDummyKARDetectors();
            Assert.That(prio.MeldingenData.Inmeldingen[0].DummyKARMelding, Is.Not.Null);

            PrioIngreepModelSync.ApplyVehicleTypeChange(prio, PrioIngreepVoertuigTypeEnum.Fiets, CCOLVersieEnum.CCOL110);

            Assert.That(prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen).Select(x => x.DummyKARMelding), Is.All.Null);
        }

        [Test]
        public void ApplyVehicleTypeChange_BusToHulpdienst_KarKeptWithDummy()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);

            PrioIngreepModelSync.ApplyVehicleTypeChange(prio, PrioIngreepVoertuigTypeEnum.Hulpdienst, CCOLVersieEnum.CCOL110);

            var kar = prio.MeldingenData.Inmeldingen[0];
            Assert.That(kar.Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
            Assert.That(kar.DummyKARMelding?.Naam, Is.EqualTo("dummykarin02bus"));
        }

        [TestCase(PrioIngreepVoertuigTypeEnum.Tram, RISVehicleRole.PUBLICTRANSPORT, RISVehicleSubrole.TRAM)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Hulpdienst, RISVehicleRole.EMERGENCY, RISVehicleSubrole.EMERGENCY)]
        [TestCase(PrioIngreepVoertuigTypeEnum.Vrachtwagen, RISVehicleRole.COMMERCIAL | RISVehicleRole.SPECIALTRANSPORT, RISVehicleSubrole.UNKNOWN)]
        public void ApplyVehicleTypeChange_SetsRisRolesOnEveryMelding(PrioIngreepVoertuigTypeEnum type, RISVehicleRole role, RISVehicleSubrole subrole)
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus,
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector);

            PrioIngreepModelSync.ApplyVehicleTypeChange(prio, type, CCOLVersieEnum.CCOL110);

            foreach (var melding in prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen))
            {
                Assert.That(melding.RisRole, Is.EqualTo(role));
                Assert.That(melding.RisSubrole, Is.EqualTo(subrole));
            }
        }

        [Test]
        public void SetRisRoles_NG_LeavesRolesAsTheyAre()
        {
            var melding = new PrioIngreepInUitMeldingModel { RisRole = RISVehicleRole.EMERGENCY, RisSubrole = RISVehicleSubrole.EMERGENCY };

            PrioIngreepModelSync.SetRisRoles(melding, PrioIngreepVoertuigTypeEnum.NG);

            Assert.That(melding.RisRole, Is.EqualTo(RISVehicleRole.EMERGENCY));
            Assert.That(melding.RisSubrole, Is.EqualTo(RISVehicleSubrole.EMERGENCY));
        }

        [Test]
        public void ApplyAllowedMeldingTypes_CcolDowngrade_RisReplacedAndReported()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus,
                PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);

            var changed = PrioIngreepModelSync.ApplyAllowedMeldingTypes(prio, CCOLVersieEnum.CCOL9);

            Assert.That(changed, Is.EqualTo(new[] { prio.MeldingenData.Inmeldingen[0] }));
            Assert.That(prio.MeldingenData.Inmeldingen[0].Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector));
            Assert.That(prio.MeldingenData.Inmeldingen[1].Type, Is.EqualTo(PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding));
        }

        [Test]
        public void RenameIngreep_RenamesDummyKarDetectors()
        {
            var prio = CreateIngreep(PrioIngreepVoertuigTypeEnum.Bus, PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding);
            prio.EnsureDummyKARDetectors();

            PrioIngreepModelSync.RenameIngreep(prio, "tram");

            Assert.That(prio.Naam, Is.EqualTo("tram"));
            Assert.That(prio.MeldingenData.Inmeldingen[0].DummyKARMelding.Naam, Is.EqualTo("dummykarin02tram"));
            Assert.That(prio.MeldingenData.Uitmeldingen[0].DummyKARMelding.Naam, Is.EqualTo("dummykaruit02tram"));
        }
    }
}
