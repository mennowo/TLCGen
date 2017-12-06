using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using TLCGen.Extensions;
using TLCGen.Generators.CCOL.CodeGeneration;
using TLCGen.Generators.CCOL.Settings;
using TLCGen.Helpers;
using TLCGen.Models;
using TLCGen.Plugins;
using TLCGen.SpecialsDenHaag;
using TLCGen.SpecialsDenHaag.Models;
using TLCGen.SpecialsDenHaag.OVMModule;

namespace TLCGen.SpecialsRotterdam
{
    [CCOLCodePieceGenerator]
    [TLCGenTabItem(-1, TabItemTypeEnum.SpecialsTab)]
    [TLCGenPlugin(TLCGenPluginElems.PlugMessaging | 
                  TLCGenPluginElems.TabControl | 
                  TLCGenPluginElems.XMLNodeWriter)]
    public class SpecialsDenHaagPlugin : CCOLCodePieceGeneratorBase, ITLCGenPlugMessaging, ITLCGenTabItem, ITLCGenXMLNodeWriter
    { 
        #region Fields

        private SpecialsDenHaagViewModel _SpecialsDenHaagTabVM;
        private SpecialsDenHaagModel _MyModel;
	    private DVMModuleDenHaag _ovmModule;

        #endregion // Fields

        #region Properties

        #endregion // Properties

        #region ITLCGen plugin shared items

        private ControllerModel _Controller;
        public ControllerModel Controller
        {
            get { return _Controller; }
            set
            {
                _Controller = value;
                if (_Controller == null)
                {
                    _MyModel = new SpecialsDenHaagModel();
	                _ovmModule = new DVMModuleDenHaag();
					_SpecialsDenHaagTabVM.Specials = _MyModel;
                }
                if (_Controller != null && _MyModel != null)
                {
                    foreach (var fc in Controller.Fasen)
                    {
                        if (_SpecialsDenHaagTabVM.AlternatievenPerBlok.All(x => x.FaseCyclus != fc.Naam))
                        {
                            _SpecialsDenHaagTabVM.AlternatievenPerBlok.Add(
                                new FaseCyclusAlternatiefPerBlokViewModel(
                                    new FaseCyclusAlternatiefPerBlokModel { FaseCyclus = fc.Naam, BitWiseBlokAlternatief = 1023 }));
                        }
                    }
                    var rems = new List<FaseCyclusAlternatiefPerBlokViewModel>();
                    foreach (var fc in _SpecialsDenHaagTabVM.AlternatievenPerBlok)
                    {
                        if (Controller.Fasen.All(x => x.Naam != fc.FaseCyclus))
                        {
                            rems.Add(fc);
                        }
                    }
                    foreach (var sg in rems)
                    {
                        _SpecialsDenHaagTabVM.AlternatievenPerBlok.Remove(sg);
                    }
                    _SpecialsDenHaagTabVM.AlternatievenPerBlok.BubbleSort();
                    _SpecialsDenHaagTabVM.RaisePropertyChanged("");
                }
            }
        }

        public string DisplayName
        {
            get
            {
                return "Den Haag";
            }
        }

        public string GetPluginName()
        {
            return "SpecialsDenHaag";
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
            }
        }

        #endregion // ITLCGen plugin shared items

        #region ITLCGenTabItem

        DataTemplate _ContentDataTemplate;
        public DataTemplate ContentDataTemplate
        {
            get
            {
                if (_ContentDataTemplate == null)
                {
                    _ContentDataTemplate = new DataTemplate();
                    var tab = new FrameworkElementFactory(typeof(SpecialsDenHaagView));
                    tab.SetValue(SpecialsDenHaagView.DataContextProperty, _SpecialsDenHaagTabVM);
                    _ContentDataTemplate.VisualTree = tab;
                }
                return _ContentDataTemplate;
            }
        }

        public ImageSource Icon
        {
            get
            {
                //ResourceDictionary dict = new ResourceDictionary();
                //Uri u = new Uri("pack://application:,,,/" +
                //    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                //    ";component/" + "Resources/Icon.xaml");
                //dict.Source = u;
                //return (DrawingImage)dict["AdditorIconDrawingImage"];
                return null;
            }
        }

        public bool CanBeEnabled()
        {
            return true;
        }

        public void OnSelected()
        {

        }

        public bool OnSelectedPreview()
        {
            return true;
        }

        public void OnDeselected()
        {

        }

        public bool OnDeselectedPreview()
        {
            return true;
        }


        public void LoadTabs()
        {

        }

        #endregion // ITLCGenTabItem

        #region ITLCGenPlugMessaging

        public void UpdateTLCGenMessaging()
        {
            _SpecialsDenHaagTabVM?.UpdateTLCGenMessaging();
        }

        #endregion // ITLCGenPlugMessaging

        #region ITLCGenXMLNodeWriter

        public void GetXmlFromDocument(XmlDocument document)
        {
            _MyModel = null;

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.LocalName == "SpecialsDenHaag")
                {
                    _MyModel = XmlNodeConverter.ConvertNode<SpecialsDenHaagModel>(node);
                    break;
                }
            }

            if (_MyModel == null)
            {
                _MyModel = new SpecialsDenHaagModel();
            }
            _SpecialsDenHaagTabVM.Specials = _MyModel;
            _SpecialsDenHaagTabVM.AlternatievenPerBlok.BubbleSort();
            _SpecialsDenHaagTabVM.RaisePropertyChanged("");
	        _ovmModule = new DVMModuleDenHaag();
		}

        public void SetXmlInDocument(XmlDocument document)
        {
            XmlDocument doc = TLCGenSerialization.SerializeToXmlDocument(_MyModel);
            XmlNode node = document.ImportNode(doc.DocumentElement, true);
            document.DocumentElement.AppendChild(node);
        }

        #endregion // ITLCGenXMLNodeWriter

        #region CCOLCodePieceGenerator

        private List<CCOLElement> _MyElements;

        public override void CollectCCOLElements(ControllerModel c)
        {
            _MyElements = new List<CCOLElement>();

            if (_MyModel.ToepassenAlternatievenPerBlok)
            {
                foreach (var fc in Controller.Fasen)
                {
                    var plfc = _MyModel.AlternatievenPerBlok.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (plfc != null)
                    {
                        _MyElements.Add(new CCOLElement($"altb{fc.Naam}", plfc.BitWiseBlokAlternatief, CCOLElementTimeTypeEnum.None, CCOLElementTypeEnum.Parameter));
                    }
                }
            }
	        foreach (var e in _ovmModule.GetCCOLElements(c))
	        {
		        _MyElements.Add(e);
	        }
        }

        public override bool HasCCOLElements()
        {
            return true;
        }

        public override IEnumerable<CCOLElement> GetCCOLElements(CCOLElementTypeEnum type)
        {
            return _MyElements.Where(x => x.Type == type);
        }

        public override int HasCode(CCOLRegCCodeTypeEnum type)
        {
            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Alternatieven:
                    return 101;
                default:
                    return 0;
            }
        }

        public override string GetCode(ControllerModel c, CCOLRegCCodeTypeEnum type, string ts)
        {
            StringBuilder sb = new StringBuilder();

            switch (type)
            {
                case CCOLRegCCodeTypeEnum.Alternatieven:
                    if (!_MyModel.ToepassenAlternatievenPerBlok)
                    {
                        return "";
                    }
                    sb.AppendLine($"{ts}/* BLOKGEBONDEN ALTERNATIEF */");
                    sb.AppendLine($"{ts}/* ======================== */");
                    sb.AppendLine($"{ts}/* Defaultinstelling is prmaltb$$ = 15 (BK1, BK2, BK3, BK4)");
                    sb.AppendLine($"{ts} * Voor instellingen de volgende waarden voor het blok waarin het alternatief mag plaatsvinden optellen:");
                    sb.AppendLine($"{ts} * 1  alternatief mogelijk in blok 1");
                    sb.AppendLine($"{ts} * 2  alternatief mogelijk in blok 2");
                    sb.AppendLine($"{ts} * 4  alternatief mogelijk in blok 3");
                    sb.AppendLine($"{ts} * 8  alternatief mogelijk in blok 4");
                    sb.AppendLine($"{ts} */");
                    foreach (var fc in Controller.Fasen)
                    {
                        sb.AppendLine($"{ts}if(!(PRM[{_prmpf}altb{fc.Naam}] & (1 << ML))) PAR[{_fcpf}{fc.Naam}] = FALSE;");
                    }
                    return sb.ToString();
                default:
                    return null;
            }
        }

        public override List<string> GetSourcesToCopy()
        {
            return new List<string>
            {
                "afmroutines.c",
                "afmroutines.h"
            };
        }

        #endregion // CCOLCodePieceGenerator

        #region Constructor

        public SpecialsDenHaagPlugin() : base()
        {
            _SpecialsDenHaagTabVM = new SpecialsDenHaagViewModel();
        }

        #endregion // Constructor
    }
}
