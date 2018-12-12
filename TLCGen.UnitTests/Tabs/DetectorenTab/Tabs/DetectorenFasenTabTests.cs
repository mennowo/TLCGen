using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using NUnit.Framework;
using TLCGen.Integrity;
using TLCGen.ViewModels;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;
using TLCGen.ModelManagement;

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class DetectorenFasenTabTests
    {
        [Test]
        public void DetectorenFasenTabOnSelected_ControllerHas5Fasen_TabAlsoExposes5Fasen()
        {
	        var model = ControllerCreator.GetSmallControllerWithDetection();
	        var vm = new DetectorenFasenTabViewModel {Controller = model};

	        vm.OnSelected();

            Assert.AreEqual(5, vm.Fasen.Count);
        }

	    [Test]
	    public void DetectorenFasenTabOnSelected_ControllerHas5Fasen_FirstFaseAndDetectorSelected()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};

		    vm.OnSelected();

		    Assert.AreEqual("01", vm.SelectedFaseNaam);
		    Assert.AreEqual("011", vm.SelectedDetector.Naam);
	    }

	    [Test]
	    public void DetectorenFasenTabSelectedFaseNaam_SetToDifferentFase_DetectorsChangeAccordingly()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};

		    vm.OnSelected();
		    vm.SelectedFaseNaam = "02";

		    Assert.AreEqual(3, vm.Detectoren.Count);
		    Assert.AreEqual("021", vm.SelectedDetector.Naam);
	    }

	    [Test]
	    public void DetectorenFasenTabAddDetectorCommand_Executed_DetectorAddedToRightFase()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};
		    Messenger.OverrideDefault(FakesCreator.CreateMessenger(model));
		    DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(FakesCreator.CreateModelManager(model));

            vm.OnSelected();
			vm.AddDetectorCommand.Execute(null);

		    Assert.AreEqual(3, vm.Detectoren.Count);
		    var fc = model.Fasen[0];
		    Assert.AreEqual("011", fc.Detectoren[0].Naam);
		    Assert.AreEqual("012", fc.Detectoren[1].Naam);
		    Assert.AreEqual("013", fc.Detectoren[2].Naam);
	    }

	    [Test]
	    public void DetectorenFasenTabAddDetectorCommand_Executed_DetectorTypeSetRight()
	    {
		    var model = ControllerCreator.GetSmallControllerWithoutDetection();
		    model.Fasen[0].Type = FaseTypeEnum.Auto;
		    model.Fasen[1].Type = FaseTypeEnum.Fiets;
		    model.Fasen[2].Type = FaseTypeEnum.Voetganger;
		    var vm = new DetectorenFasenTabViewModel {Controller = model};
		    var messengermock = FakesCreator.CreateMessenger(model);
		    Messenger.OverrideDefault(messengermock);
		    var defaultsprovidermock = FakesCreator.CreateDefaultsProvider();
		    DefaultsProvider.OverrideDefault(defaultsprovidermock);

		    vm.OnSelected();
		    vm.AddDetectorCommand.Execute(null);
		    vm.AddDetectorCommand.Execute(null);
		    vm.SelectedFaseNaam = "02";
		    vm.AddDetectorCommand.Execute(null);
		    vm.AddDetectorCommand.Execute(null);
		    vm.SelectedFaseNaam = "03";
		    vm.AddDetectorCommand.Execute(null);
		    vm.AddDetectorCommand.Execute(null);

		    Assert.AreEqual(DetectorTypeEnum.Kop, model.Fasen[0].Detectoren[0].Type);
		    Assert.AreEqual(DetectorTypeEnum.Lang, model.Fasen[0].Detectoren[1].Type);
		    Assert.AreEqual(DetectorTypeEnum.Kop, model.Fasen[1].Detectoren[0].Type);
		    Assert.AreEqual(DetectorTypeEnum.Knop, model.Fasen[1].Detectoren[1].Type);
		    Assert.AreEqual(DetectorTypeEnum.KnopBuiten, model.Fasen[2].Detectoren[0].Type);
		    Assert.AreEqual(DetectorTypeEnum.KnopBinnen, model.Fasen[2].Detectoren[1].Type);
	    }

	    [Test]
	    public void DetectorenFasenTabRemoveDetectorCommand_ExecutedWithSingleDetectorSelected_DetectorRemovedFromModel()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};
		    var controllermodifiermock = FakesCreator.CreateControllerModifier();
		    TLCGenControllerModifier.OverrideDefault(controllermodifiermock);

		    vm.OnSelected();
		    vm.RemoveDetectorCommand.Execute(null);

		    controllermodifiermock.Received().RemoveDetectorFromController("011");
	    }

	    [Test]
	    public void DetectorenFasenTabRemoveDetectorCommand_ExecutedWithMultipleDetectorsSelected_DetectorsRemovedFromModel()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};
		    var controllermodifiermock = FakesCreator.CreateControllerModifier();
		    TLCGenControllerModifier.OverrideDefault(controllermodifiermock);

		    vm.OnSelected();
		    vm.SelectedFaseNaam = "02";
		    vm.SelectedDetectoren = new List<DetectorViewModel>
		    {
			    vm.Detectoren[0],
			    vm.Detectoren[1],
			    vm.Detectoren[2]
		    };
		    vm.RemoveDetectorCommand.Execute(null);

		    controllermodifiermock.Received().RemoveDetectorFromController("021");
		    controllermodifiermock.Received().RemoveDetectorFromController("022");
		    controllermodifiermock.Received().RemoveDetectorFromController("023");
	    }

	    [Test]
	    public void DetectorenFasenTabDetectorPropertyChanged_PropertyChangedWithMultipleDetectorsSelected_AllDetectorsChanged()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};
		    var controllermodifiermock = FakesCreator.CreateControllerModifier();
		    TLCGenControllerModifier.OverrideDefault(controllermodifiermock);

		    vm.OnSelected();
		    vm.SelectedFaseNaam = "02";
		    vm.SelectedDetectoren = new List<DetectorViewModel>
		    {
			    vm.Detectoren[0],
			    vm.Detectoren[1],
			    vm.Detectoren[2]
		    };
		    vm.Detectoren[2].AanvraagDirect = true;

		    Assert.AreEqual(true, vm.Detectoren[0].AanvraagDirect);
		    Assert.AreEqual(true, vm.Detectoren[1].AanvraagDirect);
		    Assert.AreEqual(true, vm.Detectoren[2].AanvraagDirect);
		    Assert.AreEqual(true, model.Fasen[1].Detectoren[0].AanvraagDirect);
		    Assert.AreEqual(true, model.Fasen[1].Detectoren[1].AanvraagDirect);
		    Assert.AreEqual(true, model.Fasen[1].Detectoren[2].AanvraagDirect);
	    }
    }
}
