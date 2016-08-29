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

        private bool ChangeModuleCommand_CanExecute(object obj)
        {
            return DetectorenAllesLijstVM != null && DetectorenAllesLijstVM.Detectoren != null && DetectorenAllesLijstVM.Detectoren.Count > 0;
        }

        private void GenerateSimulationValuesCommand_Executed(object obj)
        {
            Random rd = new Random();
            int[] qs = { 25, 50, 100, 200 };
            int qsmax = 4;

            foreach (FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
            {
                int qthis = rd.Next(qsmax);
                foreach(DetectorViewModel dvm in fcvm.Detectoren)
                {
                    dvm.Q1 = qs[qthis];
                    int next = qthis + 1;
                    if (next >= qsmax) next -= qsmax;
                    dvm.Q2 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dvm.Q3 = qs[next];
                    ++next;
                    if (next >= qsmax) next -= qsmax;
                    dvm.Q4 = qs[next];
                }
            }
            foreach (DetectorViewModel dvm in _ControllerVM.Detectoren)
            {
                int qthis = rd.Next(qsmax);
                dvm.Q1 = qs[qthis];
                int next = qthis + 1;
                if (next >= qsmax) next -= qsmax;
                dvm.Q2 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dvm.Q3 = qs[next];
                ++next;
                if (next >= qsmax) next -= qsmax;
                dvm.Q4 = qs[next];
            }
        }

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
