using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Messaging.Messages;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Models.Enumerations;

namespace TLCGen.ViewModels
{
    public class PTPKoppelingViewModel : ViewModelBase
    {
        #region Fields

        private PTPKoppelingModel _PTPKoppeling;

        #endregion // Fields

        #region Properties

        public PTPKoppelingModel PTPKoppeling
        {
            get { return _PTPKoppeling; }
        }

        public string TeKoppelenKruispunt
        {
            get { return _PTPKoppeling.TeKoppelenKruispunt; }
            set
            {
                if (TLCGenModelManager.Default.IsElementIdentifierUnique(TLCGenObjectTypeEnum.PTPKruising, value))
                {
                    var oldname = _PTPKoppeling.TeKoppelenKruispunt;
		            _PTPKoppeling.TeKoppelenKruispunt = value;
		            RaisePropertyChanged<object>(nameof(TeKoppelenKruispunt), broadcast: true);
		            Messenger.Default.Send(new NameChangingMessage(TLCGenObjectTypeEnum.PTPKruising, oldname, value));
	            }
            }
        }
        public int AantalsignalenIn
        {
            get { return _PTPKoppeling.AantalsignalenIn; }
            set
            {
                _PTPKoppeling.AantalsignalenIn = value;
                RaisePropertyChanged<object>(nameof(AantalsignalenIn), broadcast: true);
            }
        }

        public int AantalsignalenUit
        {
            get { return _PTPKoppeling.AantalsignalenUit; }
            set
            {
                _PTPKoppeling.AantalsignalenUit = value;
                RaisePropertyChanged<object>(nameof(AantalsignalenUit), broadcast: true);
            }
        }

        public int PortnummerSimuatieOmgeving
        {
            get { return _PTPKoppeling.PortnummerSimuatieOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerSimuatieOmgeving = value;
                RaisePropertyChanged<object>(nameof(PortnummerSimuatieOmgeving), broadcast: true);
            }
        }

        public int PortnummerAutomaatOmgeving
        {
            get { return _PTPKoppeling.PortnummerAutomaatOmgeving; }
            set
            {
                _PTPKoppeling.PortnummerAutomaatOmgeving = value;
                RaisePropertyChanged<object>("PortnummerAutomaatOmgeving", broadcast: true);
            }
        }

        public int NummerSource
        {
            get { return _PTPKoppeling.NummerSource; }
            set
            {
                _PTPKoppeling.NummerSource = value;
                RaisePropertyChanged<object>("NummerSource", broadcast: true);
            }
        }

        public int NummerDestination
        {
            get { return _PTPKoppeling.NummerDestination; }
            set
            {
                _PTPKoppeling.NummerDestination = value;
                RaisePropertyChanged<object>("NummerDestination", broadcast: true);
            }
        }

        #endregion // Properties

        #region Constructor

        public PTPKoppelingViewModel(PTPKoppelingModel kop)
        {
            _PTPKoppeling = kop;
        }

        #endregion // Constructor
    }
}
