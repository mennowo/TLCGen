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
    public class CCOLMeeAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        
#pragma warning disable 0649
        private string _hmad; // help element meeaanvraag detector name
        private string _prmtypema; // help element meeaanvraag detector name
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
            {
                if (ma.DetectieAfhankelijk)
                {
                    foreach(var dm in ma.Detectoren)
                    {
                        var elem = new CCOLElement($"{_hmad}{dm.MeeaanvraagDetector}", CCOLElementTypeEnum.HulpElement);
                        if (_MyElements.Count == 0 || _MyElements.All(x => x.Naam != elem.Naam))
                        {
                            _MyElements.Add(elem);
                        }
                    }
                }
                if (ma.TypeInstelbaarOpStraat)
                {
                    var inst = 0;
                    switch (ma.Type)
                    {
                        case MeeaanvraagTypeEnum.Aanvraag:
                            inst = 1;
                            break;
                        case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                            inst = 2;
                            break;
                        case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                            inst = 3;
                            break;
                        case MeeaanvraagTypeEnum.Startgroen:
                            inst = 4;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_prmtypema}{ma.FaseVan}{ma.FaseNaar}",
                            inst,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
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

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Meeaanvragen */");
                    sb.AppendLine($"{ts}/* ------------ */");
                    bool hasdetafh = false;
                    if(c.InterSignaalGroep.Meeaanvragen.Count > 0)
                    {
                        hasdetafh = c.InterSignaalGroep.Meeaanvragen.Any(x => x.DetectieAfhankelijk);
                    }
                    if(hasdetafh)
                    {
                        sb.AppendLine($"{ts}/* Bewaar meldingen van detectie voor het zetten van een meeaanvraag */");
                        foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                        {
                            if (!ma.DetectieAfhankelijk) continue;
                            foreach(var dm in ma.Detectoren)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]= SG[{_fcpf}{ma.FaseVan}] ? FALSE : IH[{_hpf}{_hmad}{dm.MeeaanvraagDetector}] || D[{_dpf}{dm.MeeaanvraagDetector}] && !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}];");
                            }
                        }
                        sb.AppendLine();
                    }
                    foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        if (ma.TypeInstelbaarOpStraat)
                        {
                            if (!ma.DetectieAfhankelijk || !ma.Detectoren.Any())
                            {
                                sb.AppendLine($"{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, TRUE);");
                            }
                            else
                            {
                                sb.Append($"{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (bool)(");
                                var i = 0;
                                foreach (var dm in ma.Detectoren)
                                {
                                    if (i == 1)
                                    {
                                        sb.Append(" || ");
                                    }
                                    ++i;
                                    sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                                }
                                sb.AppendLine("));");
                            }
                        }
                        else if(!ma.DetectieAfhankelijk || !ma.Detectoren.Any())
                        {
                            switch(ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (!G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (SG[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            sb.Append($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) ((");
                            var i = 0;
                            foreach (var dm in ma.Detectoren)
                            {
                                if (i == 1)
                                {
                                    sb.Append(" || ");
                                }
                                ++i;
                                sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                            }

                            switch (ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($") && !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($") && RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($") && RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($") && SG[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                    sb.AppendLine();
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }
    }
}
