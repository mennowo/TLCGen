using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;

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

        public static void RemoveDetectorFromController(object obj, string remd)
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
                                var _attr = _t.GetCustomAttribute<RefersToDetectorAttribute>();
                                if (_attr != null)
                                {
                                    var listType = typeof(List<>).MakeGenericType(_t);
                                    var remitems = Activator.CreateInstance(listType);
                                    foreach (var item in elems)
                                    {
                                        if (_attr.DetectorProperty1 != null)
                                        {
                                            string val1 = (string)_t.GetProperty(_attr.DetectorProperty1).GetValue(item);
                                            if (val1 == remd)
                                            {
                                                t.GetMethod("Add").Invoke(remitems, new[] { item });
                                            }
                                            else if (_attr.DetectorProperty2 != null)
                                            {
                                                string val2 = (string)_t.GetProperty(_attr.DetectorProperty2).GetValue(item);
                                                if (val2 == remd)
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
                            RemoveDetectorFromController(item, remd);
                        }
                    }
                    else
                    {
                        RemoveDetectorFromController(propValue, remd);
                    }
                }
            }
        }

        public static void CorrectModelWithAlteredConflicts(object obj)
        {
            var c = obj as ControllerModel;
            if (c != null)
            {
                // Modules
                if (c.ModuleMolen != null)
                {
                    var remfcs = new List<ModuleFaseCyclusModel>();
                    foreach (var ml in c.ModuleMolen.Modules)
                    {
                        foreach (var mlfc1 in ml.Fasen)
                        {
                            foreach (var mlfc2 in ml.Fasen)
                            {
                                if (mlfc1.FaseCyclus == mlfc2.FaseCyclus)
                                    continue;

                                if (ControllerChecker.IsFasenConflicting(c, mlfc1.FaseCyclus, mlfc2.FaseCyclus))
                                {
                                    remfcs.Add(mlfc1);
                                    break;
                                }
                            }
                        }
                        foreach(var fc in remfcs)
                        {
                            ml.Fasen.Remove(fc);
                        }
                    }
                }
                // VA ontruimen
                if(c.VAOntruimenFasen?.Count > 0)
                {
                    foreach(var ontr in c.VAOntruimenFasen)
                    {
                        var conflicts = ControllerChecker.GetFaseConflicts(c, ontr.FaseCyclus);
                        foreach(var d in ontr.VADetectoren)
                        {
                            var addfcs = new List<VAOntruimenNaarFaseModel>();
                            var remfcs = new List<VAOntruimenNaarFaseModel>();
                            foreach (var fc in d.ConflicterendeFasen)
                            {
                                if(!ControllerChecker.IsFasenConflicting(c, ontr.FaseCyclus, fc.FaseCyclus))
                                {
                                    remfcs.Add(fc);
                                }
                            }
                            foreach(var conflict in conflicts)
                            {
                                if(conflict.Waarde < 0)
                                {
                                    continue;
                                }
                                if(!d.ConflicterendeFasen.Where(x => x.FaseCyclus == conflict.FaseNaar).Any())
                                {
                                    addfcs.Add(new VAOntruimenNaarFaseModel() { FaseCyclus = conflict.FaseNaar });
                                }
                            }
                            foreach(var fc in remfcs)
                            {
                                d.ConflicterendeFasen.Remove(fc);
                            }
                            foreach (var fc in addfcs)
                            {
                                d.ConflicterendeFasen.Add(fc);
                            }
                            d.ConflicterendeFasen.BubbleSort();
                        }
                    }
                }
                // RoBuGrover
                if(c.RoBuGrover?.ConflictGroepen?.Count > 0)
                {
                    foreach(var cg in c.RoBuGrover.ConflictGroepen)
                    {
                        var remfcs = new List<RoBuGroverConflictGroepFaseModel>();
                        foreach(var fase1 in cg.Fasen)
                        {
                            foreach (var fase2 in cg.Fasen)
                            {
                                if (fase1.FaseCyclus == fase2.FaseCyclus)
                                    continue;

                                if (!ControllerChecker.IsFasenConflicting(c, fase1.FaseCyclus, fase2.FaseCyclus))
                                {
                                    remfcs.Add(fase1);
                                }
                            }
                        }
                        foreach(var fc in remfcs)
                        {
                            cg.Fasen.Remove(fc);
                        }
                    }
                }
            }
        }
    }
}
