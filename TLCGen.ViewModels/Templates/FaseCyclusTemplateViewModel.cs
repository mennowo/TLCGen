using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels.Templates
{
    public class FaseCyclusTemplateViewModel : TemplateViewModelBase
    {
        public List<FaseCyclusModel> Fasen { get; set; }

        public override List<object> GetItems()
        {
            return Fasen.ToList<object>();
        }

        public FaseCyclusTemplateViewModel()
        {
            Fasen = new List<FaseCyclusModel>();
        }

        public FaseCyclusTemplateViewModel(List<FaseCyclusModel> items)
        {
            Fasen = new List<FaseCyclusModel>();
            foreach(FaseCyclusModel fcm in items)
            {
                Fasen.Add(fcm);
            }
        }
    }
}
