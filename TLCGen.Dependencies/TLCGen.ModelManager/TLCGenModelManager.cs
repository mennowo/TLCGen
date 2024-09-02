using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Serialization;
using TLCGen.Dependencies.Providers;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Settings;

namespace TLCGen.ModelManagement
{
    public class TLCGenModelManager : ITLCGenModelManager
    {
        #region Fields

        private static readonly object _Locker = new object();
        private static ITLCGenModelManager _Default;

        private Action<object, string> _setDefaultsAction;

        private readonly List<Tuple<string, object>> _pluginDataToMove = new List<Tuple<string, object>>();
        private ControllerModel _controller;

        #endregion // Fields

        #region Properties

        public static ITLCGenModelManager Default
        {
            get
            {
                if (_Default == null)
                {
                    lock (_Locker)
                    {
                        if (_Default == null)
                        {
                            _Default = new TLCGenModelManager();
                        }
                    }
                }
                return _Default;
            }
        }

        private IMessenger MessengerInstance { get; set; }

        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value; 
                ControllerAlerts.CollectionChanged += (sender, args) => ControllerAlertsUpdated?.Invoke(this, EventArgs.Empty);
                UpdateControllerAlerts();
            }
        }

        #endregion // Properties

        #region Public Methods

        public void SetSpecialIOPerSignalGroup(ControllerModel controller)
        { 
            if (controller.PrioData.PrioUitgangPerFase)
            {
                controller.PrioData.PrioIngrepen.ForEach(x => x.GeenEigenVerklikking = true);
                controller.Fasen.ForEach(x => x.PrioIngreep = controller.PrioData.PrioIngrepen.Any(x2 => x2.FaseCyclus == x.Naam));
                controller.Fasen.ForEach(x => x.PrioIngreepGeconditioneerd = controller.PrioData.PrioIngrepen.Any(x2 => x2.FaseCyclus == x.Naam && x2.GeconditioneerdePrioriteit != NooitAltijdAanUitEnum.Nooit));
            }
            else
            {
                controller.PrioData.PrioIngrepen.ForEach(x => x.GeenEigenVerklikking = false);
                controller.Fasen.ForEach(x => x.PrioIngreep = false);
                controller.Fasen.ForEach(x => x.PrioIngreepGeconditioneerd = false);
            }

            foreach (var fase in controller.Fasen)
            {
                fase.HasWachttijdVoorspellerBus = 
                    controller.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen && 
                    controller.Data.WachttijdvoorspellerAansturenBus &&
                    fase.WachttijdVoorspeller;
            }
        }

        public void ConvertToIntergroen(ControllerModel controller)
        {
            foreach(var igt in controller.InterSignaalGroep.Conflicten)
            {
                var fc = controller.Fasen.FirstOrDefault(x => x.Naam == igt.FaseVan);
                if (fc == null) continue;
                igt.Waarde += fc.TGL;
                if (igt.GarantieWaarde.HasValue) igt.GarantieWaarde += fc.TGL;
            }
        }

        public void ConvertToOntruimingstijden(ControllerModel controller)
        {
            foreach (var igt in controller.InterSignaalGroep.Conflicten)
            {
                var fc = controller.Fasen.FirstOrDefault(x => x.Naam == igt.FaseVan);
                if (fc == null) continue;
                igt.Waarde -= fc.TGL;
                if (igt.Waarde < 0) igt.Waarde = 0;
                if (igt.GarantieWaarde.HasValue)
                {
                    igt.GarantieWaarde -= fc.TGL;
                    if (igt.GarantieWaarde < 0) igt.GarantieWaarde = 0;
                }
            }
        }

        public static void OverrideDefault(ITLCGenModelManager provider)
        {
            _Default = provider;
        }

	    public void InjectDefaultAction(Action<object, string> setDefaultsAction)
	    {
		    _setDefaultsAction = setDefaultsAction;
	    }

        public bool CheckVersionOrder(ControllerModel controller)
        {
            var vc = Version.Parse(string.IsNullOrWhiteSpace(controller.Data.TLCGenVersie) ? "0.0.0.0" : controller.Data.TLCGenVersie);
            var vp = Assembly.GetEntryAssembly()?.GetName().Version;
            if (vc <= vp) return true;
            TLCGenDialogProvider.Default.ShowMessageBox($"Dit bestand is gemaakt met een nieuwere versie van TLCGen,\n" +
                            $"en kan met deze versie niet worden geopend.\n\n" +
                            $"Versie TLCGen: {vp}\n" +
                            $"Versie bestand: {vc}", "Versies komen niet overeen", MessageBoxButton.OK);
            return false;
        }

        /// <summary>
        /// THis method is supposed to set properties on the model that are not saved, and
        /// are only used internally to ensure correct displaying of settings to the end user
        /// </summary>
        public void PrepareModelForUI(ControllerModel controller)
        {
            // set signalen bitmap items availability
            controller.Signalen.ControllerHasPeriodRtAanvraag = controller.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag);
            controller.Signalen.ControllerHasPeriodRtAltijd = controller.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd);
            controller.Signalen.ControllerHasPeriodRtDimmen = controller.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen);
            controller.Signalen.ControllerHasPeriodBellenDimmen= controller.PeriodenData.Perioden.Any(x => x.Type == PeriodeTypeEnum.BellenDimmen);

            // set display for seperate dimming outputs
            if (controller.Signalen.DimUitgangPerTikker)
            {
                foreach (var rt in controller.Signalen.Rateltikkers)
                {
                    rt.DimmenPerUitgang = true;
                    if (rt.DimUitgangBitmapData == null) rt.DimUitgangBitmapData = new BitmapCoordinatenDataModel();
                }
            }

            // sort if needed
            if (!controller.Detectoren.IsSorted()) controller.Detectoren.BubbleSort();
            if (!controller.SelectieveDetectoren.IsSorted()) controller.SelectieveDetectoren.BubbleSort();
        }
        
        public void CorrectModelByVersion(ControllerModel controller, string filename)
        {
            // no version?
            if (controller.Data.TLCGenVersie == null)
            {
                controller.Data.TLCGenVersie = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            // move data
            if (_pluginDataToMove.Any())
            {
                var risData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "RISData")?.Item2;
                if (risData != null && (controller.RISData == null || controller.RISData.RISFasen.Count == 0)) controller.RISData = (RISDataModel)risData;
                var dhData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "SpecialsDenHaagData")?.Item2;
                if (dhData != null && (controller.AlternatievenPerBlokData == null || controller.AlternatievenPerBlokData.AlternatievenPerBlok?.Count == 0)) controller.AlternatievenPerBlokData = (AlternatievenPerBlokModel)dhData;
                else if (controller.AlternatievenPerBlokData == null) controller.AlternatievenPerBlokData = new AlternatievenPerBlokModel();
                var rtdData = (Dictionary<string, bool>)_pluginDataToMove.FirstOrDefault(x => x.Item1 == "SpecialsRotterdamData")?.Item2;
                if (rtdData != null && rtdData.ContainsKey("ToevoegenOVM")) controller.Data.ToevoegenOVM = rtdData["ToevoegenOVM"];
                if (rtdData != null && rtdData.ContainsKey("PrmLoggingTfbMax")) controller.Data.PrmLoggingTfbMax = rtdData["PrmLoggingTfbMax"];
                var prioData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "PrioData")?.Item2;
                if (prioData != null) controller.PrioData = (PrioriteitDataModel)prioData;
            }

            // correct segment items
            foreach(var s in controller.Data.SegmentenDisplayBitmapData)
            {
                if (s.Naam.StartsWith("segm"))
                {
                    s.Naam = s.Naam.Replace("segm", "");
                }
            }

            if (string.IsNullOrWhiteSpace(controller.ModuleMolen.Reeks))
            {
                controller.ModuleMolen.Reeks = "ML";
            }

            foreach (var fcm in controller.Fasen)
            {
                if (!fcm.Detectoren.IsSorted())
                {
                    fcm.Detectoren.BubbleSort();
                }
            }

            // check PostAfhandelingPrio_Add in prio.add
            if (filename != null)
            {
                var ovAddFile = Path.Combine(Path.GetDirectoryName(filename) ?? throw new InvalidOperationException(), controller.Data.Naam + "prio.add");
                if (File.Exists(ovAddFile))
                {
                    var ovaddtext = File.ReadAllLines(ovAddFile);
                    if (ovaddtext.All(x => !Regex.IsMatch(x, @"^\s*void\s+PostAfhandelingPrio_Add.*")))
                    {
                        TLCGenDialogProvider.Default.ShowMessageBox("Let op! Deze versie van TLCGen maakt een functie\n" +
                                    $"'PostAfhandelingPrio' aan in bestand {controller.Data.Naam}ov.c. Hierin wordt\n" +
                                    "de functie 'PostAfhandelingPrio_Add' aangeroepen, die echter\n" +
                                    $"ontbreekt in bestand {controller.Data.Naam}ov.add.", "Functie PostAfhandelingPrio_Add ontbreekt.\n\n" +
                                    "Voeg deze dus toe, waarschijnlijk in plaats van 'void post_AfhandelingPrio'," +
                                    "want die wordt niet aangeroepen.", MessageBoxButton.OK);
                    }
                }
            }

            // Moving old data around
            if (_pluginDataToMove.Any())
            {
                var risData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "RISData")?.Item2;
                if (risData != null && (controller.RISData == null || controller.RISData.RISFasen.Count == 0)) controller.RISData = (RISDataModel)risData;
                var dhData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "SpecialsDenHaagData")?.Item2;
                if (dhData != null && (controller.AlternatievenPerBlokData == null || controller.AlternatievenPerBlokData.AlternatievenPerBlok?.Count == 0)) controller.AlternatievenPerBlokData = (AlternatievenPerBlokModel)dhData;
                else if (controller.AlternatievenPerBlokData == null) controller.AlternatievenPerBlokData = new AlternatievenPerBlokModel();
                var rtdData = (Dictionary<string, bool>)_pluginDataToMove.FirstOrDefault(x => x.Item1 == "SpecialsRotterdamData")?.Item2;
                if (rtdData != null && rtdData.ContainsKey("ToevoegenOVM")) controller.Data.ToevoegenOVM = rtdData["ToevoegenOVM"];
                if (rtdData != null && rtdData.ContainsKey("PrmLoggingTfbMax")) controller.Data.PrmLoggingTfbMax = rtdData["PrmLoggingTfbMax"];
                var prioData = _pluginDataToMove.FirstOrDefault(x => x.Item1 == "PrioData")?.Item2;
                if (prioData != null) controller.PrioData = (PrioriteitDataModel)prioData;
            }

            var v = Version.Parse(string.IsNullOrWhiteSpace(controller.Data.TLCGenVersie) ? "0.0.0.0" : controller.Data.TLCGenVersie);
            
            // In version 0.2.3.0, handling of segments was altered.
            var checkVer = Version.Parse("0.2.3.0");
            if(v < checkVer)
            {
                foreach (var s in controller.Data.SegmentenDisplayBitmapData.Where(s => s.Naam.StartsWith("segm")))
                {
                    s.Naam = s.Naam.Replace("segm", "");
                }
            }

            // In version 0.3.5.0, voetgangers got lanes.
            checkVer = Version.Parse("0.3.5.0");
            if (v < checkVer)
            {
                foreach (var s in controller.Fasen)
                {
                    if (!s.AantalRijstroken.HasValue || s.Type == FaseTypeEnum.Voetganger && s.AantalRijstroken.Value == 1)
                    {
                        switch (s.Type)
                        {
                            case FaseTypeEnum.Voetganger:
                                s.AantalRijstroken = 2;
                                break;
                            default:
                                s.AantalRijstroken = 1;
                                break;
                        }
                    }
                }
            }

            // Version 0.5.4.0
            // Sort controller.HalfstarData.FaseCyclusInstellingen after renaming
            // Set file ingrepen doseren to default 'Doseren toepassen'
            checkVer = Version.Parse("0.5.4.0");
            if (v < checkVer)
            {
                controller.HalfstarData.FaseCyclusInstellingen.BubbleSort();
                foreach (var fm in controller.FileIngrepen) fm.ToepassenDoseren = NooitAltijdAanUitEnum.Altijd;
            }

            // Version 0.7.4.0
            checkVer = Version.Parse("0.7.4.0");
            if (v < checkVer)
            {
                // Prioriteisingrepen hebben nu een unieke naam per fase
                foreach (var prio in controller.PrioData.PrioIngrepen)
                {
                    if (string.IsNullOrWhiteSpace(prio.Naam))
                    {
                        prio.Naam = DefaultsProvider.Default.GetVehicleTypeAbbreviation(prio.Type);
                    }
                }

                // Detectoren bij fasen hebben nu een expliciete link met de fase
                foreach (var sg in controller.Fasen)
                {
                    foreach (var d in sg.Detectoren)
                    {
                        d.FaseCyclus = sg.Naam;
                    }
                }
            }

            checkVer = Version.Parse("0.7.8.0");
            if (v < checkVer)
            {
                // Prioriteismeldingen hebben nu een naam die wordt weergegeven in de UI
                foreach (var prio in controller.PrioData.PrioIngrepen)
                {
                    foreach (var m in prio.MeldingenData.Inmeldingen.Where(m => string.IsNullOrWhiteSpace(m.Naam)))
                    {
                        m.Naam = prio.FaseCyclus + prio.Naam + DefaultsProvider.Default.GetMeldingShortcode(m) + "in";
                    }
                    foreach (var m in prio.MeldingenData.Uitmeldingen.Where(m => string.IsNullOrWhiteSpace(m.Naam)))
                    {
                        m.Naam = prio.FaseCyclus + prio.Naam + DefaultsProvider.Default.GetMeldingShortcode(m) + "uit";
                    }
                }
            }

            checkVer = Version.Parse("0.9.8.0");
            if (v < checkVer)
            {
                // de uitgang perbeldim heeft nu eigen bitmap data  
                if (controller.Signalen.BellenDimmenBitmapData.Naam == "perbeldim") controller.Signalen.BellenDimmenBitmapData.Naam = "beldim";
            }
            
            checkVer = Version.Parse("0.12.0.0");
            if (v < checkVer)
            {
                foreach (var d in controller.Fasen.SelectMany(x => x.Detectoren))
                {
                    d.AanvraagDirectSch = d.AanvraagDirect
                        ? NooitAltijdAanUitEnum.SchAan
                        : NooitAltijdAanUitEnum.Nooit;
                }
            }

            checkVer = Version.Parse("12.4.0.6");
            if (v < checkVer)
            {
                foreach (var prio in controller.PrioData.PrioIngrepen)
                {
                    foreach (var melding in prio.MeldingenData.Inmeldingen)
                    {
                        if (melding.RisEta.HasValue) melding.RisEta *= 10;
                    }
                    foreach (var melding in prio.MeldingenData.Uitmeldingen)
                    {
                        if (melding.RisEta.HasValue) melding.RisEta *= 10;
                    }
                }
                foreach (var hd in controller.PrioData.HDIngrepen)
                {
                    if (hd.RisEta.HasValue) hd.RisEta *= 10;
                }
            }
        }

        private static void RenameXmlNode(XmlDocument doc, XmlNode oldRoot, string newname)
        {
            XmlNode newRoot = doc.CreateElement(newname);

            foreach (XmlNode childNode in oldRoot.ChildNodes)
            {
                newRoot.AppendChild(childNode.CloneNode(true));
            }
            var parent = oldRoot.ParentNode;
            if (parent == null) return;
            parent.AppendChild(newRoot);
            parent.RemoveChild(oldRoot);
        }

        public void CorrectXmlDocumentByVersion(XmlDocument doc)
        {
            _pluginDataToMove.Clear();

            // get version
            var vi = doc.SelectSingleNode("//Data//TLCGenVersie");

            if (vi == null || string.IsNullOrWhiteSpace(vi.InnerText)) return;

            var v = Version.Parse(vi.InnerText);

            // In version 0.2.2.0, the OVIngreepModel object was changed
            var checkVer = Version.Parse("0.2.2.0");
            if (v < checkVer)
            {
                var shownKarVecomMsg = false;
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName != "OVData") continue;
                    // Move dummy KAR dets to appropriate melding instead of ingreep
                    var ingrepen = node.SelectNodes("OVIngrepen/OVIngreep");
                    if (ingrepen == null) continue;
                    foreach (XmlNode ingreep in ingrepen)
                    {
                        var kar = ingreep.SelectSingleNode("KAR");
                        var vecom = ingreep.SelectSingleNode("Vecom");

                        if (kar != null && (kar.InnerText.ToLower() == "true" ||
                            vecom != null && vecom.InnerText.ToLower() == "true"))
                        {
                            if (!shownKarVecomMsg)
                            {
                                shownKarVecomMsg = true;
                                MessageBox.Show(
                                    "Dit is oud type TLCGen bestand. OV via KAR en/of VECOM moet opnieuw worden opgegeven.",
                                    "KAR en VECOM opnieuw invoeren.", MessageBoxButton.OK);
                            }
                        }
                    }
                }

                // Check detector type VecomIngang and rename to VecomDetector
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName == "Fasen")
                    {
                        foreach (XmlNode fase in node.ChildNodes)
                        {
                            if (node.LocalName != "Detectoren") continue;
                            foreach (XmlNode det in fase.ChildNodes)
                            {
                                var c = det.SelectSingleNode("Type");
                                if (c != null && c.InnerText == "VecomIngang")
                                {
                                    c.InnerText = "VecomDetector";
                                }
                            }
                        }
                    }
                    if (node.LocalName == "Detectoren")
                    {
                        foreach (XmlNode det in node.ChildNodes)
                        {
                            var c = det.SelectSingleNode("Type");
                            if (c != null && c.InnerText == "VecomIngang")
                            {
                                c.InnerText = "VecomDetector";
                            }
                        }
                    }
                }
            }
            checkVer = Version.Parse("0.5.4.0");
            if (v < checkVer)
            {
                // V0.5.4.0: RIS plugin moved inside TLCGen
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName == "RISData")
                    {
                        var risModel = XmlNodeConverter.ConvertNode<RISDataModel>(node);
                        _pluginDataToMove.Add(new Tuple<string, object>("RISData", risModel));
                        break;
                    }
                }

                // V0.5.4.0: property KruisingNaam on PelotonKoppelingModel changed to KoppelingNaam
                var item = doc.SelectSingleNode("//PelotonKoppelingenData//PelotonKoppelingen");
                if (item != null)
                {
                    var rowList = item.ChildNodes;
                    foreach (XmlNode n in rowList)
                    {
                        var c = n.SelectSingleNode("KruisingNaam");
                        if (c != null)
                        {
                            RenameXmlNode(doc, c, "KoppelingNaam");
                        }
                    }
                }

                // V0.5.4.0: property HalfstarFaseCyclusAlternatiefModel in HalfstarDataModel changed to HalfstarFaseCyclusInstellingenModel
                item = doc.SelectSingleNode("//HalfstarData//Alternatieven");
                if (item != null)
                {
                    var rowList = item.ChildNodes;
                    var newNodes = new List<XmlNode>();
                    for (var i = 0; i < rowList.Count; ++i)
                    {
                        XmlNode newRoot = doc.CreateElement("HalfstarFaseCyclusInstellingenModel");
                        foreach (XmlNode childNode in rowList[i].ChildNodes)
                        {
                            newRoot.AppendChild(childNode.CloneNode(true));
                        }
                        newNodes.Add(newRoot);
                    }
                    item.RemoveAll();
                    for (var i = 0; i < newNodes.Count; ++i)
                    {
                        item.AppendChild(newNodes[i]);
                    }
                    RenameXmlNode(doc, item, "FaseCyclusInstellingen");
                }
            }
            checkVer = Version.Parse("0.6.2.0");
            if (v < checkVer)
            {
                // V0.6.2.0: Den Haag specials plugin merged with TLCGen core
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName == "SpecialsDenHaag")
                    {
                        var data = @"<AlternatievenPerBlokModel>" + node.InnerXml + @"</AlternatievenPerBlokModel>";
                        using (TextReader reader = new StringReader(data))
                        {
                            var xs = new XmlSerializer(typeof(AlternatievenPerBlokModel));
                            var dhModel = xs.Deserialize(reader);
                            _pluginDataToMove.Add(new Tuple<string, object>("SpecialsDenHaagData", dhModel));
                        }
                        break;
                    }
                }
                // V0.6.2.0: Rotterdam specials plugin merged with TLCGen core
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName == "SpecialsRotterdam")
                    {
                        var dataRdam = new Dictionary<string, bool>();
                        foreach (XmlNode n in node.ChildNodes)
                        {
                            if (n.Name == "ToevoegenOVM") dataRdam.Add(n.Name, n.InnerText.ToLower() == "true");
                            if (n.Name == "PrmLoggingTfbMax") dataRdam.Add(n.Name, n.InnerText.ToLower() == "true");
                        }
                        _pluginDataToMove.Add(new Tuple<string, object>("SpecialsRotterdamData", dataRdam));
                        break;
                    }
                }
            }
            checkVer = Version.Parse("0.7.1.0");
            if (v < checkVer)
            {
                // V0.7.0.0: OV removed, PRIO added
                foreach (XmlNode node in doc.FirstChild.ChildNodes)
                {
                    if (node.LocalName == "OVData")
                    {
                        // Move dummy KAR dets to appropriate melding instead of ingreep
                        var ingrepen = node.SelectNodes("OVIngrepen/OVIngreep");
                        if (ingrepen == null) continue;
                        foreach (XmlNode ingreep in ingrepen)
                        {
                            var dummyIn = ingreep.SelectSingleNode("DummyKARInmelding");
                            var dummyUit = ingreep.SelectSingleNode("DummyKARUitmelding");
                            if (dummyIn != null)
                            {
                                ingreep.RemoveChild(dummyIn);
                                var inmeldingen = ingreep.SelectNodes("MeldingenData/Inmeldingen/Inmelding");
                                foreach (XmlNode inmelding in inmeldingen)
                                {
                                    if (inmelding.SelectSingleNode("Type").InnerText == "KARMelding")
                                    {
                                        var fase = ingreep.SelectSingleNode("FaseCyclus").InnerText;
                                        var type = ingreep.SelectSingleNode("Type").InnerText;
                                        if (Enum.TryParse(type, out PrioIngreepVoertuigTypeEnum eType))
                                        {
                                            var name = dummyIn.SelectSingleNode("Naam");
                                            name.InnerText = $"dummykarin{fase}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(eType)}";
                                            inmelding.AppendChild(dummyIn);
                                            RenameXmlNode(doc, dummyIn, "DummyKARMelding");
                                        }
                                        break;
                                    }
                                }
                            }
                            if (dummyUit != null)
                            {
                                ingreep.RemoveChild(dummyUit);
                                var uitmeldingen = ingreep.SelectNodes("MeldingenData/Uitmeldingen/Uitmelding");
                                foreach (XmlNode uitmelding in uitmeldingen)
                                {
                                    if (uitmelding.SelectSingleNode("Type").InnerText == "KARMelding")
                                    {
                                        var fase = ingreep.SelectSingleNode("FaseCyclus").InnerText;
                                        var type = ingreep.SelectSingleNode("Type").InnerText;
                                        if (Enum.TryParse(type, out PrioIngreepVoertuigTypeEnum eType))
                                        {
                                            var name = dummyUit.SelectSingleNode("Naam");
                                            name.InnerText = $"dummykaruit{fase}{DefaultsProvider.Default.GetVehicleTypeAbbreviation(eType)}";
                                            uitmelding.AppendChild(dummyUit);
                                            RenameXmlNode(doc, dummyUit, "DummyKARMelding");
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        var data = @"<PrioriteitDataModel>" + node.InnerXml + @"</PrioriteitDataModel>";
                        data = data.Replace("OV", "Prio");
                        data = data.Replace(">Uitgebreid<", ">GeneriekePrioriteit<");
                        using (TextReader reader = new StringReader(data))
                        {
                            var xs = new XmlSerializer(typeof(PrioriteitDataModel));
                            var prioModel = xs.Deserialize(reader);
                            _pluginDataToMove.Add(new Tuple<string, object>("PrioData", prioModel));
                        }
                        break;
                    }
                }
            }
        }

        public bool IsElementIdentifierUnique(TLCGenObjectTypeEnum objectType, string identifier, bool vissim = false)
        {
            return !vissim ? 
                TLCGenIntegrityChecker.IsElementNaamUnique(Controller, identifier, objectType) : 
                TLCGenIntegrityChecker.IsElementVissimNaamUnique(Controller, identifier);
        }
        
        public ObservableCollection<ControllerAlertMessage> ControllerAlerts { get; } = new ObservableCollection<ControllerAlertMessage>();

        public void AddControllerAlert(ControllerAlertMessage msg)
        {
            if (ControllerAlerts.All(x => x.Id != msg.Id))
            {
                msg.PropertyChanged += AlertMsgOnPropertyChanged;
                ControllerAlerts.Add(msg);
            }
        }

        public void RemoveControllerAlert(string id)
        {
            var alert = ControllerAlerts.FirstOrDefault(x => x.Id == id);
            if (alert != null)
            {
                ControllerAlerts.Remove(alert);
                alert.PropertyChanged -= AlertMsgOnPropertyChanged;
            }
        }

        public void UpdateControllerAlerts()
        {
            if (Controller == null)
            {
                foreach (var m in ControllerAlerts) m.PropertyChanged -= AlertMsgOnPropertyChanged;
                ControllerAlerts.Clear();
                return;
            }
            
            // wachttijdvoorspellers
            if (Controller.Fasen.Any(x => x.WachttijdVoorspeller))
            {
                if (ControllerAlerts.All(x => x.Type != ControllerAlertType.WachttijdVoorspeller))
                {
                    var msg = new ControllerAlertMessage(Guid.NewGuid().ToString())
                    {
                        Background = Brushes.Lavender,
                        Shown = true,
                        Message = "***Let op!*** De gegenereerde CCOL code voor wachttijdvoorspellers is momenteel uitsluitend geschikt voor test doeleinden.",
                        Type = ControllerAlertType.WachttijdVoorspeller
                    };
                    msg.PropertyChanged += AlertMsgOnPropertyChanged;
                    ControllerAlerts.Add(msg);
                }
            }
            else
            {
                var alert = ControllerAlerts.FirstOrDefault(x => x.Type == ControllerAlertType.WachttijdVoorspeller);
                if (alert != null)
                {
                    ControllerAlerts.Remove(alert);
                    alert.PropertyChanged -= AlertMsgOnPropertyChanged;
                }
            }

            // Rangeer elementen
            if (Controller.Data.RangeerData.RangerenOvergezet)
            {
                if (ControllerAlerts.All(x => x.Type != ControllerAlertType.RangerenOldNew))
                {
                    var msg = new ControllerAlertMessage(Guid.NewGuid().ToString())
                    {
                        Background = Brushes.BlanchedAlmond,
                        Shown = true,
                        Message = "***Let op!*** De oude rangeer elementen data is omgezet; controleer dit via tab Specials > Rangeren IO.",
                        Type = ControllerAlertType.RangerenOldNew
                    };
                    msg.PropertyChanged += AlertMsgOnPropertyChanged;
                    ControllerAlerts.Add(msg);
                }
            }
            else
            {
                var alert = ControllerAlerts.FirstOrDefault(x => x.Type == ControllerAlertType.RangerenOldNew);
                if (alert != null)
                {
                    ControllerAlerts.Remove(alert);
                    alert.PropertyChanged -= AlertMsgOnPropertyChanged;
                }
            }
            
            // Rangeer elementen
            if (Controller.Data.Intergroen && Controller.Fasen.Any(x => x.TGL != x.TGL_min))
            {
                if (ControllerAlerts.All(x => x.Type != ControllerAlertType.TglMinChanged))
                {
                    var msg = new ControllerAlertMessage(Guid.NewGuid().ToString())
                    {
                        Background = Brushes.Thistle,
                        Shown = true,
                        Message = "***Let op!*** Deze intergroen regeling heeft TGLmin tijden die afwijken van TGL. Pas TGLmin desgewenst aan in de tab.add.",
                        Type = ControllerAlertType.RangerenOldNew
                    };
                    msg.PropertyChanged += AlertMsgOnPropertyChanged;
                    ControllerAlerts.Add(msg);
                }
            }
            else
            {
                var alert = ControllerAlerts.FirstOrDefault(x => x.Type == ControllerAlertType.TglMinChanged);
                if (alert != null)
                {
                    ControllerAlerts.Remove(alert);
                    alert.PropertyChanged -= AlertMsgOnPropertyChanged;
                }
            }
        }

        private void AlertMsgOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shown")
            {
                UpdateControllerAlerts();
                ControllerAlertsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ControllerAlertsUpdated;

        #endregion // Public Methods

        #region TLCGen Messaging

        public void OnFasenChanging(FasenChangingMessage message)
        {
            if (message.AddedFasen != null)
            {
                foreach (var fcm in message.AddedFasen)
                {
                    Controller.Fasen.Add(fcm);

                    // PT Conflict prms
                    if (Controller.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                    {
                        var prms = new PrioIngreepSignaalGroepParametersModel();
                        _setDefaultsAction?.Invoke(prms, null);
                        prms.FaseCyclus = fcm.Naam;
                        Controller.PrioData.PrioIngreepSignaalGroepParameters.Add(prms);
                    }

                    // Module settings
                    var fcmlm = new FaseCyclusModuleDataModel() { FaseCyclus = fcm.Naam };
	                _setDefaultsAction?.Invoke(fcmlm, null);
                    Controller.ModuleMolen.FasenModuleData.Add(fcmlm);

                    // Green times
                    foreach (var set in Controller.GroentijdenSets)
                    {
                        var mgm = new GroentijdModel { FaseCyclus = fcm.Naam };
                        _setDefaultsAction?.Invoke(mgm, fcm.Type.ToString());
                        set.Groentijden.Add(mgm);
                    }
                }
            }
            if (message.RemovedFasen != null)
            {
                foreach (var fcm in message.RemovedFasen)
                {
                    Controller.Fasen.Remove(fcm);

                    // PT Conflict prms
                    if (Controller.PrioData.PrioIngreepType != PrioIngreepTypeEnum.Geen)
                    {
                        PrioIngreepSignaalGroepParametersModel _prms = null;
                        foreach (var prms in Controller.PrioData.PrioIngreepSignaalGroepParameters)
                        {
                            if (prms.FaseCyclus == fcm.Naam)
                            {
                                _prms = prms;
                            }
                        }
                        if (_prms != null)
                        {
                            Controller.PrioData.PrioIngreepSignaalGroepParameters.Remove(_prms);
                        }
                    }

                    // Module settings
                    FaseCyclusModuleDataModel fcvm = null;
                    foreach(var f in Controller.ModuleMolen.FasenModuleData)
                    {
                        if(fcm.Naam == f.FaseCyclus)
                        {
                            fcvm = f;
                        }
                    }
                    if (fcvm != null)
                    {
                        Controller.ModuleMolen.FasenModuleData.Remove(fcvm);
                    }

                    // Green times
                    foreach (var set in Controller.GroentijdenSets)
                    {
                        GroentijdModel mgm = null;
                        foreach (var mgvm in set.Groentijden)
                        {
                            if (mgvm.FaseCyclus == fcm.Naam)
                            {
                                mgm = mgvm;
                            }
                        }
                        if (mgm != null)
                        {
                            set.Groentijden.Remove(mgm);
                        }
                    }
                }
            }

            // Sorting
            Controller.PrioData.PrioIngreepSignaalGroepParameters.BubbleSort();
            foreach (var set in Controller.GroentijdenSets)
            {
                set.Groentijden.BubbleSort();
            }
            Controller.ModuleMolen.FasenModuleData.BubbleSort();

            // Messaging
            MessengerInstance.Send(new FasenChangedMessage(message.AddedFasen, message.RemovedFasen));
        }

        private void OnNameChanging(NameChangingMessage msg)
        {
            ChangeNameOnObject(Controller, msg.OldName, msg.NewName, msg.ObjectType);
            if (msg.ObjectType == TLCGenObjectTypeEnum.Fase)
            {
                Controller.Fasen.Sort();
            }
            MessengerInstance.Send(new NameChangedMessage(msg.ObjectType, msg.OldName, msg.NewName));

            // Force the viewmodel to order+rebuild relevant lists
            if (msg.ObjectType == TLCGenObjectTypeEnum.Fase)
            {
                MessengerInstance.Send(new FasenChangedMessage(null, null));
            }
        }

        public int ChangeNameOnObject(object obj, string oldName, string newName, TLCGenObjectTypeEnum objectType)
        {
            var i = 0;
            if (obj == null) return i;
            var objType = obj.GetType();

            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var ignore = (TLCGenIgnoreAttribute)property.GetCustomAttribute(typeof(TLCGenIgnoreAttribute));
                if (ignore != null) continue;

                var propValue = property.GetValue(obj);

                // for strings
                if (property.PropertyType == typeof(string))
                {
                    var strRefToAttr = property.GetCustomAttribute<RefersToAttribute>();

                    if (strRefToAttr == null) continue;
                    
                    var refObjectType = strRefToAttr.ObjectType;
                    // if applicable, find actual object type
                    if (strRefToAttr.ObjectTypeProperty != null)
                    {
                        var objTypeProp = properties.FirstOrDefault(x => x.Name == strRefToAttr.ObjectTypeProperty);
                        if (objTypeProp != null && objTypeProp.PropertyType == typeof(TLCGenObjectTypeEnum))
                        {
                            refObjectType = (TLCGenObjectTypeEnum) objTypeProp.GetValue(obj);
                        }
                    }
                    // set new value if applicable (do not change NULL values)
                    if (!string.IsNullOrEmpty(oldName) &&
                        objectType == refObjectType &&
                        (string) propValue == oldName)
                    {
                        property.SetValue(obj, newName);
                        ++i;
                    }
                }
                // for lists
                else if (propValue is IList elems)
                {
                    foreach (var item in elems)
                    {
                        ChangeNameOnObject(item, oldName, newName, objectType);
                    }
                }
                // for objects
                else if(!property.PropertyType.IsValueType)
                {
                    ChangeNameOnObject(propValue, oldName, newName, objectType);
                }
            }
            return i;
        }

        public void OnModelManagerMessage(ModelManagerMessageBase msg)
        {
            PrepareModelForUI(Controller);
            switch (msg)
            {
                case PrioIngreepMeldingChangedMessage meldingMsg:
                    var ovi = Controller.PrioData.PrioIngrepen.FirstOrDefault(
                        x =>
                            x.MeldingenData.Inmeldingen.Any(x2 => ReferenceEquals(x2, meldingMsg.IngreepMelding)) ||
                            x.MeldingenData.Uitmeldingen.Any(x2 => ReferenceEquals(x2, meldingMsg.IngreepMelding)));
                   
                    if (meldingMsg.IngreepMelding != null)
                    {
                        if (meldingMsg.IngreepMelding.Type ==
                            PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding)
                        {
                            switch (meldingMsg.IngreepMelding.InUit)
                            {
                                case PrioIngreepInUitMeldingTypeEnum.Inmelding:
                                    if (meldingMsg.Removing)
                                    {
                                        meldingMsg.IngreepMelding.DummyKARMelding = null;
                                    }
                                    else if (meldingMsg.IngreepMelding.DummyKARMelding == null)
                                    {
                                        meldingMsg.IngreepMelding.DummyKARMelding = new DetectorModel()
                                        {
                                            Dummy = true,
                                            Naam =
                                                $"dummykarin{ovi.FaseCyclus}{ovi.Naam}"
                                        };
                                    }

                                    break;
                                case PrioIngreepInUitMeldingTypeEnum.Uitmelding:
                                    if (meldingMsg.Removing)
                                    {
                                        meldingMsg.IngreepMelding.DummyKARMelding = null;
                                    }
                                    else if (meldingMsg.IngreepMelding.DummyKARMelding == null)
                                    {
                                        meldingMsg.IngreepMelding.DummyKARMelding = new DetectorModel()
                                        {
                                            Dummy = true,
                                            Naam =
                                                $"dummykaruit{ovi.FaseCyclus}{ovi.Naam}"
                                        };
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                        else
                        {
                            meldingMsg.IngreepMelding.DummyKARMelding = null;
                        }
                    }
                    SetSpecialIOPerSignalGroup(Controller);
                    break;
                case ModulesChangedMessage modulesMessage:
                    if (Controller.Data.UitgangPerModule)
                    {
                        if (!Controller.Data.MultiModuleReeksen)
                        {
                            foreach (var m in Controller.ModuleMolen.Modules.Where(m =>
                                Controller.Data.ModulenDisplayBitmapData.All(x => x.Naam != m.Naam)))
                            {
                                Controller.Data.ModulenDisplayBitmapData.Add(new ModuleDisplayElementModel
                                {
                                    Naam = m.Naam
                                });
                                Controller.Data.ModulenDisplayBitmapData.BubbleSort();
                            }
                        }
                        else
                        {
                            foreach (var m in Controller.MultiModuleMolens.SelectMany(x => x.Modules))
                            {
                                if (Controller.Data.ModulenDisplayBitmapData.Any(x => x.Naam == m.Naam)) continue;
                                Controller.Data.ModulenDisplayBitmapData.Add(new ModuleDisplayElementModel
                                {
                                    Naam = m.Naam
                                });
                                Controller.Data.ModulenDisplayBitmapData.BubbleSort();
                            }
                        }

                        var rd = new List<ModuleDisplayElementModel>();
                        foreach (var md in Controller.Data.ModulenDisplayBitmapData)
                        {
                            if (!Controller.Data.MultiModuleReeksen)
                            {
                                if (Controller.ModuleMolen.Modules.All(x => x.Naam != md.Naam))
                                {
                                    rd.Add(md);
                                }
                            }
                            else
                            {
                                if (Controller.MultiModuleMolens.SelectMany(x => x.Modules).All(x => x.Naam != md.Naam))
                                {
                                    rd.Add(md);
                                }
                            }
                        }

                        foreach (var r in rd)
                        {
                            Controller.Data.ModulenDisplayBitmapData.Remove(r);
                        }
                    }
                    break;
            }
        }

        private void OnPrepareForGenerationRequest(PrepareForGenerationRequest msg)
        {
            PrepareModelForUI(msg.Controller);

            foreach (var fcm in msg.Controller.Fasen)
            {
                fcm.Detectoren.BubbleSort();
            }

            msg.Controller.Detectoren.BubbleSort();
            msg.Controller.SelectieveDetectoren.BubbleSort();
        }

        private void OnDetectorenChangedMessage(DetectorenChangedMessage msg)
        {
            if (msg.RemovedDetectoren == null || !msg.RemovedDetectoren.Any()) return;

            foreach (var d in msg.RemovedDetectoren.Where(x => x.Type == DetectorTypeEnum.OpticomIngang))
            {
                foreach (var hd in msg.Controller.PrioData.HDIngrepen.Where(hd => hd.Opticom && hd.OpticomRelatedInput == d.Naam))
                {
                    hd.Opticom = false;
                    hd.OpticomRelatedInput = null;
                }
            }
        }

        private void OnFaseDetectorTypeChangedMessage(FaseDetectorTypeChangedMessage msg)
        {
            if (msg.OldType != DetectorTypeEnum.OpticomIngang || msg.NewType == DetectorTypeEnum.OpticomIngang) return;

            foreach (var hd in msg.Controller.PrioData.HDIngrepen.Where(hd => hd.Opticom && hd.OpticomRelatedInput == msg.DetectorDefine))
            {
                hd.Opticom = false;
                hd.OpticomRelatedInput = null;
            }
        }

        #endregion // TLCGen Messaging

        #region Constructor

        public TLCGenModelManager(IMessenger messengerinstance = null)
        {
            if(messengerinstance == null)
            {
                MessengerInstance = Messenger.Default;
            }
            MessengerInstance.Register(this, new Action<FasenChangingMessage>(OnFasenChanging));
            MessengerInstance.Register(this, new Action<NameChangingMessage>(OnNameChanging));
            MessengerInstance.Register(this, true, new Action<ModelManagerMessageBase>(OnModelManagerMessage));
            MessengerInstance.Register(this, new Action<PrepareForGenerationRequest>(OnPrepareForGenerationRequest));
            MessengerInstance.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChangedMessage));
            MessengerInstance.Register(this, new Action<FaseDetectorTypeChangedMessage>(OnFaseDetectorTypeChangedMessage));
        }

        #endregion // Constructor
    }
}
