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
            CollectAllIO();

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

        private List<CCOLIOElement> AllCCOLOutputElements;
        private List<CCOLIOElement> AllCCOLInputElements;
        private List<IOElementModel> AllOutputModelElements;
        private List<IOElementModel> AllInputModelElements;

        private void CollectAllIO()
        {
            AllCCOLOutputElements = new List<CCOLIOElement>();
            AllCCOLInputElements = new List<CCOLIOElement>();
            AllOutputModelElements = new List<IOElementModel>();
            AllInputModelElements = new List<IOElementModel>();

            foreach (var pgen in PieceGenerators)
            {
                if (pgen.HasCCOLBitmapOutputs())
                {
                    AllCCOLOutputElements.AddRange(pgen.GetCCOLBitmapOutputs());
                }
                if (pgen.HasCCOLBitmapInputs())
                {
                    AllCCOLInputElements.AddRange(pgen.GetCCOLBitmapInputs());
                }
            }

            foreach (var pl in Plugins.TLCGenPluginManager.Default.ApplicationParts.Concat(Plugins.TLCGenPluginManager.Default.ApplicationPlugins))
            {
                if ((pl.Item1 & Plugins.TLCGenPluginElems.IOElementProvider) == Plugins.TLCGenPluginElems.IOElementProvider)
                {
                    AllOutputModelElements.AddRange(((Plugins.ITLCGenElementProvider)pl.Item2).GetOutputItems());
                    AllInputModelElements.AddRange(((Plugins.ITLCGenElementProvider)pl.Item2).GetInputItems());
                }
            }
        }

        private string GenerateDplCIncludes(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* include files */");
            sb.AppendLine("/* ------------- */");
            sb.AppendLine($"{ts}#include \"sysdef.c\"");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}sys.h\"");
            sb.AppendLine($"{ts}#include \"dplwvar.c\"");

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
                        sb.AppendLine($"{ts}#define {fcm.GetDefine()}_{i} (USMAX + {usmaxplus})");
                        ++usmaxplus;
                    }
                }
            }

            foreach (var item in AllCCOLOutputElements)
            {
                if (item.Element.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < item.Element.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{ts}#define {item.Naam}_{i} (USMAX + {usmaxplus})");
                        ++usmaxplus;
                    }
                }
            }

            foreach (var item in AllOutputModelElements)
            {
                if (item.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < item.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{ts}#define {_uspf}{item.Naam}_{i} (USMAX + {usmaxplus})");
                        ++usmaxplus;
                    }
                }
            }

            var fasendets = controller.Fasen.SelectMany(x => x.Detectoren);
            var controllerdets = controller.Detectoren;
            var ovdummydets = controller.OVData.GetAllDummyDetectors();
            var alldets = fasendets.Concat(controllerdets).Concat(ovdummydets);

            foreach (var dm in alldets)
            {
                if (dm.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < dm.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{ts}#define {dm.GetDefine()}_{i} (ISMAX + {ismaxplus})");
                        ++ismaxplus;
                    }
                }
            }

            foreach (var item in AllCCOLInputElements)
            {
                if (item.Element.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < item.Element.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{ts}#define {item.Naam}_{i} (ISMAX + {ismaxplus})");
                        ++ismaxplus;
                    }
                }
            }

            foreach (var item in AllInputModelElements)
            {
                if (item.BitmapCoordinaten?.Count > 1)
                {
                    for (int i = 1; i < item.BitmapCoordinaten.Count; ++i)
                    {
                        sb.AppendLine($"{ts}#define {_ispf}{item.Naam}_{i} (ISMAX + {ismaxplus})");
                        ++ismaxplus;
                    }
                }
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}#define USDPLMAX (USMAX + {usmaxplus})");
            sb.AppendLine($"{ts}#define ISDPLMAX (ISMAX + {ismaxplus})");

            return sb.ToString();
        }

        private string GenerateDplCDisplayBackground(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("void display_background(void)");
            sb.AppendLine("{");
            string bmnaam = controller.Data.BitmapNaam;
            if(bmnaam != null)
            {
                if(!bmnaam.ToLower().EndsWith(".bmp"))
                {
                    bmnaam = bmnaam + ".bmp";
                }
            }
            sb.AppendLine($"{ts}load_picture_bmp(\"{bmnaam}\");");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GetCoordinatesString(IOElementModel item, string itemdefine, string itemtype)
        {
            StringBuilder sb = new StringBuilder();
            if (item.BitmapCoordinaten?.Count > 0)
            {
                sb.Append($"{ts}X_{itemtype}[{itemdefine}] = {item.BitmapCoordinaten[0].X}; ");
                sb.AppendLine($"Y_{itemtype}[{itemdefine}] = {item.BitmapCoordinaten[0].Y};");
            }
            else
            {
                sb.Append($"{ts}X_{itemtype}[{itemdefine}] = NG; ");
                sb.AppendLine($"Y_{itemtype}[{itemdefine}] = NG;");
            }

            if (item.BitmapCoordinaten?.Count > 1)
            {
                for (int i = 1; i < item.BitmapCoordinaten.Count; ++i)
                {
                    sb.Append($"{ts}X_{itemtype}[{itemdefine}_{i}] = {item.BitmapCoordinaten[i].X}; ");
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

            sb.AppendLine($"{ts}/* fasecycli */");
            sb.AppendLine($"{ts}/* --------- */");

            foreach(FaseCyclusModel fcm in controller.Fasen)
            {
                sb.Append(GetCoordinatesString(fcm as IOElementModel, fcm.GetDefine(), "us"));
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}/* detectie */");
            sb.AppendLine($"{ts}/* -------- */");

            var ovdummydets = controller.OVData.GetAllDummyDetectors();
            var alldets = controller.GetAllDetectors().Concat(ovdummydets);

            foreach (var dm in alldets)
            {
                sb.Append(GetCoordinatesString(dm as IOElementModel, dm.GetDefine(), "is"));
            }
            
            sb.AppendLine();

            sb.AppendLine($"{ts}/* overige uitgangen */");
            sb.AppendLine($"{ts}/* ----------------- */");

            foreach (var item in AllCCOLOutputElements)
            {
                sb.Append(GetCoordinatesString(item.Element, item.Naam, "us"));
            }

            foreach (var item in AllOutputModelElements)
            {
                sb.Append(GetCoordinatesString(item, _uspf + item.Naam, "us"));
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}/* overige ingangen */");
            sb.AppendLine($"{ts}/* ---------------- */");
            
            foreach (var item in AllCCOLInputElements)
            {
                sb.Append(GetCoordinatesString(item.Element, item.Naam, "is"));
            }

            foreach (var item in AllInputModelElements)
            {
                sb.Append(GetCoordinatesString(item, _ispf + item.Naam, "is"));
            }

            sb.AppendLine();

            sb.AppendLine($"{ts}/* Gebruikers toevoegingen file includen */");
            sb.AppendLine($"{ts}/* ------------------------------------- */");
            sb.AppendLine($"{ts}#include \"{controller.Data.Naam}dpl.add\"");

            sb.AppendLine();
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
