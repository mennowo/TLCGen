using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TLCGen.Interfaces;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2)]
    public class DetectorenTabViewModel : TLCGenMainTabItemViewModel, IHaveTemplates<DetectorModel>
    {
        #region Fields

        private TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel> _TemplateManagerVM;

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
                return (DrawingImage)dict["DetectorenTabDrawingImage"];
            }
        }

        public TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel> TemplateManagerVM
        {
            get
            {
                if (_TemplateManagerVM == null)
                {
                    //_TemplateManagerVM = new TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel>
                    //    (System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "templates\\detectors\\"),
                    //     this, $@"{SettingsProvider.Default.GetDetectorDefinePrefix()}([0-9])");
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
                return "Detectie";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        #endregion // TabItem Overrides

        #region ITabWithTemplates

        public List<object> GetTemplatableItems()
        {
            List<object> items = new List<object>();
            
            switch(SelectedTab?.DisplayName)
            {
                case "PerFaseTab":
#warning TODO
                    //foreach (DetectorViewModel dvm in DetectorenFasenLijstVM.SelectedDetectoren)
                    //    items.Add(dvm.Detector);
                    break;
                case "ExtraTab":
                    //foreach (DetectorViewModel dvm in DetectorenExtraLijstVM.SelectedDetectoren)
                    //    items.Add(dvm.Detector);
                    break;
            }
            return items;
        }

        public void AddFromTemplate(List<DetectorModel> items)
        {
            try
            {
                foreach(DetectorModel dm in items)
                {
                    var message1 = new IsElementIdentifierUniqueRequest(dm.Naam, ElementIdentifierType.Naam);
                    Messenger.Default.Send(message1);
                    if (message1.Handled && message1.IsUnique)
                    {
#warning TODO
                        //DetectorViewModel dvm = new DetectorViewModel(dm);
                        //dvm.FaseCyclus = DetectorenFasenLijstVM?.SelectedFaseNaam;
                        //switch (SelectedTab?.DisplayName)
                        //{
                        //    case "PerFaseTab":
                        //        DetectorenFasenLijstVM?.Detectoren?.Add(dvm);
                        //        break;
                        //    case "ExtraTab":
                        //        DetectorenExtraLijstVM?.Detectoren?.Add(dvm);
                        //        break;
                        //}
                    }
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        #endregion // ITabWithTemplates

        #region Public Methods

        /// <summary>
        /// Sets the value of the property indicated by propName to the value it has 
        /// for the parsed instance of DetectorViewModel for all selected detectors
        /// </summary>
        /// <param name="o">The instance of DetectorViewModel to take as the base case</param>
        /// <param name="propName">The property to set</param>
        public void SetAllSelectedDetectorenValue(DetectorViewModel o, string propName)
        {
            IList dets = null;
#warning TODO
            //switch(SelectedTab.DisplayName)
            //{
            //    case "PerFaseTab":
            //        dets = DetectorenFasenLijstVM.SelectedDetectoren;
            //        break;
            //    case "ExtraTab":
            //        dets = DetectorenExtraLijstVM.SelectedDetectoren;
            //        break;
            //    case "AllesTab":
            //        dets = DetectorenAllesLijstVM.SelectedDetectoren;
            //        break;
            //}
            if(dets != null)
            {
                foreach (DetectorViewModel dvm in dets)
                {
                    object value = o.GetType().GetProperty(propName).GetValue(o);
                    dvm.GetType().GetProperty(propName).SetValue(dvm, value);
                }
            }
        }

        #endregion // Public Methods

        #region Constructor

        public DetectorenTabViewModel() : base(TabItemTypeEnum.DetectieTab)
        {
            
        }

        #endregion // Constructor
    }
}
