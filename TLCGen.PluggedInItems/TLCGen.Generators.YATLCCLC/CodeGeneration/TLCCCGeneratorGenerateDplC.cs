using System.Linq;
using System.Text;
using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration
{
    public partial class TLCCCGenerator
    {
        private string GenerateDplC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* DISPLAY SETTINGS */");
            sb.AppendLine("/* ---------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "tab.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateDplCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateDplCDisplayParameters(controller));

            return sb.ToString();
        }

        private string GenerateDplCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"/* include files */");
            sb.AppendLine($"/* ------------- */");
            sb.AppendLine($"#include \"{controller.Data.Naam}sys.h\"");
            
            return sb.ToString();
        }

        private string GenerateDplCDisplayParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void DisplayParameters(void)");
            sb.AppendLine("{");
            foreach (var sg in controller.Fasen)
            {
                if (sg.BitmapCoordinaten != null && sg.BitmapCoordinaten.Count > 0 &&
                    sg.BitmapCoordinaten[0].X > 0 && sg.BitmapCoordinaten[0].Y > 0)
                {
                    sb.AppendLine($"{ts}X_us[{_sgpf}{sg.Naam}] = {sg.BitmapCoordinaten[0].X}; Y_us[{_sgpf}{sg.Naam}] = {sg.BitmapCoordinaten[0].Y};");
                }
            }
            sb.AppendLine();

            foreach (var item in controller.Data.SegmentenDisplayBitmapData)
            {
                if (item.BitmapData?.BitmapCoordinaten != null && item.BitmapData.BitmapCoordinaten.Count > 0 &&
                    item.BitmapData.BitmapCoordinaten[0].X > 0 && item.BitmapData.BitmapCoordinaten[0].Y > 0)
                {
                    sb.Append(
                        $"{ts}X_us[{_ospf}{item.Naam}+SGMAX] = {item.BitmapData.BitmapCoordinaten[0].X}; Y_us[{_ospf}{item.Naam}+SGMAX] = {item.BitmapData.BitmapCoordinaten[0].Y};");
                }
            }
            foreach (var item in controller.Data.ModulenDisplayBitmapData)
            {
                if (item.BitmapData?.BitmapCoordinaten != null && item.BitmapData.BitmapCoordinaten.Count > 0 &&
                    item.BitmapData.BitmapCoordinaten[0].X > 0 && item.BitmapData.BitmapCoordinaten[0].Y > 0)
                {
                    sb.Append(
                        $"{ts}X_us[{_ospf}{item.Naam}+SGMAX] = {item.BitmapData.BitmapCoordinaten[0].X}; Y_us[{_ospf}{item.Naam}+SGMAX] = {item.BitmapData.BitmapCoordinaten[0].Y};");
                }
            }
            sb.AppendLine();

            foreach (var d in controller.Fasen.SelectMany(x => x.Detectoren))
            {
                if (d.BitmapCoordinaten != null && d.BitmapCoordinaten.Count > 0 &&
                    d.BitmapCoordinaten[0].X > 0 && d.BitmapCoordinaten[0].Y > 0)
                {
                    sb.AppendLine($"{ts}X_is[{_sgpf}{d.Naam}] = {d.BitmapCoordinaten[0].X}; Y_is[{_sgpf}{d.Naam}] = {d.BitmapCoordinaten[0].Y};");
                }
            }
            sb.AppendLine();

            sb.AppendLine("}");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
