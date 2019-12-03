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
    public class PrioCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields
        private List<DetectorModel> _MyDetectors;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _cpriovc;
        private CCOLGeneratorCodeStringSettingModel _cpriovchd;
        private CCOLGeneratorCodeStringSettingModel _tpriogb;
        private CCOLGeneratorCodeStringSettingModel _tpriogbhd;
        private CCOLGeneratorCodeStringSettingModel _prmpriorto;
        private CCOLGeneratorCodeStringSettingModel _prmpriortbg;
        private CCOLGeneratorCodeStringSettingModel _prmpriortg;
        private CCOLGeneratorCodeStringSettingModel _prmpriortohd;
        private CCOLGeneratorCodeStringSettingModel _prmpriortbghd;
        private CCOLGeneratorCodeStringSettingModel _prmpriortghd;
        private CCOLGeneratorCodeStringSettingModel _tpriort;
        private CCOLGeneratorCodeStringSettingModel _tpriorthd;
        private CCOLGeneratorCodeStringSettingModel _tprioblk;
        private CCOLGeneratorCodeStringSettingModel _tpriobtovg;
        private CCOLGeneratorCodeStringSettingModel _usprioovinm;
        private CCOLGeneratorCodeStringSettingModel _uspriohdinm;
        private CCOLGeneratorCodeStringSettingModel _hprioov;
        private CCOLGeneratorCodeStringSettingModel _hpriohd;
        private CCOLGeneratorCodeStringSettingModel _hprioovin;
        private CCOLGeneratorCodeStringSettingModel _hpriohdin;
        private CCOLGeneratorCodeStringSettingModel _hprioovuit;
        private CCOLGeneratorCodeStringSettingModel _hpriohduit;
        private CCOLGeneratorCodeStringSettingModel _prmprioomx;
        private CCOLGeneratorCodeStringSettingModel _prmprioprio;
        private CCOLGeneratorCodeStringSettingModel _prmpriopriohd;
        private CCOLGeneratorCodeStringSettingModel _prmprioallelijnen;
        private CCOLGeneratorCodeStringSettingModel _prmpriolijn;
        private CCOLGeneratorCodeStringSettingModel _prmprioritcat;
        private CCOLGeneratorCodeStringSettingModel _prmpriomwta;
        private CCOLGeneratorCodeStringSettingModel _prmpriomwtfts;
        private CCOLGeneratorCodeStringSettingModel _prmpriomwtvtg;
        private CCOLGeneratorCodeStringSettingModel _prmpriotestdsivert;
        private CCOLGeneratorCodeStringSettingModel _prmpriotestdsilyn;
        private CCOLGeneratorCodeStringSettingModel _prmpriotestdsicat;
        private CCOLGeneratorCodeStringSettingModel _schprioupinagb;
        private CCOLGeneratorCodeStringSettingModel _schprioupinagbhd;
        private CCOLGeneratorCodeStringSettingModel _schpriovi;
        private CCOLGeneratorCodeStringSettingModel _prmpriopmgt;
        private CCOLGeneratorCodeStringSettingModel _prmprioognt;
        private CCOLGeneratorCodeStringSettingModel _prmprionofm;
        private CCOLGeneratorCodeStringSettingModel _prmpriomgcov;
        private CCOLGeneratorCodeStringSettingModel _prmpriopmgcov;
        private CCOLGeneratorCodeStringSettingModel _prmprioohpmg;
        private CCOLGeneratorCodeStringSettingModel _schpriocheckdstype;
        private CCOLGeneratorCodeStringSettingModel _uspriokarog;
        private CCOLGeneratorCodeStringSettingModel _uspriomaxwt;
        private CCOLGeneratorCodeStringSettingModel _uspriokarmelding;
        private CCOLGeneratorCodeStringSettingModel _tpriokarog;
        private CCOLGeneratorCodeStringSettingModel _tpriokarmelding;

        private CCOLGeneratorCodeStringSettingModel _tprioovin;
        private CCOLGeneratorCodeStringSettingModel _tprioovuit;
        private CCOLGeneratorCodeStringSettingModel _schprioovin;
        private CCOLGeneratorCodeStringSettingModel _schprioovuit;
        private CCOLGeneratorCodeStringSettingModel _schpriogeenwissel;
        private CCOLGeneratorCodeStringSettingModel _hpriowissel;

        private CCOLGeneratorCodeStringSettingModel _tprioovminrood;
        private CCOLGeneratorCodeStringSettingModel _tpriohdin;
        private CCOLGeneratorCodeStringSettingModel _tpriohduit;
        private CCOLGeneratorCodeStringSettingModel _schpriohdin;
        private CCOLGeneratorCodeStringSettingModel _schpriohduit;
        private CCOLGeneratorCodeStringSettingModel _schpriohdinuit;

        private CCOLGeneratorCodeStringSettingModel _schpriochecksirene;
        private CCOLGeneratorCodeStringSettingModel _schpriowisselpol;

        private CCOLGeneratorCodeStringSettingModel _schpriocovuber;

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
        private string _hrisprio;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        private string GetMeldingShortcode(PrioIngreepInUitMeldingModel melding)
        {
            switch (melding.Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    return "det";
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    return "kar";
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    return "sd";
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    return "vecio";
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISInput:
                    return "ris";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetDetectorTypeSCHString(PrioIngreepInUitMeldingVoorwaardeInputTypeEnum type)
        {
            switch (type)
            {
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    return "SD";
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    return "D";
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    return "DB";
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    return "SDB";
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    return "ED";
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    return "ETDH";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<CCOLElement> GetMeldingElements(PrioIngreepModel ov, PrioIngreepInUitMeldingModel melding, bool addHov)
        {
            var elements = new List<CCOLElement>();
            string hov;
            string schov;
            string tov;

            // type melding
            switch (melding.InUit)
            {
                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                    hov = _hprioovin.ToString();
                    schov = _schprioovin.ToString();
                    tov = _tprioovin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hprioovuit.ToString();
                    schov = _schprioovuit.ToString();
                    tov = _tprioovuit.ToString();
                    break;
            }

            var he = $"{hov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            var ti = $"{tov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            var sw = $"{schov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                he = he + melding.RelatedInput1;
                ti = ti + melding.RelatedInput1;
                sw = sw + melding.RelatedInput1 + GetDetectorTypeSCHString(melding.RelatedInput1Type);
                if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
                {
                    he = he + melding.RelatedInput2;
                    ti = ti + melding.RelatedInput2;
                    sw = sw + melding.RelatedInput2 + GetDetectorTypeSCHString(melding.RelatedInput2Type);
                }
            }

            if (addHov)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(he,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _hprioovin : _hprioovuit, ov.FaseCyclus, ov.Type.GetDescription()));
            }
            if (melding.AntiJutterTijdToepassen)
            {
                elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(ti, melding.AntiJutterTijd, CCOLElementTimeTypeEnum.TE_type,
                    melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _tprioovin : _tprioovuit, ov.FaseCyclus, ov.Type.GetDescription()));
            }
            elements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement(sw, 1, CCOLElementTimeTypeEnum.SCH_type,
                melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? _schprioovin : _schprioovuit, ov.FaseCyclus, ov.Type.GetDescription()));

            if (melding.OpvangStoring && melding.MeldingBijstoring != null)
            {
                var elems = GetMeldingElements(ov, melding.MeldingBijstoring, false);
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
            _MyDetectors = new List<DetectorModel>();
            _myBitmapOutputs = new List<CCOLIOElement>();
            _myBitmapInputs = new List<CCOLIOElement>();

            if (c.PrioData.PrioIngreepType != PrioIngreepTypeEnum.GeneriekePrioriteit) return;

            if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan || c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchUit)
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriocovuber}", c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpriocovuber));
            }

            if (c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any())
            {
                /* Variables independent of signal groups */
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriomwta}", c.PrioData.MaxWachttijdAuto, CCOLElementTimeTypeEnum.TS_type, _prmpriomwta));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriomwtfts}", c.PrioData.MaxWachttijdFiets, CCOLElementTimeTypeEnum.TS_type, _prmpriomwtfts));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriomwtvtg}", c.PrioData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type, _prmpriomwtvtg));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspriomaxwt}", _uspriomaxwt));
                _myBitmapOutputs.Add(new CCOLIOElement(c.PrioData.MaximaleWachttijdOverschredenBitmapData, $"{_uspf}{_uspriomaxwt}"));

                if (c.HasKAR())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspriokarmelding}", _uspriokarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspriokarog}", _uspriokarog));
                    _myBitmapOutputs.Add(new CCOLIOElement(c.PrioData.KARMeldingBitmapData, $"{_uspf}{_uspriokarmelding}"));
                    _myBitmapOutputs.Add(new CCOLIOElement(c.PrioData.KAROnderGedragBitmapData, $"{_uspf}{_uspriokarog}"));

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriokarmelding}", 15, CCOLElementTimeTypeEnum.TE_type, _tpriokarmelding));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriokarog}", 1440, CCOLElementTimeTypeEnum.TM_type, _tpriokarog));
                }
            }
            if (c.PrioData.PrioIngrepen.Any())
            {
                /* Variables independent of signal groups */

                if (c.HasDSI())
                {
                    var prmtest1 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriotestdsivert}", 120, CCOLElementTimeTypeEnum.None, _prmpriotestdsivert);
                    var prmtest2 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriotestdsilyn}", 0, CCOLElementTimeTypeEnum.None, _prmpriotestdsilyn);
                    var prmtest3 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriotestdsicat}", 10, CCOLElementTimeTypeEnum.None, _prmpriotestdsicat);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    prmtest3.Dummy = true;
                    _myElements.Add(prmtest1);
                    _myElements.Add(prmtest2);
                    _myElements.Add(prmtest3);
                }

                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriogeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schpriogeenwissel, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriogeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schpriogeenwissel, d.Naam));
                }
                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselStandDetector))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriowisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schpriowisselpol, d.Naam));
                }
                foreach (var d in c.Ingangen.Where(x => x.Type == IngangTypeEnum.WisselContact))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriowisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schpriowisselpol, d.Naam));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriocheckdstype}", c.PrioData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpriocheckdstype));

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

                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriopmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmpriopmgt, ovfc.FaseCyclus));
                        _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmprioognt, ovfc.FaseCyclus));
                        if (fct != FaseTypeEnum.Voetganger)
                        {
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprionofm}{ovfc.FaseCyclus}", ovfc.AantalKerenNietAfkappen, CCOLElementTimeTypeEnum.TE_type, _prmprionofm, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriomgcov}{ovfc.FaseCyclus}", ovfc.MinimumGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.TE_type, _prmpriomgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriopmgcov}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.None, _prmpriopmgcov, ovfc.FaseCyclus));
                            _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioohpmg}{ovfc.FaseCyclus}", ovfc.OphoogpercentageNaAfkappen, CCOLElementTimeTypeEnum.None, _prmprioohpmg, ovfc.FaseCyclus));
                        }
                    }
                }
            }

            /* Variables for OV */
            foreach (var ov in c.PrioData.PrioIngrepen)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(ov.PrioInmeldingBitmapData, $"{_uspf}{_usprioovinm}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}"));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usprioovinm}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", _usprioovinm, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioov}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", _hprioov, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioovin}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", _hprioovin, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hprioovuit}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", _hprioovuit, ov.FaseCyclus, ov.Type.GetDescription()));

                foreach (var melding in ov.MeldingenData.Inmeldingen.Concat(ov.MeldingenData.Uitmeldingen))
                {
                    _myElements.AddRange(GetMeldingElements(ov, melding, true));
                }

                if (ov.MeldingenData.AntiJutterVoorAlleInmeldingen && ov.MeldingenData.Inmeldingen.Any())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioovin}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tprioovin, ov.FaseCyclus, ov.Type.GetDescription()));
                }

                if (ov.MeldingenData.AntiJutterVoorAlleUitmeldingen && ov.MeldingenData.Uitmeldingen.Any())
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioovuit}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.MeldingenData.AntiJutterTijdVoorAlleUitmeldingen, CCOLElementTimeTypeEnum.TE_type, _tprioovuit, ov.FaseCyclus, ov.Type.GetDescription()));
                }

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriobtovg}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.BezettijdPrioGehinderd, CCOLElementTimeTypeEnum.TE_type, _tpriobtovg, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriort}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", 0, CCOLElementTimeTypeEnum.TE_type, _tpriort, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cpriovc}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", 999, CCOLElementTimeTypeEnum.CT_type, _cpriovc, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriogb}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tpriogb, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriorto}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriorto, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriortbg}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriortbg, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriortg}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriortg, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioomx}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.OnderMaximum, CCOLElementTimeTypeEnum.TE_type, _prmprioomx, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioblk}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.BlokkeertijdNaPrioIngreep, CCOLElementTimeTypeEnum.TE_type, _tprioblk, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schprioupinagb}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", 0, CCOLElementTimeTypeEnum.SCH_type, _schprioupinagb, ov.FaseCyclus, ov.Type.GetDescription()));
                if ((ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ||
                     ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchUit) &&
                     !string.IsNullOrWhiteSpace(ov.Koplus) && ov.Koplus != "NG")
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriovi}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpriovi, ov.FaseCyclus, ov.Type.GetDescription()));
                }
                var opties = 0;
                if (ov.AfkappenConflicten || ov.AfkappenConflictenPrio) opties += 100;
                if (ov.AfkappenConflictenPrio) opties += 300;
                if (ov.TussendoorRealiseren) opties += 3;
                if (ov.VasthoudenGroen) opties += 20;
                var sopties = opties == 0 ? "0" : opties.ToString().Replace("0", "");
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioprio}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", int.Parse(sopties), CCOLElementTimeTypeEnum.None, _prmprioprio, ov.FaseCyclus, ov.Type.GetDescription()));

                if (ov.CheckLijnNummer)
                {
                    // Note!!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioallelijnen}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None, _prmprioallelijnen, ov.FaseCyclus, ov.Type.GetDescription()));
                    var n = 1;
                    foreach (var l in ov.LijnNummers)
                    {
                        if (!int.TryParse(l.Nummer, out var num)) continue;
                        _myElements.Add(
                            CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriolijn}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}_{n:00}", num, CCOLElementTimeTypeEnum.None, _prmpriolijn, n.ToString(), ov.FaseCyclus, ov.Type.GetDescription()));
                        ++n;
                    }
                    if (ov.CheckRitCategorie)
                    {
                        n = 1;
                        foreach (var l in ov.LijnNummers)
                        {
                            if (!int.TryParse(l.RitCategorie, out var ritcat)) continue;
                            _myElements.Add(
                                CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmprioritcat}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}_{n:00}", ritcat, CCOLElementTimeTypeEnum.None, _prmprioritcat, n.ToString(), ov.FaseCyclus, ov.Type.GetDescription()));
                            ++n;
                        }
                    }
                }

                // Help elements to store wissel condition
                if (ov.HasOVIngreepWissel())
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriowissel}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", _hpriowissel, ov.FaseCyclus, ov.Type.GetDescription()));
                }

                if (ov.HasOVIngreepKAR())
                {
                    _MyDetectors.Add(ov.DummyKARInmelding);
                    _MyDetectors.Add(ov.DummyKARUitmelding);
                }

                if (ov.MeldingenData.Inmeldingen.Any(x => x.AlleenIndienRood) || ov.NoodaanvraagKoplus)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tprioovminrood}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}", ov.MinimaleRoodtijd, CCOLElementTimeTypeEnum.TE_type, _tprioovminrood, ov.FaseCyclus, ov.Type.GetDescription()));
                }
            }

            /* Variables for HD */
            foreach (var hd in c.PrioData.HDIngrepen)
            {
                _myBitmapOutputs.Add(new CCOLIOElement(hd.HDInmeldingBitmapData, $"{_uspf}{_uspriohdinm}{hd.FaseCyclus}"));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uspriohdinm}{hd.FaseCyclus}", _uspriohdinm, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohd}{hd.FaseCyclus}", _hpriohd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohdin}{hd.FaseCyclus}", _hpriohdin, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohduit}{hd.FaseCyclus}", _hpriohduit, hd.FaseCyclus));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriogbhd}{hd.FaseCyclus}", hd.GroenBewaking, CCOLElementTimeTypeEnum.TE_type, _tpriogbhd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriorthd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _tpriorthd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_cpriovchd}{hd.FaseCyclus}", 999, CCOLElementTimeTypeEnum.CT_type, _cpriovchd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriopriohd}{hd.FaseCyclus}", 9005, CCOLElementTimeTypeEnum.None, _prmpriopriohd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriortohd}{hd.FaseCyclus}", hd.RijTijdOngehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriortohd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriortbghd}{hd.FaseCyclus}", hd.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriortbghd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpriortghd}{hd.FaseCyclus}", hd.RijTijdGehinderd, CCOLElementTimeTypeEnum.TE_type, _prmpriortghd, hd.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schprioupinagbhd}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.SCH_type, _schprioupinagbhd, hd.FaseCyclus));

                // For signal groups that have HD but not OV
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriobtovg}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, _tpriobtovg, hd.FaseCyclus));

                if (hd.KAR)
                {
                    _MyDetectors.Add(hd.DummyKARInmelding);
                    _MyDetectors.Add(hd.DummyKARUitmelding);

                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohdin}{hd.FaseCyclus}kar", _hpriohdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriohdin}{hd.FaseCyclus}kar", hd.KARInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _tpriohdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriohdin}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schpriohdin, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohduit}{hd.FaseCyclus}kar", _hpriohduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriohduit}{hd.FaseCyclus}kar", hd.KARUitmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _tpriohduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriohduit}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schpriohduit, hd.FaseCyclus, "KAR"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriochecksirene}{hd.FaseCyclus}", hd.Sirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schpriochecksirene, hd.FaseCyclus));
                }
                if (hd.Opticom && !string.IsNullOrWhiteSpace(hd.OpticomRelatedInput))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohdin}{hd.FaseCyclus}opt", _hpriohdin, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tpriohdin}{hd.FaseCyclus}opt", hd.OpticomInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _tpriohdin, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schpriohdinuit}{hd.FaseCyclus}opt", 1, CCOLElementTimeTypeEnum.SCH_type, _schpriohdinuit, hd.FaseCyclus, "Opticom"));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hpriohduit}{hd.FaseCyclus}opt", _hpriohduit, hd.FaseCyclus, "Opticom"));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<DetectorModel> GetDetectors() => _MyDetectors;

        public override bool HasDetectors() => true;

        public override bool HasCCOLBitmapOutputs() => true;

        public override bool HasCCOLBitmapInputs() => true;

        public override bool HasFunctionLocalVariables() => true;

        public override IEnumerable<Tuple<string, string, string>> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return base.GetFunctionLocalVariables(c, type);

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    var result = new List<Tuple<string, string, string>>();
                    result.Add(new Tuple<string, string, string>("int", "ov", "0"));
                    return result;
            
                case CCOLCodeTypeEnum.PrioCPostAfhandelingOV:
                    var result2 = new List<Tuple<string, string, string>>();
                    if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
                    {
                        result2.Add(new Tuple<string, string, string>(c.GetBoolV(), "isHD", "FALSE"));
                        if (c.Fasen.Any(x => x.WachttijdVoorspeller)) result2.Add(new Tuple<string, string, string>(c.GetBoolV(), "isWTV", "FALSE"));

                    }
                    return result2;
            
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCTop:
                    return 61;
                case CCOLCodeTypeEnum.RegCPreApplication:
                    return 41;
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 41;
                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return 31;
                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    return 11;
                case CCOLCodeTypeEnum.PrioCPostAfhandelingOV:
                    return 11;
                default:
                    return 0;
            }
        }

        private string GetMeldingDetectieCode(PrioIngreepInUitMeldingModel melding)
        {
            var sb = new StringBuilder();
            switch (melding.RelatedInput1Type)
            {
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                    sb.Append($"SD[{_dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                    sb.Append($"D[{_dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                    sb.Append($"DB[{_dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                    sb.Append($"!DB_old[{_dpf}{melding.RelatedInput1}] && DB[{_dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                    sb.Append($"ED[{_dpf}{melding.RelatedInput1}]");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                    sb.Append($"TDH_old[{_dpf}{melding.RelatedInput1}] && !TDH[{_dpf}{melding.RelatedInput1}]");
                    break;
            }
            if (melding.TweedeInput)
            {
                switch (melding.RelatedInput2Type)
                {
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectie:
                        sb.Append($" && SD[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieOp:
                        sb.Append($" && D[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.DetectieBezet:
                        sb.Append($" && DB[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.StartDetectieBezet:
                        sb.Append($" && !DB_old[{_dpf}{melding.RelatedInput2}] && DB[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectie:
                        sb.Append($" && ED[{_dpf}{melding.RelatedInput2}]");
                        break;
                    case PrioIngreepInUitMeldingVoorwaardeInputTypeEnum.EindeDetectieHiaat:
                        sb.Append($" && TDH_old[{_dpf}{melding.RelatedInput2}] && !TDH[{_dpf}{melding.RelatedInput2}]");
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
                    hov = _hprioovin.ToString();
                    schov = _schprioovin.ToString();
                    tov = _tprioovin.ToString();
                    break;
                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                default:
                    hov = _hprioovuit.ToString();
                    schov = _schprioovuit.ToString();
                    tov = _tprioovuit.ToString();
                    break;
            }

            var he = $"{_hpf}{hov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            var ti = $"{_tpf}{tov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            var sw = $"{_schpf}{schov}{ov.FaseCyclus}{GetMeldingShortcode(melding)}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}";
            if (melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                he = he + melding.RelatedInput1;
                ti = ti + melding.RelatedInput1;
                sw = sw + melding.RelatedInput1 + GetDetectorTypeSCHString(melding.RelatedInput1Type);
                if (melding.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector && melding.TweedeInput)
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
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding:
                    break;
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
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
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
                sb.Append($"!T[{_tpf}{tov}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] && ");
            }
            if (melding.AntiJutterTijdToepassen)
            {
                sb.Append($"!T[{ti}] && ");
            }
            if (melding.AlleenIndienGeenInmelding)
            {
                sb.Append($"!C[{_ctpf}{_cpriovc}{ov.FaseCyclus}] && ");
            }
            if (melding.KijkNaarWisselStand)
            {
                sb.Append($"IH[{_hpf}{_hpriowissel}{ov.FaseCyclus}] && ");
            }
            if (melding.AlleenIndienRood)
            {
                sb.Append($"R[{_fcpf}{ov.FaseCyclus}] && !T[{_tpf}{_tprioovminrood}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] && ");
            }

            var extra = "";
            if (ov.CheckLijnNummer && ov.LijnNummers.Any())
            {
                if (!ov.CheckRitCategorie)
                {
                    extra += "DSIMeldingOV_LijnNummer_V1(" +
                             $"{_prmpf + _prmprioallelijnen + ov.FaseCyclus + CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}, " +
                             $"{ov.LijnNummers.Count})";
                }
                else
                {
                    extra += "DSIMeldingOV_LijnNummerEnRitCategorie_V1(" +
                             $"{_prmpf + _prmprioallelijnen + ov.FaseCyclus + CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}, " +
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
                    sb.AppendLine($"DSIMeldingOV_V1(0, " +
                                                    $"{vtgType}, " +
                                                    "TRUE, " +
                                                    $"{(fcNmr == -1 ? "NG" : (fcNmr > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (fcNmr - 200).ToString() : fcNmr.ToString()))}," +
                                                    "TRUE, " +
                                                    (melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "CIF_DSIN, " : "CIF_DSUIT, ") +
                                                    $"{extra});");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.SelectieveDetector:
                    sb.AppendLine($"DSIMeldingOV_V1({(_dpf + melding.RelatedInput1).ToUpper()}, " +
                                                    $"{vtgType}, " +
                                                    "FALSE, " +
                                                    "NG, " +
                                                    $"SCH[{_schpf}{_schpriocheckdstype}], " +
                                                    $"{(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "CIF_DSIN" : "CIF_DSUIT")}, " +
                                                    $"{extra});");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.Detector:
                    sb.AppendLine(GetMeldingDetectieCode(melding) + ";");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.VecomViaDetector:
                    sb.AppendLine($" SD[{_dpf}{melding.RelatedInput1}];");
                    break;
                case PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISInput:
                    sb.AppendLine($" {(melding.InUit == PrioIngreepInUitMeldingTypeEnum.Inmelding ? "SH" : "EH")}[{_hpf}{_hrisprio}{melding.RelatedInput1}];");
                    break;
            }
            sb.AppendLine($"{ts}}}");
            if (melding.OpvangStoring && melding.MeldingBijstoring != null && melding.Type != PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
            {
                inmHelems.AddRange(GetMeldingCode(c, ov, melding.MeldingBijstoring, sb, vtgType, fcNmr, ts, antiJutVoorAlles, true, he));
            }
            return inmHelems;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return null;

            StringBuilder sb = new StringBuilder();
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
                                sb.AppendLine($"{ts}RT[{_tpf}{_tprioovminrood}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = !R[{_fcpf}{ov.FaseCyclus}];");
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
                                sb.Append($"{ts}IH[{_hpf}{_hpriowissel}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = (");
                                if (ov.MeldingenData.Wissel1)
                                {
                                    if (ov.MeldingenData.Wissel1Type == OVIngreepInUitDataWisselTypeEnum.Ingang)
                                    {
                                        sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel1Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel1Input}])" :
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel1Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel1Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel1Input}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel1Input}])");
                                    }
                                    else
                                    {
                                        sb.Append(ov.MeldingenData.Wissel1InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel1Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel1Detector}])" :
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel1Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel1Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel1Detector}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel1Detector}])");
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
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel2Input}] ? !IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel2Input}])" :
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel2Input}] ? IS[{_ispf}{ov.MeldingenData.Wissel2Input}] : !IS[{_ispf}{ov.MeldingenData.Wissel2Input}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel2Input}])");
                                    }
                                    else
                                    {
                                        sb.Append(ov.MeldingenData.Wissel2InputVoorwaarde ?
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel2Detector}] ? !D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel2Detector}])" :
                                            $"((SCH[{_schpf}{_schpriowisselpol}{ov.MeldingenData.Wissel2Detector}] ? D[{_dpf}{ov.MeldingenData.Wissel2Detector}] : !D[{_dpf}{ov.MeldingenData.Wissel2Detector}]) || SCH[{_schpf}{_schpriogeenwissel}{ov.MeldingenData.Wissel2Detector}])");
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
                    foreach (var ov in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usprioovinm}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = C[{_ctpf}{_cpriovc}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}];");
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspriohdinm}{hd.FaseCyclus}] = C[{_ctpf}{_cpriovchd}{hd.FaseCyclus}];");
                    }
                    if (c.HasKAR())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Verklikken melding en ondergedrag KAR */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspriokarmelding}] = T[{_tpf}{_tpriokarmelding}];");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspriokarog}] = !T[{_tpf}{_tpriokarog}];");
                    }
                    if (c.PrioData.PrioIngrepen.Any() || c.PrioData.HDIngrepen.Any())
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{ts}/* Verklikken overschreiding maximale wachttijd */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uspriomaxwt}] = FALSE;");
                        sb.AppendLine($"{ts}for (ov = 0; ov < ovOVMAX; ++ov)");
                        sb.AppendLine($"{ts}{ts}CIF_GUS[{_uspf}{_uspriomaxwt}] |= iMaximumWachtTijdOverschreden[ov];");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    if (c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Nooit) return "";
                    sb.AppendLine($"{ts}/* Verklikken wijzigingen OV-teller */");
                    var sch = c.PrioData.VerklikkenPrioTellerUber == NooitAltijdAanUitEnum.Altijd ? "NG" : $"{_schpf}{_schpriocovuber}";
                    foreach (var ov in c.PrioData.PrioIngrepen)
                    {
                        sb.AppendLine($"{ts}OV_tprioeller({_ctpf}{_cpriovc}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}, {sch});");
                    }
                    foreach (var hd in c.PrioData.HDIngrepen)
                    {
                        sb.AppendLine($"{ts}OV_teller({_ctpf}{_cpriovchd}{hd.FaseCyclus}, {sch});");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    foreach (var ov in c.PrioData.PrioIngrepen)
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
                            case OVIngreepVoertuigTypeEnum.Vrachtwagen:
                            case OVIngreepVoertuigTypeEnum.Fiets:
                            case OVIngreepVoertuigTypeEnum.NG:
                                vtgType = "NG";
                                break;
                            default:
                                throw new IndexOutOfRangeException();
                        }

                        if (ov.MeldingenData.Inmeldingen.Any())
                        {
                            var inmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Inmelding {_fcpf}{ov.FaseCyclus} type {ov.Type.GetDescription()} */");

                            var sb2 = new StringBuilder();
                            foreach (var inm in ov.MeldingenData.Inmeldingen)
                            {
                                inmHelems.AddRange(GetMeldingCode(c, ov, inm, sb2, vtgType, fcNmr, ts, ov.MeldingenData.AntiJutterVoorAlleInmeldingen));
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hprioovin}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
                            foreach (var i in inmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");
                            sb.Append(sb2.ToString());

                            sb.Append($"{ts}IH[{_hpf}{_hprioovin}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
                            if (ov.MeldingenData.AntiJutterVoorAlleInmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tprioovin}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
                            }
                            first = true;
                            foreach (var i in inmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                        }

                        if (ov.MeldingenData.Uitmeldingen.Any())
                        {
                            var uitmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{ov.FaseCyclus} type {ov.Type.GetDescription()} */");

                            var sb2 = new StringBuilder();
                            foreach (var uitm in ov.MeldingenData.Uitmeldingen)
                            {
                                uitmHelems.AddRange(GetMeldingCode(c, ov, uitm, sb2, vtgType, fcNmr, ts, ov.MeldingenData.AntiJutterVoorAlleUitmeldingen));
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hprioovuit}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
                            foreach (var i in uitmHelems)
                            {
                                sb.Append($"IH[{i}] = ");
                            }
                            sb.AppendLine("FALSE;");
                            sb.Append(sb2.ToString());

                            sb.Append($"{ts}IH[{_hpf}{_hprioovuit}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
                            if (ov.MeldingenData.AntiJutterVoorAlleUitmeldingen)
                            {
                                sb.Append($"RT[{_tpf}{_tprioovuit}{ov.FaseCyclus}{CCOLCodeHelper.GetPriorityTypeAbbreviation(ov)}] = ");
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
                    
                    // Inmelding HD
                    foreach (var hd in c.PrioData.HDIngrepen.Where(x => x.KAR || x.Opticom))
                    {
                        if (int.TryParse(hd.FaseCyclus, out var ifc))
                        {
                            var actualIfc = ifc > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (ifc - 200).ToString() : ifc.ToString();
                            var inmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Inmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                if(hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                                {
                                    var actualAlsoFc = hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (hd.InmeldingOokDoorFase - 200).ToString() : hd.InmeldingOokDoorFase.ToString();
                                    sb.AppendLine($"{ts}" +
                                        $"IH[{_hpf}{_hpriohdin}{hd.FaseCyclus}kar] = " +
                                        $"RT[{_tpf}{_tpriohdin}{hd.FaseCyclus}kar] = " +
                                        $"!T[{_tpf}{_tpriohdin}{hd.FaseCyclus}kar] && " +
                                        $"SCH[{_schpf}{_schpriohdin}{hd.FaseCyclus}kar] && " +
                                        $"((DSIMelding_HD_V1({actualIfc}, CIF_DSIN, SCH[{_schpf}{_schpriochecksirene}{hd.FaseCyclus}]) || " +
                                          $"DSIMelding_HD_V1({actualAlsoFc}, CIF_DSIN, SCH[{_schpf}{_schpriochecksirene}{hd.FaseCyclus}])));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hpriohdin}{hd.FaseCyclus}kar] = RT[{_tpf}{_tpriohdin}{hd.FaseCyclus}kar] = !T[{_tpf}{_tpriohdin}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schpriohdin}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({actualIfc}, CIF_DSIN, SCH[{_schpf}{_schpriochecksirene}{hd.FaseCyclus}]));");
                                }
                                inmHelems.Add($"{_hpf}{_hpriohdin}{hd.FaseCyclus}kar");
                            }
                            if (hd.Opticom)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hpriohdin}{hd.FaseCyclus}opt] = !T[{_tpf}{_tpriohdin}{hd.FaseCyclus}opt] && SCH[{_schpf}{_schpriohdinuit}{hd.FaseCyclus}opt] && !C[{_ctpf}{_cpriovchd}{hd.FaseCyclus}] && DB[{_dpf}{hd.OpticomRelatedInput}];");
                                sb.AppendLine($"{ts}RT[{_tpf}{_tpriohdin}{hd.FaseCyclus}opt] = G[{_fcpf}{hd.FaseCyclus}] && C[{_ctpf}{_cpriovchd}{hd.FaseCyclus}];");
                                inmHelems.Add($"{_hpf}{_hpriohdin}{hd.FaseCyclus}opt");
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hpriohdin}{hd.FaseCyclus}] = ");
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
                            var inmHelems = new List<string>();
                            if (!first) sb.AppendLine(); first = false;
                            sb.AppendLine($"{ts}/* Uitmelding HD {_fcpf}{hd.FaseCyclus} */");
                            if (hd.KAR)
                            {
                                if (hd.InmeldingOokDoorToepassen && hd.InmeldingOokDoorFase > 0)
                                {
                                    var actualAlsoFc = hd.InmeldingOokDoorFase > 200 && c.PrioData.VerlaagHogeSignaalGroepNummers ? (hd.InmeldingOokDoorFase - 200).ToString() : hd.InmeldingOokDoorFase.ToString();
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hpriohduit}{hd.FaseCyclus}kar] = RT[{_tpf}{_tpriohduit}{hd.FaseCyclus}kar] = !T[{_tpf}{_tpriohduit}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schpriohduit}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({actualIfc}, CIF_DSUIT, FALSE) || DSIMelding_HD_V1({actualAlsoFc}, CIF_DSUIT, FALSE));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}IH[{_hpf}{_hpriohduit}{hd.FaseCyclus}kar] = RT[{_tpf}{_tpriohduit}{hd.FaseCyclus}kar] = !T[{_tpf}{_tpriohduit}{hd.FaseCyclus}kar] && SCH[{_schpf}{_schpriohduit}{hd.FaseCyclus}kar] && (DSIMelding_HD_V1({actualIfc}, CIF_DSUIT, FALSE));");
                                }
                                inmHelems.Add($"{_hpf}{_hpriohduit}{hd.FaseCyclus}kar");
                            }
                            if (hd.Opticom)
                            {
                                sb.AppendLine($"{ts}IH[{_hpf}{_hpriohduit}{hd.FaseCyclus}opt] = SCH[{_schpf}{_schpriohdinuit}{hd.FaseCyclus}opt] && !TDH[{_dpf}{hd.OpticomRelatedInput}] && TDH_old[{_dpf}{hd.OpticomRelatedInput}];");
                                inmHelems.Add($"{_hpf}{_hpriohduit}{hd.FaseCyclus}opt");
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hpriohduit}{hd.FaseCyclus}] = ");
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

                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCPostAfhandelingOV:
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
                            sb.Append($"C[{_ctpf}{_cpriovchd}{hd.FaseCyclus}]");
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
                                    sb.AppendLine($"{ts}{ts}if (!C[{_ctpf}{_cpriovchd}{fc.Naam}]) {{ RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6; }}");
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
                                    sb.AppendLine($"{ts}{ts}if (!C[{_ctpf}{_cpriovchd}{fc.Naam}]) {{ RR[{_fcpf}{fc.Naam}] |= BIT6; Z[{_fcpf}{fc.Naam}] |= BIT6; }}");
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
                                if (c.PrioData.BlokkeerNietConflictenBijHDIngreep)
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
            _hrisprio = CCOLGeneratorSettingsProvider.Default.GetElementName("hrisprio");

            return base.SetSettings(settings);
        }
    }
}