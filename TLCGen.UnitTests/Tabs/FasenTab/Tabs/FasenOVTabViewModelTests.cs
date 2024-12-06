using NUnit.Framework;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab
{
    [TestFixture]
    public class FasenOVTabViewModelTests
    {
        //[Test]
        //public void FasenOVTabSelectedFase_TabDeselectedAndSelected_SelectedFaseEqual()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    var vm = new PrioFasenTabViewModel();
        //    vm.Controller = model;

        //    vm.OnSelected();
        //    vm.SelectedFaseCyclus = vm.Fasen[3];
        //    vm.OnDeselected();
        //    vm.OnSelected();

        //    Assert.True(object.ReferenceEquals(vm.SelectedFaseCyclus, vm.Fasen[3]));
        //}

        //[Test]
        //public void SelectedFaseCyclus_SetToPhaseWithOVIngreep_SetsSelectedOVIngreep()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.PrioIngrepen.Add(new PrioIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();

        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    Assert.True(vm.SelectedFaseCyclusOVIngreep);
        //    Assert.IsNotNull(vm.SelectedOVIngreep);
        //    Assert.True(vm.SelectedOVIngreep.OVIngreep == model.PrioData.PrioIngrepen[0]);
        //}

        //[Test]
        //public void SelectedFaseCyclus_SetToPhaseWithOutOVIngreep_SetsSelectedOVIngreepToNull()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.PrioIngrepen.Add(new PrioIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();

        //    vm.SelectedFaseCyclus = vm.Fasen[2];
        //    vm.SelectedFaseCyclus = vm.Fasen[1];

        //    Assert.False(vm.SelectedFaseCyclusOVIngreep);
        //    Assert.IsNull(vm.SelectedOVIngreep);
        //}

        //[Test]
        //public void SelectedFaseCyclus_SetToPhaseWithHDIngreep_SetsSelectedHDIngreep()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", HDIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();

        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    Assert.True(vm.SelectedFaseCyclusHDIngreep);
        //    Assert.IsNotNull(vm.SelectedHDIngreep);
        //    Assert.True(vm.SelectedHDIngreep.HDIngreep == model.PrioData.HDIngrepen[0]);
        //}

        //[Test]
        //public void SelectedFaseCyclus_SetToPhaseWithOutHDIngreep_SetsSelectedHDIngreepToNull()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();

        //    vm.SelectedFaseCyclus = vm.Fasen[2];
        //    vm.SelectedFaseCyclus = vm.Fasen[1];

        //    Assert.False(vm.SelectedFaseCyclusHDIngreep);
        //    Assert.IsNull(vm.SelectedHDIngreep);
        //}

        //[Test]
        //public void SelectedFaseCyclusOVIngreep_SetToTrueOnPhaseWithoutIngreep_AddsIngreep()
        //{
        //    var model = new ControllerModel();
        //    DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();
        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    vm.SelectedFaseCyclusOVIngreep = true;

        //    Assert.IsNotNull(vm.SelectedOVIngreep);
        //    Assert.That(1 == model.PrioData.PrioIngrepen.Count);
        //    Assert.True(vm.SelectedOVIngreep.OVIngreep == model.PrioData.PrioIngrepen[0]);
        //}

        //[Test]
        //public void SelectedFaseCyclusOVIngreep_SetToFalseOnPhaseWithIngreep_RemovesIngreep()
        //{
        //    var model = new ControllerModel();
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", OVIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.PrioIngrepen.Add(new PrioIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();
        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    vm.SelectedFaseCyclusOVIngreep = false;

        //    Assert.IsNull(vm.SelectedOVIngreep);
        //    Assert.That(0 == model.PrioData.PrioIngrepen.Count);
        //}

        //[Test]
        //public void SelectedFaseCyclusHDIngreep_SetToTrueOnPhaseWithoutIngreep_AddsIngreep()
        //{
        //    var model = new ControllerModel();
        //    DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
        //    TLCGenControllerModifier.OverrideDefault(FakesCreator.CreateControllerModifier());
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();
        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    vm.SelectedFaseCyclusHDIngreep = true;

        //    Assert.IsNotNull(vm.SelectedHDIngreep);
        //    Assert.That(1 == model.PrioData.HDIngrepen.Count);
        //    Assert.True(vm.SelectedHDIngreep.HDIngreep == model.PrioData.HDIngrepen[0]);
        //}

        //[Test]
        //public void SelectedFaseCyclusHDIngreep_SetToFalseOnPhaseWithIngreep_RemovesIngreep()
        //{
        //    var model = new ControllerModel();
        //    TLCGenControllerModifier.OverrideDefault(FakesCreator.CreateControllerModifier());
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "03", HDIngreep = true });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
        //    model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
        //    model.PrioData.HDIngrepen.Add(new HDIngreepModel() { FaseCyclus = model.Fasen[2].Naam });
        //    var vm = new OVFasenTabViewModel();
        //    vm.Controller = model;
        //    vm.OnSelected();
        //    vm.SelectedFaseCyclus = vm.Fasen[2];

        //    vm.SelectedFaseCyclusHDIngreep = false;

        //    Assert.IsNull(vm.SelectedHDIngreep);
        //    Assert.That(0 == model.PrioData.HDIngrepen.Count);
        //}

    }
}
