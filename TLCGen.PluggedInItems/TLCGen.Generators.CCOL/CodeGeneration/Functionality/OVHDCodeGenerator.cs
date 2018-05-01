using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class OVHDCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private List<CCOLElement> _MyElements;
        private List<DetectorModel> _MyDetectors;
        private List<CCOLIOElement> _MyBitmapOutputs;
        private List<CCOLIOElement> _MyBitmapInputs;

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
        private CCOLGeneratorCodeStringSettingModel _prmmwta;
        private CCOLGeneratorCodeStringSettingModel _prmmwtfts;
        private CCOLGeneratorCodeStringSettingModel _prmmwtvtg;
        private CCOLGeneratorCodeStringSettingModel _prmovstp;
        private CCOLGeneratorCodeStringSettingModel _prmtestkarvert;
        private CCOLGeneratorCodeStringSettingModel _prmtestkarlyn;
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
        private CCOLGeneratorCodeStringSettingModel _uskarmelding;
        private CCOLGeneratorCodeStringSettingModel _tkarog;
        private CCOLGeneratorCodeStringSettingModel _tkarmelding;
        
        private CCOLGeneratorCodeStringSettingModel _tovin;
        private CCOLGeneratorCodeStringSettingModel _tovuit;
        private CCOLGeneratorCodeStringSettingModel _schovin;
        private CCOLGeneratorCodeStringSettingModel _schovuit;
        private CCOLGeneratorCodeStringSettingModel _schgeenwissel;

        private CCOLGeneratorCodeStringSettingModel _thdin;
        private CCOLGeneratorCodeStringSettingModel _thduit;
        private CCOLGeneratorCodeStringSettingModel _schhdin;
        private CCOLGeneratorCodeStringSettingModel _schhduit;
        private CCOLGeneratorCodeStringSettingModel _schhdinuit;

        private CCOLGeneratorCodeStringSettingModel _schchecksirene;
        private CCOLGeneratorCodeStringSettingModel _schwisselpol;
#pragma warning restore 0649

        #endregion // Fields

        #region Properties
        #endregion // Properties

        private string GetMeldingShortcode(OVIngreepMeldingTypeEnum type)
        {
            switch (type)
            {
                case OVIngreepMeldingTypeEnum.VECOM:
                    return "vec";
                case OVIngreepMeldingTypeEnum.KAR:
                    return "kar";
                case OVIngreepMeldingTypeEnum.VerlosDetector:
                    return "ss";
                case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                    return "wsk";
                case OVIngreepMeldingTypeEnum.WisselDetector:
                    return "wd";
                case OVIngreepMeldingTypeEnum.VECOM_io:
                    return "vecio";
                case OVIngreepMeldingTypeEnum.MassaPaarIn:
                    return "mpi";
                case OVIngreepMeldingTypeEnum.MassaPaarUit:
                    return "mpu";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyDetectors = new List<DetectorModel>();
            _MyBitmapOutputs = new List<CCOLIOElement>();
            _MyBitmapInputs = new List<CCOLIOElement>();

            if (c.OVData.OVIngrepen.Any() || c.OVData.HDIngrepen.Any())
            {
                /* Variables independent of signal groups */
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwta}",   c.OVData.MaxWachttijdAuto,       CCOLElementTimeTypeEnum.TS_type, _prmmwta));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtfts}", c.OVData.MaxWachttijdFiets,      CCOLElementTimeTypeEnum.TS_type, _prmmwtfts));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmwtvtg}", c.OVData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type, _prmmwtvtg));

                if (c.HasKAR())
                {
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarmelding}", _uskarmelding));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_uskarog}",      _uskarog));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KARMeldingBitmapData, $"{_uspf}{_uskarmelding}"));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KAROnderGedragBitmapData, $"{_uspf}{_uskarog}"));

                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarmelding}", 15,   CCOLElementTimeTypeEnum.TE_type, _tkarmelding));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tkarog}",      1440, CCOLElementTimeTypeEnum.TM_type, _tkarog));
                }
            }
            if (c.OVData.OVIngrepen.Any())
            {
                /* Variables independent of signal groups */
                
                if (c.OVData.OVIngrepen.Any(x => x.HasOVIngreepKAR()))
                {
                    var prmtest1 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestkarvert}", 0, CCOLElementTimeTypeEnum.None, _prmtestkarvert);
                    var prmtest2 = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmtestkarlyn}", 0, CCOLElementTimeTypeEnum.None, _prmtestkarlyn);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    _MyElements.Add(prmtest1);
                    _MyElements.Add(prmtest2);
                }
                
                foreach (var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselIngang || x.Type == DetectorTypeEnum.WisselDetector))
                {
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schgeenwissel}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schgeenwissel, d.Naam));
                }
                
                foreach(var d in c.GetAllDetectors(x => x.Type == DetectorTypeEnum.WisselIngang))
                {
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schwisselpol}{d.Naam}", 0, CCOLElementTimeTypeEnum.SCH_type, _schwisselpol, d.Naam));
                }

                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schcheckdstype}", c.OVData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schcheckdstype));
                
                /* Variables for conflicting signal groups */
                foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
                {
                    if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                    {
                        continue;
                    }

                    var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                    
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.TE_type, _prmpmgt, ovfc.FaseCyclus));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen,         CCOLElementTimeTypeEnum.TE_type, _prmognt, ovfc.FaseCyclus));
                    if(fct != FaseTypeEnum.Voetganger)
                    {
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmnofm}{ovfc.FaseCyclus}",   ovfc.AantalKerenNietAfkappen,              CCOLElementTimeTypeEnum.TE_type, _prmnofm, ovfc.FaseCyclus));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmmgcov}{ovfc.FaseCyclus}",  ovfc.MinimumGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.TE_type, _prmmgcov, ovfc.FaseCyclus));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmpmgcov}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.None,    _prmpmgcov, ovfc.FaseCyclus));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmohpmg}{ovfc.FaseCyclus}",  ovfc.OphoogpercentageNaAfkappen,           CCOLElementTimeTypeEnum.None,    _prmohpmg, ovfc.FaseCyclus));
                    }
                }
            }

            /* Variables for OV */
            foreach (var ov in c.OVData.OVIngrepen)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(ov.OVInmeldingBitmapData, $"{_uspf}{_usovinm}{ov.FaseCyclus}"));

                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovinm}{ov.FaseCyclus}", _usovinm, ov.FaseCyclus));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hov}{ov.FaseCyclus}",     _hov, ov.FaseCyclus));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovin}{ov.FaseCyclus}",   _hovin, ov.FaseCyclus));
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovuit}{ov.FaseCyclus}",  _hovuit, ov.FaseCyclus));
                
                foreach(var m in ov.Meldingen)
                {
                    if (m.Inmelding)
                    {
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(m.Type)}", _hovin, ov.FaseCyclus, m.Type.GetDescription()));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(m.Type)}", m.InmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _tovin, ov.FaseCyclus, m.Type.GetDescription()));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(m.Type)}", 1, CCOLElementTimeTypeEnum.SCH_type, _schovin, ov.FaseCyclus, m.Type.GetDescription()));
                    }
                    if (m.Uitmelding)
                    {
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(m.Type)}", _hovuit, ov.FaseCyclus, m.Type.GetDescription()));
                        _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(m.Type)}", 1, CCOLElementTimeTypeEnum.SCH_type, _schovuit, ov.FaseCyclus, m.Type.GetDescription()));
                    }
                }
                _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_tovuit}{ov.FaseCyclus}", ov.UitmeldFilterTijd, CCOLElementTimeTypeEnum.TE_type, _tovuit, ov.FaseCyclus));

                _MyElements.Add(new CCOLElement($"{_tbtovg}{ov.FaseCyclus}",        0,                             CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_trt}{ov.FaseCyclus}",           0,                             CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_cvc}{ov.FaseCyclus}",           999,                           CCOLElementTimeTypeEnum.CT_type,  CCOLElementTypeEnum.Counter));
                _MyElements.Add(new CCOLElement($"{_tgb}{ov.FaseCyclus}",           ov.GroenBewaking,              CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_prmrto}{ov.FaseCyclus}",        ov.RijTijdOngehinderd,         CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmrtbg}{ov.FaseCyclus}",       ov.RijTijdBeperktgehinderd,    CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmrtg}{ov.FaseCyclus}",        ov.RijTijdGehinderd,           CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmomx}{ov.FaseCyclus}",        ov.OnderMaximum,               CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_tblk}{ov.FaseCyclus}",          0,                             CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_schupinagb}{ov.FaseCyclus}",    0,                             CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                _MyElements.Add(new CCOLElement($"{_prmovstp}{ov.FaseCyclus}",      0,                             CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));
                if (ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ||
                    ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchUit)
                {
                    _MyElements.Add(new CCOLElement($"{_schvi}{ov.FaseCyclus}",    ov.VersneldeInmeldingKoplus == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                }
                var opties = 0;
                if (ov.AfkappenConflicten || ov.AfkappenConflictenOV) opties += 100;
                if (ov.AfkappenConflictenOV) opties += 300;
                if (ov.TussendoorRealiseren) opties += 3;
                if (ov.VasthoudenGroen) opties += 20;
                var sopties = opties.ToString().Replace("0", "");
                _MyElements.Add(new CCOLElement($"{_prmprio}{ov.FaseCyclus}", int.Parse(sopties), CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                if (ov.CheckLijnNummer)
                {
                    // Note!!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                    _MyElements.Add(new CCOLElement($"{_prmallelijnen}{ov.FaseCyclus}", ov.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    var n = 1;
                    foreach (var l in ov.LijnNummers)
                    {
                        if (!int.TryParse(l.Nummer, out var num)) continue;
                        _MyElements.Add(
                            new CCOLElement($"{_prmlijn}{ov.FaseCyclus}_{n:00}", num, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                        ++n;
                    }
                }

                if(ov.HasOVIngreepKAR())
                {
                    _MyDetectors.Add(ov.DummyKARInmelding);
                    _MyDetectors.Add(ov.DummyKARUitmelding);
                }
                if (ov.HasOVIngreepVecom())
                {
                    _MyDetectors.Add(ov.DummyVecomInmelding);
                    _MyDetectors.Add(ov.DummyVecomUitmelding);
                }
            }

            /* Variables for HD */
            foreach (var hd in c.OVData.HDIngrepen)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(hd.HDInmeldingBitmapData, $"{_uspf}{_ushdinm}{hd.FaseCyclus}"));

                _MyElements.Add(new CCOLElement($"{_ushdinm}{hd.FaseCyclus}", CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(new CCOLElement($"{_hhd}{hd.FaseCyclus}",     CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hhdin}{hd.FaseCyclus}",   CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hhduit}{hd.FaseCyclus}",  CCOLElementTypeEnum.HulpElement));

                _MyElements.Add(new CCOLElement($"{_tgbhd}{hd.FaseCyclus}",        hd.GroenBewaking,           CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_trthd}{hd.FaseCyclus}",        0,                          CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Timer));
                _MyElements.Add(new CCOLElement($"{_cvchd}{hd.FaseCyclus}",        999,                        CCOLElementTimeTypeEnum.CT_type,  CCOLElementTypeEnum.Counter));
                _MyElements.Add(new CCOLElement($"{_prmpriohd}{hd.FaseCyclus}",    9005,                       CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmrtohd}{hd.FaseCyclus}",     hd.RijTijdOngehinderd,      CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmrtbghd}{hd.FaseCyclus}",    hd.RijTijdBeperktgehinderd, CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmrtghd}{hd.FaseCyclus}",     hd.RijTijdGehinderd,        CCOLElementTimeTypeEnum.TE_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_schupinagbhd}{hd.FaseCyclus}", 0,                          CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));

                // For signal groups that have HD but not OV
                if(c.OVData.OVIngrepen.All(x => x.FaseCyclus != hd.FaseCyclus))
                {
                    _MyElements.Add(new CCOLElement($"{_tbtovg}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }

                if (hd.KAR)
                {
                    _MyDetectors.Add(hd.DummyKARInmelding);
                    _MyDetectors.Add(hd.DummyKARUitmelding);

                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}kar", _hhdin, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}kar", hd.KARInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdin}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdin, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}kar", _hhduit, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thduit}{hd.FaseCyclus}kar", hd.KARUitmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thduit, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhduit}{hd.FaseCyclus}kar", 1, CCOLElementTimeTypeEnum.SCH_type, _schhduit, hd.FaseCyclus, "KAR"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schchecksirene}{hd.FaseCyclus}", hd.Sirene ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schchecksirene, hd.FaseCyclus));
                }
                if (hd.Opticom)
                {
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhdin}{hd.FaseCyclus}opt", _hhdin, hd.FaseCyclus, "Opticom"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_thdin}{hd.FaseCyclus}opt", hd.OpticomInmeldingFilterTijd ?? 15, CCOLElementTimeTypeEnum.TE_type, _thdin, hd.FaseCyclus, "Opticom"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schhdinuit}{hd.FaseCyclus}opt", 1, CCOLElementTimeTypeEnum.SCH_type, _schhdinuit, hd.FaseCyclus, "Opticom"));
                    _MyElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hhduit}{hd.FaseCyclus}opt", _hhduit, hd.FaseCyclus, "Opticom"));
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

        public override bool HasCCOLBitmapInputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _MyBitmapInputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return 40;
                case CCOLCodeTypeEnum.OvCInUitMelden:
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
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    sb.AppendLine($"{ts}/* OV/HD verklikking */");
                    sb.AppendLine($"{ts}/* ----------------- */");
                    foreach(var ov in c.OVData.OVIngrepen)
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
                    return sb.ToString();
                case CCOLCodeTypeEnum.OvCInUitMelden:
                    sb.AppendLine($"{ts}/* Afzetten hulpelementen voor in- en uitmeldingen */");
                    foreach (var ov in c.OVData.OVIngrepen)
                    {
                        sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = FALSE;");
                    }
                    sb.AppendLine();
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
                            default:
                                throw new IndexOutOfRangeException();
                        }
                        if (ov.Meldingen.Any(x => x.Inmelding))
                        {
                            var inmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Inmelding {_fcpf}{ov.FaseCyclus} */");
                            foreach (var melding in ov.Meldingen)
                            {
                                if (!melding.Inmelding) continue;
                                switch (melding.Type)
                                {
                                    #region Inmelding OV VECOM
                                    case OVIngreepMeldingTypeEnum.VECOM:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}] && " +
                                            $"DSIMeldingOV_V1({_dpf}{melding.RelatedInput1}, " +
                                                              $"{vtgType}, " +
                                                              $"{(fcNmr == -1 ? "NG" : fcNmr.ToString())}," +
                                                              $"SCH[{_schpf}{_schcheckdstype}, " +
                                                              $"CIF_DSIN, " +
                                                              $"{(ov.CheckLijnNummer ? "TRUE" : "FALSE")}, " +
                                                              $"{(ov.CheckLijnNummer ? _prmpf + _prmallelijnen + ov.FaseCyclus : "NG")}, " +
                                                              $"{(ov.CheckLijnNummer ? ov.LijnNummers.Count : 0)}, " +
                                                              $"TRUE) && " +
                                            $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion Inmelding OV VECOM
                                    #region Inmelding OV KAR
                                    case OVIngreepMeldingTypeEnum.KAR:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                            $"DSIMeldingOV_V1(0, " +
                                                              $"{vtgType}, " +
                                                              $"{(fcNmr == -1 ? "NG" : fcNmr.ToString())}," +
                                                              $"TRUE, " +
                                                              $"CIF_DSIN, " +
                                                              $"{(ov.CheckLijnNummer ? "TRUE" : "FALSE")}, " +
                                                              $"{(ov.CheckLijnNummer ? _prmpf + _prmallelijnen + ov.FaseCyclus : "NG")}, " +
                                                              $"{(ov.CheckLijnNummer ? ov.LijnNummers.Count : 0)}, " +
                                                              $"TRUE) && " +
                                            $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Inmelding OV KAR
                                    #region Inmelding OV Verlos detector
                                    case OVIngreepMeldingTypeEnum.VerlosDetector:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ? 
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        else
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Inmelding OV Verlos detector
                                    #region Inmelding OV Wisselstroomkring detector
                                    case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                                        // TODO: optie voor wissel input
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && " +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        break;
                                    #endregion //Inmelding OV Wisselstroomkring detector
                                    #region Inmelding OV Massa paar in
                                    case OVIngreepMeldingTypeEnum.MassaPaarIn:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                      $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                      $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && ");
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}{ts}(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && ");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine((ov.WisselStandVoorwaarde ?
                                                          $"{ts}{ts}((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && " :
                                                          $"{ts}{ts}((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && "));
                                        }
                                        sb.AppendLine($"{ts}{ts}((CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                      $"SD[{_dpf}{melding.RelatedInput2}] && D[{_dpf}{melding.RelatedInput1}] ||" + Environment.NewLine +
                                                      $"{ts}{ts} (CIF_IS[{_dpf}{melding.RelatedInput2}] >= CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                      $"ED[{_dpf}{melding.RelatedInput1}] ||" + Environment.NewLine +
                                                      $"{ts}{ts} (CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] >= CIF_DET_STORING) && " +
                                                      $"SD[{_dpf}{melding.RelatedInput2}]) && " +
                                                      $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Inmelding OV Massa paar in
                                    #region Inmelding OV VECOM IO
                                    case OVIngreepMeldingTypeEnum.VECOM_io:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" +
                                                $"IS[{_ispf}{melding.RelatedInput1}] && !IS_old[{_ispf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) &&" +
                                                $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"IS[{_ispf}{melding.RelatedInput1}] && !IS_old[{_ispf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) &&" +
                                                $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        else
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"RT[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"IS[{_ispf}{melding.RelatedInput1}] && !IS_old[{_ispf}{melding.RelatedInput1}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        inmHelems.Add($"{_hpf}{_hovin}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Inmelding OV VECOM IO
                                    case OVIngreepMeldingTypeEnum.WisselDetector: // alleen uitmelding mogelijk
                                    case OVIngreepMeldingTypeEnum.MassaPaarUit:   // alleen uitmelding mogelijk
                                        break;
                                    default:
                                        throw new IndexOutOfRangeException();
                                }
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hovin}{ov.FaseCyclus}] = ");
                            var first = true;
                            foreach (var i in inmHelems)
                            {
                                if (!first) sb.Append(" || ");
                                sb.Append($"IH[{i}]");
                                first = false;
                            }
                            sb.AppendLine(";");
                            sb.AppendLine();
                        }

                        if (ov.Meldingen.Any(x => x.Uitmelding))
                        {
                            var uitmHelems = new List<string>();
                            sb.AppendLine($"{ts}/* Uitmelding {_fcpf}{ov.FaseCyclus} */");
                            foreach (var melding in ov.Meldingen)
                            {
                                if (!melding.Uitmelding) continue;
                                switch (melding.Type)
                                {
                                    #region Uitmelding OV Vecom
                                    case OVIngreepMeldingTypeEnum.VECOM:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                            $"DSIMeldingOV_V1({_dpf}{melding.RelatedInput2}, " +
                                                              $"{vtgType}, " +
                                                              $"{(fcNmr == -1 ? "NG" : fcNmr.ToString())}," +
                                                              $"SCH[{_schpf}{_schcheckdstype}, " +
                                                              $"CIF_DSIN, " +
                                                              $"{(ov.CheckLijnNummer ? "TRUE" : "FALSE")}, " +
                                                              $"{(ov.CheckLijnNummer ? _prmpf + _prmallelijnen + ov.FaseCyclus : "NG")}, " +
                                                              $"{(ov.CheckLijnNummer ? ov.LijnNummers.Count : 0)}, " +
                                                              $"TRUE) && " +
                                            $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}];");
                                        uitmHelems.Add($"{_hpf}uit{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Uitmelding OV Vecom
                                    #region Uitmelding OV KAR
                                    case OVIngreepMeldingTypeEnum.KAR:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                            $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                            $"DSIMeldingOV_V1(0, " +
                                                              $"{vtgType}, " +
                                                              $"{(fcNmr == -1 ? "NG" : fcNmr.ToString())}," +
                                                              $"TRUE, " +
                                                              $"CIF_DSUIT, " +
                                                              $"{(ov.CheckLijnNummer ? "TRUE" : "FALSE")}, " +
                                                              $"{(ov.CheckLijnNummer ? _prmpf + _prmallelijnen + ov.FaseCyclus : "NG")}, " +
                                                              $"{(ov.CheckLijnNummer ? ov.LijnNummers.Count : 0)}, " +
                                                              $"TRUE) && " +
                                            $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}];");
                                        uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Uitmelding OV KAR
                                    #region Uitmelding OV Verlos detector
                                    case OVIngreepMeldingTypeEnum.VerlosDetector:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && " +
                                                $"!TDH[{_dpf}{melding.RelatedInput1}] && " +
                                                $"TDH_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"!TDH[{_dpf}{melding.RelatedInput1}] && " +
                                                $"TDH_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        else
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"!TDH[{_dpf}{melding.RelatedInput1}] && " +
                                                $"TDH_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                        }
                                        uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Uitmelding OV Verlos detector
                                    #region Uitmelding OV Wisselstroomkring detector
                                    case OVIngreepMeldingTypeEnum.WisselStroomKringDetector:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || " +
                                                $"SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && " +
                                                $"!TDH[{_dpf}{melding.RelatedInput1}] && " +
                                                $"TDH_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"!TDH[{_dpf}{melding.RelatedInput1}] && " +
                                                $"TDH_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        break;
                                    #endregion // Uitmelding OV Wisselstroomkring detector
                                    #region Uitmelding OV Wisseldetector
                                    case OVIngreepMeldingTypeEnum.WisselDetector:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && " +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] && " +
                                                $"!DB_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"R[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"!TRG[{_fcpf}{ov.FaseCyclus}] && " +
                                                $"DB[{_dpf}{melding.RelatedInput1}] && " +
                                                $"!DB_old[{_dpf}{melding.RelatedInput1}] && " +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}] && " +
                                                $"(C_counter[{_ctpf}{_cvc}{ov.FaseCyclus}] == 0);");
                                            uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        }
                                        break;
                                    #endregion // Uitmelding OV Wisseldetector
                                    #region Uitmelding OV VECOM IO
                                    case OVIngreepMeldingTypeEnum.VECOM_io:
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" +
                                                $"IS[{_ispf}{melding.RelatedInput2}] && !IS_old[{_ispf}{melding.RelatedInput2}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) &&" +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                (ov.WisselStandVoorwaarde ?
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                    $"((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&") +
                                                $"IS[{_ispf}{melding.RelatedInput2}] && !IS_old[{_ispf}{melding.RelatedInput2}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) &&" +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        else
                                        {
                                            sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && " +
                                                $"IS[{_ispf}{melding.RelatedInput2}] && !IS_old[{_ispf}{melding.RelatedInput2}] &&" +
                                                $"(CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) && " +
                                                $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}];");
                                        }
                                        uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Uitmelding OV VECOM IO
                                    #region Uitmelding OV Massa paar uit
                                    case OVIngreepMeldingTypeEnum.MassaPaarUit:
                                        sb.AppendLine($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] = " +
                                                      $"SCH[{_schpf}{_schovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}] && ");
                                        if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Detector && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine($"{ts}{ts}(D[{_dpf}{ov.WisselStandInput}] || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) && ");
                                        }
                                        else if (ov.Wissel && ov.WisselType == OVIngreepWisselTypeEnum.Ingang && ov.WisselStandInput != null)
                                        {
                                            sb.AppendLine((ov.WisselStandVoorwaarde ?
                                                          $"{ts}{ts}((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? !IS[{_ispf}{ov.WisselStandInput}] : IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&" :
                                                          $"{ts}{ts}((SCH[{_schpf}{_schwisselpol}{ov.WisselStandInput}] ? IS[{_ispf}{ov.WisselStandInput}] : !IS[{_ispf}{ov.WisselStandInput}]) || SCH[{_schpf}{_schgeenwissel}{ov.WisselStandInput}]) &&"));
                                        }
                                        sb.AppendLine($"{ts}{ts}((CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                      $"D[{_dpf}{melding.RelatedInput2}] && ED[{_dpf}{melding.RelatedInput1}] ||" + Environment.NewLine +
                                                      $"{ts}{ts} (CIF_IS[{_dpf}{melding.RelatedInput2}] >= CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] < CIF_DET_STORING) && " +
                                                      $"ED[{_dpf}{melding.RelatedInput1}] ||" + Environment.NewLine +
                                                      $"{ts}{ts} (CIF_IS[{_dpf}{melding.RelatedInput2}] < CIF_DET_STORING) && (CIF_IS[{_dpf}{melding.RelatedInput1}] >= CIF_DET_STORING) && " +
                                                      $"ED[{_dpf}{melding.RelatedInput2}]) && " +
                                                      $"!T[{_tpf}{_tovuit}{ov.FaseCyclus}];");
                                        uitmHelems.Add($"{_hpf}{_hovuit}{ov.FaseCyclus}{GetMeldingShortcode(melding.Type)}");
                                        break;
                                    #endregion // Uitmelding OV Massa paar uit
                                    case OVIngreepMeldingTypeEnum.MassaPaarIn: // alleen inmelding mogelijk
                                        break;
                                    default:
                                        throw new IndexOutOfRangeException();
                                }
                            }
                            sb.Append($"{ts}IH[{_hpf}{_hovuit}{ov.FaseCyclus}] = RT[{_tpf}{_tovuit}{ov.FaseCyclus}] = ");
                            var first = true;
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
                    sb.AppendLine();

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
                            var first = true;
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
                            var first = true;
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
                default:
                    return null;
            }
        }
    }
}
