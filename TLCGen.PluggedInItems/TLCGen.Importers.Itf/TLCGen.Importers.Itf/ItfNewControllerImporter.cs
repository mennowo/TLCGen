using System;
using System.IO;
using System.Linq;
using System.Windows.Media;
using CodingConnected.Topology;
using Microsoft.Win32;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.Importers.Itf
{
    [TLCGenPlugin(TLCGenPluginElems.Importer)]
    public class ItfNewControllerImporter : ITLCGenImporter
    {
        public ControllerModel Controller
        {
            get => throw new NotSupportedException();

            set
            {
                
            }
        }

        public bool ImportsIntoExisting => false;

	    public string Name => "Importeer itf (in nieuwe regeling)";

	    public string GetPluginName()
        {
            return Name;
        }

        public ControllerModel ImportController(ControllerModel c = null)
        {
            if (c != null)
            {
                throw new NullReferenceException("ITF importer: Controller parsed is not null, which it should be for importing into new.");
            }

			var openFileDialog = new OpenFileDialog
			{
				CheckFileExists = true,
				Title = "Selecteer ITF xml file voor importeren",
				Filter = "ITF files|*.xml|Alle files|*.*"
			};

			var newc = new ControllerModel();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    DefaultsProvider.Default.SetDefaultsOnModel(newc.Data);
                    newc.Data.GarantieOntruimingsTijden = false;

                    var lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length <= 1)
                        throw new IndexOutOfRangeException("Het bestand heeft minder dan 2 regels.");


                    var t = TopologyReader.ReadTopologyFile(openFileDialog.FileName);
                    var signalGroups = TopologyReader.GetSignalGroups(t);
                    var conflicts = TopologyReader.GetSignalGroupConflicts(t);
                    var lanes = TopologyReader.GetSignalGroupLanes(t);

                    foreach (var sg in signalGroups)
                    {
                        var fc = new FaseCyclusModel {Naam = sg.Name};
                        fc.Type = Settings.Utilities.FaseCyclusUtilities.GetFaseTypeFromNaam(fc.Naam);
                        DefaultsProvider.Default.SetDefaultsOnModel(fc, fc.Type.ToString());
                        fc.TGL_min = (int) sg.MinAmberTime;
                        fc.TGL = (int) sg.MinAmberTime;
                        fc.TGG_min = (int) sg.MinGreenTime;
                        fc.TGG = (int) sg.MinGreenTime;
                        fc.TRG_min = (int) sg.MinRedTime;
                        fc.TRG = (int) sg.MinRedTime;
                        newc.Fasen.Add(fc);
                    }

                    var intergreen = true;
                    foreach (var conflict in conflicts)
                    {
                        if (conflict.ConflictType != SignalGroupConflictType.IntergreenTime) intergreen = false;
                        var fcTopoFrom = signalGroups.FirstOrDefault(x => x.TopologyId == conflict.SignalGroupFrom);
                        var fcTopoTo = signalGroups.FirstOrDefault(x => x.TopologyId == conflict.SignalGroupTo);
                        if (fcTopoTo != null && fcTopoFrom != null)
                        {
                            var fcFrom = newc.Fasen.FirstOrDefault(x => x.Naam == fcTopoFrom.Name);
                            var fcTo = newc.Fasen.FirstOrDefault(x => x.Naam == fcTopoTo.Name);
                            if (fcTo != null && fcFrom != null)
                            {
                                newc.InterSignaalGroep.Conflicten.Add(new ConflictModel
                                {
                                    FaseVan = fcFrom.Naam,
                                    FaseNaar = fcTo.Naam,
                                    GarantieWaarde = (int)conflict.Value,
                                    Waarde = (int)conflict.Value
                                });
                            }
                        }
                    }

                    newc.Data.GarantieOntruimingsTijden = true;
                    newc.Data.Intergroen = intergreen;

                    //if (lanes.Any())
                    //{
                    //    foreach (var fc in newc.Fasen)
                    //    {
                    //        newc.RISData.RISFasen.Add(new RISFaseCyclusDataModel
                    //        {
                    //            FaseCyclus = fc.Naam
                    //        });
                    //    }
                    //    foreach (var lane in lanes)
                    //    {
                    //        var topoSg = signalGroups.FirstOrDefault(x => x.TopologyId == lane.SignalGroup);
                    //        if (topoSg != null)
                    //        {
                    //            var sg = newc.RISData.RISFasen.FirstOrDefault(x => x.FaseCyclus == topoSg.Name);
                    //            if (sg != null && int.TryParse(lane.LaneId, out var laneId))
                    //            {
                    //                var sgLaneId = sg.LaneData.Count;
                    //                sg.LaneData.Add(new RISFaseCyclusLaneDataModel
                    //                {
                    //                    LaneID = laneId,
                    //                    RijstrookIndex = sgLaneId,
                    //                    SignalGroupName = sg.FaseCyclus
                    //                });
                    //            }
                    //        }
                    //    }
                    //}

                    return newc;
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show("Fout bij uitlezen ITF data:\n" + e.Message, "Fout bij importeren ITF bestand");
                    return null;
                }
            }
            return null;
        }
    }
}
