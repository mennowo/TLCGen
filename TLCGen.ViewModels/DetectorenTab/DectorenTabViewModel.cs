using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;

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
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["DetectorenTabDrawingImage"];
            }
        }
        
        public override string DisplayName
        {
            get
            {
                return "Detectie";
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
