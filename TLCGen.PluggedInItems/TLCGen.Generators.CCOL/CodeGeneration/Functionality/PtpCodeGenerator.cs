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
    public class PtpCodeGenerator : CCOLCodePieceGeneratorBase
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

#pragma warning disable 0649
        private string _hptp;
        private string _prmptp;
        private string _usptp;

        private string _hiks;
        private string _huks;
        private string _hoke;
        private string _herr;
        private string _herr0;
        private string _herr1;
        private string _herr2;

        private string _usoke;
        private string _userr;

        private string _prmiks;
        private string _prmuks;
        private string _prmoke;
        private string _prmerr;
        private string _prmerr0;
        private string _prmerr1;
        private string _prmerr2;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                for (int i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hiks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                }
                for (int i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_hiks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_prmiks}" + i.ToString("00"),
                            2,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }
                for (int i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_huks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                }
                for (int i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_huks}" + i.ToString("00"),
                            CCOLElementTypeEnum.HulpElement));
                    _MyElements.Add(
                        new CCOLElement(
                            $"{k.TeKoppelenKruispunt}{_prmuks}" + i.ToString("00"),
                            2,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                }

                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hoke}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr0}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr1}",
                            CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr2}",
                            CCOLElementTypeEnum.HulpElement));

                _MyElements.Add(
                        new CCOLElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_usoke}",
                            CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_userr}",
                            CCOLElementTypeEnum.Uitgang));

                _MyBitmapOutputs.Add(
                        new CCOLIOElement(
                            k.OkBitmapData as IOElementModel,
                            $"{_uspf}{_usptp}_{k.TeKoppelenKruispunt}{_usoke}"));
                _MyBitmapOutputs.Add(
                        new CCOLIOElement(
                            k.ErrorBitmapData as IOElementModel,
                            $"{_uspf}{_usptp}_{k.TeKoppelenKruispunt}{_userr}"));

                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmoke}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr0}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr1}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            CCOLElementTypeEnum.Parameter));
                _MyElements.Add(
                        new CCOLElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr2}",
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
                case CCOLCodeTypeEnum.RegCIncludes:
                    return 20;
                case CCOLCodeTypeEnum.RegCPreSystemApplication:
                    return 10;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 10;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    if (c.PTPData.PTPKoppelingen.Count > 0)
                    {
                        sb.AppendLine($"{ts}#include \"{c.Data.Naam}ptp.c\" /* PTP seriele koppeling */");
                    sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreSystemApplication:
                    if (c.PTPData.PTPKoppelingen.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                        sb.AppendLine($"{ts}ptp_pre_system_app();");
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.PTPData.PTPKoppelingen.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                        sb.AppendLine($"{ts}ptp_post_system_app();");
                        sb.AppendLine();
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
