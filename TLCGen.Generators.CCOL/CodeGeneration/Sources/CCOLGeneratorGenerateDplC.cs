using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
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
                        sb.AppendLine($"{tabspace}#define {fcm.GetDefine()}_{i} (USMAX + {usmaxplus})");
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
                            sb.AppendLine($"{tabspace}#define {dm.GetDefine()}_{i} (ISMAX + {ismaxplus})");
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
                        sb.AppendLine($"{tabspace}#define {dm.GetDefine()}_{i} (ISMAX + {ismaxplus})");
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
            string bmnaam = controller.Data.BitmapNaam;
            if(!bmnaam.ToLower().EndsWith(".bmp"))
            {
                bmnaam = bmnaam + ".bmp";
            }
            sb.AppendLine($"{tabspace}load_picture_bmp(\"{bmnaam}\");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GetCoordinatesString(IOElementModel item, string itemdefine, string itemtype)
        {
            StringBuilder sb = new StringBuilder();
            if (item.BitmapCoordinaten?.Count > 0)
            {
                sb.Append($"{tabspace}X_{itemtype}[{itemdefine}] = {item.BitmapCoordinaten[0].X}; ");
                sb.AppendLine($"Y_{itemtype}[{itemdefine}] = {item.BitmapCoordinaten[0].Y};");
            }
            else
            {
                sb.Append($"{tabspace}X_{itemtype}[{itemdefine}] = NG; ");
                sb.AppendLine($"Y_{itemtype}[{itemdefine}] = NG;");
            }

            if (item.BitmapCoordinaten?.Count > 1)
            {
                for (int i = 1; i < item.BitmapCoordinaten.Count; ++i)
                {
                    sb.Append($"{tabspace}X_{itemtype}[{itemdefine}_{i}] = {item.BitmapCoordinaten[i].X}; ");
                    sb.Append($"Y_{itemtype}[{itemdefine}_{i}] = {item.BitmapCoordinaten[i].Y}; ");
                    sb.AppendLine($"NR_{itemtype}[{itemdefine}_{i}] = {itemdefine};");
                }
            }
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
                sb.Append(GetCoordinatesString(fcm as IOElementModel, fcm.GetDefine(), "us"));
            }

            sb.AppendLine();

            sb.AppendLine($"{tabspace}/* detectie */");
            sb.AppendLine($"{tabspace}/* -------- */");

            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    sb.Append(GetCoordinatesString(dm as IOElementModel, dm.GetDefine(), "is"));
                }
            }
            foreach (DetectorModel dm in controller.Detectoren)
            {
                sb.Append(GetCoordinatesString(dm as IOElementModel, dm.GetDefine(), "is"));
            }

            sb.AppendLine($"{tabspace}/* overige uitgangen */");
            sb.AppendLine($"{tabspace}/* ----------------- */");

            // Segment display
            foreach (var item in controller.Data.SegmentenDisplayBitmapData)
            {
                sb.Append(GetCoordinatesString(item as IOElementModel, item.GetBitmapCoordinaatOutputDefine(), "us"));
            }

            foreach(var pgen in _PieceGenerators)
            {
                foreach(var item in pgen.GetCCOLBitmapOutputs())
                {
                    sb.Append(GetCoordinatesString(item.Element, item.Naam, "us"));
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
