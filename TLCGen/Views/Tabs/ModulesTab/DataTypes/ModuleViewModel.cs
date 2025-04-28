using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Models;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;


namespace TLCGen.ViewModels
{
    public class ModuleViewModel : ObservableObjectEx
    {
        #region Fields

        private ModuleModel _Module;
        private ObservableCollection<ModuleFaseCyclusViewModel> _Fasen;

        #endregion // Fields

        #region Properties

        public ModuleModel Module => _Module;

        public string Naam
        {
            get => _Module.Naam;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _Module.Naam = value;
                }
                OnPropertyChanged(nameof(Naam), broadcast: true);
            }
        }

        public ObservableCollection<ModuleFaseCyclusViewModel> Fasen
        {
            get
            {
                if(_Fasen == null)
                {
                    _Fasen = new ObservableCollection<ModuleFaseCyclusViewModel>();
                }
                return _Fasen;
            }
        }

        #endregion // Properties

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (ModuleFaseCyclusViewModel mfcvm in e.NewItems)
                {
                    _Module.Fasen.Add(mfcvm.ModuleFaseCyclus);
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (ModuleFaseCyclusViewModel mfcvm in e.OldItems)
                {
                    _Module.Fasen.Remove(mfcvm.ModuleFaseCyclus);
                }
            }
WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region Public Methods

        public void RemoveFase(ModuleFaseCyclusViewModel fc)
        {
            ModuleFaseCyclusViewModel _fc = null;
            foreach (var fc1 in Fasen)
            {
                if(fc1.FaseCyclusNaam == fc.FaseCyclusNaam)
                {
                    _fc = fc1;
                }
            }
            if(_fc != null) Fasen.Remove(_fc);
        }

        public void RemoveFase(string fcdefine)
        {
            ModuleFaseCyclusViewModel _fc = null;
            foreach (var fc1 in Fasen)
            {
                if (fc1.FaseCyclusNaam == fcdefine)
                {
                    _fc = fc1;
                }
            }
            if (_fc != null) Fasen.Remove(_fc);
        }

        #endregion // Public Methods

        #region Constructor

        public ModuleViewModel(ModuleModel module)
        {
            _Module = module;

            foreach (var mfcm in module.Fasen)
            {
                // Create ViewModel
                var mfcvm = new ModuleFaseCyclusViewModel(mfcm);

                // Add to list
                Fasen.Add(mfcvm);
            }
            Fasen.BubbleSort();
            Fasen.CollectionChanged += Fasen_CollectionChanged;
        }

        #endregion // Constructor
    }
}
