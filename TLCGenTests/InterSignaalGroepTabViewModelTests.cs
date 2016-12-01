using NUnit.Framework;
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
    [TestFixture]
    public class InterSignaalGroepTabViewModelTests
    {
        [SetUp]
        public void StartTesting()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
        }

        private void AssertConfictMatrixModelEqual(int waarde, ControllerModel c, SynchronisatieViewModel svm)
        {
            Assert.AreEqual(waarde, c.InterSignaalGroep.Conflicten.Where((x) =>
            {
                return x.FaseVan == svm.FaseVan &&
                       x.FaseNaar == svm.FaseNaar;
            }).First().Waarde);
        }

        private void AssertGarantieConfictMatrixModelEqual(int waarde, ControllerModel c, SynchronisatieViewModel svm)
        {
            Assert.AreEqual(waarde, c.InterSignaalGroep.Conflicten.Where((x) =>
            {
                return x.FaseVan == svm.FaseVan &&
                       x.FaseNaar == svm.FaseNaar;
            }).First().GarantieWaarde);
        }

        [Test]
        public void InterSGConflictMatrixIntegrity()
        {
            ControllerModel c = new ControllerModel();
            var controllervm = new ControllerViewModel(null, c);
            var fasentab = controllervm.FasenTabVM;
            var synctab = controllervm.CoordinatiesTabVM;

            Assert.IsTrue(c.Fasen.Count == 0);
            Assert.IsTrue(fasentab.AddFaseCommand.CanExecute(null));

            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);

            Assert.IsTrue(c.Fasen.Count == 5);

            Assert.IsTrue(synctab.DisplayType == TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict);
            Assert.IsTrue(synctab.ConflictMatrix != null);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(0) == 5);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(1) == 5);

            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[2, 3].ConflictValue = "FK";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) != null);

            synctab.ConflictMatrix[4, 2].ConflictValue = "50";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling);

            AssertConfictMatrixModelEqual(10, c, synctab.ConflictMatrix[0, 1]);
            AssertConfictMatrixModelEqual(20, c, synctab.ConflictMatrix[1, 0]);
            AssertConfictMatrixModelEqual(0, c, synctab.ConflictMatrix[2, 4]);
            AssertConfictMatrixModelEqual(50, c, synctab.ConflictMatrix[4, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);

            bool fail = false;
            try
            {
                synctab.ConflictMatrix[4, 4].ConflictValue = "50";
            }
            catch
            {
                fail = true;
            }
            Assert.IsTrue(fail);

            fail = false;
            try
            {
                synctab.ConflictMatrix[1, 3].IsCoupled = true;
            }
            catch
            {
                fail = true;
            }
            Assert.IsTrue(fail);
        }

        [Test]
        public void InterSGConflictMatrixWithGuaranteedIntegrity()
        {
            ControllerModel c = new ControllerModel();
            var controllervm = new ControllerViewModel(null, c);
            var fasentab = controllervm.FasenTabVM;
            var synctab = controllervm.CoordinatiesTabVM;

            Assert.IsTrue(c.Fasen.Count == 0);
            Assert.IsTrue(fasentab.AddFaseCommand.CanExecute(null));

            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);

            Assert.IsTrue(c.Fasen.Count == 5);

            Assert.IsTrue(synctab.DisplayType == TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict);
            Assert.IsTrue(synctab.ConflictMatrix != null);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(0) == 5);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(1) == 5);

            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[2, 3].ConflictValue = "FK";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) != null);

            synctab.ConflictMatrix[4, 2].ConflictValue = "50";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            AssertConfictMatrixModelEqual(10, c, synctab.ConflictMatrix[0, 1]);
            AssertConfictMatrixModelEqual(20, c, synctab.ConflictMatrix[1, 0]);
            AssertConfictMatrixModelEqual(0, c, synctab.ConflictMatrix[2, 4]);
            AssertConfictMatrixModelEqual(50, c, synctab.ConflictMatrix[4, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);

            synctab.UseGarantieOntruimingsTijden = true;

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) != null);

            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.GarantieConflict;

            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[4, 2].ConflictValue = "50";

            string s = TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c);
            Assert.IsTrue(s == null);

            synctab.ConflictMatrix[3, 2].ConflictValue = "70";

            s = TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c);
            Assert.IsTrue(s != null);

            synctab.ConflictMatrix[2, 3].ConflictValue = "90";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) != null);

            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict;

            synctab.ConflictMatrix[3, 2].ConflictValue = "70";
            synctab.ConflictMatrix[2, 3].ConflictValue = "90";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);


            bool fail = false;
            try
            {
                synctab.ConflictMatrix[4, 4].ConflictValue = "50";
            }
            catch
            {
                fail = true;
            }
            Assert.IsTrue(fail);

            fail = false;
            try
            {
                synctab.ConflictMatrix[1, 3].IsCoupled = true;
            }
            catch
            {
                fail = true;
            }
            Assert.IsTrue(fail);
        }

        [Test]
        public void InterSGConflictMatrixOrdering()
        {
            ControllerModel c = new ControllerModel();
            var controllervm = new ControllerViewModel(null, c);

            var fasentab = controllervm.FasenTabVM;
            var synctab = controllervm.CoordinatiesTabVM;

            Assert.IsTrue(c.Fasen.Count == 0);
            Assert.IsTrue(fasentab.AddFaseCommand.CanExecute(null));

            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);

            Assert.IsTrue(c.Fasen.Count == 5);

            Assert.IsTrue(synctab.DisplayType == TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict);
            Assert.IsTrue(synctab.ConflictMatrix != null);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(0) == 5);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(1) == 5);

            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[4, 2].ConflictValue = "50";
            synctab.ConflictMatrix[2, 3].ConflictValue = "FK";
            
            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling);

            // Rename 02 so it moves down
            fasentab.Fasen[1].Naam = "06";
            // Change tab index to cause sorting
            fasentab.SelectedTabIndex = 1;
            fasentab.SelectedTabIndex = 0;

            synctab.BuildConflictMatrix();

            Assert.AreEqual("10", synctab.ConflictMatrix[0, 4].ConflictValue);
            Assert.AreEqual(synctab.ConflictMatrix[0, 4].FaseVan, fasentab.Fasen[0].Define);
            Assert.AreEqual(synctab.ConflictMatrix[0, 4].FaseNaar, fasentab.Fasen[4].Define);
            Assert.AreEqual("20", synctab.ConflictMatrix[4, 0].ConflictValue);
            Assert.AreEqual(synctab.ConflictMatrix[4, 0].FaseVan, fasentab.Fasen[4].Define);
            Assert.AreEqual(synctab.ConflictMatrix[4, 0].FaseNaar, fasentab.Fasen[0].Define);
            Assert.AreEqual("0", synctab.ConflictMatrix[1, 3].ConflictValue);
            Assert.AreEqual("50", synctab.ConflictMatrix[3, 1].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[2, 1].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[1, 2].ConflictValue);

            AssertConfictMatrixModelEqual(10, c, synctab.ConflictMatrix[0, 4]);
            AssertConfictMatrixModelEqual(20, c, synctab.ConflictMatrix[4, 0]);
            AssertConfictMatrixModelEqual(0, c, synctab.ConflictMatrix[1, 3]);
            AssertConfictMatrixModelEqual(50, c, synctab.ConflictMatrix[3, 1]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[2, 1]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[2, 1]);

            Assert.IsFalse(synctab.ConflictMatrix[0, 4].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[4, 0].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 2].AllowCoupling);
        }

        [Test]
        public void InterSGSerialization()
        {

            ControllerModel c = new ControllerModel();
            var controllervm = new ControllerViewModel(null, c);
            var fasentab = controllervm.FasenTabVM;
            var synctab = controllervm.CoordinatiesTabVM;

            Assert.IsTrue(c.Fasen.Count == 0);
            Assert.IsTrue(fasentab.AddFaseCommand.CanExecute(null));

            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);

            Assert.IsTrue(c.Fasen.Count == 5);

            Assert.IsTrue(synctab.DisplayType == TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict);
            Assert.IsTrue(synctab.ConflictMatrix != null);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(0) == 5);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(1) == 5);

            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[4, 2].ConflictValue = "50";
            synctab.ConflictMatrix[3, 2].ConflictValue = "FK";

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Gelijkstart;

            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling);

            Assert.IsTrue(synctab.ConflictMatrix[1, 3].AllowCoupling);
            synctab.ConflictMatrix[1, 3].IsCoupled = true;
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].HasGelijkstart);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].HasGelijkstart);
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].HasNoCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].HasNoCoupling);

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            // Simulate saving and opening
            var ser = new TLCGen.DataAccess.SerializeT<ControllerModel>();
            var deser = new TLCGen.DataAccess.DeserializeT<ControllerModel>();
            var doc = ser.SerializeToXmlDocument(c);

            controllervm.HasChanged = false;
            controllervm.Controller = null;
            c = deser.SerializeFromXmlDocument(doc);
            controllervm = new ControllerViewModel(null, c);
            fasentab = controllervm.FasenTabVM;
            synctab = controllervm.CoordinatiesTabVM;
            synctab.BuildConflictMatrix();

            Assert.AreEqual("10", synctab.ConflictMatrix[0, 1].ConflictValue);
            Assert.AreEqual("20", synctab.ConflictMatrix[1, 0].ConflictValue);
            Assert.AreEqual("0", synctab.ConflictMatrix[2, 4].ConflictValue);
            Assert.AreEqual("50", synctab.ConflictMatrix[4, 2].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[3, 2].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[2, 3].ConflictValue);

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            AssertConfictMatrixModelEqual(10, c, synctab.ConflictMatrix[0, 1]);
            AssertConfictMatrixModelEqual(20, c, synctab.ConflictMatrix[1, 0]);
            AssertConfictMatrixModelEqual(0, c, synctab.ConflictMatrix[2, 4]);
            AssertConfictMatrixModelEqual(50, c, synctab.ConflictMatrix[4, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);

            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling);

            Assert.IsTrue(synctab.ConflictMatrix[1, 3].HasGelijkstart);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].HasGelijkstart);
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].HasNoCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].HasNoCoupling);
        }

        [Test]
        public void InterSGCouplingMatrixIntegrity()
        {
            ControllerModel c = new ControllerModel();
            var controllervm = new ControllerViewModel(null, c);
            var fasentab = controllervm.FasenTabVM;
            var synctab = controllervm.CoordinatiesTabVM;

            // Add signal groups
            Assert.IsTrue(c.Fasen.Count == 0);
            Assert.IsTrue(fasentab.AddFaseCommand.CanExecute(null));
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            fasentab.AddFaseCommand.Execute(null);
            Assert.IsTrue(c.Fasen.Count == 5);

            // Check matrix
            Assert.IsTrue(synctab.DisplayType == TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict);
            Assert.IsTrue(synctab.ConflictMatrix != null);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(0) == 5);
            Assert.IsTrue(synctab.ConflictMatrix.GetLength(1) == 5);

            // Add conflicts
            synctab.ConflictMatrix[0, 1].ConflictValue = "10";
            synctab.ConflictMatrix[1, 0].ConflictValue = "20";
            synctab.ConflictMatrix[2, 4].ConflictValue = "0";
            synctab.ConflictMatrix[4, 2].ConflictValue = "50";
            synctab.ConflictMatrix[3, 2].ConflictValue = "FK";
            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            // Check properties that set cell availability
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Gelijkstart;
            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling || synctab.ConflictMatrix[0, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling || synctab.ConflictMatrix[1, 0].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling || synctab.ConflictMatrix[2, 4].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling || synctab.ConflictMatrix[4, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling || synctab.ConflictMatrix[3, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling || synctab.ConflictMatrix[2, 3].IsEnabled);

            // Set a "gelijkstart"
            // ===================
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].AllowCoupling);
            synctab.ConflictMatrix[1, 3].IsCoupled = true;
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].HasGelijkstart);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].HasGelijkstart); // Assert symmetry
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].HasNoCoupling);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].HasNoCoupling);

            // Check properties that set cell availability in Conflict Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);

            // Check properties that set cell availability in Voorstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Voorstart;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);

            // Check properties that set cell availability in Naloop Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Naloop;
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);

            // Set a "Voorstart"
            // =================
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Voorstart;
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled);
            synctab.ConflictMatrix[4, 3].IsCoupled = true;
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled); // Assert this cannot be also set on the opposite side
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].HasOppositeVoorstart);

            // Check properties that set cell availability in Conflict Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);

            // Check properties that set cell availability in Voorstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Gelijkstart;
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);

            // Check properties that set cell availability in Naloop Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Naloop;
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].IsEnabled);

            // Set a "Naloop"
            // ==============
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Naloop;
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].IsEnabled);
            synctab.ConflictMatrix[0, 2].IsCoupled = true;
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].IsEnabled); // Assert this _can_ be also set on the opposite side
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].HasNaloop);
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].HasOppositeNaloop);

            // Check properties that set cell availability in Conflict Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[0, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Gelijkstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Gelijkstart;
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].IsEnabled); // Naloop can go together with gelijkstart
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Voorstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Voorstart;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled); // Assert this cannot be also set on the opposite side
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].HasOppositeVoorstart);
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].IsEnabled); // Naloop can go together with voorstart
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Naloop Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Naloop;
            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling || synctab.ConflictMatrix[0, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling || synctab.ConflictMatrix[1, 0].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling || synctab.ConflictMatrix[2, 4].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling || synctab.ConflictMatrix[4, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling || synctab.ConflictMatrix[3, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling || synctab.ConflictMatrix[2, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled); 
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].IsEnabled);

            // Check and save
            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            // Simulate saving and opening
            // ===========================
            var ser = new TLCGen.DataAccess.SerializeT<ControllerModel>();
            var deser = new TLCGen.DataAccess.DeserializeT<ControllerModel>();
            var doc = ser.SerializeToXmlDocument(c);

            controllervm.HasChanged = false;
            controllervm.Controller = null;
            c = deser.SerializeFromXmlDocument(doc);
            controllervm = new ControllerViewModel(null, c);
            fasentab = controllervm.FasenTabVM;
            synctab = controllervm.CoordinatiesTabVM;
            synctab.BuildConflictMatrix();

            Assert.AreEqual("10", synctab.ConflictMatrix[0, 1].ConflictValue);
            Assert.AreEqual("20", synctab.ConflictMatrix[1, 0].ConflictValue);
            Assert.AreEqual("0", synctab.ConflictMatrix[2, 4].ConflictValue);
            Assert.AreEqual("50", synctab.ConflictMatrix[4, 2].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[3, 2].ConflictValue);
            Assert.AreEqual("FK", synctab.ConflictMatrix[2, 3].ConflictValue);

            Assert.IsTrue(synctab.ConflictMatrix[1, 3].HasGelijkstart);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].HasGelijkstart);

            Assert.IsTrue(synctab.ConflictMatrix[4, 3].HasVoorstart);
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].HasOppositeVoorstart);

            Assert.IsTrue(synctab.ConflictMatrix[0, 2].HasNaloop);
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].HasOppositeNaloop);

            Assert.IsTrue(TLCGen.Integrity.IntegrityChecker.IsConflictMatrixOK(c) == null);

            AssertConfictMatrixModelEqual(10, c, synctab.ConflictMatrix[0, 1]);
            AssertConfictMatrixModelEqual(20, c, synctab.ConflictMatrix[1, 0]);
            AssertConfictMatrixModelEqual(0, c, synctab.ConflictMatrix[2, 4]);
            AssertConfictMatrixModelEqual(50, c, synctab.ConflictMatrix[4, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);
            AssertConfictMatrixModelEqual(-2, c, synctab.ConflictMatrix[3, 2]);

            // Check properties that set cell availability in Conflict Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Conflict;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[0, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Gelijkstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Gelijkstart;
            Assert.IsFalse(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].IsEnabled); // Naloop can go together with gelijkstart
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Voorstart Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Voorstart;
            Assert.IsFalse(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 4].IsEnabled); // Assert this cannot be also set on the opposite side
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[0, 2].IsEnabled); // Naloop can go together with voorstart
            Assert.IsTrue(synctab.ConflictMatrix[2, 0].IsEnabled);

            // Check properties that set cell availability in Naloop Tab
            synctab.DisplayType = TLCGen.ViewModels.Enums.SynchronisatieTypeEnum.Naloop;
            Assert.IsFalse(synctab.ConflictMatrix[0, 1].AllowCoupling || synctab.ConflictMatrix[0, 1].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[1, 0].AllowCoupling || synctab.ConflictMatrix[1, 0].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 4].AllowCoupling || synctab.ConflictMatrix[2, 4].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[4, 2].AllowCoupling || synctab.ConflictMatrix[4, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[3, 2].AllowCoupling || synctab.ConflictMatrix[3, 2].IsEnabled);
            Assert.IsFalse(synctab.ConflictMatrix[2, 3].AllowCoupling || synctab.ConflictMatrix[2, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 1].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[1, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[4, 3].IsEnabled);
            Assert.IsTrue(synctab.ConflictMatrix[3, 4].IsEnabled);
        }
    }
}
