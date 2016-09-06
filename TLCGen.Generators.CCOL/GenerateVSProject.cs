using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {
        protected string GenerateVisualStudioProjectFiles(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            string templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate.xml");
            if(File.Exists(templatefilename))
            {
                string[] projtemplate = File.ReadAllLines(templatefilename);
                foreach(string line in projtemplate)
                {
                    string writeline = line;

                    // Replace all
                    if (writeline.Contains("__"))
                    {
                        writeline = writeline.Replace("__CONTROLLERNAME__", controller.Data.Naam);
                        writeline = writeline.Replace("__GUID__", Guid.NewGuid().ToString());
                        writeline = writeline.Replace("__CCOLLIBSDIR__", CCOLLibsPath.Remove(CCOLLibsPath.Length - 1)); // Remove trailing ;
                        writeline = writeline.Replace("__CCOLLRESDIR__", CCOLResPath.Remove(CCOLResPath.Length - 1));   // Remove trailing ;
                        writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", CCOLIncludesPaden);
                        writeline = writeline.Replace("__PREPROCESSORDEFS__", CCOLPreprocessorDefinitions);
                    }

                    // If conditions
                    if (line.StartsWith("__IF"))
                    {
                        //continue;

                        // Note: this is mostly a placeholder for future functionality
                        string condition = Regex.Replace(line, @"__IF([A-Z]+)__.*", "$1");
                        switch(condition)
                        {
                            case "OV":
                                continue;
                            case "MV":
                                continue;
                            case "PTP":
                                continue;
                            case "SYNC":
                                continue;
                            case "MS":
                                continue;
                            case "NOTMS":
                                writeline = Regex.Replace(writeline, @"__IF[A-Z]+__", "");
                                break;
                            case "KS":
                                continue;
                        }
                    }
                    sb.AppendLine(writeline);
                }
            }
            return sb.ToString();
        }
    }
}
