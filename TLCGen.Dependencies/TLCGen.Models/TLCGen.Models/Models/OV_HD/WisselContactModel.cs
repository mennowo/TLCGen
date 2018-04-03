using System;

namespace TLCGen.Models
{
    [Serializable]
    public class WisselContactModel : IHaveName
    {
        #region Properties

        [ModelName]
        public string Naam { get; set; }

        #endregion // Properties
    }
}
