using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.ProjectGeneration
{
    public class CCOLVisualProjectGenerator
    {
        private string HandleFileLine(string line, CCOLCodeGeneratorPlugin plugin)
        {
            string writeline = line;

            string _ccolinclpaths = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLIncludesPaden;
            string _ccollibs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibs;
            string _ccollibspath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLLibsPath;
            string _ccolppdefs = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLPreprocessorDefinitions;
            string _ccolrespath = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings.CCOLResPath;

            // Replace all
            if (writeline.Contains("__"))
            {
                string prepro = _ccolppdefs;
                if (string.IsNullOrEmpty(prepro))
                    prepro = "";
                writeline = writeline.Replace("__CONTROLLERNAME__", plugin.Controller.Data.Naam);
                writeline = writeline.Replace("__GUID__", Guid.NewGuid().ToString());
                string ccollibspath = _ccollibspath.Remove(_ccollibspath.Length - 1);
                if(!ccollibspath.EndsWith("\\"))
                {
                    ccollibspath = ccollibspath + "\\";
                }
                writeline = writeline.Replace("__CCOLLIBSDIR__", ccollibspath);
                writeline = writeline.Replace("__CCOLLLIBS__", _ccollibs);
                string ccolrespath = _ccolrespath.Remove(_ccolrespath.Length - 1);
                if (!ccolrespath.EndsWith("\\"))
                {
                    ccolrespath = ccolrespath + "\\";
                }
                writeline = writeline.Replace("__CCOLLRESDIR__", ccolrespath);
                writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", _ccolinclpaths);
                writeline = writeline.Replace("__PREPROCESSORDEFS__", prepro);
            }

            // If conditions
            if (!string.IsNullOrWhiteSpace(line) && Regex.IsMatch(line, @"^\s*__IF.*"))
            {
                string condition = Regex.Replace(line, @"^\s*__IF([A-Z]+)__.*", "$1");
                if (!string.IsNullOrWhiteSpace(condition))
                {
                    switch (condition)
                    {
                        case "OV":
                            if(!(plugin.Controller.OVData.OVIngrepen != null &&
                                 plugin.Controller.OVData.OVIngrepen.Count > 0 || 
                                 plugin.Controller.OVData.HDIngrepen != null &&
                                 plugin.Controller.OVData.HDIngrepen.Count > 0))
                                return null;
                            break;
                        case "MV":
                            return null;
                        case "PTP":
                            if (!(plugin.Controller.PTPData.PTPKoppelingen != null &&
                                  plugin.Controller.PTPData.PTPKoppelingen.Count > 0))
                                return null;
                            break;
                        case "SYNC":
#warning TODO
                            //if (!model.Syncfunc)
                            return null;
                            //break;
                        case "MS":
#warning TODO
                            //if (!model.MultiSignal)
                                return null;
                            //break;
                        case "NOTMS":
                        //    if (model.MultiSignal)
                        //        return null;
                            break;
                        case "KS":
                            return null;
                    }
                }
                writeline = Regex.Replace(writeline, @"^(\s*)__IF[A-Z]+__", "$1");
            }
            return writeline;
        }

        public string GenerateVisualStudioProjectFiles(CCOLCodeGeneratorPlugin plugin, VisualProjectTypeEnum type)
        {
            StringBuilder sb = new StringBuilder();

            string templatefilename = "";
            string filtersfilename = "";
            string outputfilename = "";
            switch (type)
            {
                case VisualProjectTypeEnum.Visual2010:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010.xml");
                    filtersfilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010filters.xml");
                    outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName), $"{plugin.Controller.Data.Naam}_msvc2010.vcxproj");
                    break;
                case VisualProjectTypeEnum.Visual2013:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013.xml");
                    filtersfilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013filters.xml");
                    outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName), $"{plugin.Controller.Data.Naam}_msvc2013.vcxproj"); break;
                case VisualProjectTypeEnum.Visual2010Vissim:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010vissim.xml");
                    filtersfilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2010vissimfilters.xml");
                    outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName), $"{plugin.Controller.Data.Naam}_vissim_msvc2010.vcxproj");
                    break;
                case VisualProjectTypeEnum.Visual2013Vissim:
                    templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013vissim.xml");
                    filtersfilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Resources\\visualprojecttemplate2013vissimfilters.xml");
                    outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName), $"{plugin.Controller.Data.Naam}_vissim_msvc2013.vcxproj");
                    break;
            }
            if (File.Exists(templatefilename))
            {
                string[] projtemplate = File.ReadAllLines(templatefilename);
                foreach (string line in projtemplate)
                {
                    string writeline = HandleFileLine(line, plugin);


                    if (!string.IsNullOrWhiteSpace(writeline))
                        sb.AppendLine(writeline);
                }

                try
                {
                    File.WriteAllText(outputfilename, sb.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error writing Visual Studio project files:\n" + e.ToString());
                }
            }

            sb.Clear();

            if (File.Exists(filtersfilename))
            {
                string[] projtemplate = File.ReadAllLines(filtersfilename);
                foreach (string line in projtemplate)
                {
                    string writeline = HandleFileLine(line, plugin);

                    if (!string.IsNullOrWhiteSpace(writeline))
                        sb.AppendLine(writeline);
                }

                try
                {
                    File.WriteAllText(outputfilename + ".filters", sb.ToString());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error writing Visual Studio project files:\n" + e.ToString());
                }
            }

            return "Finished genrating Visual Studio project files.";
        }
    }
}
