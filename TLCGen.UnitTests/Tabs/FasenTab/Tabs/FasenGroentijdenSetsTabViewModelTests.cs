using System;
using System.Linq;
using NUnit.Framework;
using TLCGen.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using NSubstitute;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.Messaging.Requests;
using TLCGen.Messaging.Messages;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests
{
    public class FasenGroentijdenSetsTabViewModelTests
    {
        IMessenger messenger;
        ISettingsProvider settingsprovider;
        ControllerModel model;
        Action<FasenChangedMessage> fasenchangedaction;
        Action<FasenSortedMessage> fasensortedaction;
        Action<NameChangedMessage> namechangedaction;
        Action<GroentijdenTypeChangedMessage> groentijdentypechangedaction;

        [SetUp]
        public void FasenGroentijdenSetsTabSetup()
        {
            model = new ControllerModel();
            messenger = Substitute.For<IMessenger>();
            settingsprovider = Substitute.For<ISettingsProvider>();

            // In register methods: set a local variable to the action of the VM,
            // so we can invoke the action
            messenger.
                When(x => x.Register(Arg.Any<object>(), Arg.Any<Action<FasenChangedMessage>>())).
                Do(c =>
                {
                    fasenchangedaction = c.Arg<Action<FasenChangedMessage>>();
                });
            messenger.
                When(x => x.Register(Arg.Any<object>(), Arg.Any<Action<FasenSortedMessage>>())).
                Do(c =>
                {
                    fasensortedaction = c.Arg<Action<FasenSortedMessage>>();
                });
            messenger.
                When(x => x.Register(Arg.Any<object>(), Arg.Any<Action<NameChangedMessage>>())).
                Do(c =>
                {
                    namechangedaction = c.Arg<Action<NameChangedMessage>>();
                });
            messenger.
                When(x => x.Register(Arg.Any<object>(), Arg.Any<Action<GroentijdenTypeChangedMessage>>())).
                Do(c =>
                {
                    groentijdentypechangedaction = c.Arg<Action<GroentijdenTypeChangedMessage>>();
                });

            // Send methods: fake checking for unique elements
            messenger.
                When(x => x.Send(Arg.Any<IsElementIdentifierUniqueRequest>())).
                Do(c =>
                {
                    c.Arg<IsElementIdentifierUniqueRequest>().Handled = true;
                    c.Arg<IsElementIdentifierUniqueRequest>().IsUnique = model.Fasen.All(x =>
                    {
                        return c.Arg<IsElementIdentifierUniqueRequest>().Type == ElementIdentifierType.Naam && x.Naam != c.Arg<IsElementIdentifierUniqueRequest>().Identifier;
                    });
                });

            Messenger.OverrideDefault(messenger);
            SettingsProvider.OverrideDefault(settingsprovider);
        }

        [Test]
        public void RemoveGroentijdenSetCommand_NoSetPresent_CannotExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            bool result = vm.RemoveGroentijdenSetCommand.CanExecute(null);

            Assert.False(result);
        }

        [Test]
        public void RemoveGroentijdenSetCommand_SetPresentAndSelected_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);
            vm.SelectedSet = vm.GroentijdenSets[0];
            bool result = vm.RemoveGroentijdenSetCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddGroentijdenSetCommand_NoSetPresent_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            bool result = vm.AddGroentijdenSetCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddGroentijdenSetCommand_SetPresent_CanExecute()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);
            bool result = vm.AddGroentijdenSetCommand.CanExecute(null);

            Assert.True(result);
        }

        [Test]
        public void AddGroentijdenSetCommand_ExecutedWhile5FasenInModel_TabExposes5FasenNames()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);

            Assert.AreEqual(5, vm.FasenNames.Count);
        }

        [Test]
        public void AddGroentijdenSetCommand_ExecutedWhile5FasenInModel_AddsGroentijdenSetWith5FasenToModel()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;

            vm.AddGroentijdenSetCommand.Execute(null);
                        
            Assert.AreEqual(1, model.GroentijdenSets.Count);
            Assert.AreEqual(5, model.GroentijdenSets[0].Groentijden.Count);
            Assert.AreEqual("01", model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.AreEqual("02", model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.AreEqual("03", model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.AreEqual("05", model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }

        [Test]
        public void AddGroentijdenSetCommand_Executed5TimesWhile5FasenInModel_Adds5GroentijdenSetsWith5FasenToModel()
        {
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

            Assert.AreEqual(5, model.GroentijdenSets.Count);
            for (int i = 0; i < 5; ++i)
            {
                Assert.AreEqual(5, model.GroentijdenSets[i].Groentijden.Count);
                Assert.AreEqual("01", model.GroentijdenSets[i].Groentijden[0].FaseCyclus);
                Assert.AreEqual("02", model.GroentijdenSets[i].Groentijden[1].FaseCyclus);
                Assert.AreEqual("03", model.GroentijdenSets[i].Groentijden[2].FaseCyclus);
                Assert.AreEqual("04", model.GroentijdenSets[i].Groentijden[3].FaseCyclus);
                Assert.AreEqual("05", model.GroentijdenSets[i].Groentijden[4].FaseCyclus);
            }
        }

        [Test]
        public void RemoveGroentijdenSetCommand_ExecutedWhile2SetsModel_RemovesSelectedSetFromModel()
        {
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

            Assert.AreEqual(1, model.GroentijdenSets.Count);
            Assert.AreEqual(5, model.GroentijdenSets[0].Groentijden.Count);
            Assert.AreEqual("01", model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.AreEqual("02", model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.AreEqual("03", model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.AreEqual("05", model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
        }

        [Test]
        public void RemoveGroentijdenSetCommand_LastSetRemovedFromModel_CannotExecuteAnymore()
        {
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

            Assert.False(vm.RemoveGroentijdenSetCommand.CanExecute(null));
        }

        [Test]
        public void RemoveGroentijdenSetCommand_RemoveSetInMiddleOfSetList_RemainingSetsRenamedCorrectly()
        {
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
            
            Assert.AreEqual("MG1", vm.GroentijdenSets[0].Naam);
            Assert.AreEqual("MG2", vm.GroentijdenSets[1].Naam);
        }

        [Test]
        public void RenameFase_HigherThanOthers_FasenInGroentijdenSetSortedCorrectly()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vmfasen.Fasen[2].Naam = "07";
            namechangedaction.Invoke(new NameChangedMessage("03", "07"));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));

            Assert.AreEqual(1, model.GroentijdenSets.Count);
            Assert.AreEqual(5, model.GroentijdenSets[0].Groentijden.Count);
            Assert.AreEqual("01", model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.AreEqual("02", model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.AreEqual("05", model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.AreEqual("07", model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
            Assert.AreEqual("01", vm.FasenNames[0]);
            Assert.AreEqual("02", vm.FasenNames[1]);
            Assert.AreEqual("04", vm.FasenNames[2]);
            Assert.AreEqual("05", vm.FasenNames[3]);
            Assert.AreEqual("07", vm.FasenNames[4]);
        }

        [Test]
        public void RenameFase_HigherThanOthers_FasenNamesSortedCorrectly()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vmfasen.Fasen[2].Naam = "07";
            namechangedaction.Invoke(new NameChangedMessage("03", "07"));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));

            Assert.AreEqual(5, vm.FasenNames.Count);
            Assert.AreEqual("01", vm.FasenNames[0]);
            Assert.AreEqual("02", vm.FasenNames[1]);
            Assert.AreEqual("04", vm.FasenNames[2]);
            Assert.AreEqual("05", vm.FasenNames[3]);
            Assert.AreEqual("07", vm.FasenNames[4]);
        }

        [Test]
        public void RenameFase_LowerThanOthers_FasenInGroentijdenSetSortedCorrectly()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vmfasen.Fasen[2].Naam = "02";
            namechangedaction.Invoke(new NameChangedMessage("05", "02"));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));

            Assert.AreEqual(1, model.GroentijdenSets.Count);
            Assert.AreEqual(5, model.GroentijdenSets[0].Groentijden.Count);
            Assert.AreEqual("02", model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.AreEqual("03", model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.AreEqual("06", model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.AreEqual("08", model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
            Assert.AreEqual("02", vm.FasenNames[0]);
            Assert.AreEqual("03", vm.FasenNames[1]);
            Assert.AreEqual("04", vm.FasenNames[2]);
            Assert.AreEqual("06", vm.FasenNames[3]);
            Assert.AreEqual("08", vm.FasenNames[4]);
        }

        [Test]
        public void RenameFase_LowerThanOthers_FasenNamesSortedCorrectly()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;

            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vmfasen.Fasen[2].Naam = "02";
            namechangedaction.Invoke(new NameChangedMessage("05", "02"));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));
            
            Assert.AreEqual(5, vm.FasenNames.Count);
            Assert.AreEqual("02", vm.FasenNames[0]);
            Assert.AreEqual("03", vm.FasenNames[1]);
            Assert.AreEqual("04", vm.FasenNames[2]);
            Assert.AreEqual("06", vm.FasenNames[3]);
            Assert.AreEqual("08", vm.FasenNames[4]);
        }

        [Test]
        public void FaseAddedToModel_TwoGroentijdenSetsInModel_FaseAddedToBothSets()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;
            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            vmfasen.AddFaseCommand.Execute(null);
            fasenchangedaction.Invoke(
                new FasenChangedMessage(model.Fasen, 
                                        new System.Collections.Generic.List<FaseCyclusModel>() { vmfasen.Fasen[5].FaseCyclus },
                                        null));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));

            Assert.AreEqual(6, model.GroentijdenSets[0].Groentijden.Count);
            Assert.AreEqual("03", model.GroentijdenSets[0].Groentijden[0].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[0].Groentijden[1].FaseCyclus);
            Assert.AreEqual("05", model.GroentijdenSets[0].Groentijden[2].FaseCyclus);
            Assert.AreEqual("06", model.GroentijdenSets[0].Groentijden[3].FaseCyclus);
            Assert.AreEqual("08", model.GroentijdenSets[0].Groentijden[4].FaseCyclus);
            Assert.AreEqual("09", model.GroentijdenSets[0].Groentijden[5].FaseCyclus);
            Assert.AreEqual(6, model.GroentijdenSets[1].Groentijden.Count);
            Assert.AreEqual("03", model.GroentijdenSets[1].Groentijden[0].FaseCyclus);
            Assert.AreEqual("04", model.GroentijdenSets[1].Groentijden[1].FaseCyclus);
            Assert.AreEqual("05", model.GroentijdenSets[1].Groentijden[2].FaseCyclus);
            Assert.AreEqual("06", model.GroentijdenSets[1].Groentijden[3].FaseCyclus);
            Assert.AreEqual("08", model.GroentijdenSets[1].Groentijden[4].FaseCyclus);
            Assert.AreEqual("09", model.GroentijdenSets[1].Groentijden[5].FaseCyclus);
        }

        [Test]
        public void FaseAddedToModel_TwoGroentijdenSetsInModel_FasenNamesReflectChange()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            var vmfasen = new FasenLijstTabViewModel();
            vmfasen.Controller = model;
            fasenchangedaction.Invoke(new FasenChangedMessage(model.Fasen, model.Fasen, null));
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            vmfasen.AddFaseCommand.Execute(null);
            fasenchangedaction.Invoke(
                new FasenChangedMessage(model.Fasen,
                                        new System.Collections.Generic.List<FaseCyclusModel>() { vmfasen.Fasen[5].FaseCyclus },
                                        null));
            vmfasen.OnDeselected();
            fasensortedaction.Invoke(new FasenSortedMessage(model.Fasen));

            Assert.AreEqual(6, vm.FasenNames.Count);
            Assert.AreEqual("03", vm.FasenNames[0]);
            Assert.AreEqual("04", vm.FasenNames[1]);
            Assert.AreEqual("05", vm.FasenNames[2]);
            Assert.AreEqual("06", vm.FasenNames[3]);
            Assert.AreEqual("08", vm.FasenNames[4]);
            Assert.AreEqual("09", vm.FasenNames[5]);
        }

        [Test]
        public void GroentijdenTypeChanged_FromMGToVG_AllGroentijdenSetsHaveNameAndTypeChanged()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Data.TypeGroentijden = GroentijdenTypeEnum.MaxGroentijden;
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            model.Data.TypeGroentijden = GroentijdenTypeEnum.VerlengGroentijden;
            groentijdentypechangedaction.Invoke(new GroentijdenTypeChangedMessage(GroentijdenTypeEnum.VerlengGroentijden));

            Assert.AreEqual("VG1", model.GroentijdenSets[0].Naam);
            Assert.AreEqual("VG2", model.GroentijdenSets[1].Naam);
            Assert.AreEqual("VG3", model.GroentijdenSets[2].Naam);
            Assert.AreEqual(GroentijdenTypeEnum.VerlengGroentijden, model.GroentijdenSets[0].Type);
            Assert.AreEqual(GroentijdenTypeEnum.VerlengGroentijden, model.GroentijdenSets[1].Type);
            Assert.AreEqual(GroentijdenTypeEnum.VerlengGroentijden, model.GroentijdenSets[2].Type);
        }

        [Test]
        public void GroentijdenTypeChanged_FromVGToMG_AllGroentijdenSetsHaveNameAndTypeChanged()
        {
            model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "06" });
            model.Fasen.Add(new FaseCyclusModel() { Naam = "08" });
            model.Data.TypeGroentijden = GroentijdenTypeEnum.VerlengGroentijden;
            var vm = new FasenGroentijdenSetsTabViewModel();
            vm.Controller = model;
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);
            vm.AddGroentijdenSetCommand.Execute(null);

            model.Data.TypeGroentijden = GroentijdenTypeEnum.MaxGroentijden;
            groentijdentypechangedaction.Invoke(new GroentijdenTypeChangedMessage(GroentijdenTypeEnum.MaxGroentijden));

            Assert.AreEqual("MG1", model.GroentijdenSets[0].Naam);
            Assert.AreEqual("MG2", model.GroentijdenSets[1].Naam);
            Assert.AreEqual("MG3", model.GroentijdenSets[2].Naam);
            Assert.AreEqual(GroentijdenTypeEnum.MaxGroentijden, model.GroentijdenSets[0].Type);
            Assert.AreEqual(GroentijdenTypeEnum.MaxGroentijden, model.GroentijdenSets[1].Type);
            Assert.AreEqual(GroentijdenTypeEnum.MaxGroentijden, model.GroentijdenSets[2].Type);
        }
    }
}
