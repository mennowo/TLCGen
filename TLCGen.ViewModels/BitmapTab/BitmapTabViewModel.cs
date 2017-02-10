using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FloodFill;
using TLCGen.Helpers;
using System.Windows.Controls;
using TLCGen.Models;
using TLCGen.DataAccess;
using TLCGen.Messaging.Messages;
using System.Windows.Media.Imaging;
using TLCGen.Messaging;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Plugins;
using TLCGen.Models.Enumerations;
using TLCGen.Messaging.Requests;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 7)]
    public class BitmapTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<BitmappedItemViewModel> _Fasen;
        private ObservableCollection<BitmappedItemViewModel> _Detectoren;
        private ObservableCollection<BitmappedItemViewModel> _OverigeUitgangen;
        private ObservableCollection<BitmappedItemViewModel> _OverigeIngangen;
        private BitmappedItemViewModel _SelectedItem;
        private TabItem _SelectedTab;
        private BitmapImage _MyBitmap;
        private EditableBitmap _EditableBitmap;
        private QueueLinearFloodFiller _FloodFiller;
        private string _ControllerFileName;
        private string _BitmapFileName;

        private Color TestFillColor = Color.CornflowerBlue;
        private Color DefaultFillColor = Color.LightGray;
        private Color DefaultFaseColor = Color.DarkRed;
        private Color DefaultFaseSelectedColor = Color.Lime;
        private Color DefaultDetectorColor = Color.Yellow;
        private Color DefaultDetectorSelectedColor = Color.Magenta;

        private Color DefaultUitgangColor = Color.Blue;
        private Color DefaultUitgangSelectedColor = Color.Lime;
        private Color DefaultIngangColor = Color.DarkCyan;
        private Color DefaultIngangSelectedColor = Color.Cyan;

        #endregion // Fields

        #region Properties

        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                System.Windows.ResourceDictionary dict = new System.Windows.ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "TabIcons.xaml");
                dict.Source = u;
                return (System.Windows.Media.ImageSource)dict["BitmapTabDrawingImage"];
            }
        }

        public ObservableCollection<BitmappedItemViewModel> Fasen
        {
            get
            {
                if (_Fasen == null)
                {
                    _Fasen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _Fasen;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> Detectoren
        {
            get
            {
                if (_Detectoren == null)
                {
                    _Detectoren = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _Detectoren;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> OverigeUitgangen
        {
            get
            {
                if (_OverigeUitgangen == null)
                {
                    _OverigeUitgangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _OverigeUitgangen;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> OverigeIngangen
        {
            get
            {
                if (_OverigeIngangen == null)
                {
                    _OverigeIngangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _OverigeIngangen;
            }
        }

        public BitmappedItemViewModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != null && _SelectedItem.HasCoordinates)
                {
                    foreach(Point p in _SelectedItem.Coordinates)
                        FillMyBitmap(p, GetFillColor(false));
                    RefreshMyBitmapImage();
                }
                _SelectedItem = value;
                if (_SelectedItem != null && _SelectedItem.HasCoordinates)
                {
                    foreach (Point p in _SelectedItem.Coordinates)
                        FillMyBitmap(p, GetFillColor(true));
                    RefreshMyBitmapImage();
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                _SelectedTab = value;
                OnPropertyChanged("SelectedTab");
            }
        }

        public BitmapImage MyBitmap
        {
            get { return _MyBitmap; }
        }

        public string BitmapFileName
        {
            get { return _BitmapFileName; }
            set { _BitmapFileName = value; }
        }

        public string ControllerFileName
        {
            get { return _ControllerFileName; }
            set
            {
                _ControllerFileName = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        #endregion // Properties

        #region Commands

        RelayCommand _SetCoordinatesCommand;
        public ICommand SetCoordinatesCommand
        {
            get
            {
                if (_SetCoordinatesCommand == null)
                {
                    _SetCoordinatesCommand = new RelayCommand(SetCoordinatesCommand_Executed, SetCoordinatesCommand_CanExecute);
                }
                return _SetCoordinatesCommand;
            }
        }

        RelayCommand _RefreshBitmapCommand;
        public ICommand RefreshBitmapCommand
        {
            get
            {
                if (_RefreshBitmapCommand == null)
                {
                    _RefreshBitmapCommand = new RelayCommand(RefreshBitmapCommand_Executed, RefreshBitmapCommand_CanExecute);
                }
                return _RefreshBitmapCommand;
            }
        }

        RelayCommand _ResetBitmapCommand;
        public ICommand ResetBitmapCommand
        {
            get
            {
                if (_ResetBitmapCommand == null)
                {
                    _ResetBitmapCommand = new RelayCommand(ResetBitmapCommand_Executed, ResetBitmapCommand_CanExecute);
                }
                return _ResetBitmapCommand;
            }
        }

        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName
        {
            get
            {
                return "Bitmap";
            }
        }

        public override bool OnSelectedPreview()
        {
            if (File.Exists(BitmapFileName))
                return true;
            else
                return false;
        }

        public override void OnSelected()
        {
            // Collect all IO to be displayed in the lists
            CollectAllIO();
            LoadBitmap();
        }

        public override bool CanBeEnabled()
        {
            // Set the bitmap
            if (!string.IsNullOrWhiteSpace(_ControllerFileName) && 
                Controller.Data != null && 
                !string.IsNullOrWhiteSpace(Controller.Data.BitmapNaam))
                BitmapFileName =
                    System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(_ControllerFileName),
                        Controller.Data.BitmapNaam.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) ?
                        Controller.Data.BitmapNaam :
                        Controller.Data.BitmapNaam + ".bmp"
                    );

            return !string.IsNullOrWhiteSpace(BitmapFileName) && File.Exists(BitmapFileName);
        }

        #endregion // TabItem Overrides

        #region Command functionality

        void SetCoordinatesCommand_Executed(object prm)
        {
            Point p = (Point)prm;

            Color c = _EditableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y);

            if(c.ToArgb().Equals(GetFillColor(true).ToArgb()))
            {
                if (SelectedItem.HasCoordinates)
                {
                    List<Point> coords = new List<Point>();
                    foreach (Point pp in SelectedItem.Coordinates)
                    {
                        FillMyBitmap(pp, TestFillColor);
                        if(_EditableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y).ToArgb() == TestFillColor.ToArgb())
                        {
                            FillMyBitmap(pp, DefaultFillColor);
                            coords.Add(pp);
                        }
                        else
                        {
                            FillMyBitmap(pp, GetFillColor(true));
                        }
                    }
                    foreach(Point pp in coords)
                        SelectedItem.Coordinates.Remove(pp);
                }
            }
            else if (!c.ToArgb().Equals(Color.Black.ToArgb()) &&
                !c.ToArgb().Equals(DefaultFaseColor.ToArgb()) &&
                !c.ToArgb().Equals(DefaultDetectorColor.ToArgb()) &&
                !c.ToArgb().Equals(DefaultDetectorSelectedColor.ToArgb()))
            {
                SelectedItem.Coordinates.Add(p);

                FillMyBitmap(p, GetFillColor(true));

            }
            RefreshMyBitmapImage();
        }

        bool SetCoordinatesCommand_CanExecute(object prm)
        {
            return SelectedItem != null;
        }

        void RefreshBitmapCommand_Executed(object prm)
        {
            LoadBitmap();
        }

        bool RefreshBitmapCommand_CanExecute(object prm)
        {
            return BitmapFileName != null;
        }

        private void ResetBitmapCommand_Executed(object obj)
        {
            TLCGen.Controls.ZoomBorder zb = obj as TLCGen.Controls.ZoomBorder;
            zb?.Reset();
        }

        private bool ResetBitmapCommand_CanExecute(object obj)
        {
            return true;
        }

        #endregion // Command functionality

        #region Public Methods

        #endregion // Public Methods

        #region Private Methods

        private void CollectAllIO()
        {
            Fasen.Clear();
            Detectoren.Clear();
            OverigeUitgangen.Clear();
            OverigeIngangen.Clear();

#warning Need to change this to reflect settings prefixes etc

            // SignalGroups + Detectors + Waitsignalen + Rateltikkers
            foreach (var fcm in _Controller.Fasen)
            {
                Fasen.Add(new BitmappedItemViewModel(fcm as IOElementModel, fcm.Naam, BitmappedItemTypeEnum.Fase));
                foreach (var dm in fcm.Detectoren)
                {
                    Detectoren.Add(new BitmappedItemViewModel(dm as IOElementModel, dm.Naam, BitmappedItemTypeEnum.Detector));
                    if (dm.Wachtlicht)
                    {
                        OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)dm.WachtlichtBitmapData, "wl" + dm.Naam, BitmappedItemTypeEnum.Uitgang));
                    }
                }

            }

            foreach (var dm in _Controller.Detectoren)
            {
                Detectoren.Add(new BitmappedItemViewModel(dm as IOElementModel, dm.Naam, BitmappedItemTypeEnum.Detector));
                if (dm.Wachtlicht)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)dm.WachtlichtBitmapData, "wl" + dm.Naam, BitmappedItemTypeEnum.Uitgang));
                }
            }

            // Warn signals
            foreach(var gr in _Controller.Signalen.WaarschuwingsGroepen)
            {
                if(gr.Bellen)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(gr.BellenBitmapData, "wschl" + gr.Naam, BitmappedItemTypeEnum.Uitgang));
                }
                if(gr.Lichten)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(gr.LichtenBitmapData, "bel" + gr.Naam, BitmappedItemTypeEnum.Uitgang));
                }
            }
            foreach (var rt in _Controller.Signalen.Rateltikkers)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)rt.BitmapData, "rt" + rt.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
            }

            // Klokperioden
            OverigeUitgangen.Add(new BitmappedItemViewModel(_Controller.PeriodenData.DefaultPeriodeBitmapData, "perdef", BitmappedItemTypeEnum.Uitgang));
            foreach(var per in _Controller.PeriodenData.Perioden)
            {
                if(per.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden || per.Type == Models.Enumerations.PeriodeTypeEnum.Overig)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(per.BitmapData, "per" + per.Naam, BitmappedItemTypeEnum.Uitgang));
                }
            }
            if (_Controller.PeriodenData.Perioden.Count > 0)
            {
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAltijd).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.RateltikkersAltijd, "perrt"));
                }
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersAanvraag).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.RateltikkersAanvraag, "perrta"));
                }
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.RateltikkersDimmen).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.RateltikkersDimmen, "perrtdim"));
                }
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenActief).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.BellenActief, "perbel"));
                }
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.BellenDimmen).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.BellenDimmen, "perbeldim"));
                }
                if (_Controller.PeriodenData.Perioden.Where(x => x.Type == PeriodeTypeEnum.WaarschuwingsLichten).Any())
                {
                    OverigeUitgangen.Add(GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum.WaarschuwingsLichten, "pertwl"));
                }
            }

            // RoBuGrover
            if (_Controller.RoBuGrover.ConflictGroepen?.Count > 0)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(_Controller.RoBuGrover.BitmapData, "rgv", BitmappedItemTypeEnum.Uitgang));
            }

            // PTP
            if(_Controller.PTPData?.PTPKoppelingen?.Count > 0)
            {
                foreach(var k in _Controller.PTPData.PTPKoppelingen)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(k.OkBitmapData, k.TeKoppelenKruispunt + "ptpok", BitmappedItemTypeEnum.Uitgang));
                    OverigeUitgangen.Add(new BitmappedItemViewModel(k.ErrorBitmapData, k.TeKoppelenKruispunt + "ptperror", BitmappedItemTypeEnum.Uitgang));
                }
            }
            // File ingrepen
            if (_Controller.FileIngrepen?.Count > 0)
            {
                foreach (var f in _Controller.FileIngrepen)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(f.BitmapData, f.Naam + "file", BitmappedItemTypeEnum.Uitgang));
                }
            }

            // OV / HD
            foreach (var ov in _Controller.OVData.OVIngrepen)
            {
                if (ov.KAR)
                {
                    OverigeIngangen.Add(new BitmappedItemViewModel(ov.OVKARDummyInmeldingBitmapData, "dummykarin" + ov.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                    OverigeIngangen.Add(new BitmappedItemViewModel(ov.OVKARDummyUitmeldingBitmapData, "dummykaruit" + ov.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                }
                if(ov.Vecom)
                {
                    OverigeIngangen.Add(new BitmappedItemViewModel(ov.OVVecomDummyInmeldingBitmapData, "dummyvecomin" + ov.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                    OverigeIngangen.Add(new BitmappedItemViewModel(ov.OVVecomDummyUitmeldingBitmapData, "dummyvecomuit" + ov.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                }
                OverigeUitgangen.Add(new BitmappedItemViewModel(ov.OVInmeldingBitmapData, "vc" + ov.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
            }
            foreach (var hd in _Controller.OVData.HDIngrepen)
            {
                if (hd.KAR)
                {
                    OverigeIngangen.Add(new BitmappedItemViewModel(hd.HDKARDummyInmeldingBitmapData, "dummykarhdin" + hd.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                    OverigeIngangen.Add(new BitmappedItemViewModel(hd.HDKARDummyUitmeldingBitmapData, "dummykarhduit" + hd.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                }
                if (hd.Vecom)
                {
                    OverigeIngangen.Add(new BitmappedItemViewModel(hd.HDVecomDummyInmeldingBitmapData, "dummyvecomhdin" + hd.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                    OverigeIngangen.Add(new BitmappedItemViewModel(hd.HDVecomDummyUitmeldingBitmapData, "dummyvecomhduit" + hd.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
                }
                OverigeUitgangen.Add(new BitmappedItemViewModel(hd.HDInmeldingBitmapData, "vchd" + hd.FaseCyclus, BitmappedItemTypeEnum.Uitgang));
            }

            // Segment display
            for(int i = 0; i < 7; ++i)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(
                    _Controller.Data.SegmentenDisplayBitmapData[i], 
                    _Controller.Data.SegmentenDisplayBitmapData[i].Naam, 
                    BitmappedItemTypeEnum.Uitgang));
            }

            // IO from plugins
            foreach (var v in TLCGenPluginManager.Default.LoadedPlugins)
            {
                var pl = v as ITLCGenElementProvider;
                if(pl != null)
                {
                    var initems = ((ITLCGenElementProvider)v).GetInputItems();
                    var outitems = ((ITLCGenElementProvider)v).GetOutputItems();
                    foreach(var i in initems)
                    {
                        OverigeIngangen.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemTypeEnum.Ingang));
                    }
                    foreach (var o in outitems)
                    {
                        OverigeUitgangen.Add(new BitmappedItemViewModel(o, o.Naam, BitmappedItemTypeEnum.Uitgang));
                    }
                }
            }
        }

        private BitmappedItemViewModel GetBitmappedItemViewModelForPeriodType(PeriodeTypeEnum type, string itemname)
        {
            List<PeriodeModel> pers = new List<PeriodeModel>();
            if(_Controller.PeriodenData.Perioden.Count > 0)
            {
                foreach(var p in _Controller.PeriodenData.Perioden)
                {
                    if(p.Type == type)
                    {
                        return new BitmappedItemViewModel(p.BitmapData, itemname, BitmappedItemTypeEnum.Uitgang);
                    }
                }
            }
            return null;
        }

        private void LoadBitmap()
        {
            if (File.Exists(_BitmapFileName))
            {
                try
                {
                    using (Bitmap bitmap = new Bitmap(_BitmapFileName))
                    {
                        _EditableBitmap = new EditableBitmap(bitmap, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        _FloodFiller.Bitmap = _EditableBitmap;
                        FillAllIO();
                        RefreshMyBitmapImage();
                    }
                }
                catch
                {

                }
            }
            else
            {
                System.Windows.MessageBox.Show("Bestand " + _BitmapFileName + " niet gevonden.", "Bitmap niet gevonden");
            }
        }



        private void FillDefaultColor(Point p)
        {
            _FloodFiller.FillColor = DefaultFillColor;
            _FloodFiller.FloodFill(p);
            RefreshMyBitmapImage();
        }

        private Color GetFillColor(bool selected)
        {
            Color c = new Color();
            switch (SelectedItem.IOType)
            {
                case BitmappedItemTypeEnum.Fase:
                    c = selected ? DefaultFaseSelectedColor : DefaultFaseColor;
                    break;
                case BitmappedItemTypeEnum.Detector:
                    c = selected ? DefaultDetectorSelectedColor : DefaultDetectorColor;
                    break;
                case BitmappedItemTypeEnum.Uitgang:
                    c = selected ? DefaultUitgangSelectedColor : DefaultUitgangColor;
                    break;
                case BitmappedItemTypeEnum.Ingang:
                    c = selected ? DefaultIngangSelectedColor : DefaultIngangColor;
                    break;
            }
            return c;
        }

        private void FillMyBitmap(Point p, Color c)
        {
            _FloodFiller.FillColor = c;
            _FloodFiller.FloodFill(p);
        }

        private void RefreshMyBitmapImage()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                _MyBitmap = new BitmapImage();
                _EditableBitmap.Bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                MyBitmap.BeginInit();
                MyBitmap.StreamSource = memory;
                MyBitmap.CacheOption = BitmapCacheOption.OnLoad;
                MyBitmap.EndInit();

                OnPropertyChanged("MyBitmap");
            }
        }

        private void FillAllIO()
        {
            foreach (BitmappedItemViewModel bivm in Fasen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                        FillMyBitmap(p, DefaultFaseColor);
                }
            }
            foreach (BitmappedItemViewModel bivm in Detectoren)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                        FillMyBitmap(p, DefaultDetectorColor);
                }
            }
            foreach (BitmappedItemViewModel bivm in OverigeUitgangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                        FillMyBitmap(p, DefaultUitgangColor);
                }
            }
            foreach (BitmappedItemViewModel bivm in OverigeIngangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                        FillMyBitmap(p, DefaultIngangColor);
                }
            }
        }

        #endregion // Private Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        private void OnFileNameChanged(ControllerFileNameChangedMessage message)
        {
            _ControllerFileName = message.NewFileName;
        }

        private void OnRefreshBitmapRequest(RefreshBitmapRequest request)
        {
            if (request.Coordinates?.Count > 0)
            {
                foreach (Point p in request.Coordinates)
                {
                    FillDefaultColor(p);
                }
            }
            else
            {
                RefreshMyBitmapImage();
            }
        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public BitmapTabViewModel() : base()
        {
            _FloodFiller = new QueueLinearFloodFiller(null);

            Messenger.Default.Register(this, new Action<ControllerFileNameChangedMessage>(OnFileNameChanged));
            Messenger.Default.Register(this, new Action<RefreshBitmapRequest>(OnRefreshBitmapRequest));
        }

        #endregion // Constructor
    }
}