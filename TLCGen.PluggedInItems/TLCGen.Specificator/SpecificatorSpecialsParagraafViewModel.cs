using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;

namespace TLCGen.Specificator
{
    public class SpecificatorSpecialsParagraafViewModel : ObservableObjectEx, IViewModelWithItem
    {
        public SpecificatorSpecialsParagraaf Paragraaf { get; }

        public string Titel
        {
            get => Paragraaf.Titel;
            set
            {
                Paragraaf.Titel = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Text
        {
            get => Paragraaf.Text;
            set
            {
                Paragraaf.Text = value;
                OnPropertyChanged(broadcast: true);
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
