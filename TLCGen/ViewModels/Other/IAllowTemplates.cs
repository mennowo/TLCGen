using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.ViewModels
{
    public interface IAllowTemplates<T>
    {
        void InsertItemsFromTemplate(List<T> items);
    }
}
