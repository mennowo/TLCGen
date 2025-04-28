using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Extensions;
using TLCGen.Helpers;
using TLCGen.Messaging.Messages;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class HDFaseDataOverviewViewModel : ObservableObject
	{
		#region Fields

		private readonly FaseCyclusModel _faseCyclus;
		private readonly ControllerModel _controller;
		private readonly HDOverzichtTabViewModel _overVm;
		private HDIngreepViewModel _hdIngreep;

		#endregion // Fields

		#region Properties

		public string FaseCyclusNaam => _faseCyclus.Naam;

        public bool HasHDIngreep
		{
			get => _faseCyclus.HDIngreep;
			set
			{
				_faseCyclus.HDIngreep = value;
				if (value)
				{
                    var hd = _controller.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
                    if (hd != null)
                    {
                        HDIngreep = new HDIngreepViewModel(_controller, hd);
                        HDIngreep.PropertyChanged += _overVm.HDIngreep_PropertyChanged;
                        /* Trick to add dummy detectors */
                        if (hd.KAR)
                        {
                            HDIngreep.KAR = true;
                        }
                    }
                    else
                    {
                        hd = new HDIngreepModel();
                        Settings.DefaultsProvider.Default.SetDefaultsOnModel(hd);
                        hd.FaseCyclus = _faseCyclus.Naam;
                        hd.DummyKARInmelding.Simulatie.FCNr = _faseCyclus.Naam;
                        hd.DummyKARUitmelding.Simulatie.FCNr = _faseCyclus.Naam;
                        _controller.PrioData.HDIngrepen.Add(hd);
                        _controller.PrioData.HDIngrepen.BubbleSort();
                        HDIngreep = new HDIngreepViewModel(_controller, hd);
                        HDIngreep.PropertyChanged += _overVm.HDIngreep_PropertyChanged;
                        /* Trick to add dummy detectors */
                        if (hd.KAR)
                        {
                            HDIngreep.KAR = true;
                        }
                    }
				}
				else
				{
					if (HDIngreep != null)
					{
						_controller.PrioData.HDIngrepen.Remove(HDIngreep.HDIngreep);
						HDIngreep = null;
					}
					else
					{
						var hdi = _controller.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
						if (hdi != null)
						{
							_controller.PrioData.HDIngrepen.Remove(hdi);
						}
					}
				}
				WeakReferenceMessengerEx.Default.Send(new PrioIngrepenChangedMessage());
				Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredHDIngrepen();
				OnPropertyChanged();
			}
		}

		public HDIngreepViewModel HDIngreep
		{
			get => _hdIngreep;
			set
			{
				_hdIngreep = value;
				OnPropertyChanged();
			}
		}

		#endregion // Properties

		#region Constructor

		public HDFaseDataOverviewViewModel(FaseCyclusModel faseCyclus, HDOverzichtTabViewModel overvm, ControllerModel controller)
		{
			_faseCyclus = faseCyclus;
			_controller = controller;
			_overVm = overvm;
			if (_faseCyclus.HDIngreep)
			{
				var hdi = controller.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
				if (hdi != null)
				{
					HDIngreep = new HDIngreepViewModel(controller, hdi);
					HDIngreep.PropertyChanged += _overVm.HDIngreep_PropertyChanged;
				}
			}
		}

		#endregion // Constructor
	}
}
