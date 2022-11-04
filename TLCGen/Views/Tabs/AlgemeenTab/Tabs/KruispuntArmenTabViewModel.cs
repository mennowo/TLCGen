using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.ViewModels;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

namespace TLCGen.ViewModels;

public class KruispuntArmViewModel : ViewModelBase, IViewModelWithItem
{
    #region Fields

    

    #endregion // Fields

    #region Properties

    public KruispuntArmModel Model { get; }
    
    public string Naam
    {
        get => Model.Naam;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && NameSyntaxChecker.IsValidCName(value))
            {
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.KruispuntArm, value))
                {
                    var oldname = Model.Naam;
                    Model.Naam = value;

                    // Notify the messenger
                    MessengerInstance.Send(new NameChangingMessage(TLCGenObjectTypeEnum.KruispuntArm, oldname, value));
                }
            }
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    public string Omschrijving
    {
        get => Model.Omschrijving;
        set
        {
            Model.Omschrijving = value;
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    #endregion // Properties

    #region IViewModelWithItem
    
    public object GetItem()
    {
        return Model;
    }

    #endregion // IViewModelWithItem

    #region Constructor

    public KruispuntArmViewModel(KruispuntArmModel model)
    {
        Model = model;
    }

    #endregion // Constructor
}

public class KruispuntArmFaseCyclusViewModel : ViewModelBase, IViewModelWithItem
{
    #region Properties

    public KruispuntArmFaseCyclusModel Model { get; }

    public string FaseCyclus
    {
        get => Model.FaseCyclus;
        set
        {
            Model.FaseCyclus = value;
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    public string KruispuntArm
    {
        get => Model.KruispuntArm;
        set
        {
            Model.KruispuntArm = value;
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    public string KruispuntArmVolg
    {
        get => Model.KruispuntArmVolg;
        set
        {
            Model.KruispuntArmVolg = value;
            RaisePropertyChanged<object>(broadcast: true);
        }
    }

    #endregion // Properties

    #region IViewModelWithItem

    public object GetItem()
    {
        return Model;
    }

    #endregion // IViewModelWithItem

    #region Constructor

    public KruispuntArmFaseCyclusViewModel(KruispuntArmFaseCyclusModel model)
    {
        Model = model;
    }

    #endregion // Constructor
}

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
                RaisePropertyChanged(nameof(KruispuntArmen));

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
                RaisePropertyChanged(nameof(FasenMetKruispuntArmen));

                UpdateSelectables();
            }
            else if (KruispuntArmen != null)
            {
                KruispuntArmen.CollectionChanged -= KruispuntArmen_CollectionChanged;
                KruispuntArmen.Clear();
            }
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
            Naam = name, Omschrijving = ""
        }));
    }, () => Controller != null);
    
    public ICommand RemoveKruispuntArmCommand => _removeKruispuntArmCommand ??= new RelayCommand(() =>
    {
        var k = KruispuntArmen.IndexOf(SelectedKruispuntArm);
        KruispuntArmen.Remove(SelectedKruispuntArm);
        if (KruispuntArmen.Count > 0 && k < KruispuntArmen.Count) SelectedKruispuntArm = KruispuntArmen[k];
        else SelectedKruispuntArm = null;
    }, () => SelectedKruispuntArm != null);

    public KruispuntArmViewModel SelectedKruispuntArm
    {
        get => _selectedKruispuntArm;
        set
        {
            _selectedKruispuntArm = value;
            RaisePropertyChanged();
        }
    }

    private void KruispuntArmen_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        MessengerInstance.Send(new ControllerDataChangedMessage());
    }

    private void UpdateSelectables()
    {
        SelectableKruispuntArmen = new List<string>{ "NG" };
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
        RaisePropertyChanged(nameof(SelectableKruispuntArmen));
    }

    #endregion // TabItem Overrides

    #region Constructor

    public KruispuntArmenTabViewModel()
    {
        MessengerInstance.Register<FasenChangedMessage>(this, OnFasenChanged);
        UpdateSelectables();
    }

    private void OnFasenChanged(FasenChangedMessage message)
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