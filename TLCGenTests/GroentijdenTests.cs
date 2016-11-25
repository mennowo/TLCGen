using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGenTests
{
    [TestClass]
    public class GroentijdenTests
    {
        /// <summary>
        /// Checks behaviour when adding and removing greentime sets
        /// Also checks if changing a value in the coupled matrix has the right effect in the model
        /// </summary>
        [TestMethod]
        public void AddRemoveGreenTimeSet()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
            DataProvider.Instance.SetController();
            var tab = new FasenTabViewModel(DataProvider.Instance.Controller);

            Assert.AreEqual(tab.Fasen.Count, 0);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 0);

            // Add phases
            tab.AddFaseCommand.Execute(null);
            tab.AddFaseCommand.Execute(null);
            Assert.AreEqual(tab.Fasen.Count, 2);

            // Assert on greentimes
            Assert.IsTrue(tab.GroentijdenLijstVM.AddGroentijdenSetCommand.CanExecute(null));
            Assert.IsFalse(tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.CanExecute(null));
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 0);

            // Add greentime set
            tab.GroentijdenLijstVM.AddGroentijdenSetCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 1);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden.Count, 2);

            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden[0].FaseCyclus, tab.Fasen[0].Define);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden[0].Waarde,
                tab.GroentijdenLijstVM.GroentijdenMatrix[0, 0].Waarde);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden[1].Waarde,
                tab.GroentijdenLijstVM.GroentijdenMatrix[0, 1].Waarde);

            tab.GroentijdenLijstVM.GroentijdenMatrix[0, 1].Waarde = 123;
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden[1].Waarde,
                123);

            // Remove phase
            tab.SelectedFaseCyclus = tab.Fasen[0];
            tab.RemoveFaseCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden.Count, 1);

            // Remove phase: all are gone
            tab.SelectedFaseCyclus = tab.Fasen[0];
            tab.RemoveFaseCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets[0].GroentijdenSet.Groentijden.Count, 0);

            // Remove greentime set
            Assert.IsFalse(tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.CanExecute(null));
            tab.GroentijdenLijstVM.SelectedSet = tab.GroentijdenLijstVM.GroentijdenSets[0];
            Assert.IsTrue(tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.CanExecute(null));
            tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.Execute(null);
            Assert.AreEqual(tab.GroentijdenLijstVM.GroentijdenSets.Count, 0);
            Assert.IsTrue(tab.GroentijdenLijstVM.AddGroentijdenSetCommand.CanExecute(null));
            Assert.IsFalse(tab.GroentijdenLijstVM.RemoveGroentijdenSetCommand.CanExecute(null));
        }
    }
}
