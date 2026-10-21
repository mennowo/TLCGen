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
    // Generic priority (prio) code generation. This is the only registered generator for prio: its settings
    // (element names) are keyed by its class name, so it must not be renamed or split into several generators.
    // The code is split over partial files per topic, each listing the hooks (CCOLCodeTypeEnum) it serves:
    // - PrioCodeGenerator.cs              fields and settings, HasCode, local variables, GetCode dispatcher;
    //                                     PrioCIncludes, RegCTop, RegCWachtgroen and PrioCInitPrio are inline
    // - PrioCodeGenerator.Elements.cs     CCOL elements (CollectCCOLElements)
    // - PrioCodeGenerator.Meldingen.cs    in- and uitmeldingen: PrioCInUitMelden (prio.c InUitMelden())
    // - PrioCodeGenerator.Wissel.cs       RegCPreApplication (reg.c PreApplication())
    // - PrioCodeGenerator.Naloop.cs       RegCRealisatieAfhandelingNaModules (reg.c RealisatieAfhandeling())
    // - PrioCodeGenerator.Verklikking.cs  RegCSystemApplication, RegCPostSystemApplication (reg.c system_application())
    // - PrioCodeGenerator.PostAfhandeling.cs  PrioCPostAfhandelingPrio (prio.c PostAfhandelingPrio())
    // - PrioCodeGenerator.Ris.cs          RIS display code within PrioCInUitMelden
    // - PrioCodeGenerator.HDLegacy.cs     legacy HD ingrepen (c.PrioData.HDIngrepen), pending #76
    [CCOLCodePieceGenerator]
    public partial class PrioCodeGenerator : CCOLCodePieceGeneratorBase
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
        private CCOLGeneratorCodeStringSettingModel _prmupinagb;
        private CCOLGeneratorCodeStringSettingModel _prmupinagbhd;
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
        private CCOLGeneratorCodeStringSettingModel _prmrisimportance;
        private CCOLGeneratorCodeStringSettingModel _prmrisstationtype;
        private CCOLGeneratorCodeStringSettingModel _tris;
        private CCOLGeneratorCodeStringSettingModel _prmftsblok;
        private CCOLGeneratorCodeStringSettingModel _prmftsmaxpercyc;
        private CCOLGeneratorCodeStringSettingModel _prmftsminvtg;
        private CCOLGeneratorCodeStringSettingModel _prmftsminwt;
        private CCOLGeneratorCodeStringSettingModel _cftsvtg;
        private CCOLGeneratorCodeStringSettingModel _cftscyc;
        private CCOLGeneratorCodeStringSettingModel _prmvtgcat;
        private CCOLGeneratorCodeStringSettingModel _mftstelris;
        private CCOLGeneratorCodeStringSettingModel _prmftsminvtgris;
        private CCOLGeneratorCodeStringSettingModel _prmrisgrenspriotype;
        private CCOLGeneratorCodeStringSettingModel _schpriomr;

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
        private string _treallr;
        private string _prmrispstart;
        private string _prmrispend;
        private string _schrisgeencheckopsg;
        private string _prmrislaneheading;
        private string _prmrislaneheadingmarge;

        #endregion // Fields

        #region Properties
        #endregion // Properties

        public override bool HasCCOLElements() => true;

        public override IEnumerable<DetectorModel> GetDetectors() => _myDetectors;

        public override bool HasDetectors() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return base.GetFunctionLocalVariables(c, type);

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return new List<CCOLLocalVariable> { new("int", "ov", "0") };
            
                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    var result2 = new List<CCOLLocalVariable>();
                    if (c.HasHD() && c.PrioData.BlokkeerNietConflictenBijHDIngreep) result2.Add(new CCOLLocalVariable(c.GetBoolV(), "isHD", "FALSE"));
                    if (c.Fasen.Any(x => x.WachttijdVoorspeller)) result2.Add(new CCOLLocalVariable(c.GetBoolV(), "isWTV", "FALSE"));
                    if (c.Data.FixatieData.FixatieMogelijk || c.Fasen.Any(x => x.HardMeeverlengenFaseCycli.Count > 0)) result2.Add(new CCOLLocalVariable("int", "fc"));
                    if (result2.Count > 0)
                        return result2;
                    else
                        return base.GetFunctionLocalVariables(c, type);

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    if (c.PrioData.PrioIngrepen.Any(x => x.MeldingenData.Inmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde) ||
                                                         x.MeldingenData.Uitmeldingen.Any(x2 => x2.Type == PrioIngreepInUitMeldingVoorwaardeTypeEnum.RISVoorwaarde)))
                    {
                        return new List<CCOLLocalVariable> {new("int", "i", "0")};
                    }
                    else return base.GetFunctionLocalVariables(c, type);
                
                case CCOLCodeTypeEnum.PrioCInitPrio:
                    return new List<CCOLLocalVariable> {new("int", "i")};

                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCTop => new []{61},
                CCOLCodeTypeEnum.RegCPreApplication => new []{41},
                CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules => new []{41},
                CCOLCodeTypeEnum.RegCSystemApplication => new []{41},
                CCOLCodeTypeEnum.RegCPostSystemApplication => new []{31},
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{11},
                CCOLCodeTypeEnum.PrioCPostAfhandelingPrio => new []{11},
                CCOLCodeTypeEnum.PrioCInitPrio => new []{11},
                CCOLCodeTypeEnum.PrioCIncludes => new []{11},
                CCOLCodeTypeEnum.RegCWachtgroen => new []{50},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            if (c.PrioData.PrioIngreepType == PrioIngreepTypeEnum.Geen) return null;

            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCIncludes:
                    sb.AppendLine($"{c.GetBoolV()} vertraag_kar_uitm[prioFCMAX];");
                    return sb.ToString();
                    
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine("mulv C_counter_old[CTMAX];");
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCPreApplication:
                    return GetPreApplicationCode(c, ts);

                case CCOLCodeTypeEnum.RegCRealisatieAfhandelingNaModules:
                    return GetNaloopPrioConflictCode(c, ts);

                case CCOLCodeTypeEnum.RegCSystemApplication:
                    return GetSystemApplicationCode(c, ts);

                case CCOLCodeTypeEnum.RegCPostSystemApplication:
                    return GetPostSystemApplicationCode(c, ts);

                case CCOLCodeTypeEnum.RegCWachtgroen:
                    if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                    {
                        sb.AppendLine($"{ts}/* Op tijd beeindingen wachtgroen (bv voor TWL's of andere wachtstand richtingen  */");
                        sb.AppendLine($"#ifndef NO_PRIO");
                        sb.AppendLine($"{ts}PrioAanwezig();");
                        sb.AppendLine($"{ts}BeeindigenWachtgroenPrioConflicten();");
                        sb.AppendLine($"#endif /* NO_PRIO */");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    return GetInUitMeldenCode(c, ts);

                case CCOLCodeTypeEnum.PrioCPostAfhandelingPrio:
                    return GetPostAfhandelingPrioCode(c, ts);

                case CCOLCodeTypeEnum.PrioCInitPrio:
                    sb.AppendLine($"{ts}/* initialisatie variabelen vertraag_kar_uitm */");
                    sb.AppendLine($"{ts}/* ------------------------------------------ */");
                    sb.AppendLine($"{ts}for (i = 0; i < prioFCMAX; ++i)");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}vertraag_kar_uitm[i] = 0;");
                    sb.AppendLine($"{ts}}}");
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
            _treallr = CCOLGeneratorSettingsProvider.Default.GetElementName("treallr");
            _prmrispstart = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrispstart");
            _prmrispend = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrispend");
            _schrisgeencheckopsg = CCOLGeneratorSettingsProvider.Default.GetElementName("schrisgeencheckopsg");
            _prmrislaneheading = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrislaneheading");
            _prmrislaneheadingmarge = CCOLGeneratorSettingsProvider.Default.GetElementName("prmrislaneheadingmarge");

            return base.SetSettings(settings);
        }
    }
}
