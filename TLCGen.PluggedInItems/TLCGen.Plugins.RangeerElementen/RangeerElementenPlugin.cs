using System.Xml;
using TLCGen.Helpers;
using TLCGen.ModelManagement;
using TLCGen.Models;
using TLCGen.Plugins.RangeerElementen.Models;

namespace TLCGen.Plugins.RangeerElementen
{
    [TLCGenPlugin(TLCGenPluginElems.XMLNodeWriter)]
    public partial class RangeerElementenPlugin : ITLCGenXMLNodeWriter
    {
        #region Fields

        private RangeerElementenDataModel _rangeerElementenModel;

#pragma warning disable 0649
        // private CCOLGeneratorCodeStringSettingModel _prm...;
#pragma warning restore 0649

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region TLCGen plugin shared

        private ControllerModel _controller;
        public ControllerModel Controller
        {
            get => _controller;
            set
            {
                _controller = value;
                if (_controller == null)
                {
                    _rangeerElementenModel = new RangeerElementenDataModel();
                }
                UpdateModel();
            }
        }

        public string GetPluginName()
        {
            return "RangeerElementen";
        }

        #endregion // TLCGen plugin shared

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _rangeerElementenModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "RangeerTLCGenElementenData")
                {
                    _rangeerElementenModel = XmlNodeConverter.ConvertNode<RangeerElementenDataModel>(node);
                    break;
                }
            }

            if (_rangeerElementenModel == null)
            {
                _rangeerElementenModel = new RangeerElementenDataModel();
            }
        }

        public void SetXmlInDocument(XmlDocument document)
        {
        }

        #endregion // ITLCGenXMLNodeWriter

        #region Private Methods

        internal void UpdateModel()
        {
            if (_controller != null && _rangeerElementenModel != null)
            {
                // detectie
                if (_rangeerElementenModel.RangeerElementenToepassen && !_controller.Data.RangeerData.RangerenDetectoren)
                {
                    _controller.Data.RangeerData.RangerenDetectoren = true;
                    foreach (var d in _rangeerElementenModel.RangeerElementen)
                    {
                        _controller.Data.RangeerData.RangeerDetectoren.Add(new IOElementRangeerDataModel
                        {
                            Naam = d.Element,
                            RangeerIndex = d.RangeerIndex
                        });
                    }

                    _controller.Data.RangeerData.RangerenOvergezet = true;
                }

                // signaalgroepen
                if (_rangeerElementenModel.RangeerSignaalGroepenToepassen)
                {
                    _controller.Data.RangeerData.RangerenFasen = true;
                    foreach (var d in _rangeerElementenModel.RangeerSignaalGroepen)
                    {
                        _controller.Data.RangeerData.RangeerFasen.Add(new IOElementRangeerDataModel
                        {
                            Naam = d.SignaalGroep,
                            RangeerIndex = d.RangeerIndex
                        });
                    }

                    _controller.Data.RangeerData.RangerenOvergezet = true;
                }
            }
        }

        #endregion // Private Methods

        #region Constructor

        public RangeerElementenPlugin()
        {
        }

        #endregion // Constructor
    }
}
