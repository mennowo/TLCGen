using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using NSubstitute;
using NUnit.Framework;
using TLCGen.Dependencies.Providers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.UnitTests.Golden;

namespace TLCGen.UnitTests.Plugins
{
    /// <summary>
    /// End-to-end tests for the VA ontruimen code that issue #115 adds: signal groups that are not
    /// automatic hard conflicts can be added manually and must be cleared/held like hard conflicts.
    /// </summary>
    [SingleThreaded]
    [TestFixture]
    public class VAOntruimenGeneratorTests
    {
        private ITLCGenDialogProvider _dialogProvider;
        private List<string> _dialogMessages;
        private string _outputDir;

        [SetUp]
        public void SetUp()
        {
            CCOLTestGenerator.LoadDefaultSettings();
            DefaultsProvider.OverrideDefault(new DefaultsProvider());

            _dialogMessages = new List<string>();
            _dialogProvider = TLCGenDialogProvider.Default;
            var dialogs = Substitute.For<ITLCGenDialogProvider>();
            dialogs.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButton>())
                .Returns(x =>
                {
                    _dialogMessages.Add(x.ArgAt<string>(0));
                    return MessageBoxResult.OK;
                });
            TLCGenDialogProvider.Default = dialogs;

            _outputDir = Path.Combine(Path.GetTempPath(), "TLCGen_vaontruimen_test");
            if (Directory.Exists(_outputDir)) Directory.Delete(_outputDir, true);
            Directory.CreateDirectory(_outputDir);
        }

        [TearDown]
        public void TearDown()
        {
            TLCGenDialogProvider.Default = _dialogProvider;
        }

        [Test]
        public void ManualConflict_NotAHardConflict_GeneratesTimerAndClearsConflictingPhase()
        {
            var c = CreateControllerWithVAOntruimen(manualConflictFase: "33", manualConflictTime: 7);

            Generate(c);

            var tab = File.ReadAllText(Path.Combine(_outputDir, "GMtab.c"));
            var reg = File.ReadAllText(Path.Combine(_outputDir, "GMreg.c"));

            // a timer per detector for the manual conflict, with the manual time
            Assert.That(tab, Does.Contain("T_code[tva0533_d051]"));
            Assert.That(tab, Does.Contain("T_max[tva0533_d051] = 7;"));
            // and the regular hard conflict is still there
            Assert.That(tab, Does.Contain("T_max[tva0526_d051] = 4;"));

            // the manual conflict is handled in the realisation code and holds back its phase
            Assert.That(reg, Does.Contain("RT[tva0533_d051] = D[d051] && T[tvamax05] && T_max[tva0533_d051];"));
            Assert.That(reg, Does.Contain("if(T[tva0533_d051]) X[fc33] |= BIT9;"));
        }

        [Test]
        public void ManualConflict_UsesTheTimeOfEachDetector()
        {
            var c = PrioGoldenCorpus.CreateController(
                ("05", FaseTypeEnum.Auto), ("26", FaseTypeEnum.Auto), ("33", FaseTypeEnum.Auto));
            c.InterSignaalGroep.Conflicten.Single(x => x.FaseVan == "05" && x.FaseNaar == "33").Waarde = -2;

            var va = new VAOntruimenFaseModel { FaseCyclus = "05", VAOntrMax = 30 };
            va.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 4 } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 5 } }
            });
            va.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "052",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 6 } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 9 } }
            });
            c.VAOntruimenFasen.Add(va);

            Generate(c);

            var tab = File.ReadAllText(Path.Combine(_outputDir, "GMtab.c"));
            Assert.That(tab, Does.Contain("T_max[tva0533_d051] = 5;"));
            Assert.That(tab, Does.Contain("T_max[tva0533_d052] = 9;"));
        }

        [Test]
        public void ManualConflict_ThatIsAlsoAHardConflict_GeneratesOneTimer()
        {
            var c = CreateControllerWithVAOntruimen(manualConflictFase: "26", manualConflictTime: 9);

            Generate(c);

            var tab = File.ReadAllText(Path.Combine(_outputDir, "GMtab.c"));

            Assert.That(CountOccurrences(tab, "T_code[tva0526_d051]"), Is.EqualTo(1),
                "a manual conflict that is also a hard conflict must not produce a duplicate timer");
        }

        [Test]
        public void WithoutManualConflicts_DeelconflictIsNotGenerated()
        {
            var c = CreateControllerWithVAOntruimen(manualConflictFase: null, manualConflictTime: 0);

            Generate(c);

            var tab = File.ReadAllText(Path.Combine(_outputDir, "GMtab.c"));

            Assert.That(tab, Does.Not.Contain("tva0533"));
        }

        private static ControllerModel CreateControllerWithVAOntruimen(string manualConflictFase, int manualConflictTime)
        {
            var c = PrioGoldenCorpus.CreateController(
                ("05", FaseTypeEnum.Auto), ("26", FaseTypeEnum.Auto), ("33", FaseTypeEnum.Auto));

            // 05 -> 33 is a deelconflict (FK), not a hard conflict: it only reaches VA ontruimen
            // through the manual conflict list
            var deelconflict = c.InterSignaalGroep.Conflicten.Single(x => x.FaseVan == "05" && x.FaseNaar == "33");
            deelconflict.Waarde = -2;

            var va = new VAOntruimenFaseModel
            {
                FaseCyclus = "05",
                VAOntrMax = 30,
                VADetectoren =
                {
                    new VAOntruimenDetectorModel
                    {
                        Detector = "051",
                        ConflicterendeFasen =
                        {
                            new VAOntruimenNaarFaseModel { FaseCyclus = "26", VAOntruimingsTijd = 4 }
                        }
                    }
                }
            };
            if (manualConflictFase != null)
            {
                va.VADetectoren[0].ManualConflicts.Add(new VAOntruimenNaarFaseModel
                {
                    FaseCyclus = manualConflictFase,
                    VAOntruimingsTijd = manualConflictTime
                });
            }
            c.VAOntruimenFasen.Add(va);

            return c;
        }

        private void Generate(ControllerModel c)
        {
            CCOLTestGenerator.Plugin.Generator.GenerateSourceFiles(c, _outputDir);
            Assert.That(_dialogMessages, Is.Empty, "generation reported an error");
        }

        private static int CountOccurrences(string text, string value)
        {
            var count = 0;
            var index = 0;
            while ((index = text.IndexOf(value, index, System.StringComparison.Ordinal)) >= 0)
            {
                ++count;
                index += value.Length;
            }
            return count;
        }
    }
}
