using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
#pragma warning disable 0169
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
        private CCOLGeneratorCodeStringSettingModel _tdhkarin;
        private CCOLGeneratorCodeStringSettingModel _tdhkaruit;
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
        private CCOLGeneratorCodeStringSettingModel _prmlaatcrit;
        private CCOLGeneratorCodeStringSettingModel _prmallelijnen;
        private CCOLGeneratorCodeStringSettingModel _prmlijn;
        private CCOLGeneratorCodeStringSettingModel _prmmwta;
        private CCOLGeneratorCodeStringSettingModel _prmmwtfts;
        private CCOLGeneratorCodeStringSettingModel _prmmwtvtg;
        private CCOLGeneratorCodeStringSettingModel _schcprio;
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
        private CCOLGeneratorCodeStringSettingModel _schcheckopdsin;
        private CCOLGeneratorCodeStringSettingModel _uskarog;
        private CCOLGeneratorCodeStringSettingModel _uskarmelding;
        private CCOLGeneratorCodeStringSettingModel _tkarog;
        private CCOLGeneratorCodeStringSettingModel _tkarmelding;

        //private CCOLGeneratorCodeStringSettingModel _tinmdsi;
        //private CCOLGeneratorCodeStringSettingModel _hinmdsi;
        //private CCOLGeneratorCodeStringSettingModel _tuitmdsi;
        //private CCOLGeneratorCodeStringSettingModel _huitmdsi;
        //private CCOLGeneratorCodeStringSettingModel _schinmdsi;
        //private CCOLGeneratorCodeStringSettingModel _schuitmdsi;

        //private CCOLGeneratorCodeStringSettingModel _tinmkar;
        //private CCOLGeneratorCodeStringSettingModel _hinmkar;
        //private CCOLGeneratorCodeStringSettingModel _tuitmkar;
        //private CCOLGeneratorCodeStringSettingModel _huitmkar;
        //private CCOLGeneratorCodeStringSettingModel _schkarov;

        //private CCOLGeneratorCodeStringSettingModel _hinmwsk;
        //private CCOLGeneratorCodeStringSettingModel _huitmwsk;
        //private CCOLGeneratorCodeStringSettingModel _tuitmwsk;
        //private CCOLGeneratorCodeStringSettingModel _schinmwsk;
        //private CCOLGeneratorCodeStringSettingModel _schuitmwsk;

        //private CCOLGeneratorCodeStringSettingModel _hinmss;
        //private CCOLGeneratorCodeStringSettingModel _schinmss;
        //private CCOLGeneratorCodeStringSettingModel _huitmss;
        //private CCOLGeneratorCodeStringSettingModel _tuitmss;
        //private CCOLGeneratorCodeStringSettingModel _schuitmss;

        //private CCOLGeneratorCodeStringSettingModel _schgeenwissel;
#pragma warning restore 0169
#pragma warning restore 0649

        #endregion // Fields

        #region Properties
        #endregion // Properties

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyDetectors = new List<DetectorModel>();
            _MyBitmapOutputs = new List<CCOLIOElement>();
            _MyBitmapInputs = new List<CCOLIOElement>();

            if (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0)
            {
                /* Variables independent of signal groups */
                _MyElements.Add(new CCOLElement($"{_prmmwta}",   c.OVData.MaxWachttijdAuto,       CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmmwtfts}", c.OVData.MaxWachttijdFiets,      CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmmwtvtg}", c.OVData.MaxWachttijdVoetganger, CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));

                if (c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Any(x => x.KAR) ||
                    c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Any(x => x.KAR))
                {
                    _MyElements.Add(new CCOLElement($"{_uskarmelding}", CCOLElementTypeEnum.Uitgang));
                    _MyElements.Add(new CCOLElement($"{_uskarog}",      CCOLElementTypeEnum.Uitgang));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KARMeldingBitmapData, $"{_uspf}{_uskarmelding}"));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KAROnderGedragBitmapData, $"{_uspf}{_uskarog}"));

                    _MyElements.Add(new CCOLElement($"{_tkarmelding}", 15,   CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tkarog}",      1440, CCOLElementTimeTypeEnum.TM_type, CCOLElementTypeEnum.Timer));
                }
            }
            if (c.OVData.OVIngrepen.Count > 0)
            {
                /* Variables independent of signal groups */
                _MyElements.Add(new CCOLElement($"{_schcprio}",    0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                _MyElements.Add(new CCOLElement($"{_prmlaatcrit}", 0, CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));

                if (c.OVData.OVIngrepen.Any(x => x.KAR))
                {
                    var prmtest1 = new CCOLElement($"{_prmtestkarvert}", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter);
                    var prmtest2 = new CCOLElement($"{_prmtestkarlyn}", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    _MyElements.Add(prmtest1);
                    _MyElements.Add(prmtest2);
                }

				// TODO: This is not nice. Need to improve! should only be generated when needed
                //if (c.OVData.OVIngrepen.Where(x => x.Vecom).Any())
                //{
                _MyElements.Add(new CCOLElement($"{_schcheckopdsin}", c.OVData.CheckOpDSIN ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                //}

                /* Variables for conflicting signal groups */
                foreach (var ovfc in c.OVData.OVIngreepSignaalGroepParameters)
                {
                    if (!CCOLCodeHelper.HasSignalGroupConflictWithPT(c, ovfc.FaseCyclus))
                    {
                        continue;
                    }

                    var fct = c.Fasen.First(x => x.Naam == ovfc.FaseCyclus).Type;
                    
                    _MyElements.Add(new CCOLElement($"{_prmpmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                    _MyElements.Add(new CCOLElement($"{_prmognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen,         CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                    if(fct != FaseTypeEnum.Voetganger)
                    {
                        _MyElements.Add(new CCOLElement($"{_prmnofm}{ovfc.FaseCyclus}",   ovfc.AantalKerenNietAfkappen,              CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                        _MyElements.Add(new CCOLElement($"{_prmmgcov}{ovfc.FaseCyclus}",  ovfc.MinimumGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                        _MyElements.Add(new CCOLElement($"{_prmpmgcov}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdConflictOVRealisatie, CCOLElementTimeTypeEnum.None,    CCOLElementTypeEnum.Parameter));
                        _MyElements.Add(new CCOLElement($"{_prmohpmg}{ovfc.FaseCyclus}",  ovfc.OphoogpercentageNaAfkappen,           CCOLElementTimeTypeEnum.None,    CCOLElementTypeEnum.Parameter));
                    }
                }
            }

            /* Variables for OV */
            foreach (var ov in c.OVData.OVIngrepen)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(ov.OVInmeldingBitmapData, $"{_uspf}{_usovinm}{ov.FaseCyclus}"));

                _MyElements.Add(new CCOLElement($"{_usovinm}{ov.FaseCyclus}", CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(new CCOLElement($"{_hov}{ov.FaseCyclus}",     CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hovin}{ov.FaseCyclus}",   CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hovuit}{ov.FaseCyclus}",  CCOLElementTypeEnum.HulpElement));

                if (ov.KAR)
                {
                    _MyElements.Add(new CCOLElement($"{_tdhkarin}{ov.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tdhkaruit}{ov.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }

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
                if (ov.TussendoorRealiseren) opties += 30;
                if (ov.VasthoudenGroen) opties += 2;
                _MyElements.Add(new CCOLElement($"{_prmprio}{ov.FaseCyclus}", opties, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                // Note!!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                _MyElements.Add(new CCOLElement($"{_prmallelijnen}{ov.FaseCyclus}", ov.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));
                var n = 1;
                foreach (var l in ov.LijnNummers)
                {
	                if (!int.TryParse(l.Nummer, out var num)) continue;
	                _MyElements.Add(
		                new CCOLElement($"{_prmlijn}{ov.FaseCyclus}_{n:00}", num, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
	                ++n;
                }

                if(ov.KAR)
                {
                    _MyDetectors.Add(ov.DummyKARInmelding);
                    _MyDetectors.Add(ov.DummyKARUitmelding);
                }
                if (ov.Vecom)
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
                    if (hd.KAR)
                    {
                        _MyElements.Add(new CCOLElement($"{_tdhkarin}{hd.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                        _MyElements.Add(new CCOLElement($"{_tdhkaruit}{hd.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    }
                    _MyElements.Add(new CCOLElement($"{_tbtovg}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }

                if (hd.KAR)
                {
                    _MyDetectors.Add(hd.DummyKARInmelding);
                    _MyDetectors.Add(hd.DummyKARUitmelding);
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
                    if (c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Any(x => x.KAR) ||
                        c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Any(x => x.KAR))
                    {
                        sb.AppendLine($"{ts}/* Verklikken melding en ondergedrag KAR */");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarmelding}] = T[{_tpf}{_tkarmelding}];");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_uskarog}] = !T[{_tpf}{_tkarog}];");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
