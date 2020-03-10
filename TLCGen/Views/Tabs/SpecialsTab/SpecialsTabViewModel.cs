using System;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 8)]
    public class SpecialsTabViewModel : TLCGenMainTabItemViewModel
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
                return (ImageSource)dict["SpecialsTabDrawingImage"];
            }
        }

        public override string DisplayName => "Specials";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        #endregion // Public Methods

        #region Constructor

        public SpecialsTabViewModel() : base(TabItemTypeEnum.SpecialsTab)
        {
            
        }

        #endregion // Constructor
    }
}
