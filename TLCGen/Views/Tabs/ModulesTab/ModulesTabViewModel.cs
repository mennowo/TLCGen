using System;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 5)]
    public class ModulesTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public override ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                                ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (DrawingImage)dict["ModulesTabDrawingImage"];
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName => "Modulen";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        #endregion // TabItem Overrides

        #region Public Methods

        #endregion // Public Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        #endregion // TLCGen Message Handling

        #region Constructor

        public ModulesTabViewModel() : base(TabItemTypeEnum.ModulesTab)
        {
            
        }

        #endregion // Constructor
    }
}
