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
                        writeline = writeline.Replace("__CCOLLIBSDIR__", CCOLLibsPath);
                        writeline = writeline.Replace("__CCOLLRESDIR__", CCOLResPath);
                        writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", CCOLIncludesPad);
                        writeline = writeline.Replace("__PREPROCESSORDEFS__", CCOLPreprocessorDefinitions);
                        writeline = writeline.Replace("__MVOBJECTSFOLDER__", TestSetting);
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
