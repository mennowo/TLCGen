using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TLCGen.Extensions;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Integrity
{
    // TODO This all belong to the controller manager

    public interface ITLCGenControllerModifier
    {
        ControllerModel Controller { get; set; }

		void RemoveModelItemFromController(string remitem, TLCGenObjectTypeEnum objectType);
        void CorrectModel_AlteredConflicts();
        void CorrectModel_AlteredHDIngrepen();
    }

    public class TLCGenControllerModifier : ITLCGenControllerModifier
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenControllerModifier _Default;

        #endregion // Fields

        #region Properties

        public static ITLCGenControllerModifier Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TLCGenControllerModifier();
                        }
                    }
                }
                return _Default;
            }
        }

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            set => _Controller = value;
            get => _Controller;
        }

        #endregion // Properties

        #region ITLCGenControllerModifier

        public void RemoveModelItemFromController(string uniqueModelName, TLCGenObjectTypeEnum objectType)
	    {
		    RemoveFromController(_Controller, uniqueModelName, objectType);
	    }

		public void CorrectModel_AlteredConflicts()
        {
            CorrectModelWithAlteredConflicts(_Controller);
        }

        public void CorrectModel_AlteredHDIngrepen()
        {
            CorrectModel_AlteredHDIngrepen(_Controller);
        }

        #endregion // ITLCGenControllerModifier

        #region Public Methods

        public static void OverrideDefault(ITLCGenControllerModifier modifier)
        {
            _Default = modifier;
        }

        #endregion // Public Methods

        #region Private Methods
        
		private void RemoveFromController(object obj, string remObject, TLCGenObjectTypeEnum objectType)
		{
			if (obj == null) return;
			var objType = obj.GetType();
            // find and loop all properties
			var objectProperties = objType.GetProperties();
            foreach (var objectProperty in objectProperties)
			{
                // if flagged, ignore this
                var ignore = (TLCGenIgnoreAttribute)objectProperty.GetCustomAttribute(typeof(TLCGenIgnoreAttribute));
                if (ignore != null) continue;

                var propType = objectProperty.PropertyType;
                // for lists, and objects we check if items need removing
				if (!(propType == typeof(string)) && !propType.IsValueType)
				{
					var propValue = objectProperty.GetValue(obj);
                    if (propValue is IList elems)
					{
						var elemsType = elems.GetType();
						if (elemsType.IsGenericType)
						{
							var genericType = elemsType.GetGenericArguments()[0];
                            if (genericType != typeof(List<>))
                            {
                                var listType = typeof(List<>).MakeGenericType(genericType);
                                var remitems = Activator.CreateInstance(listType);
                                foreach (var item in elems)
                                {
                                    var itemProperties = genericType.GetProperties();
                                    foreach (var itemProperty in itemProperties)
                                    {
                                        var itemPropIgnore = (TLCGenIgnoreAttribute) itemProperty.GetCustomAttribute(typeof(TLCGenIgnoreAttribute));
                                        if (itemPropIgnore != null) continue;

                                        // for strings
                                        if (itemProperty.PropertyType == typeof(string))
                                        {
                                            var itemPropValue = itemProperty.GetValue(item);
                                            var itemRefToAttr = itemProperty.GetCustomAttribute<RefersToAttribute>();
                                            if (itemRefToAttr == null) continue;
                                            var refObjectType = itemRefToAttr.ObjectType;
                                            // if applicable, find actual object type
                                            if (itemRefToAttr.ObjectTypeProperty != null)
                                            {
                                                var objTypeProp = itemProperties.FirstOrDefault(x => x.Name == itemRefToAttr.ObjectTypeProperty);
                                                if (objTypeProp != null && objTypeProp.PropertyType == typeof(TLCGenObjectTypeEnum))
                                                {
                                                    refObjectType = (TLCGenObjectTypeEnum) objTypeProp.GetValue(item);
                                                }
                                            }

                                            // add to removal list if needed
                                            if (objectType == refObjectType &&
                                                itemPropValue as string == remObject)
                                            {
                                                elemsType.GetMethod("Add").Invoke(remitems, new[] {item});
                                                break;
                                            }
                                        }
                                    }
                                }

                                foreach (var item in (IList) remitems)
                                {
                                    elems.Remove(item);
                                }
                            }
                        }
                        // now, remaining elements are recursible checked for containing elements
						foreach (var item in elems)
						{
							RemoveFromController(item, remObject, objectType);
						}
					}
                    // objects are recursible checked for containing elements
                    else
					{
                        RemoveFromController(propValue, remObject, objectType);
					}
				}
			}
        }

		private void CorrectModelWithAlteredConflicts(object obj)
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

                                if (TLCGenControllerChecker.IsFasenConflicting(c, mlfc1.FaseCyclus, mlfc2.FaseCyclus))
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
                        var conflicts = TLCGenControllerChecker.GetFaseConflicts(c, ontr.FaseCyclus);
                        foreach(var d in ontr.VADetectoren)
                        {
                            var addfcs = new List<VAOntruimenNaarFaseModel>();
                            var remfcs = new List<VAOntruimenNaarFaseModel>();
                            foreach (var fc in d.ConflicterendeFasen)
                            {
                                if(!TLCGenControllerChecker.IsFasenConflicting(c, ontr.FaseCyclus, fc.FaseCyclus))
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

                                if (!c.RoBuGrover.ToestaanNietConflictenInConflictGroepen && !TLCGenControllerChecker.IsFasenConflicting(c, fase1.FaseCyclus, fase2.FaseCyclus))
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
        
        private void CorrectModel_AlteredHDIngrepen(ControllerModel c)
        {
            var remfcs = new List<HDIngreepMeerealiserendeFaseCyclusModel>();
            foreach(var hd in c.PrioData.HDIngrepen)
            {
                foreach(var mfc in hd.MeerealiserendeFaseCycli)
                {
                    var ok = false;
                    foreach (var hd2 in c.PrioData.HDIngrepen)
                    {
                        if(hd != hd2 && mfc.FaseCyclus == hd2.FaseCyclus)
                        {
                            ok = true;
                            break;
                        }
                    }
                    if(!ok)
                    {
                        remfcs.Add(mfc);
                    }
                }
                foreach(var rfc in remfcs)
                {
                    hd.MeerealiserendeFaseCycli.Remove(rfc);
                }
            }
        }

        #endregion // Private Methods
    }
}
