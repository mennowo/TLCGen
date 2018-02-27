using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class MeeAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        
#pragma warning disable 0649
        private string _hmad; // help element meeaanvraag detector name
        private string _prmtypema; // help element meeaanvraag detector name
        private string _tuitgestma; // help element meeaanvraag detector name
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

                if ((ma.Type == MeeaanvraagTypeEnum.Startgroen || ma.TypeInstelbaarOpStraat) &&
                    ma.Uitgesteld)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}",
                            ma.UitgesteldTijdsduur,
                            CCOLElementTimeTypeEnum.TE_type,
                            CCOLElementTypeEnum.Timer));
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

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
                    return 30;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCAanvragen:
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
                        var mads = new List<Tuple<string, string>>();
                        foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                        {
                            if (!ma.DetectieAfhankelijk) continue;
                            foreach(var d in ma.Detectoren)
                            {
                                if(!mads.Any(x => x.Item1 == ma.FaseVan && x.Item2 == d.MeeaanvraagDetector))
                                {
                                    mads.Add(new Tuple<string, string>(ma.FaseVan, d.MeeaanvraagDetector));
                                }
                            }
                        }
                        foreach (var mad in mads)
                        {
                            sb.AppendLine($"{ts}IH[{_hpf}{_hmad}{mad.Item2}]= SG[{_fcpf}{mad.Item1}] ? FALSE : IH[{_hpf}{_hmad}{mad.Item2}] || D[{_dpf}{mad.Item2}] && !G[{_fcpf}{mad.Item1}] && A[{_fcpf}{mad.Item1}];");
                        }
                        sb.AppendLine();
                    }
                    foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        if (ma.Uitgesteld)
                        {
                            sb.AppendLine($"{ts}/* Uitgestelde meeaanvraag op startgroen */");
                            
                            sb.Append($"{ts}RT[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}] = SG[{_fcpf}{ma.FaseVan}]");
                            if (ma.DetectieAfhankelijk && ma.Detectoren.Any())
                            {
                                sb.Append(" && (");
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
                                sb.Append(")");
                            }
                            sb.AppendLine(";");
                        }
                        if (ma.TypeInstelbaarOpStraat)
                        {
                            if (!ma.DetectieAfhankelijk || !ma.Detectoren.Any())
                            {
                                sb.AppendLine(!ma.Uitgesteld
                                    ? $"{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (bool)(TRUE));"
                                    : $"{ts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (bool)(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                            }
                            else
                            {
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{ts}if (PRM[{_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}] == 4)");
                                    sb.AppendLine($"{ts}{{");
                                    sb.AppendLine($"{ts}{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool)(ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    sb.AppendLine($"{ts}}}");
                                    sb.AppendLine($"{ts}else");
                                    sb.AppendLine($"{ts}{{");
                                }

                                var uts = ma.Uitgesteld ? ts + ts : ts;
                                sb.Append($"{uts}mee_aanvraag_prm({_fcpf}{ma.FaseNaar}, {_fcpf}{ma.FaseVan}, {_prmpf}{_prmtypema}{ma.FaseVan}{ma.FaseNaar}, (bool)(");
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
                                
                                if (ma.Uitgesteld)
                                {
                                    sb.AppendLine($"{ts}}}");
                                }
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
                                    sb.AppendLine(!ma.Uitgesteld
                                        ? $"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (SG[{_fcpf}{ma.FaseVan}]));"
                                        : $"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            sb.Append($"{ts}mee_aanvraag({_fcpf}{ma.FaseNaar}, (bool) (");
                            var i = 0;
                            if (!ma.Uitgesteld)
                            {
                                sb.Append("(");
                                foreach (var dm in ma.Detectoren)
                                {
                                    if (i == 1)
                                    {
                                        sb.Append(" || ");
                                    }

                                    ++i;
                                    sb.Append($"H[{_hpf}{_hmad}{dm.MeeaanvraagDetector}]");
                                }
                                sb.Append(") ");
                            }

                            switch (ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"&& !G[{_fcpf}{ma.FaseVan}] && A[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"&& RA[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraagGeenConflicten:
                                    sb.AppendLine($"&& RA[{_fcpf}{ma.FaseVan}] && !K[{_fcpf}{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine(!ma.Uitgesteld
                                        ? $"&& SG[{_fcpf}{ma.FaseVan}]));"
                                        : $"ET[{_tpf}{_tuitgestma}{ma.FaseVan}{ma.FaseNaar}]));");
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
