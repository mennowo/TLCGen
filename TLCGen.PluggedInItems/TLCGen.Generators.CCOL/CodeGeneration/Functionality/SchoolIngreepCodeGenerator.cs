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
                            $"{_schschoolingreep}",
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
                            _tdbsi,
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
                            d.Naam));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapInputs() => true;

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 70;
                case CCOLCodeTypeEnum.RegCMeetkriterium:
                    return 40;
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:
                    return 20;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 0; // Handled where waitsignals are handled TODO
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}/* School ingreep */");
                    var dets = c.Fasen.Where(x => x.SchoolIngreep != Models.Enumerations.NooitAltijdAanUitEnum.Nooit)
                                      .SelectMany(x => x.Detectoren.Where(x2 => x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBinnen ||
                                                                                x2.Type == Models.Enumerations.DetectorTypeEnum.KnopBuiten)).ToList();
                    foreach (var d in dets)
                    {
                        sb.AppendLine($"{ts}RT[{_tpf}{_tdbsi}{_dpf}{d.Naam}] = !D[{_dpf}{d.Naam}];");
                    }
                    foreach (var d in dets)
                    {
                        sb.AppendLine($"{ts}IH[hschmfunctiedr35a] = D[dr35a] && !(RT[tdbdr35a] || T[tdbdr35a]) && !(CIF_IS[dr35a] >= CIF_DET_STORING) && (R[fc35] || FG[fc35] || H[hschmfunctiedr35a]) || TDH[dr35a] && !(CIF_IS[dr35a] >= CIF_DET_STORING) && H[hschmfunctiedr35a];");
                    }
                    break;
                case CCOLCodeTypeEnum.RegCMeetkriterium: // TODO
                    // if (H[hschmfunctiedr35a]||H[hschmfunctiedr35b]) MK[fc35]|=BIT7; else MK[fc35]&=~BIT7;
                    break;
                case CCOLCodeTypeEnum.RegCRealisatieAfhandeling:  // TODO
                    // na nalopen! voor drukknoppen die nalopen zetten voor de richting:
                    // HT[tnlsg3536] = CV[fc35] && G[fc35] && IH[hschmfunctiedr35a];
                    break;
            }

            return sb.ToString();
        }
    }
}
