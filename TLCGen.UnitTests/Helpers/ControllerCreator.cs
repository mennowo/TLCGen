using TLCGen.Models;

namespace TLCGen.UnitTests
{
	public static class ControllerCreator
	{
		public static ControllerModel GetSmallControllerWithoutDetection()
		{
			var model = new ControllerModel();
			model.Fasen.Add(new FaseCyclusModel() { Naam = "01" });
			model.Fasen.Add(new FaseCyclusModel() { Naam = "02" });
			model.Fasen.Add(new FaseCyclusModel() { Naam = "03" });
			model.Fasen.Add(new FaseCyclusModel() { Naam = "04" });
			model.Fasen.Add(new FaseCyclusModel() { Naam = "05" });
			return model;
		}

		public static ControllerModel GetSmallControllerWithDetection()
		{
			var model = new ControllerModel();
			model.Fasen.Add(new FaseCyclusModel
			{
				Naam = "01",
				Detectoren = 
				{ 
					new DetectorModel { Naam = "011" },
					new DetectorModel { Naam = "012" }
				}
			});
			model.Fasen.Add(new FaseCyclusModel
			{
				Naam = "02",
				Detectoren = 
				{ 
					new DetectorModel { Naam = "021" },
					new DetectorModel { Naam = "022" },
					new DetectorModel { Naam = "023" }
				}
			});
			model.Fasen.Add(new FaseCyclusModel
			{
				Naam = "03", 
				Detectoren = 
				{ 
					new DetectorModel { Naam = "031" },
					new DetectorModel { Naam = "032" }
				}
			});
			model.Fasen.Add(new FaseCyclusModel
			{
				Naam = "04",
				Detectoren = 
				{ 
					new DetectorModel { Naam = "041" },
					new DetectorModel { Naam = "042" }
				}
			});
			model.Fasen.Add(new FaseCyclusModel
			{
				Naam = "05",
				Detectoren = 
				{ 
					new DetectorModel { Naam = "051" },
					new DetectorModel { Naam = "052" }
				}
			});
			return model;
		}

        public static ControllerModel GetSmallControllerWithConflicts()
        {
            var c = new ControllerModel();
            c.Fasen.Add(new FaseCyclusModel { Naam = "01" });
            c.Fasen.Add(new FaseCyclusModel { Naam = "02" });
            c.Fasen.Add(new FaseCyclusModel { Naam = "03" });
            c.Fasen.Add(new FaseCyclusModel { Naam = "04" });
            c.Fasen.Add(new FaseCyclusModel { Naam = "05" });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "01", FaseNaar = "02", Waarde = 10 });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "02", FaseNaar = "01", Waarde = 10 });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "03", FaseNaar = "04", Waarde = 10 });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "04", FaseNaar = "03", Waarde = 10 });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "01", FaseNaar = "05", Waarde = 10 });
            c.InterSignaalGroep.Conflicten.Add(new ConflictModel { FaseVan = "05", FaseNaar = "01", Waarde = 10 });
            return c;
        }
	}
}