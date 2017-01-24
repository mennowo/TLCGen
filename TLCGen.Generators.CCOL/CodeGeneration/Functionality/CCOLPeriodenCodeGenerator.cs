using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Generators.CCOL.Extensions;
using TLCGen.Models;

namespace TLCGen.Generators.CCOL.CodeGeneration
{
    public class CCOLPeriodenGenerator : ICCOLCodePieceGenerator
    {
        private List<CCOLElement> _MyElements;
        private List<CCOLIOElement> _MyBitmapOutputs;

        public void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();
            _MyBitmapOutputs = new List<CCOLIOElement>();

            // outputs
            _MyElements.Add(new CCOLElement(c.PeriodenData.DefaultPeriodeBitmapData.GetBitmapCoordinaatOutputDefine("perdef"), "perdef", CCOLElementTypeEnum.Uitgang));
            _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData as IOElementModel, "usperdef"));
            foreach (var item in c.PeriodenData.Perioden)
            {
                _MyElements.Add(new CCOLElement(item.BitmapData.GetBitmapCoordinaatOutputDefine("per" + item.Naam), "per" + item.Naam, CCOLElementTypeEnum.Uitgang));
                _MyBitmapOutputs.Add(new CCOLIOElement(item.BitmapData as IOElementModel, "usper" + item.Naam));
            }

            // parameters
            int i = 1;
            foreach (var per in c.PeriodenData.Perioden)
            {
                var hours = per.StartTijd.Hours;
                if (per.StartTijd.Days == 1)
                {
                    hours = 24;
                }
                var inst = hours * 100 + per.StartTijd.Minutes;
                _MyElements.Add(new CCOLElement($"prmstkp{i}", $"stkp{i}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _MyElements.Add(new CCOLElement($"prmetkp{i}", $"etkp{i}", inst, CCOLElementTimeTypeEnum.TI_type, CCOLElementTypeEnum.Parameter));
                _MyElements.Add(new CCOLElement($"prmdckp{i}", $"dckp{i}", (int)per.DagCode, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                ++i;
            }
        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapInputs()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CCOLIOElement> GetCCOLBitmapOutputs()
        {
            return _MyBitmapOutputs;
        }

        public IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();
            int i;

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.KlokPerioden:

                    sb.AppendLine("void KlokPerioden(void)");
                    sb.AppendLine("{");
                    sb.AppendLine($"{tabspace}/* default klokperiode voor max.groen */");
                    sb.AppendLine($"{tabspace}/* ---------------------------------- */");
                    sb.AppendLine($"{tabspace}MM[mperiod] = 0;");
                    sb.AppendLine();
                    i = 1;
                    foreach (PeriodeModel kpm in c.PeriodenData.Perioden)
                    {
                        string comm = kpm.Commentaar;
                        if (comm == null) comm = "";
                        sb.AppendLine($"{tabspace}/* klokperiode: {comm} */");
                        sb.AppendLine($"{tabspace}/* -------------{new string('-', comm.Length)} */");
                        sb.AppendLine($"{tabspace}if (klokperiode(PRM[prmstkp{i}], PRM[prmetkp{i}]) &&");
                        sb.AppendLine($"{tabspace}    dagsoort(PRM[prmdckp{i}]))");
                        sb.AppendLine($"{tabspace}{tabspace}MM[mperiod] = {i};");
                        sb.AppendLine();
                        ++i;
                    }
                    sb.AppendLine($"{tabspace}KlokPerioden_Add();");
                    sb.AppendLine("}");
                    return sb.ToString();
                case CCOLRegCCodeTypeEnum.SystemApplication:
                    sb.AppendLine("/* periode verklikking */");
                    sb.AppendLine("/* ------------------- */");
                    i = 0;
                    sb.AppendLine($"{tabspace}CIF_GUS[usperdef] = (MM[mperiod] == {i++});");
                    foreach (var per in c.PeriodenData.Perioden)
                    {
                        sb.AppendLine($"{tabspace}CIF_GUS[{per.BitmapData.GetBitmapCoordinaatOutputDefine("per" + per.Naam)}] = (MM[mperiod] == {i++});");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }
    }
}
