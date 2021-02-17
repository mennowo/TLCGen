using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    public static class PrioCodeGeneratorHelper
    {
        public static string GetDetectorTypeSCHString(PrioIngreepInUitMeldingVoorwaardeInputTypeEnum type)
        {
            return type switch
            {
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie => "SD",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp => "D",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet => "DB",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet => "SDB",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie => "ED",
                PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat => "ETDH",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static bool IsOV(this PrioIngreepModel prio)
        {
            return prio.Type == PrioIngreepVoertuigTypeEnum.Bus ||
                   prio.Type == PrioIngreepVoertuigTypeEnum.Tram;
        }
    }

    [CCOLCodePieceGenerator]
    public class PrioCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields
        private List<DetectorModel> _myDetectors;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _uskarog;
        private CCOLGeneratorCodeStringSettingModel _usmaxwt;
        private CCOLGeneratorCodeStringSettingModel _usovinm;
        private CCOLGeneratorCodeStringSettingModel _ushdinm;
        private CCOLGeneratorCodeStringSettingModel _cvc;
        private CCOLGeneratorCodeStringSettingModel _cvchd;
        private CCOLGeneratorCodeStringSettingModel _tgb;
        private CCOLGeneratorCodeStringSettingModel _tgbhd;
        private CCOLGeneratorCodeStringSettingModel _trt;
        private CCOLGeneratorCodeStringSettingModel _trthd;
        private CCOLGeneratorCodeStringSettingModel _tblk;
        private CCOLGeneratorCodeStringSettingModel _tbtovg;
        private CCOLGeneratorCodeStringSettingModel _tkarog;
        private CCOLGeneratorCodeStringSettingModel _tkarmelding;
        private CCOLGeneratorCodeStringSettingModel _tprioin;
        private CCOLGeneratorCodeStringSettingModel _tpriouit;
        private CCOLGeneratorCodeStringSettingModel _tovminrood;
        private CCOLGeneratorCodeStringSettingModel _thdin;
        private CCOLGeneratorCodeStringSettingModel _thduit;
        private CCOLGeneratorCodeStringSettingModel _hprio;
        private CCOLGeneratorCodeStringSettingModel _hhd;
        private CCOLGeneratorCodeStringSettingModel _hprioin;
        private CCOLGeneratorCodeStringSettingModel _hhdin;
        private CCOLGeneratorCodeStringSettingModel _hpriouit;
        private CCOLGeneratorCodeStringSettingModel _hhduit;
        private CCOLGeneratorCodeStringSettingModel _hwissel;
        private CCOLGeneratorCodeStringSettingModel _prmrto;
        private CCOLGeneratorCodeStringSettingModel _prmrtbg;
        private CCOLGeneratorCodeStringSettingModel _prmrtg;
        private CCOLGeneratorCodeStringSettingModel _prmrtohd;
        private CCOLGeneratorCodeStringSettingModel _prmrtbghd;
        private CCOLGeneratorCodeStringSettingModel _prmrtghd;
        private CCOLGeneratorCodeStringSettingModel _prmomx;
        private CCOLGeneratorCodeStringSettingModel _prmprio;
        private CCOLGeneratorCodeStringSettingModel _prmpriohd;
        private CCOLGeneratorCodeStringSettingModel _prmallelijnen;
        private CCOLGeneratorCodeStringSettingModel _prmlijn;
        private CCOLGeneratorCodeStringSettingModel _prmritcat;
        private CCOLGeneratorCodeStringSettingModel _prmmwta;
        private CCOLGeneratorCodeStringSettingModel _prmmwtfts;
        private CCOLGeneratorCodeStringSettingModel _prmmwtvtg;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsivert;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsilyn;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsicat;
        private CCOLGeneratorCodeStringSettingModel _prmpmgt;
        private CCOLGeneratorCodeStringSettingModel _prmognt;
        private CCOLGeneratorCodeStringSettingModel _prmnofm;
        private CCOLGeneratorCodeStringSettingModel _prmmgcov;
        private CCOLGeneratorCodeStringSettingModel _prmpmgcov;
        private CCOLGeneratorCodeStringSettingModel _prmohpmg;
        private CCOLGeneratorCodeStringSettingModel _schcheckdstype;
        private CCOLGeneratorCodeStringSettingModel _schprioin;
        private CCOLGeneratorCodeStringSettingModel _schpriouit;
        private CCOLGeneratorCodeStringSettingModel _schgeenwissel;
        private CCOLGeneratorCodeStringSettingModel _uskarmelding;
        private CCOLGeneratorCodeStringSettingModel _schupinagb;
        private CCOLGeneratorCodeStringSettingModel _schupinagbhd;
        private CCOLGeneratorCodeStringSettingModel _schvi;
        private CCOLGeneratorCodeStringSettingModel _schhdin;
        private CCOLGeneratorCodeStringSettingModel _schhduit;
        private CCOLGeneratorCodeStringSettingModel _schhdinuit;
        private CCOLGeneratorCodeStringSettingModel _schchecksirene;
        private CCOLGeneratorCodeStringSettingModel _schwisselpol;
        private CCOLGeneratorCodeStringSettingModel _schcovuber;
        private CCOLGeneratorCodeStringSettingModel _prmkarsg;
        private CCOLGeneratorCodeStringSettingModel _prmkarsghd;
        private CCOLGeneratorCodeStringSettingModel _hrisprio;
        private CCOLGeneratorCodeStringSettingModel _prmrisstart;
        private CCOLGeneratorCodeStringSettingModel _prmrisend;
        private CCOLGeneratorCodeStringSettingModel _prmriseta;
        private CCOLGeneratorCodeStringSettingModel _prmrisrole;
        private CCOLGeneratorCodeStringSettingModel _prmrissubrole;
        private CCOLGeneratorCodeStringSettingModel _prmrisstationtype;
        private CCOLGeneratorCodeStringSettingModel _tris;
        private CCOLGeneratorCodeStringSettingModel _prmftsblok;
        private CCOLGeneratorCodeStringSettingModel _prmftsmaxpercyc;
        private CCOLGeneratorCodeStringSettingModel _prmftsminvtg;
        private CCOLGeneratorCodeStringSettingModel _prmftsminwt;
        private CCOLGeneratorCodeStringSettingModel _cftsvtg;
        private CCOLGeneratorCodeStringSettingModel _cftscyc;

#pragma warning restore 0649

        private string _tnlfg;
        private string _tnlfgd;
        private string _tnlsg;
        private string _tnlsgd;
        private string _tnlcv;
        private string _tnlcvd;
        private string _tnleg;
        private string _tnlegd;
        private string _mwtvm;
        private string _prmwtvnhaltmin;
        private string _prmrislaneid;
        private string _prmrisapproachid;
        private string _hperiod;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        private void AddAllHDIngreepElements(HDIngreepModel hd, ControllerModel c)
        {
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ushdinm}{hd.FaseCyclus}", _ushdinm, hd.HDInmeldingBitmapData, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhd}{hd.FaseCyclus}", _hhd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}", _hhdin, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}", _hhduit, hd.FaseCyclus));

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tgbhd}{hd.FaseCyclus}", hd.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tgbhd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_trthd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _trthd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvchd}{hd.FaseCyclus}", 999, CCOLElementTimeTypeEnum.CT_type, _cvchd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriohd}{hd.FaseCyclus}", 9005, CCOLElementTimeTypeEnum.None, _prmpriohd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtohd}{hd.FaseCyclus}", hd.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtohd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtbghd}{hd.FaseCyclus}", hd.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtbghd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtghd}{hd.FaseCyclus}", hd.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtghd, hd.FaseCyclus));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schupinagbhd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.SCH_type, _schupinagbhd, hd.FaseCyclus));

            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{hd.FaseCyclus}hd", 0, CCOLElementTimeTypeEnum.TE_type, _tbtovg, hd.FaseCyclus));

            if (hd.KAR)
            {
                _myDetectors.Add(hd.DummyKARInmelding);
                _myDetectors.Add(hd.DummyKARUitmelding);

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}kar", _hhdin, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}kar", hd.KARInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}kar", _hhduit, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thduit}{hd.FaseCyclus}kar", hd.KARUitmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thduit, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "KAR"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schchecksirene}{hd.FaseCyclus}", hd.Sirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schchecksirene, hd.FaseCyclus));
            }

            if (hd.RIS)
            {
                var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == hd.FaseCyclus);
                if (fcRis != null && fcRis.LaneData.Count > 0)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}ris", _hhdin, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}ris", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}ris", _hhduit, hd.FaseCyclus, "RIS"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}ris", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "RIS"));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstart}{hd.FaseCyclus}hd", hd.RisStart, CCOLElementTimeTypeEnum.None, _prmrisstart, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisend}{hd.FaseCyclus}hd", hd.RisEnd, CCOLElementTimeTypeEnum.None, _prmrisend, hd.FaseCyclus));
                    if (hd.RisEta.HasValue)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmriseta}{hd.FaseCyclus}hd", hd.RisEta.Value, CCOLElementTimeTypeEnum.None, _prmriseta, hd.FaseCyclus));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisrole}{hd.FaseCyclus}hd", 64, CCOLElementTimeTypeEnum.None, _prmrisrole, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrissubrole}{hd.FaseCyclus}hd", 32, CCOLElementTimeTypeEnum.None, _prmrissubrole, hd.FaseCyclus));
                   
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstationtype}{hd.FaseCyclus}hd", 0x0400, CCOLElementTimeTypeEnum.None, _prmrisstationtype, hd.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisapproachid}{hd.FaseCyclus}hd", fcRis.ApproachID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, ""));

                    foreach (var lane in fcRis.LaneData)
                    {
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrislaneid}{hd.FaseCyclus}hd_{lane.RijstrookIndex}", lane.LaneID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, ""));
                    }
                }
            }

            if (hd.Opticom && !string.IsNullOrWhiteSpace(hd.OpticomRelatedInput))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}opt", _hhdin, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}opt", hd.OpticomInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdinuit}{hd.FaseCyclus}opt", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdinuit, hd.FaseCyclus, "Opticom"));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}opt", _hhduit, hd.FaseCyclus, "Opticom"));
            }
        }

        private void AddAllIngreepElements(PrioIngreepModel prio, bool prioUitgangPerFase, ControllerModel c)
        {
            // Verklikken prio
            if (!prioUitgangPerFase)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{CCOLCodeHelper.GetPriorityName(prio)}", _usovinm, prio.PrioInmeldingBitmapData, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            // Hulp elementen prio
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprio}{CCOLCodeHelper.GetPriorityName(prio)}", _hprio, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioin}{CCOLCodeHelper.GetPriorityName(prio)}", _hprioin, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriouit}{CCOLCodeHelper.GetPriorityName(prio)}", _hpriouit, prio.FaseCyclus, prio.Type.GetDescription()));

            // Elementen voor in- en uitmeldingen
            foreach (var melding in prio.MeldingenData.Inmeldingen.Concat(prio.MeldingenData.Uitmeldingen))
            {
                _myElements.AddRange(GetMeldingElements(prio, melding, true));
            }

            // Anti-jutter timers indien actief voor alle in- of uitmeldingen gezamenlijk
            if (prio.MeldingenData.AntiJutterVoorAlleInmeldingen && prio.MeldingenData.Inmeldingen.Any())
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioin}{CCOLCodeHelper.GetPriorityName(prio)}", prio.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tprioin, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if (prio.MeldingenData.AntiJutterVoorAlleUitmeldingen && prio.MeldingenData.Uitmeldingen.Any())
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriouit}{CCOLCodeHelper.GetPriorityName(prio)}", prio.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            // Generieke elementen: voor elke ingreep
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{CCOLCodeHelper.GetPriorityName(prio)}", prio.BezettijdPrioGehinderd, CCOLElementTimeTypeEnum.TE_type, _tbtovg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_trt}{CCOLCodeHelper.GetPriorityName(prio)}", 0, CCOLElementTimeTypeEnum.TE_type, _trt, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvc}{CCOLCodeHelper.GetPriorityName(prio)}", 999, CCOLElementTimeTypeEnum.CT_type, _cvc, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tgb}{CCOLCodeHelper.GetPriorityName(prio)}", prio.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tgb, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrto}{CCOLCodeHelper.GetPriorityName(prio)}", prio.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrto, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtbg}{CCOLCodeHelper.GetPriorityName(prio)}", prio.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtbg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtg}{CCOLCodeHelper.GetPriorityName(prio)}", prio.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtg, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmomx}{CCOLCodeHelper.GetPriorityName(prio)}", prio.OnderMaximum, CCOLElementTimeTypeEnum.TE_type, _prmomx, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tblk}{CCOLCodeHelper.GetPriorityName(prio)}", prio.BlokkeertijdNaPrioIngreep, CCOLElementTimeTypeEnum.TE_type, _tblk, prio.FaseCyclus, prio.Type.GetDescription()));
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schupinagb}{CCOLCodeHelper.GetPriorityName(prio)}", 0, CCOLElementTimeTypeEnum.SCH_type, _schupinagb, prio.FaseCyclus, prio.Type.GetDescription()));
            if ((prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ||
                 prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchUit) &&
                !string.IsNullOrWhiteSpace(prio.Koplus) && prio.Koplus != "NG")
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvi}{CCOLCodeHelper.GetPriorityName(prio)}", prio.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvi, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            var opties = 0;
            if (prio.AfkappenConflicten || prio.AfkappenConflictenPrio) opties += 100;
            if (prio.AfkappenConflictenPrio) opties += 300;
            if (prio.TussendoorRealiseren) opties += 3;
            if (prio.VasthoudenGroen) opties += 20;
            var sopties = opties == 0 ? "0" : opties.ToString().Replace("0", "");
            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprio}{CCOLCodeHelper.GetPriorityName(prio)}", int.Parse(sopties), CCOLElementTimeTypeEnum.None, _prmprio, prio.FaseCyclus, prio.Type.GetDescription()));

            if (prio.CheckLijnNummer)
            {
                // !!! NOTE !!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmallelijnen}{CCOLCodeHelper.GetPriorityName(prio)}", prio.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None, _prmallelijnen, prio.FaseCyclus, prio.Type.GetDescription()));
                var n = 1;
                foreach (var l in prio.LijnNummers)
                {
                    if (!int.TryParse(l.Nummer, out var num)) continue;
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmlijn}{CCOLCodeHelper.GetPriorityName(prio)}_{n:00}", num, CCOLElementTimeTypeEnum.None, _prmlijn, n.ToString(), prio.FaseCyclus, prio.Type.GetDescription()));
                    ++n;
                }

                if (prio.CheckRitCategorie)
                {
                    n = 1;
                    foreach (var l in prio.LijnNummers)
                    {
                        if (!int.TryParse(l.RitCategorie, out var ritcat)) continue;
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmritcat}{CCOLCodeHelper.GetPriorityName(prio)}_{n:00}", ritcat, CCOLElementTimeTypeEnum.None, _prmritcat, n.ToString(), prio.FaseCyclus, prio.Type.GetDescription()));
                        ++n;
                    }
                }
            }

            // Help elements to store wissel condition
            if (prio.Type == PrioIngreepVoertuigTypeEnum.Tram && prio.HasOVIngreepWissel())
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hwissel}{CCOLCodeHelper.GetPriorityName(prio)}", _hwissel, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            if (prio.HasPrioIngreepKAR())
            {
                _myDetectors.AddRange(prio.GetDummyInDetectors());
                _myDetectors.AddRange(prio.GetDummyUitDetectors());
            }

            if (prio.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || prio.NoodaanvraagKoplus)
            {
                _myElements.Add(
                    CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovminrood}{CCOLCodeHelper.GetPriorityName(prio)}", prio.MinimaleRoodtijd, CCOLElementTimeTypeEnum.TE_type, _tovminrood, prio.FaseCyclus, prio.Type.GetDescription()));
            }

            var inRis = prio.MeldingenData.Inmeldingen.Where(x =>
                x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde).ToList();
            var uitRis = prio.MeldingenData.Uitmeldingen.Where(x =>
                x.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde).ToList();

            if (inRis.Any() || uitRis.Any())
            {
                foreach (var inR in inRis)
                {
                    var fcRis = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == prio.FaseCyclus);

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstart}{CCOLCodeHelper.GetPriorityName(prio)}", inR.RisStart, CCOLElementTimeTypeEnum.None, _prmrisstart, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisend}{CCOLCodeHelper.GetPriorityName(prio)}", inR.RisEnd, CCOLElementTimeTypeEnum.None, _prmrisend, prio.FaseCyclus));
                    if (inR.RisEta.HasValue)
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmriseta}{CCOLCodeHelper.GetPriorityName(prio)}", inR.RisEta.Value, CCOLElementTimeTypeEnum.None, _prmriseta, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisrole}{CCOLCodeHelper.GetPriorityName(prio)}", (int)inR.RisRole, CCOLElementTimeTypeEnum.None, _prmrisrole, prio.FaseCyclus));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrissubrole}{CCOLCodeHelper.GetPriorityName(prio)}", (int)inR.RisSubrole, CCOLElementTimeTypeEnum.None, _prmrissubrole, prio.FaseCyclus));

                    var stationtype = prio.Type
                        switch
                        {
                            PrioIngreepVoertuigTypeEnum.Bus => 0x0040,
                            PrioIngreepVoertuigTypeEnum.Tram => 0x0800,
                            PrioIngreepVoertuigTypeEnum.Fiets => 0x0004,
                            PrioIngreepVoertuigTypeEnum.Vrachtwagen => 0x0080 | 0x0100 | 0x0200,
                            PrioIngreepVoertuigTypeEnum.Auto => 0x0020,
                            PrioIngreepVoertuigTypeEnum.NG => 0x0001,
                            _ => 0x0001
                        };

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrisstationtype}{CCOLCodeHelper.GetPriorityName(prio)}", stationtype, CCOLElementTimeTypeEnum.None, _prmrisstationtype, prio.FaseCyclus));
                    
                    if (fcRis != null)
                    {
                        var approach = $"{_prmrisapproachid}{CCOLCodeHelper.GetPriorityName(prio)}";
                        if (_myElements.All(x => x.Naam != approach))
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(approach, fcRis.ApproachID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, ""));
                        }
                        foreach (var lane in fcRis.LaneData)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrislaneid}{CCOLCodeHelper.GetPriorityName(prio)}_{lane.RijstrookIndex}", lane.LaneID, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter, ""));
                        }
                    }
                }

                foreach (var inR in uitRis)
                {

                }
            }
        }
        
        private IEnumerable<CCOLElement> GetMeldingElements(PrioIngreepModel prio, PrioIngreepInUitMeldingModel melding, bool addHov)
        {
            var elements = new List<CCOLElement>();
            string hprio;
            string schprio;
            string tprio;

            // type melding
            switch (melding.InUit)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    hprio = _hprioin.ToString();
                    schprio = _schprioin.ToString();
                    tprio = _tprioin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hprio = _hpriouit.ToString();
                    schprio = _schpriouit.ToString();
                    tprio = _tpriouit.ToString();
                    break;
            }

            var he = $"{hprio}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var ti = $"{tprio}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var sw = $"{schprio}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                he += melding.RelatedInput1;
                ti += melding.RelatedInput1;
                sw = sw + melding.RelatedInput1;
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
                {
                    sw += PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput1Type);
                }
                if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
                {
                    he += melding.RelatedInput2;
                    ti += melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }

            if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftscyc}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, _cftscyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cftsvtg}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, _cftsvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsblok}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitBlok, CCOLElementTimeTypeEnum.None, _prmftsblok, prio.FaseCyclus));
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsmaxpercyc}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitAantalKeerPerCyclus, CCOLElementTimeTypeEnum.None, _prmftsmaxpercyc, prio.FaseCyclus));
                if (melding.FietsPrioriteitGebruikLus)
                {
                    elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminvtg}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitMinimumAantalVoertuigen, CCOLElementTimeTypeEnum.None, _prmftsminvtg, prio.FaseCyclus));
                }
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmftsminwt}{CCOLCodeHelper.GetPriorityName(prio)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}", melding.FietsPrioriteitMinimumWachttijdVoorPrioriteit, CCOLElementTimeTypeEnum.TS_type, _prmftsminwt, prio.FaseCyclus));
            }

            if (addHov)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(he,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioin : _hpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            if (melding.AntiJutterTijdToepassen)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(ti, melding.AntiJutterTijd, CCOLElementTimeTypeEnum.TE_type,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _tprioin : _tpriouit, prio.FaseCyclus, prio.Type.GetDescription()));
            }
            elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(sw, 1, CCOLElementTimeTypeEnum.SCH_type,
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _schprioin : _schpriouit, prio.FaseCyclus, prio.Type.GetDescription()));

            if (melding.OpvangStoring && melding.MeldingBijstoring != null)
            {
                var elems = GetMeldingElements(prio, melding.MeldingBijstoring, false);
                foreach (var e in elems)
                {
                    if (_myElements.All(x => x.Naam != e.Naam))
                    {
                        _myElements.Add(e);
                    }
                }
            }

            return elements;
        }

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
            _myDetectors = new List<DetectorModel>();

            if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.GeneriekePrioriteit) return;

            if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan || c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchUit)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcovuber}", c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcovuber));
            }

            if (c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any())
            {
                /* Variables independent of signal groups */
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwta}", c.PrioData.MaxWachttijdAuto, CCOLElementTimeTypeEnum.TS_type, _prmmwta));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtfts}", c.PrioData.MaxWachttijdFiets, CCOLElementTimeTypeEnum.TS_type, _prmmwtfts));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtvtg}", c.PrioData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type, _prmmwtvtg));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmaxwt}", _usmaxwt, c.PrioData.MaximaleWachttijdOverschredenBitmapData));

                if (c.HasKAR())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarmelding}", _uskarmelding, c.PrioData.KARMeldingBitmapData));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarog}", _uskarog, c.PrioData.KAROnderGedragBitmapData));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarmelding}", 15, CCOLElementTimeTypeEnum.TE_type, _tkarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarog}", 1440, CCOLElementTimeTypeEnum.TM_type, _tkarog));
                }
            }
            if (c.PrioData.PrioIngrepen.Any())
            {
                /* Variables independent of signal groups */

                if (c.HasDSI())
                {
                    var prmtest1 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsivert}", 120, CCOLElementTimeTypeEnum.None, _prmtestdsivert);
                    var prmtest2 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsilyn}", 0, CCOLElementTimeTypeEnum.None, _prmtestdsilyn);
                    var prmtest3 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestdsicat}", 10, CCOLElementTimeTypeEnum.None, _prmtestdsicat);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    prmtest3.Dummy = true;
                    _myElements.Add(prmtest1);
                    _myElements.Add(prmtest2);
                    _myElements.Add(prmtest3);
                }

                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schgeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schgeenwissel, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schgeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schgeenwissel, d.Naam));
                }
                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schwisselpol, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schwisselpol, d.Naam));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcheckdstype}", c.PrioData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcheckdstype));

                /* Variables for conflicting signal groups */
                if (!c.PrioData.PrioIngreepSignaalGroepParametersHard)
                {
                    foreach (var ovfc in c.PrioData.PrioIngreepSignaalGroepParameters)
                    {
                        if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                        {
                            continue;
                        }

                        var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;

                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmpmgt, ovfc.FaseCyclus));
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmognt, ovfc.FaseCyclus));
                        if (fct != FaseTypeEnum.Voetganger)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmnofm}{ovfc.FaseCyclus}", ovfc.AantalKerenNietAfkappen, CCOLElementTimeTypeEnum.TE_type, _prmnofm, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmgcov}{ovfc.FaseCyclus}", ovfc.MinimumGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.TE_type, _prmmgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgcov}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.None, _prmpmgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmohpmg}{ovfc.FaseCyclus}", ovfc.OphoogpercentageNaAfkappen, CCOLElementTimeTypeEnum.None, _prmohpmg, ovfc.FaseCyclus));
                        }
                    }
                }
            }

            if (c.PrioData.PrioUitgangPerFase)
            {
                foreach (var sg in c.Fasen.Where(x => x.PrioIngreep))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{sg.Naam}", _usovinm, sg.PrioIngreepBitmapData, sg.Naam, ""));
                }
            }

            /* Variables for OV */
            foreach (var prio in c.PrioData.PrioIngrepen)
            {
                AddAllIngreepElements(prio, c.PrioData.PrioUitgangPerFase, c);
            }

            /* Variables for HD */
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                AddAllHDIngreepElements(hd, c);
            }

            // RIS prms per fase
            var ovRis = c.PrioData.PrioIngrepen
                .Where(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                            x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                .ToList();
            if (ovRis.Any())
            {
                foreach (var ov in ovRis)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tris}{ov.FaseCyclus}{ov.Naam}", 0, CCOLElementTimeTypeEnum.TE_type, _tris, ov.FaseCyclus));
                }
            }

            if (c.PrioData.KARSignaalGroepNummersInParameters)
            {
                foreach (var sg in c.Fasen)
                {
                    var hasOvKar = c.PrioData.PrioIngrepen.Any(x => x.FaseCyclus == sg.Naam && x.HasPrioIngreepKAR());
                    if (hasOvKar)
                    {
                        if (!int.TryParse(sg.Naam, out var iFc)) continue;
                        if (c.PrioData.VerlaagHogeSignaalGroepNummers && iFc > 200) iFc -= 200;
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_prmkarsg}{sg.Naam}", iFc, CCOLElementTimeTypeEnum.None, _prmkarsg, sg.Naam));
                    }
                }
                foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                {
                    if (!int.TryParse(hd.FaseCyclus, out var iFc)) continue;
                    if (c.PrioData.VerlaagHogeSignaalGroepNummers && iFc > 200) iFc -= 200;
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(
                        $"{_prmkarsghd}{hd.FaseCyclus}", iFc, CCOLElementTimeTypeEnum.None, _prmkarsghd, hd.FaseCyclus));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<DetectorModel> GetDetectors() => _myDetectors;

        public override bool HasDetectors() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return base.GetFunctionLocalVariables(c, type);

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return new List<CCOLLocalVariable>
                    {
                        new CCOLLocalVariable("int", "ov", "0")
                    };
            
                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
                    {
                        var result2 = new List<CCOLLocalVariable> {new CCOLLocalVariable(c.GetBoolV(), "isHD", "FALSE")};
                        if (c.Fasen.Any(x => x.WachttijdVoorspeller)) result2.Add(new CCOLLocalVariable(c.GetBoolV(), "isWTV", "FALSE"));
                        return result2;
                    }
                    else return base.GetFunctionLocalVariables(c, type);

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    if (c.PrioData.PrioIngrepen.Any(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                         x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
                    {
                        return new List<CCOLLocalVariable> {new CCOLLocalVariable("int", "i", "0")};
                    }
                    else return base.GetFunctionLocalVariables(c, type);

                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => 61,
                CCOLCodeTypeEnum.RegCPreApplication => 41,
                CCOLCodeTypeEnum.RegCSystemApplication => 41,
                CCOLCodeTypeEnum.RegCPostSystemApplication => 31,
                CCOLCodeTypeEnum.PrioCInUitMelden => 11,
                CCOLCodeTypeEnum.PrioCTegenhoudenConflicten => 10,
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio => 11,
                _ => 0
            };
        }

        private static string GetMeldingDetectieCode(PrioIngreepInUitMeldingModel melding, string dpf)
        {
            var sb = new StringBuilder();
            switch (melding.RelatedInput1Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    sb.Append($"SD[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    sb.Append($"D[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    sb.Append($"DB[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    sb.Append($"!DB_old[{dpf}{melding.RelatedInput1}] && DB[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    sb.Append($"ED[{dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    sb.Append($"TDH_old[{dpf}{melding.RelatedInput1}] && !TDH[{dpf}{melding.RelatedInput1}]");
                    break;
            }
            if (melding.TweedeInput)
            {
                switch (melding.RelatedInput2Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                        sb.Append($" && SD[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                        sb.Append($" && D[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                        sb.Append($" && DB[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                        sb.Append($" && !DB_old[{dpf}{melding.RelatedInput2}] && DB[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                        sb.Append($" && ED[{dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                        sb.Append($" && TDH_old[{dpf}{melding.RelatedInput2}] && !TDH[{dpf}{melding.RelatedInput2}]");
                        break;
                }
            }
            return sb.ToString();
        }

        private List<string> GetMeldingCode(ControllerModel c, PrioIngreepModel ov, PrioIngreepInUitMeldingModel melding, StringBuilder sb, string vtgType, int fcNmr, string ts, bool antiJutVoorAlles, bool opvang = false, string otherHov = null)
        {
            var inmHelems = new List<string>();
            string hov;
            string schov;
            string tov;

            // type melding
            switch (melding.InUit)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    hov = _hprioin.ToString();
                    schov = _schprioin.ToString();
                    tov = _tprioin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hpriouit.ToString();
                    schov = _schpriouit.ToString();
                    tov = _tpriouit.ToString();
                    break;
            }

            var he = $"{_hpf}{hov}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var ti = $"{_tpf}{tov}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            var sw = $"{_schpf}{schov}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding &&
                melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                he += melding.RelatedInput1;
                ti += melding.RelatedInput1;
                sw += melding.RelatedInput1;
                if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
                {
                    sw += PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput1Type);
                }
                if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
                {
                    he += melding.RelatedInput2;
                    ti += melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + PrioCodeGeneratorHelper.GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }
            if (otherHov != null) he = otherHov;
            else inmHelems.Add(he);

            var tts = ts;

            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.AppendLine($"#ifndef NO_RIS");
            }

            if (!opvang)
            {
                tts = ts + ts;
                sb.Append($"{ts}if (SCH[{sw}]");
            }
            else if (opvang)
            {
                tts = ts + ts;
                sb.Append($"{ts}else if (SCH[{sw}]");
            }
            switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                        if (melding.TweedeInput && !string.IsNullOrWhiteSpace(melding.RelatedInput2))
                        {
                            sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING)");
                        }
                    }
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                    }
                    break;
            }
            sb.AppendLine(")");
            sb.AppendLine($"{ts}{{");

            sb.Append($"{tts}IH[{he}] = ");
            if (melding.AntiJutterTijdToepassen)
            {
                sb.Append($"RT[{ti}] = ");
            }
            if (antiJutVoorAlles && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.Append($"!T[{_tpf}{tov}{CCOLCodeHelper.GetPriorityName(ov)}] && ");
            }
            if (melding.AntiJutterTijdToepassen && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding ||
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding && melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton)
            {
                sb.Append($"!C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(ov)}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(ov)}] && ");
            }
            if (melding.AlleenIndienRood)
            {
                sb.Append($"R[{_fcpf}{ov.FaseCyclus}] && !T[{_tpf}{_tovminrood}{CCOLCodeHelper.GetPriorityName(ov)}] && ");
            }

            var extra = "";
            if ((melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector ||
                 melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding) &&
                ov.CheckLijnNummer && ov.LijnNummers.Any())
            {
                if (!ov.CheckRitCategorie || melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector)
                {
                    extra += "DSIMeldingPRIO_LijnNummer_V1(" +
                             $"{_prmpf + _prmallelijnen + CCOLCodeHelper.GetPriorityName(ov)}, " +
                             $"{ov.LijnNummers.Count})";
                }
                else
                {
                    extra += "DSIMeldingPRIO_LijnNummerEnRitCategorie_V1(" +
                             $"{_prmpf + _prmallelijnen + CCOLCodeHelper.GetPriorityName(ov)}, " +
                             $"{ov.LijnNummers.Count})";
                }
            }
            if (ov.CheckWagenNummer)
            {
                extra += (extra == "" ? "" : " && ");
                extra += (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? $"WDNST_check_in({_fcpf}{ov.FaseCyclus})" : $"WDNST_check_uit({_fcpf}{ov.FaseCyclus})");
            }
            if (extra == "") extra = "TRUE";

            switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters
                            ? $"PRM[{_prmpf}{_prmkarsg}{ov.FaseCyclus}]"
                            : fcNmr > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (fcNmr - 200).ToString() : fcNmr.ToString();
                    sb.AppendLine($"DSIMeldingPRIO_V1(0, " +
                                                    $"{vtgType}, " +
                                                    "TRUE, " +
                                                    $"{(fcNmr == -1 ? "NG" : sgCheck)}," +
                                                    "TRUE, " +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding
                                                        ? "CIF_DSIN, " 
                                                        : "CIF_DSUIT, ") +
                                                    $"{extra});");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    sb.AppendLine($"DSIMeldingPRIO_V1({(_dpf + melding.RelatedInput1).ToUpper()}, " +
                                                    $"{vtgType}, " +
                                                    "FALSE, " +
                                                    "NG, " +
                                                    $"SCH[{_schpf}{_schcheckdstype}], " +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ||
                                                     melding.CheckAltijdOpDsinBijVecom 
                                                        ? "CIF_DSIN, " 
                                                        : "CIF_DSUIT, ") +
                                                    $"{extra});");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    sb.AppendLine(GetMeldingDetectieCode(melding, _dpf) + ";");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    sb.AppendLine($" SD[{_dpf}{melding.RelatedInput1}];");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.FietsMassaPeloton:
                    if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding)
                    {
                        sb.AppendLine($"fietsprio_inmelding({_fcpf}{ov.FaseCyclus}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_dpf}{melding.RelatedInput1}, " : "NG, ") +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_ctpf}{_cftsvtg}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " : "NG, ") +
                                      $"{_ctpf}{_cftscyc}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"{_prmpf}{_prmftsblok}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"{_prmpf}{_prmftsmaxpercyc}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      (melding.FietsPrioriteitGebruikLus ? $"{_prmpf}{_prmftsminvtg}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " : "NG, ") +
                                      $"{_prmpf}{_prmftsminwt}{CCOLCodeHelper.GetPriorityName(ov)}{DefaultsProvider.Default.GetMeldingShortcode(melding)}, " +
                                      $"SH[{he}], ML);");
                    }
                    else
                    {
                        sb.AppendLine($"C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(ov)}] && G[{_fcpf}{ov.FaseCyclus}];");
                    }
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde:
                    switch (melding.InUit)
                    {
                        case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                            var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == ov.FaseCyclus);
                            var first = true;
                            
                            if (risFc != null)
                            {
                                sb.AppendLine();
                                foreach (var lane in risFc.LaneData)
                                {
                                    if (!first) sb.AppendLine(" ||");
                                    var itf = c.RISData.HasMultipleSystemITF
                                        ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == lane.SystemITF) : -1;
                                    sb.Append($"{ts}{ts}{ts}ris_inmelding_selectief(" +
                                              $"{_fcpf}{ov.FaseCyclus}, " +
                                              $"PRM[{_prmpf}{_prmrisapproachid}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                              $"PRM[{_prmpf}{_prmrislaneid}{CCOLCodeHelper.GetPriorityName(ov)}_{lane.RijstrookIndex}], " +
                                              $"PRM[{_prmpf}{_prmrisstationtype}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"PRM[{_prmpf}{_prmrisstart}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"PRM[{_prmpf}{_prmrisend}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"PRM[{_prmpf}{_prmrisrole}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"PRM[{_prmpf}{_prmrissubrole}{CCOLCodeHelper.GetPriorityName(ov)}], " +
                                              $"{(melding.RisEta.HasValue ? $"PRM[{_prmpf}{_prmriseta}{CCOLCodeHelper.GetPriorityName(ov)}]" : "NG")}, " +
                                              $"prioFC{CCOLCodeHelper.GetPriorityName(ov)})");
                                    first = false;
                                }
                                sb.AppendLine(";");
                            }
                            break;
                        case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                            sb.AppendLine($"ris_uitmelding_selectief(prioFC{CCOLCodeHelper.GetPriorityName(ov)});");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }
            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                if (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding)
                    sb.AppendLine($"{ts}{ts}if (IH[{inmHelems.Last()}]) iPrioriteitNooitAfkappen[prioFC{CCOLCodeHelper.GetPriorityName(ov)}] = TRUE;");
                else
                    sb.AppendLine($"{ts}{ts}if (IH[{inmHelems.Last()}] && iAantalInmeldingen[prioFC{CCOLCodeHelper.GetPriorityName(ov)}] == 0) iPrioriteitNooitAfkappen[prioFC{CCOLCodeHelper.GetPriorityName(ov)}] = FALSE;");
            }
            sb.AppendLine($"{ts}}}");
            if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)
            {
                sb.AppendLine($"#endif /* NO_RIS */");
            }
            if (melding.OpvangStoring && melding.MeldingBijstoring != null && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                inmHelems.AddRange(GetMeldingCode(c, ov, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return inmHelems;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return null;

            var sb = new StringBuilder();
            var first = false;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("mulv C_counter_old[CTMAX];");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:

                    var start = true;
                    if(c.PrioData.PrioIngrepen.Any(x => x.CheckWagenNummer))
                    {
                        sb.AppendLine($"{ts}/* Opschonen wagennummer buffers */");
                        sb.AppendLine($"{ts}WDNST_cleanup();");
                        start = false;
                    }
                    if (c.PrioData.PrioIngrepen.Any(ov => ov.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || ov.NoodaanvraagKoplus))
                    {
                        if (!start) sb.AppendLine();
                        sb.AppendLine($"{ts}/* Herstarting meting roodtijd tbv minimale roodtijd OV */");
                        foreach (var ov in c.PrioData.PrioIngrepen)
                        {
                            if (ov.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || ov.NoodaanvraagKoplus)
                            {
                                sb.AppendLine($"{ts}RT[{_tpf}{_tovminrood}{CCOLCodeHelper.GetPriorityName(ov)}] = !R[{_fcpf}{ov.FaseCyclus}];");
                            }
                        }
                        start = false;
                    }
                    if (c.PrioData.PrioIngrepen.Any(x => x.HasOVIngreepWissel()))
                    {
                        sb.AppendLine($"{ts}/* Onthouden wissel voorwaarden per fasecyclus */");
                        foreach (var ov in c.PrioData.PrioIngrepen)
                        {
                            if (ov.HasOVIngreepWissel())
                            {
                                sb.Append($"{ts}IH[{_hpf}{_hwissel}{CCOLCodeHelper.GetPriorityName(ov)}] = (");
                                if (ov.MeldingenData.Wissel1)
                                {
                                    if (ov.MeldingenData.Wissel1Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
                                    {
                                        sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Input}])" :
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Input}])");
                                    }
                                    else
                                    {
                                        sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Detector}])" :
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel1Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel1Detector}])");
                                    }
                                }
                                if (ov.MeldingenData.Wissel2)
                                {
                                    if (ov.MeldingenData.Wissel1)
                                    {
                                        sb.Append(" && ");
                                    }
                                    if (ov.MeldingenData.Wissel2Type == PrioIngreepInUitDataWisselTypeEnum.Ingang)
                                    {
                                        sb.Append(ov.MeldingenData.Wissel2InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Input}])" :
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Input}])");
                                    }
                                    else
                                    {
                                        sb.Append(ov.MeldingenData.Wissel2InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Detector}])" :
                                            $"((SCH[{_schpf}{_schwisselpol}{ov.MeldingenData.Wissel2Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schgeenwissel}{ov.MeldingenData.Wissel2Detector}])");
                                    }
                                }
                                sb.AppendLine(");");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    sb.AppendLine($"{ts}/* PRIO verklikking */");
                    sb.AppendLine($"{ts}/* ---------------- */");
                    if (!c.PrioData.PrioUitgangPerFase)
                    {
                        foreach (var ov in c.PrioData.PrioIngrepen)
                        {
                            sb.AppendLine(
                                $"{ts}CIF_GUS[{_uspf}{_usovinm}{CCOLCodeHelper.GetPriorityName(ov)}] = C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(ov)}];");
                        }
                    }
                    else
                    {
                        foreach (var sg in c.Fasen.Where(x => x.PrioIngreep))
                        {
                            sb.Append(
                                $"{ts}CIF_GUS[{_uspf}{_usovinm}{sg.Naam}] = ");
                            var firstSg = true;
                            foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.FaseCyclus == sg.Naam))
                            {
                                if (!firstSg) sb.Append(" || ");
                                sb.Append($"C[{_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(ov)}]");
                                firstSg = false;
                            }
                            sb.AppendLine(";");
                        }
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_ushdinm}{hd.FaseCyclus}] = C[{_ctpf}{_cvchd}{hd.FaseCyclus}];");
                    }
                    if (c.HasKAR())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Verklikken melding en ondergedrag KAR */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarmelding}] = T[{_tpf}{_tkarmelding}];");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarog}] = !T[{_tpf}{_tkarog}];");
                    }
                    if (c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Verklikken overschreiding maximale wachttijd */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usmaxwt}] = FALSE;");
                        sb.AppendLine($"{ts}for (ov = 0; ov < prioFCMAX; ++ov)");
                        sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_usmaxwt}] |= iMaximumWachtTijdOverschreden[ov];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Nooit) return "";
                    sb.AppendLine($"{ts}/* Verklikken wijzigingen OV-teller */");
                    var sch = c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Altijd ? "NG" : $"{_schpf}{_schcovuber}";
                    foreach (var ov in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}PRIO_teller({_ctpf}{_cvc}{CCOLCodeHelper.GetPriorityName(ov)}, {sch});");
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}PRIO_teller({_ctpf}{_cvchd}{hd.FaseCyclus}, {sch});");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    foreach (var prio in c.PrioData.PrioIngrepen)
                    {
                        var vtgType = "";
                        if (!int.TryParse(prio.FaseCyclus, out var fcNmr)) fcNmr = -1;
                        vtgType = prio.Type switch
                        {
                            PrioIngreepVoertuigTypeEnum.Tram => "CIF_TRAM",
                            PrioIngreepVoertuigTypeEnum.Bus => "CIF_BUS",
                            PrioIngreepVoertuigTypeEnum.Vrachtwagen => "NG",
                            PrioIngreepVoertuigTypeEnum.Fiets => "NG",
                            PrioIngreepVoertuigTypeEnum.NG => "NG",
                            _ => throw new IndexOutOfRangeException()
                        };

                        if (prio.MeldingenData.Inmeldingen.Any())
                        {
                            var inmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Inmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");

                            var sb2 = new StringBuilder();
                            foreach (var inm in prio.MeldingenData.Inmeldingen)
                            {
                                var hulps = GetMeldingCode(c, prio, inm, sb2, vtgType, fcNmr, ts, prio.MeldingenData.AntiJutterVoorAlleInmeldingen);
                                inmHelems.AddRange(hulps);
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            foreach (var i in inmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");

                            sb.Append(sb2);

                            sb.Append($"{ts}IH[{_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            if (prio.MeldingenData.AntiJutterVoorAlleInmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tprioin}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            }
                            var ifirst = true;
                            if (prio.CheckPeriode && prio.GerelateerdePerioden.Any(x => !string.IsNullOrEmpty(x.Periode)))
                            {
                                if (prio.GerelateerdePerioden.Count > 1) sb.Append("(");
                                var pfirst = true;
                                foreach (var p in prio.GerelateerdePerioden)
                                {
                                    if (!pfirst) sb.Append(" || "); pfirst = false;
                                    sb.Append($"IH[{_hpf}{_hperiod}{p.Periode}]");
                                }
                                if (prio.GerelateerdePerioden.Count > 1) sb.Append(")");
                                sb.Append(" && ");
                                
                                sb.Append("(");
                            }
                            foreach (var i in inmHelems)
                            {
                                if (!ifirst) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                ifirst = false;
                            }

                            if (prio.CheckPeriode && prio.GerelateerdePerioden.Any(x => !string.IsNullOrEmpty(x.Periode)))
                            {
                                sb.Append(")");
                            }
                            sb.AppendLine(";");
                        }

                        if (prio.MeldingenData.Uitmeldingen.Any())
                        {
                            var uitmHelems = new List<string>();
                            if (!first) sb.AppendLine(); 
                            first = false;
                            sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{prio.FaseCyclus} type {prio.Type.GetDescription()} */");

                            var sb2 = new StringBuilder();
                            foreach (var uitm in prio.MeldingenData.Uitmeldingen)
                            {
                                var hulps = GetMeldingCode(c, prio, uitm, sb2, vtgType, fcNmr, ts, prio.MeldingenData.AntiJutterVoorAlleUitmeldingen);
                                uitmHelems.AddRange(hulps);
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            foreach (var i in uitmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");
                            sb.Append(sb2);

                            sb.Append($"{ts}IH[{_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            if (prio.MeldingenData.AntiJutterVoorAlleUitmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tpriouit}{CCOLCodeHelper.GetPriorityName(prio)}] = ");
                            }
                            first = true;
                            foreach (var i in uitmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine($";");
                        }
                    }

                    #region RIS extra code

                    var ovRis = c.PrioData.PrioIngrepen
                        .Where(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                    x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde))
                        .ToList();
                    if (ovRis.Any())
                    {
                        sb.AppendLine();
                        /* printf buffers  */
                        sb.AppendLine($"{ts}#ifndef AUTOMAAT");
                        sb.AppendLine($"{ts}{ts}/* RIS_PRIOREQUEST_AP */");
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < 5; ++i) {{");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(40, i+3, \"                                                     \");");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}xyprintf(40, 1, \"RIS_PRIOREQUEST_AP                                \");");
                        sb.AppendLine($"{ts}{ts}xyprintf(40, 2, \"sg id------------------ sequenceNumber-requestType-routeName\");");
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < RIS_PRIOREQUEST_AP_NUMBER; ++i) {{");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(40, i+3, \"%s\", RIS_PRIOREQUEST_AP[i].signalGroup);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(43, i+3, \"%s\", RIS_PRIOREQUEST_AP[i].id);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(64, i+3, \"%d\", RIS_PRIOREQUEST_AP[i].sequenceNumber);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(79, i+3, \"%d\", RIS_PRIOREQUEST_AP[i].requestType);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(91, i+3, \"%s\", RIS_PRIOREQUEST_AP[i].routeName);");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}{ts}/* RIS_PRIOREQUEST_EX_AP */");
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < 5; i++) {{");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(40, i+10, \"                                                                          \");");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine($"{ts}{ts}xyprintf(40, 8, \"RIS_PRIOREQUEST_EX_AP                                             \");");
                        sb.AppendLine($"{ts}{ts}xyprintf(40, 9, \"   id------------------ sequenceNumber-prioState-prioControlState\");");
                        sb.AppendLine($"{ts}{ts}for (i = 0; i < RIS_PRIOREQUEST_EX_AP_NUMBER; ++i) {{");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(43, i+10, \"%s\", RIS_PRIOREQUEST_EX_AP[i].id);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(64, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].sequenceNumber);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(79, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].prioState);");
                        sb.AppendLine($"{ts}{ts}{ts}xyprintf(89, i+10, \"%d\", RIS_PRIOREQUEST_EX_AP[i].prioControlState);");
                        sb.AppendLine($"{ts}{ts}}}");
                        sb.AppendLine();
                        sb.AppendLine($"{ts}#endif /* #ifndef AUTOMAAT */");
                        sb.AppendLine();
                    }

                    #endregion // RIS extra code
                    
                    #region HD meldingen

                    // Inmelding HD
                    foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                    {
                        if (int.TryParse(hd.FaseCyclus, out var ifc))
                        {
                            var sgCheck = c.PrioData.KARSignaalGroepNummersInParameters 
                                ? $"PRM[{_prmpf}{_prmkarsghd}{hd.FaseCyclus}]"
                                : ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (ifc - 200).ToString() : ifc.ToString();
                            var inmHelems = new List<string>();
                            if (!first) sb.AppendLine(); 
                            first = false;
                            sb.AppendLine($"{ts}/* Inmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                if(hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                                {
                                    var actualAlsoFc = hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (hd.InmeldingOokDoorFase - 200).ToString() : hd.InmeldingOokDoorFase.ToString();
                                    sb.AppendLine($"{ts}" +
                                        $"IH[{_hpf}{_hhdin}{hd.FaseCyclus}kar] = " +
                                        $"RT[{_tpf}{_thdin}{hd.FaseCyclus}kar] = " +
                                        $"!T[{_tpf}{_thdin}{hd.FaseCyclus}kar] && " +
                                        $"SCH[{_schpf}{_schhdin}{hd.FaseCyclus}kar] && " +
                                        $"((DSIMelding_HD_V1({sgCheck}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}]) || " +
                                        $"DSIMelding_HD_V1({actualAlsoFc}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}])));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}kar] = RT[{_tpf}{_thdin}{hd.FaseCyclus}kar] = !T[{_tpf}{_thdin}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schhdin}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({sgCheck}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}]));");
                                }
                                inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}kar");
                            }
                            
                            var risFc = c.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == hd.FaseCyclus);
                            if (risFc != null && hd.RIS)
                            {
                                sb.Append($"{ts}" +
                                              $"IH[{_hpf}{_hhdin}{hd.FaseCyclus}ris] = " +
                                              $"SCH[{_schpf}{_schhdin}{hd.FaseCyclus}ris] && (");

                                first = true;
                                if (risFc != null)
                                {
                                    sb.AppendLine();
                                    foreach (var lane in risFc.LaneData)
                                    {
                                        if (!first) sb.AppendLine(" ||");
                                        var itf = c.RISData.HasMultipleSystemITF
                                            ? c.RISData.MultiSystemITF.FindIndex(x => x.SystemITF == lane.SystemITF) : -1;
                                        sb.Append($"{ts}{ts}{ts}ris_inmelding_selectief(" +
                                                  $"NG, " +
                                                  $"PRM[{_prmpf}{_prmrisapproachid}{risFc.FaseCyclus}hd], " +
                                                  $"SYSTEM_ITF{(itf >= 0 ? (itf + 1).ToString() : "")}, " +
                                                  $"PRM[{_prmpf}{_prmrislaneid}{risFc.FaseCyclus}hd_{lane.RijstrookIndex}], " +
                                                  $"PRM[{_prmpf}{_prmrisstationtype}{risFc.FaseCyclus}hd], " +
                                                  $"PRM[{_prmpf}{_prmrisstart}{hd.FaseCyclus}hd], " +
                                                  $"PRM[{_prmpf}{_prmrisend}{hd.FaseCyclus}hd], " +
                                                  $"PRM[{_prmpf}{_prmrisrole}{risFc.FaseCyclus}hd], " +
                                                  $"PRM[{_prmpf}{_prmrissubrole}{risFc.FaseCyclus}hd], " +
                                                  $"{(hd.RisEta.HasValue ? $"PRM[{_prmpf}{_prmriseta}{risFc.FaseCyclus}hd]" : "NG")}, " +
                                                  $"hdFC{hd.FaseCyclus})");
                                        first = false;
                                    }
                                }
                                sb.AppendLine(");");
                                inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}ris");
                            }

                            if (hd.Opticom)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}opt] = !T[{_tpf}{_thdin}{hd.FaseCyclus}opt] && SCH[{_schpf}{_schhdinuit}{hd.FaseCyclus}opt] && !C[{_ctpf}{_cvchd}{hd.FaseCyclus}] && DB[{_dpf}{hd.OpticomRelatedInput}];");
                                sb.AppendLine($"{ts}RT[{_tpf}{_thdin}{hd.FaseCyclus}opt] = G[{_fcpf}{hd.FaseCyclus}] && C[{_ctpf}{_cvchd}{hd.FaseCyclus}];");
                                inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}opt");
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}] = ");
                            first = true;
                            foreach (var i in inmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                        }
                    }

                    // Uitmelding HD
                    foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                    {
                        if (int.TryParse(hd.FaseCyclus, out var ifc))
                        {
                            var actualIfc = ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (ifc - 200).ToString() : ifc.ToString();
                            var uitmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Uitmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                if (hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                                {
                                    var actualAlsoFc = hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (hd.InmeldingOokDoorFase - 200).ToString() : hd.InmeldingOokDoorFase.ToString();
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}kar] = RT[{_tpf}{_thduit}{hd.FaseCyclus}kar] = !T[{_tpf}{_thduit}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schhduit}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({actualIfc}, CIF_DSUIT, FALSE) || DSIMelding_HD_V1({actualAlsoFc}, CIF_DSUIT, FALSE));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}kar] = RT[{_tpf}{_thduit}{hd.FaseCyclus}kar] = !T[{_tpf}{_thduit}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schhduit}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({actualIfc}, CIF_DSUIT, FALSE));");
                                }
                                uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}kar");
                            }

                            if (hd.RIS)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}ris] = SCH[{_schpf}{_schhduit}{hd.FaseCyclus}ris] && (ris_uitmelding_selectief(hdFC{hd.FaseCyclus}));");
                                uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}ris");
                            }

                            if (hd.Opticom)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}opt] = SCH[{_schpf}{_schhdinuit}{hd.FaseCyclus}opt] && !TDH[{_dpf}{hd.OpticomRelatedInput}] && TDH_old[{_dpf}{hd.OpticomRelatedInput}];");
                                uitmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}opt");
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = ");
                            first = true;
                            foreach (var i in uitmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                        }
                    }
                    
                    #endregion // HD meldingen

                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
                    {
                        sb.AppendLine($"{ts}/* Bepalen of een HD ingreep actief is */");
                        sb.Append($"{ts}isHD = ");
                        first = true;
                        foreach (var hd in c.PrioData.HDIngrepen)
                        {
                            if (!first)
                            {
                                sb.Append(" || ");
                            }
                            sb.Append($"C[{_ctpf}{_cvchd}{hd.FaseCyclus}]");
                            first = false;
                        }
                        sb.AppendLine(";");
                        sb.AppendLine();
                        if (!c.PrioData.BlokkeerNietConflictenAlleenLangzaamVerkeer)
                        {
                            sb.AppendLine($"{ts}/* Blokkeren alle richtingen zonder HD ingreep */");
                            sb.AppendLine($"{ts}if (isHD)");
                            sb.AppendLine($"{ts}{{");
                            foreach (var fc in c.Fasen)
                            {
                                if (c.PrioData.HDIngrepen.All(x => x.FaseCyclus != fc.Naam))
                                {
                                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6;");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}{ts}if (!C[{_ctpf}{_cvchd}{fc.Naam}]) {{ RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6; }}");
                                }
                            }
                        }
                        else
                        {
                            sb.AppendLine($"{ts}/* Blokkeren alle langzaam verkeer (tevens niet-conflicten) */");
                            if(c.Fasen.Any(x => x.WachttijdVoorspeller))
                            {
                                sb.AppendLine($"{ts}/* Blokkeren uitstellen indien een wachttijdvoorspeller onder het minimum is */");
                                foreach(var fc in c.Fasen.Where(x => x.WachttijdVoorspeller))
                                {
                                    sb.AppendLine($"{ts}isWTV |= (MM[{_mpf}{_mwtvm}{fc.Naam}] && MM[{_mpf}{_mwtvm}{fc.Naam}] <= PRM[{_prmpf}{_prmwtvnhaltmin}]);");
                                }
                                sb.AppendLine($"{ts}if (isHD && !isWTV)");
                            }
                            else
                            {
                                sb.AppendLine($"{ts}if (isHD)");
                            }
                            sb.AppendLine($"{ts}{{");
                            foreach (var fc in c.Fasen.Where(x => x.Type == FaseTypeEnum.Fiets || x.Type == FaseTypeEnum.Voetganger))
                            {
                                if (c.PrioData.HDIngrepen.All(x => x.FaseCyclus != fc.Naam))
                                {
                                    sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6;");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}{ts}if (!C[{_ctpf}{_cvchd}{fc.Naam}]) {{ RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6; }}");
                                }
                            }
                        }
                        sb.AppendLine($"{ts}}}");
                    }

                    if (c.InterSignaalGroep.Nalopen.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Niet afkappen naloop richtingen wanneer een naloop tijd nog loopt */");
                        foreach (var fc in c.Fasen)
                        {
                            var nl = c.InterSignaalGroep.Nalopen.FirstOrDefault(x => x.FaseNaar == fc.Naam);
                            if (nl != null)
                            {
                                sb.Append($"{ts}if (");
                                first = true;
                                foreach (var nlt in nl.Tijden)
                                {
                                    if (!first) sb.Append(" || ");
                                    first = false;
                                    var _tnl = nlt.Type switch
                                    {
                                        NaloopTijdTypeEnum.StartGroen => _tnlsg,
                                        NaloopTijdTypeEnum.StartGroenDetectie => _tnlsgd,
                                        NaloopTijdTypeEnum.VastGroen => _tnlfg,
                                        NaloopTijdTypeEnum.VastGroenDetectie => _tnlfgd,
                                        NaloopTijdTypeEnum.EindeGroen => _tnleg,
                                        NaloopTijdTypeEnum.EindeGroenDetectie => _tnlegd,
                                        NaloopTijdTypeEnum.EindeVerlengGroen => _tnlcv,
                                        NaloopTijdTypeEnum.EindeVerlengGroenDetectie => _tnlcvd,
                                        _ => throw new ArgumentOutOfRangeException()
                                    };
                                    sb.Append($"RT[{_tpf}{_tnl}{nl.FaseVan}{nl.FaseNaar}] || T[{_tpf}{_tnl}{nl.FaseVan}{nl.FaseNaar}]");
                                }
                                sb.AppendLine(")");
                                sb.AppendLine($"{ts}{{");
                                if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
                                {
                                    sb.AppendLine($"{ts}{ts}Z[{_fcpf}{fc.Naam}] &= ~BIT6;");
                                }
                                sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~BIT6;");
                                sb.AppendLine($"{ts}{ts}FM[{_fcpf}{fc.Naam}] &= ~PRIO_FM_BIT;");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCTegenhoudenConflicten:
                    var naloopWithPrioConflicts = new List<(NaloopModel naloop, List<ConflictModel> conflicts)>();
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen || x.Type == NaloopTypeEnum.CyclischVerlengGroen))
                    {
                        // search conflicts of naloop fc (search nl 'to' and conflict 'from')
                        var conflicts = c.InterSignaalGroep.Conflicten.Where(x => x.FaseVan == nl.FaseNaar);
                        if (!conflicts.Any()) continue;
                        // find out if conflict has prio (search conflict 'to')
                        var prioConflicts = new List<ConflictModel>();
                        foreach (var conflict in conflicts)
                        {
                            var prio = c.PrioData.PrioIngrepen.Any(x => x.FaseCyclus == conflict.FaseNaar);
                            if (prio) prioConflicts.Add(conflict);
                        }
                        if (prioConflicts.Any()) naloopWithPrioConflicts.Add((nl, prioConflicts));
                    }

                    if (!naloopWithPrioConflicts.Any()) return "";

                    sb.AppendLine($"{ts}/* Tegenhouden voedende richting, bij een conflicterende prio-ingreep van de nalooprichting */");
                    sb.AppendLine($"{ts}/* Afzetten RR */");
                    foreach (var nl in naloopWithPrioConflicts)
                    {
                        sb.Append($"{ts}if (");
                        first = true;
                        foreach (var conflict in nl.conflicts)
                        {
                            if (!first)
                            {
                                sb.AppendLine(" &&");
                                sb.Append($"{ts}    ");
                            }
                            sb.Append($"(G[{_fcpf}{conflict:naar}] || !(YV[{_fcpf}{conflict:naar}] & PRIO_YV_BIT))");
                            first = false;
                        }
                        sb.AppendLine($") RR[{_fcpf}{nl.naloop:van}] &= ~BIT10;");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Opzetten RR */");
                    foreach (var nl in naloopWithPrioConflicts)
                    {
                        sb.Append($"{ts}if (");
                        first = true;
                        foreach (var conflict in nl.conflicts)
                        {
                            if (!first)
                            {
                                sb.AppendLine(" ||");
                                sb.Append($"{ts}    ");
                            }
                            sb.Append($"((Z[{_fcpf}{nl.naloop:van}] & PRIO_Z_BIT) && (YV[{_fcpf}{conflict:naar}] & PRIO_YV_BIT) && !G[{_fcpf}{conflict:naar}])");
                            first = false;
                        }
                        sb.AppendLine($") RR[{_fcpf}{nl.naloop:van}] |= BIT10;");
                    }
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _tnlcv = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcv");
            _tnlcvd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlcvd");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _mwtvm = CCOLGeneratorSettingsProvider.Default.GetElementName("mwtvm");
            _prmwtvnhaltmin = CCOLGeneratorSettingsProvider.Default.GetElementName("prmwtvnhaltmin");
            _prmrislaneid = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrislaneid");
            _prmrisapproachid = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrisapproachid");
            _hperiod = CCOLGeneratorSettingsProvider.Default.GetElementName("hperiod");

            return base.SetSettings(settings);
        }
    }
}