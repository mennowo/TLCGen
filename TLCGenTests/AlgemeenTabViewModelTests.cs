using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGenTests
{
    [TestFixture]
    public class AlgemeenTabViewModelTests
    {
        private MainWindowViewModel mainwinvm;

        [SetUp]
        public void StartTesting()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
            mainwinvm = new MainWindowViewModel();
        }

        [Test]
        public void AlgemeenVersioning()
        {
            Assert.IsTrue(mainwinvm.NewFileCommand.CanExecute(null));
            mainwinvm.NewFileCommand.Execute(null);
            var tab = mainwinvm.ControllerVM.AlgemeenTabVM;

            // empty version list
            Assert.IsTrue(tab.AddVersieCommand.CanExecute(null));
            Assert.IsFalse(tab.RemoveVersieCommand.CanExecute(null));
            Assert.IsTrue(tab.Versies.Count == 0);

            // add versions and test automatic versioning
            tab.AddVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 1);
            Assert.IsTrue(tab.Versies[0].Versie == "1.0.0");

            tab.AddVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 2);
            Assert.IsTrue(tab.Versies[1].Versie == "1.1.0");

            tab.Versies[1].Versie = "2.0.0";
            tab.AddVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 3);
            Assert.IsTrue(tab.Versies[2].Versie == "2.1.0");

            // remove versions and assert they disappear
            tab.SelectedVersie = tab.Versies[1];
            Assert.IsTrue(tab.RemoveVersieCommand.CanExecute(null));
            tab.RemoveVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 2);

            Assert.IsFalse(tab.RemoveVersieCommand.CanExecute(null));
            tab.SelectedVersie = tab.Versies[1];
            Assert.IsTrue(tab.RemoveVersieCommand.CanExecute(null));
            tab.RemoveVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 1);

            Assert.IsFalse(tab.RemoveVersieCommand.CanExecute(null));
            tab.SelectedVersie = tab.Versies[0];
            Assert.IsTrue(tab.RemoveVersieCommand.CanExecute(null));
            tab.RemoveVersieCommand.Execute(null);
            Assert.IsTrue(tab.Versies.Count == 0);

            mainwinvm.ControllerVM.HasChanged = false;
            Assert.IsTrue(mainwinvm.CloseFileCommand.CanExecute(null));
            mainwinvm.CloseFileCommand.Execute(null);
        }
    }
}
