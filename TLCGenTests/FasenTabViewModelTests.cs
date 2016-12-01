using NUnit.Framework;
using System.Threading;
using TLCGen.DataAccess;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGenTests
{
    [TestFixture]
    public class FasenTabViewModelTests
    {
        private MainWindowViewModel mainwinvm;

        [SetUp]
        public void StartTesting()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
            mainwinvm = new MainWindowViewModel();
        }

        private void AddFaseAndAssert(FasenTabViewModel tab, int count)
        {
            tab.AddFaseCommand.Execute(null);
            Assert.IsTrue(tab.AddFaseCommand.CanExecute(null));
            Assert.IsFalse(tab.RemoveFaseCommand.CanExecute(null));
            Assert.AreEqual(tab.Fasen.Count, count);
        }

        private void RemoveFaseAndAssert(FasenTabViewModel tab, int count)
        {
            tab.SelectedFaseCyclus = tab.Fasen[0];
            Assert.IsTrue(tab.RemoveFaseCommand.CanExecute(null));
            tab.RemoveFaseCommand.Execute(null);
            Assert.AreEqual(tab.Fasen.Count, count);
            Assert.IsFalse(tab.RemoveFaseCommand.CanExecute(null));
            Assert.IsTrue(tab.AddFaseCommand.CanExecute(null));
        }

        private void AssertGroentijd(FasenTabViewModel tab, int maxindex, int phaseindex, int val)
        {
            Assert.IsTrue(tab.GroentijdenLijstVM.GroentijdenSets.Count > 0);
            Assert.IsTrue(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSetList.Count == maxindex);
            Assert.IsTrue(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden.Count == maxindex);
            Assert.AreEqual(val, tab.GroentijdenLijstVM.GroentijdenMatrix[0, phaseindex].Waarde);
            Assert.AreEqual(val, tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSetList[phaseindex].Waarde);
            Assert.AreEqual(val, tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden[phaseindex].Waarde);
        }

        /// <summary>
        /// Tests adding and removing phases
        /// </summary>
        [Test]
        public void FasenAddAndRemove()
        {
            Assert.IsTrue(mainwinvm.NewFileCommand.CanExecute(null));
            mainwinvm.NewFileCommand.Execute(null);
            var tab = mainwinvm.ControllerVM.FasenTabVM;

            // Add phase possible, remove phase not
            Assert.IsTrue(tab.AddFaseCommand.CanExecute(null));
            Assert.IsFalse(tab.RemoveFaseCommand.CanExecute(null));
            // No phases yet
            Assert.AreEqual(tab.Fasen.Count, 0);

            // Add phase, assert command can execute is the same, phases count increased
            AddFaseAndAssert(tab, 1);
            // Add phase, assert command can execute is the same, phases count increased
            AddFaseAndAssert(tab, 2);

            // Remove phase
            RemoveFaseAndAssert(tab, 1);
            // Remove phase: all are gone
            RemoveFaseAndAssert(tab, 0);

            mainwinvm.ControllerVM.HasChanged = false;
            Assert.IsTrue(mainwinvm.CloseFileCommand.CanExecute(null));
            mainwinvm.CloseFileCommand.Execute(null);
        }

        /// <summary>
        /// Tests behaviour when changing Fasen names such that sorting should occur
        /// </summary>
        [Test]
        public void FasenSort()
        {
            Assert.IsTrue(mainwinvm.NewFileCommand.CanExecute(null));
            mainwinvm.NewFileCommand.Execute(null);
            var tab = mainwinvm.ControllerVM.FasenTabVM;

            // Add phase possible, remove phase not
            Assert.IsTrue(tab.AddFaseCommand.CanExecute(null));
            Assert.IsFalse(tab.RemoveFaseCommand.CanExecute(null));
            // No phases yet
            Assert.AreEqual(tab.Fasen.Count, 0);

            // Add phase, assert command can execute is the same, phases count increased
            AddFaseAndAssert(tab, 1);
            // Add phase, assert command can execute is the same, phases count increased
            AddFaseAndAssert(tab, 2);
            // Add phase, assert command can execute is the same, phases count increased
            AddFaseAndAssert(tab, 3);

            // Add greentime set
            tab.GroentijdenLijstVM.AddGroentijdenSetCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 1);
            // Change value of second phase
            tab.GroentijdenLijstVM.GroentijdenMatrix[0, 0].Waarde = 111; // 1
            tab.GroentijdenLijstVM.GroentijdenMatrix[0, 1].Waarde = 222; // 2
            tab.GroentijdenLijstVM.GroentijdenMatrix[0, 2].Waarde = 333; // 3

            // Rename second phase
            tab.Fasen[1].Naam = "04"; // 02 --> 04 (2)
            // Change tab index to cause sorting
            tab.SelectedTabIndex = 1;
            tab.SelectedTabIndex = 0;

            // Assert the last two phases changed places
            Assert.AreEqual("01", tab.Fasen[0].Naam); // 1
            Assert.AreEqual("03", tab.Fasen[1].Naam); // 3
            Assert.AreEqual("04", tab.Fasen[2].Naam); // 2
            // Assert the greentime for phase 04 moved to the bottom
            Assert.AreEqual("01", tab.GroentijdenLijstVM.FasenNames[0]); // 1
            Assert.AreEqual("03", tab.GroentijdenLijstVM.FasenNames[1]); // 3
            Assert.AreEqual("04", tab.GroentijdenLijstVM.FasenNames[2]); // 2
            AssertGroentijd(tab, 3, 0, 111);
            AssertGroentijd(tab, 3, 1, 333);
            AssertGroentijd(tab, 3, 2, 222);

            // Rename second phase
            tab.Fasen[0].Naam = "15"; // 01 --> 15 (1)
            // Change tab index to cause sorting
            tab.SelectedTabIndex = 1;
            tab.SelectedTabIndex = 0;
            // Assert the last two phases changed places
            Assert.AreEqual("03", tab.Fasen[0].Naam); // 2
            Assert.AreEqual("04", tab.Fasen[1].Naam); // 3
            Assert.AreEqual("15", tab.Fasen[2].Naam); // 1
            // Assert the greentime for phase 04 moved to the bottom
            Assert.AreEqual("03", tab.GroentijdenLijstVM.FasenNames[0]); // 2
            Assert.AreEqual("04", tab.GroentijdenLijstVM.FasenNames[1]); // 3
            Assert.AreEqual("15", tab.GroentijdenLijstVM.FasenNames[2]); // 1
            AssertGroentijd(tab, 3, 0, 333);
            AssertGroentijd(tab, 3, 1, 222);
            AssertGroentijd(tab, 3, 2, 111);

            // Rename third phase
            tab.Fasen[1].Naam = "01"; // 04 --> 01 (2)
            // Change tab index to cause sorting
            tab.SelectedTabIndex = 1;
            tab.SelectedTabIndex = 0;
            // Assert the last two phases changed places
            Assert.AreEqual("01", tab.Fasen[0].Naam); // 2
            Assert.AreEqual("03", tab.Fasen[1].Naam); // 3
            Assert.AreEqual("15", tab.Fasen[2].Naam); // 1
            // Assert the greentime for phase 04 moved to the bottom
            Assert.AreEqual("01", tab.GroentijdenLijstVM.FasenNames[0]); // 2
            Assert.AreEqual("03", tab.GroentijdenLijstVM.FasenNames[1]); // 3
            Assert.AreEqual("15", tab.GroentijdenLijstVM.FasenNames[2]); // 1
            AssertGroentijd(tab, 3, 0, 222);
            AssertGroentijd(tab, 3, 1, 333);
            AssertGroentijd(tab, 3, 2, 111);

            // clean up: remove phases, sets
            RemoveFaseAndAssert(tab, 2);
            RemoveFaseAndAssert(tab, 1);
            RemoveFaseAndAssert(tab, 0);
            tab.GroentijdenLijstVM.SelectedSet = tab.GroentijdenLijstVM.GroentijdenSets[0];
            tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 0);

            mainwinvm.ControllerVM.HasChanged = false;
            Assert.IsTrue(mainwinvm.CloseFileCommand.CanExecute(null));
            mainwinvm.CloseFileCommand.Execute(null);
        }
    }
}
