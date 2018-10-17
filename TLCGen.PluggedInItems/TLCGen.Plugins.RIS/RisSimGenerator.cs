using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Models;
using TLCGen.Plugins.RIS.Models;

namespace TLCGen.Plugins.RIS
{
    public partial class RISPlugin
    {
        internal void GenerateRisSimC(ControllerModel c, RISDataModel model, string ts)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* APPLICATIE RIS SIMULATIEPROGRAMMA */");
            sb.AppendLine("/* --------------------------------- */");
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateFileHeader(c.Data, "rissim.c"));
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* INCLUDE FILES */");
            sb.AppendLine("/* ============= */");
            sb.AppendLine($"#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine("#include \"cif.inc\"            /* declaratie CIF_IS[]           */");
            sb.AppendLine("#include \"isvar.h\"            /* declaratie IS[]               */");
            sb.AppendLine("#include \"rissimvar.c\"        /* ris-simulatie variabelen      */");
            sb.AppendLine("#include \"xyprintf.h\"         /* declaratie xyprintf-functie   */");
            sb.AppendLine();
            sb.AppendLine("/* RIS-FI - ObjectID<Intersection, LaneID en ObjectID<SIgnalGroupID> */");
            sb.AppendLine("/* ================================================================= */");
            sb.AppendLine("/* The ID of the intersection can be retrieved from the ITF controlData section, ");
            sb.AppendLine(" * element “name” in “controlledIntersection” (SYSTEM_ITF).");
            sb.AppendLine(" * The LaneNr is a unique number within the intersection (LaneID).");
            sb.AppendLine(" * The (TLC) ID of the signal group can be retrieved from the ITF controlData section, ");
            sb.AppendLine(" * element “name” in “sg” (FC_code[]).");
            sb.AppendLine(" */");
            sb.AppendLine($"/*RIS SIMULATIE PARAMETERS */");
            sb.AppendLine($"/*======================== */");
            sb.AppendLine($"");
            sb.AppendLine($"void ris_simulation_parameters(void)");
            sb.AppendLine($"{{");
            foreach (var l in model.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach(var s in l.SimulatedStations)
                {
                    sb.AppendLine($"{ts}ris_simulation_itsstation_parameters(SYSTEM_ITF, ris_lane{l.SignalGroupName}{l.RijstrookIndex}, {_fcpf}{l.SignalGroupName}, RIF_STATIONTYPE_{s.Type}, 0, 0, {s.Flow}, {s.Snelheid}, 10, {s.Afstand}, 10, 1);");
                }
            }
            sb.AppendLine();
            foreach (var l in model.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach (var s in l.SimulatedStations)
                {
                    var tl = s.Type == RISStationTypeSimEnum.PEDESTRIAN ? 1 : s.Type == RISStationTypeSimEnum.CYCLIST ? 2 : 6;
                    var dl = s.Type == RISStationTypeSimEnum.PEDESTRIAN ? 50 : s.Type == RISStationTypeSimEnum.CYCLIST ? 100 : 300;
                    sb.AppendLine($"{ts}ris_display_lane_parameters(SYSTEM_ITF, ris_lane{l.SignalGroupName}{l.RijstrookIndex}, \"{l.SignalGroupName}-{l.RijstrookIndex}\", {tl}, {dl});");
                }
            }
            sb.AppendLine($"}}");
            sb.AppendLine($"");
            sb.AppendLine($"/* RIS SIMULATIE APPLICATIE */");
            sb.AppendLine($"/* ======================== */");
            sb.AppendLine($"");
            sb.AppendLine($"#define SIS(is)  (IS[is] && !IS_old[is])");
            sb.AppendLine($"void ris_simulation_application(void)");
            sb.AppendLine($"{{");
            foreach (var l in model.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach (var s in l.SimulatedStations)
                {
                    sb.AppendLine($"{ts}if (SIS({_ispf}{s.Naam})) ris_simulation_put_itsstation_pb( SYSTEM_ITF, ris_lane{l.SignalGroupName}{l.RijstrookIndex}, {_fcpf}{l.SignalGroupName}, RIF_STATIONTYPE_{s.Type}, 0, 0, {s.Snelheid}, {s.Afstand}, 1);");
                }
            }
            sb.AppendLine($"");
            sb.AppendLine($"/* Display ris_lanes met ItsStations */");
            sb.AppendLine($"/* --------------------------------- */");
            var i = 15;
            foreach (var l in model.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                sb.AppendLine($"xyprintf(0, {i}, \"%s\", RIS_DISPLAY_LANE_STRING[ris_lane{l.SignalGroupName}{l.RijstrookIndex}]);");
                ++i;
            }
            sb.AppendLine($"");
            sb.AppendLine($"/* Display aantal ItsStations en PrioRequests */");
            sb.AppendLine($"/* ------------------------------------------ */  ");
            sb.AppendLine($"xyprintf(0, {i + 1}, \"ItsStation =% -3d ItsStation - Ex =% -3d\", RIS_ITSSTATION_AP_NUMBER,  RIS_ITSSTATION_EX_AP_NUMBER);");
            sb.AppendLine($"xyprintf(0, {i + 2}, \"PrioRequest =% -3d PrioRequest_Ex =% -3d\", RIS_PRIOREQUEST_AP_NUMBER, RIS_PRIOREQUEST_EX_AP_NUMBER);");

            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName), $"{c.Data.Naam}rissim.c"), sb.ToString());
        }
    }
}
