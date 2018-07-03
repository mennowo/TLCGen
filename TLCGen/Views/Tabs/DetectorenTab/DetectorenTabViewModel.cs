using System;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2)]
    public class DetectorenTabViewModel : TLCGenMainTabItemViewModel
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
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["DetectorenTabDrawingImage"];
            }
        }
        
        public override string DisplayName
        {
            get
            {
                return "Detectie/ingangen";
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

        public DetectorenTabViewModel() : base(TabItemTypeEnum.DetectieTab)
        {
            
        }

        #endregion // Constructor
    }
}
