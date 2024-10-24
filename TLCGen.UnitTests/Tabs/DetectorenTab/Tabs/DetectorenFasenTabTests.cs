﻿using System.Collections.Generic;
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

            Assert.That(5 == vm.Fasen.Count);
        }

	    [Test]
	    public void DetectorenFasenTabOnSelected_ControllerHas5Fasen_FirstFaseAndDetectorSelected()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};

		    vm.OnSelected();

		    Assert.That("01" == vm.SelectedFaseNaam);
		    Assert.That("011" == vm.SelectedDetector.Naam);
	    }

	    [Test]
	    public void DetectorenFasenTabSelectedFaseNaam_SetToDifferentFase_DetectorsChangeAccordingly()
	    {
		    var model = ControllerCreator.GetSmallControllerWithDetection();
		    var vm = new DetectorenFasenTabViewModel {Controller = model};

		    vm.OnSelected();
		    vm.SelectedFaseNaam = "02";

		    Assert.That(3 == vm.Detectoren.Count);
		    Assert.That("021" == vm.SelectedDetector.Naam);
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

		    Assert.That(3 == vm.Detectoren.Count);
		    var fc = model.Fasen[0];
		    Assert.That("011" == fc.Detectoren[0].Naam);
		    Assert.That("012" == fc.Detectoren[1].Naam);
		    Assert.That("013" == fc.Detectoren[2].Naam);
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

		    Assert.That(DetectorTypeEnum.Kop == model.Fasen[0].Detectoren[0].Type);
		    Assert.That(DetectorTypeEnum.Lang == model.Fasen[0].Detectoren[1].Type);
		    Assert.That(DetectorTypeEnum.Kop == model.Fasen[1].Detectoren[0].Type);
		    Assert.That(DetectorTypeEnum.Knop == model.Fasen[1].Detectoren[1].Type);
		    Assert.That(DetectorTypeEnum.KnopBuiten == model.Fasen[2].Detectoren[0].Type);
		    Assert.That(DetectorTypeEnum.KnopBinnen == model.Fasen[2].Detectoren[1].Type);
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

		    controllermodifiermock.Received().RemoveModelItemFromController("011", TLCGenObjectTypeEnum.Detector);
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

		    controllermodifiermock.Received().RemoveModelItemFromController("021", TLCGenObjectTypeEnum.Detector);
		    controllermodifiermock.Received().RemoveModelItemFromController("022", TLCGenObjectTypeEnum.Detector);
		    controllermodifiermock.Received().RemoveModelItemFromController("023", TLCGenObjectTypeEnum.Detector);
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
		    vm.Detectoren[2].AanvraagDirect = NooitAltijdAanUitEnum.SchAan;

		    Assert.That(vm.Detectoren[0].AanvraagDirect == NooitAltijdAanUitEnum.SchAan);
		    Assert.That(vm.Detectoren[1].AanvraagDirect == NooitAltijdAanUitEnum.SchAan);
		    Assert.That(vm.Detectoren[2].AanvraagDirect == NooitAltijdAanUitEnum.SchAan);
		    Assert.That(model.Fasen[1].Detectoren[0].AanvraagDirectSch == NooitAltijdAanUitEnum.SchAan);
		    Assert.That(model.Fasen[1].Detectoren[1].AanvraagDirectSch == NooitAltijdAanUitEnum.SchAan);
		    Assert.That(model.Fasen[1].Detectoren[2].AanvraagDirectSch == NooitAltijdAanUitEnum.SchAan);
	    }
    }
}
