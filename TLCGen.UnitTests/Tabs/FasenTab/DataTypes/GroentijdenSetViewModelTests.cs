using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using NUnit.Framework;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.DataTypes
{
    [TestFixture]
    public class GroentijdenSetViewModelTests
    {
        [Test]
        public void GronetijdSetViewModel_TypeChangedFormMGToVG_NameChangedCorrectly()
        {
            Messenger.OverrideDefault(FakesCreator.CreateMessenger());
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.GroentijdenSets.Add(new GroentijdenSetModel
            {
                Groentijden = new List<GroentijdModel>
                {
                    new GroentijdModel{ FaseCyclus = "01", Waarde = 0}
                }, Naam = "MG1", Type = GroentijdenTypeEnum.MaxGroentijden
            });
            var vm = new GroentijdenSetViewModel(model.GroentijdenSets[0]);
            
            vm.Type = GroentijdenTypeEnum.VerlengGroentijden;

            Assert.That("VG1" == vm.Naam);
        }

        [Test]
        public void GronetijdSetViewModel_TypeChangedFormVGToMG_NameChangedCorrectly()
        {
            Messenger.OverrideDefault(FakesCreator.CreateMessenger());
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.GroentijdenSets.Add(new GroentijdenSetModel
            {
                Groentijden = new List<GroentijdModel>
                {
                    new GroentijdModel{ FaseCyclus = "01", Waarde = 0}
                },
                Naam = "VG1",
                Type = GroentijdenTypeEnum.VerlengGroentijden
            });
            var vm = new GroentijdenSetViewModel(model.GroentijdenSets[0]);

            vm.Type = GroentijdenTypeEnum.MaxGroentijden;

            Assert.That("MG1" == vm.Naam);
        }
    }
}