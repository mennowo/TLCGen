using System;
using CommunityToolkit.Mvvm.Messaging;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TLCGen.DataAccess;
using TLCGen.Helpers;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels;

namespace TLCGen.UnitTests
{
    /// <summary>
    /// Resets the application-wide singletons (messenger, providers, model manager) to fresh fakes
    /// around an empty controller before every test, so tests cannot leak state into each other.
    /// Tests that need specific state still override what they need in the test itself.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ResetGlobalStateAttribute : Attribute, ITestAction
    {
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            var controller = new ControllerModel();

            // a fresh messenger first: view-models and the model manager register on it when constructed
            WeakReferenceMessengerEx.OverrideDefault(new WeakReferenceMessenger());
            SettingsProvider.OverrideDefault(FakesCreator.CreateSettingsProvider());
            DefaultsProvider.OverrideDefault(FakesCreator.CreateDefaultsProvider());
            TLCGenControllerDataProvider.OverrideDefault(FakesCreator.CreateControllerDataProvider(controller));
            TLCGenModelManager.OverrideDefault(new TLCGenModelManager { Controller = controller });
            ControllerAccessProvider.OverrideDefault(FakesCreator.CreateControllerAccessProvider(controller));
        }

        public void AfterTest(ITest test)
        {
        }
    }
}
