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
    public class CCOLDetectieAanvragenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _prmpf; // parameter prefix local storage
        private string _prmd;  // parameter request type name

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            List<DetectorModel> dets = new List<DetectorModel>();
            dets.AddRange(c.Fasen.SelectMany(x => x.Detectoren));
            dets.AddRange(c.Detectoren);

            // Detectie aanvraag functie
            foreach (DetectorModel dm in dets)
            {
                if (dm.Aanvraag == Models.Enumerations.DetectorAanvraagTypeEnum.Geen)
                    continue;

                int set = 0;
                switch (dm.Aanvraag)
                {
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Uit:
                        set = 0;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RnietTRG:
                        set = 1;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.Rood:
                        set = 2;
                        break;
                    case Models.Enumerations.DetectorAanvraagTypeEnum.RoodGeel:
                        set = 3;
                        break;
                }
                _MyElements.Add(new CCOLElement($"{_prmd}{dm.Naam}", set, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
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
                    sb.AppendLine($"{tabspace}/* Detectie aanvragen */");
                    sb.AppendLine($"{tabspace}/* ------------------ */");
                    foreach (FaseCyclusModel fcm in c.Fasen)
                    {
                        bool HasA = false;
                        if (fcm.Detectoren?.Count > 0)
                        {
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                                {
                                    HasA = true;
                                    break;
                                }
                            }
                        }
                        if (HasA)
                        {
                            sb.AppendLine($"{tabspace}aanvraag_detectie_prm_va_arg((count) {fcm.GetDefine()}, ");
                            foreach (DetectorModel dm in fcm.Detectoren)
                            {
                                if (dm.Aanvraag != DetectorAanvraagTypeEnum.Geen)
                                    sb.AppendLine($"{tabspace}{tabspace}(va_count) {dm.GetDefine()}, (va_mulv) PRM[{_prmpf}{_prmd}{dm.Naam}], ");
                            }
                            sb.AppendLine($"{tabspace}{tabspace}(va_count) END);");
                        }
                    }
                    sb.AppendLine("");
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool HasSettings()
        {
            return true;
        }

        public override void SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            foreach(var s in settings.Settings)
            {
                if (s.Default == "da") _prmd = s.Setting == null ? s.Default : s.Setting;
            }

            _prmpf = CCOLGeneratorSettingsProvider.Default.GetPrefix("prm");
        }
    }
}
