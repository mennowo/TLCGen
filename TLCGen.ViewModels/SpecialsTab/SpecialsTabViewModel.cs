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
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 6)]
    public class SpecialsTabViewModel : TLCGenMainTabItemViewModel
    {
        #region Fields

        #endregion // Fields

        #region Properties

        public ImageSource Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (ImageSource)dict["SpecialsTabDrawingImage"];
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
                return "Specials";
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

        public SpecialsTabViewModel(ControllerModel controller) : base(controller)
        {
            SortedDictionary<int, Type> TabTypes = new SortedDictionary<int, Type>();

            var attr = typeof(PTPKoppelingenTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(PTPKoppelingenTabViewModel));
            attr = typeof(FileTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FileTabViewModel));
            attr = typeof(SignalenTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(SignalenTabViewModel));
            attr = typeof(VAOntruimenTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(VAOntruimenTabViewModel));

            foreach (var tab in TabTypes)
            {
                var v = Activator.CreateInstance(tab.Value, _Controller);
                TabItems.Add(v as ITLCGenTabItem);
            }
        }

        #endregion // Constructor
    }
}
