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
	}
}