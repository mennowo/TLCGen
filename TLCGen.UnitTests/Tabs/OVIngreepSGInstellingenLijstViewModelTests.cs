using NUnit.Framework;
using System.Linq;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.FasenTab.Other
{
    [TestFixture]
    public class OVIngreepSGInstellingenLijstViewModelTests
    {
        //[Test]
        //public void OnOVIngreepSignaalGroepParametersChanged_ModelWithGelijkstart_FasenWithGelijkstartAllChanged()
        //{
        //    var model = new ControllerModel();
        //
        //    model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
        //    model.Fasen.Add(new FaseCyclusModel { Naam = "02" });
        //    model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
        //    model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
        //    model.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel { FaseCyclus = "01" });
        //    model.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel { FaseCyclus = "02" });
        //    model.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel { FaseCyclus = "03" });
        //    model.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel { FaseCyclus = "04" });
        //    model.InterSignaalGroep.Gelijkstarten.Add(new GelijkstartModel { FaseVan = "02", FaseNaar = "03" });
        //    var vm = new PrioriteitSignaalGroepInstellingenTabViewModel { Controller = model };
        //
        //    vm.IngreepSGParameters.First(x => x.FaseCyclus == "02").MinimumGroentijdConflictOVRealisatie = 55;
        //    vm.OnOVIngreepSignaalGroepParametersChanged(new Messaging.Messages.PrioIngreepSignaalGroepParametersChangedMessage(vm.OVIngreepSGParameters.First(x => x.FaseCyclus == "02").Parameters));
        //    
        //    Assert.That(model.PrioData.PrioIngreepSignaalGroepParameters.First(x => x.FaseCyclus == "03").MinimumGroentijdConflictOVRealisatie == 55);
        //}
    }
}
