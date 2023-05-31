using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class IsgFuncCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        //private CCOLGeneratorCodeStringSettingModel _mtest;
#pragma warning restore 0649
        private string _prmaltg;
        private string _tnlfg;
        private string _tnlfgd;
        private string _tnleg;
        private string _tnlegd;
        private string _tnlsg;
        private string _tnlsgd;
        private string _tfo; // TODO this should maybe not be shared with REALfunc
        private string _trealvs; // TODO this should maybe not be shared with REALfunc
        private string _treallr; // TODO this should maybe not be shared with REALfunc
        private string _hfile;
        private string _prmfperc;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    return new List<CCOLLocalVariable>
                    {
                        new("int", "fc"),
                    };
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    return new List<CCOLLocalVariable>
                    {
                        new("count", "i"),
                        new("count", "j"),
                        new(c.GetBoolV(), "wijziging", "TRUE"),
                    };
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCIncludes => new[] { 140 },
                CCOLCodeTypeEnum.RegCTop=> new[] { 140 },
                CCOLCodeTypeEnum.RegCVerlenggroen => new[] { 90, 100 },
                CCOLCodeTypeEnum.RegCMaxgroen => new[] { 90 },
                CCOLCodeTypeEnum.RegCInitApplication => new[] { 140 },
                CCOLCodeTypeEnum.RegCBepaalRealisatieTijden => new[] { 10 },
                CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijden => new[] { 10 },
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.RegCIncludes:
                    sb.AppendLine("#include \"isgfunc.c\" /* Interstartgroenfuncties */");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCTop:
                    sb.AppendLine($"{c.GetBoolV()} init_tvg;");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                case CCOLCodeTypeEnum.RegCMaxgroen:
                    if (order == 90)
                    {
                        sb.AppendLine($"{ts}if (EVG[fc] && PR[fc] || init_tvg)");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}TVG_PR[fc] = TVG_max[fc];");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine($"{ts}else");
                        sb.AppendLine($"{ts}{{");
                        sb.AppendLine($"{ts}{ts}TVG_max[fc] = TVG_PR[fc];");
                        sb.AppendLine($"{ts}}}");
                        sb.AppendLine($"{ts}init_tvg = TRUE;");
                        sb.AppendLine($"{ts}/* Bepaal de minimale maximale verlengroentijd bij alternatieve realisaties */");
                        foreach (var fc in c.Fasen)
                        {
                            sb.AppendLine($"{ts}TVG_AR[{_fcpf}{fc.Naam}] = ((PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}]) >= 0) ? PRM[{_prmpf}{_prmaltg}{fc.Naam}] - TFG_max[{_fcpf}{fc.Naam}] : NG;");
                        }
                    }

                    if (order == 100)
                    {
                        sb.AppendLine($"/* TVG_max nalooprichting ophogen als naloop niet past */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            switch (nl.Type)
                            {
                                case NaloopTypeEnum.StartGroen:
                                    var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                    var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}NaloopVtg_TVG_Correctie({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlsg}, {nlsgd});");
                                    break;
                                case NaloopTypeEnum.EindeGroen:
                                    var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                    var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                    var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                    var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                    sb.AppendLine($"{ts}NaloopEG_TVG_Correctie({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                                    break;
                                case NaloopTypeEnum.CyclischVerlengGroen:
                                    break;
                            }
                        }

                        sb.AppendLine($"{ts}BepaalRealisatieTijden();");
                        sb.AppendLine($"{ts}BepaalInterStartGroenTijden();");
                        sb.AppendLine($"{ts}Bepaal_Realisatietijd_per_richting();");
                    }

                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCInitApplication:
                    sb.AppendLine($"{ts}init_tvg = FALSE;");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalRealisatieTijden:
                    sb.AppendLine($"{ts}BepaalIntergroenTijden();");
                    sb.AppendLine();

                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                        var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}corrigeerTIGRvoorNalopen({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                    }
                    sb.AppendLine();

                    sb.AppendLine($"{ts}BepaalIntersignaalgroepTijden();");
                    sb.AppendLine($"{ts}RealisatieTijden_VulHaldeConflictenIn();");
                    sb.AppendLine($"{ts}RealisatieTijden_VulGroenGroenConflictenIn();");
                    sb.AppendLine($"{ts}CorrigeerIntersignaalgroepTijdObvGarantieTijden();");
                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v. nalopen */");
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.EindeGroen))
                    {
                        var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                        var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                        var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                        var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}Realisatietijd_NLEG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                    }
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => x.Type == NaloopTypeEnum.StartGroen))
                    {
                        var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                        var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                        sb.AppendLine($"{ts}Realisatietijd_NLSG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlsg}, {nlsgd});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v ontruimende deelconflicten */");
                    foreach (var vs in c.InterSignaalGroep.Gelijkstarten.Where(x => x.DeelConflict))
                    {
                        sb.AppendLine($"{ts}Ontruiming_Deelconflict_Gelijkstart({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_tfo}{vs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}Ontruiming_Deelconflict_Voorstart({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_tfo}{vs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}Ontruiming_Deelconflict_LateRelease({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_tfo}{vs:vannaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}/* Pas realisatietijden aan a.g.v. deelconflicten/voorstarts die nog groen moeten worden */");
                    sb.AppendLine($"{ts}do");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Gelijkstart / voorstart / late release */");
                    foreach (var vs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        sb.AppendLine($"{ts}wijziging |= Correctie_REALTIJD_Gelijkstart({_fcpf}{vs:naar}, {_fcpf}{vs:van});");
                    }
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}wijziging |= Correctie_REALTIJD_Voorstart({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_trealvs}{vs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}wijziging |= Correctie_REALTIJD_LateRelease({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_treallr}{vs:vannaar});");
                    }
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}/* Inlopen voetgangers */");
                    foreach (var nl in c.InterSignaalGroep.Nalopen.Where(x => 
                        c.Fasen.Any(x2 => x2.Naam == x.FaseVan && x2.Type == FaseTypeEnum.Voetganger) &&
                        c.Fasen.Any(x2 => x2.Naam == x.FaseNaar && x2.Type == FaseTypeEnum.Voetganger) &&
                        (x.MaximaleVoorstart.HasValue || x.InrijdenTijdensGroen)))
                    {
                        sb.AppendLine($"{ts}wijziging |= Correctie_REALTIJD_LateRelease({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {_tpf}{_treallr}{nl:vannaar});");
                    }
                    
                    sb.AppendLine();
                    sb.AppendLine($"{ts}{ts}wijziging |= CorrectieRealisatieTijd_Add();");
                    sb.AppendLine($"{ts}}} while (wijziging);");
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCBepaalInterStartGroenTijden:

                    sb.AppendLine($"{ts}InterStartGroenTijden_VulHaldeConflictenIn();");
                    sb.AppendLine($"{ts}InterStartGroenTijden_VulGroenGroenConflictenIn();");

                    sb.AppendLine();

                    sb.AppendLine($"{ts}/* Pas interstartgroentijden aan a.g.v. nalopen */");

                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        switch (nl.Type)
                        {
                            case NaloopTypeEnum.StartGroen:
                                var nlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                var nlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}InterStartGroenTijd_NLSG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlsg}, {nlsgd});");
                                break;
                            case NaloopTypeEnum.EindeGroen:
                                var nlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                var nlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                var nleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                var nlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}InterStartGroenTijd_NLEG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {nlfg}, {nlfgd}, {nleg}, {nlegd}, {_tpf}vgnaloop{nl:vannaar});");
                                break;
                            case NaloopTypeEnum.CyclischVerlengGroen:
                                break;
                        }
                    }

                    sb.AppendLine();

                    sb.AppendLine($"{ts}do");
                    sb.AppendLine($"{ts}{{");
                    sb.AppendLine($"{ts}{ts}wijziging = FALSE;");
                    foreach (var vs in c.InterSignaalGroep.Voorstarten)
                    {
                        sb.AppendLine($"{ts}{ts}wijziging |= Correctie_TISG_Voorstart({_fcpf}{vs:naar}, {_fcpf}{vs:van}, {_tpf}{_trealvs}{vs:naarvan});");
                    }
                    foreach (var vs in c.InterSignaalGroep.LateReleases)
                    {
                        sb.AppendLine($"{ts}{ts}wijziging |= Correctie_TISG_LateRelease({_fcpf}{vs:van}, {_fcpf}{vs:naar}, {_tpf}{_trealvs}{vs:vannaar});");
                    }
                    sb.AppendLine($"{ts}{ts}wijziging |= Correctie_InterStartGroentijdTijd_Add();");
                    sb.AppendLine($"{ts}}} while (wijziging);");

                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _prmaltg = CCOLGeneratorSettingsProvider.Default.GetElementName("prmaltg");
            _hfile = CCOLGeneratorSettingsProvider.Default.GetElementName("hfile");
            _prmfperc = CCOLGeneratorSettingsProvider.Default.GetElementName("prmfperc");
            _tnlfg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfg");
            _tnlfgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlfgd");
            _tnleg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnleg");
            _tnlegd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlegd");
            _tnlsg = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsg");
            _tnlsgd = CCOLGeneratorSettingsProvider.Default.GetElementName("tnlsgd");
            _tfo = CCOLGeneratorSettingsProvider.Default.GetElementName("tfo");
            _treallr = CCOLGeneratorSettingsProvider.Default.GetElementName("treallr");
            _trealvs = CCOLGeneratorSettingsProvider.Default.GetElementName("trealvs");

            return base.SetSettings(settings);
        }
    }

}
