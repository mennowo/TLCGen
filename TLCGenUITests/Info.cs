using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TLCGen;

namespace TLCGenUITests
{
    public static class Info
    {
        public static ProcessStartInfo ProcessStartInfo
        {
            get
            {
                var assembly = typeof(MainWindow).Assembly;
                var uri = new Uri(assembly.CodeBase, UriKind.Absolute);
                var fileName = uri.AbsolutePath;
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                return processStartInfo;
            }
        }
    }
}