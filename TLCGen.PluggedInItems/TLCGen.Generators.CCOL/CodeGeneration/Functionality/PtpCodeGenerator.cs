﻿using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class PtpCodeGenerator : CCOLCodePieceGeneratorBase
    {
#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _hptp;
        private CCOLGeneratorCodeStringSettingModel _prmptp;
        private CCOLGeneratorCodeStringSettingModel _usptp;

        private CCOLGeneratorCodeStringSettingModel _hiks;
        private CCOLGeneratorCodeStringSettingModel _huks;
        private CCOLGeneratorCodeStringSettingModel _hoke;
        private CCOLGeneratorCodeStringSettingModel _herr;
        private CCOLGeneratorCodeStringSettingModel _herr0;
        private CCOLGeneratorCodeStringSettingModel _herr1;
        private CCOLGeneratorCodeStringSettingModel _herr2;

        private CCOLGeneratorCodeStringSettingModel _usoke;
        private CCOLGeneratorCodeStringSettingModel _userr;

        private CCOLGeneratorCodeStringSettingModel _prmiks;
        private CCOLGeneratorCodeStringSettingModel _prmuks;
        private CCOLGeneratorCodeStringSettingModel _prmoke;
        private CCOLGeneratorCodeStringSettingModel _prmerr;
        private CCOLGeneratorCodeStringSettingModel _prmerr0;
        private CCOLGeneratorCodeStringSettingModel _prmerr1;
        private CCOLGeneratorCodeStringSettingModel _prmerr2;

        private CCOLGeneratorCodeStringSettingModel _prmportnr;
        private CCOLGeneratorCodeStringSettingModel _prmbakportnr;
        private CCOLGeneratorCodeStringSettingModel _prmsrc;
        private CCOLGeneratorCodeStringSettingModel _prmdest;
        private CCOLGeneratorCodeStringSettingModel _prmtmsgw;
        private CCOLGeneratorCodeStringSettingModel _prmtmsgs;
        private CCOLGeneratorCodeStringSettingModel _prmtmsga;
        private CCOLGeneratorCodeStringSettingModel _prmcmsg;

        private CCOLGeneratorCodeStringSettingModel _isiks;
        private CCOLGeneratorCodeStringSettingModel _usuks;
#pragma warning restore 0649

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var k in c.PTPData.PTPKoppelingen)
            {
                for (var i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{k.TeKoppelenKruispunt}{_hiks}" + i.ToString("00"), _hiks, k.TeKoppelenKruispunt));
                }
                for (var i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{k.TeKoppelenKruispunt}{_huks}" + i.ToString("00"), _huks, k.TeKoppelenKruispunt));
                }
            }

            foreach (var k in c.PTPData.PTPKoppelingen.Where(x => !x.Dummy))
            {
                for (var i = 1; i <= k.AantalsignalenIn; ++i)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{k.TeKoppelenKruispunt}{_prmiks}" + i.ToString("00"), 
                            2, 
                            CCOLElementTimeTypeEnum.None, 
                            _prmiks, k.TeKoppelenKruispunt));
                }
                for (var i = 1; i <= k.AantalsignalenUit; ++i)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{k.TeKoppelenKruispunt}{_prmuks}" + i.ToString("00"),
                            2,
                            CCOLElementTimeTypeEnum.None,
                            _prmuks, k.TeKoppelenKruispunt));
                }

                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_hoke}",
                            _hoke, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr}",
                            _herr, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr0}",
                            _herr0, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr1}",
                            _herr1, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hptp}_{k.TeKoppelenKruispunt}{_herr2}",
                            _herr2, k.TeKoppelenKruispunt));

                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_usoke}",
                            _usoke, k.OkBitmapData, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_usptp}_{k.TeKoppelenKruispunt}{_userr}",
                            _userr, k.ErrorBitmapData, k.TeKoppelenKruispunt));

                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmoke}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            _prmoke, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            _prmerr, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr0}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            _prmerr0, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr1}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            _prmerr1, k.TeKoppelenKruispunt));
                _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmptp}_{k.TeKoppelenKruispunt}{_prmerr2}",
                            0,
                            CCOLElementTimeTypeEnum.None,
                            _prmerr2, k.TeKoppelenKruispunt));

                if (c.PTPData.PTPInstellingenInParameters)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmportnr}{k.TeKoppelenKruispunt}",
                            k.PortnummerAutomaatOmgeving,
                            CCOLElementTimeTypeEnum.None,
                            _prmportnr, k.TeKoppelenKruispunt));
                    if (int.TryParse(k.PortNaarBackupRegeling, out int bakpoort) && Enumerable.Range(0, 10).Contains(bakpoort))
                    {
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement(
                                $"{_prmbakportnr}{k.TeKoppelenKruispunt}bak",
                                bakpoort,
                                CCOLElementTimeTypeEnum.None,
                                _prmbakportnr, k.TeKoppelenKruispunt));
                    }
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmsrc}{k.TeKoppelenKruispunt}",
                            k.NummerSource,
                            CCOLElementTimeTypeEnum.None,
                            _prmsrc, k.TeKoppelenKruispunt));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmdest}{k.TeKoppelenKruispunt}",
                            k.NummerDestination,
                            CCOLElementTimeTypeEnum.None,
                            _prmdest, k.TeKoppelenKruispunt));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmtmsgw}{k.TeKoppelenKruispunt}",
                            200,
                            CCOLElementTimeTypeEnum.None,
                            _prmtmsgw, k.TeKoppelenKruispunt));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmtmsgs}{k.TeKoppelenKruispunt}",
                            10,
                            CCOLElementTimeTypeEnum.None,
                            _prmtmsgs, k.TeKoppelenKruispunt));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmtmsga}{k.TeKoppelenKruispunt}",
                            10,
                            CCOLElementTimeTypeEnum.None,
                            _prmtmsga, k.TeKoppelenKruispunt));
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmcmsg}{k.TeKoppelenKruispunt}",
                            3,
                            CCOLElementTimeTypeEnum.None,
                            _prmcmsg, k.TeKoppelenKruispunt));
                }
            }

            foreach (var k in c.PTPData.PTPKoppelingen.Where(x => x.Dummy && x.MaakIO))
            {
                int i = 1;
                foreach (var ptpio in k.PtpIoIngangen)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{k.TeKoppelenKruispunt}{_isiks}" + i.ToString("00"), _isiks, ptpio.PtpIoIsBitmapData, k.TeKoppelenKruispunt));
                    ++i;
                }
                i = 1;
                foreach (var ptpio in k.PtpIoUitgangen)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{k.TeKoppelenKruispunt}{_usuks}" + i.ToString("00"), _usuks, ptpio.PtpIoIsBitmapData, k.TeKoppelenKruispunt));
                    ++i;
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new []{20},
                CCOLCodeTypeEnum.RegCPreApplication => new []{10},
                CCOLCodeTypeEnum.RegCPostApplication => new[] { 180 },
                CCOLCodeTypeEnum.RegCPreSystemApplication => new[] { 10 },
              //CCOLCodeTypeEnum.RegCPostSystemApplication => new []{10},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    if ((c.PTPData.PTPKoppelingen.Count > 0) && c.PTPData.PTPKoppelingen.Any(x => !x.Dummy))
                    {
                        sb.AppendLine($"{ts}#include \"{c.Data.Naam}ptp.c\" /* PTP seriele koppeling */");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (c.PTPData.PTPKoppelingen.Any(x => x.Dummy && x.MaakIO))
                    {
                        sb.AppendLine($"{ts}/* overbrengen ingangen naar hulpelementen tbv koppeling */");
                    }
                    foreach (var k in c.PTPData.PTPKoppelingen.Where(x => x.Dummy && x.MaakIO))
                    {
                        for (var i = 1; i <= k.AantalsignalenIn; ++i)
                        {
                            sb.Append($"{ts}IH[{_hpf}{k.TeKoppelenKruispunt}{_hiks}" + i.ToString("00") + "]");
                            sb.AppendLine($" = IS[{_ispf}{k.TeKoppelenKruispunt}{_isiks}" + i.ToString("00") + "] & BIT0;");
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (c.PTPData.PTPKoppelingen.Any(x => x.Dummy && x.MaakIO))
                        sb.AppendLine($"{ts}/* overbrengen hulpelementen naar uitgangen tbv koppeling */");
                    foreach (var k in c.PTPData.PTPKoppelingen.Where(x => x.Dummy && x.MaakIO))
                    {
                        for (var i = 1; i <= k.AantalsignalenUit; ++i)
                        {
                            sb.Append($"{ts}CIF_GUS[{_uspf}{k.TeKoppelenKruispunt}{_usuks}" + i.ToString("00") + "]");
                            sb.AppendLine($" = IH[{_hpf}{k.TeKoppelenKruispunt}{_huks}" + i.ToString("00") + "];");
                        }
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreSystemApplication:
                    if ((c.PTPData.PTPKoppelingen.Count > 0) && c.PTPData.PTPKoppelingen.Any(x => !x.Dummy))
                    {
                        sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                        sb.AppendLine($"{ts}ptp_pre_system_app();");
                    }
                    return sb.ToString();
                //case CCOLCodeTypeEnum.RegCPostSystemApplication:
                //    if ((c.PTPData.PTPKoppelingen.Count > 0) && c.PTPData.PTPKoppelingen.Any(x => !x.Dummy))
                //    {
                //        sb.AppendLine($"{ts}/* aanroepen PTP loop tbv seriele koppeling */");
                //        sb.AppendLine($"{ts}ptp_post_system_app();");
                //    }
                //    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
