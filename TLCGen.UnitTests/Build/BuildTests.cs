using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TLCGen.Generators.CCOL;
using TLCGen.Generators.CCOL.ProjectGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.UnitTests.Build
{
    [SingleThreaded]
    [TestFixture]
    public class BuildTests
    {
        private ControllerModel GetEmptyController()
        {
            var c = new ControllerModel();

            c.Data.SetSegmentOutputs();
            c.Data.Naam = "TEST";
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL100;
            c.Data.Intergroen = true;
            
            var groenSet = new GroentijdenSetModel {Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG1"};
            c.GroentijdenSets.Add(groenSet);
            groenSet = new GroentijdenSetModel {Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG2"};
            c.GroentijdenSets.Add(groenSet);

            
            c.PeriodenData.DefaultPeriodeNaam = "dal";
            c.PeriodenData.DefaultPeriodeGroentijdenSet = "MG1";

            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                StartTijd = new TimeSpan(0, 0, 0),
                EindTijd = new TimeSpan(24, 0, 0), 
                Naam = "dag", 
                GroentijdenSet = "MG1", 
                Type = PeriodeTypeEnum.Groentijden,
                DagCode = PeriodeDagCodeEnum.AlleDagen
            });

            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                StartTijd = new TimeSpan(6, 0, 0),
                EindTijd = new TimeSpan(9, 0, 0), 
                Naam = "ochtend", 
                GroentijdenSet = "MG2", 
                Type = PeriodeTypeEnum.Groentijden,
                DagCode = PeriodeDagCodeEnum.Werkdagen
            });

            return c;
        }

        private static void AddFaseToController(ControllerModel c, string naam, FaseTypeEnum type, int ml)
        {
            var fc = new FaseCyclusModel
            {
                Naam = naam, Type = type,
            };

            switch (fc.Type)
            {
                case FaseTypeEnum.Auto:
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "1", Type = DetectorTypeEnum.Kop
                    });
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "2", Type = DetectorTypeEnum.Lang
                    });
                    break;
                case FaseTypeEnum.Fiets:
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "1", Type = DetectorTypeEnum.Kop
                    });
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "2", Type = DetectorTypeEnum.Verweg
                    });
                    
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = "k" + naam, Type = DetectorTypeEnum.Knop
                    });
                    break;
                case FaseTypeEnum.Voetganger:
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "1", Type = DetectorTypeEnum.KnopBuiten
                    });
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "2", Type = DetectorTypeEnum.KnopBinnen
                    });
                    break;
                case FaseTypeEnum.OV:
                    fc.Detectoren.Add(new DetectorModel
                    {
                        Naam = naam + "1", Type = DetectorTypeEnum.Kop
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var i = Math.Max(1, c.ModuleMolen.Modules.Count);
            while (ml > c.ModuleMolen.Modules.Count - 1)
            {
                c.ModuleMolen.Modules.Add(new ModuleModel{Naam = "ML" + i});
                ++i;
            }
            c.ModuleMolen.Modules[ml].Fasen.Add(new ModuleFaseCyclusModel{FaseCyclus = naam});

            c.Fasen.Add(fc);
        }

        private ControllerModel GetBasicController()
        {
            var c = new ControllerModel();
            
            c.Data.SetSegmentOutputs();
            c.Data.Naam = "TEST";
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL100;
            c.Data.Intergroen = true;

            c.Fasen.Add(new FaseCyclusModel
            {
                Naam = "02", Type = FaseTypeEnum.Auto,
            });
            c.Fasen.Add(new FaseCyclusModel
            {
                Naam = "05", Type = FaseTypeEnum.Auto,
            });

            c.Fasen[0].Detectoren.Add(new DetectorModel
            {
                Naam = "021", Type = DetectorTypeEnum.Kop
            });
            c.Fasen[0].Detectoren.Add(new DetectorModel
            {
                Naam = "022", Type = DetectorTypeEnum.Lang
            });
            
            c.Fasen[1].Detectoren.Add(new DetectorModel
            {
                Naam = "051", Type = DetectorTypeEnum.Kop
            });
            c.Fasen[1].Detectoren.Add(new DetectorModel
            {
                Naam = "052", Type = DetectorTypeEnum.Lang
            });

            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "02", FaseNaar = "05", Waarde = 20, GarantieWaarde = 20});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "05", FaseNaar = "02", Waarde = 50, GarantieWaarde = 50});

            c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel{FaseCyclus = "02"});
            c.ModuleMolen.FasenModuleData.Add(new FaseCyclusModuleDataModel{FaseCyclus = "05"});
            c.ModuleMolen.LangstWachtendeAlternatief = true;
            c.ModuleMolen.WachtModule = "ML1";
            c.ModuleMolen.Modules.Add(
                new ModuleModel
                    { 
                        Naam = "ML1", 
                        Fasen = new List<ModuleFaseCyclusModel>
                        {
                            new ModuleFaseCyclusModel{ FaseCyclus = "02" }
                        }
                    });
            c.ModuleMolen.Modules.Add(
                new ModuleModel
                { 
                    Naam = "ML2", 
                    Fasen = new List<ModuleFaseCyclusModel>
                    {
                        new ModuleFaseCyclusModel{ FaseCyclus = "05" }
                    }
                });

            var groenSet = new GroentijdenSetModel {Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG1"};
            groenSet.Groentijden.Add(new GroentijdModel{ FaseCyclus = "02", Waarde = 300 });
            groenSet.Groentijden.Add(new GroentijdModel{ FaseCyclus = "05", Waarde = 300 });
            c.GroentijdenSets.Add(groenSet);
            groenSet = new GroentijdenSetModel {Type = GroentijdenTypeEnum.MaxGroentijden, Naam = "MG2"};
            groenSet.Groentijden.Add(new GroentijdModel{ FaseCyclus = "02", Waarde = 200 });
            groenSet.Groentijden.Add(new GroentijdModel{ FaseCyclus = "05", Waarde = 500 });
            c.GroentijdenSets.Add(groenSet);

            c.PeriodenData.DefaultPeriodeNaam = "dal";
            c.PeriodenData.DefaultPeriodeGroentijdenSet = "MG1";

            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                StartTijd = new TimeSpan(0, 0, 0),
                EindTijd = new TimeSpan(24, 0, 0), 
                Naam = "dag", 
                GroentijdenSet = "MG1", 
                Type = PeriodeTypeEnum.Groentijden,
                DagCode = PeriodeDagCodeEnum.AlleDagen
            });

            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                StartTijd = new TimeSpan(6, 0, 0),
                EindTijd = new TimeSpan(9, 0, 0), 
                Naam = "ochtend", 
                GroentijdenSet = "MG2", 
                Type = PeriodeTypeEnum.Groentijden,
                DagCode = PeriodeDagCodeEnum.Werkdagen
            });

            return c;
        }

        private CCOLGeneratorSettingsModel GetBasicSettings()
        {
            return TLCGenSerialization.DeSerializeData<CCOLGeneratorSettingsModel>(
                ResourceReader.GetResourceTextFile("TLCGen.Generators.CCOL.Settings.ccolgendefaults.xml", this, typeof(CCOLGeneratorSettingsProvider)));
        }

        private static readonly string MsBuildPath = Path.Combine(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin", "MSBuild.exe");

        private void GenerateController(string path, ControllerModel c)
        {
            var file = Path.Combine(path, "testGen1.tlc");
            
            CcolGen.ControllerFileName = file;
            CcolGen.Controller = c;
            CcolGen.GenerateController();
            var visualGen = new CCOLVisualProjectGenerator();
            visualGen.GenerateVisualStudioProjectFiles(CcolGen, "Visual_2017", 2017);
            TLCGenSerialization.Serialize(file, c);
        }

        private static Process BuildController(string path, ICollection<string> output)
        {
            var start = new ProcessStartInfo();
            var p = new Process();
            start.Arguments = " \"" + Path.Combine(path, "TEST_Visual_2017.vcxproj") + "\"";
            start.FileName = MsBuildPath;
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            p.StartInfo = start;
            p.OutputDataReceived += (o, e) => output.Add(e.Data);
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit(30000);

            if (p.ExitCode != 0 && Directory.Exists(path))
            {
                File.WriteAllText(Path.Combine(path, "output.txt"), string.Join(Environment.NewLine, output));
            }

            return p;
        }

        private static CCOLCodeGeneratorPlugin CcolGen = new CCOLCodeGeneratorPlugin(true);

        [OneTimeSetUp]
        public void Setup()
        {
            CCOLGeneratorSettingsProvider.Default.Settings = GetBasicSettings();
            CcolGen.LoadSettings();
        }

        [Test]
        public void SimpleController_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basis";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            
            var c = GetBasicController();

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }

        [Test]
        public void SimpleControllerCCOL9_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisCCOL9";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            
            var c = GetBasicController();
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL9;
            c.Data.Intergroen = false;

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }
        
        [Test]
        public void SimpleControllerWithPrio_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisMetPrio";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            
            var c = GetBasicController();
            
            c.PrioData.PrioIngreepType = PrioIngreepTypeEnum.GeneriekePrioriteit;
            var ingreep = new PrioIngreepModel
            {
                FaseCyclus = "02",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };
            ingreep.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding
            });
            ingreep.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding
            });
            c.PrioData.PrioIngrepen.Add(ingreep);
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "02" });
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "05" });

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }

        
        [Test]
        public void SimpleControllerWithPrioCCOL9_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisMetPrio";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            
            var c = GetBasicController();
            c.Data.CCOLVersie = CCOLVersieEnum.CCOL9;
            c.Data.Intergroen = false;

            c.PrioData.PrioIngreepType = PrioIngreepTypeEnum.GeneriekePrioriteit;
            var ingreep = new PrioIngreepModel
            {
                FaseCyclus = "02",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };
            ingreep.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding
            });
            ingreep.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding
            });
            c.PrioData.PrioIngrepen.Add(ingreep);
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "02" });
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "05" });

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }
        
        [Test]
        public void SimpleControllerWithPrioAndNevenMelding_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisMetPrioEnNevenMelding";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            
            var c = GetEmptyController();
            AddFaseToController(c, "02", FaseTypeEnum.Auto, 0);
            AddFaseToController(c, "05", FaseTypeEnum.Auto, 1);
            AddFaseToController(c, "41", FaseTypeEnum.OV, 2);
            AddFaseToController(c, "42", FaseTypeEnum.OV, 2);
            c.Fasen[2].Detectoren.Clear();
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "02", FaseNaar = "05", Waarde = 20, GarantieWaarde = 20});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "05", FaseNaar = "02", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "02", FaseNaar = "41", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "02", FaseNaar = "42", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "05", FaseNaar = "41", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "05", FaseNaar = "42", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "41", FaseNaar = "02", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "42", FaseNaar = "02", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "41", FaseNaar = "05", Waarde = 50, GarantieWaarde = 50});
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel{FaseVan = "42", FaseNaar = "05", Waarde = 50, GarantieWaarde = 50});

            c.PrioData.PrioIngreepType = PrioIngreepTypeEnum.GeneriekePrioriteit;
            var ingreep = new PrioIngreepModel
            {
                FaseCyclus = "41", 
                Naam = "bus",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };
            ingreep.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding
            });
            ingreep.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding
            });
            c.PrioData.PrioIngrepen.Add(ingreep);
            ingreep = new PrioIngreepModel
            {
                FaseCyclus = "42",
                Naam = "bus",
                Type = PrioIngreepVoertuigTypeEnum.Bus
            };
            ingreep.MeldingenData.Inmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Inmelding
            });
            ingreep.MeldingenData.Uitmeldingen.Add(new PrioIngreepInUitMeldingModel
            {
                Type = PrioIngreepInUitMeldingVoorwaardeTypeEnum.KARMelding,
                InUit = PrioIngreepInUitMeldingTypeEnum.Uitmelding
            });
            c.PrioData.PrioIngrepen.Add(ingreep);
            c.PrioData.NevenMeldingen.Add(new NevenMeldingModel
            {
                FaseCyclus1 = "41", FaseCyclus2 = "42", FaseCyclus3 = "NG", BezetTijdHoog = 50, BezetTijdLaag = 20
            });

            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "02" });
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "05" });
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "41" });
            c.PrioData.PrioIngreepSignaalGroepParameters.Add(new PrioIngreepSignaalGroepParametersModel{ FaseCyclus = "42" });

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }
        
        [Test]
        public void SimpleControllerWithStar_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisStar";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Thread.Sleep(150);
                Directory.CreateDirectory(path);
                Thread.Sleep(150);
            }
            var c = GetBasicController();
            MakeStarController(c);

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }

        [Test]
        public void SimpleControllerWithStarProgrammaTijdenInParameters_Generated_BuildsSuccesfully()
        {
            var path = @"C:\temp\TLCGen_buildTests\basisStarPrms";
            var output = new List<string>();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            else
            {
                Directory.Delete(path, true);
                Thread.Sleep(150);
                Directory.CreateDirectory(path);
                Thread.Sleep(150);
            }
            var c = GetBasicController();
            MakeStarController(c);

            GenerateController(path, c);

            var p = BuildController(path, output);

            Assert.Equals(0, p.ExitCode);
        }

        private static void MakeStarController(ControllerModel c)
        {
            c.StarData.ToepassenStar = true;

            c.StarData.ProgrammaTijdenInParameters = true;

            c.StarData.Programmas.Add(new StarProgrammaModel
            {
                Cyclustijd = 35, Naam = "default", Fasen = new List<StarProgrammaFase>
                {
                    new StarProgrammaFase {FaseCyclus = "02", Start1 = 1, Eind1 = 10},
                    new StarProgrammaFase {FaseCyclus = "05", Start1 = 17, Eind1 = 30},
                }
            });
            c.StarData.Programmas.Add(new StarProgrammaModel
            {
                Cyclustijd = 36, Naam = "weekeind", Fasen = new List<StarProgrammaFase>
                {
                    new StarProgrammaFase {FaseCyclus = "02", Start1 = 2, Eind1 = 11},
                    new StarProgrammaFase {FaseCyclus = "05", Start1 = 18, Eind1 = 31},
                }
            });
            c.StarData.DefaultProgramma = "default";
            c.StarData.ProgrammaSturingViaParameter = true;

            c.StarData.ProgrammaSturingViaKlok = true;
            c.StarData.PeriodenData.Add(new StarPeriodeDataModel
            {
                Periode = "star1", StarProgramma = "default"
            });
            c.StarData.PeriodenData.Add(new StarPeriodeDataModel
            {
                Periode = "starWeekeind", StarProgramma = "weekeind"
            });

            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                Naam = "star1",
                DagCode = PeriodeDagCodeEnum.Werkdagen,
                StartTijd = new TimeSpan(12, 0, 0),
                EindTijd = new TimeSpan(16, 0, 0),
                Type = PeriodeTypeEnum.StarRegelen,
                Commentaar = "star regelen default"
            });
            c.PeriodenData.Perioden.Add(new PeriodeModel
            {
                Naam = "starWeekeind",
                DagCode = PeriodeDagCodeEnum.Weekeind,
                StartTijd = new TimeSpan(18, 0, 0),
                EindTijd = new TimeSpan(21, 0, 0),
                Type = PeriodeTypeEnum.StarRegelen,
                Commentaar = "star regelen weekeind"
            });
        }
    }
}
