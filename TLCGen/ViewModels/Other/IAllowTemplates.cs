using System.Collections.Generic;

namespace TLCGen.ViewModels
{
    public interface IAllowTemplates<T>
    {
        void InsertItemsFromTemplate(List<T> items);
        void UpdateAfterApplyTemplate(T item);
    }
}
