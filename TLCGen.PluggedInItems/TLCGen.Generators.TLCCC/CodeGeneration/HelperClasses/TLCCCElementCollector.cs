using TLCGen.Models;

namespace TLCGen.Generators.TLCCC.CodeGeneration.HelperClasses
{
    public static class TLCCCElementCollector
    {
        public static void AddAllMaxElements(TLCCCElemListData[] lists)
        {
            lists[0].Elements.Add(new TLCCCElement { Define = "USMAX" });
            lists[1].Elements.Add(new TLCCCElement { Define = "ISMAX" });
            lists[2].Elements.Add(new TLCCCElement { Define = "TMMAX" });
            lists[3].Elements.Add(new TLCCCElement { Define = "SWMAX" });
            lists[4].Elements.Add(new TLCCCElement { Define = "PRMMAX" });

            foreach (var l in lists)
            {
                l.SetMax();
            }
        }

        public static TLCCCElemListData[] CollectAllCCOLElements(ControllerModel controller)
        {
            var lists = new TLCCCElemListData[5];

            for (var i = 0; i < 5; ++i)
            {
                lists[i] = new TLCCCElemListData();
            }

            // outputs
            foreach (var segm in controller.Data.SegmentenDisplayBitmapData)
            {
                lists[0].Elements.Add(new TLCCCElement(segm.Naam, TLCCCElementTypeEnum.Output));
            }
            foreach (var segm in controller.Data.ModulenDisplayBitmapData)
            {
                lists[0].Elements.Add(new TLCCCElement(segm.Naam, TLCCCElementTypeEnum.Output));
            }

            // inputs: none, no dummy needed

            // timers, switches, parameters: create dummy
            lists[2].Elements.Add(new TLCCCElement("dummy", 0, TLCCCElementTimeTypeEnum.TE_type, TLCCCElementTypeEnum.Timer));
            lists[3].Elements.Add(new TLCCCElement("dummy", 0, TLCCCElementTimeTypeEnum.None, TLCCCElementTypeEnum.Switch));
            lists[4].Elements.Add(new TLCCCElement("dummy", 0, TLCCCElementTimeTypeEnum.TE_type, TLCCCElementTypeEnum.Parameter));

            return lists;
        }
    }
}