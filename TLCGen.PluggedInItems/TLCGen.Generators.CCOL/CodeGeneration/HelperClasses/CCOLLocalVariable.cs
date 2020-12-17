namespace TLCGen.Generators.CCOL.CodeGeneration.HelperClasses
{
    public class CCOLLocalVariable
    {   
        public string Type { get; }
        public string Name { get; }
        public string InitialValue { get; set; }
        public string DefineCondition { get; set; }

        public CCOLLocalVariable(string type, string name, string initialValue = "", string defineCondition = "")
        {
            Type = type;
            Name = name;
            InitialValue = initialValue;
            DefineCondition = defineCondition;
        }
    }
}
