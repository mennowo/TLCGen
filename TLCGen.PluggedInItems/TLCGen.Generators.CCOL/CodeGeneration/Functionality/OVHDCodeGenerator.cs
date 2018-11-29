using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class OVHDCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields
        private List<DetectorModel> _MyDetectors;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _cvc;
        private CCOLGeneratorCodeStringSettingModel _cvchd;
        private CCOLGeneratorCodeStringSettingModel _tgb;
        private CCOLGeneratorCodeStringSettingModel _tgbhd;
        private CCOLGeneratorCodeStringSettingModel _prmrto;
        private CCOLGeneratorCodeStringSettingModel _prmrtbg;
        private CCOLGeneratorCodeStringSettingModel _prmrtg;
        private CCOLGeneratorCodeStringSettingModel _prmrtohd;
        private CCOLGeneratorCodeStringSettingModel _prmrtbghd;
        private CCOLGeneratorCodeStringSettingModel _prmrtghd;
        private CCOLGeneratorCodeStringSettingModel _trt;
        private CCOLGeneratorCodeStringSettingModel _trthd;
        private CCOLGeneratorCodeStringSettingModel _tblk;
        private CCOLGeneratorCodeStringSettingModel _tbtovg;
        private CCOLGeneratorCodeStringSettingModel _usovinm;
        private CCOLGeneratorCodeStringSettingModel _ushdinm;
        private CCOLGeneratorCodeStringSettingModel _hov;
        private CCOLGeneratorCodeStringSettingModel _hhd;
        private CCOLGeneratorCodeStringSettingModel _hovin;
        private CCOLGeneratorCodeStringSettingModel _hhdin;
        private CCOLGeneratorCodeStringSettingModel _hovuit;
        private CCOLGeneratorCodeStringSettingModel _hhduit;
        private CCOLGeneratorCodeStringSettingModel _prmomx;
        private CCOLGeneratorCodeStringSettingModel _prmprio;
        private CCOLGeneratorCodeStringSettingModel _prmpriohd;
        private CCOLGeneratorCodeStringSettingModel _prmallelijnen;
        private CCOLGeneratorCodeStringSettingModel _prmlijn;
        private CCOLGeneratorCodeStringSettingModel _prmalleritcat;
        private CCOLGeneratorCodeStringSettingModel _prmritcat;
        private CCOLGeneratorCodeStringSettingModel _prmmwta;
        private CCOLGeneratorCodeStringSettingModel _prmmwtfts;
        private CCOLGeneratorCodeStringSettingModel _prmmwtvtg;
        private CCOLGeneratorCodeStringSettingModel _prmovstp;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsivert;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsilyn;
        private CCOLGeneratorCodeStringSettingModel _prmtestdsicat;
        private CCOLGeneratorCodeStringSettingModel _schupinagb;
        private CCOLGeneratorCodeStringSettingModel _schupinagbhd;
        private CCOLGeneratorCodeStringSettingModel _schvi;
        private CCOLGeneratorCodeStringSettingModel _prmpmgt;
        private CCOLGeneratorCodeStringSettingModel _prmognt;
        private CCOLGeneratorCodeStringSettingModel _prmnofm;
        private CCOLGeneratorCodeStringSettingModel _prmmgcov;
        private CCOLGeneratorCodeStringSettingModel _prmpmgcov;
        private CCOLGeneratorCodeStringSettingModel _prmohpmg;
        private CCOLGeneratorCodeStringSettingModel _schcheckdstype;
        private CCOLGeneratorCodeStringSettingModel _uskarog;
        private CCOLGeneratorCodeStringSettingModel _usmaxwt;
        private CCOLGeneratorCodeStringSettingModel _uskarmelding;
        private CCOLGeneratorCodeStringSettingModel _tkarog;
        private CCOLGeneratorCodeStringSettingModel _tkarmelding;

        private CCOLGeneratorCodeStringSettingModel _tovin;
        private CCOLGeneratorCodeStringSettingModel _tovuit;
        private CCOLGeneratorCodeStringSettingModel _schovin;
        private CCOLGeneratorCodeStringSettingModel _schovuit;
        private CCOLGeneratorCodeStringSettingModel _schgeenwissel;
        private CCOLGeneratorCodeStringSettingModel _hwissel;

        private CCOLGeneratorCodeStringSettingModel _thdin;
        private CCOLGeneratorCodeStringSettingModel _thduit;
        private CCOLGeneratorCodeStringSettingModel _schhdin;
        private CCOLGeneratorCodeStringSettingModel _schhduit;
        private CCOLGeneratorCodeStringSettingModel _schhdinuit;

        private CCOLGeneratorCodeStringSettingModel _schchecksirene;
        private CCOLGeneratorCodeStringSettingModel _schwisselpol;
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

        #endregion // Fields

        #region Properties
        #endregion // Properties

        private string GetMeldingShortcode(OVIngreepInUitMeldingModel melding)
        {
            switch (melding.Type)
            {
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    return "det";
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    return "kar";
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    return "sd";
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    return "vecio";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetDetectorTypeSCHString(OVIngreepInUitMeldingVoorwaardeInputTypeEnum type)
        {
            switch (type)
            {
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    return "SD";
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    return "D";
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    return "DB";
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    return "SDB";
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    return "ED";
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    return "ETDH";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<CCOLElement> GetMeldingElements(OVIngreepModel ov, OVIngreepInUitMeldingModel melding, bool addHov)
        {
            var elements = new List<CCOLElement>();
            string hov;
            string schov;
            string tov;

            // type melding
            switch (melding.InUit)
            {
                case OVIngreepInUitMeldingTypeEnum.Inmelding:
                    hov = _hovin.ToString();
                    schov = _schovin.ToString();
                    tov = _tovin.ToString();
                    break;
                case OVIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hovuit.ToString();
                    schov = _schovuit.ToString();
                    tov = _tovuit.ToString();
                    break;
            }

            var he = $"{hov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            var ti = $"{tov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            var sw = $"{schov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            if (melding.Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                he = he + melding.RelatedInput1;
                ti = ti + melding.RelatedInput1;
                sw = sw + melding.RelatedInput1 + GetDetectorTypeSCHString(melding.RelatedInput1Type);
                if (melding.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
                {
                    he = he + melding.RelatedInput2;
                    ti = ti + melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }

            if (addHov)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(he,
                    melding.InUit == OVIngreepInUitMeldingTypeEnum.Inmelding ? _hovin : _hovuit, ov.FaseCyclus, melding.Type.GetDescription()));
            }
            if (melding.AntiJutterTijdToepassen)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(ti, melding.AntiJutterTijd, CCOLElementTimeTypeEnum.TE_type,
                    melding.InUit == OVIngreepInUitMeldingTypeEnum.Inmelding ? _tovin : _tovuit, ov.FaseCyclus, melding.Type.GetDescription()));
            }
            elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(sw, 1, CCOLElementTimeTypeEnum.SCH_type,
                melding.InUit == OVIngreepInUitMeldingTypeEnum.Inmelding ? _schovin : _schovuit, ov.FaseCyclus, melding.Type.GetDescription()));

            if(melding.OpvangStoring && melding.MeldingBijstoring != null)
            {
                var elems = GetMeldingElements(ov, melding.MeldingBijstoring, false);
                foreach(var e in elems)
                {
                    if(_myElements.All(x => x.Naam != e.Naam))
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
            _MyDetectors = new List<DetectorModel>();
            _myBitmapOutputs = new List<CCOLIOElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            if (c.OVData.OVIngrepen.Any() || c.OVData.HDIngrepen.Any())
            {
                /* Variables independent of signal groups */
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwta}", c.OVData.MaxWachttijdAuto, CCOLElementTimeTypeEnum.TS_type, _prmmwta));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtfts}", c.OVData.MaxWachttijdFiets, CCOLElementTimeTypeEnum.TS_type, _prmmwtfts));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtvtg}", c.OVData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type, _prmmwtvtg));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usmaxwt}", _usmaxwt));
                _myBitmapOutputs.Add(new CCOLIOElement(c.OVData.MaximaleWachttijdOverschredenBitmapData, $"{_uspf}{_usmaxwt}"));

                if (c.HasKAR())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarmelding}", _uskarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarog}", _uskarog));
                    _myBitmapOutputs.Add(new CCOLIOElement(c.OVData.KARMeldingBitmapData, $"{_uspf}{_uskarmelding}"));
                    _myBitmapOutputs.Add(new CCOLIOElement(c.OVData.KAROnderGedragBitmapData, $"{_uspf}{_uskarog}"));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarmelding}", 15, CCOLElementTimeTypeEnum.TE_type, _tkarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarog}", 1440, CCOLElementTimeTypeEnum.TM_type, _tkarog));
                }
            }
            if (c.OVData.OVIngrepen.Any())
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

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcheckdstype}", c.OVData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcheckdstype));

                /* Variables for conflicting signal groups */
                foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
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

            /* Variables for OV */
            foreach (var ov in c.OVData.OVIngrepen)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(ov.OVInmeldingBitmapData, $"{_uspf}{_usovinm}{ov.FaseCyclus}"));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{ov.FaseCyclus}", _usovinm, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hov}{ov.FaseCyclus}", _hov, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovin}{ov.FaseCyclus}", _hovin, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovuit}{ov.FaseCyclus}", _hovuit, ov.FaseCyclus));

                foreach(var melding in ov.MeldingenData.Inmeldingen.Concat(ov.MeldingenData.Uitmeldingen))
                {
                    _myElements.AddRange(GetMeldingElements(ov, melding, true));
                }

                if (ov.MeldingenData.AntiJutterVoorAlleInmeldingen && ov.MeldingenData.Inmeldingen.Any())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovin}{ov.FaseCyclus}", ov.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tovin, ov.FaseCyclus));
                }

                if (ov.MeldingenData.AntiJutterVoorAlleUitmeldingen && ov.MeldingenData.Uitmeldingen.Any())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovuit}{ov.FaseCyclus}", ov.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tovuit, ov.FaseCyclus));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{ov.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _tbtovg, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_trt}{ov.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _trt, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cvc}{ov.FaseCyclus}", 999, CCOLElementTimeTypeEnum.CT_type, _cvc, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tgb}{ov.FaseCyclus}", ov.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tgb, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrto}{ov.FaseCyclus}", ov.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrto, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtbg}{ov.FaseCyclus}", ov.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtbg, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmrtg}{ov.FaseCyclus}", ov.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmrtg, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmomx}{ov.FaseCyclus}", ov.OnderMaximum, CCOLElementTimeTypeEnum.TE_type, _prmomx, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tblk}{ov.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _tblk, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schupinagb}{ov.FaseCyclus}", 0, CCOLElementTimeTypeEnum.SCH_type, _schupinagb, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmovstp}{ov.FaseCyclus}", 0, CCOLElementTimeTypeEnum.None, _prmovstp, ov.FaseCyclus));
                if ((ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ||
                     ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchUit) &&
                     !string.IsNullOrWhiteSpace(ov.Koplus) && ov.Koplus != "NG")
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schvi}{ov.FaseCyclus}", ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schvi, ov.FaseCyclus));
                }
                var opties = 0;
                if (ov.AfkappenConflicten || ov.AfkappenConflictenOV) opties += 100;
                if (ov.AfkappenConflictenOV) opties += 300;
                if (ov.TussendoorRealiseren) opties += 3;
                if (ov.VasthoudenGroen) opties += 20;
                var sopties = opties.ToString().Replace("0", "");
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprio}{ov.FaseCyclus}", int.Parse(sopties), CCOLElementTimeTypeEnum.None, _prmprio, ov.FaseCyclus));

                if (ov.CheckLijnNummer)
                {
                    // Note!!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmallelijnen}{ov.FaseCyclus}", ov.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None, _prmallelijnen, ov.FaseCyclus));
                    var n = 1;
                    foreach (var l in ov.LijnNummers)
                    {
                        if (!int.TryParse(l.Nummer, out var num)) continue;
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmlijn}{ov.FaseCyclus}_{n:00}", num, CCOLElementTimeTypeEnum.None, _prmlijn, ov.FaseCyclus));
                        ++n;
                    }
                }

                if (ov.CheckRitCategorie)
                {
                    // Note!!! "alleritcat" must alway be DIRECTLY above the cat prms, cause of the way these prms are used in code
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmalleritcat}{ov.FaseCyclus}", ov.AlleRitCategorien == true ? 1 : 0, CCOLElementTimeTypeEnum.None, _prmalleritcat, ov.FaseCyclus));
                    var n = 1;
                    foreach (var l in ov.RitCategorien)
                    {
                        if (!int.TryParse(l.Nummer, out var num)) continue;
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmritcat}{ov.FaseCyclus}_{n:00}", num, CCOLElementTimeTypeEnum.None, _prmritcat, ov.FaseCyclus));
                        ++n;
                    }
                }


                // Help elements to store wissel condition
                if (ov.HasOVIngreepWissel())
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hwissel}{ov.FaseCyclus}", _hwissel, ov.FaseCyclus));
                }

                if (ov.HasOVIngreepKAR())
                {
                    _MyDetectors.Add(ov.DummyKARInmelding);
                    _MyDetectors.Add(ov.DummyKARUitmelding);
                }
            }

            /* Variables for HD */
            foreach (var hd in c.OVData.HDIngrepen)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(hd.HDInmeldingBitmapData, $"{_uspf}{_ushdinm}{hd.FaseCyclus}"));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_ushdinm}{hd.FaseCyclus}", _ushdinm, hd.FaseCyclus));
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

                // For signal groups that have HD but not OV
                if (c.OVData.OVIngrepen.All(x => x.FaseCyclus != hd.FaseCyclus))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tbtovg}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _tbtovg, hd.FaseCyclus));
                }

                if (hd.KAR)
                {
                    _MyDetectors.Add(hd.DummyKARInmelding);
                    _MyDetectors.Add(hd.DummyKARUitmelding);

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}kar", _hhdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}kar", hd.KARInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}kar", _hhduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thduit}{hd.FaseCyclus}kar", hd.KARUitmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schchecksirene}{hd.FaseCyclus}", hd.Sirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schchecksirene, hd.FaseCyclus));
                }
                if (hd.Opticom)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}opt", _hhdin, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}opt", hd.OpticomInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdinuit}{hd.FaseCyclus}opt", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdinuit, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}opt", _hhduit, hd.FaseCyclus, "Opticom"));
                }
            }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<DetectorModel> GetDetectors()
        {
            return _MyDetectors;
        }

        public override bool HasDetectors()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _myElements.Where(x => x.Type == type);
        }

        public override bool HasCCOLBitmapOutputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _myBitmapOutputs;
        }

        public override bool HasCCOLBitmapInputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _myBitmapInputs;
        }

        public override bool HasFunctionLocalVariables()
        {
            return true;
        }

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.OvCPostAfhandelingOV:
                    var result = new List<Tuple<string, string, string>>();
                    if (c.OVData.BlokkeerNietConflictenBijHDIngreep)
                    {
                        result.Add(new Tuple<string, string, string>("bool", "isHD", "FALSE"));
                        if (c.Fasen.Any(x => x.WachttijdVoorspeller)) result.Add(new Tuple<string, string, string>("bool", "isWTV", "FALSE"));

                    }
                    return result;                        
                    
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 40;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 40;
                case CCOLCodeTypeEnum.OvCInUitMelden:
                    return 10;
                case CCOLCodeTypeEnum.OvCPostAfhandelingOV:
                    return 10;
                default:
                    return 0;
            }
        }

        private string GetMeldingDetectieCode(OVIngreepInUitMeldingModel melding)
        {
            var sb = new StringBuilder();
            switch (melding.RelatedInput1Type)
            {
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    sb.Append($"SD[{_dpf}{melding.RelatedInput1}]");
                    break;
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    sb.Append($"D[{_dpf}{melding.RelatedInput1}]");
                    break;
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    sb.Append($"DB[{_dpf}{melding.RelatedInput1}]");
                    break;
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    sb.Append($"!DB_old[{_dpf}{melding.RelatedInput1}] && DB[{_dpf}{melding.RelatedInput1}]");
                    break;
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    sb.Append($"ED[{_dpf}{melding.RelatedInput1}]");
                    break;
                case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    sb.Append($"TDH_old[{_dpf}{melding.RelatedInput1}] && !TDH[{_dpf}{melding.RelatedInput1}]");
                    break;
            }
            if (melding.TweedeInput)
            {
                switch (melding.RelatedInput2Type)
                {
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                        sb.Append($" && SD[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                        sb.Append($" && D[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                        sb.Append($" && DB[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                        sb.Append($" && !DB_old[{_dpf}{melding.RelatedInput2}] && DB[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                        sb.Append($" && ED[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case OVIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                        sb.Append($" && TDH_old[{_dpf}{melding.RelatedInput2}] && !TDH[{_dpf}{melding.RelatedInput2}]");
                        break;
                }
            }
            return sb.ToString();
        }

        private List<string> GetMeldingCode(ControllerModel c, OVIngreepModel ov, OVIngreepInUitMeldingModel melding, StringBuilder sb, string vtgType, int fcNmr, string ts, bool antiJutVoorAlles, bool opvang = false, string otherHov = null)
        {
            var inmHelems = new List<string>();
            string hov;
            string schov;
            string tov;

            // type melding
            switch (melding.InUit)
            {
                case OVIngreepInUitMeldingTypeEnum.Inmelding:
                    hov = _hovin.ToString();
                    schov = _schovin.ToString();
                    tov = _tovin.ToString();
                    break;
                case OVIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hovuit.ToString();
                    schov = _schovuit.ToString();
                    tov = _tovuit.ToString();
                    break;
            }

            var he = $"{_hpf}{hov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            var ti = $"{_tpf}{tov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            var sw = $"{_schpf}{schov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}";
            if (melding.Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                he = he + melding.RelatedInput1;
                ti = ti + melding.RelatedInput1;
                sw = sw + melding.RelatedInput1 + GetDetectorTypeSCHString(melding.RelatedInput1Type);
                if (melding.Type == OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
                {
                    he = he + melding.RelatedInput2;
                    ti = ti + melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }
            if (otherHov != null) he = otherHov;
            else inmHelems.Add(he);

            var tts = ts;
            
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
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                        if (melding.TweedeInput && !string.IsNullOrWhiteSpace(melding.RelatedInput2))
                        {
                            sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING)");
                        }
                    }
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                    {
                        sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                    }
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    var sd = c.SelectieveDetectoren.FirstOrDefault(x => x.Naam == melding.RelatedInput1);
                    if (sd != null && !sd.Dummy)
                    {
                        if (!string.IsNullOrWhiteSpace(melding.RelatedInput1))
                        {
                            sb.Append($" && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING)");
                        }
                    }
                    break;
            }
            sb.AppendLine($")");
            sb.AppendLine($"{ts}{{");

            sb.Append($"{tts}IH[{he}] = ");
            if (melding.AntiJutterTijdToepassen)
            {
                sb.Append($"RT[{ti}] = ");
            }
            if (antiJutVoorAlles)
            {
                sb.Append($"!T[{_tpf}{tov}{ov.FaseCyclus}] && ");
            }
            if (melding.AntiJutterTijdToepassen)
            { 
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding)
            {
                sb.Append($"!C[{_ctpf}{_cvc}{ov.FaseCyclus}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hwissel}{ov.FaseCyclus}] && ");
            }

            var extra = "";
            if (ov.CheckLijnNummer && ov.LijnNummers.Any())
            {
                extra += "DSIMeldingOV_LijnNummer_V1(" +
                         $"{_prmpf + _prmallelijnen + ov.FaseCyclus}, " +
                         $"{ov.LijnNummers.Count})";
            }
            if (ov.CheckRitCategorie && ov.RitCategorien.Any())
            {
                if (extra != "") extra += " && ";
                extra += "DSIMeldingOV_RitCategorie_V1(" +
                         $"{_prmpf + _prmalleritcat + ov.FaseCyclus}, " +
                         $"{ov.RitCategorien.Count})";
            }
            if (extra == "") extra = "TRUE";

            switch (melding.Type)
            {
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    sb.AppendLine($"DSIMeldingOV_V1(0, " +
                                                    $"{vtgType}, " +
                                                    "TRUE, " + 
                                                    $"{(fcNmr == -1 ? "NG" : fcNmr.ToString())}," +
                                                    "TRUE, " +
                                                    (melding.InUit == OVIngreepInUitMeldingTypeEnum.Inmelding ? $"CIF_DSIN, " : $"CIF_DSUIT, ") +
                                                    $"{extra});");
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    sb.AppendLine($"DSIMeldingOV_V1({(_dpf + melding.RelatedInput1).ToUpper()}, " +
                                                    $"{vtgType}, " +
                                                    "FALSE, " + 
                                                    "NG, " +
                                                    $"SCH[{_schpf}{_schcheckdstype}], " +
                                                    $"{(melding.InUit == OVIngreepInUitMeldingTypeEnum.Inmelding ? "CIF_DSIN" : "CIF_DSUIT")}, " +
                                                    $"{extra});");
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    sb.AppendLine(GetMeldingDetectieCode(melding) + ";");
                    break;
                case OVIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    sb.AppendLine($" && SD[{_dpf}{melding.RelatedInput1}];");
                    break;
            }
            sb.AppendLine($"{ts}}}");
            if (melding.OpvangStoring && melding.MeldingBijstoring != null && melding.Type != OVIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                inmHelems.AddRange(GetMeldingCode(c, ov, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return inmHelems;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();
            var first = false;

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (!c.OVData.OVIngrepen.Any(x => x.HasOVIngreepWissel())) return null;
                    sb.AppendLine($"{ts}/* Onthouden wissel voorwaarden per fasecyclus */");
                    foreach (var ov in c.OVData.OVIngrepen)
                    {
                        if (ov.HasOVIngreepWissel())
                        {
                            sb.Append($"{ts}IH[{_hpf}{_hwissel}{ov.FaseCyclus}] = (");
                            if (ov.MeldingenData.Wissel1)
                            {
                                if (ov.MeldingenData.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
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
                                if (ov.MeldingenData.Wissel2Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
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
                    sb.AppendLine();
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    sb.AppendLine($"{ts}/* OV/HD verklikking */");
                    sb.AppendLine($"{ts}/* ----------------- */");
                    foreach (var ov in c.OVData.OVIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovinm}{ov.FaseCyclus}] = C[{_ctpf}{_cvc}{ov.FaseCyclus}];");
                    }
                    foreach (var hd in c.OVData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_ushdinm}{hd.FaseCyclus}] = C[{_ctpf}{_cvchd}{hd.FaseCyclus}];");
                    }
                    sb.AppendLine();
                    if (c.HasKAR())
                    {
                        sb.AppendLine($"{ts}/* Verklikken melding en ondergedrag KAR */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarmelding}] = T[{_tpf}{_tkarmelding}];");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarog}] = !T[{_tpf}{_tkarog}];");
                        sb.AppendLine();
                    }
                    if (c.OVData.OVIngrepen.Any() || c.OVData.HDIngrepen.Any())
                    {
                        sb.AppendLine($"{ts}/* Verklikken overschreiding maximale wachttijd */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usmaxwt}] = iMaximumWachtTijdOverschreden;");
                        sb.AppendLine();
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.OvCInUitMelden:
                    //sb.AppendLine($"{ts}/* Afzetten hulpelementen voor in- en uitmeldingen */");
                    //foreach (var ov in c.OVData.OVIngrepen)
                    //{
                    //    sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = FALSE;");
                    //}
                    //sb.AppendLine();
                    foreach (var ov in c.OVData.OVIngrepen)
                    {
                        var vtgType = "";
                        int fcNmr;
                        if (!Int32.TryParse(ov.FaseCyclus, out fcNmr)) fcNmr = -1;
                        switch (ov.Type)
                        {
                            case OVIngreepVoertuigTypeEnum.Tram:
                                vtgType = "CIF_TRAM";
                                break;
                            case OVIngreepVoertuigTypeEnum.Bus:
                                vtgType = "CIF_BUS";
                                break;
                            case OVIngreepVoertuigTypeEnum.NG:
                                vtgType = "NG";
                                break;
                            default:
                                throw new IndexOutOfRangeException();
                        }

                        if (ov.MeldingenData.Inmeldingen.Any())
                        {
                            var inmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Inmelding {_fcpf}{ov.FaseCyclus} */");

                            var sb2 = new StringBuilder();
                            foreach (var inm in ov.MeldingenData.Inmeldingen)
                            {
                                inmHelems.AddRange(GetMeldingCode(c, ov, inm, sb2, vtgType, fcNmr, ts, ov.MeldingenData.AntiJutterVoorAlleInmeldingen));
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = ");
                            foreach (var i in inmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");
                            sb.Append(sb2.ToString());

                            sb.Append($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = ");
                            if (ov.MeldingenData.AntiJutterVoorAlleInmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tovin}{ov.FaseCyclus}] = ");
                            }
                            first = true;
                            foreach (var i in inmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }

                        if (ov.MeldingenData.Uitmeldingen.Any())
                        {
                            var uitmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{ov.FaseCyclus} */");


                            var sb2 = new StringBuilder();
                            foreach (var uitm in ov.MeldingenData.Uitmeldingen)
                            {
                                uitmHelems.AddRange(GetMeldingCode(c, ov, uitm, sb2, vtgType, fcNmr, ts, ov.MeldingenData.AntiJutterVoorAlleUitmeldingen));
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = ");
                            foreach (var i in uitmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");
                            sb.Append(sb2.ToString());
                            
                            sb.Append($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = ");
                            if (ov.MeldingenData.AntiJutterVoorAlleUitmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tovuit}{ov.FaseCyclus}] = ");
                            }
                            first = true;
                            foreach (var i in uitmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine($";");
                            sb.AppendLine();
                        }
                    }
                    
                    // Inmelding HD
                    foreach (var hd in c.OVData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                    {
                        if (int.TryParse(hd.FaseCyclus, out var ifc))
                        {
                            var inmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Inmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhdin}{hd.FaseCyclus}kar] = RT[{_tpf}{_thdin}{hd.FaseCyclus}kar] = !T[{_tpf}{_thdin}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schhdin}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({ifc}, CIF_DSIN, SCH[{_schpf}{_schchecksirene}{hd.FaseCyclus}]));");
                                inmHelems.Add($"{_hpf}{_hhdin}{hd.FaseCyclus}kar");
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
                            sb.AppendLine();
                        }
                    }

                    // Uitmelding HD
                    foreach (var hd in c.OVData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                    {
                        if (int.TryParse(hd.FaseCyclus, out var ifc))
                        {
                            var inmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Uitmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}kar] = RT[{_tpf}{_thduit}{hd.FaseCyclus}kar] = !T[{_tpf}{_thduit}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schhduit}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({ifc}, CIF_DSUIT, FALSE));");
                                inmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}kar");
                            }
                            if (hd.Opticom)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}opt] = SCH[{_schpf}{_schhdinuit}{hd.FaseCyclus}opt] && !TDH[{_dpf}{hd.OpticomRelatedInput}] && TDH_old[{_dpf}{hd.OpticomRelatedInput}];");
                                inmHelems.Add($"{_hpf}{_hhduit}{hd.FaseCyclus}opt");
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hhduit}{hd.FaseCyclus}] = ");
                            first = true;
                            foreach (var i in inmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }
                    }

                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCPostAfhandelingOV:
                    if (c.OVData.BlokkeerNietConflictenBijHDIngreep)
                    {
                        sb.AppendLine($"{ts}/* Bepalen of een HD ingreep actief is */");
                        sb.Append($"{ts}isHD = ");
                        first = true;
                        foreach (var hd in c.OVData.HDIngrepen)
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
                        if (!c.OVData.BlokkeerNietConflictenAlleenLangzaamVerkeer)
                        {
                            sb.AppendLine($"{ts}/* Blokkeren alle richtingen zonder HD ingreep */");
                            sb.AppendLine($"{ts}if (isHD)");
                            sb.AppendLine($"{ts}{{");
                            foreach (var fc in c.Fasen)
                            {
                                if (c.OVData.HDIngrepen.All(x => x.FaseCyclus != fc.Naam))
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
                                if (c.OVData.HDIngrepen.All(x => x.FaseCyclus != fc.Naam))
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
                        sb.AppendLine();
                    }

                    if (c.InterSignaalGroep.Nalopen.Any())
                    {
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
                                    var _tnl = "";
                                    switch (nlt.Type)
                                    {
                                        case NaloopTijdTypeEnum.StartGroen: _tnl = _tnlsg; break;
                                        case NaloopTijdTypeEnum.StartGroenDetectie: _tnl = _tnlsgd; break;
                                        case NaloopTijdTypeEnum.VastGroen: _tnl = _tnlfg; break;
                                        case NaloopTijdTypeEnum.VastGroenDetectie: _tnl = _tnlfgd; break;
                                        case NaloopTijdTypeEnum.EindeGroen: _tnl = _tnleg; break;
                                        case NaloopTijdTypeEnum.EindeGroenDetectie: _tnl = _tnlegd; break;
                                        case NaloopTijdTypeEnum.EindeVerlengGroen: _tnl = _tnlcv; break;
                                        case NaloopTijdTypeEnum.EindeVerlengGroenDetectie: _tnl = _tnlcvd; break;
                                        default: throw new ArgumentOutOfRangeException();
                                    }
                                    sb.Append($"RT[{_tpf}{_tnl}{nl.FaseVan}{nl.FaseNaar}] || T[{_tpf}{_tnl}{nl.FaseVan}{nl.FaseNaar}]");
                                }
                                sb.AppendLine(")");
                                sb.AppendLine($"{ts}{{");
                                if (c.OVData.BlokkeerNietConflictenBijHDIngreep)
                                {
                                    sb.AppendLine($"{ts}{ts}Z[{_fcpf}{fc.Naam}] &= ~BIT6;");
                                }
                                sb.AppendLine($"{ts}{ts}RR[{_fcpf}{fc.Naam}] &= ~BIT6;");
                                sb.AppendLine($"{ts}{ts}FM[{_fcpf}{fc.Naam}] &= ~OV_FM_BIT;");
                                sb.AppendLine($"{ts}}}");
                            }
                        }
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

            return base.SetSettings(settings);
        }
    }
}