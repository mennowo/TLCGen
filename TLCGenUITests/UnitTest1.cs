using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TLCGen;
using TestStack.White.Factory;
using System.Windows.Controls;
using NUnit.Framework;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TLCGen.Views;

namespace TLCGenUITests
{
    public class UnitTest1
    {
        public void TestMethod1()
        {

            var assembly = typeof(MainWindow).Assembly;
            var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
            var fileName = uri.AbsolutePath;
            Application app = Application.Launch(fileName);
            
            var window = app.GetWindow("TLCGen", InitializeOption.NoCache);
            var menu = window.MenuBar.MenuItem("File", "New");
            menu.Click();
                        
        }
    }
}
