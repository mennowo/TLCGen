using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL
{
    public partial class CCOLCodeGenerator
    {
        private string GenerateSysH(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ALGEMENE APPLICATIEFILE */");
            sb.AppendLine("/* ----------------------- */");
            sb.AppendLine();
            sb.Append(GenerateFileHeader(controller.Data, "sys.h"));
            sb.AppendLine();
            sb.Append(GenerateVersionHeader(controller.Data));
            sb.AppendLine();
            sb.Append(GenerateSysHFasen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHUitgangen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHDetectors(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHIngangen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHHulpElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHGeheugenElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHTijdElementen(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHCounters(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHSchakelaars(controller));
            sb.AppendLine();
            sb.Append(GenerateSysHParameters(controller));
            sb.AppendLine();
            sb.AppendLine("/* modulen */");
            sb.AppendLine("/* ------- */");
            sb.AppendLine($"{tabspace}#define MLMAX1 {controller.ModuleMolen.Modules.Count} /* aantal modulen                        */");
            sb.AppendLine();
            sb.AppendLine("/* Gebruikers toevoegingen file includen */");
            sb.AppendLine("/* ------------------------------------- */");
            sb.AppendLine($"{tabspace}#include \"{controller.Data.Naam}sys.add\"");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GenerateSysHFasen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* fasecycli */");
            sb.AppendLine("/* --------- */");

            int pad1 = "FCMAX".Length;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                if (fcm.Define.Length > pad1) pad1 = fcm.Define.Length;
            }
            pad1 = pad1 + $"{tabspace}#define  ".Length;

            int pad2 = controller.Fasen.Count.ToString().Length;

            int index = 0;
            foreach (FaseCyclusModel fcm in controller.Fasen)
            {
                sb.Append($"{tabspace}#define {fcm.Define} ".PadRight(pad1));
                sb.AppendLine($"{index.ToString()}".PadLeft(pad2));
                ++index;
            }
            sb.Append($"{tabspace}#define FCMAX ".PadRight(pad1));
            sb.Append($"{index.ToString()} ".PadLeft(pad2));
            sb.AppendLine("/* aantal fasecycli */");

            return sb.ToString();
        }

        private string GenerateSysHUitgangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige uitgangen */");
            sb.AppendLine("/* ----------------- */");

            int pad1 = Uitgangen.DefineMaxWidth + $"{tabspace}#define  ".Length;
            int pad2 = Uitgangen.Elements.Count.ToString().Length;

            int index = 0;
            foreach (CCOLElement elem in Uitgangen.Elements)
            {
                sb.Append($"{tabspace}#define {elem.Define} ".PadRight(pad1));
                sb.Append($"(FCMAX + ");
                sb.Append($"{index.ToString()}".PadLeft(pad2));
                sb.AppendLine($")");
                ++index;
            }
            sb.Append($"{tabspace}#define USMAX".PadRight(pad1));
            sb.Append($"(FCMAX + ");
            sb.Append($"{index.ToString()}".PadLeft(pad2));
            sb.AppendLine($") /* aantal overige uitgangen */");

            return sb.ToString();
        }

        private string GenerateSysHDetectors(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* detectie */");
            sb.AppendLine("/* -------- */");

            return sb.ToString();
        }

        private string GenerateSysHIngangen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* overige ingangen */");
            sb.AppendLine("/* ---------------- */");

            int pad1 = Ingangen.DefineMaxWidth + $"{tabspace}#define  ".Length;
            int pad2 = Ingangen.Elements.Count.ToString().Length;

            int index = 0;
            foreach (CCOLElement elem in Ingangen.Elements)
            {
                sb.Append($"{tabspace}#define {elem.Define} ".PadRight(pad1));
                sb.Append($"(DPMAX + ");
                sb.Append($"{index.ToString()}".PadLeft(pad2));
                sb.AppendLine($")");
                ++index;
            }
            sb.Append($"{tabspace}#define ISMAX ".PadRight(pad1));
            sb.Append($"(DPMAX + ");
            sb.Append($"{index.ToString()}".PadLeft(pad2));
            sb.AppendLine($") /* aantal overige ingangen */");

            return sb.ToString();
        }

        private string GenerateSysHHulpElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* hulp elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(HulpElementen, "HEMAX"));

            return sb.ToString();
        }

        private string GenerateSysHGeheugenElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* geheugen elementen */");
            sb.AppendLine("/* ------------------ */");

            sb.Append(GetAllElementsSysHLines(GeheugenElementen, "MEMAX"));

            return sb.ToString();
        }

        private string GenerateSysHTijdElementen(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* tijd elementen */");
            sb.AppendLine("/* -------------- */");

            sb.Append(GetAllElementsSysHLines(Timers, "TMMAX"));

            return sb.ToString();
        }
        
        private string GenerateSysHCounters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* teller elementen */");
            sb.AppendLine("/* ---------------- */");

            sb.Append(GetAllElementsSysHLines(Counters, "CTMAX"));

            return sb.ToString();
        }

        private string GenerateSysHSchakelaars(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* schakelaars */");
            sb.AppendLine("/* ----------- */");

            sb.Append(GetAllElementsSysHLines(Schakelaars, "SCHMAX"));

            return sb.ToString();
        }


        private string GenerateSysHParameters(ControllerModel controller)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* parameters */");
            sb.AppendLine("/* ---------- */");

            sb.Append(GetAllElementsSysHLines(Parameters, "PRMMAX"));

            return sb.ToString();
        }
    }
}
