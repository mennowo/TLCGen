using DocumentFormat.OpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Specificator
{
    public class TableGenerator
    {
        private static ResourceDictionary _texts;
        public static ResourceDictionary Texts
        {
            get
            {
                if (_texts == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    using (var stream = assembly.GetManifestResourceStream("TLCGen.Specificator.Resources.SpecificatorTexts.xaml"))
                    {
                        _texts = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream);
                    }
                }
                return _texts;
            }
        }

        public static List<OpenXmlCompositeElement> GetTable_FasenTijden(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Tijden"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Vastgroen",
                    "Garantiegroen",
                    "Minimum garantiegroen",
                    "Garantierood",
                    "Minimum garantierood",
                    "Geel",
                    "Minimum geel",
                    "Kopmax"
                }
            };
            // TODO: veiligheidsgroen
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.TFG.ToString(),
                    fc.TGG.ToString(),
                    fc.TGG_min.ToString(),
                    fc.TRG.ToString(),
                    fc.TRG_min.ToString(),
                    fc.TGL.ToString(),
                    fc.TGL_min.ToString(),
                    fc.Detectoren.Any(x => (x.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax)) ? fc.Kopmax.ToString() : "-" });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_FasenFuncties(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Functies"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Type",
                    "Rijstroken",
                    "Vaste aanvraag",
                    "Wachtgroen",
                    "Meeverlengen",
                    "Wachttijd voorspellers",
                    "Aantal detectoren"
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.Type.GetDescription(),
                    fc.AantalRijstroken.ToString(),
                    fc.VasteAanvraag.GetDescription(),
                    fc.Wachtgroen.GetDescription(),
                    fc.Meeverlengen.GetDescription(),
                    fc.WachttijdVoorspeller == true ? "X" : "-",
                    fc.Detectoren.Count.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetTable_Modulen(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Modulen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Modulen"], (string)Texts["Generic_Fasen"] }
            };
            c.ModuleMolen.Modules.ForEach(m => l.Add(new List<string> { m.Naam, m.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) }));
            items.Add(OpenXmlHelper.GetTable(l));

            return items;
        }
    }
}
