using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Integrity;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab
{
    [TestFixture]
    public class FasenOVTabViewModelTests
    {
        [Test]
        public void FasenOVTabSelectedFase_TabDeselectedAndSelected_SelectedFaseEqual()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;

            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[3];
            vm.OnDeselected();
            vm.OnSelected();

            Assert.True(object.ReferenceEquals(vm.SelectedFaseCyclus, vm.Fasen[3]));
        }

        [Test]
        public void SelectedFaseCyclus_SetToPhaseWithOVIngreep_SetsSelectedOVIngreep()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.OVIngrepen.Add(new OVIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();

            vm.SelectedFaseCyclus = vm.Fasen[2];

            Assert.True(vm.SelectedFaseCyclusOVIngreep);
            Assert.IsNotNull(vm.SelectedOVIngreep);
            Assert.True(vm.SelectedOVIngreep.OVIngreep == model.OVData.OVIngrepen[0]);
        }

        [Test]
        public void SelectedFaseCyclus_SetToPhaseWithOutOVIngreep_SetsSelectedOVIngreepToNull()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.OVIngrepen.Add(new OVIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();

            vm.SelectedFaseCyclus = vm.Fasen[2];
            vm.SelectedFaseCyclus = vm.Fasen[1];

            Assert.False(vm.SelectedFaseCyclusOVIngreep);
            Assert.IsNull(vm.SelectedOVIngreep);
        }

        [Test]
        public void SelectedFaseCyclus_SetToPhaseWithHDIngreep_SetsSelectedHDIngreep()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", HDIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();

            vm.SelectedFaseCyclus = vm.Fasen[2];

            Assert.True(vm.SelectedFaseCyclusHDIngreep);
            Assert.IsNotNull(vm.SelectedHDIngreep);
            Assert.True(vm.SelectedHDIngreep.HDIngreep == model.OVData.HDIngrepen[0]);
        }

        [Test]
        public void SelectedFaseCyclus_SetToPhaseWithOutHDIngreep_SetsSelectedHDIngreepToNull()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();

            vm.SelectedFaseCyclus = vm.Fasen[2];
            vm.SelectedFaseCyclus = vm.Fasen[1];

            Assert.False(vm.SelectedFaseCyclusHDIngreep);
            Assert.IsNull(vm.SelectedHDIngreep);
        }

        [Test]
        public void SelectedFaseCyclusOVIngreep_SetToTrueOnPhaseWithoutIngreep_AddsIngreep()
        {
            var model = new ControllerModel();
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[2];

            vm.SelectedFaseCyclusOVIngreep = true;

            Assert.IsNotNull(vm.SelectedOVIngreep);
            Assert.AreEqual(1, model.OVData.OVIngrepen.Count);
            Assert.True(vm.SelectedOVIngreep.OVIngreep == model.OVData.OVIngrepen[0]);
        }

        [Test]
        public void SelectedFaseCyclusOVIngreep_SetToFalseOnPhaseWithIngreep_RemovesIngreep()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.OVIngrepen.Add(new OVIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[2];

            vm.SelectedFaseCyclusOVIngreep = false;

            Assert.IsNull(vm.SelectedOVIngreep);
            Assert.AreEqual(0, model.OVData.OVIngrepen.Count);
        }

        [Test]
        public void SelectedFaseCyclusHDIngreep_SetToTrueOnPhaseWithoutIngreep_AddsIngreep()
        {
            var model = new ControllerModel();
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerModifier.OverrideDefault(FakesCreator.CreateControllerModifier());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[2];

            vm.SelectedFaseCyclusHDIngreep = true;

            Assert.IsNotNull(vm.SelectedHDIngreep);
            Assert.AreEqual(1, model.OVData.HDIngrepen.Count);
            Assert.True(vm.SelectedHDIngreep.HDIngreep == model.OVData.HDIngrepen[0]);
        }

        [Test]
        public void SelectedFaseCyclusHDIngreep_SetToFalseOnPhaseWithIngreep_RemovesIngreep()
        {
            var model = new ControllerModel();
            TLCGenControllerModifier.OverrideDefault(FakesCreator.CreateControllerModifier());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", HDIngreep = true });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.OVData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
            var vm = new FasenOVTabViewModel();
            vm.Controller = model;
            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[2];

            vm.SelectedFaseCyclusHDIngreep = false;

            Assert.IsNull(vm.SelectedHDIngreep);
            Assert.AreEqual(0, model.OVData.HDIngrepen.Count);
        }

    }
}
