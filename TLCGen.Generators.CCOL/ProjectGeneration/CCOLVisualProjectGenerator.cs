using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.ProjectGeneration
{
    public class CCOLVisualProjectGenerator
    {
        public string GenerateVisualStudioProjectFiles(CCOLCodeGeneratorPlugin plugin, VisualProjectTypeEnum type)
        {
            StringBuilder sb = new StringBuilder();

            string templatefilename = "";
            switch (type)
            {
                case VisualProjectTypeEnum.Visual2010:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010.xml");
                    break;
                case VisualProjectTypeEnum.Visual2013:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013.xml");
                    break;
                case VisualProjectTypeEnum.Visual2010Vissim:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010vissim.xml");
                    break;
                case VisualProjectTypeEnum.Visual2013Vissim:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013vissim.xml");
                    break;
            }
            if(File.Exists(templatefilename))
            {
                string[] projtemplate = File.ReadAllLines(templatefilename);
                foreach(string line in projtemplate)
                {
                    string writeline = line;

                    // Replace all
                    if (writeline.Contains("__"))
                    {
                        writeline = writeline.Replace("__CONTROLLERNAME__", plugin.Controller.Data.Naam);
                        writeline = writeline.Replace("__GUID__", Guid.NewGuid().ToString());
                        writeline = writeline.Replace("__CCOLLIBSDIR__", plugin.CCOLLibsPath.Remove(plugin.CCOLLibsPath.Length - 1)); // Remove trailing ;
                        writeline = writeline.Replace("__CCOLLRESDIR__", plugin.CCOLResPath.Remove(plugin.CCOLResPath.Length - 1));   // Remove trailing ;
                        writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", plugin.CCOLIncludesPaden);
                        writeline = writeline.Replace("__PREPROCESSORDEFS__", plugin.CCOLPreprocessorDefinitions);
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
