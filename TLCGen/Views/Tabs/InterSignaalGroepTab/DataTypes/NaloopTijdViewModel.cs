using GalaSoft.MvvmLight;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Helpers;

namespace TLCGen.ViewModels
{
    public class NaloopTijdViewModel : ViewModelBase, IViewModelWithItem
    {
        private NaloopTijdModel _model;

        public NaloopTijdTypeEnum Type
        {
            get => _model.Type;
            set
            {
                _model.Type = value;
                RaisePropertyChanged<object>(nameof(Type), broadcast: true);
            }
        }
        public int Waarde
        {
            get => _model.Waarde;
            set
            {
                _model.Waarde = value;
                RaisePropertyChanged<object>(nameof(Waarde), broadcast: true);
            }
        }

        public object GetItem()
        {
            return _model;
        }

        public NaloopTijdViewModel(NaloopTijdModel model)
        {
            _model = model;
        }
    }
}
