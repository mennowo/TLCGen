using NUnit.Framework;
using System.Linq;
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

            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel { FaseCyclus = "01" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel { FaseCyclus = "02" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel { FaseCyclus = "03" });
            model.OVData.OVIngreepSignaalGroepParameters.Add(new OVIngreepSignaalGroepParametersModel { FaseCyclus = "04" });
            model.InterSignaalGroep.Gelijkstarten.Add(new GelijkstartModel { FaseVan = "02", FaseNaar = "03" });
            var vm = new OVSignaalGroepInstellingenTabViewModel { Controller = model };

            vm.OVIngreepSGParameters.First(x => x.FaseCyclus == "02").MinimumGroentijdConflictOVRealisatie = 55;
            vm.OnOVIngreepSignaalGroepParametersChanged(new Messaging.Messages.OVIngreepSignaalGroepParametersChangedMessage(vm.OVIngreepSGParameters.First(x => x.FaseCyclus == "02").Parameters));
            
            Assert.AreEqual(model.OVData.OVIngreepSignaalGroepParameters.First(x => x.FaseCyclus == "03").MinimumGroentijdConflictOVRealisatie, 55);
        }
    }
}
