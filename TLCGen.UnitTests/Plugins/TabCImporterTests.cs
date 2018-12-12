using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using TLCGen.Dependencies.Providers;
using TLCGen.Importers.TabC;
using TLCGen.Models;
using TLCGen.Settings;

namespace TLCGen.UnitTests.Plugins
{
    public class TabCImporterTests
    {
        [TestFixture]
        public class TabCExistingControllerImporterTests
        {
            [Test]
            public void ExistingControllerHas5Fasen_NewControllerHas5FasenConflictsHigherValues_ConfictValuesUpdated()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc01][fc02] = 15;" + Environment.NewLine,
                    "TO_max[fc02][fc01] = 20;" + Environment.NewLine,
                    "TO_max[fc03][fc04] = 25;" + Environment.NewLine,
                    "TO_max[fc04][fc03] = 30;" + Environment.NewLine,
                    "TO_max[fc01][fc05] = 35;" + Environment.NewLine,
                    "TO_max[fc05][fc01] = 40;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(6, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual(15, nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.AreEqual(20, nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.AreEqual(25, nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.AreEqual(30, nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.AreEqual(35, nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.AreEqual(40, nc.InterSignaalGroep.Conflicten[5].Waarde);
            }

            [Test]
            public void ExistingControllerHas5Fasen_NewControllerHas5FasenConflictsHigherValuesWithComments_ConfictValuesUpdatedCommentsSkipped()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "   TO_max[fc01][fc02] = 15;" + Environment.NewLine,
                    "   TO_max[fc02][fc01] = 20;" + Environment.NewLine,
                    "   TO_max[fc03][fc04] = 25;" + Environment.NewLine,
                    "   TO_max[fc04][fc03] = 30;" + Environment.NewLine,
                    "/* TO_max[fc03][fc05] = 40; = deelconflict */" + Environment.NewLine,
                    "/* TO_max[fc05][fc03] = 40; = deelconflict */" + Environment.NewLine,
                    "   TO_max[fc01][fc05] = 35;" + Environment.NewLine,
                    "   TO_max[fc05][fc01] = 40;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(6, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual(15, nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.AreEqual(20, nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.AreEqual(25, nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.AreEqual(30, nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.AreEqual(35, nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.AreEqual(40, nc.InterSignaalGroep.Conflicten[5].Waarde);
            }

            [Test]
            public void ExistingControllerHas5Fasen_NewControllerHas5FasenConflictsLowerValues_ConfictValuesUpdated()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc01][fc02] = 9;" + Environment.NewLine,
                    "TO_max[fc02][fc01] = 8;" + Environment.NewLine,
                    "TO_max[fc03][fc04] = 7;" + Environment.NewLine,
                    "TO_max[fc04][fc03] = 6;" + Environment.NewLine,
                    "TO_max[fc01][fc05] = 5;" + Environment.NewLine,
                    "TO_max[fc05][fc01] = 4;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(6, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual(9, nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.AreEqual(8, nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.AreEqual(7, nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.AreEqual(6, nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.AreEqual(5, nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.AreEqual(4, nc.InterSignaalGroep.Conflicten[5].Waarde);
            }

            [Test]
            public void ExistingControllerHas5Fasen6Conflicts_NewControllerHas5Fasen4Conflicts_ConfictsRemoved()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc01][fc02] = 9;" + Environment.NewLine,
                    "TO_max[fc02][fc01] = 8;" + Environment.NewLine,
                    "TO_max[fc01][fc04] = 5;" + Environment.NewLine,
                    "TO_max[fc04][fc01] = 4;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(4, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[0].FaseVan);
                Assert.AreEqual("04", nc.InterSignaalGroep.Conflicten[3].FaseVan);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[3].FaseNaar);
                Assert.AreEqual(9, nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.AreEqual(8, nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.AreEqual(5, nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.AreEqual(4, nc.InterSignaalGroep.Conflicten[3].Waarde);
            }

            [Test]
            public void ExistingControllerHas5Fasen6Conflicts_NewControllerHas5Fasen6DifferentConflicts_ConfictsUpdated()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc01][fc02] = 9;" + Environment.NewLine,
                    "TO_max[fc02][fc01] = 8;" + Environment.NewLine,
                    "TO_max[fc01][fc04] = 5;" + Environment.NewLine,
                    "TO_max[fc04][fc01] = 4;" + Environment.NewLine,
                    "TO_max[fc02][fc05] = 3;" + Environment.NewLine,
                    "TO_max[fc05][fc02] = 2;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(6, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[0].FaseVan);
                Assert.AreEqual("02", nc.InterSignaalGroep.Conflicten[1].FaseVan);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[2].FaseVan);
                Assert.AreEqual("04", nc.InterSignaalGroep.Conflicten[3].FaseVan);
                Assert.AreEqual("02", nc.InterSignaalGroep.Conflicten[4].FaseVan);
                Assert.AreEqual("05", nc.InterSignaalGroep.Conflicten[5].FaseVan);
                Assert.AreEqual("02", nc.InterSignaalGroep.Conflicten[0].FaseNaar);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[1].FaseNaar);
                Assert.AreEqual("04", nc.InterSignaalGroep.Conflicten[2].FaseNaar);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten[3].FaseNaar);
                Assert.AreEqual("05", nc.InterSignaalGroep.Conflicten[4].FaseNaar);
                Assert.AreEqual("02", nc.InterSignaalGroep.Conflicten[5].FaseNaar);
                Assert.AreEqual(9, nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.AreEqual(8, nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.AreEqual(5, nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.AreEqual(4, nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.AreEqual(3, nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.AreEqual(2, nc.InterSignaalGroep.Conflicten[5].Waarde);
            }

            [Test]
            public void ExistingControllerHas5Fasen_NewControllerHas2Fasen_ConflictsRemovedForAbsentFasen()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc03][fc04] = 10;" + Environment.NewLine,
                    "TO_max[fc04][fc03] = 10;" + Environment.NewLine
                };
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(2, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual("03", nc.InterSignaalGroep.Conflicten.First().FaseVan);
                Assert.AreEqual("04", nc.InterSignaalGroep.Conflicten.Last().FaseVan);
            }

            [Test]
            public void ExistingControllerHas5Fasen_NewControllerHas6Fasen_FaseAddedWithConflicts()
            {
                var c = ControllerCreator.GetSmallControllerWithConflicts();
                var tabC = new[]
                {
                    "/* Aangemaakt met: OTTO" + Environment.NewLine,
                    "TO_max[fc01][fc02] = 10;" + Environment.NewLine,
                    "TO_max[fc02][fc01] = 10;" + Environment.NewLine,
                    "TO_max[fc03][fc04] = 10;" + Environment.NewLine,
                    "TO_max[fc04][fc03] = 10;" + Environment.NewLine,
                    "TO_max[fc01][fc05] = 10;" + Environment.NewLine,
                    "TO_max[fc05][fc01] = 10;" + Environment.NewLine,
                    "TO_max[fc01][fc06] = 10;" + Environment.NewLine,
                    "TO_max[fc06][fc01] = 10;" + Environment.NewLine
                };
                DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
                TLCGenDialogProvider.Default = FakesCreator.CreateDialogProvider();
                TLCGenDialogProvider.Default.ShowMessageBox("", "", System.Windows.MessageBoxButton.YesNo).ReturnsForAnyArgs(System.Windows.MessageBoxResult.Yes);
                TLCGenDialogProvider.Default.ShowOpenFileDialog("", "", false, out var fn).ReturnsForAnyArgs(true);
                TLCGenFileAccessProvider.Default = FakesCreator.CreateFileAccessProvider();
                TLCGenFileAccessProvider.Default.ReadAllLines("").ReturnsForAnyArgs(tabC);
                var importer = new TabCExistingControllerImporter();

                var nc = importer.ImportController(c);

                Assert.AreEqual(6, nc.Fasen.Count);
                Assert.AreEqual("06", nc.Fasen.Last().Naam);
                Assert.AreEqual(8, nc.InterSignaalGroep.Conflicten.Count);
                Assert.AreEqual("01", nc.InterSignaalGroep.Conflicten.First().FaseVan);
                Assert.AreEqual("06", nc.InterSignaalGroep.Conflicten.Last().FaseVan);
            }
        }
    }
}
