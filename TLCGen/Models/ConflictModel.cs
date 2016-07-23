using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLCGen.Models
{
    public class ConflictModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public long FaseVan { get; set; }
        public long FaseNaar { get; set; }
        public int Waarde { get; set; }

        #endregion // Properties

        #region Constructor

        public ConflictModel() : base()
        {

        }

        #endregion // Constructor
    }
}
