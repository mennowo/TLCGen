using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Helpers;

namespace TLCGen.Models
{
    public class ModelBase
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public long ID { get; set; }

        #endregion // Properties

        #region Constructor

        public ModelBase()
        {
            ID = IDProvider.GetNextID();
        }

        #endregion
    }
}
