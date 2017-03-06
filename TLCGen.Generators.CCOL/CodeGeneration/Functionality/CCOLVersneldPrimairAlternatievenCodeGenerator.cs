using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLVersneldPrimairAlternatievenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        
#pragma warning disable 0649
        private string _prmmlfpr;
        private string _prmaltg;
        private string _prmaltp;
        private string _schaltg;
#pragma warning restore 0649
        private string _tnlsg;
        private string _tnlfg;
        private string _tnlcv;
        private string _tnleg;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
            foreach (var fc in c.ModuleMolen.FasenModuleData)
            {
                // Vooruit realisaties
                _MyElements.Add(
                    new CCOLElement(
                        $"{_prmmlfpr}{fc.FaseCyclus}",
                        fc.ModulenVooruit,
                        CCOLElementTimeTypeEnum.None,
                        CCOLElementTypeEnum.Parameter));

                // Alternatieven
                if (c.ModuleMolen.LangstWachtendeAlternatief)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_prmaltg}{fc.FaseCyclus}",
                            fc.AlternatieveGroenTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Parameter));

                    // For prmaltp and schaltg: combine if the signal group is part of a simultaneous synchronisation
                    var gs = gelijkstarttuples.Where(x => x.Item1 == fc.FaseCyclus).First();

                    if (gs.Item2.Count > 1)
                    {
                        bool containsaltp = false;
                        bool containsaltg = false;
                        string namealtp = _prmaltp + string.Join(string.Empty, gs.Item2);
                        string namealtg = _schaltg + string.Join(string.Empty, gs.Item2);
                        foreach (var i in _MyElements)
                        {
                            if (i.Naam == namealtp)
                            {
                                containsaltp = true;
                                break;
                            }
                        }
                        foreach (var i in _MyElements)
                        {
                            if (i.Naam == namealtg)
                            {
                                containsaltg = true;
                                break;
                            }
                        }
                        if (!containsaltp)
                        {
                            _MyElements.Add(
                                new CCOLElement(
                                    namealtp,
                                    fc.AlternatieveRuimte,
                                    CCOLElementTimeTypeEnum.TE_type,
                                    CCOLElementTypeEnum.Parameter));
                        }
                        if (!containsaltg)
                        {
                            _MyElements.Add(
                                new CCOLElement(
                                    namealtg,
                                    fc.AlternatiefToestaan == true ? 1 : 0,
                                    CCOLElementTimeTypeEnum.SCH_type,
                                    CCOLElementTypeEnum.Schakelaar));
                        }
                    }
                    else
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_prmaltp}{fc.FaseCyclus}",
                                fc.AlternatieveRuimte,
                                CCOLElementTimeTypeEnum.TE_type,
                                CCOLElementTypeEnum.Parameter));
                        _MyElements.Add(
                            new CCOLElement(
                                $"{_schaltg}{fc.FaseCyclus}",
                                fc.AlternatiefToestaan == true ? 1 : 0,
                                CCOLElementTimeTypeEnum.SCH_type,
                                CCOLElementTypeEnum.Schakelaar));
                    }
                }
            }

        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.RealisatieAfhandelingModules:
                    sb.AppendLine($"{ts}/* versnelde primaire realisaties */");
                    sb.AppendLine($"{ts}/* ------------------------------ */");
                    sb.AppendLine($"{ts}/* periode versnelde primaire realisatie - aantal modulen vooruit */");
                    sb.AppendLine($"{ts}/* -------------------------------------------------------------- */");
                    foreach (var fc in c.ModuleMolen.FasenModuleData)
                        sb.AppendLine($"    PFPR[{_fcpf}{fc.FaseCyclus}] = ml_fpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmmlfpr}{fc.FaseCyclus}], PRML, ML, MLMAX);");
                    sb.AppendLine("");
                    sb.AppendLine($"{ts}VersneldPrimair_Add();");
                    sb.AppendLine("");
                    sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                    sb.AppendLine($"{ts}{ts}set_FPRML(fc, PRML, ML, MLMAX, (bool)PFPR[fc]);");
                    sb.AppendLine();
                    if (c.ModuleMolen.LangstWachtendeAlternatief)
                    {
                        sb.AppendLine($"{ts}/* langstwachtende alternatieve realisatie */");
                        sb.AppendLine($"{ts}/* --------------------------------------- */");
                        sb.AppendLine("");
                        sb.AppendLine($"{ts}afsluiten_aanvraaggebied_pr(PRML, ML);");
                        sb.AppendLine("");
                        sb.AppendLine($"{ts}for (fc=0; fc<FCMAX; fc++)");
                        sb.AppendLine($"{ts}" + "{");
                        sb.AppendLine($"{ts}{ts}RR[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}{ts}FM[fc] &= ~BIT5;");
                        sb.AppendLine($"{ts}" + "}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* zet richtingen die alternatief gaan realiseren         */");
                        sb.AppendLine($"{ts}/* terug naar RV als er geen alternatieve ruimte meer is. */");
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                            sb.AppendLine($"{ts}RR[{_fcpf}{fc.FaseCyclus}] |= R[{_fcpf}{fc.FaseCyclus}] && AR[{_fcpf}{fc.FaseCyclus}] && (!PAR[{_fcpf}{fc.FaseCyclus}] || ERA[{_fcpf}{fc.FaseCyclus}]) ? BIT5 : 0;");
                        sb.AppendLine();

                        var gelijkstarttuples = CCOLCodeHelper.GetFasenWithGelijkStarts(c);
                        bool yes = false;
                        foreach (var gs in gelijkstarttuples)
                        {
                            if(gs.Item2.Count > 1)
                            {
                                yes = true;
                                sb.Append($"{ts}RR[{_fcpf}{gs.Item1}] |= R[{_fcpf}{gs.Item1}] && ");
                                if (gs.Item2.Count > 1) sb.Append("(");
                                int i = 0;
                                foreach(var ofc in gs.Item2)
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
                        if(yes)
                        {
                            sb.AppendLine();
                        }

                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                        {
                            sb.AppendLine($"{ts}FM[{_fcpf}{fc.FaseCyclus}] |= (fm_ar_kpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltg}{fc.FaseCyclus}])) ? BIT5 : 0;");
                        }
                        sb.AppendLine();
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
                                sb.Append($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = (max_tar_to({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}");
                                foreach(var ofc in hasgs.Item2)
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
                                sb.AppendLine($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = (max_tar_to({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}]) && SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                            }
                        }
                        sb.AppendLine();

                        if (c.InterSignaalGroep.Nalopen.Count > 0)
                        {
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
#warning Is this correct and desired? Need to look (also?) at other timers?
                                string tnl = "";
                                if (nl.Tijden.Where(x => x.Type == NaloopTijdTypeEnum.VastGroen).Any())
                                {
                                    tnl = _tnlfg;
                                }
                                else if (nl.Tijden.Where(x => x.Type == NaloopTijdTypeEnum.StartGroen).Any())
                                {
                                    tnl = _tnlsg;
                                }
                                else if (nl.Tijden.Where(x => x.Type == NaloopTijdTypeEnum.EindeGroen).Any())
                                {
                                    tnl = _tnleg;
                                }
                                else if (nl.Tijden.Where(x => x.Type == NaloopTijdTypeEnum.EindeVerlengGroen).Any())
                                {
                                    tnl = _tnlcv;
                                }
                                sb.AppendLine($"{ts}PAR[{_fcpf}{nl.FaseVan}] = PAR[{_fcpf}{nl.FaseVan}] && ((max_tar_to({_fcpf}{nl.FaseNaar}) >= T_max[{_tpf}{tnl}{nl.FaseVan}{nl.FaseNaar}]) || G[{_fcpf}{nl.FaseVan}] || !A[{_fcpf}{nl.FaseNaar}]);");
                            }
                            sb.AppendLine();
                        }

                        yes = false;
                        foreach (var gs in gelijkstarttuples)
                        {
                            if (gs.Item2.Count > 1)
                            {
                                yes = true;
                                sb.Append($"{ts}PAR[{_fcpf}{gs.Item1}] = PAR[{_fcpf}{gs.Item1}]");
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
                        
                        sb.AppendLine($"{ts}Alternatief_Add();");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}langstwachtende_alternatief_modulen(PRML, ML, ML_MAX);");
                    }
                    sb.AppendLine();

                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");

            return base.SetSettings(settings);
        }
    }
}
