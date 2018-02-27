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
                        case "MS":
                        case "KS":
                            if (!(plugin.Controller.PTPData.PTPKoppelingen != null &&
                                  plugin.Controller.PTPData.PTPKoppelingen.Count > 0))
                                return null;
                            break;
                        case "NOTMS":
                            if ((plugin.Controller.PTPData.PTPKoppelingen != null &&
                                 plugin.Controller.PTPData.PTPKoppelingen.Count > 0))
                                return null;
                            break;
                        case "SYNC":
                            if (!plugin.Controller.InterSignaalGroep.Gelijkstarten.Any() &&
                                !plugin.Controller.InterSignaalGroep.Voorstarten.Any())
                                return null;
                            break;
                        case "HS":
                            if (!plugin.Controller.HalfstarData.IsHalfstar)
                                return null;
                            break;
                        case "NOTHS":
                            if (plugin.Controller.HalfstarData.IsHalfstar)
                                return null;
                            break;
                    }
                }
                writeline = Regex.Replace(writeline, @"^(\s*)__IF[A-Z]+__", "$1");
            }
            return writeline;
        }

        public string GenerateVisualStudioProjectFiles(CCOLCodeGeneratorPlugin plugin, string templateName)
        {
            StringBuilder sb = new StringBuilder();

            string templatefilename = "";
            string filtersfilename = "";
            string outputfilename = "";

			templatefilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"Settings\\VisualTemplates\\{templateName}.xml");
	        filtersfilename = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"Settings\\VisualTemplates\\{templateName}_filters.xml");
            outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName) ?? throw new InvalidOperationException(), $"{plugin.Controller.Data.Naam}_{templateName}.vcxproj");
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
