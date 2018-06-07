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

        private List<CCOLElement> _myElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        private string _hov;
        private string _hovin;
        private string _hovuit;
        private string _cvc;
        private string _cvchd;

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
            _MyBitmapOutputs = new List<CCOLIOElement>();

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

                _MyBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioTeVroegBitmapData, $"{_uspf}{_usovtevroeg}{ov.FaseCyclus}"));
                _MyBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioOpTijdBitmapData, $"{_uspf}{_usovoptijd}{ov.FaseCyclus}"));
                _MyBitmapOutputs.Add(new CCOLIOElement(ov.GeconditioneerdePrioTeLaatBitmapData, $"{_uspf}{_usovtelaat}{ov.FaseCyclus}"));
            }
        }

        public override bool HasCCOLElements()
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
            return _MyBitmapOutputs;
        }

        public override int HasCode(CCOLCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLCodeTypeEnum.OvCTop:
                    return 50;
                case CCOLCodeTypeEnum.OvCInUitMelden:
                    return 20;
                case CCOLCodeTypeEnum.OvCPrioriteitsOpties:
                    return 20;
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
                    sb.AppendLine();
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
                    sb.AppendLine();
                    return sb.ToString();
                case CCOLCodeTypeEnum.OvCPrioriteitsOpties:
                    if (!c.OVData.OVIngrepen.Any(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit)) return "";
                    sb.AppendLine($"{ts}/* Geconditioneerde prioriteit instellen */");
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{ts}IH[{_hpf}{_hstp}{ov.FaseCyclus}] = ");
                        var hd = c.OVData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == ov.FaseCyclus);
                        if (hd != null)
                        {
                            sb.Append($"!C[{_ctpf}{_cvchd}{hd.FaseCyclus}]");
                            foreach (var mfc in hd.MeerealiserendeFaseCycli)
                            {
                                sb.Append($" && !C[{_ctpf}{_cvchd}{mfc.FaseCyclus}]");
                            }
                        }
                        if(ov.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Altijd) sb.Append($" && SCH[{_schpf}{_schovstipt}{ov.FaseCyclus}]");
                        sb.AppendLine($";");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{ts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_VROEG || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstipttevroeg});");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{ts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_OP_TIJD || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstiptoptijd});");
                    }
                    foreach (var ov in c.OVData.OVIngrepen.Where(x => x.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit))
                    {
                        sb.Append($"{ts}if (IH[{_hpf}{_hstp}{ov.FaseCyclus}] && (MM[{_mpf}{_mstp}{ov.FaseCyclus}] == CIF_TE_LAAT || !MM[{_mpf}{_mstp}{ov.FaseCyclus}])) ");
                        sb.AppendLine($"iPrioriteitsOpties[ovFC{ov.FaseCyclus}] = BepaalPrioriteitsOpties({_prmpf}{ov.FaseCyclus}{_prmovstipttelaat});");
                    }
                    sb.AppendLine();
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
                    sb.AppendLine();
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

            return base.SetSettings(settings);
        }

        #endregion // CCOLCodePieceGeneratorBase Overrides
        
    }
}