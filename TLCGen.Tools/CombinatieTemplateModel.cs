using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public static class CombinatieTemplateProvider
    {
        #region Public Methods

        public static void ApplyCombinatieTemplate(ControllerModel c, CombinatieTemplateModel t)
        {
            // check options
            foreach (var opt in t.Opties)
            {
                switch (opt.Type)
                {
                    case CombinatieTemplateOptieTypeEnum.Fase:
                        var fc = c.Fasen.FirstOrDefault(x => x.Naam == opt.Replace);
                        if (fc == null)
                        {
                            // TODO: alert user!
                            return;
                        }
                        break;
                }
            }

            // gather and check items
            var items = new List<Tuple<object, object>>();
            foreach (var i in t.Items)
            {
                var o = i.GetObject();
                FaseCyclusModel fc1 = null;
                FaseCyclusModel fc2 = null;
                foreach(var opt in t.Opties)
                {
                    switch (opt.Type)
                    {
                        case CombinatieTemplateOptieTypeEnum.Fase:
                            var tfc = c.Fasen.FirstOrDefault(x => x.Naam == opt.Replace);
                            if (tfc != null)
                            {
                                var r = ModelManagement.TLCGenModelManager.Default.ChangeNameOnObject(o, opt.Search, opt.Replace, Models.Enumerations.TLCGenObjectTypeEnum.Fase);
                                if (r > 0)
                                {
                                    if (fc1 == null) fc1 = tfc;
                                    else if (fc2 == null) fc2 = tfc;
                                }
                            }
                            else
                            {
                                // TODO: alert user!
                                return;
                            }
                            break;
                    }
                }
                switch (i.Type)
                {
                    case CombinatieTemplateItemTypeEnum.Detector:
                        if (fc1 != null)
                        {
                            items.Add(new Tuple<object, object>(o, fc1));
                        }
                        break;
                    case CombinatieTemplateItemTypeEnum.Naloop:
                    case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    case CombinatieTemplateItemTypeEnum.LateRelease:
                        var ise = (IInterSignaalGroepElement)o;
                        if(Integrity.TLCGenControllerChecker.IsFasenConflicting(c, ise.FaseVan, ise.FaseNaar))
                        {
                            // TODO: alert user!
                            return;
                        }
                        items.Add(new Tuple<object, object>(o, null));
                        break;
                    case CombinatieTemplateItemTypeEnum.Rateltikker:
                        // TODO: check detectors?
                        items.Add(new Tuple<object, object>(o, null));
                        break;
                }
            }

            // apply
            foreach(var i in items)
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
                }
            }
        }

        #endregion // Public Methods
    }

    public class CombinatieTemplateModel
    {
        #region Properties

        public string Name { get; set; }
        public List<CombinatieTemplateItemModel> Items { get; set; }
        public List<CombinatieTemplateOptieModel> Opties { get; set; }

        #endregion // Properties

        #region Constructor

        public CombinatieTemplateModel()
        {
            Items = new List<CombinatieTemplateItemModel>();
            Opties = new List<CombinatieTemplateOptieModel>();
        }

        #endregion // Constructor
    }

    public enum CombinatieTemplateItemTypeEnum
    {
        Detector,
        Naloop,
        Meeaanvraag,
        Rateltikker,
        Gelijkstart,
        LateRelease
    }

    public enum CombinatieTemplateOptieTypeEnum
    {
        Fase
    }

    public class CombinatieTemplateOptieModel
    {
        public CombinatieTemplateOptieTypeEnum Type { get; set; }
        public string Description { get; set; }
        public string Search { get; set; }
        public string Replace { get; set; }
    }

    public class CombinatieTemplateItemModel
    {
        public CombinatieTemplateItemTypeEnum Type { get; set; }
        public string ObjectJson { get; set; }

        public object GetObject()
        {
            if (string.IsNullOrWhiteSpace(ObjectJson)) return null;
            switch (Type)
            {
                case CombinatieTemplateItemTypeEnum.Detector:
                    return JsonConvert.DeserializeObject<DetectorModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Naloop:
                    return JsonConvert.DeserializeObject<NaloopModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    return JsonConvert.DeserializeObject<MeeaanvraagModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Rateltikker:
                    return JsonConvert.DeserializeObject<RatelTikkerModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    return JsonConvert.DeserializeObject<GelijkstartModel>(ObjectJson);
                case CombinatieTemplateItemTypeEnum.LateRelease:
                    return JsonConvert.DeserializeObject<LateReleaseModel>(ObjectJson);
            }
            return null;
        }

        public object GetObject<T>()
        {
            return (T)GetObject();
        }

        public CombinatieTemplateItemModel()
        {

        }
    }
}
