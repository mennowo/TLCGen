using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class CCOLOVHDCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;
        private List<CCOLIOElement> _MyBitmapInputs;

#pragma warning disable 0649
#pragma warning disable 0169
        private string _cvc;
        private string _cvchd;
        private string _tgb;
        private string _tgbhd;
        private string _prmrto;
        private string _prmrtbg;
        private string _prmrtg;
        private string _prmrtohd;
        private string _prmrtbghd;
        private string _prmrtghd;
        private string _trt;
        private string _trthd;
        private string _tblk;
        private string _tbtovg;
        private string _tdhkarin;
        private string _tdhkaruit;
        private string _usovinm;
        private string _ushdinm;
        private string _hov;
        private string _hhd;
        private string _hovin;
        private string _hhdin;
        private string _hovuit;
        private string _hhduit;
        private string _prmomx;
        private string _prmprio;
        private string _prmpriohd;
        private string _prmlaatcrit;
        private string _prmallelijnen;
        private string _prmlijn;
        private string _prmmwta;
        private string _prmmwtfts;
        private string _prmmwtvtg;
        private string _schcprio;
        private string _prmovstp;
        private string _prmtestkarvert;
        private string _prmtestkarlyn;
        private string _isdummykarin;
        private string _isdummykaruit;
        private string _isdummyvecomin;
        private string _isdummyvecomuit;
        private string _isdummykarhdin;
        private string _isdummykarhduit;
        private string _isdummyvecomhdin;
        private string _isdummyvecomhduit;
        private string _schupinagb;
        private string _schupinagbhd;
        private string _prmpmgt;
        private string _prmognt;
        private string _prmnofm;
        private string _prmmgcov;
        private string _prmpmgcov;
        private string _prmohpmg;
        private string _schcheckopdsin;
        private string _uskarog;
        private string _uskarmelding;
        private string _tkarog;
        private string _tkarmelding;
#pragma warning restore 0169
#pragma warning restore 0649

        #endregion // Fields

        #region Properties
        #endregion // Properties

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();
            _MyBitmapInputs = new List<CCOLIOElement>();

            if (c.OVData.OVIngrepen.Count > 0 || c.OVData.HDIngrepen.Count > 0)
            {
                /* Variables independent of signal groups */
                _MyElements.Add(new CCOLElement($"{_prmmwta}",     0, CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmmwtfts}",   0, CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"{_prmmwtvtg}",   0, CCOLElementTimeTypeEnum.TS_type,  CCOLElementTypeEnum.Parameter));

                if ((c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Where(x => x.KAR).Any()) ||
                    (c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Where(x => x.KAR).Any()))
                {
                    _MyElements.Add(new CCOLElement($"{_uskarmelding}", CCOLElementTypeEnum.Uitgang));
                    _MyElements.Add(new CCOLElement($"{_uskarog}",      CCOLElementTypeEnum.Uitgang));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KARMeldingBitmapData as IOElementModel, $"{_uspf}{_uskarmelding}"));
                    _MyBitmapOutputs.Add(new CCOLIOElement(c.OVData.KAROnderGedragBitmapData as IOElementModel, $"{_uspf}{_uskarog}"));

                    _MyElements.Add(new CCOLElement($"{_tkarmelding}", 15,   CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tkarog}",      1440, CCOLElementTimeTypeEnum.TM_type, CCOLElementTypeEnum.Timer));
                }
            }
            if (c.OVData.OVIngrepen.Count > 0)
            {
                /* Variables independent of signal groups */
                _MyElements.Add(new CCOLElement($"{_schcprio}",    0, CCOLElementTimeTypeEnum.SCH_type, CCOLElementTypeEnum.Schakelaar));
                _MyElements.Add(new CCOLElement($"{_prmlaatcrit}", 0, CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));

                if (c.OVData.OVIngrepen.Where(x => x.KAR).Any())
                {
                    var prmtest1 = new CCOLElement($"{_prmtestkarvert}", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter);
                    var prmtest2 = new CCOLElement($"{_prmtestkarlyn}", 0, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter);
                    prmtest1.Dummy = true;
                    prmtest2.Dummy = true;
                    _MyElements.Add(prmtest1);
                    _MyElements.Add(prmtest2);
                }

#warning This is not nice. Need to improve! should only be generated when needed
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

                    var fct = c.Fasen.Where(x => x.Naam == ovfc.FaseCyclus).First().Type;
                    
                    _MyElements.Add(new CCOLElement($"{_prmpmgt}{ovfc.FaseCyclus}", ovfc.PercMaxGroentijdVoorTerugkomen, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                    _MyElements.Add(new CCOLElement($"{_prmognt}{ovfc.FaseCyclus}", ovfc.OndergrensNaTerugkomen,         CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Parameter));
                    if(fct != Models.Enumerations.FaseTypeEnum.Voetganger)
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
                _MyBitmapOutputs.Add(new CCOLIOElement(ov.OVInmeldingBitmapData as IOElementModel, $"{_uspf}{_usovinm}{ov.FaseCyclus}"));

                _MyElements.Add(new CCOLElement($"{_usovinm}{ov.FaseCyclus}", CCOLElementTypeEnum.Uitgang));
                _MyElements.Add(new CCOLElement($"{_hov}{ov.FaseCyclus}",     CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hovin}{ov.FaseCyclus}",   CCOLElementTypeEnum.HulpElement));
                _MyElements.Add(new CCOLElement($"{_hovuit}{ov.FaseCyclus}",  CCOLElementTypeEnum.HulpElement));

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
                int opties = 0;
                if (ov.AfkappenConflicten || ov.AfkappenConflictenOV) opties += 100;
                if (ov.AfkappenConflictenOV) opties += 300;
                if (ov.TussendoorRealiseren) opties += 20;
                if (ov.VasthoudenGroen) opties += 3;
                _MyElements.Add(new CCOLElement($"{_prmprio}{ov.FaseCyclus}", opties, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));

                if (ov.Vecom)
                {
                    var elem1 = new CCOLElement($"{_isdummyvecomin}{ov.FaseCyclus}",  CCOLElementTypeEnum.Ingang);
                    var elem2 = new CCOLElement($"{_isdummyvecomuit}{ov.FaseCyclus}", CCOLElementTypeEnum.Ingang);
                    elem1.Dummy = true;
                    elem2.Dummy = true;
                    _MyElements.Add(elem1);
                    _MyElements.Add(elem2);

                    var iselem1 = new CCOLIOElement(ov.OVVecomDummyInmeldingBitmapData as IOElementModel,  $"{_ispf}{_isdummyvecomin}{ov.FaseCyclus}");
                    var iselem2 = new CCOLIOElement(ov.OVVecomDummyUitmeldingBitmapData as IOElementModel, $"{_ispf}{_isdummyvecomuit}{ov.FaseCyclus}");
                    iselem1.Dummy = iselem2.Dummy = true;
                    _MyBitmapInputs.Add(iselem1);
                    _MyBitmapInputs.Add(iselem2);
                }

                if (ov.KAR)
                {
                    var elem1 = new CCOLElement($"{_isdummykarin}{ov.FaseCyclus}",  CCOLElementTypeEnum.Ingang);
                    var elem2 = new CCOLElement($"{_isdummykaruit}{ov.FaseCyclus}", CCOLElementTypeEnum.Ingang);
                    elem1.Dummy = true;
                    elem2.Dummy = true;
                    _MyElements.Add(elem1);
                    _MyElements.Add(elem2);

                    var iselem1 = new CCOLIOElement(ov.OVKARDummyInmeldingBitmapData as IOElementModel,  $"{_ispf}{_isdummykarin}{ov.FaseCyclus}");
                    var iselem2 = new CCOLIOElement(ov.OVKARDummyUitmeldingBitmapData as IOElementModel, $"{_ispf}{_isdummykaruit}{ov.FaseCyclus}");
                    iselem1.Dummy = iselem2.Dummy = true;
                    _MyBitmapInputs.Add(iselem1);
                    _MyBitmapInputs.Add(iselem2);

                    _MyElements.Add(new CCOLElement($"{_tdhkarin}{ov.FaseCyclus}",  15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tdhkaruit}{ov.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }

                // Note!!! "allelijnen" must alway be DIRECTLY above the line prms, cause of the way these prms are used in code
                _MyElements.Add(new CCOLElement($"{_prmallelijnen}{ov.FaseCyclus}", ov.AlleLijnen == true ? 1 : 0, CCOLElementTimeTypeEnum.None,     CCOLElementTypeEnum.Parameter));
                int n = 1;
                foreach (var l in ov.LijnNummers)
                {
                    int num;
                    if (Int32.TryParse(l.Nummer, out num))
                    {
                        _MyElements.Add(
                            new CCOLElement($"{_prmlijn}{ov.FaseCyclus}_{n.ToString("00")}", num, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                        ++n;
                    }
                }
            }

            /* Variables for HD */
            foreach (var hd in c.OVData.HDIngrepen)
            {
                _MyBitmapOutputs.Add(new CCOLIOElement(hd.HDInmeldingBitmapData as IOElementModel, $"{_uspf}{_ushdinm}{hd.FaseCyclus}"));

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
                
                if (hd.KAR)
                {
                    var elem1 = new CCOLElement($"{_isdummykarhdin}{hd.FaseCyclus}",  CCOLElementTypeEnum.Ingang);
                    var elem2 = new CCOLElement($"{_isdummykarhduit}{hd.FaseCyclus}", CCOLElementTypeEnum.Ingang);
                    elem1.Dummy = true;
                    elem2.Dummy = true;
                    _MyElements.Add(elem1);
                    _MyElements.Add(elem2);

                    var iselem1 = new CCOLIOElement(hd.HDKARDummyInmeldingBitmapData as IOElementModel,  $"{_ispf}{_isdummykarhdin}{hd.FaseCyclus}");
                    var iselem2 = new CCOLIOElement(hd.HDKARDummyUitmeldingBitmapData as IOElementModel, $"{_ispf}{_isdummykarhduit}{hd.FaseCyclus}");
                    iselem1.Dummy = iselem2.Dummy = true;
                    _MyBitmapInputs.Add(iselem1);
                    _MyBitmapInputs.Add(iselem2);
                }

                // For signal groups that have HD but not OV
                if(!c.OVData.OVIngrepen.Where(x => x.FaseCyclus == hd.FaseCyclus).Any())
                {
                    _MyElements.Add(new CCOLElement($"{_tdhkarin}{hd.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tdhkaruit}{hd.FaseCyclus}", 15, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                    _MyElements.Add(new CCOLElement($"{_tbtovg}{hd.FaseCyclus}", 0, CCOLElementTimeTypeEnum.TE_type, CCOLElementTypeEnum.Timer));
                }
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

        public override bool HasCCOLBitmapInputs()
        {
            return true;
        }

        public override IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            return _MyBitmapInputs;
        }

        public override bool HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.SystemApplication:
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
                case CCOLRegCCodeTypeEnum.SystemApplication:
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
                    if ((c.OVData.OVIngrepen.Count > 0 && c.OVData.OVIngrepen.Where(x => x.KAR).Any()) ||
                        (c.OVData.HDIngrepen.Count > 0 && c.OVData.HDIngrepen.Where(x => x.KAR).Any()))
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
