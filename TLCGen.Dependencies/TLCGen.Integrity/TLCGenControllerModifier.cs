using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Extensions;
using TLCGen.Models;

namespace TLCGen.Integrity
{
    // TODO This all belong to the controller manager

    public interface ITLCGenControllerModifier
    {
        ControllerModel Controller { get; set; }

		// TODO: remove upper two below and make into one generic!
        void RemoveSignalGroupFromController(string remsg);
        void RemoveDetectorFromController(string remd);
        void RemoveModelItemFromController(string remitem);
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
            set { _Controller = value; }
            get
            {
                return _Controller;
            }
        }

        #endregion // Properties

        #region ITLCGenControllerModifier

        public void RemoveSignalGroupFromController(string remsg)
        {
            RemoveFromController(_Controller, remsg);
        }

        public void RemoveDetectorFromController(string remd)
        {
            RemoveFromController(_Controller, remd);

            // Manage OV detectoren
            // TODO
#warning TODO
            //var dets = _Controller.Fasen.SelectMany(x => x.Detectoren).Concat(_Controller.Detectoren);
            //foreach (var ov in _Controller.OVData.OVIngrepen)
            //{
            //    if (ov.VerlosAanvraagDetector != null && !dets.Any(x => x.Naam == ov.VerlosAanvraagDetector))
            //    {
            //        ov.VerlosAanvraagDetector = null;
            //        ov.VerlosAanvraag = false;
            //    }
            //
            //    if (ov.WisselDetector != null && !dets.Any(x => x.Naam == ov.WisselDetector))
            //    {
            //        ov.WisselDetector = null;
            //        ov.Wissel = false;
            //    }
            //
            //    if (ov.WisselStroomKringDetector != null && !dets.Any(x => x.Naam == ov.WisselStroomKringDetector))
            //    {
            //        ov.WisselStroomKringDetector = null;
            //        ov.WisselStroomKring = false;
            //    }
            //}
        }

	    public void RemoveModelItemFromController(string uniqueModelName)
	    {
		    RemoveFromController(_Controller, uniqueModelName);
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
        
		private void RemoveFromController(object obj, string remObject)
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
								var _attr = _t.GetCustomAttribute<RefersToAttribute>();
								if (_attr != null)
								{
									var listType = typeof(List<>).MakeGenericType(_t);
									var remitems = Activator.CreateInstance(listType);
									foreach (var item in elems)
									{
										if (_attr.ReferProperty1 != null)
										{
											string val1 = (string)_t.GetProperty(_attr.ReferProperty1).GetValue(item);
											if (val1 == remObject)
											{
												t.GetMethod("Add").Invoke(remitems, new[] { item });
											}
											else if (_attr.ReferProperty2 != null)
											{
												string val2 = (string)_t.GetProperty(_attr.ReferProperty2).GetValue(item);
												if (val2 == remObject)
												{
													t.GetMethod("Add").Invoke(remitems, new[] { item });
												}
                                                else if (_attr.ReferProperty3 != null)
                                                {
                                                    string val3 = (string)_t.GetProperty(_attr.ReferProperty3).GetValue(item);
                                                    if (val3 == remObject)
                                                    {
                                                        t.GetMethod("Add").Invoke(remitems, new[] { item });
                                                    }
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
							RemoveFromController(item, remObject);
						}
					}
					else
					{
                        RemoveFromController(propValue, remObject);
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

                                if (!TLCGenControllerChecker.IsFasenConflicting(c, fase1.FaseCyclus, fase2.FaseCyclus))
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
            foreach(var hd in c.OVData.HDIngrepen)
            {
                foreach(var mfc in hd.MeerealiserendeFaseCycli)
                {
                    bool ok = false;
                    foreach (var hd2 in c.OVData.HDIngrepen)
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
