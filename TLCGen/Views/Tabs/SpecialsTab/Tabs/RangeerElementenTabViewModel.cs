using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.DataAccess;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;


namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 8, type: TabItemTypeEnum.SpecialsTab)]
    public class RangeerElementenTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<IOElementViewModel> _fasen;
        private ObservableCollection<IOElementViewModel> _detectoren;
        private ObservableCollection<IOElementViewModel> _outputs;
        private ObservableCollection<IOElementViewModel> _inputs;
        private ObservableCollection<IOElementViewModel> _selectieveDetectoren;

        #endregion // Fields

        #region Properties
        
        public bool RangerenFasen
        {
            get => _Controller?.Data?.RangeerData?.RangerenFasen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenFasen = value;
                OnPropertyChanged(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenDetectoren
        {
            get => _Controller?.Data?.RangeerData?.RangerenDetectoren ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenDetectoren = value;
                OnPropertyChanged(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenIngangen
        {
            get => _Controller?.Data?.RangeerData?.RangerenIngangen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenIngangen = value;
                OnPropertyChanged(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenUitgangen
        {
            get => _Controller?.Data?.RangeerData?.RangerenUitgangen ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenUitgangen = value;
                OnPropertyChanged(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public bool RangerenSelectieveDetectoren
        {
            get => _Controller?.Data?.RangeerData?.RangerenSelectieveDetectoren ?? false;
            set
            {
                _Controller.Data.RangeerData.RangerenSelectieveDetectoren = value;
                OnPropertyChanged(broadcast: true);
                UpdateRangeerIndices(_Controller);
            }
        }

        public ObservableCollection<IOElementViewModel> Fasen
        {
            get => _fasen;
            set
            {
                _fasen = value; 
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<IOElementViewModel> Detectoren
        {
            get => _detectoren;
            set
            {
                _detectoren = value; 
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<IOElementViewModel> Uitgangen
        {
            get => _outputs;
            set
            {
                _outputs = value; 
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IOElementViewModel> Ingangen
        {
            get => _inputs;
            set
            {
                _inputs = value; 
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IOElementViewModel> SelectieveDetectoren
        {
            get => _selectieveDetectoren;
            set
            {
                _selectieveDetectoren = value; 
                OnPropertyChanged();
            }
        }

        public IOElementModelListDropTarget DropTarget { get; } = new IOElementModelListDropTarget();

        #endregion // Properties

        #region TLCGen TabItem overrides

        public override string DisplayName => "Rangeren IO";

        public override void OnSelected()
        {
            UpdateRangeerIndices(_Controller);
        }

        public override ControllerModel Controller
        {
            get => base.Controller;

            set
            {
                base.Controller = value;
                if (base.Controller != null)
                {
                    UpdateModel(value);
                    UpdateRangeerIndices(value);
                }
                OnPropertyChanged("");
            }
        }

        #endregion // TLCGen TabItem overrides

        private void UpdateModel(ControllerModel controller)
        {
            // check and correct rangeren
            if (controller.Data.RangeerData != null)
            {
                var rd = controller.Data.RangeerData;
                var rangeerLists = new List<List<IOElementRangeerDataModel>>
                {
                    rd.RangeerFasen, rd.RangeerDetectoren, rd.RangeerSelectieveDetectoren,
                    rd.RangeerIngangen, rd.RangeerUitgangen
                };
                var elements = TLCGenControllerDataProvider.Default.CurrentGenerator.GetAllIOElements(controller);
                if (elements == null) return;

                var modelLists = new List<List<IOElementModel>>
                {
                    elements.Where(x => x.ElementType == IOElementTypeEnum.FaseCyclus).ToList(),
                    elements.Where(x => x.ElementType == IOElementTypeEnum.Detector).ToList(),
                    elements.Where(x => x.ElementType == IOElementTypeEnum.SelectiveDetector).ToList(),
                    elements.Where(x => x.ElementType == IOElementTypeEnum.Input).ToList(),
                    elements.Where(x => x.ElementType == IOElementTypeEnum.Output).ToList(),
                };
                var rem = new List<IOElementRangeerDataModel>();
                
                // check and fix potential doubles
                foreach (var list in rangeerLists)
                {
                    rem.Clear();
                    foreach (var model in list)
                    {
                        if (list.Any(x => !ReferenceEquals(model, x) && x.Naam == model.Naam) && rem.All(x => x.Naam != model.Naam))
                        {
                            rem.Add(model);
                        }
                    }

                    foreach (var r in rem) list.Remove(r);
                }

                // remove orphaned items
                for (var i = 0; i < rangeerLists.Count; i++)
                {
                    var modelList = modelLists[i];
                    rem.Clear();
                    foreach (var model in rangeerLists[i])
                    {
                        if (modelList.All(x => x.Naam != model.Naam))
                        {
                            rem.Add(model);
                        }
                    }
                    foreach (var r in rem) rangeerLists[i].Remove(r);
                }
            }
        }
        
        private void UpdateRangeerIndices(ControllerModel c)
        {
            if (!RangerenFasen && !RangerenDetectoren && !RangerenSelectieveDetectoren && !RangerenUitgangen && !RangerenIngangen) return;

            var elements = TLCGenControllerDataProvider.Default.CurrentGenerator.GetAllIOElements(c);
            if (elements == null) return;

            var vms = new (List<IOElementViewModel> items, IOElementTypeEnum type)[]
            {
                (new List<IOElementViewModel>(), IOElementTypeEnum.FaseCyclus),
                (new List<IOElementViewModel>(), IOElementTypeEnum.Detector),
                (new List<IOElementViewModel>(), IOElementTypeEnum.Input),
                (new List<IOElementViewModel>(), IOElementTypeEnum.Output),
                (new List<IOElementViewModel>(), IOElementTypeEnum.SelectiveDetector)
            };
            var models = new[]
            {
                c.Data.RangeerData.RangeerFasen,
                c.Data.RangeerData.RangeerDetectoren,
                c.Data.RangeerData.RangeerIngangen,
                c.Data.RangeerData.RangeerUitgangen,
                c.Data.RangeerData.RangeerSelectieveDetectoren
            };

            for (var i = 0; i < 5; i++)
            {
                if (models[i] == null) continue;

                // build viewmodel list
                foreach (var e in elements.Where(x => x != null && x.ElementType == vms[i].type))
                {
                    vms[i].items.Add(new IOElementViewModel(e));
                }
                
                // for regular detectors, also add selective detectors with non-dummy coupled regular detector
                if (i == 1 && models[4] != null)
                {
                    foreach (var e in elements.Where(x => x != null && x.ElementType == vms[4].type))
                    {
                        if (e is SelectieveDetectorModel { Dummy: false })
                        {
                            vms[i].items.Add(new IOElementViewModel(e));
                        }
                    }
                }

                // for each item, match with saved data
                foreach (var vm in vms[i].items)
                {
                    var model = models[i].FirstOrDefault(x => x != null && x.Naam == vm.Element.Naam);
                    if (model != null)
                    {
                        // for regular detector list, load secondary index for selective detectors
                        if (i == 1 && vm.Element is SelectieveDetectorModel)
                        {
                            vm.RangeerIndex = model.RangeerIndex2;
                            vm.UseSecondaryIndex = true;
                        }
                        else
                        {
                            vm.RangeerIndex = model.RangeerIndex;
                            vm.UseSecondaryIndex = false;
                        }

                        vm.SavedData = model;
                    }
                    else
                    {
                        var ind = models[i].Count;
                        var m = new IOElementRangeerDataModel
                        {
                            Naam = vm.Element.Naam,
                            RangeerIndex = ind,
                        };
                        models[i].Add(m);
                        vm.SavedData = m;
                    }
                }

                // clean up saved model items that are no longer present
                var remModels = models[i].Where(x => vms[i].items.All(x2 => x2.Element != null && x2.Element.Naam != x.Naam)).ToList();
                foreach (var r in remModels) models[i].Remove(r);
            }
            
            // update
            Fasen = new ObservableCollection<IOElementViewModel>(vms[0].items.OrderBy(x => x.RangeerIndex));
            Detectoren = new ObservableCollection<IOElementViewModel>(vms[1].items.OrderBy(x => x.RangeerIndex));
            Ingangen = new ObservableCollection<IOElementViewModel>(vms[2].items.OrderBy(x => x.RangeerIndex));
            Uitgangen = new ObservableCollection<IOElementViewModel>(vms[3].items.OrderBy(x => x.RangeerIndex));
            SelectieveDetectoren = new ObservableCollection<IOElementViewModel>(vms[4].items.OrderBy(x => x.RangeerIndex));
        }

        private void OnPrepareForGenerationRequestReceived(object sender, PrepareForGenerationRequest obj)
        {
            UpdateRangeerIndices(obj.Controller);
        }

        #region Constructor

        public RangeerElementenTabViewModel()
        {
            WeakReferenceMessengerEx.Default.Register<PrepareForGenerationRequest>(this, OnPrepareForGenerationRequestReceived);
        }

        #endregion // Constructor
    }
}
