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
    public class CCOLPtpCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;

        private string _hptp;
        private string _prmptp;
        private string _usptp;

        private string _hptpiks;
        private string _hptpuks;
        private string _hptpoke;
        private string _hptperr;
        private string _hptperr0;
        private string _hptperr1;
        private string _hptperr2;

        private string _usptpoke;
        private string _usptperr;

        private string _prmptpiks;
        private string _prmptpuks;
        private string _prmptpoke;
        private string _prmptperr;
        private string _prmptperr0;
        private string _prmptperr1;
        private string _prmptperr2;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            foreach(var k in c.PTPData.PTPKoppelingen)
            {
                for (int i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptpiks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                }
                for (int i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_hptpiks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_prmptpiks}" + i.ToString("00"),
                            2,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }
                for (int i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptpuks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                }
                for (int i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_hptpuks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_prmptpuks}" + i.ToString("00"),
                            2,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }

                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptpoke}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptperr}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptperr0}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptperr1}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hptperr2}",
                            CCOLElementTypeEnum.HulpElement));

                _MyElements.Add(
                        new CCOLElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_usptpoke}",
                            CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_usptperr}",
                            CCOLElementTypeEnum.Uitgang));

                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmptpoke}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr0}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr1}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmptperr2}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
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
                case CCOLRegCCodeTypeEnum.Includes:
                case CCOLRegCCodeTypeEnum.PreSystemApplication:
                case CCOLRegCCodeTypeEnum.PostSystemApplication:
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
                case CCOLRegCCodeTypeEnum.Includes:
                    sb.AppendLine();
                    sb.AppendLine($"{ts}#include \"{c.Data.Naam}ptp.c\" /* PTP seriele koppeling */");
                    return sb.ToString();
                case CCOLRegCCodeTypeEnum.PreSystemApplication:
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                    sb.AppendLine($"{ts}ptp_pre_system_app();");
                    return sb.ToString();
                case CCOLRegCCodeTypeEnum.PostSystemApplication:
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                    sb.AppendLine($"{ts}ptp_post_system_app();");
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
            foreach (var s in settings.Settings)
            {
                switch(s.Default)
                {
                    case "ptp":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptp = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptp = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Uitgang:     _usptp = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "iks":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptpiks = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptpiks = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "uks":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptpuks = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptpuks = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "oke":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptpoke = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptpoke = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Uitgang:     _usptpoke = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "err":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptperr = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptperr = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Uitgang:     _usptperr = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "err0":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptperr0 = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptperr0 = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "err1":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptperr1 = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptperr1 = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                    case "err2":
                        switch (s.Type)
                        {
                            case CCOLGeneratorSettingTypeEnum.HulpElement: _hptperr2 = s.Setting == null ? s.Default : s.Setting; break;
                            case CCOLGeneratorSettingTypeEnum.Parameter:   _prmptperr2 = s.Setting == null ? s.Default : s.Setting; break;
                        }
                        break;
                }
            }

            base.SetSettings(settings);
        }
    }
}
