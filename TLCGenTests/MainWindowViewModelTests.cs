using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Messaging;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGenTests
{
    [TestFixture]
    public class MainWindowViewModelTests
    {
        private MainWindowViewModel mainwinvm;

        [SetUp]
        public void StartTesting()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
            mainwinvm = new MainWindowViewModel();
        }

        private void AssertIsNothingOpen(MainWindowViewModel mwvm)
        {
            Assert.IsTrue(mwvm.NewFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.CloseFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.SaveFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.SaveAsFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.ControllerVM == null);
        }

        private void AssertIsNewOpen(MainWindowViewModel mwvm)
        {
            Assert.IsTrue(mwvm.NewFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.ControllerVM != null);
            Assert.IsFalse(mwvm.ControllerVM.HasChanged);
            Assert.IsTrue(mwvm.HasController);
            Assert.IsTrue(mwvm.CloseFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.SaveFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.SaveAsFileCommand.CanExecute(null));
        }

        private void AssertIsNewUnsavedOpen(MainWindowViewModel mwvm)
        {
            Assert.IsTrue(mwvm.NewFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.ControllerVM != null);
            Assert.IsTrue(mwvm.ControllerVM.HasChanged);
            Assert.IsTrue(mwvm.HasController);
            Assert.IsTrue(mwvm.CloseFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.SaveFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.SaveAsFileCommand.CanExecute(null));
        }

        private void AssertIsSavedOpen(MainWindowViewModel mwvm)
        {
            Assert.IsTrue(mwvm.NewFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.ControllerVM != null);
            Assert.IsFalse(mwvm.ControllerVM.HasChanged);
            Assert.IsTrue(mwvm.HasController);
            Assert.IsTrue(mwvm.CloseFileCommand.CanExecute(null));
            Assert.IsFalse(mwvm.SaveFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.SaveAsFileCommand.CanExecute(null));
        }

        private void AssertIsUnsavedOpen(MainWindowViewModel mwvm)
        {
            Assert.IsTrue(mwvm.NewFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.ControllerVM != null);
            Assert.IsTrue(mwvm.ControllerVM.HasChanged);
            Assert.IsTrue(mwvm.HasController);
            Assert.IsTrue(mwvm.CloseFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.SaveFileCommand.CanExecute(null));
            Assert.IsTrue(mwvm.SaveAsFileCommand.CanExecute(null));
        }

        [Test]
        public void NewFileAndCloseFile()
        {
            AssertIsNothingOpen(mainwinvm);
            mainwinvm.NewFileCommand.Execute(null);
            AssertIsNewOpen(mainwinvm);
            mainwinvm.CloseFileCommand.Execute(null);
            AssertIsNothingOpen(mainwinvm);
        }

        [Test]
        public void NewFileSaveAndCloseFile()
        {
            AssertIsNothingOpen(mainwinvm);
            mainwinvm.NewFileCommand.Execute(null);
            AssertIsNewOpen(mainwinvm);

#warning TODO: need to alter the implementation of saving in order to be able to unit test
            
            mainwinvm.ControllerVM.HasChanged = false;

            AssertIsSavedOpen(mainwinvm);
            mainwinvm.CloseFileCommand.Execute(null);
            AssertIsNothingOpen(mainwinvm);
        }
    }
}
