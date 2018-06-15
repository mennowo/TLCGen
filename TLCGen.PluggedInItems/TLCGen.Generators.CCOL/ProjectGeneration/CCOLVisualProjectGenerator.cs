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
		private bool _prevCondition = false;

        private string HandleFileLine(string line, CCOLCodeGeneratorPlugin plugin)
        {
            string writeline = line;

            CCOLGeneratorVisualSettingsModel settings = null;
            switch (plugin.Controller.Data.CCOLVersie)
            {
                case Models.Enumerations.CCOLVersieEnum.CCOL8:
                    settings = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettings;
                    break;
                case Models.Enumerations.CCOLVersieEnum.CCOL9:
                    settings = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL9;
                    break;
                case Models.Enumerations.CCOLVersieEnum.CCOL95:
                    settings = CCOLGeneratorSettingsProvider.Default.Settings.VisualSettingsCCOL95;
                    break;
            }

            var _ccolinclpaths = settings.CCOLIncludesPaden;
            var _ccollibs = settings.CCOLLibs;
			var _ccollibsnotig = settings.CCOLLibsPathNoTig;
			var _ccollibspath = settings.CCOLLibsPath;
            var _ccolppdefs = settings.CCOLPreprocessorDefinitions;
            var _ccolrespath = settings.CCOLResPath;

            // Replace all
            if (writeline.Contains("__"))
            {
                string prepro = _ccolppdefs;
                if (string.IsNullOrEmpty(prepro))
                    prepro = "";
                writeline = writeline.Replace("__CONTROLLERNAME__", plugin.Controller.Data.Naam);
                writeline = writeline.Replace("__GUID__", Guid.NewGuid().ToString());
                string ccollibspath = _ccollibspath == null ? "" : _ccollibspath.Remove(_ccollibspath == null ? 0 : _ccollibspath.Length - 1);
                if(!ccollibspath.EndsWith("\\"))
                {
                    ccollibspath = ccollibspath + "\\";
                }
                writeline = writeline.Replace("__CCOLLIBSDIR__", ccollibspath == null ? "" : ccollibspath);
                writeline = writeline.Replace("__CCOLLLIBS__", _ccollibs == null ? "" : _ccollibs);
                writeline = writeline.Replace("__CCOLLLIBSNOTIG__", _ccollibsnotig == null ? "" : _ccollibsnotig);
                string ccolrespath = _ccolrespath == null ? "" : _ccolrespath.Remove(_ccolrespath == null ? 0 : _ccolrespath.Length - 1);
                if (!ccolrespath.EndsWith("\\"))
                {
                    ccolrespath = ccolrespath + "\\";
                }
                writeline = writeline.Replace("__CCOLLRESDIR__", ccolrespath == null ? "" : ccolrespath);
                writeline = writeline.Replace("__ADDITIONALINCLUDEDIRS__", _ccolinclpaths == null ? "" : _ccolinclpaths);
                writeline = writeline.Replace("__PREPROCESSORDEFS__", prepro == null ? "" : prepro);
            }

            // If conditions
            if (!string.IsNullOrWhiteSpace(line))
			{
				var lineif = Regex.IsMatch(line, @"^\s*__IF;.*");
				var lineelif = Regex.IsMatch(line, @"^\s*__ELIF;.*");
				var lineelse = Regex.IsMatch(line, @"^\s*__ELSE__*");

				bool result = false;

				if (lineif || lineelif && !_prevCondition)
				{
					#region Conditions
					var conditionsString = Regex.Replace(line, @"^\s*__(IF|ELIF);([A-Z0-9;!]+)__.*", "$2");
					var conditions = conditionsString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var condition in conditions)
					{
						var invert = condition.StartsWith("!");
						var actualCondition = condition.Replace("!", "");
						switch (actualCondition)
						{
							case "IGT":
								result = plugin.Controller.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL95 &&
										 plugin.Controller.Data.Intergroen;
								break;
							case "CCOL9ORHIGHER":
								result = plugin.Controller.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL9;
								break;
							case "CCOL95ORHIGHER":
								result = plugin.Controller.Data.CCOLVersie >= Models.Enumerations.CCOLVersieEnum.CCOL95;
								break;
							case "OV":
								result = plugin.Controller.OVData.OVIngrepen != null &&
										 plugin.Controller.OVData.OVIngrepen.Any() ||
										 plugin.Controller.OVData.HDIngrepen != null &&
										 plugin.Controller.OVData.HDIngrepen.Any();
								break;
							case "MV":
								result = plugin.Controller.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen;
								break;
							case "MVVIALIS":
								result = plugin.Controller.Data.KWCType == Models.Enumerations.KWCTypeEnum.Vialis;
								break;
							case "MVOVERIG":
								result = plugin.Controller.Data.KWCType != Models.Enumerations.KWCTypeEnum.Vialis &&
										 plugin.Controller.Data.KWCType != Models.Enumerations.KWCTypeEnum.Geen;
								break;
							case "PTP":
							case "KS":
								result = plugin.Controller.PTPData.PTPKoppelingen != null &&
										 plugin.Controller.PTPData.PTPKoppelingen.Any();
								break;
							case "MS":
								result = plugin.Controller.Data.CCOLMulti;
								break;
							case "SYNC":
								result = plugin.Controller.InterSignaalGroep.Gelijkstarten.Any() ||
										 plugin.Controller.InterSignaalGroep.Voorstarten.Any();
								break;
							case "HS":
								result = plugin.Controller.HalfstarData.IsHalfstar;
								break;
							default:
								result = false;
								break;
						}
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
