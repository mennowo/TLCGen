using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TLCGen.Models;

namespace TLCGen.Specificator.Tester
{
    class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;

        static void Main(string[] args)
        {
            IntPtr windowPtr = FindWindowByCaption(IntPtr.Zero, "SP16.docx - Word");
            if (windowPtr == IntPtr.Zero)
            {
                Console.WriteLine("Window not found");
            }

            SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            var sp = new SpecificatorPlugin()
            {
                Controller = Helpers.TLCGenSerialization.DeSerialize<ControllerModel>(@"C:\Users\menno\Documents\temp\SP16\SP16.tlc")
            };
            
            DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName =
                sp.ControllerFileName = @"C:\Users\menno\Documents\temp\SP16\SP16.tlc";

            sp.SpecificatorVM.Data = new SpecificatorDataModel()
            {
                EMail = "testEmail",
                Organisatie = "testOrg",
                Postcode = "12345",
                Stad = "New York",
                Straat = "Broadway",
                TelefoonNummer = "0123456789",
                Website = "www.www.www"
            };
            sp.SpecificatorVM.GenerateSpecification();

            var p = new Process();
            p.StartInfo.FileName = @"C:\Users\menno\Documents\temp\SP16\SP16.docx";
            p.Start();
        }
    }
}
