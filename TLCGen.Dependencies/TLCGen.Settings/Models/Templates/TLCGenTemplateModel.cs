using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TLCGen.Settings
{
    [Serializable]
    public class TLCGenTemplateModel<T>
    {
        public string Naam { get; set; }
        public string Replace { get; set; }

        public List<object> GetItems()
        {
            List<object> items = new List<object>();
            foreach (var fc in Items)
            {
                items.Add(fc);
            }
            return items;
        }

        [XmlArrayItem(ElementName = "Item")]
        public List<T> Items { get; set; }

        public TLCGenTemplateModel()
        {
            Items = new List<T>();
        }
    }
}
