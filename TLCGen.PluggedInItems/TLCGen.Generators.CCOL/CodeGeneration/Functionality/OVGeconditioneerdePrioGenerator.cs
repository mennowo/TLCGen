using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Generators.CCOL.CodeGeneration.Functionality
{
    [CCOLCodePieceGenerator]
    public class OVGeconditioneerdePrioGenerator : CCOLCodePieceGeneratorBase
    {
        #region Fields

        private string _hov;
        private string _hovin;
        private string _hovuit;
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
            _myBitmapOutputs = new List<CCOLIOElement>();

            if(c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
            {
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmOVtstpgrensvroeg}", c.OVData.GeconditioneerdePrioGrensTeVroeg, CCOLElementTimeTypeEnum.TS_type, _prmOVtstpgrensvroeg));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_prmOVtstpgrenslaat}", c.OVData.GeconditioneerdePrioGrensTeLaat, CCOLElementTimeTypeEnum.TS_type, _prmOVtstpgrenslaat));

            }

            foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
            {
                if(ov.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Altijd)
                {
                    _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_schovstipt}{ov.FaseCyclus}", ov.GeconditioneerdePrioriteit == NooitAltijdAanUitEnum.SchAan ? 1 : 0, CCOLElementTimeTypeEnum.SCH_type, _schovstipt, ov.FaseCyclus));
                }
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_mstp}{ov.FaseCyclus}", _mstp, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_hstp}{ov.FaseCyclus}", _hstp, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{ov.FaseCyclus}{_prmovstipttevroeg}", ov.GeconditioneerdePrioTeVroeg, CCOLElementTimeTypeEnum.None, _prmovstipttevroeg, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{ov.FaseCyclus}{_prmovstiptoptijd}", ov.GeconditioneerdePrioOpTijd, CCOLElementTimeTypeEnum.None, _prmovstiptoptijd, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{ov.FaseCyclus}{_prmovstipttelaat}", ov.GeconditioneerdePrioTeLaat, CCOLElementTimeTypeEnum.None, _prmovstipttelaat, ov.FaseCyclus));

                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtevroeg}{ov.FaseCyclus}", _usovtevroeg, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovoptijd}{ov.FaseCyclus}", _usovoptijd, ov.FaseCyclus));
                _myElements.Add(CCOLGeneratorSettingsProvider.Default.CreateElement($"{_usovtelaat}{ov.FaseCyclus}", _usovtelaat, ov.FaseCyclus));

                _myBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioTeVroegBitmapData, $"{_uspf}{_usovtevroeg}{ov.FaseCyclus}"));
                _myBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioOpTijdBitmapData, $"{_uspf}{_usovoptijd}{ov.FaseCyclus}"));
                _myBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioTeLaatBitmapData, $"{_uspf}{_usovtelaat}{ov.FaseCyclus}"));
            }
        }

        public override bool HasCCOLElements() => true;

        public override bool HasCCOLBitmapOutputs() => true;
        
        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.OvCTop:
                    return 50;
                case CCOLCodeTypeEnum.OvCInUitMelden:
                    return 20;
                case CCOLCodeTypeEnum.OvCPrioriteitsOpties:
                    return 10;
                case CCOLCodeTypeEnum.RegCPostApplication:
                    return 10;
            }
            return 0;
        }

        public override string GetCode(ControllerModel c, CCOLCodeTypeEnum type, string ts)
        {
            var sb = new StringBuilder();

            switch (type)
            {
                case CCOLCodeTypeEnum.OvCTop:
                    if (!c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"/* Variabelen tbv registreren stiptheid bij inmelding via KAR: tbv bepalen prioriteit in OV.ADD */");
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"int iKARInSTP{ov.FaseCyclus}[MAX_AANTAL_INMELDINGEN] = {{ 0 }}; int iAantInm{ov.FaseCyclus} = 0;");
                    }
                    return sb.ToString();

                case CCOLCodeTypeEnum.OvCInUitMelden:
                    if (!c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Bijhouden stiptheidsklassen ingemelde voertuigen */");
                    sb.AppendLine($"{ts}/* Bij inmelding: registeren stiptheidsklasse achterste voertuig */");
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}TrackStiptObvTSTP({_hpf}{_hovin}{ov.FaseCyclus}, {_hpf}{_hovuit}{ov.FaseCyclus}, &iAantInm{ov.FaseCyclus}, iKARInSTP{ov.FaseCyclus}, {_hpf}{_hov}{ov.FaseCyclus}, PRM[{_prmpf}{_prmOVtstpgrensvroeg}], PRM[{_prmpf}{_prmOVtstpgrenslaat}]);");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}MM[{_mpf}{_mstp}{ov.FaseCyclus}] = iAantInm{ov.FaseCyclus} > 0 ? iKARInSTP{ov.FaseCyclus}[0] : 0;");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.OvCPrioriteitsOpties:
                    if (!c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Geconditioneerde prioriteit instellen */");
                    var tsts = ts; 
                    if (c.HalfstarData.IsHalfstar)
                    {
                        tsts += ts;
                        sb.AppendLine($"{ts}/* Geconditioneerde prioriteit werkt nog niet in combinatie met prioriteit tijdens PL regelen */");
                        sb.AppendLine($"{ts}if (!IH[{_hpf}{_hplact}])");
                        sb.AppendLine($"{ts}{{");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        var hasconditions = false;
                        var sbc = new StringBuilder();
                        sbc.Append($"{tsts}IH[{_hpf}{_hstp}{ov.FaseCyclus}] = ");
                        var hd = c.OVData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == ov.FaseCyclus);
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
							sbc.Append($"SCH[{_schpf}{_schovstipt}{ov.FaseCyclus}]");
                            hasconditions = true;
                        }
                        if (hasconditions)
                        {
                            sbc.AppendLine($";");
                            sb.Append(sbc.ToString());
                        }
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_VROEG || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstipttevroeg});");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_OP_TIJD || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstiptoptijd});");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{tsts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_LAAT || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstipttelaat});");
                    }
                    if (c.HalfstarData.IsHalfstar)
                    {
                        sb.AppendLine($"{ts}}}");
                    }
                    return sb.ToString();
                case CCOLCodeTypeEnum.RegCPostApplication:
                    if (!c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Verklikken stiptheid OV */");
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovtevroeg}{ov.FaseCyclus}] = MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_VROEG;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovoptijd}{ov.FaseCyclus}] = MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_OP_TIJD;");
                        sb.AppendLine($"{ts}CIF_GUS[{_uspf}{_usovtelaat}{ov.FaseCyclus}] = MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_LAAT;");
                    }
                    return sb.ToString();

            }
            return null;
        }

        public override bool SetSettings(CCOLGeneratorClassWithSettingsModel settings)
        {
            _hov = CCOLGeneratorSettingsProvider.Default.GetElementName("hov");
            _hovin = CCOLGeneratorSettingsProvider.Default.GetElementName("hovin");
            _hovuit = CCOLGeneratorSettingsProvider.Default.GetElementName("hovuit");
            _cvc = CCOLGeneratorSettingsProvider.Default.GetElementName("cvc");
            _cvchd = CCOLGeneratorSettingsProvider.Default.GetElementName("cvchd");
            _hplact = CCOLGeneratorSettingsProvider.Default.GetElementName("hplact");

            return base.SetSettings(settings);
        }

        #endregion // CCOLCodePieceGeneratorBase Overrides
        
    }
}