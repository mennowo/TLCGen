using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels.Templates
{
    public class DetectorTemplateViewModel : TemplateViewModelBase
    {
        public List<DetectorModel> Detectoren { get; set; }

        public override List<object> GetItems()
        {
            return Detectoren.ToList<object>();
        }

        public DetectorTemplateViewModel()
        {
            Detectoren = new List<DetectorModel>();
        }

        public DetectorTemplateViewModel(List<DetectorModel> items)
        {
            Detectoren = new List<DetectorModel>();
            foreach (DetectorModel dm in items)
            {
                Detectoren.Add(dm);
            }
        }
    }
}
