using NUnit.Framework;
using TLCGen.ViewModels;
using TLCGen.Models;
using TLCGen.Settings;
using System.Collections.Generic;

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class FasenLijstTimersTabViewModelTests
    {
        [Test]
        public void FasenTimersTabSelected_ControllerHas5Fasen_TabAlsoExposes5Fasen()
        {
            var model = new ControllerModel();
            TemplatesProvider.OverrideDefault(FakesCreator.CreateTemplatesProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTimersTabViewModel {Controller = model};

            vm.OnSelected();

            Assert.AreEqual(5, vm.Fasen.Count);
        }

        [Test]
        public void FasenTimersTabSelectedFase_TabDeselectedAndSelected_SelectedFaseEqual()
        {
            var model = new ControllerModel();
            TemplatesProvider.OverrideDefault(FakesCreator.CreateTemplatesProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenLijstTimersTabViewModel { Controller = model };

            vm.OnSelected();
            vm.SelectedFaseCyclus = vm.Fasen[3];
            vm.OnDeselected();
            vm.OnSelected();

            Assert.True(object.ReferenceEquals(vm.SelectedFaseCyclus, vm.Fasen[3]));
        }

        [Test]
        public void FasenTimersTabMultipleSelectionEdit_TGLChangedOnOnePhase_ChangesAllSelected()
        {
            var model = new ControllerModel();
            TemplatesProvider.OverrideDefault(FakesCreator.CreateTemplatesProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04", TGL = 30 });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05", TGL = 30 });
            var vm = new FasenLijstTimersTabViewModel { Controller = model };

            vm.OnSelected();
            vm.SelectedFaseCycli = new List<FaseCyclusViewModel>() { vm.Fasen[1], vm.Fasen[2], vm.Fasen[3] };
            vm.Fasen[3].TGL = 50;

            Assert.AreEqual(30, vm.Fasen[0].TGL);
            Assert.AreEqual(50, vm.Fasen[1].TGL);
            Assert.AreEqual(50, vm.Fasen[2].TGL);
            Assert.AreEqual(50, vm.Fasen[3].TGL);
            Assert.AreEqual(30, vm.Fasen[4].TGL);
        }
    }
}
