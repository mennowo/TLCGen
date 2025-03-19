using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.SpecialsTab)]
    public class PTPKoppelingenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private PTPKoppelingViewModel _SelectedPTPKoppeling;
        private ObservableCollection<PTPKoppelingViewModel> _PTPKoppelingen;

        #endregion // Fields

        #region Properties

        public bool PTPInstellingenInParameters
        {
            get => Controller?.PTPData?.PTPInstellingenInParameters == true;
            set
            {
                Controller.PTPData.PTPInstellingenInParameters = value;
                OnPropertyChanged(broadcast: true);
            }
        }
        public bool PTPAlleenTijdensControl
        {
            get => Controller?.PTPData?.PTPAlleenTijdensControl == true;
            set
            {
                Controller.PTPData.PTPAlleenTijdensControl = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public ObservableCollection<PTPKoppelingViewModel> PTPKoppelingen
        {
            get
            {
                if (_PTPKoppelingen == null)
                    _PTPKoppelingen = new ObservableCollection<PTPKoppelingViewModel>();
                return _PTPKoppelingen;
            }
        }

        public PTPKoppelingViewModel SelectedPTPKoppeling
        {
            get => _SelectedPTPKoppeling;
            set
            {
                _SelectedPTPKoppeling = value;
                OnPropertyChanged("SelectedPTPKoppeling");
            }
        }
        
        public bool ShowMultivalent => DataAccess.TLCGenControllerDataProvider.Default.Controller?.Data?.CCOLVersie >= CCOLVersieEnum.CCOL110;

        public bool ShowTest => System.DateTime.Now.Year == Controller.Data.Fasebewaking; // tbv test ptp naar backup verbergen/tonen in GUI (indien TFB == huidig jaar)

        #endregion // Properties

        #region Commands

        RelayCommand _AddPTPKoppelingCommand;
        public ICommand AddPTPKoppelingCommand
        {
            get
            {
                if (_AddPTPKoppelingCommand == null)
                {
                    _AddPTPKoppelingCommand = new RelayCommand(AddPTPKoppelingCommand_Executed, AddPTPKoppelingCommand_CanExecute);
                }
                return _AddPTPKoppelingCommand;
            }
        }

        RelayCommand _RemovePTPKoppelingCommand;
        public ICommand RemovePTPKoppelingCommand
        {
            get
            {
                if (_RemovePTPKoppelingCommand == null)
                {
                    _RemovePTPKoppelingCommand = new RelayCommand(RemovePTPKoppelingCommand_Executed, RemovePTPKoppelingCommand_CanExecute);
                }
                return _RemovePTPKoppelingCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private bool AddPTPKoppelingCommand_CanExecute()
        {
            return true;
        }

        private void AddPTPKoppelingCommand_Executed()
        {
	        var inewname = 1;
			var ptp = new PTPKoppelingModel();
	        do
	        {
		        inewname++;
		        ptp.TeKoppelenKruispunt = "ptpkruising" + (inewname < 10 ? "0" : "") + inewname;
	        }
	        while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.PTPKruising, ptp.TeKoppelenKruispunt));
            var vm = new PTPKoppelingViewModel(ptp);
            PTPKoppelingen.Add(vm);
            SelectedPTPKoppeling = vm;
			WeakReferenceMessenger.Default.Send(new PTPKoppelingenChangedMessage());
        }

        private bool RemovePTPKoppelingCommand_CanExecute()
        {
            return SelectedPTPKoppeling != null;
        }

        private void RemovePTPKoppelingCommand_Executed()
        {
            PTPKoppelingen.Remove(SelectedPTPKoppeling);
            SelectedPTPKoppeling = PTPKoppelingen.FirstOrDefault();
			WeakReferenceMessenger.Default.Send(new PTPKoppelingenChangedMessage());
        }

        #endregion // Command Functionality

        #region Public methods

        #endregion // Public methods

        #region TabItem Overrides

        public override string DisplayName => "PTP";

        public override bool IsEnabled
        {
            get => true;
            set { }
        }

        public override void OnSelected()
        {
            if (Controller == null) return;
            foreach (var ptp in PTPKoppelingen)
            {
                ptp.KoppelSignalenAlles.Clear();
                ptp.KoppelSignalenAllesId = 0;
            }
            var signalen = GetAllKoppelSignalen(Controller);
            foreach (var s in signalen)
            {
                var ptp = PTPKoppelingen.FirstOrDefault(x => x.TeKoppelenKruispunt == s.Koppeling);
                if (ptp != null)
                {
                    ptp.KoppelSignalenAlles.Add(new KoppelSignaalViewModel(s, ptp.KoppelSignalenAllesId));
                    ++ptp.KoppelSignalenAllesId;
                }
            }
            foreach (var ptp in PTPKoppelingen) ptp.UpdateSignalen();
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {

                    PTPKoppelingen.CollectionChanged -= PTPKoppelingen_CollectionChanged;
                    PTPKoppelingen.Clear();
                    foreach (var ptp in _Controller.PTPData.PTPKoppelingen)
                    {
                        PTPKoppelingen.Add(new PTPKoppelingViewModel(ptp));
                    }
                    SelectedPTPKoppeling = PTPKoppelingen.FirstOrDefault();
                    PTPKoppelingen.CollectionChanged += PTPKoppelingen_CollectionChanged;
                }
                else
                {
                    PTPKoppelingen.CollectionChanged -= PTPKoppelingen_CollectionChanged;
                    PTPKoppelingen.Clear();
                }
            }
        }

        #endregion // TabItem Overrides

        private List<KoppelSignaalModel> GetAllKoppelSignalen(object obj)
        {
            var l = new List<KoppelSignaalModel>();
            if (obj == null) return l;

            var objType = obj.GetType();

            var ignore = objType.GetCustomAttribute<TLCGenIgnoreAttribute>();
            if (ignore != null) return l;

            var attr = objType.GetCustomAttribute<HasKoppelSignalenAttribute>();
            if (attr != null)
            {
                var i = (IHaveKoppelSignalen)obj;
                var elems2 = i.UpdateKoppelSignalen();
                l.AddRange(elems2);
            }

            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var ignoreP = property.GetCustomAttribute<TLCGenIgnoreAttribute>();
                if (ignoreP != null) continue;

                var hasSignalen = property.GetCustomAttribute<HasKoppelSignalenAttribute>();
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string)) continue;
                var propValue = property.GetValue(obj);
                var elems = propValue as IList;
                if (elems != null)
                {
                    l.AddRange(from object item in elems from i in GetAllKoppelSignalen(item) select i);
                }
                else if(hasSignalen != null)
                {
                    var i = (IHaveKoppelSignalen)propValue;
                    var elems2 = i.UpdateKoppelSignalen();
                    l.AddRange(elems2);
                }
                else
                {
                    l.AddRange(GetAllKoppelSignalen(propValue));
                }
            }
            return l;
        }

        #region Collection Changed

        private void PTPKoppelingen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (PTPKoppelingViewModel ptp in e.NewItems)
                {
                    _Controller.PTPData.PTPKoppelingen.Add(ptp.PTPKoppeling);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (PTPKoppelingViewModel ptp in e.OldItems)
                {
                    _Controller.PTPData.PTPKoppelingen.Remove(ptp.PTPKoppeling);
                }
            };
            WeakReferenceMessenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed
        
        #region Constructor

        public PTPKoppelingenTabViewModel() : base()
        {

        }

        #endregion // Constructor
    }
}
