using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TLCGen.Messaging;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.ViewModels
{
    public abstract class TLCGenTabItemViewModel : ViewModelBase, ITLCGenTabItem
    {
        #region Fields

        protected ControllerModel _Controller;
        protected bool _IsEnabled;

        #endregion // Fields

        #region Properties

        public abstract string DisplayName { get; }
        public virtual bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public virtual System.Windows.DataTemplate ContentDataTemplate { get { return null; } }

        public ControllerModel Controller
        {
            get { return _Controller; }
            set { _Controller = value; }
        }

        public virtual ImageSource Icon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        #region Constructor

        public TLCGenTabItemViewModel(ControllerModel controller)
        {
            _Controller = controller;
        }

        #endregion // Constructor
    }
}
