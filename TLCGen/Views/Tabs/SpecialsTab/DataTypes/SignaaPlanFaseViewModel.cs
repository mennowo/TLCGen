using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using TLCGen.Helpers;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class SignaalPlanFaseViewModel : ViewModelBase, IViewModelWithItem, IComparable
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
				RaisePropertyChanged();
			}
        }
        public int B1
        {
            get => _Fase.B1;
	        set
            {
                _Fase.B1 = value;
				RaisePropertyChanged();
			}
        }
        public int? C1
        {
            get => _Fase.C1;
	        set
            {
                _Fase.C1 = value;
				RaisePropertyChanged();
			}
        }
        public int D1
        {
            get => _Fase.D1;
	        set
            {
                _Fase.D1 = value;
				RaisePropertyChanged();
			}
        }
        public int? E1
        {
            get => _Fase.E1;
	        set
            {
                _Fase.E1 = value;
				RaisePropertyChanged();
			}
        }
        public int? A2
        {
            get => _Fase.A2;
	        set
            {
                _Fase.A2 = value;
				RaisePropertyChanged();
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            RaisePropertyChanged(nameof(B2));
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            RaisePropertyChanged(nameof(D2));
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
			            RaisePropertyChanged(nameof(D2));
		            }
	            }
	            else
	            {
		            _Fase.A2 = null;
		            _Fase.C2 = null;
		            _Fase.D2 = null;
		            _Fase.E2 = null;
		            RaisePropertyChanged(nameof(A2));
		            RaisePropertyChanged(nameof(C2));
		            RaisePropertyChanged(nameof(D2));
		            RaisePropertyChanged(nameof(E2));
	            }
	            RaisePropertyChanged();
            }
        }
        public int? C2
        {
            get => _Fase.C2;
	        set
            {
                _Fase.C2 = value;
				RaisePropertyChanged();
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            RaisePropertyChanged(nameof(B2));
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            RaisePropertyChanged(nameof(D2));
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
			            RaisePropertyChanged(nameof(B2));
		            }
	            }
	            else
	            {
		            _Fase.A2 = null;
		            _Fase.B2 = null;
		            _Fase.C2 = null;
		            _Fase.E2 = null;
		            RaisePropertyChanged(nameof(A2));
		            RaisePropertyChanged(nameof(B2));
		            RaisePropertyChanged(nameof(C2));
		            RaisePropertyChanged(nameof(E2));
	            }
	            RaisePropertyChanged();
			}
        }
        public int? E2
        {
            get => _Fase.E2;
	        set
            {
                _Fase.E2 = value;
				RaisePropertyChanged();
	            if (B2 == null)
	            {
		            _Fase.B2 = 0;
		            RaisePropertyChanged(nameof(B2));
	            }
	            if (D2 == null)
	            {
		            _Fase.D2 = 0;
		            RaisePropertyChanged(nameof(D2));
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
