using System.Windows.Media;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public abstract class TLCGenTabItemViewModel : ObservableObjectEx, ITLCGenTabItem
    {
        #region Fields

        protected ControllerModel _Controller;
        protected bool _IsEnabled;

        #endregion // Fields

        #region Properties

        public abstract string DisplayName { get; }
        public virtual bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public virtual bool Visibility { get; set; } = true;

        public virtual System.Windows.DataTemplate ContentDataTemplate => null;

        public virtual ControllerModel Controller
        {
            get => _Controller;
            set
            {
                _Controller = value;
                if(value != null) OnPropertyChanged("");
            }
        }

		public virtual ImageSource Icon => null;

	    public virtual bool OnSelectedPreview()
        {
            return true;
        }

        public virtual void OnSelected()
        {
        }

        public virtual bool OnDeselectedPreview()
        {
            return true;
        }

        public virtual void OnDeselected()
        {
        }

        public virtual string GetPluginName()
        {
            return "Non named tab";
        }

        public virtual bool CanBeEnabled()
        {
            return true;
        }

        #endregion // Properties

        #region Public Methods

        public virtual void LoadTabs()
        {

        }

        #endregion // Public Methods

        #region Constructor

        public TLCGenTabItemViewModel()
        {
        }
        #endregion // Constructor
    }
}