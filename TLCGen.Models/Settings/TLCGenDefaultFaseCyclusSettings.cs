using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models.Settings
{
    public class TLCGenDefaultFaseCyclusSettings
    {
        public FaseCyclusModel DefaultAutoModel { get; set; }
        public FaseCyclusModel DefaultFietsModel { get; set; }
        public FaseCyclusModel DefaultVoetgangerModel { get; set; }
        public FaseCyclusModel DefaultOVModel { get; set; }

        public FaseCyclusModel[] DefaultFasen = new FaseCyclusModel[4];

        public TLCGenDefaultFaseCyclusSettings()
        {
            DefaultAutoModel = new FaseCyclusModel() {
                Type = Enumerations.FaseTypeEnum.Auto,
                Kopmax = 80,
                TFG = 40,
                TGG = 40,
                TGG_min = 40,
                TGL = 30,
                TGL_min = 30,
                TRG = 20,
                TRG_min = 20,
                Meeverlengen = Enumerations.NooitAltijdAanUitEnum.SchAan,
                VasteAanvraag = Enumerations.NooitAltijdAanUitEnum.SchUit,
                Wachtgroen = Enumerations.NooitAltijdAanUitEnum.SchUit
            };

            DefaultFietsModel = new FaseCyclusModel()
            {
                Type = Enumerations.FaseTypeEnum.Fiets,
                Kopmax = 80,
                TFG = 40,
                TGG = 40,
                TGG_min = 40,
                TGL = 30,
                TGL_min = 30,
                TRG = 20,
                TRG_min = 20,
                Meeverlengen = Enumerations.NooitAltijdAanUitEnum.SchAan,
                VasteAanvraag = Enumerations.NooitAltijdAanUitEnum.SchUit,
                Wachtgroen = Enumerations.NooitAltijdAanUitEnum.SchUit
            };

            DefaultVoetgangerModel = new FaseCyclusModel()
            {
                Type = Enumerations.FaseTypeEnum.Voetganger,
                TFG = 60,
                TGG = 40,
                TGG_min = 40,
                TGL = 30,
                TGL_min = 30,
                TRG = 20,
                TRG_min = 20,
                Meeverlengen = Enumerations.NooitAltijdAanUitEnum.SchAan,
                VasteAanvraag = Enumerations.NooitAltijdAanUitEnum.SchUit,
                Wachtgroen = Enumerations.NooitAltijdAanUitEnum.SchUit
            };

            DefaultOVModel = new FaseCyclusModel()
            {
                Type = Enumerations.FaseTypeEnum.OV,
                TFG = 40,
                TGG = 40,
                TGG_min = 40,
                TGL = 30,
                TGL_min = 30,
                TRG = 20,
                TRG_min = 20,
                Meeverlengen = Enumerations.NooitAltijdAanUitEnum.SchUit,
                VasteAanvraag = Enumerations.NooitAltijdAanUitEnum.SchUit,
                Wachtgroen = Enumerations.NooitAltijdAanUitEnum.SchUit
            };

            DefaultFasen[0] = DefaultAutoModel;
            DefaultFasen[1] = DefaultFietsModel;
            DefaultFasen[2] = DefaultVoetgangerModel;
            DefaultFasen[3] = DefaultOVModel;
        }
    }
}
