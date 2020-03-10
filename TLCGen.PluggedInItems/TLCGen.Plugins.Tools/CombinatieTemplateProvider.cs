using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public static class CombinatieTemplateProvider
    {
        #region Public Methods

        private static int ChangeIntOnObject(object obj, int oldInt, int newInt)
        {
            var i = 0;
            if (obj == null) return i;
            var objType = obj.GetType();

            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var ignore = (TLCGenIgnoreAttributeAttribute)property.GetCustomAttribute(typeof(TLCGenIgnoreAttributeAttribute));
                if (ignore != null) continue;

                var propValue = property.GetValue(obj);

                // for ints
                if (property.PropertyType == typeof(int))
                {
                    if ((int)propValue == oldInt)
                    {
                        property.SetValue(obj, newInt);
                        ++i;
                    }
                }
                // for lists
                else if (propValue is IList elems)
                {
                    foreach (var item in elems)
                    {
                        ChangeIntOnObject(item, oldInt, newInt);
                    }
                }
                // for objects
                else if (property.PropertyType != typeof(string) && !property.PropertyType.IsValueType)
                {
                    ChangeIntOnObject(propValue, oldInt, newInt);
                }
            }
            return i;
        }

        private static int ChangeStringOnObject(object obj, string oldString, string newString)
        {
            var i = 0;
            if (obj == null) return i;
            var objType = obj.GetType();

            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var propValue = property.GetValue(obj);

                // for ints
                if (property.PropertyType == typeof(string) && propValue != null)
                {
                    if (Regex.IsMatch((string)propValue, oldString))
                    {
                        var changed = Regex.Replace((string)propValue, oldString, newString);
                        property.SetValue(obj, changed);
                        ++i;
                    }
                }
                // for lists
                else if (propValue is IList elems)
                {
                    foreach (var item in elems)
                    {
                        ChangeStringOnObject(item, oldString, newString);
                    }
                }
                // for objects
                else if (!property.PropertyType.IsValueType)
                {
                    ChangeStringOnObject(propValue, oldString, newString);
                }
            }
            return i;
        }

        public static Tuple<bool, string> ApplyCombinatieTemplate(ControllerModel c, CombinatieTemplateModel t)
        {
            var alert = "";
            // check options
            foreach (var opt in t.Opties)
            {
                switch (opt.Type)
                {
                    case CombinatieTemplateOptieTypeEnum.Fase:
                        var fc = c.Fasen.FirstOrDefault(x => x.Naam == opt.Replace);
                        if (fc == null)
                        {
                            return new Tuple<bool, string>(false, $"Fase {opt.Replace} komt niet voor in deze regeling; template niet toegepast.");
                        }
                        break;
                }
            }

            // gather and check items
            var items = new List<Tuple<object, object>>();
            foreach (var i in t.Items)
            {
                var o = i.GetObject();
                var ok = true;
                FaseCyclusModel fc1 = null;
                FaseCyclusModel fc2 = null;
                FaseCyclusModel fc3 = null;
                foreach (var opt in t.Opties)
                {
                    switch (opt.Type)
                    {
                        case CombinatieTemplateOptieTypeEnum.Fase:
                            var tfc = c.Fasen.FirstOrDefault(x => x.Naam == opt.Replace);
                            if (tfc != null)
                            {
                                var r = ChangeStringOnObject(o, opt.Search, opt.Replace);
                                if (r > 0)
                                {
                                    if (fc1 == null) fc1 = tfc;
                                    else if (fc2 == null) fc2 = tfc; // as of yet: unused
                                    else if (fc3 == null) fc3 = tfc; // as of yet: unused
                                }
                            }
                            break;
                        case CombinatieTemplateOptieTypeEnum.Int:
                            ChangeIntOnObject(o, int.Parse(opt.Search), int.Parse(opt.Replace));
                            break;
                        case CombinatieTemplateOptieTypeEnum.String:
                            ChangeStringOnObject(o, opt.Search, opt.Replace);
                            break;
                    }
                }
                switch (i.Type)
                {
                    case CombinatieTemplateItemTypeEnum.Detector:
                        if(c.GetAllDetectors().Any(x => x.Naam == ((DetectorModel)o).Naam))
                        {
                            alert += $"De regeling bevat reeds een detector met de naam {((DetectorModel)o).Naam}.\n";
                        }
                        else if (fc1 != null)
                        {
                            items.Add(new Tuple<object, object>(o, fc1));
                        }
                        else
                        {
                            alert += $"Detector {((DetectorModel)o).Naam} uit de template heeft geen waarde voor de fase; deze wordt niet toegevoegd.\n";
                        }
                        break;
                    case CombinatieTemplateItemTypeEnum.Naloop:
                    case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    case CombinatieTemplateItemTypeEnum.LateRelease:
                        var ise = (IInterSignaalGroepElement)o;
                        if(Integrity.TLCGenControllerChecker.IsFasenConflicting(c, ise.FaseVan, ise.FaseNaar))
                        {
                            return new Tuple<bool, string>(false, $"Fasen {ise.FaseVan} en {ise.FaseNaar} hebben een conflict; template niet toegepast.");
                        }
                        switch (o)
                        {
                            case NaloopModel nl:
                                if (c.InterSignaalGroep.Nalopen.Any(x => x.FaseVan == nl.FaseVan && x.FaseNaar == nl.FaseNaar))
                                {
                                    alert += $"De regeling bevat reeds een naloop van {nl.FaseVan} naar {nl.FaseNaar}.\n";
                                    ok = false;
                                }
                                break;
                            case MeeaanvraagModel ma:
                                if (c.InterSignaalGroep.Meeaanvragen.Any(x => x.FaseVan == ma.FaseVan && x.FaseNaar == ma.FaseNaar))
                                {
                                    alert += $"De regeling bevat reeds een meeaanvraag van {ma.FaseVan} naar {ma.FaseNaar}.\n";
                                    ok = false;
                                }
                                break;
                            case GelijkstartModel gs:
                                if (c.InterSignaalGroep.Gelijkstarten.Any(x => x.FaseVan == gs.FaseVan && x.FaseNaar == gs.FaseNaar))
                                {
                                    alert += $"De regeling bevat reeds een gelijkstart van {gs.FaseVan} naar {gs.FaseNaar}.\n";
                                    ok = false;
                                }
                                break;
                            case LateReleaseModel lr:
                                if (c.InterSignaalGroep.LateReleases.Any(x => x.FaseVan == lr.FaseVan && x.FaseNaar == lr.FaseNaar))
                                {
                                    alert += $"De regeling bevat reeds een late release van {lr.FaseVan} naar {lr.FaseNaar}.\n";
                                    ok = false;
                                }
                                break;
                        }
                        if (ok) items.Add(new Tuple<object, object>(o, null));
                        break;
                    case CombinatieTemplateItemTypeEnum.Rateltikker:
                        if(!c.Signalen.Rateltikkers.Any(x => x.FaseCyclus == ((RatelTikkerModel)o).FaseCyclus))
                        {
                            items.Add(new Tuple<object, object>(o, null));
                        }
                        else
                        {
                            alert += $"De regeling bevat reeds rateltikker voor fase {((RatelTikkerModel)o).FaseCyclus}.\n";
                        }
                        break;
                }
            }

            // apply
            foreach (var i in items)
            {
                switch (i.Item1)
                {
                    case DetectorModel d:
                        var fc = (FaseCyclusModel)i.Item2;
                        fc.Detectoren.Add(d);
                        Messenger.Default.Send(new Messaging.Messages.DetectorenChangedMessage(c, new List<DetectorModel> { d }, null));
                        break;
                    case NaloopModel nl:
                        c.InterSignaalGroep.Nalopen.Add(nl);
                        Messenger.Default.Send(new Messaging.Messages.InterSignaalGroepChangedMessage(nl.FaseVan, nl.FaseNaar, nl, isnew: true));
                        break;
                    case MeeaanvraagModel ma:
                        c.InterSignaalGroep.Meeaanvragen.Add(ma);
                        Messenger.Default.Send(new Messaging.Messages.InterSignaalGroepChangedMessage(ma.FaseVan, ma.FaseNaar, ma, isnew: true));
                        break;
                    case GelijkstartModel gs:
                        c.InterSignaalGroep.Gelijkstarten.Add(gs);
                        Messenger.Default.Send(new Messaging.Messages.InterSignaalGroepChangedMessage(gs.FaseVan, gs.FaseNaar, gs, isnew: true));
                        break;
                    case LateReleaseModel lr:
                        c.InterSignaalGroep.LateReleases.Add(lr);
                        Messenger.Default.Send(new Messaging.Messages.InterSignaalGroepChangedMessage(lr.FaseVan, lr.FaseNaar, lr, isnew: true));
                        break;
                    case RatelTikkerModel rt:
                        c.Signalen.Rateltikkers.Add(rt);
                        // Trick to force rebuilding list in UI
                        Messenger.Default.Send(new Messaging.Messages.DetectorenChangedMessage(c, null, null));
                        break;
                }
            }
            return new Tuple<bool, string>(true, alert);
        }

        #endregion // Public Methods
    }
}
