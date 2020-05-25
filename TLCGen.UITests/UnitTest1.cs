using System;
using System.Linq;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;

namespace TLCGen.UITests
{
    public static class Utility
    {
        public static WindowsElement FindElementByAccessibilityIdRetry(this WindowsDriver<WindowsElement> desktopSession, string xPath, int nTryCount = 15)
        {
            WindowsElement uiTarget = null;

            while (nTryCount-- > 0)
            {
                try
                {
                    uiTarget = desktopSession.FindElementByTagName(xPath);
                }
                catch {}

                if (uiTarget != null)
                {
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }

            return uiTarget;
        }
    }

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
                
                // exit
                file.Click();
                exitApplicationMenuItem.Click();
                var msg = WaitForElement(() => window.ModalWindows.FirstOrDefault().AsWindow());
                var yesButton = msg.FindFirstChild(cf => cf.ByName("No")).AsButton();
                yesButton.Invoke();
            }
        }

        [Test]
        public void Testing()
        {
            var app = FlaUI.Core.Application.Launch(@"C:\Users\menno\CodingConnected\Repos\TLCGen\TLCGen\bin\Debug\TLCGen.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                var unkownMenumainMenu = window.FindFirstDescendant(e => e.ByAutomationId("mainMenu"));
                var fileMenuItemfileMenuItem = unkownMenumainMenu.FindFirstDescendant(e => e.ByAutomationId("fileMenuItem"));
                fileMenuItemfileMenuItem.Click();
                // Wait a bit for the animation 
                System.Threading.Thread.Sleep(500);
                var newMenuItemnewFileMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("newFileMenuItem"));
                newMenuItemnewFileMenuItem.Click();
                // Wait a bit for the animation 
                System.Threading.Thread.Sleep(500);
                var unkownCustomcontrollerView = window.FindFirstDescendant(e => e.ByAutomationId("controllerView"));
                var unkownTabcontrollerViewMainTab = unkownCustomcontrollerView.FindFirstDescendant(e => e.ByAutomationId("controllerViewMainTab"));
                var tLCGenViewModelsFasenTabViewModelTabItem_1 = unkownTabcontrollerViewMainTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenTabViewModel"));
                tLCGenViewModelsFasenTabViewModelTabItem_1.Click();
                var unkownCustom_1 = tLCGenViewModelsFasenTabViewModelTabItem_1.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var unkownTabfasenTab = unkownCustom_1.FindFirstDescendant(e => e.ByAutomationId("fasenTab"));
                var tLCGenViewModelsFasenLijstTabViewModelTabItem_1 = unkownTabfasenTab.FindFirstChild(e => e.ByName("TLCGen.ViewModels.FasenLijstTabViewModel"));
                var unkownCustomfasenLijstTab = tLCGenViewModelsFasenLijstTabViewModelTabItem_1.FindFirstDescendant(e => e.ByAutomationId("fasenLijstTab"));
                var unkownCustom_2 = unkownCustomfasenLijstTab.FindFirstChild(e => e.ByControlType(FlaUI.Core.Definitions.ControlType.Custom));
                var fasetoevButton_1 = unkownCustom_2.FindFirstChild(e => e.ByName("Fase toev."));
                fasetoevButton_1.Click();
                fasetoevButton_1.Click();
                fasetoevButton_1.Click();
                fasetoevButton_1.Click();
                fileMenuItemfileMenuItem.Click();
                // Wait a bit for the animation 
                System.Threading.Thread.Sleep(500);
                var exitMenuItemexitMenuItem = fileMenuItemfileMenuItem.FindFirstDescendant(e => e.ByAutomationId("exitMenuItem"));
                exitMenuItemexitMenuItem.Click();
                // Wait a bit for the animation 
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
