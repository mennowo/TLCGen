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
        private string _hplact;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _schtoon7s;
        private CCOLGeneratorCodeStringSettingModel _ussegm;
        private CCOLGeneratorCodeStringSettingModel _usML;
        private CCOLGeneratorCodeStringSettingModel _prmxx;
        private CCOLGeneratorCodeStringSettingModel _prmyy;
        private CCOLGeneratorCodeStringSettingModel _prmzz;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myBitmapOutputs = new List<CCOLIOElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            // Segment display elements
            foreach (var item in c.Data.SegmentenDisplayBitmapData)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{_ussegm}{item.Naam}"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ussegm}{item.Naam}", _ussegm));
            }
            if (c.Data.SegmentDisplayType == Models.Enumerations.SegmentDisplayTypeEnum.DrieCijferDisplay)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schtoon7s}", 1, CCOLElementTimeTypeEnum.SCH_type, _schtoon7s));
            }

            // Module display elements
            if (c.Data.UitgangPerModule)
            {
                foreach (var item in c.Data.ModulenDisplayBitmapData)
                {
                    _myBitmapOutputs.Add(new CCOLIOElement(item.BitmapData, $"{_uspf}{item.Naam.Replace("ML", _usML.Setting)}"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{item.Naam.Replace("ML", _usML.Setting)}", _usML, item.Naam.Replace("ML", _usML.Setting)));
                }
            }

            // Inputs
            foreach (var i in c.Ingangen)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(i.Naam, CCOLElementTypeEnum.Ingang, i.Omschrijving));
                _myBitmapInputs.Add(new CCOLIOElement(i, $"{_ispf}{i.Naam}"));
            }

            // Versie beheer
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmxx}", c.Data.HuidigeVersieMajor, CCOLElementTimeTypeEnum.None, _prmxx));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmyy}", c.Data.HuidigeVersieMinor, CCOLElementTimeTypeEnum.None, _prmyy));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmzz}", c.Data.HuidigeVersieRevision, CCOLElementTimeTypeEnum.None, _prmzz));
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override bool HasCCOLBitmapInputs() => true;

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
                            sb.AppendLine($"{ts}{ts}SegmentSturingDrie(TX_timer,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}a1, {_uspf}{_ussegm}a2, {_uspf}{_ussegm}a3, {_uspf}{_ussegm}a4, {_uspf}{_ussegm}a5, {_uspf}{_ussegm}a6, {_uspf}{_ussegm}a7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}b1, {_uspf}{_ussegm}b2, {_uspf}{_ussegm}b3, {_uspf}{_ussegm}b4, {_uspf}{_ussegm}b5, {_uspf}{_ussegm}b6, {_uspf}{_ussegm}b7,");
                            sb.AppendLine($"{ts}{ts}{ts}{_uspf}{_ussegm}c1, {_uspf}{_ussegm}c2, {_uspf}{_ussegm}c3, {_uspf}{_ussegm}c4, {_uspf}{_ussegm}c5, {_uspf}{_ussegm}c6, {_uspf}{_ussegm}c7);");
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
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{m.Naam.Replace("ML", _usML.Setting)}] = ML == {m.Naam};");
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
