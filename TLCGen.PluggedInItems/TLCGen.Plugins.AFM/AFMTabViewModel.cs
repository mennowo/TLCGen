using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TLCGen.Helpers;
using TLCGen.Plugins.AFM.Models;
using System;
using TLCGen.Messaging.Messages;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.Plugins.AFM
{
    public class AFMTabViewModel : ObservableObjectEx
    {
        #region Fields

        private AFMPlugin _plugin;

        #endregion // Fields

        #region Properties

        public List<string> AllFasen { get; } = new List<string>();
        private ObservableCollection<string> _SelectableDummyFasen = new ObservableCollection<string>();
        public ObservableCollection<string> SelectableDummyFasen => _SelectableDummyFasen;

        private AFMFaseCyclusDataViewModel _selectedAFMFase;
        public AFMFaseCyclusDataViewModel SelectedAFMFase
        {
            get => _selectedAFMFase;
            set
            {
                _selectedAFMFase = value;
                OnPropertyChanged();
            }
        }

        private AFMDataModel _afmModel;
        public AFMDataModel AfmModel
        {
            get => _afmModel;
            set
            {
                _afmModel = value;
                AFMFasen = new ObservableCollectionAroundList<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel>(_afmModel.AFMFasen);
            }
        }

        public bool AFMToepassen
        {
            get => _afmModel.AFMToepassen;
            set
            {
                _afmModel.AFMToepassen = value;
                OnPropertyChanged(broadcast: true);
            }
        }

        public ObservableCollectionAroundList<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel> AFMFasen { get; private set; }

        private AddRemoveItemsManager<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel, string> _AFMFasenManager;
        public AddRemoveItemsManager<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel, string> AFMFasenManager =>
            _AFMFasenManager ?? (_AFMFasenManager = new AddRemoveItemsManager<AFMFaseCyclusDataViewModel, AFMFaseCyclusDataModel, string>(
                AFMFasen,
                (x => new AFMFaseCyclusDataViewModel(new AFMFaseCyclusDataModel
                {
                    FaseCyclus = x,
                    DummyFaseCyclus = "NG",
                    MinimaleGroentijd = 6,
                    MaximaleGroentijd = 80
                })),
                ((x1, x2) => x1.FaseCyclus == x2)
                ));

        #endregion // Properties

        #region Commands

        #endregion // Commands

        #region TLCGen messaging

        private void OnFasenChanged(object sender, FasenChangedMessage msg)
        {
            if(msg.RemovedFasen != null && msg.RemovedFasen.Any())
            {
                foreach(var fc in msg.RemovedFasen)
                {
                    var afmFc = AFMFasen.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if(afmFc != null)
                    {
                        AFMFasen.Remove(afmFc);
                    }
                    var afmFcDummy = AFMFasen.FirstOrDefault(x => x.DummyFaseCyclus == fc.Naam);
                    if (afmFcDummy != null)
                    {
                        afmFcDummy.DummyFaseCyclus = "NG";
                    }
                }
            }
        }

        private void OnNameChanged(object sender, NameChangedMessage msg)
        {
            if(msg.ObjectType == TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase)
            {
                TLCGenModelManager.Default.ChangeNameOnObject(AfmModel, msg.OldName, msg.NewName, TLCGen.Models.Enumerations.TLCGenObjectTypeEnum.Fase);
                AFMFasen.Rebuild();
                AFMFasen.BubbleSort();
            }
        }

        #endregion // TLCGen messaging

        #region Public Methods

        public void UpdateMessaging()
        {
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
        }

        public void UpdateSelectableFasen(IEnumerable<string> allFasen)
        {
            if(allFasen != null)
            {
            //    AllFasen.Clear();
                _SelectableDummyFasen.Clear();
                _SelectableDummyFasen.Add("NG");
                foreach (var f in allFasen)
                {
            //        AllFasen.Add(f);
                    _SelectableDummyFasen.Add(f);
                }
            }
            //SelectableFasen.Clear();
            //foreach(var f in AllFasen)
            //{
            //    if(!AFMFasen.Any(x => x.FaseCyclus == f))
            //    {
            //        SelectableFasen.Add(f);
            //    }
            //}
            //if(SelectableFasen.Any()) SelectedFaseToAdd = SelectableFasen[0];
            AFMFasenManager.UpdateSelectables(allFasen);
        }

        #endregion // Public Methods

        #region Constructor

        public AFMTabViewModel(AFMPlugin plugin)
        {
            _plugin = plugin;
        }

        #endregion // Constructor
    }
}
