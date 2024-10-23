using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Documents;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class IsgFuncCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        //private CCOLGeneratorCodeStringSettingModel _mtest;
        private CCOLGeneratorCodeStringSettingModel _misgar;
        private CCOLGeneratorCodeStringSettingModel _tisgfo;
        private CCOLGeneratorCodeStringSettingModel _tisgvs;
        private CCOLGeneratorCodeStringSettingModel _tisglr;
        private CCOLGeneratorCodeStringSettingModel _tisginl;
        private CCOLGeneratorCodeStringSettingModel _tisgxnl;
        private CCOLGeneratorCodeStringSettingModel _hisglos;
        private CCOLGeneratorCodeStringSettingModel _schisglos;
        private CCOLGeneratorCodeStringSettingModel _hisgmad; // obsolete ?
        private CCOLGeneratorCodeStringSettingModel _usisgtijd;
#pragma warning restore 0649
        private string _prmaltg;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnleg;
        private string _tnlegd;
        private string _tnlcv;
        private string _tnlcvd;
        private string _tnlsg;
        private string _tnlsgd;
        private string _hfile;
        private string _prmfperc;
        private string _prmxnl;
        private string _hmad;
        private string _tvgnaloop;
        private string _schgs;
        private string _hnlsg;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_misgar}{fc.Naam}", _misgar, fc.Naam));
            }

            foreach (var vs in c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict))
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgfo}{vs:vannaar}", vs.GelijkstartOntruimingstijdFaseVan, CCOLElementTimeTypeEnum.TE_type, _tisgfo, vs.FaseVan, vs.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgfo}{vs:naarvan}", vs.GelijkstartOntruimingstijdFaseNaar, CCOLElementTimeTypeEnum.TE_type, _tisgfo, vs.FaseVan, vs.FaseNaar));
            }

            foreach (var vs in c.InterSignaalGroep.Voorstarten)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgvs}{vs:vannaar}", vs.VoorstartTijd, CCOLElementTimeTypeEnum.TE_type, _tisgvs, vs.FaseVan, vs.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgfo}{vs:naarvan}", vs.VoorstartOntruimingstijd, CCOLElementTimeTypeEnum.TE_type, _tisgfo, vs.FaseVan, vs.FaseNaar));
            }

            foreach (var lr in c.InterSignaalGroep.LateReleases)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisglr}{lr:vannaar}", lr.LateReleaseTijd, CCOLElementTimeTypeEnum.TE_type, _tisglr, lr.FaseVan, lr.FaseNaar));
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgfo}{lr:vannaar}", lr.LateReleaseOntruimingstijd, CCOLElementTimeTypeEnum.TE_type, _tisgfo, lr.FaseVan, lr.FaseNaar));
            }

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);

                if (fc1 == null || fc2 == null) return;

                if (fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger &&
                    (nl.InrijdenTijdensGroen || nl.MaximaleVoorstart.HasValue))
                {
                    _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_hisglos}{fc1.Naam}", _hisglos, fc1.Naam));

                    _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisginl}{fc1.Naam}{fc2.Naam}",
                        nl.MaximaleVoorstart ?? 0, CCOLElementTimeTypeEnum.TE_type,
                        _tisginl, fc1.Naam));
                }
            }

            var helps = new List<string>();

            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen))
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tisgxnl}{nl:vannaar}",
                        nl.MaximaleVoorstart ?? 0, CCOLElementTimeTypeEnum.TE_type,
                        _tisgxnl, nl.FaseVan, nl.FaseNaar));
            }

            foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x =>
                        c.Fasen.Any(x2 => x2.Naam == x.FaseVan && x2.Type == FaseTypeEnum.Voetganger) &&
                        c.Fasen.Any(x2 => x2.Naam == x.FaseNaar && x2.Type == FaseTypeEnum.Voetganger) &&
                        (x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen)))
            {
                var fc = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                if (fc == null) continue;
                //var dBinnen = fc.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);
                //if (dBinnen != null)
                //{
                //    _myElements.Add(
                //    CCOLGeneratorSettingsProvider.Default.CreateElement(
                //        $"{_hisgmad}{dBinnen.Naam}",
                //        nl.MaximaleVoorstart ?? 0, CCOLElementTimeTypeEnum.TE_type,
                //        _hisgmad, dBinnen.Naam));
                //}

                if (!helps.Contains($"s{_schisglos}{nl.FaseVan}_1"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schisglos}{nl.FaseVan}_1", 0, CCOLElementTimeTypeEnum.SCH_type, _schisglos, nl.FaseVan));
                    helps.Add($"s{_schisglos}{nl.FaseVan}_1");
                }
                if (!helps.Contains($"s{_schisglos}{nl.FaseVan}_2"))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schisglos}{nl.FaseVan}_2", 0, CCOLElementTimeTypeEnum.SCH_type, _schisglos, nl.FaseVan));
                    helps.Add($"s{_schisglos}{nl.FaseVan}_2");
                }
            }

            if (c.HasPTorHD())
            {
                foreach (var sg in c.Fasen)
                {
                    sg.Interfunc = true;
                    sg.IsgTijdBitmapData.Multivalent = true;
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usisgtijd}{sg.Naam}", _usisgtijd, sg.IsgTijdBitmapData, sg.Naam));
                }
            }

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return new List<CCOLLocalVariable>
                    {
                        new("int", "fc"),
                    };
                case CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijden:
                    return new List<CCOLLocalVariable>
                    {
                        new(c.GetBoolV(), "wijziging", "TRUE"),
                    };
                case CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijdenPrio:
                    return new List<CCOLLocalVariable>
                    {
                        new(c.GetBoolV(), "wijziging", "TRUE"),
                    };
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    return new List<CCOLLocalVariable>
                    {
                        new("count", "i"),
                        new("count", "j"),
                        new(c.GetBoolV(), "wijziging", "TRUE"),
                    };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new[] { 140 },
                CCOLCodeTypeEnum.RegCTop=> new[] { 140 },
                CCOLCodeTypeEnum.RegCVerlenggroen => new[] { 25, 90, 130, 140 },
                CCOLCodeTypeEnum.RegCMaxgroen => new[] { 90, 130, 140 },
                CCOLCodeTypeEnum.RegCInitApplication => new[] { 140 },
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => new[] { 10 },
                CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijden => new[] { 10 },
                CCOLCodeTypeEnum.RegCRealisatieAfhandeling => new[] { 150 },
                CCOLCodeTypeEnum.RegCMeeverlengen => new[] { 40 },
                CCOLCodeTypeEnum.TabCIncludes => new[] { 140 },
                
                CCOLCodeTypeEnum.RegCPreApplication => new[] { 140 },
                CCOLCodeTypeEnum.RegCAanvragen => new[] { 140 },
                CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijdenPrio => new[] { 140 },
                CCOLCodeTypeEnum.RegCMeetkriterium => new[] { 140 },
                CCOLCodeTypeEnum.RegCPostApplication => new[] { 140 },
                
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();
            var f = true;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    if (!c.HasPTorHD()) return "";
                    sb.AppendLine($"{ts}BepaalVolgrichtingen();");
                    sb.AppendLine($"{ts}PrioMeetKriteriumISG();");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}ResetIsgVars(); /* zet alle interstartgroentijden op -1 @@ niet isgfunc_prio.c maar isgfunc.c @@ @@ aanroepen net voor bepalen isg tijden en niet hier @@ */");
                    if (!c.HasPTorHD()) return "";
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Aanroepen tbv prioriteitsingrepen */");
                    sb.AppendLine($"{ts}RijTijdScenario();");
                    sb.AppendLine($"{ts}RijTijdScenario_Add();");
                    sb.AppendLine($"{ts}InUitMelden();");
                    sb.AppendLine($"{ts}InUitMelden_Add();");
                    sb.AppendLine($"{ts}PrioInstellingen();");
                    sb.AppendLine($"{ts}PrioInstellingen_Add();");
                    sb.AppendLine($"{ts}PrioTimers();");
                    sb.AppendLine($"{ts}WachtTijdBewaking();");
                    sb.AppendLine($"{ts}WachtTijdBewaking_Add();");
                    sb.AppendLine($"{ts}OnderMaximum();");
                    sb.AppendLine($"{ts}OnderMaximumExtra();");
                    sb.AppendLine($"{ts}OnderMaximum_Add();");
                    sb.AppendLine($"{ts}BlokkeringsTijd();");
                    sb.AppendLine($"{ts}BlokkeringsTijd_Add();");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Aanroepen tbv prioriteitsingrepen ISG */");
                    sb.AppendLine($"{ts}PrioriteitsToekenning_ISG(); /* prioGKFC_MAX *gewijzigd naar prioFKFC_MAX tov functie in prio.c */");
                    sb.AppendLine($"{ts}PrioriteitsToekenning_ISG_Add(); /*@@ deze functie aanroep laten staan maar de functie zelf moet in moet in de 123456prio_add file @@ */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCAanvragen:
                    if (!c.HasPTorHD()) return "";
                    sb.AppendLine($"{ts}/* openbaar vervoer aanvragen */");
                    sb.AppendLine($"{ts}PrioAanvragen();");

                    f = true;
                    foreach (var sync in c.GetAllSynchronisations(false))
                    {
                        if (f)
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* ISG deelconflict aanvragen; tevens wordt de AA en BR opgezet @@ tijdelijk tot we code hebben dat de deelconflicten niet hard mee moeten komen @@ @@ aanvraag hoort hier wel maar de AA en BR op een andere plek plaatsen @@*/");
                            f = false;
                        }

                        switch (sync)
                        {
                            case VoorstartModel vs:
                                sb.AppendLine($"{ts}PrioMeerealisatieDeelconflictVoorstart({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisgvs}{vs:vannaar});");
                                break;
                            case LateReleaseModel lr:
                                sb.AppendLine($"{ts}PrioMeerealisatieDeelconflictLateRelease({_fcpf}{lr:van}, {_fcpf}{lr:naar}, {_tpf}{_tisglr}{lr:vannaar});");
                                break;
                            case GelijkstartModel gs:
                                break;
                        }
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijdenPrio:
                    sb.AppendLine($"{ts}VulHardEnGroenConflictenInPrioVars();");
                    if (c.InterSignaalGroep.Nalopen.Count > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Pas interstartgroentijden aan a.g.v. nalopen */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            var tnlfg = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroen) ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                            var tnlfgd = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.VastGroenDetectie) ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:
                                    var tnlsg = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroen) ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                    var tnlsgd = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.StartGroenDetectie) ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}InterStartGroenTijd_NLSG_PRIO({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {tnlsg}, {tnlsgd});");
                                    break;
                                case NaloopTypeEnum.EindeGroen:
                                    var tnleg = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroen) ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                    var tnlegd = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeGroenDetectie) ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}InterStartGroenTijd_NLEG_PRIO({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {tnlfg}, {tnlfgd}, {tnleg}, {tnlegd}, {_tpf}{_tvgnaloop}{nl:vannaar});");
                                    break;
                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    var tnlcv = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroen) ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                                    var tnlcvd = nl.Tijden.Any(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroenDetectie) ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}InterStartGroenTijd_NLEVG_PRIO({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {tnlfg}, {tnlfgd}, {tnlcv}, {tnlcvd}, {_tpf}{_tvgnaloop}{nl:vannaar});");
                                    break;
                            }
                        }
                    }

                    foreach (var sync in c.GetAllSynchronisations(false))
                    {
                        switch (sync)
                        {
                            case VoorstartModel vs:
                                sb.AppendLine($"{ts}InterStartGroentijd_MeeverlengenDeelconflict_PRIO({_fcpf}{vs:naar}, {_fcpf}{vs:van});");
                                break;
                            case LateReleaseModel lr:
                                sb.AppendLine($"{ts}InterStartGroentijd_MeeverlengenDeelconflict_PRIO({_fcpf}{lr:naar}, {_fcpf}{lr:van});");
                                break;
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}do");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");

                    sb.AppendLine($"{ts}{ts}/* Pas realisatietijden aan a.g.v. nalopen */");
                    foreach (var sync in c.GetAllSynchronisations())
                    {
                        switch (sync)
                        {
                            case VoorstartModel vs:
                                sb.AppendLine($"{ts}{ts}wijziging |= TISG_Voorstart_PRIO_Correctie({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisgvs}{vs:vannaar});");
                                break;
                            case LateReleaseModel lr:
                                sb.AppendLine($"{ts}{ts}wijziging |= TISG_LateRelease_PRIO_Correctie({_fcpf}{lr:van}, {_fcpf}{lr:naar}, {_tpf}{_tisglr}{lr:vannaar});");
                                break;
                            case NaloopModel nl when nl.Type == NaloopTypeEnum.EindeGroen:
                                sb.AppendLine($"{ts}{ts}wijziging |= TISG_LateRelease_PRIO_Correctie({_fcpf}{nl:naar}, {_fcpf}{nl:van}, {_tpf}{_tisgxnl}{nl:vannaar});");
                                break;
                            case GelijkstartModel gs:
                                sb.Append($"{ts}{ts}");
                                if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                                {
                                    sb.Append($"if (SCH[{_schpf}{_schgs}{gs:vannaar}]) ");
                                }
                                sb.AppendLine($"wijziging |= TISG_Gelijkstart_PRIO_Correctie({_fcpf}{gs:van}, {_fcpf}{gs:naar});");
                                break;
                        }
                    }

                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x =>
                        c.Fasen.Any(x2 => x2.Naam == x.FaseVan && x2.Type == FaseTypeEnum.Voetganger) &&
                        c.Fasen.Any(x2 => x2.Naam == x.FaseNaar && x2.Type == FaseTypeEnum.Voetganger) &&
                        (x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen)))
                    {
                        var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                        if (fc1 == null) continue;
                        sb.AppendLine($"{ts}{ts}wijziging |= TISG_LateRelease_PRIO_Correctie({_fcpf}{nl:naar}, {_fcpf}{nl:van}, {_tpf}{_tisginl}{nl:vannaar});");
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}wijziging |= Correctie_InterStartGroentijdTijd_PRIO_Add();");

                    sb.AppendLine($"{ts}}} while (wijziging);");

                    return sb.ToString();

                case CCOLCodeTypeEnum.TabCIncludes:
                    sb.AppendLine("#include \"isgfunc.h\" /* Interstartgroenfuncties */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("#include \"isgfunc.c\" /* Interstartgroen functies */");
                    
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (!c.HasPTorHD()) return "";

                    sb.AppendLine($"{ts}PrioCcol();");
                    foreach (var sg in c.Fasen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usisgtijd}{sg.Naam}] = ((twacht[{_fcpf}{sg.Naam}] * (A[{_fcpf}{sg.Naam}] || PRIOFC[{_fcpf}{sg.Naam}]) * R[{_fcpf}{sg.Naam}]) >= 0) ? (twacht[{_fcpf}{sg.Naam}] * (A[{_fcpf}{sg.Naam}] || PRIOFC[{_fcpf}{sg.Naam}]) * R[{_fcpf}{sg.Naam}]) : 0;");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine($"{c.GetBoolV()} init_tvg;");
                    sb.AppendLine($"{c.GetBoolV()} PAR_los[FCMAX] = {{ 0 }};");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    if (order == 25)
                    {
                        sb.AppendLine($"{ts}/* Bepaal de minimale maximale verlengroentijd bij alternatieve realisaties */");
                        foreach (var fc in c.Fasen)
                        {
                            sb.AppendLine($"{ts}TVG_AR[{_fcpf}{fc.Naam}] = ((PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}]) >= 0) ? PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}] : NG;");
                        }
                    }

                    if (order == 90)
                    {
                        sb.AppendLine($"{ts}/* TVG_max alleen aanpassen op EVG van de primaire richting of tijdens initialisatie */");
                        sb.AppendLine($"{ts}IsgCorrectieTvgPrTvgMax();");
                        sb.AppendLine($"{ts}init_tvg = FALSE;");

                        if (c.HasPTorHD())
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}ResetNietGroentijdOphogen(); /* @@ verplaatsen naar isgfunc.c @@ */");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* groentijd conflict volgrichting  mag niet opgehoogd worden tijdens inlopen @@ verplaatsen naar isgfunc.c @@*/");
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                sb.AppendLine($"{ts}VerhoogGroentijdNietTijdensInrijden({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_tpf}{_tisgxnl}{nl:vannaar});");
                            }
                        }
                    }
                    
                    if (order == 130 && c.HasPTorHD())
                    {
                        sb.AppendLine($"{ts}AfkapGroen();");
                        sb.AppendLine($"{ts}AfkapGroenExtra();");
                        sb.AppendLine($"#ifdef PRIO_ADDFILE");
                        sb.AppendLine($"{ts}AfkapGroen_Add();");
                        sb.AppendLine($"#endif /* PRIO_ADDFILE */");
                        sb.AppendLine($"");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}TVG_afkap[fc] = ((iAfkapGroenTijd[fc] - TFG_max[fc]) > 0) ? iAfkapGroenTijd[fc] - TFG_max[fc] : 0; /* Vullen groentijd die gemaakt mag worden als er een prioriteitsingreep is */");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine($"{ts}BepaalTVG_BR(); /* Maximale verlenggroentijd bijzondere realisatie als deze nog niet groen is */");
                        sb.AppendLine($"{ts}VerhoogTVG_maxDoorPrio(); /* Voldoende verlenggroentijd om prioriteitsrealisatie te faciliteren */");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}TVG_max_voor_afkap[fc] = TVG_max[fc];");
                        sb.AppendLine($"{ts}{ts}TVG_AR_voor_afkap[fc] = TVG_AR[fc];");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine($"{ts}VerlaagTVG_maxDoorConfPrio(); /* Geef richtingen minder groen door conflicterende prioriteitsrealisatie */");
                        sb.AppendLine($"{ts}/* Niet verhogen TVG_max tijdens groen. Bijvoorbeeld als prioriteitsingreep of file ingreep wegvalt @@ Peter past commentaar nog aan @@ */");
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}if (SG[fc])");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}TVG_old[fc] = TVG_max[fc];");
                        sb.AppendLine($"{ts}{ts}{ts}TVG_AR_old[fc] = TVG_AR[fc];");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}if (G[fc] && !MG[fc])");
                        sb.AppendLine($"{ts}{ts}{{");
                        sb.AppendLine($"{ts}{ts}{ts}if ((TVG_max[fc] > TVG_old[fc]) && !TVG_max_opgehoogd[fc]) TVG_max[fc] = TVG_old[fc];");
                        sb.AppendLine($"{ts}{ts}{ts}if ((TVG_AR[fc] > TVG_AR_old[fc]) && !TVG_AR_opgehoogd[fc]) TVG_AR[fc] = TVG_AR_old[fc];");
                        sb.AppendLine($"{ts}{ts}{ts}TVG_AR_old[fc] = TVG_AR[fc];");
                        sb.AppendLine($"{ts}{ts}{ts}TVG_old[fc] = TVG_max[fc];");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}BepaalRealisatieTijden();");
                        sb.AppendLine($"");
                        sb.AppendLine($"{ts}BepaalStartGroenMomentenPrioIngrepen(); /* bepaal wanneer prioriteitsrealisatie mag komen */");
                        sb.AppendLine($"{ts}PasTVG_maxAanStartGroenMomentenPrioIngrepen(); /* pas de verlenggroentijden hier weer op aan*/");
                    }

                    if (order == 140)
                    {
                        sb.AppendLine($"/* TVG_max nalooprichting ophogen als naloop niet past */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:
                                    var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                    var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}NaloopVtg_TVG_Correctie({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_hpf}{_hnlsg}{nl:vannaar}, {nlsg}, {nlsgd});");
                                    break;
                                case NaloopTypeEnum.EindeGroen:
                                    var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                    var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                    var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                    var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}NaloopEG_TVG_Correctie({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                                    break;
                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    var nlfg2 = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                    var nlfgd2 = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                    var nlcv = nl.VasteNaloop ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                                    var nlcvd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}NaloopEG_TVG_Correctie({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg2}, {nlfgd2}, {nlcv}, {nlcvd}, {_tpf}vgnaloop{nl:vannaar});");
                                    break;
                            }
                        }

                        sb.AppendLine($"{ts}BepaalRealisatieTijden();");
                        sb.AppendLine($"{ts}BepaalInterStartGroenTijden();");

                        if (c.HasPTorHD())
                        {
                            sb.AppendLine($"{ts}BepaalInterStartGroenTijden_PRIO();");
                            sb.AppendLine($"{ts}PrioTegenhoudenISG(); /* Houdt richtingen die conflicterend zijn met priorealisatie als er niet meer genoeg ruimte voor realisatie is  */");
                            sb.AppendLine($"{ts}PasRealisatieTijdenAanVanwegeRRPrio(); /* Pas realisatietijden aan voor richtingen conflicterend met prioriteitsrealisatie*/");
                            sb.AppendLine($"{ts}Bepaal_Realisatietijd_per_richting();");
                            foreach (var lr in c.InterSignaalGroep.LateReleases)
                            {
                                sb.AppendLine($"{ts}PasRealisatieTijdenAanVanwegeBRLateRelease({_fcpf}{lr:van});");
                            }
                            sb.AppendLine($"{ts}Bepaal_Realisatietijd_per_richting();");
                        }
                    }

                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}init_tvg = TRUE;");
                    if (c.HasPTorHD())
                    {
                        sb.AppendLine($"{ts}PrioInit_ISG(); /* initialisatie prioriteits instellingen @@ functienaam wijzigen / ISG weghalen @@ */");
                        sb.AppendLine($"{ts}PrioInitExtra(); /* initialisatie variabelen vertraag_kar_uitm */");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    sb.AppendLine($"{ts}/* TIGR */");
                    sb.AppendLine($"{ts}/* Correctie tijdens omdat bv fc22 hard meeverlengt met fc05 en dus bij naloop fc22-->fc21 deze maatgevend kan worden. */");
                    sb.AppendLine($"{ts}BepaalIntergroenTijden(); /* initialisatie TIGR[][] */");
                    sb.AppendLine();

                    f = true;
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
                    {
                        if (f)
                        {
                            sb.AppendLine($"{ts}/* Neem EG-naloopconflicten ook mee in TIGR[][] */");
                            f = false;
                        }

                        var nleg = nl.Type == NaloopTypeEnum.EindeGroen 
                            ? nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG"
                            : nl.VasteNaloop ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                        var nlegd = nl.Type == NaloopTypeEnum.EindeGroen
                            ? nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG"
                            : nl.DetectieAfhankelijk ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}corrigeerTIGRvoorNalopen({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                    }
                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* Realisatietijden */");
                    sb.AppendLine($"{ts}InitRealisatieTijden(); /* initialisatie REALISATIETIJD[][] */");
                    sb.AppendLine($"{ts}RealisatieTijden_VulHardeConflictenIn();");
                    sb.AppendLine($"{ts}RealisatieTijden_VulGroenGroenConflictenIn(); /* @@ in principe zijn er geen groen groen conflicten @@*/");
                    sb.AppendLine($"{ts}CorrigeerRealisatieTijdenObvGarantieTijden(); /* een richting mag na groen niet direct weer realiseren (eerst GL en TRG) */");
                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v. nalopen */");
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                        var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                        var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                        var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}Realisatietijd_NLEG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                    }
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.CyclischVerlengGroen))
                    {
                        var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                        var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                        var nlcv = nl.VasteNaloop ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                        var nlcvd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}Realisatietijd_NLEVG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nlcv}, {nlcvd}, {_tpf}vgnaloop{nl:vannaar});");
                    }
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.StartGroen))
                    {
                        var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                        var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}Realisatietijd_NLSG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlsg}, {nlsgd});");
                    }
                    foreach (var fc in c.Fasen.Where(x => x.HardMeeverlengenFaseCycli.Any()))
                    {
                        foreach (var hmfc in fc.HardMeeverlengenFaseCycli)
                        {
                            sb.AppendLine($"{ts}Realisatietijd_HardMeeverlengenDeelconflict({_fcpf}{hmfc.FaseCyclus}, {_fcpf}{fc.Naam});");
                        }
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v ontruimende deelconflicten */");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict))
                    {
                        sb.Append($"{ts}");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schgs}{gs:vannaar}]) ");
                        }
                        sb.AppendLine($"Realisatietijd_Ontruiming_Gelijkstart({_fcpf}{gs:naar}, {_fcpf}{gs:van}, {_tpf}{_tisgfo}{gs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}Realisatietijd_Ontruiming_Voorstart({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_tisgfo}{vs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}Realisatietijd_Ontruiming_LateRelease({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_tisglr}{vs:vannaar}, {_tpf}{_tisgfo}{vs:vannaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v. deelconflicten/voorstarts die nog groen moeten worden */");
                    sb.AppendLine($"{ts}do");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.Append($"{ts}");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schgs}{gs:vannaar}]) ");
                        }
                        sb.AppendLine($"wijziging |= Realisatietijd_Gelijkstart_Correctie({_fcpf}{gs:naar}, {_fcpf}{gs:van});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}wijziging |= Realisatietijd_Voorstart_Correctie({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisgvs}{vs:vannaar});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}wijziging |= Realisatietijd_LateRelease_Correctie({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisglr}{vs:vannaar});");
                    }
                    sb.AppendLine();

                    if (c.InterSignaalGroep.Nalopen.Count(x => x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen) > 0)
                    {
                        sb.AppendLine($"{ts}/* Inlopen / inrijden nalopen */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen))
                        {
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:
                                //    sb.AppendLine($"{ts}wijziging |= Correctie_REALISATIETIJD_LateRelease({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_tpf}{_tisgxnl}{nl:naarvan});");
                                //    break;
                                case NaloopTypeEnum.EindeGroen:
                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    sb.AppendLine($"{ts}wijziging |= Realisatietijd_LateRelease_Correctie({_fcpf}{nl:naar}, {_fcpf}{nl:van}, {_tpf}{_tisgxnl}{nl:vannaar});");
                                    break;
                            }
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine($"{ts}{ts}wijziging |= CorrectieRealisatieTijd_Add();");
                    sb.AppendLine($"{ts}}} while (wijziging);");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}Bepaal_Realisatietijd_per_richting(); /* bepaal de maximale realisatietijd voor een richting */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijden:

                    sb.AppendLine($"{ts}InitInterStartGroenTijden();");
                    sb.AppendLine($"{ts}InterStartGroenTijden_VulHardeConflictenIn();");
                    sb.AppendLine($"{ts}InterStartGroenTijden_VulGroenGroenConflictenIn();");

                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* Pas interstartgroentijden aan a.g.v. nalopen */");

                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        switch (nl.Type)
                        {
                            case NaloopTypeEnum.StartGroen:
                                var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}InterStartGroenTijd_NLSG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlsg}, {nlsgd});");
                                break;
                            case NaloopTypeEnum.EindeGroen:
                                var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}InterStartGroenTijd_NLEG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                                break;
                            case NaloopTypeEnum.CyclischVerlengGroen:
                                var nlfg1 = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                var nlfgd1 = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                var nlcv = nl.VasteNaloop ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                                var nlcvd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}InterStartGroenTijd_NLEVG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg1}, {nlfgd1}, {nlcv}, {nlcvd}, {_tpf}vgnaloop{nl:vannaar});");
                                break;
                        }
                    }

                    foreach (var fc in c.Fasen.Where(x => x.HardMeeverlengenFaseCycli.Any()))
                    {
                        foreach (var hmfc in fc.HardMeeverlengenFaseCycli)
                        {
                            sb.AppendLine($"{ts}InterStartGroentijd_HardMeeverlengenDeelconflict({_fcpf}{hmfc.FaseCyclus}, {_fcpf}{fc.Naam});");
                        }
                    }

                    sb.AppendLine();

                    sb.AppendLine($"{ts}do");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.Append($"{ts}");
                        if (gs.Schakelbaar != AltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"if (SCH[{_schpf}{_schgs}{gs:vannaar}]) ");
                        }
                        sb.AppendLine($"{ts}{ts}wijziging |= InterStartGroenTijd_Gelijkstart_Correctie({_fcpf}{gs:naar}, {_fcpf}{gs:van});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}{ts}wijziging |= InterStartGroenTijd_Voorstart_Correctie({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisgvs}{vs:vannaar});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}{ts}wijziging |= InterStartGroenTijd_LateRelease_Correctie({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tisglr}{vs:vannaar});");
                    }

                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Inlopen / inrijden */");
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen))
                    {
                        sb.AppendLine($"{ts}{ts}wijziging |= InterStartGroenTijd_LateRelease_Correctie({_fcpf}{nl:naar}, {_fcpf}{nl:van}, {_tpf}{_tisgxnl}{nl:vannaar});");
                    }
                    sb.AppendLine();

                    sb.AppendLine($"{ts}{ts}wijziging |= Correctie_TISG_add();");
                    sb.AppendLine($"{ts}}} while (wijziging);");

                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMeeverlengen:
                    sb.AppendLine($"{ts}MeeverlengenUitDoorPrio();");
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x =>
                        c.Fasen.Any(x2 => x2.Naam == x.FaseVan && x2.Type == FaseTypeEnum.Voetganger) &&
                        c.Fasen.Any(x2 => x2.Naam == x.FaseNaar && x2.Type == FaseTypeEnum.Voetganger) &&
                        (x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen)))
                    {
                        var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                        if (fc1 == null) continue;
                        var insideDp = fc1.Detectoren.FirstOrDefault(x => x.Type == DetectorTypeEnum.KnopBinnen);
                        if (insideDp == null) continue;
                        sb.AppendLine($"{ts}MeeverlengenUitDoorVoetgangerLos({_fcpf}{nl:van}, {_hpf}{_hmad}{insideDp.Naam});");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _prmfperc = CCOLGeneratorSettingsProvider.Default.GetElementName("prmfperc");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _hmad = CCOLGeneratorSettingsProvider.Default.GetElementName("hmad");
            _prmxnl = CCOLGeneratorSettingsProvider.Default.GetElementName("prmxnl");
            _tvgnaloop = CCOLGeneratorSettingsProvider.Default.GetElementName("tvgnaloop");
            _schgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schgs");
            _hnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("hnlsg");

            return base.SetSettings(settings);
        }
    }

}
