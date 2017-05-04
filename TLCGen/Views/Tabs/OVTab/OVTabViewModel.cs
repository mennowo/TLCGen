using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 3)]
    public class OVTabViewModel : TLCGenMainTabItemViewModel
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
                return (ImageSource)dict["OVTabDrawingImage"];
            }
        }

        public override string DisplayName
        {
            get
            {
                return "OV/HD";
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

        public OVTabViewModel() : base(TabItemTypeEnum.OVTab)
        {
            
        }

        #endregion // Constructor
    }
}
