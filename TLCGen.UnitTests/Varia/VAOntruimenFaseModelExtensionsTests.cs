using System.Linq;
using NUnit.Framework;
using TLCGen.Models;

namespace TLCGen.UnitTests.Varia
{
    [TestFixture]
    public class VAOntruimenFaseModelExtensionsTests
    {
        [Test]
        public void GetAllConflicts_ReturnsHardConflictsFollowedByManualConflicts()
        {
            var fase = new VAOntruimenFaseModel { FaseCyclus = "05" };
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen =
                {
                    new VAOntruimenNaarFaseModel { FaseCyclus = "26" },
                    new VAOntruimenNaarFaseModel { FaseCyclus = "31" }
                },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33" } }
            });

            Assert.That(fase.GetAllConflicts().Select(x => x.FaseCyclus), Is.EqualTo(new[] { "26", "31", "33" }));
        }

        [Test]
        public void GetAllConflicts_IncludesConflictsOfEveryDetectorWithoutDuplicates()
        {
            var fase = new VAOntruimenFaseModel { FaseCyclus = "05" };
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33" } }
            });
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "052",
                ConflicterendeFasen =
                {
                    new VAOntruimenNaarFaseModel { FaseCyclus = "26" },
                    new VAOntruimenNaarFaseModel { FaseCyclus = "31" }
                },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33" } }
            });

            Assert.That(fase.GetAllConflicts().Select(x => x.FaseCyclus), Is.EqualTo(new[] { "26", "31", "33" }));
        }

        [Test]
        public void GetAllConflicts_DoesNotDuplicateAManualConflictThatIsAlsoAHardConflict()
        {
            var fase = new VAOntruimenFaseModel { FaseCyclus = "05" };
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } }
            });

            Assert.That(fase.GetAllConflicts().Count, Is.EqualTo(1));
        }

        [Test]
        public void GetManualConflicts_ReturnsTheUnionOverAllDetectors()
        {
            var fase = new VAOntruimenFaseModel { FaseCyclus = "05" };
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33" } }
            });
            fase.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "052",
                ManualConflicts =
                {
                    new VAOntruimenNaarFaseModel { FaseCyclus = "33" },
                    new VAOntruimenNaarFaseModel { FaseCyclus = "34" }
                }
            });

            Assert.That(fase.GetManualConflicts(), Is.EqualTo(new[] { "33", "34" }));
        }

        [Test]
        public void GetConflictForDetector_PrefersTheHardConflictOverTheManualConflict()
        {
            var hard = new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 4 };
            var manual = new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 9 };
            var detector = new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { hard },
                ManualConflicts = { manual }
            };

            Assert.That(detector.GetConflictForDetector("26"), Is.SameAs(hard));
        }

        [Test]
        public void GetConflictForDetector_FallsBackToTheManualConflict()
        {
            var manual = new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 7 };
            var detector = new VAOntruimenDetectorModel { Detector = "051", ManualConflicts = { manual } };

            Assert.That(detector.GetConflictForDetector("33"), Is.SameAs(manual));
        }

        [Test]
        public void GetConflictForDetector_ReturnsNullForAnUnknownConflict()
        {
            Assert.That(new VAOntruimenDetectorModel().GetConflictForDetector("33"), Is.Null);
        }
    }
}
