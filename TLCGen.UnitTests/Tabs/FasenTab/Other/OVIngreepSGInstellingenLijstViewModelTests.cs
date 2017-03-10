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
        public void OnOVIngreepSignaalGroepParametersChanged_ModelWithGelijkstart_FasenWithGelijkstartAllChanged()
        {
            var model = new ControllerModel();

            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel() { FaseCyclus = "01" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel() { FaseCyclus = "02" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel() { FaseCyclus = "03" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel() { FaseCyclus = "04" });
            model.InterSignaalGroep.Gelijkstarten.Add(new GelijkstartModel() { FaseVan = "02", FaseNaar = "03" });
            var vm = new OVIngreepSGInstellingenLijstViewModel();
            vm.Controller = model;

            vm.OVIngreepSGParameters.Where(x => x.FaseCyclus == "02").First().MinimumGroentijdConflictOVRealisatie = 55;
            vm.OnOVIngreepSignaalGroepParametersChanged(new Messaging.Messages.OVIngreepSignaalGroepParametersChangedMessage(vm.OVIngreepSGParameters.Where(x => x.FaseCyclus == "02").First().Parameters));
            
            Assert.AreEqual(model.OVData.OVIngreepSignaalGroepParameters.Where(x => x.FaseCyclus == "03").First().MinimumGroentijdConflictOVRealisatie, 55);
        }
    }
}
