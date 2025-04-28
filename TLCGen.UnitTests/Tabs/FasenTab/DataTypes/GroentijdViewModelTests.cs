using System.Collections.Generic;
using NUnit.Framework;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.DataTypes
{
    [TestFixture]
    public class GroentijdViewModelTests
    {
        [Test]
        public void GroentijdViewModel_ComparedToOther_OrdersCorrectly()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.GroentijdenSets.Add(new GroentijdenSetModel
            {
                Groentijden = new List<GroentijdModel>
                {
                    new GroentijdModel{ FaseCyclus = "01", Waarde = 0},
                    new GroentijdModel{ FaseCyclus = "02", Waarde = 0}
                },
                Naam = "MG1",
                Type = GroentijdenTypeEnum.MaxGroentijden
            });
            var vm = new GroentijdenSetViewModel(model.GroentijdenSets[0]);

            Assert.That(vm.Groentijden[0].CompareTo(vm.Groentijden[1]) < 0, vm.Naam);
            Assert.That(vm.Groentijden[1].CompareTo(vm.Groentijden[0]) > 0, vm.Naam);
        }
    }
}