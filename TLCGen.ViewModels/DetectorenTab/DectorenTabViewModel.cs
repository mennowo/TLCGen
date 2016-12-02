using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Settings;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 2)]
    public class DetectorenTabViewModel : TLCGenTabItemViewModel, IHaveTemplates<DetectorModel>
    {
        #region Fields

        private DetectorenFasenLijstViewModel _DetectorenFasenLijstVM;
        private DetectorenExtraLijstViewModel _DetectorenExtraLijstVM;
        private DetectorenAllesLijstViewModel _DetectorenAllesLijstVM;
        private TabItem _SelectedTab;
        private TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel> _TemplateManagerVM;

        #endregion // Fields

        #region Properties

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                _SelectedTab = value;
#warning TODO
                //foreach(FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
                //{
                //    fcvm.Detectoren.BubbleSort();
                //}
                //_ControllerVM.Detectoren.BubbleSort();
                if(_SelectedTab.Name == "AllesTab" || _SelectedTab.Name == "SimulatieTab")
                {
                    _DetectorenAllesLijstVM.SetDetectorenChanged();
                }
                OnPropertyChanged("SelectedTab");
            }
        }

        public TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel> TemplateManagerVM
        {
            get
            {
                if (_TemplateManagerVM == null)
                {
                    _TemplateManagerVM = new TemplatesManagerViewModelT<DetectorTemplateViewModel, DetectorModel>
                        (System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "templates\\detectors\\"),
                         this, $@"{SettingsProvider.Instance.GetDetectorDefinePrefix()}([0-9])");
                }
                return _TemplateManagerVM;
            }
        }

        public DetectorenExtraLijstViewModel DetectorenExtraLijstVM
        {
            get { return _DetectorenExtraLijstVM; }
        }

        public DetectorenFasenLijstViewModel DetectorenFasenLijstVM
        {
            get { return _DetectorenFasenLijstVM; }
        }

        public DetectorenAllesLijstViewModel DetectorenAllesLijstVM
        {
            get { return _DetectorenAllesLijstVM; }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _GenerateSimulationValuesCommand;
        public ICommand GenerateSimulationValuesCommand
        {
            get
            {
                if (_GenerateSimulationValuesCommand == null)
                {
                    _GenerateSimulationValuesCommand = new RelayCommand(GenerateSimulationValuesCommand_Executed, ChangeModuleCommand_CanExecute);
                }
                return _GenerateSimulationValuesCommand;
            }
        }

        #endregion // Commands

        #region Command functionality

        private bool ChangeModuleCommand_CanExecute(object obj)
        {
            return DetectorenAllesLijstVM != null && DetectorenAllesLijstVM.Detectoren != null && DetectorenAllesLijstVM.Detectoren.Count > 0;
        }

        private void GenerateSimulationValuesCommand_Executed(object obj)
        {
            Random rd = new Random();
            int[] qs = { 25, 50, 100, 200,
                         5,  25, 50,  200,
                         5,  25, 100, 200,
                         5,  50, 100, 200 };
            int qsmax = 16;

            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                int qthis = rd.Next(qsmax);
                foreach (DetectorModel dm in fcm.Detectoren)
                {
                    dm.Simulatie.Q1 = qs[qthis];
                    int next = qthis + 1;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q2 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q3 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dm.Simulatie.Q4 = qs[next];

                    switch (fcm.Type)
                    {
                        case Models.Enumerations.FaseTypeEnum.Auto:
                            dm.Simulatie.Stopline = 1800;
                            break;
                        case Models.Enumerations.FaseTypeEnum.Fiets:
                            dm.Simulatie.Stopline = 5000;
                            break;
                        case Models.Enumerations.FaseTypeEnum.Voetganger:
                            dm.Simulatie.Stopline = 10000;
                            break;
                    }
                    dm.Simulatie.FCNr = fcm.Define;
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                int qthis = rd.Next(qsmax);
                dm.Simulatie.Q1 = qs[qthis];
                int next = qthis + 1;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q2 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q3 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dm.Simulatie.Q4 = qs[next];
                dm.Simulatie.Stopline = 1800;
            }

            DetectorenAllesLijstVM.SetDetectorenChanged();
        }

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

        public override void OnSelected()
        {
            DetectorenFasenLijstVM.Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                DetectorenFasenLijstVM.Fasen.Add(fcm.Naam);
            }
        }

        #endregion // TabItem Overrides

        #region ITabWithTemplates

        public List<object> GetTemplatableItems()
        {
            List<object> items = new List<object>();
            
            switch(SelectedTab?.Name)
            {
                case "PerFaseTab":
                    foreach (DetectorViewModel dvm in DetectorenFasenLijstVM.SelectedDetectoren)
                        items.Add(dvm.Detector);
                    break;
                case "ExtraTab":
                    foreach (DetectorViewModel dvm in DetectorenExtraLijstVM.SelectedDetectoren)
                        items.Add(dvm.Detector);
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
                    var message2 = new IsElementIdentifierUniqueRequest(dm.Define, ElementIdentifierType.Define);
                    MessageManager.Instance.SendWithRespons(message1);
                    MessageManager.Instance.SendWithRespons(message2);
                    if (message1.Handled && message1.IsUnique &&
                        message2.Handled && message2.IsUnique)
                    {
                        DetectorViewModel dvm = new DetectorViewModel(dm);
                        dvm.FaseCyclus = DetectorenFasenLijstVM?.SelectedFaseNaam;
                        switch (SelectedTab?.Name)
                        {
                            case "PerFaseTab":
                                DetectorenFasenLijstVM?.Detectoren?.Add(dvm);
                                break;
                            case "ExtraTab":
                                DetectorenExtraLijstVM?.Detectoren?.Add(dvm);
                                break;
                        }
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
            switch(SelectedTab.Name)
            {
                case "PerFaseTab":
                    dets = DetectorenFasenLijstVM.SelectedDetectoren;
                    break;
                case "ExtraTab":
                    dets = DetectorenExtraLijstVM.SelectedDetectoren;
                    break;
                case "AllesTab":
                    dets = DetectorenAllesLijstVM.SelectedDetectoren;
                    break;
            }
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

        public DetectorenTabViewModel(ControllerModel controller) : base(controller)
        {
            _DetectorenExtraLijstVM = new DetectorenExtraLijstViewModel(_Controller);
            _DetectorenFasenLijstVM = new DetectorenFasenLijstViewModel(_Controller);
            _DetectorenAllesLijstVM = new DetectorenAllesLijstViewModel(_Controller);
        }

        #endregion // Constructor
    }
}
