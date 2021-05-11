using Newtonsoft.Json;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateItemModel
    {
        public CombinatieTemplateItemTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string ObjectJson { get; set; }

        public object GetObject()
        {
            if (string.IsNullOrWhiteSpace(ObjectJson)) return null;
            switch (Type)
            {
                case CombinatieTemplateItemTypeEnum.Detector:
                    return JsonConvert.DeserializeObject<DetectorModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Naloop:
                    return JsonConvert.DeserializeObject<NaloopModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    return JsonConvert.DeserializeObject<MeeaanvraagModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Rateltikker:
                    return JsonConvert.DeserializeObject<RatelTikkerModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    return JsonConvert.DeserializeObject<GelijkstartModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.LateRelease:
                    return JsonConvert.DeserializeObject<LateReleaseModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.PrioIngreep:
                    return JsonConvert.DeserializeObject<PrioIngreepModel>(ObjectJson);
            }
            return null;
        }
    }
}
