using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    // Prio: LEGACY HD ingrepen (c.PrioData.HDIngrepen, HDIngreepModel). Kept as is, pending the migration to
    // generic HD ingrepen (PrioIngreepModel with type Hulpdienst, #76); delete together with HDIngreepModel.
    // Hooks: CollectCCOLElements (AddAllHDIngreepElements) and PrioCInUitMelden (prio.c InUitMelden()).
    // Small legacy HD parts remain inline elsewhere: CollectCCOLElements (HD loop, KAR sg parameters),
    // GetSystemApplicationCode, GetPostSystemApplicationCode and GetPostAfhandelingPrioCode.
    public partial class PrioCodeGenerator
    {
        private void AddAllHDIngreepElements(HDIngreepModel hd, ControllerModel c)
        {
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ushdinm}{hd.FaseCyclus}", _ushdinm, hd.HDInmeldingBitmapData, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhd}{hd.FaseCyclus}", _hhd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}", _hhdin, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}", _hhduit, hd.FaseCyclus));

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tgbhd}{hd.FaseCyclus}", hd.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tgbhd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_trthd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _trthd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvchd}{hd.FaseCyclus}", 999, CCOLElementTimeTypeEnum.CT_type, _cvchd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriohd}{hd.FaseCyclus}", 9005, CCOLElementTimeTypeEnum.None, _prmpriohd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtohd}{hd.FaseCyclus}", hd.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtohd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtbghd}{hd.FaseCyclus}", hd.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtbghd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtghd}{hd.FaseCyclus}", hd.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtghd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmupinagbhd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.None, _prmupinagbhd, hd.FaseCyclus));

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{hd.FaseCyclus}hd", 0, CCOLElementTimeTypeEnum.TE_type, _tbtovg, hd.FaseCyclus));

            if (hd.KAR)
            {
                _myDetectors.Add(hd.DummyKARInmelding);
                _myDetectors.Add(hd.DummyKARUitmelding);

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}kar", _hhdin, hd.FaseCyclus, "KAR"));
                if (hd.KARToepassenFilterTijden)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}kar", hd.KARInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "KAR"));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}kar", _hhduit, hd.FaseCyclus, "KAR"));
                if (hd.KARToepassenFilterTijden)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thduit}{hd.FaseCyclus}kar", hd.KARUitmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thduit, hd.FaseCyclus, "KAR"));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schchecksirene}{hd.FaseCyclus}", hd.Sirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schchecksirene, hd.FaseCyclus));
            }

            if (hd.RIS)
            {
                var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == hd.FaseCyclus);
                if (fcRis != null && fcRis.LaneData.Count > 0)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}ris", _hhdin, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}ris", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}ris", _hhduit, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}ris", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "RIS"));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstart}{hd.FaseCyclus}hd", hd.RisStart, CCOLElementTimeTypeEnum.None, _prmrisstart, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisend}{hd.FaseCyclus}hd", hd.RisEnd, CCOLElementTimeTypeEnum.None, _prmrisend, hd.FaseCyclus));
                    if (hd.RisEta.HasValue)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmriseta}{hd.FaseCyclus}hd", hd.RisEta.Value, CCOLElementTimeTypeEnum.TE_type, _prmriseta, hd.FaseCyclus));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisrole}{hd.FaseCyclus}hd", 64, CCOLElementTimeTypeEnum.None, _prmrisrole, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrissubrole}{hd.FaseCyclus}hd", 32, CCOLElementTimeTypeEnum.None, _prmrissubrole, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisimportance}{hd.FaseCyclus}hd", (int)hd.RisImportance, CCOLElementTimeTypeEnum.None, _prmrisimportance, hd.FaseCyclus));
                   
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstationtype}{hd.FaseCyclus}hd", 0, CCOLElementTimeTypeEnum.None, _prmrisstationtype, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisgrenspriotype}{hd.FaseCyclus}hd", 9, CCOLElementTimeTypeEnum.None, _prmrisgrenspriotype, hd.FaseCyclus));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmrisapproachid}{hd.FaseCyclus}hd", fcRis.ApproachID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                                PrioCodeGeneratorHelper.CAT_Prioriteren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));

                    foreach (var lane in fcRis.LaneData)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmrislaneid}{hd.FaseCyclus}hd_{lane.RijstrookIndex}",
                                lane.LaneID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, "",
                                PrioCodeGeneratorHelper.CAT_Prioriteren, PrioCodeGeneratorHelper.SUBCAT_OpenbaarVervoer));
                    }
                }
            }

            if (hd.Opticom && !string.IsNullOrWhiteSpace(hd.OpticomRelatedInput))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}opt", _hhdin, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}opt", hd.OpticomInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdinuit}{hd.FaseCyclus}opt", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdinuit, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}opt", _hhduit, hd.FaseCyclus, "Opticom"));
            }
        }

        private void AppendHDLegacyInUitMeldenCode(ControllerModel c, StringBuilder sb, string ts, ref bool first)
        {
            // Inmelding HD
            foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom || x.RIS))
            {
                var ifc = -1;
                if (int.TryParse(hd.FaseCyclus, out var ifc2))
                {
                    ifc = ifc2;
                }

                var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters
                    ? $"PRM[{_prmpf}{_prmkarsghd}{hd.FaseCyclus}]"
                    : ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers
                        ? (ifc - 200).ToString()
                        : ifc.ToString();
                var inmHelems = new List<string>();
                if (!first) sb.AppendLine();
                first = false;
                sb.AppendLine($"{ts}/* Inmelding HD {_fcpf}{hd.FaseCyclus} */");
                if (hd.KAR && ifc >= 0)
                {
                    if (hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                    {
                        var hdpriofcnr = (hd.InmeldingOokDoorFase.ToString().Length < 2) ? "0" + hd.InmeldingOokDoorFase.ToString() 
                                                                                         : hd.InmeldingOokDoorFase.ToString();
                        var actualAlsoFc = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsghd}{hdpriofcnr}]"
                            : hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers 
                                ? (hd.InmeldingOokDoorFase - 200).ToString() 
                                : hd.InmeldingOokDoorFase.ToString();
                        sb.AppendLine($"{ts}" +
                                      $"IH[{_hpf}{_hhdin}{hd.FaseCyclus}kar] = " +
                                      (hd.KARToepassenFilterTijden 
                                          ? $"RT[{_tpf}{_thdin}{hd.FaseCyclus}kar] = " +
                                            $"!T[{_tpf}{_thdin}{hd.FaseCyclus}kar] && "
                                          : "") +
                                      $"SCH[{_schpf}{_schhdin}{hd.FaseCyclus}kar] && " +
                                      $"((DSIMelding_HD_V1({sgCheck}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}]) || " +
                                      $"DSIMelding_HD_V1({actualAlsoFc}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}])));");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}kar] = " +
                                      (hd.KARToepassenFilterTijden 
                                          ? $"RT[{_tpf}{_thdin}{hd.FaseCyclus}kar] = " +
                                            $"!T[{_tpf}{_thdin}{hd.FaseCyclus}kar] && "
                                          : "") +
                                      $"SCH[{_schpf}{_schhdin}{hd.FaseCyclus}kar] && " +
                                      $"(DSIMelding_HD_V1({sgCheck}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}]));");
                    }

                    inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}kar");
                }

                var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == hd.FaseCyclus);
                if (risFc != null && hd.RIS)
                {
                    sb.AppendLine("#ifndef NO_RIS");

                    sb.Append($"{ts}" +
                              $"IH[{_hpf}{_hhdin}{hd.FaseCyclus}ris] = " +
                              $"SCH[{_schpf}{_schhdin}{hd.FaseCyclus}ris] && (");

                    first = true;
                    if (risFc != null)
                    {
                        sb.AppendLine();
                        foreach (var lane in risFc.LaneData)
                        {
                            if (!first) sb.AppendLine(" ||");
                            var itf = c.RISData.HasMultipleSystemITF
                                ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == lane.SystemITF)
                                : -1;
                            sb.Append($"{ts}{ts}{ts}ris_inmelding_selectief(" +
                                      $"{_fcpf}{risFc.FaseCyclus}, " +
                                      $"PRM[{_prmpf}{_prmrisapproachid}{risFc.FaseCyclus}hd], " +
                                      $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                      $"PRM[{_prmpf}{_prmrislaneid}{risFc.FaseCyclus}hd_{lane.RijstrookIndex}], " +
                                      $"PRM[{_prmpf}{_prmrisstationtype}{risFc.FaseCyclus}hd], " +
                                      $"PRM[{_prmpf}{_prmrisstart}{hd.FaseCyclus}hd], " +
                                      $"PRM[{_prmpf}{_prmrisend}{hd.FaseCyclus}hd], " +
                                      $"PRM[{_prmpf}{_prmrisrole}{risFc.FaseCyclus}hd], " +
                                      $"PRM[{_prmpf}{_prmrissubrole}{risFc.FaseCyclus}hd], " +
                                      $"PRM[{_prmpf}{_prmrisimportance}{risFc.FaseCyclus}hd], " +
                                      $"{(hd.RisEta.HasValue ? $"PRM[{_prmpf}{_prmriseta}{risFc.FaseCyclus}hd]" : "NG")}, " +
                                      $"hdFC{hd.FaseCyclus})");
                            first = false;
                        }
                    }

                    sb.AppendLine(");");
                    inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}ris");

                    sb.AppendLine("#endif /* NO_RIS */");
                }

                if (hd.Opticom)
                {
                    sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}opt] = !T[{_tpf}{_thdin}{hd.FaseCyclus}opt] && SCH[{_schpf}{_schhdinuit}{hd.FaseCyclus}opt] && !C[{_ctpf}{_cvchd}{hd.FaseCyclus}] && DB[{_dpf}{hd.OpticomRelatedInput}];");
                    sb.AppendLine($"{ts}RT[{_tpf}{_thdin}{hd.FaseCyclus}opt] = G[{_fcpf}{hd.FaseCyclus}] && C[{_ctpf}{_cvchd}{hd.FaseCyclus}];");
                    inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}opt");
                }

                sb.Append($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] = ");
                first = true;
                foreach (var i in inmHelems)
                {
                    if (!first) sb.Append(" || ");
                    sb.Append($"IH[{i}]");
                    first = false;
                }

                sb.AppendLine(";");
            }

            // Uitmelding HD
            foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom || x.RIS))
            {
                var ifc = -1;
                if (int.TryParse(hd.FaseCyclus, out var ifc2))
                {
                    ifc = ifc2;
                }

                var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters
                    ? $"PRM[{_prmpf}{_prmkarsghd}{hd.FaseCyclus}]"
                    : ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers
                        ? (ifc - 200).ToString()
                        : ifc.ToString();
                var uitmHelems = new List<string>();
                if (!first) sb.AppendLine();
                first = false;
                sb.AppendLine($"{ts}/* Uitmelding HD {_fcpf}{hd.FaseCyclus} */");
                if (hd.KAR && ifc >= 0)
                {
                    if (hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                    {
                        var hdpriofcnr = (hd.InmeldingOokDoorFase.ToString().Length < 2) ? "0" + hd.InmeldingOokDoorFase.ToString()
                                                                                            : hd.InmeldingOokDoorFase.ToString();
                        var actualAlsoFc = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsghd}{hdpriofcnr}]"
                            : hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers 
                                ? (hd.InmeldingOokDoorFase - 200).ToString() 
                                : hd.InmeldingOokDoorFase.ToString();
                        sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}kar] = " +
                                      (hd.KARToepassenFilterTijden
                                          ? $"RT[{_tpf}{_thduit}{hd.FaseCyclus}kar] = " +
                                            $"!T[{_tpf}{_thduit}{hd.FaseCyclus}kar] && "
                                          : "") +
                                      $"SCH[{_schpf}{_schhduit}{hd.FaseCyclus}kar] && " +
                                      $"(DSIMelding_HD_V1({sgCheck}, CIF_DSUIT, FALSE) || DSIMelding_HD_V1({actualAlsoFc}, CIF_DSUIT, FALSE));");
                    }
                    else
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}kar] = " +
                                      (hd.KARToepassenFilterTijden
                                          ? $"RT[{_tpf}{_thduit}{hd.FaseCyclus}kar] = " +
                                            $"!T[{_tpf}{_thduit}{hd.FaseCyclus}kar] && "
                                          : "") + 
                                      $"SCH[{_schpf}{_schhduit}{hd.FaseCyclus}kar] && " +
                                      $"(DSIMelding_HD_V1({sgCheck}, CIF_DSUIT, FALSE));");
                    }

                    uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}kar");
                }

                if (hd.RIS)
                {
                    sb.AppendLine("#ifndef NO_RIS");

                    sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}ris] = SCH[{_schpf}{_schhduit}{hd.FaseCyclus}ris] && (ris_uitmelding_selectief(hdFC{hd.FaseCyclus}));");
                    uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}ris");

                    sb.AppendLine("#endif /* NO_RIS */");
                }

                if (hd.Opticom)
                {
                    sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}opt] = SCH[{_schpf}{_schhdinuit}{hd.FaseCyclus}opt] && !TDH[{_dpf}{hd.OpticomRelatedInput}] && TDH_old[{_dpf}{hd.OpticomRelatedInput}];");
                    uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}opt");
                }

                sb.Append($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = ");
                first = true;
                foreach (var i in uitmHelems)
                {
                    if (!first) sb.Append(" || ");
                    sb.Append($"IH[{i}]");
                    first = false;
                }

                sb.AppendLine(";");
            }
        }
    }
}
