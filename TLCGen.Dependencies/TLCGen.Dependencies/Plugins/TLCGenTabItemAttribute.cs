namespace TLCGen.Plugins
{
    public enum TabItemTypeEnum
    {
        MainWindow,
        AlgemeenTab,
        FasenTab,
        PrioTab,
        PrioriteitTab,
        DetectieTab,
        ModulesTab,
        PeriodenTab,
        SpecialsTab
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class TLCGenTabItemAttribute : System.Attribute
    {
        /// <summary>
        /// Zero based index indicating how to order tabs loaded into TLCGen
        /// </summary>
        public int Index { get; set; }
        public TabItemTypeEnum Type { get; set; }

        public TLCGenTabItemAttribute(int index, TabItemTypeEnum type = TabItemTypeEnum.MainWindow)
        {
            Index = index;
            Type = type;
        }
    }
}
