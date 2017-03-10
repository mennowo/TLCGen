using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.Other
{
    [TestFixture]
    public class OVIngreepSGInstellingenLijstViewModelTests
    {
        [Test]
        public void OnFasenChanged_FaseAddedToModel_TabAlsoExposes5Fasen()
        {
            var model = new ControllerModel();
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            var vm = new OVIngreepSGInstellingenLijstViewModel();
            vm.Controller = model;

            var newfc = new FaseCyclusModel() { Naam = "05" };
            model.Fasen.Add(newfc);
            vm.OnFasenChanged(new Messaging.Messages.FasenChangedMessage(new List<FaseCyclusModel>() { newfc }, null));

            Assert.AreEqual(5, vm.OVIngreepSGParameters.Count);
        }
    }
}
