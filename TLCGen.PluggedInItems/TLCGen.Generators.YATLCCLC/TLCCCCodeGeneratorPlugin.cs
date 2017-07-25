using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Generators.TLCCC.CodeGeneration;
using TLCGen.Generators.TLCCC.GeneratorUI;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Generators.TLCCC
{
    [TLCGenPlugin(TLCGenPluginElems.Generator | TLCGenPluginElems.PlugMessaging)]
    public class TLCCCCodeGeneratorPlugin : ITLCGenGenerator, ITLCGenPlugMessaging
    {
        #region ITLCGenGenerator

        public UserControl GeneratorView => _generatorView;

        public string GetGeneratorName()
        {
            return "TLCCC";
        }

        public string GetGeneratorVersion()
        {
            return "0.1";
        }

        public string GetPluginName()
        {
            return GetGeneratorName();
        }

        public ControllerModel Controller
        {
            get;
            set;
        }

        public void GenerateController()
        {
            if (_myVm.GenerateCodeCommand.CanExecute(null))
            {
                _myVm.GenerateCodeCommand.Execute(null);
            }
        }

        public bool CanGenerateController()
        {
            return false;
        }

        #endregion // ITLCGenGenerator

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            Messenger.Default.Register(this, new Action<Messaging.Messages.ControllerFileNameChangedMessage>(OnControllerFileNameChanged));
        }

        #endregion // ITLCGenPlugMessaging

        #region Fields

        private readonly UserControl _generatorView;
        private readonly TLCCCGenerator _generator;
        private readonly TLCCCGeneratorViewModel _myVm;

        #endregion // Fields

        #region Properties

        public string ControllerFileName { get; set; }

        #endregion // Properties

        #region Private Methods

        #endregion // Private Methods

        #region Properties

        #endregion // Properties

        #region TLCGen Events

        private void OnControllerFileNameChanged(Messaging.Messages.ControllerFileNameChangedMessage msg)
        {
            ControllerFileName = msg.NewFileName;
        }

        #endregion // TLCGen Events

        #region Constructor

        public TLCCCCodeGeneratorPlugin()
        {
            _generatorView = new TLCCCGeneratorView();
            _generator = new TLCCCGenerator();
            _myVm = new TLCCCGeneratorViewModel(this, _generator);
            _generatorView.DataContext = _myVm;
        }

        #endregion // Constructor

    }
}
