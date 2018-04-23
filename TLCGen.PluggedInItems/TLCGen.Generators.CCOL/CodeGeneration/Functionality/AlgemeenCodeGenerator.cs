using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class AlgemeenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        private string _hplact;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtoon7s;
        private CCOLGeneratorCodeStringSettingModel _ussegm;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            // Segment display elements
            foreach (var item in c.Data.SegmentenDisplayBitmapData)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{_ussegm}{item.Naam}"));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ussegm}{item.Naam}", _ussegm));
            }

            // Module display elements
            if (c.Data.UitgangPerModule)
            {
                foreach (var item in c.Data.ModulenDisplayBitmapData)
                {
                    _MyBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{item.Naam}"));
                    _MyElements.Add(new CCOLElement($"{item.Naam}", CCOLElementTypeEnum.Uitgang, item.Naam));
                }
            }

            if (c.Data.SegmentDisplayType == Models.Enumerations.SegmentDisplayTypeEnum.DrieCijferDisplay)
            {
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtoon7s}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtoon7s));
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

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 20;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.EnkelDisplay)
                    {
                        sb.AppendLine($"{ts}SegmentSturing(ML+1, {_uspf}{_ussegm}1, {_uspf}{_ussegm}2, {_uspf}{_ussegm}3, {_uspf}{_ussegm}4, {_uspf}{_ussegm}5, {_uspf}{_ussegm}6, {_uspf}{_ussegm}7);");
                    }
                    else if (c.Data.SegmentDisplayType == SegmentDisplayTypeEnum.DrieCijferDisplay)
                    {
                        if (c.HalfstarData.IsHalfstar)
                        {
                            sb.AppendLine($"{ts}if (IH[{_hpf}{_hplact}] && SCH[{_schpf}{_schtoon7s}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking signaalplantijd */");
                            sb.AppendLine($"{ts}{ts}SegmentSturing(TX_timer / 100, {_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7);");
                            sb.AppendLine($"{ts}{ts}SegmentSturing((TX_timer % 100) / 10, {_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7);");
                            sb.AppendLine($"{ts}{ts}SegmentSturing(TX_timer % 10, {_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine($"{ts}else if (SCH[{_schpf}{_schtoon7s}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}/* Uitsturen segmenten verklikking module regelen */");
                            sb.AppendLine($"{ts}{ts}SegmentSturingDrie(ML + 1,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                            sb.AppendLine($"{ts}}}");
                        }
                        else
                        {
                            sb.AppendLine($"{ts}if (SCH[{_schpf}{_schtoon7s}])");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}SegmentSturingDrie(ML + 1,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
                            sb.AppendLine($"{ts}}}");
                        }
                    }
                    sb.AppendLine();
                    if (c.Data.UitgangPerModule && c.Data.ModulenDisplayBitmapData.Any())
                    {
                        foreach(var m in c.Data.ModulenDisplayBitmapData)
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{m.Naam}] = ML == {m.Naam};");
                        }
                    }

                    break;
            }

            return sb.ToString();
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }
    }
}
