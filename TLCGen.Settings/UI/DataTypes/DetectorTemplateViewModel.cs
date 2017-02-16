using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.Settings
{
    public class DetectorTemplateViewModel : ViewModelBase, IViewModelWithItem
    {
        #region Fields

        private TLCGenTemplateModel<DetectorModel> _Template;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _Template.Naam; }
            set
            {
                _Template.Naam = value;
                RaisePropertyChanged("Naam");
            }
        }

        public string Replace
        {
            get { return _Template.Replace; }
            set
            {
                _Template.Replace = value;
                RaisePropertyChanged("Replace");
            }
        }

        private DetectorModel _SelectedDetector;
        public DetectorModel SelectedDetector
        {
            get { return _SelectedDetector; }
            set
            {
                _SelectedDetector = value;
                if (value != null)
                    SelectedDetectorTypeString = value.Type.GetDescription();
                else
                    SelectedDetectorTypeString = null;

                RaisePropertyChanged(null);
            }
        }


        private ObservableCollection<DetectorModel> _Detectoren;
        public ObservableCollection<DetectorModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<DetectorModel>();
                }
                return _Detectoren;
            }
        }

        private List<string> _DetectorTypeOpties;
        public List<string> DetectorTypeOpties
        {
            get
            {
                if (_DetectorTypeOpties == null)
                {
                    _DetectorTypeOpties = new List<string>();
                }
                return _DetectorTypeOpties;
            }
        }

        private string _SelectedDetectorTypeString;
        public string SelectedDetectorTypeString
        {
            get { return _SelectedDetectorTypeString; }
            set
            {
                _SelectedDetectorTypeString = value;
                if (SelectedDetector != null)
                {
                    if (value == DetectorTypeEnum.Kop.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Kop;

                    }
                    else if (value == DetectorTypeEnum.Lang.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Lang;

                    }
                    else if (value == DetectorTypeEnum.Verweg.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Verweg;

                    }
                    else if (value == DetectorTypeEnum.File.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.File;
                    }
                    else if (value == DetectorTypeEnum.KnopBinnen.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.KnopBinnen;
                    }
                    else if (value == DetectorTypeEnum.KnopBuiten.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.KnopBuiten;
                    }
                    else if (value == DetectorTypeEnum.Radar.GetDescription())
                    {
                        SelectedDetector.Type = DetectorTypeEnum.Radar;
                    }
                }
                RaisePropertyChanged("SelectedDetectorTypeString");
            }
        }

        #endregion // Properties


        #region Commands


        RelayCommand _AddDetectorCommand;
        public ICommand AddDetectorCommand
        {
            get
            {
                if (_AddDetectorCommand == null)
                {
                    _AddDetectorCommand = new RelayCommand(new Action<object>(AddDetectorCommand_Executed), new Predicate<object>(AddDetectorCommand_CanExecute));
                }
                return _AddDetectorCommand;
            }
        }


        RelayCommand _RemoveDetectorCommand;
        public ICommand RemoveDetectorCommand
        {
            get
            {
                if (_RemoveDetectorCommand == null)
                {
                    _RemoveDetectorCommand = new RelayCommand(new Action<object>(RemoveDetectorCommand_Executed), new Predicate<object>(RemoveDetectorCommand_CanExecute));
                }
                return _RemoveDetectorCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality
        
        private void AddDetectorCommand_Executed(object prm)
        {
            DetectorModel d = new DetectorModel();
            d.Naam = Replace + "_" + (Detectoren.Count + 1);
            Detectoren.Add(d);
        }

        bool AddDetectorCommand_CanExecute(object prm)
        {
            return true;
        }

        void RemoveDetectorCommand_Executed(object prm)
        {
            Detectoren.Remove(SelectedDetector);
            SelectedDetector = null;
        }

        bool RemoveDetectorCommand_CanExecute(object prm)
        {
            return SelectedDetector != null && Detectoren.Count > 0;
        }

        #endregion // Command Functionality

        #region IViewModelWithItem

        public object GetItem()
        {
            return _Template;
        }

        #endregion // IViewModelWithItem

        #region Collection Changed
        
        private void Detectoren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (DetectorModel d in e.NewItems)
                {
                    _Template.Items.Add(d);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (DetectorModel d in e.OldItems)
                {
                    _Template.Items.Remove(d);
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public DetectorTemplateViewModel(TLCGenTemplateModel<DetectorModel> template)
        {
            _Template = template;
            foreach (var fc in template.Items)
            {
                Detectoren.Add(fc);
            }
            Detectoren.CollectionChanged += Detectoren_CollectionChanged;

            DetectorTypeOpties.Clear();
            DetectorTypeOpties.Add(DetectorTypeEnum.Kop.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Lang.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Verweg.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.File.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.KnopBinnen.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.KnopBuiten.GetDescription());
            DetectorTypeOpties.Add(DetectorTypeEnum.Radar.GetDescription());
        }

        #endregion // Constructor
    }
}
