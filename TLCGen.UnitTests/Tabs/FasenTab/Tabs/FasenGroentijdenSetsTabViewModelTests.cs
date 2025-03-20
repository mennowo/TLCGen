using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TLCGen.ViewModels;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;
using TLCGen.DataAccess;
using TLCGen.ModelManagement;

namespace TLCGen.UnitTests
{
    [TestFixture]
    public class FasenGroentijdenSetsTabViewModelTests
    {
        [Test]
        public void RemoveGroentijdenSetCommand_NoSetPresent_CannotExecute()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            var result = vm.RemoveGroentijdenSetCommand.CanExecute(null);

            Assert.That(!result);
        }

        [Test]
        public void RemoveGroentijdenSetCommand_SetPresentAndSelected_CanExecute()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[0];
            var result = vm.RemoveGroentijdenSetCommand.CanExecute(null);
        
            Assert.That(result);
        }
        
        [Test]
        public void AddGroentijdenSetCommand_NoSetPresent_CanExecute()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            var result = vm.AddGroentijdenSetCommand.CanExecute(null);
        
            Assert.That(result);
        }
        
        [Test]
        public void AddGroentijdenSetCommand_SetPresent_CanExecute()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            var result = vm.AddGroentijdenSetCommand.CanExecute(null);
        
            Assert.That(result);
        }
        
        [Test]
        public void AddGroentijdenSetCommand_ExecutedWhile5FasenInModel_TabExposes5FasenNames()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
        
            Assert.That(5 == vm.FasenNames.Count);
        }
        
        [Test]
        public void AddGroentijdenSetCommand_ExecutedWhile5FasenInModel_AddsGroentijdenSetWith5FasenToModel()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
                        
            Assert.That(1 == model.GroentijdenSets.Count);
            Assert.That(5 == model.GroentijdenSets[0].Groentijden.Count);
            Assert.That("01" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("02" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("03" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }
        
        [Test]
        public void AddGroentijdenSetCommand_Executed5TimesWhile5FasenInModel_Adds5GroentijdenSetsWith5FasenToModel()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
        
            Assert.That(5 == model.GroentijdenSets.Count);
            for (var i = 0; i < 5; ++i)
            {
                Assert.That(5 == model.GroentijdenSets[i].Groentijden.Count);
                Assert.That("01" == model.GroentijdenSets[i].Groentijden[0].FaseCyclus);
                Assert.That("02" == model.GroentijdenSets[i].Groentijden[1].FaseCyclus);
                Assert.That("03" == model.GroentijdenSets[i].Groentijden[2].FaseCyclus);
                Assert.That("04" == model.GroentijdenSets[i].Groentijden[3].FaseCyclus);
                Assert.That("05" == model.GroentijdenSets[i].Groentijden[4].FaseCyclus);
            }
        }
        
        [Test]
        public void RemoveGroentijdenSetCommand_ExecutedWhile2SetsModel_RemovesSelectedSetFromModel()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[0];
            vm.RemoveGroentijdenSetCommand.Execute(null);
        
            Assert.That(1 == model.GroentijdenSets.Count);
            Assert.That(5 == model.GroentijdenSets[0].Groentijden.Count);
            Assert.That("01" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("02" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("03" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }
        
        [Test]
        public void RemoveGroentijdenSetCommand_LastSetRemovedFromModel_CannotExecuteAnymore()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[0];
            vm.RemoveGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[0];
            vm.RemoveGroentijdenSetCommand.Execute(null);
        
            Assert.That(!vm.RemoveGroentijdenSetCommand.CanExecute(null));
        }
        
        [Test]
        public void RemoveGroentijdenSetCommand_RemoveSetInMiddleOfSetList_RemainingSetsRenamedCorrectly()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[1];
            vm.RemoveGroentijdenSetCommand.Execute(null);
            
            Assert.That("MG1" == vm.GroentijdenSets[0].Naam);
            Assert.That("MG2" == vm.GroentijdenSets[1].Naam);
        }
        
        [Test]
        public void OnNameChanged_FaseRenamedHigherThanOthers_FaseNamedCorrectlyInGroentijdenSet()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;
        
            vm.AddGroentijdenSetCommand.Execute(null);
            var oldname = vmfasen.Fasen[2].Naam;
            vmfasen.Fasen[2].Naam = "07";
            vm.OnNameChanged(this, new NameChangedMessage(TLCGenObjectTypeEnum.Fase, oldname, vmfasen.Fasen[2].Naam));
            
            Assert.That("01" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("02" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That("07" == model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }

        [Test]
        public void OnNameChanged_FaseRenamedHigherThanOthers_FaseNamedCorrectlyForDisplay()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            model.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel {Controller = model};
            var vmfasen = new FasenLijstTabViewModel {Controller = model};

            vm.AddGroentijdenSetCommand.Execute(null);
            vmfasen.Fasen[2].Naam = "07";

            Assert.That("01" == vm.FasenNames[0]);
            Assert.That("02" == vm.FasenNames[1]);
            Assert.That("04" == vm.FasenNames[2]);
            Assert.That("05" == vm.FasenNames[3]);
            Assert.That("07" == vm.FasenNames[4]);
        }

        [Test]
        public void OnNameChanged_FaseRenamedLowerThanOthers_FaseNamedCorrectlyInGroentijdenSet()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "07" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "09" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);
            var oldname = vmfasen.Fasen[2].Naam;
            vmfasen.Fasen[2].Naam = "02";
            vm.OnNameChanged(this, new NameChangedMessage(TLCGenObjectTypeEnum.Fase, oldname, vmfasen.Fasen[2].Naam));

            Assert.That("02" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("06" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("08" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That("09" == model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }

        [Test]
        public void OnNameChanged_FaseRenamedLowerThanOthers_FaseNamedCorrectlyForDisplay()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "07" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "09" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);
            var oldname = vmfasen.Fasen[2].Naam;
            vmfasen.Fasen[2].Naam = "02";
            vm.OnNameChanged(this, new NameChangedMessage(TLCGenObjectTypeEnum.Fase, oldname, vmfasen.Fasen[2].Naam));

            Assert.That("02" == vm.FasenNames[0]);
            Assert.That("05" == vm.FasenNames[1]);
            Assert.That("06" == vm.FasenNames[2]);
            Assert.That("08" == vm.FasenNames[3]);
            Assert.That("09" == vm.FasenNames[4]);
        }

        [Test]
        public void FaseAddedToModel_TwoGroentijdenSetsInModel_FaseAddedToBothSets()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel {Controller = model};
            var vmfasen = new FasenLijstTabViewModel {Controller = model};
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            vmfasen.AddFaseCommand.Execute(null);
            vm.OnFasenChanged(this, new FasenChangedMessage(new List<FaseCyclusModel>{model.Fasen[5]}, null));

            Assert.That(6 == model.GroentijdenSets[0].Groentijden.Count);
            Assert.That("03" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("06" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That("08" == model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
            Assert.That("09" == model.GroentijdenSets[0].Groentijden[5].FaseCyclus);
            Assert.That(6 == model.GroentijdenSets[1].Groentijden.Count);
            Assert.That("03" == model.GroentijdenSets[1].Groentijden[0].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[1].Groentijden[1].FaseCyclus);
            Assert.That("05" == model.GroentijdenSets[1].Groentijden[2].FaseCyclus);
            Assert.That("06" == model.GroentijdenSets[1].Groentijden[3].FaseCyclus);
            Assert.That("08" == model.GroentijdenSets[1].Groentijden[4].FaseCyclus);
            Assert.That("09" == model.GroentijdenSets[1].Groentijden[5].FaseCyclus);
        }

        [Test]
        public void FaseRemovedFromModel_TwoGroentijdenSetsInModel_FaseRemovedFromBothSets()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager{Controller = model});
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(model));
            model.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel { Controller = model };
            var vmfasen = new FasenLijstTabViewModel { Controller = model };
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            vmfasen.SelectedFaseCyclus = vmfasen.Fasen[2];
            vmfasen.RemoveFaseCommand.Execute(null);

            Assert.That(4 == model.GroentijdenSets[0].Groentijden.Count);
            Assert.That("03" == model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.That("06" == model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.That("08" == model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.That(4 == model.GroentijdenSets[1].Groentijden.Count);
            Assert.That("03" == model.GroentijdenSets[1].Groentijden[0].FaseCyclus);
            Assert.That("04" == model.GroentijdenSets[1].Groentijden[1].FaseCyclus);
            Assert.That("06" == model.GroentijdenSets[1].Groentijden[2].FaseCyclus);
            Assert.That("08" == model.GroentijdenSets[1].Groentijden[3].FaseCyclus);
        }

        [Test]
        public void GroentijdenTypeChanged_FromMGToVG_AllGroentijdenSetsHaveNameAndTypeChanged()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Data.TypeGroentijden = GroentijdenTypeEnum.MaxGroentijden;
            var vm = new FasenGroentijdenSetsTabViewModel {Controller = model};
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
        
            model.Data.TypeGroentijden = GroentijdenTypeEnum.VerlengGroentijden;
            vm.OnGroentijdenTypeChanged(this, new GroentijdenTypeChangedMessage(GroentijdenTypeEnum.VerlengGroentijden));
        
            Assert.That("VG1" == model.GroentijdenSets[0].Naam);
            Assert.That("VG2" == model.GroentijdenSets[1].Naam);
            Assert.That("VG3" == model.GroentijdenSets[2].Naam);
            Assert.That(GroentijdenTypeEnum.VerlengGroentijden == model.GroentijdenSets[0].Type);
            Assert.That(GroentijdenTypeEnum.VerlengGroentijden == model.GroentijdenSets[1].Type);
            Assert.That(GroentijdenTypeEnum.VerlengGroentijden == model.GroentijdenSets[2].Type);
        }
        
        [Test]
        public void GroentijdenTypeChanged_FromVGToMG_AllGroentijdenSetsHaveNameAndTypeChanged()
        {
            var model = new ControllerModel();
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Data.TypeGroentijden = GroentijdenTypeEnum.VerlengGroentijden;
            var vm = new FasenGroentijdenSetsTabViewModel {Controller = model};
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
        
            model.Data.TypeGroentijden = GroentijdenTypeEnum.MaxGroentijden;
            vm.OnGroentijdenTypeChanged(this, new GroentijdenTypeChangedMessage(GroentijdenTypeEnum.MaxGroentijden));

            Assert.That("MG1" == model.GroentijdenSets[0].Naam);
            Assert.That("MG2" == model.GroentijdenSets[1].Naam);
            Assert.That("MG3" == model.GroentijdenSets[2].Naam);
            Assert.That(GroentijdenTypeEnum.MaxGroentijden == model.GroentijdenSets[0].Type);
            Assert.That(GroentijdenTypeEnum.MaxGroentijden == model.GroentijdenSets[1].Type);
            Assert.That(GroentijdenTypeEnum.MaxGroentijden == model.GroentijdenSets[2].Type);
        }
    }
}
