using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TLCGen.Generators.CCOL.CodeGeneration.HelperClasses;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Integrity;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class NalopenCodeGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _hnla;
        private CCOLGeneratorCodeStringSettingModel _tnlfg;
        private CCOLGeneratorCodeStringSettingModel _tnlfgd;
        private CCOLGeneratorCodeStringSettingModel _tnlsg;
        private CCOLGeneratorCodeStringSettingModel _tnlsgd;
        private CCOLGeneratorCodeStringSettingModel _tnlcv;
        private CCOLGeneratorCodeStringSettingModel _tnlcvd;
        private CCOLGeneratorCodeStringSettingModel _tnleg;
        private CCOLGeneratorCodeStringSettingModel _tnlegd;
        private CCOLGeneratorCodeStringSettingModel _txnl;
        private CCOLGeneratorCodeStringSettingModel _hnlsg;
        private CCOLGeneratorCodeStringSettingModel _hnleg;
        private CCOLGeneratorCodeStringSettingModel _tvgnaloop;
#pragma warning restore 0649
	    private string _homschtegenh;
	    private string _trealil;
	    private string _treallr;
	    private string _hmad;

        #endregion // Fields

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            foreach (var nl in c.InterSignaalGroep.Nalopen)
            {
                foreach (var nlt in nl.Tijden)
                {
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
                        _ => null
                    };
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tnl}{nl.FaseVan}{nl.FaseNaar}",
                            nlt.Waarde,
                            CCOLElementTimeTypeEnum.TE_type, 
                            _tnl, nl.FaseVan, nl.FaseNaar));
                }
                if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc &&
                    nl.Type == NaloopTypeEnum.EindeGroen)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hnleg}{nl.FaseVan}{nl.FaseNaar}",
                            _hnleg, nl.FaseVan, nl.FaseNaar));
                }
                if (nl.Type == NaloopTypeEnum.StartGroen)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_hnlsg}{nl.FaseVan}{nl.FaseNaar}",
                            _hnlsg, nl.FaseVan, nl.FaseNaar));
                }
                else if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_tvgnaloop}{nl.FaseVan}{nl.FaseNaar}",
                            nl.MaxUitverlengenVolgrichting,
                            CCOLElementTimeTypeEnum.TE_type,
                            _tvgnaloop, nl.FaseVan, nl.FaseNaar));
                }
                if (nl.DetectieAfhankelijk)
                {
                    foreach (var nld in nl.Detectoren)
                    {
                        var elem = CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hnla}{nld.Detector}", _hnla, nld.Detector, nl.FaseVan, nl.FaseNaar);
                        if (_myElements.Count == 0 || _myElements.All(x => x.Naam != elem.Naam))
                        {
                            _myElements.Add(elem);
                        }
                    }
                }
                if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.RealFunc && nl.MaximaleVoorstart.HasValue)
                {
                    _myElements.Add(
                        CCOLGeneratorSettingsProvider.Default.CreateElement(
                            $"{_txnl}{nl.FaseVan}{nl.FaseNaar}",
                            nl.MaximaleVoorstart.Value,
                            CCOLElementTimeTypeEnum.TE_type, 
                            _txnl, nl.FaseVan, nl.FaseNaar));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override IEnumerable<CCOLLocalVariable> GetFunctionLocalVariables(ControllerModel c, CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    if(c.InterSignaalGroep?.Nalopen?.Count > 0)
                        return new List<CCOLLocalVariable> { new("int", "fc") };
                    return base.GetFunctionLocalVariables(c, type);
                default:
                    return base.GetFunctionLocalVariables(c, type);
            }
        }

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.RegCInitApplication => new[] { 30 },
                CCOLCodeTypeEnum.RegCPreApplication => new []{30},
                CCOLCodeTypeEnum.RegCSynchronisaties => new []{20},
                CCOLCodeTypeEnum.RegCMaxgroenNaAdd => new []{10},
                CCOLCodeTypeEnum.RegCVerlenggroenNaAdd => new []{10},
                CCOLCodeTypeEnum.RegCMaxgroen => new []{20},
                CCOLCodeTypeEnum.RegCVerlenggroen => new []{10},
                CCOLCodeTypeEnum.RegCAlternatieven => new []{20},
                CCOLCodeTypeEnum.RegCMeetkriteriumNaDetectieStoring => new []{10},
                CCOLCodeTypeEnum.PrioCPrioriteitsNiveau => new []{20},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
				case CCOLCodeTypeEnum.RegCInitApplication:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0 &&
                        c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}gk_InitGK();");
                        sb.AppendLine($"{ts}gk_InitNL();");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPreApplication:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0 && 
                        c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                    {
                        sb.AppendLine($"{ts}/* Nalopen */");
                        sb.AppendLine($"{ts}/* ------- */");
                        sb.AppendLine($"{ts}gk_ResetGK();");
                        sb.AppendLine($"{ts}gk_ResetNL();");
                    }
					return sb.ToString();
                case CCOLCodeTypeEnum.RegCSynchronisaties:
                    if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc &&
                        c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        sb.AppendLine($"{ts}TegenhoudenDoorRealisatietijden();");
                    }

                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMaxgroen:
                case CCOLCodeTypeEnum.RegCVerlenggroen:
                    if (c.InterSignaalGroep?.Nalopen?.Count > 0)
                    {
                        if (c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                        {
                            sb.AppendLine($"{ts}/* Nalopen */");
                            sb.AppendLine($"{ts}/* ------- */");
                            sb.AppendLine();
                            sb.AppendLine($"{ts}for (fc = 0; fc < FCMAX; ++fc)");
                            sb.AppendLine($"{ts}{{");
                            sb.AppendLine($"{ts}{ts}RW[fc] &= ~BIT2;");
                            sb.AppendLine($"{ts}{ts}YV[fc] &= ~BIT2;");
                            sb.AppendLine($"{ts}{ts}YM[fc] &= ~BIT2;");
                            sb.AppendLine($"{ts}}}");
                            sb.AppendLine();
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                var vn = nl.FaseVan + nl.FaseNaar;
                                switch (nl.Type)
                                {
#warning This only works for pedestrians
                                    case NaloopTypeEnum.StartGroen:
                                        if (nl.VasteNaloop)
                                        {
                                            sb.AppendLine(
                                                $"{ts}NaloopVtg({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlsg}{vn});");
                                        }

                                        if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                        {
                                            sb.AppendLine(
                                                $"{ts}NaloopVtgDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_dpf}{nl.Detectoren[0].Detector}, {_hpf}{_hnla}{nl.Detectoren[0].Detector}, {_tpf}{_tnlsgd}{vn});");
                                        }

                                        break;

                                    case NaloopTypeEnum.EindeGroen:
                                        if (nl.VasteNaloop)
                                        {
                                            sb.AppendLine(
                                                $"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfg}{vn});");
                                            sb.AppendLine(
                                                $"{ts}NaloopEG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnleg}{vn});");
                                        }

                                        if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                        {
                                            sb.Append(
                                                $"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfgd}{vn}");
                                            foreach (var d in nl.Detectoren)
                                            {
                                                sb.Append($", {_dpf}{d.Detector}");
                                            }

                                            sb.AppendLine(", END);");
                                            sb.Append(
                                                $"{ts}NaloopEGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlegd}{vn}");
                                            foreach (var d in nl.Detectoren)
                                            {
                                                sb.Append($", {_dpf}{d.Detector}");
                                            }

                                            sb.AppendLine(", END);");
                                        }

                                        break;

                                    case NaloopTypeEnum.CyclischVerlengGroen:
                                        if (nl.VasteNaloop)
                                        {
                                            sb.AppendLine(
                                                $"{ts}NaloopFG({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfg}{vn});");
                                            sb.AppendLine(
                                                $"{ts}NaloopCV({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlcv}{vn});");
                                        }

                                        if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0)
                                        {
                                            sb.Append(
                                                $"{ts}NaloopFGDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlfgd}{vn}");
                                            foreach (var d in nl.Detectoren)
                                            {
                                                sb.Append($", {_dpf}{d.Detector}");
                                            }

                                            sb.AppendLine(", END);");
                                            sb.Append(
                                                $"{ts}NaloopCVDet({_fcpf}{nl.FaseVan}, {_fcpf}{nl.FaseNaar}, {_tpf}{_tnlcvd}{vn}");
                                            foreach (var d in nl.Detectoren)
                                            {
                                                sb.Append($", {_dpf}{d.Detector}");
                                            }

                                            sb.AppendLine(", END);");
                                        }

                                        break;
                                }
                            }

                            sb.AppendLine();
                        }
                    }

                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCMeetkriteriumNaDetectieStoring:
                    if (!c.InterSignaalGroep.Nalopen.Any()) return "";

                    if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                    { 
                        sb.AppendLine($"{ts}/* Volgrichting wordt vastgehouden m.b.v. het meetkriterium tijdens verlenggroen */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                            var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);

                            if (fc1 == null || fc2 == null) continue;
                        
                            if (nl.Type == NaloopTypeEnum.StartGroen &&
                                fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger)
                            {
                                var dp = nl.DetectieAfhankelijk ? nl.Detectoren.FirstOrDefault() : null;
                                var tnlsg = nl.VasteNaloop ? $"{_tpf}{_tnlsg}{nl:vannaar}" : "NG";
                                var tnlsgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlsgd}{nl:vannaar}" : "NG";
                                sb.AppendLine($"{ts}NaloopVtg({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {(dp == null ? "NG" : $"{_dpf}{dp.Detector}")}, " +
                                              $"{(dp == null ? "NG" : $"{_hpf}{_hmad}{dp.Detector}")}, {_hpf}{_hnlsg}{nl:vannaar}, {tnlsg}, {tnlsgd});");
                            }
                            else if (nl.Type == NaloopTypeEnum.EindeGroen)
                            {
                                var tnlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                var tnlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                var tnleg = nl.VasteNaloop ? $"{_tpf}{_tnleg}{nl:vannaar}" : "NG";
                                var tnlegd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlegd}{nl:vannaar}" : "NG";
                                sb.Append($"{ts}NaloopEG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {tnlfg}, {tnlfgd}, {tnleg}, {tnlegd}, {_tpf}{_tvgnaloop}{nl:vannaar}, ");
                                if (nl.DetectieAfhankelijk)
                                {
                                    foreach (var d in nl.Detectoren)
                                    {
                                        sb.Append($"{_dpf}{d.Detector}, ");
                                    }
                                }
                                sb.AppendLine("END);");
                            }
                            else if (nl.Type == NaloopTypeEnum.CyclischVerlengGroen)
                            {
                                var tnlfg = nl.VasteNaloop ? $"{_tpf}{_tnlfg}{nl:vannaar}" : "NG";
                                var tnlfgd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlfgd}{nl:vannaar}" : "NG";
                                var tnlcv = nl.VasteNaloop ? $"{_tpf}{_tnlcv}{nl:vannaar}" : "NG";
                                var tnlcvd = nl.DetectieAfhankelijk ? $"{_tpf}{_tnlcvd}{nl:vannaar}" : "NG";
                                sb.Append($"{ts}NaloopEVG({_fcpf}{nl:van}, {_fcpf}{nl:naar}, {tnlfg}, {tnlfgd}, {tnlcv}, {tnlcvd}, {_tpf}{_tvgnaloop}{nl:vannaar}, ");
                                if (nl.DetectieAfhankelijk)
                                {
                                    foreach (var d in nl.Detectoren)
                                    {
                                        sb.Append($"{_dpf}{d.Detector}, ");
                                    }
                                }
                                sb.AppendLine("END);");
                            }
                        }
                    }

                    var first = true;
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {

                        var fc1 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                        var fc2 = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                        if (nl.Type == NaloopTypeEnum.StartGroen &&
                            fc1.Type == FaseTypeEnum.Voetganger && fc2.Type == FaseTypeEnum.Voetganger)
                        {
                            var dp = nl.DetectieAfhankelijk ? nl.Detectoren.FirstOrDefault() : null;
                            if (dp != null)
                            {
                                if (first)
                                {
                                    sb.AppendLine();
                                    first = false;
                                }
                                sb.AppendLine($"{ts}IH[{_hpf}{_hnla}{dp.Detector}] = IH[{_hpf}{_hmad}{dp.Detector}];");
                            }
                        }
                    }
                    
                    return sb.ToString();

                case CCOLCodeTypeEnum.RegCMaxgroenNaAdd:
                    if (c.InterSignaalGroep.Nalopen.Count > 0 &&
                        c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                        sb.AppendLine($"{ts}gk_ControlGK();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCVerlenggroenNaAdd:
                    if (c.InterSignaalGroep.Nalopen.Count > 0 &&
                        c.Data.SynchronisatiesType != SynchronisatiesTypeEnum.InterFunc)
                        sb.AppendLine($"{ts}gk_ControlGK();");
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.RegCAlternatieven:
                    if (c.InterSignaalGroep.Nalopen.Count > 0)
                    {
                        sb.AppendLine($"{ts}/* set meerealisatie voor richtingen met nalopen */");
                        sb.AppendLine($"{ts}/* --------------------------------------------- */");
                        foreach (var nl in c.InterSignaalGroep.Nalopen)
                        {
                            var sgv = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseVan);
                            var sgn = c.Fasen.FirstOrDefault(x => x.Naam == nl.FaseNaar);
                            var tinl = c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.RealFunc ? _trealil : _txnl.ToString();
                            if (nl.DetectieAfhankelijk && nl.Detectoren?.Count > 0 && 
                                sgv is { Type: FaseTypeEnum.Voetganger } && sgn is { Type: FaseTypeEnum.Voetganger })
                            {
                                if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                                {
                                    var d = nl.Detectoren.First();
                                    sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl:naar}, {_fcpf}{nl:van}, ({c.GetBoolV()})" +
                                              $"(SG[{_fcpf}{nl:van}] && A[{_fcpf}{nl:naar}] && IH[{_hpf}{_hnla}{d.Detector}] && IH[{_hpf}{_hnlsg}{nl:vannaar}]));");
                                }
                                else if (nl.MaximaleVoorstart.HasValue)
                                {
                                    sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) " +
                                              $"((T[{_tpf}{tinl}{nl.FaseVan}{nl.FaseNaar}] || RT[{_tpf}{tinl}{nl.FaseVan}{nl.FaseNaar}]) && A[{_fcpf}{nl.FaseNaar}] && !G[{_fcpf}{nl.FaseNaar}] && !kcv({_fcpf}{nl.FaseNaar})));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}set_MRLW({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) " +
                                                  $"(SG[{_fcpf}{nl.FaseVan}] && A[{_fcpf}{nl.FaseNaar}] && !kcv({_fcpf}{nl.FaseNaar})));");
                                }
                            }
                            else
                            {
                                if (c.Data.SynchronisatiesType == SynchronisatiesTypeEnum.InterFunc)
                                {
                                    sb.AppendLine($"{ts}set_MRLW_nl({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()})(G[{_fcpf}{nl.FaseVan}] && !G[{_fcpf}{nl.FaseNaar}] && A[{_fcpf}{nl.FaseNaar}] && IH[{_hpf}{_hnleg}{nl:vannaar}]));");
                                }
                                else if (nl.MaximaleVoorstart.HasValue)
                                {
                                    var tt = sgv is { Type: FaseTypeEnum.Voetganger } && sgn is { Type: FaseTypeEnum.Voetganger }
                                        ? tinl
                                        : _treallr;
                                    sb.AppendLine($"{ts}set_MRLW_nl({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) " +
                                                  $"((T[{_tpf}{tt}{nl.FaseNaar}{nl.FaseVan}] || RT[{_tpf}{tt}{nl.FaseNaar}{nl.FaseVan}]) && A[{_fcpf}{nl.FaseNaar}] && !G[{_fcpf}{nl.FaseNaar}]));");
                                }
                                else
                                {
                                    sb.AppendLine($"{ts}set_MRLW_nl({_fcpf}{nl.FaseNaar}, {_fcpf}{nl.FaseVan}, ({c.GetBoolV()}) (G[{_fcpf}{nl.FaseVan}] && !G[{_fcpf}{nl.FaseNaar}] && A[{_fcpf}{nl.FaseNaar}]));");
                                }
                            }
                        }
                    }
                    return sb.ToString();
                
                case CCOLCodeTypeEnum.PrioCPrioriteitsNiveau:
                    //if(!c.InterSignaalGroep.Nalopen.Any()) return "";
                    //sb.AppendLine($"{ts}/* Tegenhouden OV prio met conflict met nalooprichting indien die nog moet komen */");
                    //foreach (var nl in c.InterSignaalGroep.Nalopen)
                    //{
                    //    foreach (var ov in c.OVData.OVIngrepen.Where(x => TLCGenControllerChecker.IsFasenConflicting(c, nl.FaseNaar, x.FaseCyclus)))
                    //    {
                    //        sb.AppendLine($"{ts}iXPrio[ovFC{ov.FaseCyclus}] |= G[{_fcpf}{nl.FaseVan}] && CV[{_fcpf}{nl.FaseVan}] && !G[{_fcpf}{nl.FaseNaar}] &&;");
                    //    }
                    //}
                    //sb.AppendLine();
                    return sb.ToString();

                default:
                    return null;
            }
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings) 
        { 
            _homschtegenh = CCOLGeneratorSettingsProvider.Default.GetElementName("homschtegenh");
            _treallr = CCOLGeneratorSettingsProvider.Default.GetElementName("treallr");
            _trealil = CCOLGeneratorSettingsProvider.Default.GetElementName("trealil");
            _hmad = CCOLGeneratorSettingsProvider.Default.GetElementName("hmad");
            return base.SetSettings(settings);
	    }
    }
}