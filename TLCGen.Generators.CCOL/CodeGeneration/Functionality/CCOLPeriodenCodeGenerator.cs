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
            _MyElements.Add(new CCOLElement() { Define = c.PeriodenData.DefaultPeriodeBitmapData.GetBitmapCoordinaatOutputDefine("perdef"), Naam = "perdef", Type = CCOLElementType.Uitgang });
            _MyBitmapOutputs.Add(new CCOLIOElement(c.PeriodenData.DefaultPeriodeBitmapData as IOElementModel, "usperdef"));
            foreach (var item in c.PeriodenData.Perioden)
            {
                _MyElements.Add(new CCOLElement() { Define = item.BitmapData.GetBitmapCoordinaatOutputDefine("per" + item.Naam), Naam = "per" + item.Naam, Type = CCOLElementType.Uitgang });
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
                _MyElements.Add(new CCOLElement() { Define = $"prmstkp{i}", Naam = $"stkp{i}", Instelling = inst.ToString(), TType = "TI_type", Type = CCOLElementType.Parameter });
                hours = per.EindTijd.Hours;
                if (per.EindTijd.Days == 1)
                {
                    hours = 24;
                }
                inst = hours * 100 + per.EindTijd.Minutes;
                _MyElements.Add(new CCOLElement() { Define = $"prmetkp{i}", Naam = $"etkp{i}", Instelling = inst.ToString(), TType = "TI_type", Type = CCOLElementType.Parameter });
                _MyElements.Add(new CCOLElement() { Define = $"prmdckp{i}", Naam = $"dckp{i}", Instelling = ((int)per.DagCode).ToString(), Type = CCOLElementType.Parameter });
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

        public IEnumerable<CCOLElement> GetCCOLElements(CCOLElementType type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public string GetCode(ControllerModel c, CCOLCodeType type, string tabspace)
        {
            StringBuilder sb = new StringBuilder();
            int i;

            switch (type)
            {
                case CCOLCodeType.KlokPerioden:

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
                case CCOLCodeType.SystemApplication:
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
