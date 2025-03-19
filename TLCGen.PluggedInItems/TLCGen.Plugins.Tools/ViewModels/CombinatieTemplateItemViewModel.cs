using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class CombinatieTemplateItemViewModel : ObservableObject
    {
        #region Fields

        private RelayCommand _checkTemplateCommand;
        private RelayCommand _applyItemFromControllerCommand;
        private bool _isObjectJsonOk;
        private TemplateObject _selectedItem;

        #endregion // Fields

        #region Properties

        [Browsable(false)]
        public CombinatieTemplateItemModel CombinatieTemplateItem { get; }

        public ObservableCollection<TemplateObject> SelectableItems { get; } = new ObservableCollection<TemplateObject>();

        public TemplateObject SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => CombinatieTemplateItem.Description;
            set
            {
                CombinatieTemplateItem.Description = value;
                OnPropertyChanged();
            }
        }

        public CombinatieTemplateItemTypeEnum Type
        {
            get => CombinatieTemplateItem.Type;
            set
            {
                CombinatieTemplateItem.Type = value;
                OnPropertyChanged();
                SetSelectableItems();
            }
        }

        [Browsable(false)]
        public string ObjectJson
        {
            get => CombinatieTemplateItem.ObjectJson ?? "";
            set
            {
                CombinatieTemplateItem.ObjectJson = value;
                OnPropertyChanged();
                CheckTemplateCommand.Execute(null);
            }
        }

        [Browsable(false)]
        public bool IsObjectJsonOk
        {
            get => _isObjectJsonOk;
            set
            {
                _isObjectJsonOk = value;
                OnPropertyChanged();
            }
        }

        [Browsable(false)]
        public Brush Foreground => IsObjectJsonOk ? Brushes.DarkGreen : Brushes.DarkRed;

        #endregion // Properties

        #region Public Methods

        public void SetSelectableItems()
        {
            var c = DataAccess.TLCGenControllerDataProvider.Default.Controller;
            SelectableItems.Clear();
            if (c == null) return;
            switch (Type)
            {
                case CombinatieTemplateItemTypeEnum.Detector:
                    foreach(var d in c.GetAllDetectors())
                    {
                        SelectableItems.Add(new TemplateObject(d, CombinatieTemplateItemTypeEnum.Detector, d.Naam));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Naloop:
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        SelectableItems.Add(new TemplateObject(nl, CombinatieTemplateItemTypeEnum.Naloop, $"Naloop van {nl.FaseVan} naar {nl.FaseNaar}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    foreach (var ma in c.InterSignaalGroep.Meeaanvragen)
                    {
                        SelectableItems.Add(new TemplateObject(ma, CombinatieTemplateItemTypeEnum.Meeaanvraag, $"Meeaanvraag van {ma.FaseVan} naar {ma.FaseNaar}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Rateltikker:
                    foreach (var rt in c.Signalen.Rateltikkers)
                    {
                        SelectableItems.Add(new TemplateObject(rt, CombinatieTemplateItemTypeEnum.Rateltikker, $"Rateltikker van {rt.FaseCyclus}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    foreach (var gs in c.InterSignaalGroep.Gelijkstarten)
                    {
                        SelectableItems.Add(new TemplateObject(gs, CombinatieTemplateItemTypeEnum.Gelijkstart, $"Gelijkstart van {gs.FaseVan} naar {gs.FaseNaar}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.LateRelease:
                    foreach (var lr in c.InterSignaalGroep.LateReleases)
                    {
                        SelectableItems.Add(new TemplateObject(lr, CombinatieTemplateItemTypeEnum.LateRelease, $"Late release van {lr.FaseVan} naar {lr.FaseNaar}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.PrioIngreep:
                    foreach (var prio in c.PrioData.PrioIngrepen)
                    {
                        SelectableItems.Add(new TemplateObject(prio, CombinatieTemplateItemTypeEnum.PrioIngreep, $"Prio ingreep {prio.Naam} voor fase {prio.FaseCyclus}"));
                    }
                    break;
            }
        }

        #endregion // Public Methods

        #region Commands

        public ICommand CheckTemplateCommand => _checkTemplateCommand ?? (_checkTemplateCommand = new RelayCommand(() =>
        {
            try
            {
                switch (Type)
                {
                    case CombinatieTemplateItemTypeEnum.Detector:
                        JsonConvert.DeserializeObject<DetectorModel>(ObjectJson);
                        break;
                    case CombinatieTemplateItemTypeEnum.Naloop:
                        JsonConvert.DeserializeObject<NaloopModel>(ObjectJson);
                        break;
                    case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                        JsonConvert.DeserializeObject<MeeaanvraagModel>(ObjectJson);
                        break;
                    case CombinatieTemplateItemTypeEnum.Rateltikker:
                        JsonConvert.DeserializeObject<RatelTikkerModel>(ObjectJson);
                        break;
                    case CombinatieTemplateItemTypeEnum.Gelijkstart:
                        JsonConvert.DeserializeObject<GelijkstartModel>(ObjectJson);
                        break;
                    case CombinatieTemplateItemTypeEnum.LateRelease:
                        JsonConvert.DeserializeObject<LateReleaseModel>(ObjectJson);
                        break;
                }
                IsObjectJsonOk = true;
            }
            catch
            {
                IsObjectJsonOk = false;
            }
            OnPropertyChanged(nameof(Foreground));
        }));

        public ICommand ApplyItemFromControllerCommand => _applyItemFromControllerCommand ?? (_applyItemFromControllerCommand = new RelayCommand(() =>
        {
            ObjectJson = JsonConvert.SerializeObject(SelectedItem.Object, Formatting.Indented);
            Type = SelectedItem.Type;
        },
        () => SelectedItem != null));

        #endregion // Commands

        #region Constructor

        public CombinatieTemplateItemViewModel(CombinatieTemplateItemModel combinatieTemplateItem)
        {
            CombinatieTemplateItem = combinatieTemplateItem;
            SetSelectableItems();
            CheckTemplateCommand.Execute(null);
        }

        #endregion // Constructor
    }
}
