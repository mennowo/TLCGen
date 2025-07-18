﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class VersneldPrimairAlternatievenCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _prmmlfpr;
        private CCOLGeneratorCodeStringSettingModel _prmaltg;
        private CCOLGeneratorCodeStringSettingModel _prmaltp;
        private CCOLGeneratorCodeStringSettingModel _schaltg;
#pragma warning restore 0649
        private string _tnlsg;
        private string _tnlfg;
        private string _tnlcv;
        private string _tnleg;
        private string _tnlsgd;
        private string _tnlfgd;
        private string _tnlcvd;
        private string _tnlegd;
	    private string _hfile;
	    private string _schgs;
	    private string _schrealgs;
	    private string _hlos;
	    private string _mar;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);

            foreach (var fc in c.ModuleMolen.FasenModuleData)
            {
                // Vooruit realisaties
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmmlfpr}{fc.FaseCyclus}",
                        fc.ModulenVooruit,
                        CCOLElementTimeTypeEnum.None,
                        _prmmlfpr,
                        fc.FaseCyclus));
            }

            if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc)
            {
                var groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
                var sortedSyncs = GroenSyncDataModel.OrderSyncs(c, groenSyncData);
                foreach (var nl in c.InterSignaalGroep.Nalopen)
                {
                    // Only do this for pedestrians with sync
                    var sync = sortedSyncs.twoWayPedestrians?.FirstOrDefault(x =>
                        x.m1.FaseVan == nl.FaseVan && x.m1.FaseNaar == nl.FaseNaar
                        || x.m1.FaseVan == nl.FaseNaar && x.m1.FaseNaar == nl.FaseVan);
                    if (sync == null) continue;

                    var hnl = nl.Type switch
                    {
                        NaloopTypeEnum.StartGroen => _tnlsg,
                        NaloopTypeEnum.EindeGroen => _tnleg,
                        NaloopTypeEnum.CyclischVerlengGroen => _tnlcv,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{hnl}{nl.FaseVan}{nl.FaseNaar}", 0, CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.HulpElement, "",
                            null, null));
                }
            }

            // Alternatieven
            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                foreach (var fc in c.ModuleMolen.FasenModuleData)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmaltg}{fc.FaseCyclus}",
                            fc.AlternatieveGroenTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _prmaltg, fc.FaseCyclus));

                    // For prmaltp and schaltg: combine if the signal group is part of a simultaneous synchronisation
                    var gs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == fc.FaseCyclus);

                    if (gs != null)
                    {
                        var namealtp = _prmaltp + string.Join(string.Empty, gs.Item2);
                        var namealtg = _schaltg + string.Join(string.Empty, gs.Item2);
                        var containsaltp = _myElements.Any(i => i.Naam == namealtp);
                        var containsaltg = _myElements.Any(i => i.Naam == namealtg);
                        if (!containsaltp)
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    namealtp, fc.AlternatieveRuimte, CCOLElementTimeTypeEnum.TE_type, _prmaltp, "fasen", string.Join(", ", gs.Item2)));
                        }
                        if (!containsaltg)
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    namealtg, fc.AlternatiefToestaan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schaltg, "fasen", string.Join(", ", gs.Item2)));
                        }
                    }
                    else
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmaltp}{fc.FaseCyclus}", fc.AlternatieveRuimte, CCOLElementTimeTypeEnum.TE_type, _prmaltp, "fase", fc.FaseCyclus));
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_schaltg}{fc.FaseCyclus}", fc.AlternatiefToestaan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schaltg, "fase", fc.FaseCyclus));
                    }
                }
            }
            else
            {
                // Build a list of dictionaries for signalgroups that may realize alternatively;
                // each dictionary contains alternative signalgroups as keys, and a list of primary
                // signalgroups under whose CV the alternative may realise
                var altsdict =
                    new List<Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>>();
                foreach (var ml in c.ModuleMolen.Modules)
                {
                    var altdict =
                        new Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>();
                    foreach (var mlfc in ml.Fasen)
                    {
                        foreach (var amlfc in mlfc.Alternatieven)
                        {
                            if (!altdict.ContainsKey(amlfc.FaseCyclus))
                            {
                                altdict.Add(
                                    amlfc.FaseCyclus, 
                                    new List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>
                                    {
                                        new(amlfc, mlfc)
                                    });
                            }
                        }
                    }
                    altsdict.Add(altdict);
                }
                var mlidx = 1;
                foreach (var alts in altsdict)
                {
                    foreach (var altg in alts)
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmaltg}{mlidx}{altg.Key}",
                                altg.Value.First().Item1.AlternatieveGroenTijd,
                                CCOLElementTimeTypeEnum.TE_type,
                                _prmaltg, altg.Key, "ML" + mlidx));
                    }
                    ++mlidx;
                }

                /* not supported yet
                foreach (var r in c.MultiModuleMolens)
                {
                    foreach (var ml in r.Modules)
                    {
                        var altdict =
                            new Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>();
                        foreach (var mlfc in ml.Fasen)
                        {
                            foreach (var amlfc in mlfc.Alternatieven)
                            {
                                if (!altdict.ContainsKey(amlfc.FaseCyclus))
                                {
                                    altdict.Add(
                                        amlfc.FaseCyclus,
                                        new List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>
                                        {
                                        new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc, mlfc)
                                        });
                                }
                            }
                        }
                        altsdict.Add(altdict);
                    }
                    var mlidx2 = 1;
                    foreach (var alts in altsdict)
                    {
                        foreach (var altg in alts)
                        {
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement(
                                    $"{_prmaltg}{mlidx}{altg.Key}",
                                    altg.Value.First().Item1.AlternatieveGroenTijd,
                                    CCOLElementTimeTypeEnum.TE_type,
                                    _prmaltg, altg.Key, r.Reeks + mlidx));
                        }
                        ++mlidx2;
                    }
                }
                */
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules => new List<CCOLLocalVariable>{new("int", "fc")},
                CCOLCodeTypeEnum.PrioCPARCorrecties => 
                    c.InterSignaalGroep.Gelijkstarten.Any() || c.InterSignaalGroep.Nalopen.Any()
                        ? new List<CCOLLocalVariable>{new("int", "fc")}
                        : base.GetFunctionLocalVariables(c, type),
                _ => base.GetFunctionLocalVariables(c, type)
            };
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules => new []{10},
                CCOLCodeTypeEnum.HstCAlternatief => new []{10},
                CCOLCodeTypeEnum.PrioCPARCorrecties => new []{10},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            var molens = new List<ModuleMolenModel> { c.ModuleMolen };
            if (c.Data.MultiModuleReeksen)
            {
                molens = c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())).ToList();
            }

            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCPARCorrecties:
                    if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc) sb.Append(GetRealFuncPARCorrections(c, ts, false));
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules:
                    sb.AppendLine($"{ts}/* versnelde primaire realisaties */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    sb.AppendLine($"{ts}/* periode versnelde primaire realisatie - aantal modulen vooruit */");
                    sb.AppendLine($"{ts}/* -------------------------------------------------------------- */");
                    // NOTE: all fasen are present in the list in the ML module. others are empty
                    // this accounts for all places below where FasenModuleData is references
                    foreach (var fc in c.ModuleMolen.FasenModuleData)
                    {
                        var r = molens.FirstOrDefault(x => x.Modules.Any(x2 => x2.Fasen.Any(x3 => x3.FaseCyclus == fc.FaseCyclus)));
                        if(r != null)
                        {
                            sb.AppendLine($"{ts}PFPR[{_fcpf}{fc.FaseCyclus}] = ml_fpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmlfpr}{fc.FaseCyclus}], PR{r.Reeks}, {r.Reeks}, {r.Reeks}MAX);");
                        }
                    }
                    sb.AppendLine("");

                    foreach (var gen in CCOLGenerator.OrderedPieceGenerators[CCOLCodeTypeEnum.RegCRealisatieAfhandelingVersneldPrimair])
                    {
                        sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCRealisatieAfhandelingVersneldPrimair, ts, order));
                        sb.AppendLine();
                    }

                    sb.AppendLine($"{ts}VersneldPrimair_Add();");
                    sb.AppendLine("");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{{");
                    foreach (var r in molens)
                    {
                        sb.AppendLine($"{ts}{ts}set_FPRML_fk_gkl(fc, PR{r.Reeks}, {r.Reeks}, {r.Reeks}MAX, ({c.GetBoolV()})PFPR[fc]);");
                    }
                    sb.AppendLine($"{ts}}}");
                    sb.AppendLine();
                    if (c.ModuleMolen.LangstWachtendeAlternatief)
                    {
                        sb.AppendLine($"{ts}/* langstwachtende alternatieve realisatie */");
                        sb.AppendLine($"{ts}/* --------------------------------------- */");
                        sb.AppendLine();

                        foreach (var r in molens)
                        {
                            sb.AppendLine($"{ts}afsluiten_aanvraaggebied_pr(PR{r.Reeks}, {r.Reeks});");
                        }
                        sb.AppendLine();
                        sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* zet richtingen die alternatief gaan realiseren         */");
                        sb.AppendLine($"{ts}/* terug naar RV als er geen alternatieve ruimte meer is. */");

                        if (c.Fasen.Any(x => x.WachttijdVoorspeller && molens.Any(x2 => x2.FasenModuleData.Any(x3 => x3.FaseCyclus == x.Naam))))
                        {
                            sb.AppendLine($"{ts}/* Dit gebeurt niet voor fasen met een wachttijd voorspeller, */");
                            sb.AppendLine($"{ts}/* of fasen waarvan de voedende richting die heeft. */");
                        }
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            // find signalgroup instance
                            var ffc = c.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);
                            // if the sg has no predictor
                            if (ffc is {WachttijdVoorspeller: false})
                            {
                                // find a potential feeding sg
                                var fcnl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseNaar == fc.FaseCyclus);
                                // if there is a feeding sg, set the sg instance to that sg
                                if (fcnl != null) ffc = c.Fasen.FirstOrDefault(x => x.Naam == fcnl.FaseVan);
                            }
                            // if the instance is not null, and it has a predictor, skip setting RR
                            if (ffc is {WachttijdVoorspeller: true}) continue;
                            sb.AppendLine(
                                $"{ts}RR[{_fcpf}{fc.FaseCyclus}] |= R[{_fcpf}{fc.FaseCyclus}] && AR[{_fcpf}{fc.FaseCyclus}] && (!PAR[{_fcpf}{fc.FaseCyclus}] || ERA[{_fcpf}{fc.FaseCyclus}]) ? BIT5 : 0;");
                        }
                        sb.AppendLine();

                        List<Tuple<string, List<string>>> gelijkstarttuples = null;
                        gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                        var yes = false;
                        
                        if (c.InterSignaalGroep.Nalopen.Any())
                        {
                            sb.AppendLine();
                            sb.AppendLine($"{ts}/* Niet intrekken alternatief nalooprichting tijdens inlopen voedende richting */");
                            foreach (var fc in c.Fasen)
                            {
                                // zoeken naloop waarvan deze fase de naloop richting is
                                var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseNaar == fc.Naam);
                                if (nl != null)
                                {
                                    sb.Append($"{ts}if (");
                                    var first = true;
                                    foreach (var nlt in nl.Tijden)
                                    {
                                        if (!first) sb.Append(" || ");
                                        first = false;
                                        var tnl = nlt.Type switch
                                        {
                                            NaloopTijdTypeEnum.StartGroen => _tnlsg,
                                            NaloopTijdTypeEnum.StartGroenDetectie => _tnlsgd,
                                            NaloopTijdTypeEnum.VastGroen => _tnlfg,
                                            NaloopTijdTypeEnum.VastGroenDetectie => _tnlfgd,
                                            NaloopTijdTypeEnum.EindeGroen => _tnleg,
                                            NaloopTijdTypeEnum.EindeGroenDetectie => _tnlegd,
                                            NaloopTijdTypeEnum.EindeVerlengGroen => _tnlcv,
                                            NaloopTijdTypeEnum.EindeVerlengGroenDetectie => _tnlcvd,
                                            _ => throw new ArgumentOutOfRangeException()
                                        };
                                        sb.Append($"RT[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}] || T[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}]");
                                    }
                                    sb.AppendLine(")");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~BIT5;");
                                    sb.AppendLine($"{ts}}}");
                                }
                            }
                        }

                        if (c.TimingsData.TimingsToepassen && c.Data.CCOLVersie >= CCOLVersieEnum.CCOL110)
                        {
                            sb.AppendLine($"#ifndef NO_TIMETOX");
                            foreach (var sg in c.Fasen)
                            {
                                sb.AppendLine($"{ts}if (P[{_fcpf}{sg.Naam}]) {{ RR[{_fcpf}{sg.Naam}] &= ~BIT5; }}");
                            }
                            sb.AppendLine($"#endif // NO_TIMETOX");
                            sb.AppendLine();
                        }

                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            sb.AppendLine(
                                $"{ts}FM[{_fcpf}{fc.FaseCyclus}] |= (fm_ar_kpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltg}{fc.FaseCyclus}])) ? BIT5 : 0;");
                        }
                        sb.AppendLine();

                        AppendNalopenEG_RRFMCorrection(c, sb, ts);

                        var maxtartotig = c.Data.CCOLVersie >= CCOLVersieEnum.CCOL95 && c.Data.Intergroen ? "max_tar_tig" : "max_tar_to";
                        
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            Tuple<string, List<string>> hasgs = null;
                            if (gelijkstarttuples != null)
                            {
                                foreach (var gs in gelijkstarttuples)
                                {
                                    if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                                    {
                                        hasgs = gs;
                                        break;
                                    }
                                }
                            }

                            var fcf = c.Fasen.FirstOrDefault(x => x.Naam == fc.FaseCyclus);

                            if (fcf == null) continue;
                            
                            if (hasgs != null)
                            {
                                switch (fcf.AlternatieveRuimteType)
                                {
                                    case AlternatieveRuimteTypeEnum.MaxTarToTig:
                                        sb.Append($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ({maxtartotig}({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
                                        break;
                                    case AlternatieveRuimteTypeEnum.MaxTar:
                                        sb.Append($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = (max_tar({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
                                        break;
                                    case AlternatieveRuimteTypeEnum.RealRuimte:
                                        sb.Append($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = (Real_Ruimte({_fcpf}{fc.FaseCyclus}, {_mpf}{_mar}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                foreach (var ofc in hasgs.Item2)
                                {
                                    sb.Append(ofc);
                                }

                                sb.Append($"]) && SCH[{_schpf}{_schaltg}");
                                foreach (var ofc in hasgs.Item2)
                                {
                                    sb.Append(ofc);
                                }

                                sb.AppendLine("];");
                            }
                            else
                            {
                                sb.Append($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ");
                                switch (fcf.AlternatieveRuimteType)
                                {
                                    case AlternatieveRuimteTypeEnum.MaxTarToTig:
                                        sb.Append($"({maxtartotig}({_fcpf}{fc.FaseCyclus})");
                                        break;
                                    case AlternatieveRuimteTypeEnum.MaxTar:
                                        sb.Append($"(max_tar({_fcpf}{fc.FaseCyclus})");
                                        break;              
                                    case AlternatieveRuimteTypeEnum.RealRuimte:
                                        sb.Append($"(Real_Ruimte({_fcpf}{fc.FaseCyclus}, {_mpf}{_mar}{fc.FaseCyclus})");
                                        break;                                        
                                }
                                sb.AppendLine($" >= PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}]) && SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                            }
                        }
                        sb.AppendLine();
                        sb.Append(GetRealFuncPARCorrections(c, ts, true));

                        yes = false;
                        foreach (var fcm in c.Fasen)
                        {
                            foreach (var fm in c.FileIngrepen.Where(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam)))
                            {
                                if (fm is { FileMetingLocatie: FileMetingLocatieEnum.NaStopstreep })
                                {
                                    if (!yes)
                                    {
                                        yes = true;
                                        sb.AppendLine();
                                        sb.AppendLine($"{ts}/* Niet alternatief komen tijdens file */");
                                    }

                                    sb.AppendLine(
                                        $"{ts}if (IH[{_hpf}{_hfile}{fm.Naam}]) PAR[{_fcpf}{fcm.Naam}] = FALSE;");
                                }
                            }
                        }
                        if (yes)
                        {
                            sb.AppendLine();
                        }

                        foreach (var gen in CCOLGenerator.OrderedPieceGenerators[CCOLCodeTypeEnum.RegCAlternatieven])
                        {
                            sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCAlternatieven, ts, order));
                            sb.AppendLine();
                        }

                        sb.AppendLine($"{ts}Alternatief_Add();");
                        sb.AppendLine();
                        foreach (var r in molens)
                        {
                            sb.AppendLine($"{ts}langstwachtende_alternatief_modulen(PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX);");
                        }
                    }
                    else if (!c.Data.MultiModuleReeksen) // This is not yet supported for multi-ML
                    {
                        var r = c.ModuleMolen;
                        sb.AppendLine($"{ts}/* alternatieve realisaties */");
                        sb.AppendLine($"{ts}/* ------------------------ */");
                        if (c.ModuleMolen.Modules.SelectMany(x => x.Fasen).Any(x => x.Alternatieven.Any()))
                        {
                            // Build a list of dictionaries for signalgroups that may realize alternatively;
                            // each dictionary contains alternative signalgroups as keys, and a list of primary
                            // signalgroups under whose CV the alternative may realise
                            var modulesWithAlternatives =
                                new List<Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>>();
                            foreach (var ml in r.Modules)
                            {
                                var altdict =
                                    new Dictionary<string, List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>>();
                                foreach (var mlfc in ml.Fasen)
                                {
                                    foreach (var amlfc in mlfc.Alternatieven)
                                    {
                                        if (!altdict.ContainsKey(amlfc.FaseCyclus))
                                        {
                                            altdict.Add(
                                                amlfc.FaseCyclus,
                                                new List<Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>>
                                                {
                                                    new(amlfc,
                                                        mlfc)
                                                });
                                        }
                                        else
                                        {
                                            altdict[amlfc.FaseCyclus].Add(new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc, mlfc));
                                        }
                                    }
                                }
                                modulesWithAlternatives.Add(altdict);
                            }

                            var mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {
                                    var pre = $"{ts}set_FARML({_fcpf}{alternativeSignalGroup.Key}, PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX, {r.Reeks}{mlidx}, "
                                        .Length;
                                    sb.Append(
                                        $"{ts}set_FARML({_fcpf}{alternativeSignalGroup.Key}, PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX, {r.Reeks}{mlidx}, ");
                                    var i = 0;
                                    foreach (var primarySignalGroup in alternativeSignalGroup.Value)
                                    {
                                        if (i > 0)
                                        {
                                            sb.AppendLine(" || ");
                                            sb.Append("".PadLeft(pre));
                                        }
                                        ++i;
                                        sb.Append($"({c.GetBoolV()}) (CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte({_fcpf}{primarySignalGroup.Item2.FaseCyclus}, {_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}))");
                                    }
                                    sb.AppendLine(");");
                                }
                                ++mlidx;
                            }
                            sb.AppendLine();
                            sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT5;");
                            sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT5;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine();
                            mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {

                                    var pre = $"{ts}RR[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && RA[{_fcpf}{alternativeSignalGroup.Key}] && "
                                        .Length;
                                    sb.Append(
                                        $"{ts}RR[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && RA[{_fcpf}{alternativeSignalGroup.Key}] && ");
                                    var i = 0;
                                    foreach (var primarySignalGroup in alternativeSignalGroup.Value)
                                    {
                                        if (i > 0)
                                        {
                                            sb.AppendLine(" && ");
                                            sb.Append("".PadLeft(pre));
                                        }
                                        ++i;
                                        sb.Append($"!(CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte(" +
                                                  $"{_fcpf}{primarySignalGroup.Item2.FaseCyclus}, {_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}))");
                                    }
                                    sb.AppendLine(") ? BIT5 : 0;");
                                }
                                ++mlidx;
                            }
                            sb.AppendLine();

                            mlidx = 1;
                            foreach (var moduleWithAlternatives in modulesWithAlternatives)
                            {
                                foreach (var alternativeSignalGroup in moduleWithAlternatives)
                                {
                                    sb.AppendLine(
                                        $"{ts}FM[{_fcpf}{alternativeSignalGroup.Key}] |= (AR[{_fcpf}{alternativeSignalGroup.Key}] && " +
                                        $"G[{_fcpf}{alternativeSignalGroup.Key}] && fm_ar_kpr({_fcpf}{alternativeSignalGroup.Key}, PRM[{_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}])) ? BIT5 : 0;");
                                }
                                ++mlidx;
                            }

                            AppendNalopenEG_RRFMCorrection(c, sb, ts);
                        }
                        sb.AppendLine();
                        sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}if (SR[fc] && AG[fc])");
                        sb.AppendLine($"{ts}{ts}{{");

                        sb.AppendLine($"{ts}{ts}{ts}reset_FARML(fc, PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX);");

                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine();
                        

                        foreach (var gen in CCOLGenerator.OrderedPieceGenerators[CCOLCodeTypeEnum.RegCAlternatieven])
                        {
                            sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCAlternatieven, ts, order));
                            sb.AppendLine();
                        }
                        
                        sb.AppendLine($"{ts}Alternatief_Add();");
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}Alternatief_halfstar();");
                        }
                    }
                    return sb.ToString();
                
                default:
                    return null;
            }
        }

        private string GetRealFuncPARCorrections(ControllerModel c, string ts, bool generateNaloopOk)
        {
            var sb = new StringBuilder();
            var groenSyncData = GroenSyncDataModel.ConvertSyncFuncToRealFunc(c);
            var sortedSyncs = GroenSyncDataModel.OrderSyncs(c, groenSyncData);

            var first = true;
            var pars = new[] { new List<string>(), new List<string>(), new List<string>() };
            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                // Only do this for pedestrians with sync
                var sync = sortedSyncs.twoWayPedestrians.FirstOrDefault(x =>
                    x.m1.FaseVan == nl.FaseVan && x.m1.FaseNaar == nl.FaseNaar
                    || x.m1.FaseVan == nl.FaseNaar && x.m1.FaseNaar == nl.FaseVan);
                if (sync.m1 == null)
                {
                    sync = sortedSyncs.twoWay.FirstOrDefault(x =>
                        x.m1.FaseVan == nl.FaseVan && x.m1.FaseNaar == nl.FaseNaar
                        || x.m1.FaseVan == nl.FaseNaar && x.m1.FaseNaar == nl.FaseVan);
                    if (sync.m1 == null)
                    {
                        continue;
                    }
                }

                string tnl;
                string hnl;
                switch (nl.Type)
                {
                    case NaloopTypeEnum.StartGroen:
                        tnl = nl.DetectieAfhankelijk ? _tnlsgd : _tnlsg;
                        hnl = _tnlsg;
                        break;
                    case NaloopTypeEnum.EindeGroen:
                        tnl = nl.DetectieAfhankelijk ? _tnlegd : _tnleg;
                        hnl = _tnleg;
                        break;
                    case NaloopTypeEnum.CyclischVerlengGroen:
                        tnl = nl.DetectieAfhankelijk ? _tnlcvd : _tnlcv;
                        hnl = _tnlcv;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (generateNaloopOk)
                {
                    pars[0].Add($"{ts}IH[{_hpf}{hnl}{nl.FaseVan}{nl.FaseNaar}] = Naloop_OK({_fcpf}{nl.FaseVan}, {_mpf}{_mar}{nl.FaseNaar}, {_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar});");
                }
                if (sync.gelijkstart)
                {
                    pars[1].Add($"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && IH[{_hpf}{hnl}{nl.FaseVan}{nl.FaseNaar}];");
                    pars[2].Add($"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && PAR[{_fcpf}{nl.FaseNaar}];");
                }
                else
                {
                    pars[1].Add($"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && (IH[{_hpf}{hnl}{nl.FaseVan}{nl.FaseNaar}] || IH[{_hpf}{_hlos}{nl.FaseVan}]);");
                    pars[2].Add($"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && (PAR[{_fcpf}{nl.FaseNaar}] || IH[{_hpf}{_hlos}{nl.FaseVan}]);");
                }
            }

            if (pars[0].Count > 0)
            {
                sb.AppendLine($"{ts} /* Bepaal naloop voetgangers wel/niet toegestaan */");
                foreach (var s in pars[0]) sb.AppendLine(s);
                sb.AppendLine();
            }
            
            if (pars[1].Count > 0)
            {
                sb.AppendLine($"{ts} /* PAR-correcties nalopen voetgangers stap 1: naloop past of los OK */");
                foreach (var s in pars[1]) sb.AppendLine(s);
                sb.AppendLine();
            }

            if (pars[2].Count > 0 || c.InterSignaalGroep.Gelijkstarten.Any())
            {
                sb.AppendLine($"{ts}/* PAR-correcties 10 keer checken ivm onderlinge afhankelijkheden */");
                sb.AppendLine($"{ts}for (fc = 0; fc < 10; ++fc)");
                sb.AppendLine($"{ts}{{");
                if (pars[2].Count > 0)
                {
                    sb.AppendLine($"{ts}{ts}/* PAR-correcties nalopen voetgangers stap 2: beide PAR of los OK */");
                    foreach (var s in pars[2]) sb.AppendLine(ts + s);
                    if (sortedSyncs.oneWay.Count > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}/* PAR = PAR */");
                        foreach (var sync in sortedSyncs.oneWay)
                        {
                            var gs = c.InterSignaalGroep.Gelijkstarten.FirstOrDefault(x => x.FaseVan == sync.FaseVan && x.FaseNaar == sync.FaseNaar);
                            var vs = c.InterSignaalGroep.LateReleases.FirstOrDefault(x => x.FaseVan == sync.FaseVan && x.FaseNaar == sync.FaseNaar);
                            var lr = c.InterSignaalGroep.Voorstarten.FirstOrDefault(x => x.FaseVan == sync.FaseVan && x.FaseNaar == sync.FaseNaar);
                            if (gs != null && gs.DeelConflict || vs != null || lr != null)
                            {
                                sb.AppendLine($"{ts}{ts}PAR[{_fcpf}{sync.FaseNaar}] = PAR[{_fcpf}{sync.FaseNaar}] && (PAR[{_fcpf}{sync.FaseVan}] || !A[{_fcpf}{sync.FaseVan}]);");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}{ts}PAR[{_fcpf}{sync.FaseNaar}] = PAR[{_fcpf}{sync.FaseNaar}] && PAR[{_fcpf}{sync.FaseVan}];");
                            }
                        }
                    }

                }

                
                if (c.InterSignaalGroep.Gelijkstarten.Any())
                {
                    if (pars[2].Count > 0) sb.AppendLine();
                    first = true;
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        if (first)
                        {
                            sb.AppendLine($"{ts}{ts}/* PAR correcties gelijkstart synchronisaties */");
                            first = false;
                        }

                        var front = gs.Schakelbaar != AltijdAanUitEnum.Altijd ? $"{ts}{ts}if (SCH[{_schpf}{_schrealgs}{gs:vannaar}]) " : $"{ts}{ts}";
                        sb.AppendLine($"{front}PAR[{_fcpf}{gs:van}] = PAR[{_fcpf}{gs:van}] && (PAR[{_fcpf}{gs:naar}] || !A[{_fcpf}{gs:naar}]);");
                        sb.AppendLine($"{front}PAR[{_fcpf}{gs:naar}] = PAR[{_fcpf}{gs:naar}] && (PAR[{_fcpf}{gs:van}] || !A[{_fcpf}{gs:van}]);");
                    }
                }

                sb.AppendLine($"{ts}}}");

                if (sortedSyncs.oneWay.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* PAR correcties eenzijdige synchronisaties */");
                    foreach (var sync in sortedSyncs.oneWay)
                    {
                        sb.AppendLine($"{ts}PAR[{_fcpf}{sync.FaseVan}] = PAR[{_fcpf}{sync.FaseVan}] || G[{_fcpf}{sync.FaseNaar}];");
                    }
                }
            }

            return sb.ToString();
        }

        private void AppendNalopenEG_RRFMCorrection(ControllerModel c, StringBuilder sb, string ts)
        {
            if (c.InterSignaalGroep.Nalopen.Any(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
            {
                sb.AppendLine($"{ts}/* Bij nalopen op EG mag de volgrichting niet RR en FM");
                sb.AppendLine($"{ts}   gestuurd worden indien de voedende richting groen is */");
                foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
                {
                    sb.AppendLine($"{ts}if (!R[{_fcpf}{nl.FaseVan}] || TNL[{_fcpf}{nl.FaseNaar}]) {{ RR[{_fcpf}{nl.FaseNaar}] &= ~BIT5; FM[{_fcpf}{nl.FaseNaar}] &= ~BIT5; }}");
                }
                sb.AppendLine();
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _schgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schgs");
            _schrealgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schrealgs");
            _hlos = CCOLGeneratorSettingsProvider.Default.GetElementName("hlos");
            _mar = CCOLGeneratorSettingsProvider.Default.GetElementName("mar");

            return base.SetSettings(settings);
        }
    }
}

