using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1)]
    public class FasenTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields
        
        private IList _SelectedFaseCycli = new ArrayList();

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
                return (ImageSource)dict["FasenTabDrawingImage"];
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Fasen";
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

        public FasenTabViewModel() : base(TabItemTypeEnum.FasenTab)
        {
            
        }

        #endregion // Constructor
    }
}
