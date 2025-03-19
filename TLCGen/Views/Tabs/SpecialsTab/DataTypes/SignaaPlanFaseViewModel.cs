using System;
using CommunityToolkit.Mvvm.ComponentModel;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class SignaalPlanFaseViewModel : ObservableObjectEx, IViewModelWithItem, IComparable
    {

        #region Fields

	    private SignaalPlanFaseModel _Fase;

        #endregion // Fields

        #region Properties

        public SignaalPlanFaseModel Fase => _Fase;

	    public string FaseCyclus => Fase.FaseCyclus;

        public int? A1
        {
            get => _Fase.A1;
	        set
            {
                _Fase.A1 = value;
				OnPropertyChanged(broadcast: true);
			}
        }
        public int B1
        {
            get => _Fase.B1;
	        set
            {
                _Fase.B1 = value;
				OnPropertyChanged(broadcast: true);
			}
        }
        public int? C1
        {
            get => _Fase.C1;
	        set
            {
                _Fase.C1 = value;
				OnPropertyChanged(broadcast: true);
			}
        }
        public int D1
        {
            get => _Fase.D1;
	        set
            {
                _Fase.D1 = value;
				OnPropertyChanged(broadcast: true);
			}
        }
        public int? E1
        {
            get => _Fase.E1;
	        set
            {
                _Fase.E1 = value;
				OnPropertyChanged(broadcast: true);
			}
        }
        public int? A2
        {
            get => _Fase.A2;
	        set
            {
                _Fase.A2 = value;
				OnPropertyChanged(broadcast: true);
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            OnPropertyChanged(nameof(B2), broadcast: true);
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            OnPropertyChanged(nameof(D2), broadcast: true);
	            }
			}
        }
        public int? B2
        {
            get => _Fase.B2;
	        set
            {
                _Fase.B2 = value;
	            if (value != null)
	            {
		            if (D2 == null)
		            {
			            _Fase.D2 = 0;
			            OnPropertyChanged(nameof(D2), broadcast: true);
		            }
	            }
	            else
	            {
		            _Fase.A2 = null;
		            _Fase.C2 = null;
		            _Fase.D2 = null;
		            _Fase.E2 = null;
		            OnPropertyChanged(nameof(A2), broadcast: true);
		            OnPropertyChanged(nameof(C2));
		            OnPropertyChanged(nameof(D2));
		            OnPropertyChanged(nameof(E2));
	            }
                OnPropertyChanged(broadcast: true);
            }
        }
        public int? C2
        {
            get => _Fase.C2;
	        set
            {
                _Fase.C2 = value;
				OnPropertyChanged(broadcast: true);
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            OnPropertyChanged(nameof(B2), broadcast: true);
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            OnPropertyChanged(nameof(D2), broadcast: true);
	            }
			}
        }
        public int? D2
        {
            get => _Fase.D2;
	        set
            {
                _Fase.D2 = value;
	            if (value != null)
	            {
		            if (B2 == null)
		            {
			            _Fase.B2 = 0;
			            OnPropertyChanged(nameof(B2), broadcast: true);
		            }
	            }
	            else
	            {
		            _Fase.A2 = null;
		            _Fase.B2 = null;
		            _Fase.C2 = null;
		            _Fase.E2 = null;
		            OnPropertyChanged(nameof(A2), broadcast: true);
		            OnPropertyChanged(nameof(B2));
		            OnPropertyChanged(nameof(C2));
		            OnPropertyChanged(nameof(E2));
	            }
	            OnPropertyChanged(broadcast: true);
			}
        }
        public int? E2
        {
            get => _Fase.E2;
	        set
            {
                _Fase.E2 = value;
				OnPropertyChanged(broadcast: true);
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            OnPropertyChanged(nameof(B2), broadcast: true);
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            OnPropertyChanged(nameof(D2), broadcast: true);
	            }
			}
        }

	    #endregion // Properties

		#region Commands
		#endregion // Commands

		#region Command Functionality
		#endregion // Command Functionality

		#region Private Methods
		#endregion // Private Methods

		#region Public Methods
		#endregion // Public Methods

		#region Constructor

	    public SignaalPlanFaseViewModel(SignaalPlanFaseModel fase)
	    {
		    _Fase = fase;
	    }

	    #endregion // Constructor

		#region IViewModelWithItem

	    public object GetItem()
	    {
		    return Fase;
	    }

		#endregion // IViewModelWithItem

		#region IComparable

		public int CompareTo(object obj)
		{
			return string.Compare(FaseCyclus, ((SignaalPlanFaseViewModel) obj).FaseCyclus, StringComparison.Ordinal);
		}

		#endregion // IComparable
    }
}
