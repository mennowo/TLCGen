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
using TLCGen.Interfaces;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1)]
    public class FasenTabViewModel : TLCGenMainTabItemViewModel, IHaveTemplates<FaseCyclusModel>
    {
        #region Fields
        
        private IList _SelectedFaseCycli = new ArrayList();
        private TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel> _TemplateManagerVM;

        #endregion // Fields

        #region Properties
        
        public DrawingImage Icon
        {
            get
            {
                ResourceDictionary dict = new ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (DrawingImage)dict["FasenTabDrawingImage"];
            }
        }

        public TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel> TemplateManagerVM
        {
            get
            {
                if(_TemplateManagerVM == null)
                {
                    _TemplateManagerVM = new TemplatesManagerViewModelT<FaseCyclusTemplateViewModel, FaseCyclusModel>
                        (System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "templates\\fasecycli\\"),
                         this, $@"{SettingsProvider.Default.GetFaseCyclusDefinePrefix()}([0-9])");
                }
                return _TemplateManagerVM;
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

        #region IHaveTemplates

        public List<object> GetTemplatableItems()
        {
            List<object> items = new List<object>();
            //if (SelectedFaseCycli != null)
            //    foreach (FaseCyclusViewModel fcvm in SelectedFaseCycli)
            //        items.Add(fcvm.FaseCyclus);
            return items;
        }

        public void AddFromTemplate(List<FaseCyclusModel> items)
        {
            try
            {
                foreach (FaseCyclusModel fcm in items)
                {
                    var message1 = new IsElementIdentifierUniqueRequest(fcm.Naam, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message1);
                    if (message1.Handled && message1.IsUnique)
                    {
                        bool IsOK = true;
                        foreach(DetectorModel dm in fcm.Detectoren)
                        {
                            var message2 = new IsElementIdentifierUniqueRequest(dm.Naam, ElementIdentifierType.Naam);
                            Messenger.Default.Send(message2);
                            if (!(message2.Handled && message2.IsUnique))
                            {
                                IsOK = false;
                                break;
                            }
                        }
                        if(IsOK)
                        {
                            FaseCyclusViewModel fcvm = new FaseCyclusViewModel(fcm);
                            //Fasen.Add(fcvm);
                        }
                    }
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        #endregion // IHaveTemplates

        

        #region Constructor

        public FasenTabViewModel(ControllerModel controller) : base(controller)
        {
            SortedDictionary<int, Type> TabTypes = new SortedDictionary<int, Type>();

            var attr = typeof(FasenLijstTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FasenLijstTabViewModel));
            attr = typeof(FasenLijstTimersTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FasenLijstTimersTabViewModel));
            attr = typeof(FasenDetailsTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FasenDetailsTabViewModel));
            attr = typeof(FasenGroentijdenSetsTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FasenGroentijdenSetsTabViewModel));
            attr = typeof(FasenOVTabViewModel).GetCustomAttributes(typeof(TLCGenTabItemAttribute), true).FirstOrDefault() as TLCGenTabItemAttribute;
            TabTypes.Add(attr.Index, typeof(FasenOVTabViewModel));

            foreach (var tab in TabTypes)
            {
                var v = Activator.CreateInstance(tab.Value, _Controller);
                TabItems.Add(v as ITLCGenTabItem);
            }
        }

        #endregion // Constructor
    }
}
