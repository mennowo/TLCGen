using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLMeeAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
            {
                if (ma.DetectieAfhankelijk)
                {
                    foreach(MeeaanvraagDetectorModel dm in ma.Detectoren)
                    {
                        _MyElements.Add(
                            new CCOLElement(
                                $"hmad{dm.MeeaanvraagDetector}",
                                $"mad{dm.MeeaanvraagDetector}",
                                CCOLElementTypeEnum.HulpElement));
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
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    return true;
                default:
                    return false;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Aanvragen:
                    sb.AppendLine();
                    sb.AppendLine($"{tabspace}/* Meeaanvragen */");
                    sb.AppendLine($"{tabspace}/* ------------ */");
                    bool hasdetafh = false;
                    if(c.InterSignaalGroep.Meeaanvragen.Count > 0)
                    {
                        hasdetafh = c.InterSignaalGroep.Meeaanvragen.Where(x => x.DetectieAfhankelijk == true).Any();
                    }
                    if(hasdetafh)
                    {
                        sb.AppendLine($"{tabspace}/* Bewaar meldingen van detectie voor het zetten van een meeaanvraag */");
                        foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
                        {
                            if (ma.DetectieAfhankelijk)
                            {
                                foreach(MeeaanvraagDetectorModel dm in ma.Detectoren)
                                {
                                    sb.AppendLine($"{tabspace}IH[hmad{dm.MeeaanvraagDetector}]= SG[fc{ma.FaseVan}] ? FALSE : IH[hmad{dm.MeeaanvraagDetector}] || D[d{dm.MeeaanvraagDetector}] && !G[fc{ma.FaseVan}] && A[fc{ma.FaseVan}];");
                                }
                            }
                        }
                        sb.AppendLine();
                    }
                    foreach (MeeaanvraagModel ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        if(!ma.DetectieAfhankelijk)
                        {
                            switch(ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($"{tabspace}mee_aanvraag(fc{ma.FaseNaar}, (bool) (!G[fc{ma.FaseVan}] && A[fc{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($"{tabspace}mee_aanvraag(fc{ma.FaseNaar}, (bool) (RA[fc{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($"{tabspace}mee_aanvraag(fc{ma.FaseNaar}, (bool) (SG[fc{ma.FaseVan}]));");
                                    break;
                            }
                        }
                        else
                        {
                            sb.Append($"{tabspace}mee_aanvraag(fc{ma.FaseNaar}, (bool) ((");
                            int i = 0;
                            foreach (MeeaanvraagDetectorModel dm in ma.Detectoren)
                            {
                                if (i == 1)
                                {
                                    sb.Append(" || ");
                                }
                                ++i;
                                sb.Append($"H[hmad{dm.MeeaanvraagDetector}]");
                            }

                            switch (ma.Type)
                            {
                                case MeeaanvraagTypeEnum.Aanvraag:
                                    sb.AppendLine($") && !G[fc{ma.FaseVan}] && A[fc{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.RoodVoorAanvraag:
                                    sb.AppendLine($") && RA[fc{ma.FaseVan}]));");
                                    break;
                                case MeeaanvraagTypeEnum.Startgroen:
                                    sb.AppendLine($") && SG[fc{ma.FaseVan}]));");
                                    break;
                            }
                        }
                    }
                    sb.AppendLine("");
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
