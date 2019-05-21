using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using TLCGen.Models;

namespace TLCGen.Plugins.Tools
{
    public class TemplateObject
    {
        public object Object { get; set; }
        public string Description { get; set; }

        public TemplateObject(object @object, string description)
        {
            Object = @object;
            Description = description;
        }
    }

    public class CombinatieTemplateItemViewModel : ViewModelBase
    {
        #region Fields

        private RelayCommand _checkTemplateCommand;
        private RelayCommand _applyItemFromControllerCommand;
        private bool _isObjectJsonOK;
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
                RaisePropertyChanged();
            }
        }

        public string Description
        {
            get => CombinatieTemplateItem.Description;
            set
            {
                CombinatieTemplateItem.Description = value;
                RaisePropertyChanged();
            }
        }

        public CombinatieTemplateItemTypeEnum Type
        {
            get => CombinatieTemplateItem.Type;
            set
            {
                CombinatieTemplateItem.Type = value;
                RaisePropertyChanged();
                SetSelectableItems();
            }
        }

        [Browsable(false)]
        public string ObjectJson
        {
            get => CombinatieTemplateItem.ObjectJson;
            set
            {
                CombinatieTemplateItem.ObjectJson = value;
                RaisePropertyChanged();
                CheckTemplateCommand.Execute(null);
            }
        }

        [Browsable(false)]
        public bool IsObjectJsonOK
        {
            get => _isObjectJsonOK;
            set
            {
                _isObjectJsonOK = value;
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public Brush Foreground => IsObjectJsonOK ? Brushes.DarkGreen : Brushes.DarkRed;

        #endregion // Properties

        #region Public Methods

        public void SetSelectableItems()
        {
            var c = TLCGen.DataAccess.TLCGenControllerDataProvider.Default.Controller;
            SelectableItems.Clear();
            if (c == null) return;
            switch (Type)
            {
                case CombinatieTemplateItemTypeEnum.Detector:
                    foreach(var d in c.GetAllDetectors())
                    {
                        SelectableItems.Add(new TemplateObject(d, d.Naam));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Naloop:
                    foreach (var nl in c.InterSignaalGroep.Nalopen)
                    {
                        SelectableItems.Add(new TemplateObject(nl, $"Naloop van {nl.FaseVan} naar {nl.FaseNaar}"));
                    }
                    break;
                case CombinatieTemplateItemTypeEnum.Meeaanvraag:
                    break;
                case CombinatieTemplateItemTypeEnum.Rateltikker:
                    break;
                case CombinatieTemplateItemTypeEnum.Gelijkstart:
                    break;
                case CombinatieTemplateItemTypeEnum.LateRelease:
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
                IsObjectJsonOK = true;
            }
            catch
            {
                IsObjectJsonOK = false;
            }
            RaisePropertyChanged(nameof(Foreground));
        }));

        public ICommand ApplyItemFromControllerCommand => _applyItemFromControllerCommand ?? (_applyItemFromControllerCommand = new RelayCommand(() =>
        {
            ObjectJson = JsonConvert.SerializeObject(SelectedItem.Object, Formatting.Indented);
        },
        () => SelectedItem != null));

        #endregion // Commands

        #region Constructor

        public CombinatieTemplateItemViewModel(CombinatieTemplateItemModel combinatieTemplateItem)
        {
            CombinatieTemplateItem = combinatieTemplateItem;
            SetSelectableItems();
        }

        #endregion // Constructor
    }
}
