using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 1, type: TabItemTypeEnum.SpecialsTab)]
    public class FileTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<FileIngreepViewModel> _FileIngrepen;
        private ObservableCollection<string> _Fasen;
        private ObservableCollection<string> _Detectoren;
        private string _SelectedFaseNaam;
        private string _SelectedDetectorNaam;
        
        #endregion // Fields

        #region Properties

        public string SelectedFaseNaam
        {
            get { return _SelectedFaseNaam; }
            set
            {
                _SelectedFaseNaam = value;
                OnPropertyChanged("SelectedFaseNaam");
            }
        }

        public string SelectedDetectorNaam
        {
            get { return _SelectedDetectorNaam; }
            set
            {
                _SelectedDetectorNaam = value;
                OnPropertyChanged("SelectedDetectorNaam");
            }
        }

        public ObservableCollection<FileIngreepViewModel> FileIngrepen
        {
            get
            {
                if(_FileIngrepen == null)
                {
                    _FileIngrepen = new ObservableCollection<FileIngreepViewModel>();
                }
                return _FileIngrepen;
            }
        }

        public ObservableCollection<string> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<string>();
                }
                return _Fasen;
            }
        }

        public ObservableCollection<string> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<string>();
                }
                return _Detectoren;
            }
        }

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region Command functionality

        #endregion // Command functionality

        #region Private methods

        #endregion // Private methods

        #region Public methods

        #endregion // Public methods

        #region TLCGen TabItem overrides

        public override string DisplayName
        {
            get { return "File"; }
        }

        public override void OnSelected()
        {
            Fasen.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                Fasen.Add(fcm.Naam);
            }
            Detectoren.Clear();
            foreach (FaseCyclusModel fcm in _Controller.Fasen)
            {
                foreach(DetectorModel dm in fcm.Detectoren)
                {
                    Detectoren.Add(dm.Naam);
                }
            }
            foreach (DetectorModel dm in _Controller.Detectoren)
            {
                Detectoren.Add(dm.Naam);
            }
        }

        #endregion // TLCGen TabItem overrides

        #region Constructor

        public FileTabViewModel(ControllerModel controller) : base(controller)
        {

        }

        #endregion // Constructor
    }
}
