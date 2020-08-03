using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.Generators.CCOL.ProjectGeneration
{
    public class CCOLVisualProjectGenerator
    {
		private bool _prevCondition;

        private string HandleFileLine(string line, ITLCGenPlugin plugin, int visualVer)
        {
            var writeline = line;

            var settings = plugin.Controller.Data.CCOLVersie switch
            {
                CCOLVersieEnum.CCOL8 => CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings,
                CCOLVersieEnum.CCOL9 => CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9,
                CCOLVersieEnum.CCOL95 => CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95,
                CCOLVersieEnum.CCOL100 => CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL100,
                CCOLVersieEnum.CCOL110 => CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL110,
                _ => throw new ArgumentOutOfRangeException()
            };

            var ccolinclpaths = settings.CCOLIncludesPaden;
            var ccolextralibs = settings.CCOLLibs;
			var ccollibsnotig = settings.CCOLLibsPathNoTig;
			var ccollibspath = settings.CCOLLibsPath;
            var ccolppdefs = string.Join(";", GetNeededPreprocDefs(plugin.Controller, false, visualVer).OrderBy(x => x));
            var ccolppdefsextra = settings.CCOLPreprocessorDefinitions;
            var ccolrespath = settings.CCOLResPath;
            var ccollibs = string.Join(";", GetNeededCCOLLibraries(plugin.Controller, false, visualVer).OrderBy(x => x));

            // Replace all
            if (writeline.Contains("__"))
            {
                writeline = writeline.Replace("__CONTROLLERNAME__", plugin.Controller.Data.Naam);
                writeline = writeline.Replace("__GUID__", Guid.NewGuid().ToString());
                var actualccollibspath = ccollibspath == null ? "" : ccollibspath.Remove(ccollibspath.Length - 1);
                if(!actualccollibspath.EndsWith("\\"))
                {
                    actualccollibspath += "\\";
                }
                writeline = writeline.Replace("__CCOLLIBSDIR__", actualccollibspath);
                writeline = writeline.Replace("__CCOLLIBSDIRNOTIG__", ccollibsnotig ?? "");
                writeline = writeline.Replace("__CCOLLIBS__", ccollibs);
                writeline = writeline.Replace("__CCOLLIBSEXTRA__", ccolextralibs ?? "");
                
                var actualccolrespath = ccolrespath == null ? "" : ccolrespath.Remove(ccolrespath.Length - 1);
                if (!actualccolrespath.EndsWith("\\"))
                {
                    actualccolrespath += "\\";
                }
                writeline = writeline.Replace("__CCOLLRESDIR__", actualccolrespath);
                writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", ccolinclpaths ?? "");
                writeline = writeline.Replace("__PREPROCESSORDEFS__", ccolppdefs);
                writeline = writeline.Replace("__PREPROCESSORDEFSEXTRA__", ccolppdefsextra);
            }

            // If conditions
            if (!string.IsNullOrWhiteSpace(line))
			{
				var lineif = Regex.IsMatch(line, @"^\s*__IF;.*");
				var lineelif = Regex.IsMatch(line, @"^\s*__ELIF;.*");
				var lineelse = Regex.IsMatch(line, @"^\s*__ELSE__*");

				var result = false;

				if (lineif || lineelif && !_prevCondition)
				{
					#region Conditions
					var conditionsString = Regex.Replace(line, @"^\s*__(IF|ELIF);([A-Z0-9;!]+)__.*", "$2");
					var conditions = conditionsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var condition in conditions)
					{
						var invert = condition.StartsWith("!");
						var actualCondition = condition.Replace("!", "");
                        result = actualCondition switch
                        {
                            "IGT" => (plugin.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && plugin.Controller.Data.Intergroen),
                            "CCOL9ORHIGHER" => (plugin.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL9),
                            "CCOL95ORHIGHER" => (plugin.Controller.Data.CCOLVersie >= CCOLVersieEnum.CCOL95),
                            "PRIO" => (plugin.Controller.PrioData.PrioIngrepen != null && plugin.Controller.PrioData.PrioIngrepen.Any() || plugin.Controller.PrioData.HDIngrepen != null && plugin.Controller.PrioData.HDIngrepen.Any()),
                            "MV" => (plugin.Controller.Data.KWCType != KWCTypeEnum.Geen),
                            "MVVIALIS" => (plugin.Controller.Data.KWCType == KWCTypeEnum.Vialis),
                            "MVOVERIG" => (plugin.Controller.Data.KWCType != KWCTypeEnum.Vialis && plugin.Controller.Data.KWCType != KWCTypeEnum.Geen),
                            "PTP" => (plugin.Controller.PTPData.PTPKoppelingen != null && plugin.Controller.PTPData.PTPKoppelingen.Any()),
                            "KS" => (plugin.Controller.PTPData.PTPKoppelingen != null && plugin.Controller.PTPData.PTPKoppelingen.Any()),
                            "MS" => plugin.Controller.Data.CCOLMulti,
                            "SYNC" => (plugin.Controller.InterSignaalGroep.Gelijkstarten.Any() || plugin.Controller.InterSignaalGroep.Voorstarten.Any()),
                            "HS" => plugin.Controller.HalfstarData.IsHalfstar,
                            "RIS" => (plugin.Controller.RISData.RISToepassen && plugin.Controller.RISData.RISFasen.Any(x => x.LaneData.Any(x2 => x2.SimulatedStations.Any()))),
                            _ => false
                        };
                        if (invert) result = !result;
						if (!result) break;
						#endregion // Conditions
					}
				}
				else if (lineelse && !_prevCondition)
				{
					result = true;
				}
				if ((lineif || lineelif || lineelse) &&
					!result)
				{
					writeline = null;
					if(lineif) _prevCondition = false;
				}
				else if (lineif || lineelif || lineelse)
				{
					writeline = Regex.Replace(writeline, @"^(\s*)__(IF|ELIF|ELSE)[A-Z0-9;!]*__", "$1");
					_prevCondition = true;
				}
			}
			return writeline;
        }

        public static List<string> GetNeededPreprocDefs(ControllerModel c, bool automaat, int visualVer)
        {
            var neededpps = new List<string> {"CCOL_IS_SPECIAL", "_CRT_SECURE_NO_WARNINGS"};

            if (c.Data.CCOLVersie < CCOLVersieEnum.CCOL95) return neededpps;

            neededpps.Add(c.Data.Intergroen ? "CCOLTIG" : "NO_TIGMAX");

            return neededpps;
        }

        public static List<string> GetNeededCCOLLibraries(ControllerModel c, bool automaat, int visualVer)
        {
            // Collect needed libraries
            var neededlibs = new List<string> {"ccolreg.lib", "lwmlfunc.lib", "stdfunc.lib"};
            if (!automaat)
            {
                neededlibs.Add("comctl32.lib");
                neededlibs.Add("ccolsim.lib");
                neededlibs.Add(c.Data.CCOLMulti ? "ccolmainms.lib" : "ccolmain.lib");
                if (c.PTPData.PTPKoppelingen != null &&
                    c.PTPData.PTPKoppelingen.Any())
                {
                    neededlibs.Add("ccolks.lib");
                }
                switch (c.Data.CCOLVersie)
                {
                    case CCOLVersieEnum.CCOL8:
                        break;
                    case CCOLVersieEnum.CCOL9:
                    case CCOLVersieEnum.CCOL95:
                    case CCOLVersieEnum.CCOL100:
                    case CCOLVersieEnum.CCOL110:
                        neededlibs.Add("htmlhelp.lib");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (c.Data.VLOGType != VLOGTypeEnum.Geen)
            {
                neededlibs.Add("ccolvlog.lib");
            }
            if (c.HalfstarData.IsHalfstar)
            {
                neededlibs.Add("plfunc.lib");
                neededlibs.Add("plefunc.lib");
                neededlibs.Add("tx_synch.lib");
                neededlibs.Add(c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "trigfunc.lib" : "tigfunc.lib");
            }
            if (c.HasDSI())
            {
                neededlibs.Add("dsifunc.lib");
            }

            switch (visualVer)
            {
                case 0:
                    break;
                case 2010:
                    break;
                case 2013:
                    break;
                case 2017:
                    neededlibs.Add("legacy_stdio_definitions.lib");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return neededlibs;
        }

        public string GenerateVisualStudioProjectFiles(CCOLCodeGeneratorPlugin plugin, string templateName, int visualVer)
        {
            var sb = new StringBuilder();

            var templatefilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Settings\\VisualTemplates\\{templateName}.xml");
	        var filtersfilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Settings\\VisualTemplates\\{templateName}_filters.xml");
            var outputfilename = Path.Combine(Path.GetDirectoryName(plugin.ControllerFileName) ?? throw new InvalidOperationException(), $"{plugin.Controller.Data.Naam}_{templateName}.vcxproj");
            if (File.Exists(templatefilename))
            {
                var projtemplate = File.ReadAllLines(templatefilename);
                foreach (var line in projtemplate)
                {
                    var writeline = HandleFileLine(line, plugin, visualVer);

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
                var projtemplate = File.ReadAllLines(filtersfilename);
                foreach (var line in projtemplate)
                {
                    var writeline = HandleFileLine(line, plugin, visualVer);

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
