using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
	    private string _hmlact;
	    private string _hplact;
	    private string _hnla;
	    private string _schgs;

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
                                        new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc, mlfc)
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

        public override bool HasFunctionLocalVariables() => true;
        
        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules:
                    return new List<Tuple<string, string, string>> { new Tuple<string, string, string>("int", "fc", "") };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingModules:
                    return 10;
                case CCOLCodeTypeEnum.HstCAlternatief:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            var molens = new List<ModuleMolenModel> { c.ModuleMolen };
            if (c.Data.MultiModuleReeksen)
            {
                molens = c.MultiModuleMolens.Where(x => x.Modules.Any(x2 => x2.Fasen.Any())).ToList();
            }

            switch (type)
            {
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
                            if (ffc != null && !ffc.WachttijdVoorspeller)
                            {
                                // find a potential feeding sg
                                var fcnl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseNaar == fc.FaseCyclus);
                                // if there is a feeding sg, set the sg instance to that sg
                                if (fcnl != null) ffc = c.Fasen.FirstOrDefault(x => x.Naam == fcnl.FaseVan);
                            }
                            // if the instance is not null, and it has a predictor, skip setting RR
                            if (ffc != null && ffc.WachttijdVoorspeller) continue;
                            sb.AppendLine(
                                $"{ts}RR[{_fcpf}{fc.FaseCyclus}] |= R[{_fcpf}{fc.FaseCyclus}] && AR[{_fcpf}{fc.FaseCyclus}] && (!PAR[{_fcpf}{fc.FaseCyclus}] || ERA[{_fcpf}{fc.FaseCyclus}]) ? BIT5 : 0;");
                        }
                        sb.AppendLine();

                        var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                        var yes = false;
                        foreach (var gs in gelijkstarttuples.Where(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.Item1)))
                        {
                            if (gs.Item2.Count > 1)
                            {
                                yes = true;
                                var gsInstance = c.InterSignaalGroep.Gelijkstarten.FirstOrDefault(x => x.FaseNaar == gs.Item1 || x.FaseVan == gs.Item1);
                                sb.Append($"{ts}");
                                if (gsInstance != null && gsInstance.Schakelbaar != AltijdAanUitEnum.Altijd) sb.Append($"if (SCH[{_schpf}{_schgs}{gsInstance.FaseVan}{gsInstance.FaseNaar}]) ");
                                sb.Append($"RR[{_fcpf}{gs.Item1}] |= R[{_fcpf}{gs.Item1}] && ");
                                if (gs.Item2.Count > 1) sb.Append("(");
                                var i = 0;
                                foreach (var ofc in gs.Item2)
                                {
                                    if (ofc == gs.Item1)
                                    {
                                        continue;
                                    }

                                    if (i > 0) sb.Append(" || ");
                                    sb.Append($"(RR[{_fcpf}{ofc}] & BIT5)");
                                    ++i;
                                }
                                if (gs.Item2.Count > 1) sb.Append(")");
                                sb.AppendLine(" ? BIT5 : 0;");
                            }
                        }
                        if (yes)
                        {
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
                            foreach (var gs in gelijkstarttuples)
                            {
                                if (gs.Item1 == fc.FaseCyclus && gs.Item2.Count > 1)
                                {
                                    hasgs = gs;
                                    break;
                                }
                            }
                            if (hasgs != null)
                            {
                                sb.Append(
                                    $"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ({maxtartotig}({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
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
                                sb.AppendLine(
                                    $"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = ({maxtartotig}({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}]) && SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                            }
                        }
                        sb.AppendLine();

                        if (c.InterSignaalGroep.Nalopen.Count > 0)
                        {
                            if (c.InterSignaalGroep.Nalopen.Any(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.FaseVan)))
                            {
                                sb.AppendLine($"{ts}/* Verzorgen PAR voor voedende richtingen */");
                                foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.FaseVan)))
                                {
                                    var hasgs = gelijkstarttuples.FirstOrDefault(x => x.Item1 == nl.FaseVan && x.Item2.Count > 1);
                                    if (hasgs != null)
                                    {
                                        sb.Append(
                                            $"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && PAR[{_fcpf}{nl.FaseNaar}] && ({maxtartotig}({_fcpf}{nl.FaseNaar}) >= (PRM[{_prmpf}{_prmaltp}");
                                        foreach (var ofc in hasgs.Item2)
                                        {
                                            sb.Append(ofc);
                                        }
                                        sb.AppendLine($"] + TNL_PAR[{_fcpf}{nl.FaseNaar}]));");
                                    }
                                    else
                                    {
                                        sb.AppendLine(
                                            $"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && PAR[{_fcpf}{nl.FaseNaar}] && ({maxtartotig}({_fcpf}{nl.FaseNaar}) >= (PRM[{_prmpf}{_prmaltp}{nl.FaseVan}] + TNL_PAR[{_fcpf}{nl.FaseNaar}]));");
                                    }
                                }
                                sb.AppendLine();
                            }
                            
                            sb.AppendLine($"{ts}/* Verzorgen PAR voor naloop richtingen */");
                            if (c.InterSignaalGroep.Nalopen.Any(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.FaseVan)))
                            {
                                foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.FaseVan)))
                                {
                                    sb.AppendLine(
                                        $"{ts}PAR[{_fcpf}{nl.FaseNaar}] = PAR[{_fcpf}{nl.FaseNaar}] || RA[{_fcpf}{nl.FaseVan}] || FG[{_fcpf}{nl.FaseVan}];");
                                }
                                sb.AppendLine();
                            }
                        }

                        yes = false;
                        foreach (var gs in gelijkstarttuples.Where(x => c.ModuleMolen.FasenModuleData.Any(x2 => x2.FaseCyclus == x.Item1)))
                        {
                            if (gs.Item2.Count > 1)
                            {
                                yes = true;
                                var gsInstance = c.InterSignaalGroep.Gelijkstarten.FirstOrDefault(x => x.FaseVan == gs.Item1 || x.FaseNaar == gs.Item1);
                                sb.Append($"{ts}");
                                if (gsInstance != null && gsInstance.Schakelbaar != AltijdAanUitEnum.Altijd) sb.Append($"if (SCH[{_schpf}{_schgs}{gsInstance.FaseVan}{gsInstance.FaseNaar}]) ");

                                sb.Append($"PAR[{_fcpf}{gs.Item1}] = PAR[{_fcpf}{gs.Item1}]");
                                foreach (var ofc in gs.Item2)
                                {
                                    if (ofc == gs.Item1)
                                    {
                                        continue;
                                    }
                                    sb.Append($" && (PAR[{_fcpf}{ofc}] || !A[{_fcpf}{ofc}])");
                                }
                                sb.AppendLine(";");
                            }
                        }

                        if (yes)
                        {
                            sb.AppendLine();
                        }

                        yes = false;
                        foreach (var fcm in c.Fasen)
                        {
                            foreach (var fm in c.FileIngrepen.Where(x => x.TeDoserenSignaalGroepen.Any(x2 => x2.FaseCyclus == fcm.Naam)))
                            {
                                if (fm != null && fm.FileMetingLocatie == FileMetingLocatieEnum.NaStopstreep)
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
                            sb.Append(gen.Value.GetCode(c, CCOLCodeTypeEnum.RegCAlternatieven, ts));
                            sb.AppendLine();
                        }

                        sb.AppendLine($"{ts}Alternatief_Add();");
                        sb.AppendLine();
                        foreach (var r in molens)
                        {
                            sb.AppendLine($"{ts}langstwachtende_alternatief_modulen(PR{r.Reeks}, {r.Reeks}, {r.Reeks}_MAX);");
                        }
                    }
                    else if (!c.Data.MultiModuleReeksen)
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
                                                    new Tuple<ModuleFaseCyclusAlternatiefModel, ModuleFaseCyclusModel>(amlfc,
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
                                        sb.Append($"({c.GetBoolV()}) (CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte({_fcpf}{alternativeSignalGroup.Key}, {_fcpf}{primarySignalGroup.Item2.FaseCyclus}, {_prmpf}{_prmaltg}{mlidx}{alternativeSignalGroup.Key}))");
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
                                        sb.Append($"!(CV[{_fcpf}{primarySignalGroup.Item2.FaseCyclus}] && AlternatieveRuimte({_fcpf}{alternativeSignalGroup.Key}, " +
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
            _hmlact = CCOLGeneratorSettingsProvider.Default.GetElementName("hmlact");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");
            _hnla = CCOLGeneratorSettingsProvider.Default.GetElementName("hnla");
            _schgs = CCOLGeneratorSettingsProvider.Default.GetElementName("schgs");

            return base.SetSettings(settings);
        }
    }
}
