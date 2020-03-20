using GalaSoft.MvvmLight;
using TLCGen.Dependencies.Messaging.Messages;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class RISSystemITFViewModel : ViewModelBase, IViewModelWithItem
    {
        private readonly RISSystemITFModel _model;

        public string SystemITF
        {
            get => _model.SystemITF;
            set
            {
                var old = _model.SystemITF;
                _model.SystemITF = value;
                RaisePropertyChanged<object>(broadcast: true);
                MessengerInstance.Send(new SystemITFChangedMessage(old, _model.SystemITF));
            }
        }

        public RISSystemITFViewModel(RISSystemITFModel model)
        {
            _model = model;
        }

        public object GetItem()
        {
            return _model;
        }
    }
}
