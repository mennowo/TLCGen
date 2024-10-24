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

                Assert.That(6 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That(15 == nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.That(20 == nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.That(25 == nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.That(30 == nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.That(35 == nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.That(40 == nc.InterSignaalGroep.Conflicten[5].Waarde);
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

                Assert.That(6 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That(15 == nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.That(20 == nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.That(25 == nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.That(30 == nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.That(35 == nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.That(40 == nc.InterSignaalGroep.Conflicten[5].Waarde);
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

                Assert.That(6 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That(9 == nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.That(8 == nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.That(7 == nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.That(6 == nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.That(5 == nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.That(4 == nc.InterSignaalGroep.Conflicten[5].Waarde);
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

                Assert.That(4 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[0].FaseVan);
                Assert.That("04" == nc.InterSignaalGroep.Conflicten[3].FaseVan);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[3].FaseNaar);
                Assert.That(9 == nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.That(8 == nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.That(5 == nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.That(4 == nc.InterSignaalGroep.Conflicten[3].Waarde);
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

                Assert.That(6 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[0].FaseVan);
                Assert.That("02" == nc.InterSignaalGroep.Conflicten[1].FaseVan);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[2].FaseVan);
                Assert.That("04" == nc.InterSignaalGroep.Conflicten[3].FaseVan);
                Assert.That("02" == nc.InterSignaalGroep.Conflicten[4].FaseVan);
                Assert.That("05" == nc.InterSignaalGroep.Conflicten[5].FaseVan);
                Assert.That("02" == nc.InterSignaalGroep.Conflicten[0].FaseNaar);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[1].FaseNaar);
                Assert.That("04" == nc.InterSignaalGroep.Conflicten[2].FaseNaar);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten[3].FaseNaar);
                Assert.That("05" == nc.InterSignaalGroep.Conflicten[4].FaseNaar);
                Assert.That("02" == nc.InterSignaalGroep.Conflicten[5].FaseNaar);
                Assert.That(9 == nc.InterSignaalGroep.Conflicten[0].Waarde);
                Assert.That(8 == nc.InterSignaalGroep.Conflicten[1].Waarde);
                Assert.That(5 == nc.InterSignaalGroep.Conflicten[2].Waarde);
                Assert.That(4 == nc.InterSignaalGroep.Conflicten[3].Waarde);
                Assert.That(3 == nc.InterSignaalGroep.Conflicten[4].Waarde);
                Assert.That(2 == nc.InterSignaalGroep.Conflicten[5].Waarde);
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

                Assert.That(2 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That("03" == nc.InterSignaalGroep.Conflicten.First().FaseVan);
                Assert.That("04" == nc.InterSignaalGroep.Conflicten.Last().FaseVan);
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

                Assert.That(6 == nc.Fasen.Count);
                Assert.That("06" == nc.Fasen.Last().Naam);
                Assert.That(8 == nc.InterSignaalGroep.Conflicten.Count);
                Assert.That("01" == nc.InterSignaalGroep.Conflicten.First().FaseVan);
                Assert.That("06" == nc.InterSignaalGroep.Conflicten.Last().FaseVan);
            }
        }
    }
}
