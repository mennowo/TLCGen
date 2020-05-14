namespace TLCGen.ViewModels
{
    public class GarantieTijdConvertHelper : GalaSoft.MvvmLight.ViewModelBase
    {
        private int _Van;
        private int _Tot;
        private int _Verschil;
        private int _MinVan;
        private SynchronisatiesTabViewModel _MainVM;

        public int Van
        {
            get => _Van;
            set
            {
                if (value >= MinVan && value <= Tot)
                    _Van = value;
                RaisePropertyChanged("Van");

                _MainVM.SetGarantieConvertValuesTot();
            }
        }

        public int Tot
        {
            get => _Tot;
            set
            {
                if (value >= Van)
                    _Tot = value;
                RaisePropertyChanged("Tot");

                _MainVM.SetGarantieConvertValuesVan();
            }
        }

        public int Verschil
        {
            get => _Verschil;
            set
            {
                _Verschil = value;
                RaisePropertyChanged("Verschil");
            }
        }

        public int MinVan
        {
            get => _MinVan;
            set
            {
                _MinVan = value;
                RaisePropertyChanged("MinVan");
            }
        }

        public GarantieTijdConvertHelper(SynchronisatiesTabViewModel mainvm)
        {
            _MainVM = mainvm;
        }
    }
}
