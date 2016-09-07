using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models.Enumerations;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace TLCGen.Models.Settings
{
    [Editor()]
    public class TLCGenDefaultFaseCyclusSettings
    {
        [ExpandableObject]
        [DisplayName("Default auto fasen")]
        [Description("Default auto fasen")]
        public FaseCyclusModel DefaultAutoModel { get; set; }
        [ExpandableObject]
        [DisplayName("Default fiets fasen")]
        [Description("Default fiets fasen")]
        public FaseCyclusModel DefaultFietsModel { get; set; }
        [ExpandableObject]
        [DisplayName("Default voetganger fasen")]
        [Description("Default voetganger fasen")]
        public FaseCyclusModel DefaultVoetgangerModel { get; set; }
        [ExpandableObject]
        [DisplayName("Default OV fasen")]
        [Description("Default OV fasen")]
        public FaseCyclusModel DefaultOVModel { get; set; }

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
        }
    }
}
