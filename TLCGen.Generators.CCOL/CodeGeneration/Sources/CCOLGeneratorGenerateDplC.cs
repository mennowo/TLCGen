using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateDplC(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* DISPLAY APPLICATIE */");
            sb.AppendLine("/* ------------------ */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "dpl.c"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateDplCExtraDefines(controller));
            sb.AppendLine();
            sb.Append(GenerateDplCIncludes(controller));
            sb.AppendLine();
            sb.Append(GenerateDplCDisplayBackground(controller));
            sb.AppendLine();
            sb.Append(GenerateDplCDisplayParameters(controller));

            return sb.ToString();
        }

        private string GenerateDplCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"{tabspace}#include \"sysdef.c\"");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}sys.h\"");
            sb.AppendLine($"{tabspace}#include \"dplwvar.c\"");

            return sb.ToString();
        }

        private string GenerateDplCExtraDefines(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* aantal ingangs-/uitgangs signalen */");
            sb.AppendLine("/* --------------------------------- */");
            int usmaxplus = 0;
            int ismaxplus = 0;

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < fcm.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{tabspace}#define {fcm.Define}_{i} (USMAX + {usmaxplus})");
                        ++usmaxplus;
                    }
                }
            }

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.BitmapCoordinaten?.Count > 1)
                    {
                        for (int i = 1; i < dm.BitmapCoordinaten.Count; ++i)
                        {
                            sb.AppendLine($"{tabspace}#define {dm.Define}_{i} (ISMAX + {ismaxplus})");
                            ++ismaxplus;
                        }
                    }
                }
            }

            foreach (DetectorModel dm in controller.Detectoren)
            {
                if (dm.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < dm.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{tabspace}#define {dm.Define}_{i} (ISMAX + {ismaxplus})");
                        ++ismaxplus;
                    }
                }
            }

#warning Collecting double IO coordinates should also happen for inputs and outputs

            sb.AppendLine();

            sb.AppendLine($"{tabspace}#define USDPLMAX (USMAX + {usmaxplus})");
            sb.AppendLine($"{tabspace}#define ISDPLMAX (ISMAX + {ismaxplus})");

            return sb.ToString();
        }

        private string GenerateDplCDisplayBackground(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void display_background(void)");
            sb.AppendLine("{");
            sb.AppendLine($"{tabspace}load_picture_bmp(\"{controller.Data.Naam}.bmp\");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateDplCDisplayParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void display_parameters(void)");
            sb.AppendLine("{");

            sb.AppendLine($"{tabspace}/* fasecycli */");
            sb.AppendLine($"{tabspace}/* --------- */");

            foreach(FaseCyclusModel fcm in controller.Fasen)
            {
                if(fcm.BitmapCoordinaten?.Count > 0)
                {
                    sb.Append($"{tabspace}X_us[{fcm.Define}] = {fcm.BitmapCoordinaten[0].X}; ");
                    sb.AppendLine($"Y_us[{fcm.Define}] = {fcm.BitmapCoordinaten[0].Y};");
                }
                else
                {
                    sb.Append($"{tabspace}X_us[{fcm.Define}] = NG; ");
                    sb.AppendLine($"Y_us[{fcm.Define}] = NG;");
                }
                if (fcm.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < fcm.BitmapCoordinaten.Count; ++i)
                    {
                        sb.Append($"{tabspace}X_us[{fcm.Define}_{i}] = {fcm.BitmapCoordinaten[i].X}; ");
                        sb.Append($"Y_us[{fcm.Define}_{i}] = {fcm.BitmapCoordinaten[i].Y}; ");
                        sb.AppendLine($"NR_us[{fcm.Define}_{i}] = {fcm.Define};");
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine($"{tabspace}/* detectie */");
            sb.AppendLine($"{tabspace}/* -------- */");

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    if (dm.BitmapCoordinaten?.Count > 0)
                    {
                        sb.Append($"{tabspace}X_is[{dm.Define}] = {dm.BitmapCoordinaten[0].X}; ");
                        sb.AppendLine($"Y_is[{dm.Define}] = {dm.BitmapCoordinaten[0].Y};");
                    }
                    else
                    {
                        sb.Append($"{tabspace}X_is[{dm.Define}] = NG; ");
                        sb.AppendLine($"Y_is[{dm.Define}] = NG;");
                    }
                    if (dm.BitmapCoordinaten?.Count > 1)
                    {
                        for (int i = 1; i < dm.BitmapCoordinaten.Count; ++i)
                        {
                            sb.Append($"{tabspace}X_is[{dm.Define}_{i}] = {dm.BitmapCoordinaten[i].X}; ");
                            sb.Append($"Y_is[{dm.Define}_{i}] = {dm.BitmapCoordinaten[i].Y}; ");
                            sb.AppendLine($"NR_is[{dm.Define}_{i}] = {dm.Define};");
                        }
                    }
                }
            }
            foreach (DetectorModel dm in controller.Detectoren)
            {
                if (dm.BitmapCoordinaten?.Count > 0)
                {
                    sb.Append($"{tabspace}X_is[{dm.Define}] = {dm.BitmapCoordinaten[0].X}; ");
                    sb.AppendLine($"Y_is[{dm.Define}] = {dm.BitmapCoordinaten[0].Y};");
                }
                else
                {
                    sb.Append($"{tabspace}X_is[{dm.Define}] = NG; ");
                    sb.AppendLine($"Y_is[{dm.Define}] = NG;");
                }
                if (dm.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < dm.BitmapCoordinaten.Count; ++i)
                    {
                        sb.Append($"{tabspace}X_is[{dm.Define}_{i}] = {dm.BitmapCoordinaten[i].X}; ");
                        sb.Append($"Y_is[{dm.Define}_{i}] = {dm.BitmapCoordinaten[i].Y}; ");
                        sb.AppendLine($"NR_is[{dm.Define}_{i}] = {dm.Define};");
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine($"{tabspace}/* Gebruikers toevoegingen file includen */");
            sb.AppendLine($"{tabspace}/* ------------------------------------- */");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}dpl.add\"");

            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
