using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Operations
{
    public static class ControllerModifier
    {
        public static void RemoveSignalGroupFromController(object obj, string remsg)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Type propType = property.PropertyType;
                if (!(propType == typeof(string)) && !propType.IsValueType)
                {
                    object propValue = property.GetValue(obj);
                    var elems = propValue as IList;
                    if (elems != null)
                    {
                        var t = elems.GetType();
                        if (t.IsGenericType)
                        {
                            var _t = t.GetGenericArguments()[0];
                            if (_t != typeof(List<>))
                            {
                                var _attr = _t.GetCustomAttribute<RefersToSignalGroupAttribute>();
                                if (_attr != null)
                                {
                                    var listType = typeof(List<>).MakeGenericType(_t);
                                    var remitems = Activator.CreateInstance(listType);
                                    foreach (var item in elems)
                                    {
                                        if (_attr.SignalGroupProperty1 != null)
                                        {
                                            string val1 = (string)_t.GetProperty(_attr.SignalGroupProperty1).GetValue(item);
                                            if (val1 == remsg)
                                            {
                                                t.GetMethod("Add").Invoke(remitems, new[] { item });
                                            }
                                            else if (_attr.SignalGroupProperty2 != null)
                                            {
                                                string val2 = (string)_t.GetProperty(_attr.SignalGroupProperty2).GetValue(item);
                                                if (val2 == remsg)
                                                {
                                                    t.GetMethod("Add").Invoke(remitems, new[] { item });
                                                }
                                            }
                                        }
                                    }
                                    foreach (var item in (IList)remitems)
                                    {
                                        elems.Remove(item);
                                    }
                                }
                            }
                        }
                        foreach (var item in elems)
                        {
                            RemoveSignalGroupFromController(item, remsg);
                        }
                    }
                    else
                    {
                        RemoveSignalGroupFromController(propValue, remsg);
                    }
                }
            }
        }
    }
}
