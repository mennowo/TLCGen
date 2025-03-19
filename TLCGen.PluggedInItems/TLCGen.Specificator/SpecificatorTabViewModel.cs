using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Helpers;

namespace TLCGen.Specificator
{
    public class SpecificatorTabViewModel : ObservableObjectEx
    {
        #region Fields

        private SpecificatorDataModel _data;
        private RelayCommand _addParagraafCommand;
        private RelayCommand _removeParagraafCommand;
        private RelayCommand _moveParagraafUpCommand;
        private RelayCommand _moveParagraafDownCommand;
        private SpecificatorSpecialsParagraafViewModel _selectedSpecialsParagraaf;

        #endregion // Fields

        #region Properties

        public SpecificatorDataModel Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged("");
            }
        }

        public string Organisatie
        {
            get => Data?.Organisatie;
            set
            {
                Data.Organisatie = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Straat
        {
            get => Data?.Straat;
            set
            {
                Data.Straat = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Postcode
        {
            get => Data?.Postcode;
            set
            {
                Data.Postcode = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Stad
        {
            get => Data?.Stad;
            set
            {
                Data.Stad = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string TelefoonNummer
        {
            get => Data.TelefoonNummer;
            set
            {
                Data.TelefoonNummer = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string EMail
        {
            get => Data.EMail;
            set
            {
                Data.EMail = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public string Website
        {
            get => Data.Website;
            set
            {
                Data.Website = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public SpecificatorSpecialsParagraafViewModel SelectedSpecialsParagraaf
        {
            get => _selectedSpecialsParagraaf;
            set
            {
                _selectedSpecialsParagraaf = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollectionAroundList<SpecificatorSpecialsParagraafViewModel, SpecificatorSpecialsParagraaf> SpecialsParagrafen { get; set; }

        #endregion // Properties

        #region Commands

        public ICommand AddParagraafCommand => _addParagraafCommand ?? (_addParagraafCommand = new RelayCommand(() =>
        {
            var par = new SpecificatorSpecialsParagraafViewModel(new SpecificatorSpecialsParagraaf { Titel = "Paragraaf titel", Text = "Paragraaf text" });
            SpecialsParagrafen.Add(par);
            SelectedSpecialsParagraaf = par;
            WeakReferenceMessenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        }));

        public ICommand RemoveParagraafCommand => _removeParagraafCommand ?? (_removeParagraafCommand = new RelayCommand(() =>
        {
            var index = SpecialsParagrafen.IndexOf(SelectedSpecialsParagraaf);
            SpecialsParagrafen.Remove(SelectedSpecialsParagraaf);
            SelectedSpecialsParagraaf = null;
            if (SpecialsParagrafen.Count > 0)
            {
                if (index >= SpecialsParagrafen.Count)
                {
                    SelectedSpecialsParagraaf = SpecialsParagrafen[SpecialsParagrafen.Count - 1];
                }
                else
                {
                    SelectedSpecialsParagraaf = SpecialsParagrafen[index];
                }
            }
            WeakReferenceMessenger.Default.Send(new Messaging.Messages.ControllerDataChangedMessage());
        },
        () => SelectedSpecialsParagraaf != null));

        public ICommand MoveParagraafUpCommand => _moveParagraafUpCommand ?? (_moveParagraafUpCommand = new RelayCommand(() =>
        {
            var index = -1;
            foreach (var mvm in SpecialsParagrafen)
            {
                ++index;
                if (mvm == SelectedSpecialsParagraaf)
                {
                    break;
                }
            }
            if (index >= 1)
            {
                var mvm = SelectedSpecialsParagraaf;
                SelectedSpecialsParagraaf = null;
                SpecialsParagrafen.Remove(mvm);
                SpecialsParagrafen.Insert(index - 1, mvm);
                SelectedSpecialsParagraaf = mvm;
            }
        },
        () => SelectedSpecialsParagraaf != null));

        public ICommand MoveParagraafDownCommand => _moveParagraafDownCommand ?? (_moveParagraafDownCommand = new RelayCommand(() =>
        {
            var index = -1;
            foreach (var mvm in SpecialsParagrafen)
            {
                ++index;
                if (mvm == SelectedSpecialsParagraaf)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (SpecialsParagrafen.Count - 2)))
            {
                var mvm = SelectedSpecialsParagraaf;
                SelectedSpecialsParagraaf = null;
                SpecialsParagrafen.Remove(mvm);
                SpecialsParagrafen.Insert(index + 1, mvm);
                SelectedSpecialsParagraaf = mvm;
            }
        },
        () => SelectedSpecialsParagraaf != null));

        #endregion // Commands

        #region Constructor

        public SpecificatorTabViewModel()
        {
        }

        #endregion // Constructor
    }
}
