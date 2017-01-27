using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 4)]
    public class ModulesTabViewModel : TLCGenMainTabItemViewModel
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
                return (DrawingImage)dict["ModulesTabDrawingImage"];
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
                return "Modulen";
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

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        private void OnFasenChanged(FasenChangedMessage message)
        {

        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public ModulesTabViewModel(ControllerModel controller) : base(controller)
        {
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));

            SortedDictionary<int, Type> TabTypes = new SortedDictionary<int, Type>();

            var attr = typeof(ModulesDetailsTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(ModulesDetailsTabViewModel));
            attr = typeof(ModulesFasenInstellingenTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(ModulesFasenInstellingenTabViewModel));

            foreach (var tab in TabTypes)
            {
                var v = Activator.CreateInstance(tab.Value, _Controller);
                TabItems.Add(v as ITLCGenTabItem);
            }
        }

        #endregion // Constructor
    }
}
