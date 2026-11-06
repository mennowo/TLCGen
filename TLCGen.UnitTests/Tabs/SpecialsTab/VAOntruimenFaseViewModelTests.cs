using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using NSubstitute;
using NUnit.Framework;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests.Tabs.SpecialsTab
{
    [TestFixture]
    [ResetGlobalState]
    [NonParallelizable]
    public class VAOntruimenFaseViewModelTests
    {
        [Test]
        public void AvailableManualConflicts_ExcludesTheFaseItselfAndItsHardConflicts()
        {
            var vm = CreateViewModel();

            Assert.That(vm.AvailableManualConflicts, Is.EqualTo(new[] { "33" }));
        }

        [Test]
        public void AddManualConflictCommand_AddsAMatrixRowAndAnEntryInEveryDetector()
        {
            var model = new VAOntruimenFaseModel { FaseCyclus = "05" };
            var vm = CreateViewModel(model);

            vm.SelectedManualConflictToAdd = "33";
            vm.AddManualConflictCommand.Execute(null);

            Assert.That(model.VADetectoren[0].ManualConflicts.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "33" }));
            Assert.That(vm.VAOntruimenMatrixRowHeaders.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "26", "33" }));
            Assert.That(vm.VAOntruimenMatrixRowHeaders.Select(x => x.IsManual), Is.EqualTo(new[] { false, true }),
                "the manual row must be identifiable for the blue row header");
            Assert.That(vm.AvailableManualConflicts, Is.Empty, "an added conflict can no longer be added");
        }

        [Test]
        public void RemoveManualConflictCommand_RemovesTheEntryAndTheMatrixRow()
        {
            var model = new VAOntruimenFaseModel { FaseCyclus = "05" };
            var vm = CreateViewModel(model);
            vm.SelectedManualConflictToAdd = "33";
            vm.AddManualConflictCommand.Execute(null);

            vm.SelectedManualConflictToRemove = vm.ManualConflictFases.Single();
            vm.RemoveManualConflictCommand.Execute(null);

            Assert.That(model.VADetectoren[0].ManualConflicts, Is.Empty);
            Assert.That(vm.VAOntruimenMatrixRowHeaders.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "26" }));
            Assert.That(vm.AvailableManualConflicts, Is.EqualTo(new[] { "33" }));
        }

        [Test]
        public void ManualConflict_HasATimePerDetector()
        {
            var model = new VAOntruimenFaseModel { FaseCyclus = "05" };
            model.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } }
            });
            model.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "052",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } }
            });
            var vm = CreateViewModel(model);

            vm.SelectedManualConflictToAdd = "33";
            vm.AddManualConflictCommand.Execute(null);

            // membership is fase-wide, but each detector has its own cell (and thus its own time)
            Assert.That(model.VADetectoren.All(d => d.ManualConflicts.Any(x => x.FaseCyclus == "33")), Is.True);
            Assert.That(vm.VAOntruimenMatrix[1, 0], Is.Not.SameAs(vm.VAOntruimenMatrix[1, 1]));

            vm.VAOntruimenMatrix[1, 0].VAOntruimingsTijd = 5;

            Assert.That(model.VADetectoren[0].ManualConflicts.Single().VAOntruimingsTijd, Is.EqualTo(5));
            Assert.That(model.VADetectoren[1].ManualConflicts.Single().VAOntruimingsTijd, Is.EqualTo(0));
        }

        [Test]
        public void AddingADetector_CopiesTheManualConflictMembership()
        {
            var model = new VAOntruimenFaseModel { FaseCyclus = "05" };
            model.VADetectoren.Add(new VAOntruimenDetectorModel
            {
                Detector = "051",
                ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } },
                ManualConflicts = { new VAOntruimenNaarFaseModel { FaseCyclus = "33", VAOntruimingsTijd = 7 } }
            });
            var vm = CreateViewModel(model);

            vm.DetectorManager.SelectedItemToAdd = "052";
            vm.DetectorManager.AddItemCommand.Execute(null);

            var added = model.VADetectoren.Single(x => x.Detector == "052");
            Assert.That(added.ManualConflicts.Select(x => x.FaseCyclus), Is.EqualTo(new[] { "33" }));
            Assert.That(added.ManualConflicts[0].VAOntruimingsTijd, Is.EqualTo(0),
                "a new detector starts with its own (empty) time");
        }

        [Test]
        public void AddAndRemoveManualConflict_BroadcastsAChange()
        {
            var vm = CreateViewModel();
            var broadcasts = 0;
            var recipient = new object();
            WeakReferenceMessengerEx.Default.Register<BroadcastMessage>(recipient, (r, m) => ++broadcasts);

            vm.SelectedManualConflictToAdd = "33";
            vm.AddManualConflictCommand.Execute(null);
            Assert.That(broadcasts, Is.EqualTo(1), "adding a manual conflict must mark the controller as changed");

            vm.SelectedManualConflictToRemove = vm.ManualConflictFases.Single();
            vm.RemoveManualConflictCommand.Execute(null);
            Assert.That(broadcasts, Is.EqualTo(2), "removing a manual conflict must mark the controller as changed");
        }

        private static VAOntruimenFaseViewModel CreateViewModel(VAOntruimenFaseModel model = null)
        {
            model ??= new VAOntruimenFaseModel { FaseCyclus = "05" };
            if (model.VADetectoren.Count == 0)
            {
                model.VADetectoren.Add(new VAOntruimenDetectorModel
                {
                    Detector = "051",
                    ConflicterendeFasen = { new VAOntruimenNaarFaseModel { FaseCyclus = "26" } }
                });
            }

            var provider = Substitute.For<IControllerAccessProvider>();
            provider.AllSignalGroupStrings.Returns(new ObservableCollection<string> { "05", "26", "33" });
            provider.AllDetectorStrings.Returns(new ObservableCollection<string> { "051", "052" });
            ControllerAccessProvider.OverrideDefault(provider);

            return new VAOntruimenFaseViewModel(model);
        }
    }
}
