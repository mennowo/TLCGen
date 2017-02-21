using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Helpers
{
    public static class ModelStringSetter
    {
        public static void SetStringInModel(object obj, string oldstring, string newstring)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj);
                if (property.PropertyType == typeof(string))
                {
                    string propString = (string)propValue;
                    if (propString == oldstring)
                    {
                        property.SetValue(obj, newstring);
                    }
                }
                else if (!property.PropertyType.IsValueType)
                {
                    var elems = propValue as IList;
                    if (elems != null)
                    {
                        foreach (var item in elems)
                        {
                            SetStringInModel(item, oldstring, newstring);
                        }
                    }
                    else
                    {
                        SetStringInModel(propValue, oldstring, newstring);
                    }
                }
            }
        }

        public static void ReplaceStringInModel(object obj, string oldstring, string newstring)
        {
            if (obj == null) return;
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propValue = property.GetValue(obj);
                if (property.PropertyType == typeof(string))
                {
                    string propString = (string)propValue;
                    if (propString != null && propString.Contains(oldstring))
                    {
                        property.SetValue(obj, propString.Replace(oldstring, newstring));
                    }
                }
                else if (!property.PropertyType.IsValueType)
                {
                    var elems = propValue as IList;
                    if (elems != null)
                    {
                        foreach (var item in elems)
                        {
                            ReplaceStringInModel(item, oldstring, newstring);
                        }
                    }
                    else
                    {
                        ReplaceStringInModel(propValue, oldstring, newstring);
                    }
                }
            }
        }
    }
}
