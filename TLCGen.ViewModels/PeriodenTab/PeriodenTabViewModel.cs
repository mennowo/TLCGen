using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 5)]
    public class PeriodenTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public override ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["KlokPeriodenTabDrawingImage"];
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Klokperioden";
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

        public PeriodenTabViewModel() : base(TabItemTypeEnum.PeriodenTab)
        {
            
        }

        #endregion // Constructor
    }
}
