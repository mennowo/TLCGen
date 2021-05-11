namespace TLCGen.Plugins.Tools
{
    public class TemplateObject
    {
        public object Object { get; set; }
        public string Description { get; set; }
        public CombinatieTemplateItemTypeEnum Type { get; set; }

        public TemplateObject(object @object, CombinatieTemplateItemTypeEnum type, string description)
        {
            Object = @object;
            Type = type;
            Description = description;
        }
    }
}