using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Interfaces
{
    public interface IHaveTemplates<T>
    {
        void AddFromTemplate(List<T> items);
        List<object> GetTemplatableItems();
    }
}
