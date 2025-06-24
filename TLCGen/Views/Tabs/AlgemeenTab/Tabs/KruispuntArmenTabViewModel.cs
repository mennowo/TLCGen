using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
    [TLCGenTabItem(index: 6, type: TabItemTypeEnum.AlgemeenTab)]
    public class KruispuntArmenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private RelayCommand _addKruispuntArmCommand;
        private RelayCommand _removeKruispuntArmCommand;
        private KruispuntArmViewModel _selectedKruispuntArm;

        #endregion // Fields

        #region TabItem Overrides

        public override string DisplayName => "Kruispunt armen";

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    if (KruispuntArmen != null) KruispuntArmen.CollectionChanged -= KruispuntArmen_CollectionChanged;
                    KruispuntArmen = new ObservableCollectionAroundList<KruispuntArmViewModel, KruispuntArmModel>(value.Kruispunt.KruispuntArmen);
                    KruispuntArmen.CollectionChanged += KruispuntArmen_CollectionChanged;
                    OnPropertyChanged(nameof(KruispuntArmen));

                    FasenMetKruispuntArmen = new ObservableCollectionAroundList<KruispuntArmFaseCyclusViewModel, KruispuntArmFaseCyclusModel>(value.Kruispunt.FasenMetKruispuntArmen);
                    foreach (var fc in value.Fasen)
                    {
                        if (FasenMetKruispuntArmen.All(x => x.FaseCyclus != fc.Naam))
                        {
                            FasenMetKruispuntArmen.Add(new KruispuntArmFaseCyclusViewModel(new KruispuntArmFaseCyclusModel
                            {
                                FaseCyclus = fc.Naam,
                                KruispuntArm = "NG",
                                KruispuntArmVolg = "NG"
                            }));
                        }
                    }
                    OnPropertyChanged(nameof(FasenMetKruispuntArmen));

                    UpdateSelectables();
                }
                else if (KruispuntArmen != null)
                {
                    KruispuntArmen.CollectionChanged -= KruispuntArmen_CollectionChanged;
                    KruispuntArmen.Clear();
                }
                _addKruispuntArmCommand?.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollectionAroundList<KruispuntArmViewModel, KruispuntArmModel> KruispuntArmen { get; private set; }

        public ObservableCollectionAroundList<KruispuntArmFaseCyclusViewModel, KruispuntArmFaseCyclusModel> FasenMetKruispuntArmen { get; private set; }

        public List<string> SelectableKruispuntArmen { get; set; }

        public ICommand AddKruispuntArmCommand => _addKruispuntArmCommand ??= new RelayCommand(() =>
        {
            var k = 1;
            var name = "ARM" + (KruispuntArmen.Count + k);
            while (!TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.KruispuntArm, name))
            {
                ++k;
                name = "ARM" + (KruispuntArmen.Count + k);
            }
            KruispuntArmen.Add(new KruispuntArmViewModel(new KruispuntArmModel
            {
                Naam = name,
                Omschrijving = ""
            }));
            UpdateSelectables();
        }, () => Controller != null);

        public ICommand RemoveKruispuntArmCommand => _removeKruispuntArmCommand ??= new RelayCommand(() =>
        {
            // Remove kruispunt arm from relevant fasen
            foreach (var fase in FasenMetKruispuntArmen)
            {
                if (fase.KruispuntArm == SelectedKruispuntArm.Naam)
                {
                    fase.KruispuntArm = "NG";
                }
                if (fase.HasVolgArm && fase.KruispuntArmVolg == SelectedKruispuntArm.Naam)
                {
                    fase.KruispuntArmVolg = "NG";
                }
            }
            var k = KruispuntArmen.IndexOf(SelectedKruispuntArm);
            KruispuntArmen.Remove(SelectedKruispuntArm);
            if (KruispuntArmen.Count > 0 && k < KruispuntArmen.Count) SelectedKruispuntArm = KruispuntArmen[k];
            else SelectedKruispuntArm = null;
            UpdateSelectables();
        }, () => SelectedKruispuntArm != null);

        public KruispuntArmViewModel SelectedKruispuntArm
        {
            get => _selectedKruispuntArm;
            set
            {
                _selectedKruispuntArm = value;
                OnPropertyChanged();
                _removeKruispuntArmCommand?.NotifyCanExecuteChanged();
            }
        }

        private void KruispuntArmen_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            WeakReferenceMessengerEx.Default.Send(new ControllerDataChangedMessage());
        }

        private void UpdateSelectables()
        {
            SelectableKruispuntArmen = new List<string> { "NG" };
            if (KruispuntArmen != null)
            {
                foreach (var arm in KruispuntArmen)
                {
                    SelectableKruispuntArmen.Add(arm.Naam);
                }
                foreach (var fc in FasenMetKruispuntArmen)
                {
                    if (!SelectableKruispuntArmen.Contains(fc.KruispuntArm)) fc.KruispuntArm = "NG";
                    if (!SelectableKruispuntArmen.Contains(fc.KruispuntArmVolg)) fc.KruispuntArmVolg = "NG";
                }
            }
            OnPropertyChanged(nameof(SelectableKruispuntArmen));
        }

        public override bool CanBeEnabled()
        {
            return _Controller?.Data.TraffickCompatible == true;
        }

        #endregion // TabItem Overrides

        #region Constructor

        public KruispuntArmenTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            UpdateSelectables();
        }

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            if (message.AddedFasen != null && message.AddedFasen.Count > 0)
            {
                foreach (var fc in message.AddedFasen)
                {
                    FasenMetKruispuntArmen.Add(new KruispuntArmFaseCyclusViewModel(new KruispuntArmFaseCyclusModel
                    {
                        FaseCyclus = fc.Naam,
                        KruispuntArm = "NG",
                        KruispuntArmVolg = "NG"
                    }));
                }
            }

            if (message.RemovedFasen != null && message.RemovedFasen.Count > 0)
            {
                foreach (var fc in message.RemovedFasen)
                {
                    var afc = FasenMetKruispuntArmen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (afc != null)
                    {
                        FasenMetKruispuntArmen.Remove(afc);
                    }
                }
            }
        }

        #endregion // Constructor
    }
}