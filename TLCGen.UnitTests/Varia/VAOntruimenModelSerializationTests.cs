using System.IO;
using System.Linq;
using NUnit.Framework;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.UnitTests.Varia
{
    [TestFixture]
    public class VAOntruimenModelSerializationTests
    {
        [Test]
        public void ManualConflicts_SurviveASerializationRoundTrip()
        {
            var c = new ControllerModel();
            var va = new VAOntruimenFaseModel { FaseCyclus = "05", VAOntrMax = 30 };
            va.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 4 } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 7 } }
            });
            c.VAOntruimenFasen.Add(va);

            var file = Path.GetTempFileName();
            try
            {
                TLCGenSerialization.Serialize(file, c);
                var loaded = TLCGenSerialization.DeSerialize<ControllerModel>(file);

                var loadedDetector = loaded.VAOntruimenFasen.Single().VADetectoren.Single();
                Assert.That(loadedDetector.ManualConflicts, Is.Not.Null);
                Assert.That(loadedDetector.ManualConflicts.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "33" }));
                Assert.That(loadedDetector.ManualConflicts[0].VAOntruimingsTijd, Is.EqualTo(7));
                // the automatically maintained list is untouched
                Assert.That(loadedDetector.ConflicterendeFasen.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "26" }));
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}
