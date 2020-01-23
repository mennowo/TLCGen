using System;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4)]
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

        //public override Visibility Visibility { get => Visibility.Collapsed; set { } }

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

        public override bool CanBeEnabled()
        {
            return _Controller.PrioData.PrioIngreepType == Models.Enumerations.PrioIngreepTypeEnum.GeneriekePrioriteit;
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
