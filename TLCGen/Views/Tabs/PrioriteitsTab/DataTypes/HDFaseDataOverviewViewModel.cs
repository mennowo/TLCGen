using System.Linq;
using GalaSoft.MvvmLight;
using TLCGen.Extensions;
using TLCGen.Messaging.Messages;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class OVHDFaseDataOverviewViewModel : ViewModelBase
	{
		#region Fields

		private FaseCyclusModel _faseCyclus;
		private OVIngreepViewModel _OVIngreep;
		private HDIngreepViewModel _HDIngreep;
		private ControllerModel _controller;
		private HDOverzichtTabViewModel _overVM;

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
                        HDIngreep.PropertyChanged += _overVM.HDIngreep_PropertyChanged;
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
                        _controller.PrioData.HDIngrepen.Add(hd);
                        _controller.PrioData.HDIngrepen.BubbleSort();
                        HDIngreep = new HDIngreepViewModel(_controller, hd);
                        HDIngreep.PropertyChanged += _overVM.HDIngreep_PropertyChanged;
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
				MessengerInstance.Send(new PrioIngrepenChangedMessage());
				Integrity.TLCGenControllerModifier.Default.CorrectModel_AlteredHDIngrepen();
				RaisePropertyChanged();
			}
		}

		public HDIngreepViewModel HDIngreep
		{
			get => _HDIngreep;
			set
			{
				_HDIngreep = value;
				RaisePropertyChanged();
			}
		}

		#endregion // Properties

		#region Constructor

		public OVHDFaseDataOverviewViewModel(FaseCyclusModel faseCyclus, HDOverzichtTabViewModel overvm, ControllerModel controller)
		{
			_faseCyclus = faseCyclus;
			_controller = controller;
			_overVM = overvm;
			if (_faseCyclus.HDIngreep)
			{
				var hdi = controller.PrioData.HDIngrepen.FirstOrDefault(x => x.FaseCyclus == _faseCyclus.Naam);
				if (hdi != null)
				{
					HDIngreep = new HDIngreepViewModel(controller, hdi);
					HDIngreep.PropertyChanged += _overVM.HDIngreep_PropertyChanged;
				}
			}
		}

		#endregion // Constructor
	}
}
