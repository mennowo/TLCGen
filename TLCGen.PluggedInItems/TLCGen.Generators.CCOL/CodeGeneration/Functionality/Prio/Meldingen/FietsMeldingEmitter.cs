using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: meldingen of type FietsMassaPeloton (fiets prioriteit): inmelding via fietsprio_inmelding(), counting
    // cyclists on a loop (FietsPrioriteitGebruikLus) and/or via RIS (FietsPrioriteitGebruikRIS); uitmelding at
    // the end of the green time.
    public partial class PrioCodeGenerator
    {
        private sealed class FietsMeldingEmitter : PrioMeldingEmitter
        {
            public FietsMeldingEmitter(PrioCodeGenerator generator) : base(generator)
            {
            }

            public override void AddElements(PrioMeldingContext m, List<CCOLElement> elements)
            {
                if (!m.IsInmelding) return;

                var melding = m.Melding;
                var fc = m.Prio.FaseCyclus;
                var b = m.Names.Base;
                var provider = CCOLGeneratorSettingsProvider.Default;
                elements.Add(provider.CreateElement($"{G._cftscyc}{b}", 999, CCOLElementTimeTypeEnum.None, G._cftscyc, fc));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(provider.CreateElement($"{G._cftsvtg}{b}", 999, CCOLElementTimeTypeEnum.None, G._cftsvtg, fc));
                }
                elements.Add(provider.CreateElement($"{G._prmftsblok}{b}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, G._prmftsblok, fc));
                elements.Add(provider.CreateElement($"{G._prmftsmaxpercyc}{b}", melding.FietsPrioriteitAantalKeerPerCyclus, CCOLElementTimeTypeEnum.None, G._prmftsmaxpercyc, fc));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(provider.CreateElement($"{G._prmftsminvtg}{b}", melding.FietsPrioriteitMinimumAantalVoertuigen, CCOLElementTimeTypeEnum.None, G._prmftsminvtg, fc));
                }
                elements.Add(provider.CreateElement($"{G._prmftsminwt}{b}", melding.FietsPrioriteitMinimumWachttijdVoorPrioriteit, CCOLElementTimeTypeEnum.TS_type, G._prmftsminwt, fc));

                if (melding.FietsPrioriteitGebruikRIS)
                {
                    elements.Add(provider.CreateElement($"{G._mftstelris}{b}", G._mftstelris, m.PrioName));
                    elements.Add(provider.CreateElement($"{G._prmftsminvtgris}{b}",
                        melding.FietsPrioriteitMinimumAantalVoertuigenRIS, CCOLElementTimeTypeEnum.None, G._prmftsminvtgris, fc));
                }
            }

            public override bool RequiresNoActiveInmelding(PrioMeldingContext m) => m.IsInmelding;

            public override void AppendCondition(PrioMeldingContext m, StringBuilder sb)
            {
                var melding = m.Melding;
                var prio = m.Prio;
                var b = m.Names.Base;
                if (m.IsInmelding)
                {
                    sb.AppendLine();
                    sb.AppendLine($"{m.Tts}{m.Ts}fietsprio_inmelding({G._fcpf}{prio.FaseCyclus}, " +
                                  (melding.FietsPrioriteitGebruikLus ? $"{G._dpf}{melding.RelatedInput1}, " : "NG, ") +
                                  (melding.FietsPrioriteitGebruikLus ? $"{G._ctpf}{G._cftsvtg}{b}, " : "NG, ") +
                                  $"{G._ctpf}{G._cftscyc}{b}, " +
                                  $"{G._prmpf}{G._prmftsblok}{b}, " +
                                  $"{G._prmpf}{G._prmftsmaxpercyc}{b}, " +
                                  (melding.FietsPrioriteitGebruikLus ? $"{G._prmpf}{G._prmftsminvtg}{b}, " : "NG, ") +
                                  $"{G._prmpf}{G._prmftsminwt}{b}, " +
                                  $"ML, " +
                                  (melding.FietsPrioriteitGebruikRIS
                                      ? $"{G._mpf}{G._mftstelris}{b}, {G._prmpf}{G._prmftsminvtgris}{b}"
                                      : "NG, NG") +
                                  ");");
                }
                else
                {
                    sb.AppendLine($"C[{G._ctpf}{G._cvc}{m.PrioName}] && G[{G._fcpf}{prio.FaseCyclus}] && (T[{G._tpf}{G._tgb}{m.PrioName}] && (T_timer[{G._tpf}{G._tgb}{m.PrioName}] > 1));");
                }
            }

            /// <summary>
            /// fietsprio_update() call of a fiets inmelding, emitted before the meldingen of the ingreep.
            /// </summary>
            public void AppendUpdateCode(PrioMeldingContext m, StringBuilder sb)
            {
                var melding = m.Melding;
                var hov = m.IsInmelding ? G._hprioin.ToString() : G._hpriouit.ToString();
                // NOTE: unlike Names.Hulp, this only includes the input when the loop is used (todo note 1579)
                var he = $"{G._hpf}{hov}{m.Names.Base}";
                if (melding.FietsPrioriteitGebruikLus)
                {
                    he += melding.RelatedInput1;
                }
                sb.AppendLine($"{m.Ts}fietsprio_update({G._fcpf}{m.Prio.FaseCyclus}, " +
                              (melding.FietsPrioriteitGebruikLus ? $"{G._dpf}{melding.RelatedInput1}, " : "NG, ") +
                              (melding.FietsPrioriteitGebruikLus ? $"{G._ctpf}{G._cftsvtg}{m.Names.Base}, " : "NG, ") +
                              $"{G._ctpf}{G._cftscyc}{m.Names.Base}," +
                              $"SH[{he}], ML);");
            }

            /// <summary>
            /// Counting of cyclists via RIS (MM[mftstelris]) at the start of the if-block of the melding.
            /// </summary>
            public void AppendRisTellingCode(PrioMeldingContext m, StringBuilder sb)
            {
                var c = m.C;
                var prio = m.Prio;
                var tts = m.Tts;
                var mm = $"{G._mpf}{G._mftstelris}{m.Names.Base}";
                sb.AppendLine($"{tts}MM[{mm}] = 0;");
                var lanes = c.RISData.RISPelotonLanes.Where(x => x.SignalGroupName == prio.FaseCyclus).ToArray();
                if (lanes.Length > 0)
                {
                    sb.AppendLine($"#ifndef NO_RIS");
                    foreach (var l in lanes)
                    {
                        // opzoeken lane instellingen
                        var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);
                        if (risFc == null) continue;

                        var fcLane = risFc.LaneData.FirstOrDefault(x => x.RijstrookIndex == l.RijstrookIndex);

                        var itf = c.RISData.HasMultipleSystemITF
                            ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == fcLane?.SystemITF) : -1;

                        var laneIdPrm = $"{G._prmpf}{G._prmrislaneid}_{l.RijstrookIndex}";
                        if (m.PrioName != prio.FaseCyclus)
                        {
                            laneIdPrm = $"{G._prmpf}{G._prmrislaneid}{m.PrioName}_{l.RijstrookIndex}";
                        }

                        sb.AppendLine($"{tts}MM[{mm}] += {(fcLane.UseHeading ? "ris_itsstations_heading" : "ris_itsstations")}(" +
                                      $"{G._fcpf}{prio.FaseCyclus}, " +
                                      $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                      $"PRM[{laneIdPrm}], " +
                                      $"RIS_{l.Type}, " +
                                      $"PRM[{G._prmpf}{G._prmrispstart}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], " +
                                      $"PRM[{G._prmpf}{G._prmrispend}{l.SignalGroupName}{l.Type.GetDescription()}{l.RijstrookIndex}], " +
                                      $"SCH[{G._schpf}{G._schrisgeencheckopsg}]" +
                                      (fcLane.UseHeading
                                          ? $", PRM[{G._prmpf}{G._prmrislaneheading}{l.SignalGroupName}_{l.RijstrookIndex}], " +
                                            $"PRM[{G._prmpf}{G._prmrislaneheadingmarge}{l.SignalGroupName}_{l.RijstrookIndex}]);"
                                          : ");"));
                    }
                    sb.AppendLine($"#endif /* NO_RIS */");
                }
            }
        }
    }
}
