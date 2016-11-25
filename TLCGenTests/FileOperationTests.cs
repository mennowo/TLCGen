using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.DataAccess;
using TLCGen.Messaging;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGenTests
{
    [TestClass]
    public class FileOperationTests
    {
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

        [TestMethod]
        public void NewFileAndCloseFile()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();
            MainWindowViewModel mwvm = new MainWindowViewModel();
            AssertIsNothingOpen(mwvm);
            mwvm.NewFileCommand.Execute(null);
            AssertIsNewOpen(mwvm);
            mwvm.CloseFileCommand.Execute(null);
            AssertIsNothingOpen(mwvm);
        }

        [TestMethod]
        public void NewFileSaveAndCloseFile()
        {
            // Setup variables
            SettingsProvider.Instance.Settings = new TLCGen.Models.Settings.TLCGenSettingsModel();

            MainWindowViewModel mwvm = new MainWindowViewModel();
            AssertIsNothingOpen(mwvm);
            mwvm.NewFileCommand.Execute(null);
            AssertIsNewOpen(mwvm);

#warning TODO: need to alter the implementation of saving in order to be able to unit test
            DataProvider.Instance.FileName = "test";
            mwvm.ControllerVM.HasChanged = false;

            AssertIsSavedOpen(mwvm);
            mwvm.CloseFileCommand.Execute(null);
            AssertIsNothingOpen(mwvm);
        }
    }
}
