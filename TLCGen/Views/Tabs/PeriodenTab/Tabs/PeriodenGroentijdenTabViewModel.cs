using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Integrity;
using TLCGen.Messaging.Messages;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Models.Enumerations;
using TLCGen.Plugins;
using TLCGen.Settings;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 0, type: TabItemTypeEnum.PeriodenTab)]
    public class PeriodenGroentijdenTabViewModel : TLCGenTabItemViewModel, IAllowTemplates<PeriodeModel>
    {
        #region Fields
        
        private PeriodeViewModel _SelectedPeriode;
        private ObservableCollection<string> _GroentijdenSets;

        #endregion // Fields

        #region Properties

        public ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel> Periodes
        {
            get; private set;
        }

        public ObservableCollection<string> GroentijdenSets
        {
            get
            {
                if (_GroentijdenSets == null)
                {
                    _GroentijdenSets = new ObservableCollection<string>();
                }
                return _GroentijdenSets;
            }
        }

        public PeriodeViewModel SelectedPeriode
        {
            get { return _SelectedPeriode; }
            set
            {
                _SelectedPeriode = value;
                RaisePropertyChanged("SelectedPeriode");
                TemplatesProviderVM.SetSelectedApplyToItem(value.Periode);
            }
        }

        public string DefaultPeriodeGroentijdenSet
        {
            get { return _Controller?.PeriodenData.DefaultPeriodeGroentijdenSet; }
            set
            {
                _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = value;
                RaisePropertyChanged<object>(nameof(DefaultPeriodeGroentijdenSet), null, null, true);
            }
        }

        public string DefaultPeriodeNaam
        {
            get { return _Controller?.PeriodenData.DefaultPeriodeNaam; }
            set
            {
	            var message = new IsElementIdentifierUniqueRequest(value, ElementIdentifierType.Naam);
		        Messenger.Default.Send(message);
	            if (message.IsUnique)
	            {
		            var oldName = _Controller.PeriodenData.DefaultPeriodeNaam;
					_Controller.PeriodenData.DefaultPeriodeNaam = value;
		            MessengerInstance.Send(new NameChangedMessage(oldName, value));
	            }
                RaisePropertyChanged<object>(nameof(DefaultPeriodeNaam), null, null, true);
            }
        }

        private TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel> _TemplatesProviderVM;
        public TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel> TemplatesProviderVM
        {
            get
            {
                if (_TemplatesProviderVM == null)
                {
                    _TemplatesProviderVM = new TemplateProviderViewModel<TLCGenTemplateModel<PeriodeModel>, PeriodeModel>(this);
                }
                return _TemplatesProviderVM;
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _AddPeriodeCommand;
        public ICommand AddPeriodeCommand
        {
            get
            {
                if (_AddPeriodeCommand == null)
                {
                    _AddPeriodeCommand = new RelayCommand(AddNewPeriodeCommand_Executed, AddNewPeriodeCommand_CanExecute);
                }
                return _AddPeriodeCommand;
            }
        }


        RelayCommand _RemovePeriodeCommand;
        public ICommand RemovePeriodeCommand
        {
            get
            {
                if (_RemovePeriodeCommand == null)
                {
                    _RemovePeriodeCommand = new RelayCommand(RemovePeriodeCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _RemovePeriodeCommand;
            }
        }

        RelayCommand _MovePeriodeUpCommand;
        public ICommand MovePeriodeUpCommand
        {
            get
            {
                if (_MovePeriodeUpCommand == null)
                {
                    _MovePeriodeUpCommand = new RelayCommand(MovePeriodeUpCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _MovePeriodeUpCommand;
            }
        }

        RelayCommand _MovePeriodeDownCommand;
        public ICommand MovePeriodeDownCommand
        {
            get
            {
                if (_MovePeriodeDownCommand == null)
                {
                    _MovePeriodeDownCommand = new RelayCommand(MovePeriodeDownCommand_Executed, ChangePeriodeCommand_CanExecute);
                }
                return _MovePeriodeDownCommand;
            }
        }

        #endregion // Commands

        #region Command Functionality

        private void MovePeriodeUpCommand_Executed(object obj)
        {
            var index = -1;
            foreach (var mvm in Periodes)
            {
                ++index;
                if (mvm == SelectedPeriode)
                {
                    break;
                }
            }
            if (index >= 1 && Periodes[index - 1].Type == Models.Enumerations.PeriodeTypeEnum.Groentijden)
            {
                var mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index - 1, mvm);
                SelectedPeriode = mvm;
                Periodes.RebuildList();
                Messenger.Default.Send(new PeriodenChangedMessage());
            }
        }

        private void MovePeriodeDownCommand_Executed(object obj)
        {
            var index = -1;
            
            foreach (var mvm in Periodes)
            {
                ++index;
                if (mvm == SelectedPeriode)
                {
                    break;
                }
            }
            if (index >= 0 && (index <= (Periodes.Count - 2)) && Periodes[index + 1].Type == Models.Enumerations.PeriodeTypeEnum.Groentijden)
            {
                var mvm = SelectedPeriode;
                SelectedPeriode = null;
                Periodes.Remove(mvm);
                Periodes.Insert(index + 1, mvm);
                SelectedPeriode = mvm;
                Periodes.RebuildList();
                Messenger.Default.Send(new PeriodenChangedMessage());
            }

        }

        void AddNewPeriodeCommand_Executed(object prm)
        {
            var mm = new PeriodeModel();
            mm.Type = PeriodeTypeEnum.Groentijden;
            mm.DagCode = PeriodeDagCodeEnum.AlleDagen;
	        var inewname = Periodes.Count;
	        IsElementIdentifierUniqueRequest message;
	        do
	        {
		        inewname++;
				mm.Naam = "periode" + (inewname < 10 ? "0" : "") + inewname;
		        message = new IsElementIdentifierUniqueRequest(mm.Naam, ElementIdentifierType.Naam);
		        Messenger.Default.Send(message);
	        }
	        while (!message.IsUnique);
			var mvm = new PeriodeViewModel(mm);

            if (Periodes.Any(x => x.Type == PeriodeTypeEnum.Groentijden))
            {
                var index = Periodes.Count(x => x.Type == PeriodeTypeEnum.Groentijden);
                Periodes.Insert(index, mvm);
            }
            else
            {
                Periodes.Insert(0, mvm);
            }
	        Messenger.Default.Send(new PeriodenChangedMessage());
		}

		bool AddNewPeriodeCommand_CanExecute(object prm)
        {
            return Periodes != null && GroentijdenSets?.Count > 0;
        }

        void RemovePeriodeCommand_Executed(object prm)
        {
			TLCGenControllerModifier.Default.RemoveModelItemFromController(SelectedPeriode.Naam);
	        Periodes.Remove(SelectedPeriode);
	        SelectedPeriode = null;
	        Messenger.Default.Send(new PeriodenChangedMessage());
		}

		bool ChangePeriodeCommand_CanExecute(object prm)
        {
            return SelectedPeriode != null;
        }

        #endregion // Command Functionality

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Groentijden";
            }
        }

        public override bool IsEnabled
        {
            get { return true; }
            set { }
        }

        public override void OnSelected()
        {
            var v = _Controller.PeriodenData.DefaultPeriodeGroentijdenSet;
            GroentijdenSets.Clear();
            foreach (var gsm in _Controller.GroentijdenSets)
            {
                GroentijdenSets.Add(gsm.Naam);
            }
            _Controller.PeriodenData.DefaultPeriodeGroentijdenSet = v;
            RaisePropertyChanged("DefaultPeriodeGroentijdenSet");
        }

        public override ControllerModel Controller
        {
            get
            {
                return base.Controller;
            }

            set
            {
                base.Controller = value;
                if (Periodes != null)
                {
                    Periodes.CollectionChanged -= Periodes_CollectionChanged;
                }
                if (base.Controller != null)
                {
                    Periodes = new ObservableCollectionAroundList<PeriodeViewModel, PeriodeModel>(base.Controller.PeriodenData.Perioden);
                    Periodes.CollectionChanged += Periodes_CollectionChanged;
                    var view = CollectionViewSource.GetDefaultView(Periodes);
                    view.Filter = FilterPerioden;
                }
                else
                {
                    Periodes = null;
                }
                RaisePropertyChanged("Periodes");
            }
        }

        #endregion // TabItem Overrides

        #region Private Methods

        private bool FilterPerioden(object o)
        {
            var per = (PeriodeViewModel)o;
            return per.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden;
        }

        #endregion // Private Methods

        #region Collection Changed

        private void Periodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Messenger.Default.Send(new ControllerDataChangedMessage());
        }

        #endregion // Collection Changed

        #region IAllowTemplates

        public void InsertItemsFromTemplate(List<PeriodeModel> items)
        {
            if (_Controller == null)
                return;

            foreach (var per in items)
            {
                if (!(TLCGenIntegrityChecker.IsElementNaamUnique(_Controller, per.Naam)))
                {
                    MessageBox.Show("Error bij toevoegen van periode met naam " + per.Naam + ".\nNaam van de periode is niet uniek in de regeling.", "Error bij toepassen template");
                    return;
                }
                if(_Controller.GroentijdenSets.All(x => x.Naam != per.GroentijdenSet))
                {
                    MessageBox.Show("Error bij toevoegen van periode verwijzend naar groentijdenset " + per.GroentijdenSet + ".\nDeze groentijdenset ontbreekt in de regeling.", "Error bij toepassen template");
                    return;
                }
            }
            foreach (var per in items)
            {
                Periodes.Add(new PeriodeViewModel(per));
            }
	        Messenger.Default.Send(new PeriodenChangedMessage());
        }

        public void UpdateAfterApplyTemplate(PeriodeModel item)
        {
            throw new NotImplementedException();
        }

        #endregion // IAllowTemplates

        #region TLCGen Events

        private void OnPeriodenChanged(PeriodenChangedMessage message)
        {
            var sel = SelectedPeriode;
            Periodes.Rebuild();
            if (sel != null)
            {
                foreach (var p in Periodes)
                {
                    if (sel.Naam == p.Naam)
                    {
                        SelectedPeriode = p;
                    }
                }
            }
        }

        #endregion // TLCGen Events

        #region Constructor

        public PeriodenGroentijdenTabViewModel() : base()
        {
            Messenger.Default.Register(this, new Action<PeriodenChangedMessage>(OnPeriodenChanged));
        }

        #endregion // Constructor
    }
}
