﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class OVGeconditioneerdePrioGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private string _hprio;
        private string _hprioin;
        private string _hpriouit;
        private string _cvc;
        private string _cvchd;
        private string _hplact;

#pragma warning disable 0649
        private CCOLGeneratorCodeStringSettingModel _mstp;
        private CCOLGeneratorCodeStringSettingModel _hstp;
        private CCOLGeneratorCodeStringSettingModel _schovstipt;
        private CCOLGeneratorCodeStringSettingModel _prmovstipttevroeg;
        private CCOLGeneratorCodeStringSettingModel _usovtevroeg;
        private CCOLGeneratorCodeStringSettingModel _usovoptijd;
        private CCOLGeneratorCodeStringSettingModel _usovtelaat;
        private CCOLGeneratorCodeStringSettingModel _prmovstiptoptijd;
        private CCOLGeneratorCodeStringSettingModel _prmovstipttelaat;
        private CCOLGeneratorCodeStringSettingModel _prmOVtstpgrenslaat;
        private CCOLGeneratorCodeStringSettingModel _prmOVtstpgrensvroeg;
#pragma warning restore 0649

        #endregion

        #region CCOLCodePieceGeneratorBase Overrides

        public override void CollectCCOLElements(ControllerModel c)
        {
            _myElements = new List<CCOLElement>();

            if(c.PrioData.PrioIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmOVtstpgrensvroeg}", c.PrioData.GeconditioneerdePrioGrensTeVroeg, CCOLElementTimeTypeEnum.TS_type, _prmOVtstpgrensvroeg));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmOVtstpgrenslaat}", c.PrioData.GeconditioneerdePrioGrensTeLaat, CCOLElementTimeTypeEnum.TS_type, _prmOVtstpgrenslaat));

            }

            foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
            {
                if(ov.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schovstipt}{CCOLCodeHelper.GetPriorityName(c, ov)}", ov.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schovstipt, ov.FaseCyclus, ov.Type.GetDescription()));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}", _mstp, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hstp}{CCOLCodeHelper.GetPriorityName(c, ov)}", _hstp, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmovstipttevroeg}{CCOLCodeHelper.GetPriorityName(c, ov)}", ov.GeconditioneerdePrioTeVroeg, CCOLElementTimeTypeEnum.None, _prmovstipttevroeg, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmovstiptoptijd}{CCOLCodeHelper.GetPriorityName(c, ov)}", ov.GeconditioneerdePrioOpTijd, CCOLElementTimeTypeEnum.None, _prmovstiptoptijd, ov.FaseCyclus, ov.Type.GetDescription()));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmovstipttelaat}{CCOLCodeHelper.GetPriorityName(c, ov)}", ov.GeconditioneerdePrioTeLaat, CCOLElementTimeTypeEnum.None, _prmovstipttelaat, ov.FaseCyclus, ov.Type.GetDescription()));
                if (!c.PrioData.PrioUitgangPerFase)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtevroeg}{CCOLCodeHelper.GetPriorityName(c, ov)}", _usovtevroeg, ov.GeconditioneerdePrioTeVroegBitmapData, ov.FaseCyclus, ov.Type.GetDescription()));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovoptijd}{CCOLCodeHelper.GetPriorityName(c, ov)}", _usovoptijd, ov.GeconditioneerdePrioOpTijdBitmapData, ov.FaseCyclus, ov.Type.GetDescription()));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtelaat}{CCOLCodeHelper.GetPriorityName(c, ov)}", _usovtelaat, ov.GeconditioneerdePrioTeLaatBitmapData, ov.FaseCyclus, ov.Type.GetDescription()));
                }
            }
            if (c.PrioData.PrioUitgangPerFase)
            {
                foreach (var sg in c.Fasen.Where(x => c.PrioData.PrioIngrepen.Any(x2 =>
                    x2.FaseCyclus == x.Naam && x2.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)))
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtevroeg}{sg.Naam}", _usovtevroeg, sg.GeconditioneerdePrioTeVroegBitmapData, sg.Naam, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovoptijd}{sg.Naam}", _usovoptijd, sg.GeconditioneerdePrioOpTijdBitmapData, sg.Naam, ""));
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtelaat}{sg.Naam}", _usovtelaat, sg.GeconditioneerdePrioTeLaatBitmapData, sg.Naam, ""));
                }
            }
        }

        public override bool HasCCOLElements() => true;

        public override int[] HasCode(CCOLCodeTypeEnum type)
        {
            return type switch
            {
                CCOLCodeTypeEnum.PrioCTop => new []{50},
                CCOLCodeTypeEnum.PrioCInUitMelden => new []{20},
                CCOLCodeTypeEnum.PrioCPrioriteitsOpties => new []{10},
                CCOLCodeTypeEnum.RegCPostApplication => new []{10},
                _ => null
            };
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts, int order)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.PrioCTop:
                    if (c.PrioData.PrioIngrepen.All(x => x.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"/* Variabelen tbv registreren stiptheid bij inmelding via KAR: tbv bepalen prioriteit in OV.ADD */");
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"int iKARInSTP{CCOLCodeHelper.GetPriorityName(c, ov)}[MAX_AANTAL_INMELDINGEN] = {{ 0 }}; int iAantInm{CCOLCodeHelper.GetPriorityName(c, ov)} = 0;");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.PrioCInUitMelden:
                    if (c.PrioData.PrioIngrepen.All(x => x.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Bijhouden stiptheidsklassen ingemelde voertuigen */");
                    sb.AppendLine($"{ts}/* Bij inmelding: registeren stiptheidsklasse achterste voertuig */");
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}TrackStiptObvTSTP({_hpf}{_hprioin}{CCOLCodeHelper.GetPriorityName(c, ov)}, {_hpf}{_hpriouit}{CCOLCodeHelper.GetPriorityName(c, ov)}, &iAantInm{CCOLCodeHelper.GetPriorityName(c, ov)}, iKARInSTP{CCOLCodeHelper.GetPriorityName(c, ov)}, {_hpf}{_hprio}{CCOLCodeHelper.GetPriorityName(c, ov)}, PRM[{_prmpf}{_prmOVtstpgrensvroeg}], PRM[{_prmpf}{_prmOVtstpgrenslaat}]);");
                    }
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] = (mulv) (iAantInm{CCOLCodeHelper.GetPriorityName(c, ov)} > 0 ? iKARInSTP{CCOLCodeHelper.GetPriorityName(c, ov)}[0] : 0);");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.PrioCPrioriteitsOpties:
                    if (c.PrioData.PrioIngrepen.All(x => x.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Geconditioneerde prioriteit instellen */");
                    var tsts = ts; 
                    if (c.HalfstarData.IsHalfstar)
                    {
                        tsts += ts;
                        sb.AppendLine($"{ts}/* Geconditioneerde prioriteit werkt nog niet in combinatie met prioriteit tijdens PL regelen */");
                        sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                        sb.AppendLine($"{ts}{{");
                    }
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        var hasconditions = false;
                        var sbc = new StringBuilder();
                        sbc.Append($"{tsts}IH[{_hpf}{_hstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] = ");
                        var hd = c.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == ov.FaseCyclus);
                        if (hd != null)
                        {
                            sbc.Append($"!C[{_ctpf}{_cvchd}{hd.FaseCyclus}]");
                            foreach (var mfc in hd.MeerealiserendeFaseCycli)
                            {
                                sbc.Append($" && !C[{_ctpf}{_cvchd}{mfc.FaseCyclus}]");
                            }
                            hasconditions = true;
                        }
						if (ov.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Altijd)
						{
							if (hd != null) sbc.Append(" && ");
							sbc.Append($"SCH[{_schpf}{_schovstipt}{CCOLCodeHelper.GetPriorityName(c, ov)}]");
                            hasconditions = true;
                        }
                        if (hasconditions)
                        {
                            sbc.AppendLine(";");
                            sb.Append(sbc);
                        }
                    }
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] && (MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_VROEG || !MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}] = BepaalPrioriteitsOpties({_prmpf}{_prmovstipttevroeg}{CCOLCodeHelper.GetPriorityName(c, ov)});");
                    }
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] && (MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_OP_TIJD || !MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}] = BepaalPrioriteitsOpties({_prmpf}{_prmovstiptoptijd}{CCOLCodeHelper.GetPriorityName(c, ov)});");
                    }
                    foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] && (MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_LAAT || !MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[prioFC{CCOLCodeHelper.GetPriorityName(c, ov)}] = BepaalPrioriteitsOpties({_prmpf}{_prmovstipttelaat}{CCOLCodeHelper.GetPriorityName(c, ov)});");
                    }
                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}}}");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (c.PrioData.PrioIngrepen.All(x => x.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Verklikken stiptheid OV */");
                    if (!c.PrioData.PrioUitgangPerFase)
                    {
                        foreach (var ov in c.PrioData.PrioIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                        {
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovtevroeg}{CCOLCodeHelper.GetPriorityName(c, ov)}] = MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_VROEG;");
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovoptijd}{CCOLCodeHelper.GetPriorityName(c, ov)}] = MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_OP_TIJD;");
                            sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovtelaat}{CCOLCodeHelper.GetPriorityName(c, ov)}] = MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_LAAT;");
                        }
                    }
                    else
                    {
                        foreach (var sg in c.Fasen.Where(x => c.PrioData.PrioIngrepen.Any(x2 =>
                            x2.FaseCyclus == x.Naam && x2.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)))
                        {
                            var ingSg = c.PrioData.PrioIngrepen.Where(x =>
                                    x.FaseCyclus == sg.Naam &&
                                    x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)
                                .ToList();
                            sb.Append($"{ts}CIF_GUS[{_uspf}{_usovtevroeg}{sg.Naam}] = ");
                            var firstIng = true;
                            foreach (var ov in ingSg)
                            {
                                if (!firstIng) sb.Append(" || ");
                                sb.Append($"MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_VROEG");
                                firstIng = false;
                            }
                            sb.AppendLine(";");

                            firstIng = true;
                            sb.Append($"{ts}CIF_GUS[{_uspf}{_usovoptijd}{sg.Naam}] = ");
                            foreach (var ov in ingSg)
                            {
                                if (!firstIng) sb.Append(" || ");
                                sb.Append($"MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_OP_TIJD");
                                firstIng = false;
                            }
                            sb.AppendLine(";");

                            
                            firstIng = true;
                            sb.Append($"{ts}CIF_GUS[{_uspf}{_usovtelaat}{sg.Naam}] = ");
                            foreach (var ov in ingSg)
                            {
                                if (!firstIng) sb.Append(" || ");
                                sb.Append($"MM[{_mpf}{_mstp}{CCOLCodeHelper.GetPriorityName(c, ov)}] == CIF_TE_LAAT");
                                firstIng = false;
                            }
                            sb.AppendLine(";");
                        }
                    }
                    return sb.ToString();

            }
            return null;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hprio = CCOLGeneratorSettingsProvider.Default.GetElementName("hprio");
            _hprioin = CCOLGeneratorSettingsProvider.Default.GetElementName("hprioin");
            _hpriouit = CCOLGeneratorSettingsProvider.Default.GetElementName("hpriouit");
            _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }

        #endregion // CCOLCodePieceGeneratorBase Overrides
        
    }
}