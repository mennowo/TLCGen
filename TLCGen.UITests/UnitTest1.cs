using System;
using System.Linq;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;

namespace TLCGen.UITests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void StartTLCGen_NewFile_CloseSuccesfully()
        {
            var app = FlaUI.Core.Application.Launch(@"C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Debug\TLCGen.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                Console.WriteLine(window.Title);
                var mainMenu = window.FindFirstChild("mainMenu").AsMenu();
                var file = mainMenu.Items["File"];
                file.Click();
                var newFile = file.Items["New"];
                var exitApplicationMenuItem = file.FindFirstChild("exitMenuItem");
                newFile.Click();
                
                Thread.Sleep(500);

                // exit
                file.Click();
                exitApplicationMenuItem.Click();
            }
        }

        [Test]
        public void StartTLCGenWithNewController_Add4SignalGroups_CorrectNumberAdded()
        {
            var app = FlaUI.Core.Application.Launch(@"C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Debug\TLCGen.exe");
            using (var automation = new UIA3Automation())
            {
                // start
                var window = app.GetMainWindow(automation);
                var windowMainMenu = window.FindFirstDescendant(e => e.ByAutomationId("mainMenu"));

                // new file
                var fileMenuItemfileMenuItem = windowMainMenu.FindFirstDescendant(e => e.ByAutomationId("fileMenuItem"));
                fileMenuItemfileMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var newMenuItemnewFileMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("newFileMenuItem"));
                newMenuItemnewFileMenuItem.Click();
                System.Threading.Thread.Sleep(500);

                // controller view & tabs
                var controllerView = window.FindFirstDescendant(e => e.ByAutomationId("controllerView"));
                var controllerViewMainTab = controllerView.FindFirstDescendant(e => e.ByAutomationId("controllerViewMainTab"));
                var controllerViewMainTabFasenButton = controllerViewMainTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenTabViewModel"));
                controllerViewMainTabFasenButton.Click();
                var fasenTabContentTemplate = controllerViewMainTabFasenButton.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var fasenTab = fasenTabContentTemplate.FindFirstDescendant(e => e.ByAutomationId("fasenTab"));
                var fasenTabLijstTabButton = fasenTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenLijstTabViewModel"));
                var fasenTabLijstTab = fasenTabLijstTabButton.FindFirstDescendant(e => e.ByAutomationId("fasenLijstTab"));
                var fasenTabLijstTabAddRemoveControl = fasenTabLijstTab.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var fasenTabLijstTabAddRemoveControlAddButton = fasenTabLijstTabAddRemoveControl.FindFirstChild(e => e.ByName("Fase toev."));

                // action!
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                
                // assert
                var fasenLijstDataGrid = fasenTabLijstTab.FindFirstChild(e => e.ByAutomationId("fasenLijstDataGrid")).AsDataGridView();
                Assert.AreEqual(fasenLijstDataGrid.Rows.Length, 4);
                
                // close
                fileMenuItemfileMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var exitMenuItemexitMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("exitMenuItem"));
                exitMenuItemexitMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var deregelingisgewijzigdOpslaanWindow_1 = window.FindFirstChild(e => e.ByName("De regeling is gewijzigd. Opslaan?"));
                var noButton7 = deregelingisgewijzigdOpslaanWindow_1.FindFirstDescendant(e => e.ByAutomationId("7"));
                noButton7.Click();
            }
        }

        
        [Test]
        public void StartTLCGenWithNewController_Add4SignalGroupsThenRemove1_CorrectNumberRemains()
        {
            var app = FlaUI.Core.Application.Launch(@"C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Debug\TLCGen.exe");
            using (var automation = new UIA3Automation())
            {
                // start
                var window = app.GetMainWindow(automation);
                var windowMainMenu = window.FindFirstDescendant(e => e.ByAutomationId("mainMenu"));

                // new file
                var fileMenuItemfileMenuItem = windowMainMenu.FindFirstDescendant(e => e.ByAutomationId("fileMenuItem"));
                fileMenuItemfileMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var newMenuItemnewFileMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("newFileMenuItem"));
                newMenuItemnewFileMenuItem.Click();
                System.Threading.Thread.Sleep(500);

                // controller view & tabs
                var controllerView = window.FindFirstDescendant(e => e.ByAutomationId("controllerView"));
                var controllerViewMainTab = controllerView.FindFirstDescendant(e => e.ByAutomationId("controllerViewMainTab"));
                var controllerViewMainTabFasenButton = controllerViewMainTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenTabViewModel"));
                controllerViewMainTabFasenButton.Click();
                var fasenTabContentTemplate = controllerViewMainTabFasenButton.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var fasenTab = fasenTabContentTemplate.FindFirstDescendant(e => e.ByAutomationId("fasenTab"));
                var fasenTabLijstTabButton = fasenTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenLijstTabViewModel"));
                var fasenTabLijstTab = fasenTabLijstTabButton.FindFirstDescendant(e => e.ByAutomationId("fasenLijstTab"));
                var fasenTabLijstTabAddRemoveControl = fasenTabLijstTab.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var fasenTabLijstTabAddRemoveControlAddButton = fasenTabLijstTabAddRemoveControl.FindFirstChild(e => e.ByName("Fase toev."));
                var fasenTabLijstTabAddRemoveControlRemoveButton = fasenTabLijstTabAddRemoveControl.FindFirstChild(e => e.ByName("Fase verw."));
                var fasenLijstDataGrid = fasenTabLijstTab.FindFirstChild(e => e.ByAutomationId("fasenLijstDataGrid")).AsDataGridView();

                // action!
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenTabLijstTabAddRemoveControlAddButton.Click();
                fasenLijstDataGrid.Rows[0].Click();
                fasenTabLijstTabAddRemoveControlRemoveButton.Click();
                
                // assert
                Assert.AreEqual(fasenLijstDataGrid.Rows.Length, 3);
                
                // close
                fileMenuItemfileMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var exitMenuItemexitMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("exitMenuItem"));
                exitMenuItemexitMenuItem.Click();
                System.Threading.Thread.Sleep(500);
                var deregelingisgewijzigdOpslaanWindow_1 = window.FindFirstChild(e => e.ByName("De regeling is gewijzigd. Opslaan?"));
                var noButton7 = deregelingisgewijzigdOpslaanWindow_1.FindFirstDescendant(e => e.ByAutomationId("7"));
                noButton7.Click();
            }
        }

        // Where WaitForElement is defined as 
        public T WaitForElement<T>(Func<T> getter)
        {
            var retry = Retry.WhileNull<T>(
                getter,
                TimeSpan.FromMilliseconds(5000));

            if (!retry.Success)
            {
                Assert.Fail($"Failed to get an element within a {5000}ms");
            }

            return retry.Result;
        }
    }
}
