using System;
using System.IO;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public partial class CCOLGenerator
    {
        private string GenerateRisSimC(ControllerModel c)
        {
            var risModel = c.RISData;

            var _prmrislaneid = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrislaneid");
            var _prmrisapproachid = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid");
            var _prmlijn = CCOLGeneratorSettingsProvider.Default.GetElementName("prmlijn");

            var sb = new StringBuilder();

            sb.AppendLine("/* APPLICATIE RIS SIMULATIEPROGRAMMA */");
            sb.AppendLine("/* --------------------------------- */");
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateFileHeader(c.Data, "rissim.c"));
            sb.AppendLine();
            sb.Append(CCOLHeaderGenerator.GenerateVersionHeader(c.Data));
            sb.AppendLine();
            sb.AppendLine("/* INCLUDE FILES */");
            sb.AppendLine("/* ============= */");
            sb.AppendLine("#include <stdio.h>");
            sb.AppendLine();
            sb.AppendLine($"#include \"{c.Data.Naam}sys.h\"");
            sb.AppendLine("#include \"cif.inc\"            /* declaratie CIF_IS[]           */");
            sb.AppendLine("#include \"isvar.h\"            /* declaratie IS[]               */");
            sb.AppendLine("#include \"rissimvar.c\"        /* ris-simulatie variabelen      */");
            sb.AppendLine("#include \"xyprintf.h\"         /* declaratie xyprintf-functie   */");
            sb.AppendLine("#include \"prmvar.h\"           /* declaratie PRM[]              */");
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
            sb.AppendLine($"{ts}#ifdef RISSIMULATIE");
            foreach (var l in risModel.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach(var s in l.SimulatedStations)
                {
                    var sitf = "SYSTEM_ITF";
                    if (risModel.HasMultipleSystemITF)
                    {
                        var msitf = risModel.MultiSystemITF.FirstOrDefault(x => x.SystemITF == s.SystemITF);
                        if (msitf != null)
                        {
                            var j = risModel.MultiSystemITF.IndexOf(msitf);
                            sitf = $"SYSTEM_ITF{j + 1}";
                        }
                    }
                    sb.AppendLine($"{ts}ris_simulation_itsstation_parameters({sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], {_fcpf}{l.SignalGroupName}, RIF_STATIONTYPE_{s.Type}, 0, {(s.Prioriteit ? "1" : "0")}, {s.Flow}, {s.Snelheid}, 10, {s.Afstand}, 10, 1);");
                }
            }
            sb.AppendLine($"{ts}#endif /* RISSIMULATIE */");
            sb.AppendLine();
            foreach (var l in risModel.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach (var s in l.SimulatedStations)
                {
                    var tl = s.Type == RISStationTypeSimEnum.PEDESTRIAN ? 1 : s.Type == RISStationTypeSimEnum.CYCLIST ? 2 : 6;
                    var dl = s.Type == RISStationTypeSimEnum.PEDESTRIAN ? 50 : s.Type == RISStationTypeSimEnum.CYCLIST ? 100 : 300;
                    var sitf = "SYSTEM_ITF";
                    if (risModel.HasMultipleSystemITF)
                    {
                        var msitf = risModel.MultiSystemITF.FirstOrDefault(x => x.SystemITF == s.SystemITF);
                        if (msitf != null)
                        {
                            var j = risModel.MultiSystemITF.IndexOf(msitf);
                            sitf = $"SYSTEM_ITF{j + 1}";
                        }
                    }
                    sb.AppendLine($"{ts}ris_display_lane_parameters({sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], \"{l.SignalGroupName}-{l.RijstrookIndex}\", {tl}, {dl});");
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
            sb.AppendLine($"{ts}#if (!defined AUTOMAAT_TEST)");
            var first = true;
            foreach (var l in risModel.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                foreach (var s in l.SimulatedStations)
                {
                    if (first)
                    {
                        sb.AppendLine($"{ts}char buffer[128];");
                        first = false;
                    }

                    var sitf = "SYSTEM_ITF";
                    if (risModel.HasMultipleSystemITF)
                    {
                        var msitf = risModel.MultiSystemITF.FirstOrDefault(x => x.SystemITF == s.SystemITF);
                        if (msitf != null)
                        {
                            var j = risModel.MultiSystemITF.IndexOf(msitf);
                            sitf = $"SYSTEM_ITF{j + 1}";
                        }
                    }

                    switch (s.Type)
                    {
                        case RISStationTypeSimEnum.UNKNOWN:
                            break;
                        case RISStationTypeSimEnum.PEDESTRIAN:
                        case RISStationTypeSimEnum.CYCLIST:
                        case RISStationTypeSimEnum.MOPED:
                        case RISStationTypeSimEnum.MOTORCYCLE:
                        case RISStationTypeSimEnum.PASSENGERCAR:
                            sb.AppendLine($"{ts}if (SIS({_ispf}{s.Naam})) ris_simulation_put_itsstation_pb({sitf}, PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], {_fcpf}{s.SimulationData.FCNr}, RIF_STATIONTYPE_{s.Type}, 0, {(s.Prioriteit ? "1" : "0")}, {s.Snelheid}, {s.Afstand}, 1);");
                            break;
                        case RISStationTypeSimEnum.LIGHTTRUCK:
                        case RISStationTypeSimEnum.HEAVYTRUCK:
                        case RISStationTypeSimEnum.TRAILER:
                        case RISStationTypeSimEnum.BUS:
                        case RISStationTypeSimEnum.TRAM:
                            sb.AppendLine($"{ts}if (SIS({_ispf}{s.Naam})) {{");
                            var prio = c.PrioData.PrioIngrepen.FirstOrDefault(x => x.FaseCyclus == s.SignalGroupName && x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde));
                            var lijn = "0";
                            if (prio != null && prio.CheckLijnNummer && prio.LijnNummers.Any())
                            {
                                lijn = $"PRM[{_prmpf}{_prmlijn}{CCOLCodeHelper.GetPriorityName(prio)}_01]";
                                sb.AppendLine($"{ts}{ts}sprintf(buffer, \"%d\", {lijn});");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}sprintf(buffer, \"\");");
                            }
                            sb.AppendLine($"{ts}{ts}ris_simulation_put_itsstation_pb_ex(" +
                                      $"{sitf}, " +
                                      $"PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                      $"{_fcpf}{l.SignalGroupName}, " +
                                      $"RIF_STATIONTYPE_{s.Type}, " +
                                      $"{lijn}, " +
                                      $"{(s.Prioriteit ? "1" : "0")}, " +
                                      $"{s.Snelheid}, " +
                                      $"{s.Afstand}, " +
                                      $"1," +
                                      $"RIF_VEHICLEROLE_{s.VehicleRole}," +
                                      $"RIF_VEHICLESUBROLE_{s.VehicleSubrole}," +
                                      $"25," +
                                      $"{(s.SimulationData.FCNr == "NG" ? "NG" : $"{_fcpf}{s.SimulationData.FCNr}")}," +
                                      $"PRM[{_prmpf}{_prmrisapproachid}{s.SignalGroupName}]," +
                                      $"buffer," +
                                      $"123," +
                                      $"{s.Importance});");
                            sb.AppendLine($"{ts}}}"); 
                            break;
                        case RISStationTypeSimEnum.SPECIALVEHICLES:
                            sb.AppendLine($"{ts}if (SIS({_ispf}{s.Naam})) {{");
                            sb.AppendLine($"{ts}{ts}ris_simulation_put_itsstation_pb_ex(" +
                                      $"{sitf}, " +
                                      $"PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                      $"{_fcpf}{l.SignalGroupName}, " +
                                      $"RIF_STATIONTYPE_{s.Type}, " +
                                      $"-1, " +
                                      $"{(s.Prioriteit ? "1" : "0")}, " +
                                      $"{s.Snelheid}, " +
                                      $"{s.Afstand}, " +
                                      $"1," +
                                      $"RIF_VEHICLEROLE_{s.VehicleRole}," +
                                      $"RIF_VEHICLESUBROLE_{s.VehicleSubrole}," +
                                      $"25," +
                                      $"NG," +
                                      $"PRM[{_prmpf}{_prmrisapproachid}{s.SignalGroupName}]," +
                                      $"\"NG\"," +
                                      $"123," +
                                      $"{s.Importance});");
                            sb.AppendLine($"{ts}}}"); 
                            break;
                        case RISStationTypeSimEnum.ROADSIDEUNIT:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            sb.AppendLine($"");
            sb.AppendLine($"{ts}/* Display ris_lanes met ItsStations */");
            sb.AppendLine($"{ts}/* --------------------------------- */");
            var i = 15;
            foreach (var l in risModel.RISFasen.SelectMany(x => x.LaneData).Where(x => x.SimulatedStations.Any()))
            {
                sb.AppendLine($"{ts}xyprintf(40, {i}, \"%s\", RIS_DISPLAY_LANE_STRING[PRM[{_prmpf}{_prmrislaneid}{l.SignalGroupName}_{l.RijstrookIndex}]]);");
                ++i;
            }
            sb.AppendLine($"");
            sb.AppendLine($"{ts}/* Display aantal ItsStations en PrioRequests */");
            sb.AppendLine($"{ts}/* ------------------------------------------ */  ");
            sb.AppendLine($"{ts}xyprintf(40, {i + 1}, \"ItsStation =% -3d ItsStation - Ex =% -3d\", RIS_ITSSTATION_AP_NUMBER,  RIS_ITSSTATION_EX_AP_NUMBER);");
            sb.AppendLine($"{ts}xyprintf(40, {i + 2}, \"PrioRequest =% -3d PrioRequest_Ex =% -3d\", RIS_PRIOREQUEST_AP_NUMBER, RIS_PRIOREQUEST_EX_AP_NUMBER);");
            sb.AppendLine($"{ts}#endif");

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
