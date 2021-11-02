using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class SchoolIngreepCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _tdbsi;
        private CCOLGeneratorCodeStringSettingModel _hschoolingreep;
        private CCOLGeneratorCodeStringSettingModel _schschoolingreep;
        private CCOLGeneratorCodeStringSettingModel _tschoolingreepmaxg;
#pragma warning restore 0649
        private string _tnlsgd;
        private string _uswt;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var fc in c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit && 
                                                  x.Detectoren.Any(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen ||
                                                                         x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)))
            {
                if (fc.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_schschoolingreep}{fc.Naam}",
                            fc.SchoolIngreep == Models.Enumerations.NooitAltijdAanUitEnum.SchAan ? 1 : 0,
                            CCOLElementTimeTypeEnum.SCH_type,
                            _schschoolingreep,
                            fc.Naam));
                }
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_tschoolingreepmaxg}{fc.Naam}",
                        fc.SchoolIngreepMaximumGroen,
                        CCOLElementTimeTypeEnum.TE_type,
                        _tschoolingreepmaxg,
                        fc.Naam));
                foreach (var d in fc.Detectoren.Where(x => x.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten))
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tdbsi}{_dpf}{d.Naam}",
                            fc.SchoolIngreepBezetTijd,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tdbsi,
                            d.Naam));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hschoolingreep}{_dpf}{d.Naam}",
                            _hschoolingreep,
                            fc.Naam,
                            d.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        //public override bool HasCCOLBitmapInputs() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCPreApplication => new []{70},
                CCOLCodeTypeEnum.RegCMeetkriterium => new []{40},
                CCOLCodeTypeEnum.RegCRealisatieAfhandeling => new []{20},
                CCOLCodeTypeEnum.RegCPostSystemApplication => new []{90},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();


            var dets = new List<Tuple<FaseCyclusModel, DetectorModel>>();
            foreach (var fc in c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
            {
                foreach (var d in fc.Detectoren.Where(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen || x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten))
                {
                    dets.Add(new Tuple<FaseCyclusModel, DetectorModel>(fc, d));
                }
            }
            if (!dets.Any()) return "";

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    sb.AppendLine($"{ts}/* School ingreep */");        
                    foreach (var d in dets)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_tdbsi}{_dpf}{d.Item2.Naam}] = !D[{_dpf}{d.Item2.Naam}];");
                    }
                    foreach (var d in dets)
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}] = D[{_dpf}{d.Item2.Naam}] && !(RT[{_tpf}{_tdbsi}{_dpf}{d.Item2.Naam}] || T[{_tpf}{_tdbsi}{_dpf}{d.Item2.Naam}]) && !(CIF_IS[{_dpf}{d.Item2.Naam}] >= CIF_DET_STORING) && (R[{_fcpf}{d.Item1.Naam}] || FG[{_fcpf}{d.Item1.Naam}] || H[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}]) || TDH[{_dpf}{d.Item2.Naam}] && !(CIF_IS[{_dpf}{d.Item2.Naam}] >= CIF_DET_STORING) && H[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}];");
                    }
                    break;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    sb.AppendLine($"{ts}/* School ingreep: reset BITs */");
                    foreach (var fc in c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}RW[{_fcpf}{fc.Naam}] &= ~BIT8;");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* School ingreep: set RW BIT8 */");
                    foreach (var d in dets)
                    {
                        if(d.Item1.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"{ts}if (SCH[{_schpf}{_schschoolingreep}{d.Item1.Naam}] && ");
                        }
                        else
                        {
                            sb.Append($"{ts}if (");
                        }
                        sb.AppendLine($"H[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}] && T[{_tpf}{_tschoolingreepmaxg}{d.Item1.Naam}]) RW[{_fcpf}{d.Item1.Naam}] |= BIT8;");
                    }
                    break;
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    sb.AppendLine($"{ts}/* School ingreep: bijhouden max groen & vasthouden naloop tijd */");
                    foreach (var fc in c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_tschoolingreepmaxg}{fc.Naam}] = SG[{_fcpf}{fc.Naam}];");
                    }
                    foreach (var d in dets)
                    {
                        var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.Type == Models.Enumerations.NaloopTypeEnum.StartGroen && x.DetectieAfhankelijk && x.Detectoren.Any(x2 => x2.Detector == d.Item2.Naam));
                        if (nl != null)
                        {
                            sb.AppendLine($"{ts}HT[{_tpf}{_tnlsgd}{nl.FaseVan}{nl.FaseNaar}] = T[{_tpf}{_tschoolingreepmaxg}{d.Item1.Naam}] && CV[{_fcpf}{d.Item1.Naam}] && G[{_fcpf}{d.Item1.Naam}] && IH[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}];");
                        }
                    }
                    break;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    sb.AppendLine($"{ts}/* School ingreep: knipperen wachtlicht */");
                    foreach (var d in dets)
                    {
                        if(!d.Item2.Wachtlicht) continue;
                        
                        if (d.Item1.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Altijd)
                        {
                            sb.Append($"{ts}if (SCH[{_schpf}{_schschoolingreep}{d.Item1.Naam}]) ");
                        }
                        sb.AppendLine($"CIF_GUS[{_uspf}{_uswt}{d.Item2.Naam}] = CIF_GUS[{_uspf}{_uswt}{d.Item2.Naam}] && !(IH[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}] && Knipper_1Hz) || G[{_fcpf}{d.Item1.Naam}] && D[{_dpf}{d.Item2.Naam}] && IH[{_hpf}{_hschoolingreep}{_dpf}{d.Item2.Naam}] && Knipper_1Hz;");
                    }
                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _uswt = CCOLGeneratorSettingsProvider.Default.GetElementName("uswt");

            return base.SetSettings(settings);
        }
    }
}
