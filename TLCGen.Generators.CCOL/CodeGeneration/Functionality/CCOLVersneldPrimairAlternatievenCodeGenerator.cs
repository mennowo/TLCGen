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

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

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
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                            sb.AppendLine($"{ts}FM[{_fcpf}{fc.FaseCyclus}] |= (fm_ar_kpr({_fcpf}{fc.FaseCyclus}, PRM[{_prmpf}{_prmaltg}{fc.FaseCyclus}])) ? BIT5 : 0;");
                        sb.AppendLine();
                        foreach (var fc in c.ModuleMolen.FasenModuleData)
                            sb.AppendLine($"{ts}PAR[{_fcpf}{fc.FaseCyclus}] = (max_tar_to({_fcpf}{fc.FaseCyclus}) >= PRM[{_prmpf}{_prmaltp}{fc.FaseCyclus}]) && SCH[{_schpf}{_schaltg}{fc.FaseCyclus}];");
                        sb.AppendLine();
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
    }
}
