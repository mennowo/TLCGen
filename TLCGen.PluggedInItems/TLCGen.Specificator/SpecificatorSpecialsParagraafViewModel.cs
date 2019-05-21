using GalaSoft.MvvmLight;
using TLCGen.Helpers;

namespace TLCGen.Specificator
{
    public class SpecificatorSpecialsParagraafViewModel : ViewModelBase, IViewModelWithItem
    {
        public SpecificatorSpecialsParagraaf Paragraaf { get; }

        public string Titel
        {
            get => Paragraaf.Titel;
            set
            {
                Paragraaf.Titel = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public string Text
        {
            get => Paragraaf.Text;
            set
            {
                Paragraaf.Text = value;
                RaisePropertyChanged<object>(broadcast: true);
            }
        }

        public SpecificatorSpecialsParagraafViewModel(SpecificatorSpecialsParagraaf paragraaf)
        {
            Paragraaf = paragraaf;
        }

        public object GetItem()
        {
            return Paragraaf;
        }
    }
}
