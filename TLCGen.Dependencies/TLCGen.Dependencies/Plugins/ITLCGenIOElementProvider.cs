using System.Collections.Generic;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Plugins
{
    public interface ITLCGenElementProvider : ITLCGenPlugin
    {
        List<IOElementModel> GetOutputItems();
        List<IOElementModel> GetInputItems();
        List<object> GetAllItems();

        bool IsElementNameUnique(string name, TLCGenObjectTypeEnum type);
    }
}
