using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Helpers
{
    public static class MultiPropertySetter
    {
        public static void SetPropertyForAllItems<T>(object sender, string propname, IList items)
        {
            var v = typeof(T).GetProperty(propname).GetValue(sender);
            foreach (T item in items)
            {
                if (!object.ReferenceEquals(item, sender))
                {
                    var prop = sender.GetType().GetProperty(propname, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(item, v, null);
                    }
                }
            }
        }
    }
}
