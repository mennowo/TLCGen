using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Helpers;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index:0)]
    public class AlgemeenTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region Public methods

        #endregion // Public methods

        #region Commands
        
        #endregion // Commands

        #region Command Functionality
        
        #endregion // Command Functionality

        #region TabItem Overrides

        public override ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["AlgemeenTabDrawingImage"];
            }
        }

        public override string DisplayName
        {
            get
            {
                return "Algemeen";
            }
        }

        public override void OnSelected()
        {
            if(SelectedTab == null && TabItems?.Count > 0)
            {
                SelectedTab = TabItems[0];
            }
            base.OnSelected();
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region Constructor

        public AlgemeenTabViewModel() : base(TabItemTypeEnum.AlgemeenTab)
        {

        }

        #endregion // Constructor
    }
}
