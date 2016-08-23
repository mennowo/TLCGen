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
using TLCGen.Helpers;
using TLCGen.Interfaces;
using TLCGen.Models;
using TLCGen.ViewModels.Templates;

namespace TLCGen.ViewModels
{
    public class DetectorenTabViewModel : TabViewModel, IHaveTemplates<DetectorModel>
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
                         this, $@"{SettingsProvider.AppSettings.PrefixSettings.DetectorDefinePrefix}([0-9])");
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

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

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
                    if ((_ControllerVM.IsElementDefineUnique(dm.Define) &&
                         _ControllerVM.IsElementNaamUnique(dm.Naam)))
                    {
                        DetectorViewModel dvm = new DetectorViewModel(_ControllerVM, dm);
                        dvm.FaseVM = DetectorenFasenLijstVM?.SelectedFase;
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

        #region Constructor

        public DetectorenTabViewModel(ControllerViewModel controllervm) : base(controllervm)
        {
            _DetectorenExtraLijstVM = new DetectorenExtraLijstViewModel(_ControllerVM);
            _DetectorenFasenLijstVM = new DetectorenFasenLijstViewModel(_ControllerVM);
            _DetectorenAllesLijstVM = new DetectorenAllesLijstViewModel(_ControllerVM);
        }

        #endregion // Constructor
    }
}
