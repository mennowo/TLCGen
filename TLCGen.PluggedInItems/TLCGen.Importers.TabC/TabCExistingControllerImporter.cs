using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TLCGen.Dependencies.Providers;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.Importers.TabC
{
    [TLCGenPlugin(TLCGenPluginElems.Importer)]
    public class TabCExistingControllerImporter : ITLCGenImporter
    {
        public ControllerModel Controller
        {
            get => throw new NotSupportedException();

            set
            {
                
            }
        }

        public bool ImportsIntoExisting => true;

	    public string Name => "Importeer tab.c (in geopende regeling)";

	    public string GetPluginName()
        {
            return Name;
        }

        public ControllerModel ImportController(ControllerModel c = null)
        {
            if(c == null)
            {
                throw new NullReferenceException("TabC importer: Controller to import into cannot be null.");
            }

            if (TLCGenDialogProvider.Default
                .ShowOpenFileDialog(
                    "Selecteer tab.c file voor importeren", 
                    "tab.c files|*tab.c;*.ccol|Alle files|*.*", true, out var filename))
            {
                try
                {
                    var lines = TLCGenFileAccessProvider.Default.ReadAllLines(filename);

                    // Build a list of the Phases with conflicts from the tab.c file
                    var newData = TabCImportHelper.GetNewData(lines, false);
                    var allPhasesMessage = "";

                    var result = MessageBoxResult.Yes;
                    if(newData.Intergroen && !c.Data.Intergroen)
                    {
                        result = TLCGenDialogProvider.Default.ShowMessageBox("De geïmporteerde data bevat intergroentijden, de regeling NIET.\n\nDoorgaan?", "Intergroen tijden", MessageBoxButton.YesNo);
                        if(result == MessageBoxResult.Yes)
                        {
                            c.Data.Intergroen = true;
                            if (c.Data.CCOLVersie < Models.Enumerations.CCOLVersieEnum.CCOL95)
                            {
                                c.Data.CCOLVersie = Models.Enumerations.CCOLVersieEnum.CCOL95;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    // Find Phases not present in current data
                    var newfcs = new List<FaseCyclusModel>();
                    foreach (var fcm in c.Fasen.Where(fcm => newData.Fasen.All(x => x.Naam != fcm.Naam)))
                    {
                        allPhasesMessage = allPhasesMessage + fcm.Naam + "\n";
                        newfcs.Add(fcm);
                    }
                    if (!string.IsNullOrEmpty(allPhasesMessage))
                    {
                        result = TLCGenDialogProvider.Default.ShowMessageBox("Niet alle fasen uit de regeling komen voor in de tab.c file.\nConflicten van de volgende fasen worden verwijderd:\n\n" +
                            allPhasesMessage + "\nDoorgaan?", "Niet alle fasen gevonden", MessageBoxButton.YesNo);
                    }

                    // Continue...
                    if (result == MessageBoxResult.Yes)
                    {
                        // Clear conflicts from Phases not in tab.c file
                        var tempcs = new List<ConflictModel>();
                        foreach (var cm in c.InterSignaalGroep.Conflicten)
                        {
                            if(newfcs.Any(x => x.Naam == cm.FaseVan || x.Naam == cm.FaseNaar))
                            {
                                tempcs.Add(cm);
                            }
                        }
                        foreach(var tc in tempcs)
                        {
                            c.InterSignaalGroep.Conflicten.Remove(tc);
                        }

                        // Copy the results into the ControllerVM
                        var NewPhasesMessage = "";

                        // Store current conflicts
                        var OldConflicts = new List<ConflictModel>();
                        foreach (var _cm in c.InterSignaalGroep.Conflicten)
                            OldConflicts.Add(_cm);

                        foreach (var newfcm in newData.Fasen)
                        {
                            // Search for existing phases
	                        var fc = c.Fasen.FirstOrDefault(x => x.Naam == newfcm.Naam);
                            if (fc == null)
                            {
                                newfcm.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(newfcm.Naam);
                                DefaultsProvider.Default.SetDefaultsOnModel(newfcm, newfcm.Type.ToString());
                                c.Fasen.Add(newfcm);
                                c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel { FaseCyclus = newfcm.Naam });
                                NewPhasesMessage = NewPhasesMessage + newfcm.Naam + "\n";
                            }
                            else
                            {
	                            foreach (var nd in newfcm.Detectoren)
	                            {
		                            if (fc.Detectoren.All(x => x.Naam != nd.Naam))
		                            {
										fc.Detectoren.Add(nd);
		                            }
	                            }
                                if (newData.Intergroen)
                                {
                                    if (fc.TGL_min != newfcm.TGL_min)
                                    {
                                        fc.TGL_min = newfcm.TGL_min;
                                        if (fc.TGL > fc.TGL_min) fc.TGL = fc.TGL_min;
                                    }
                                    if (fc.TGL != newfcm.TGL && newfcm.TGL >= fc.TGL_min) fc.TGL = newfcm.TGL;
                                }
                            }
                        }

                        // find and remove old conflicts
                        var remConflicts = new List<ConflictModel>();
                        foreach(var cm in c.InterSignaalGroep.Conflicten)
                        {
                            if(!newData.Conflicten.Any(x => x.FaseVan == cm.FaseVan && x.FaseNaar == cm.FaseNaar))
                            {
                                remConflicts.Add(cm);
                            }
                        }
                        if (remConflicts.Any())
                        {
                            var ok = TLCGenDialogProvider.Default.ShowMessageBox(
                                "Er zijn conflicten VERWIJDERD uit de regeling. Controleer of dit klopt:\n\n" + string.Join("\n", remConflicts.Select(x => x.FaseVan + " > " + x.FaseNaar + " [waarde: " + x.SerializedWaarde + "]")) + "\n\nDoorgaan?",
                                "Conflicten verwijderd", MessageBoxButton.YesNo);
                            if (ok == MessageBoxResult.No) return null;
                            foreach (var r in remConflicts) c.InterSignaalGroep.Conflicten.Remove(r);
                        }

                        var conflictsChanged = false;
                        var newConflicts = new List<ConflictModel>();

                        foreach(var cm in newData.Conflicten)
                        {
                            var _cm = new ConflictModel
                            {
                                FaseVan = cm.FaseVan,
                                FaseNaar = cm.FaseNaar,
                                Waarde = cm.Waarde,
                                GarantieWaarde = cm.GarantieWaarde
                            };
                            var old = c.InterSignaalGroep.Conflicten.FirstOrDefault(x => x.FaseVan == cm.FaseVan && x.FaseNaar == cm.FaseNaar);
                            if (old != null)
                            {
                                c.InterSignaalGroep.Conflicten.Remove(old);
                            }
                            else
                            {
                                // Corrigeren VA ontruimen
                                var vaont = c.VAOntruimenFasen.FirstOrDefault(x => x.FaseCyclus == _cm.FaseVan);
                                if (vaont != null)
                                {
                                    foreach (var d in vaont.VADetectoren)
                                    {
                                        d.ConflicterendeFasen.Add(new VAOntruimenNaarFaseModel
                                        {
                                            FaseCyclus = _cm.FaseNaar,
                                            VAOntruimingsTijd = 0
                                        });
                                    }
                                }
                                // new conflict found
                                newConflicts.Add(cm);
                                conflictsChanged = true;
                            }

                            c.InterSignaalGroep.Conflicten.Add(_cm);
                        }
                        // correct guaranteed
                        c.Data.GarantieOntruimingsTijden = false;
                        if (newData.Garantie && !c.Data.GarantieOntruimingsTijden)
                        {
                            if(c.InterSignaalGroep.Conflicten.Any(x => x.GarantieWaarde == null))
                            {
                                TLCGenDialogProvider.Default.ShowMessageBox(
                                "Er zijn conflicten zonder garantie tijden gevonden in nieuwe data:\n\n" +
                                string.Join("\n", c.InterSignaalGroep.Conflicten.Where(x => x.GarantieWaarde == null).Select(x => x.FaseVan + " => " + x.FaseNaar)) +
                                "\n\nGarantie tijden worden uitgeschakeld.",
                                "Fout in garantie tijden", MessageBoxButton.OK);
                            }
                            else
                            {
                                c.Data.GarantieOntruimingsTijden = true;
                            }
                        }
                        if (conflictsChanged)
                        {
                            // Corrigeren modules
                            foreach (var m in c.ModuleMolen.Modules)
                            {
                                var rmfcs = new List<ModuleFaseCyclusModel>();
                                var br = false;
                                foreach (var mfc1 in m.Fasen)
                                {
                                    foreach (var mfc2 in m.Fasen)
                                    {
                                        if(Integrity.TLCGenControllerChecker.IsFasenConflicting(c, mfc1.FaseCyclus, mfc2.FaseCyclus))
                                        {
                                            rmfcs.Add(mfc1);
                                            rmfcs.Add(mfc2);
                                            br = true;
                                            break;
                                        }
                                    }
                                    if (br) break;
                                }
                                foreach(var rmfc in rmfcs)
                                {
                                    m.Fasen.Remove(rmfc);
                                }
                            }

                            // Corrigeren synchronisaties
                            var remgs = new List<GelijkstartModel>();
                            var remvs = new List<VoorstartModel>();
                            var remnl = new List<NaloopModel>();
                            var remma = new List<MeeaanvraagModel>();
                            foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                            {
                                if (Integrity.TLCGenControllerChecker.IsFasenConflicting(c, gs.FaseVan, gs.FaseNaar))
                                {
                                    remgs.Add(gs);
                                }
                            }
                            foreach (var vs in c.InterSignaalGroep.Voorstarten)
                            {
                                if (Integrity.TLCGenControllerChecker.IsFasenConflicting(c, vs.FaseVan, vs.FaseNaar))
                                {
                                    remvs.Add(vs);
                                }
                            }
                            foreach (var nl in c.InterSignaalGroep.Nalopen)
                            {
                                if (Integrity.TLCGenControllerChecker.IsFasenConflicting(c, nl.FaseVan, nl.FaseNaar))
                                {
                                    remnl.Add(nl);
                                }
                            }
                            foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                            {
                                if (Integrity.TLCGenControllerChecker.IsFasenConflicting(c, ma.FaseVan, ma.FaseNaar))
                                {
                                    remma.Add(ma);
                                }
                            }
                            foreach (var r in remgs) c.InterSignaalGroep.Gelijkstarten.Remove(r);
                            foreach (var r in remvs) c.InterSignaalGroep.Voorstarten.Remove(r);
                            foreach (var r in remnl) c.InterSignaalGroep.Nalopen.Remove(r);
                            foreach (var r in remma) c.InterSignaalGroep.Meeaanvragen.Remove(r);



                            TLCGenDialogProvider.Default.ShowMessageBox(
                                "Er zijn nieuwe conflicten gevonden in de regeling:\n\n" +
                                string.Join("\n", newConflicts.Select(x => x.FaseVan + " => " + x.FaseNaar)) +
                                "\n\nLoop de module molen na, en evt. synchronisaties, RoBuGrover en VA ontruimen",
                                "Nieuwe conflicten gevonden", MessageBoxButton.OK);
                        }

                        if (!string.IsNullOrEmpty(NewPhasesMessage))
                        {
                            TLCGenDialogProvider.Default.ShowMessageBox("De volgende fasen uit de tab.c file zijn nieuw toegevoegd in de regeling:\n\n" +
                                NewPhasesMessage, "Nieuwe fasen toegevoegd", MessageBoxButton.OK);
                        }
                        return c;
                    }
                    else
                        return null;
                }
                catch (Exception e)
                {
                    TLCGenDialogProvider.Default.ShowMessageBox("Fout bij uitlezen tab.c.:\n" + e.Message, "Fout bij importeren tab.c", MessageBoxButton.OK);
                    return null;
                }
            }

            return null;
        }
    }
}
