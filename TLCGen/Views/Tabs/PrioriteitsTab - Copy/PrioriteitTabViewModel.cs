using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;
using TLCGen.ViewModels;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: -1)]
    public class PrioriteitTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region TabItem Overrides

        public override ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["OVTabDrawingImage"];
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Prioriteit";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        #endregion // Public Methods

        #region Constructor

        public PrioriteitTabViewModel() : base(TabItemTypeEnum.PrioriteitTab)
        {

        }

        #endregion // Constructor
    }
}
