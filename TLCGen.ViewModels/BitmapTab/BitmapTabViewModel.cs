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
        private Color DefaultFaseSelectedColor = Color.Red;
        private Color DefaultDetectorColor = Color.DarkCyan;
        private Color DefaultDetectorSelectedColor = Color.Cyan;

        private Color DefaultUitgangColor = Color.DarkRed;
        private Color DefaultUitgangSelectedColor = Color.Red;
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
                Fasen.Add(new BitmappedItemViewModel(fcm as IOElementModel, fcm.Naam, BitmappedItemViewModel.Type.Fase));
                if(fcm.RatelTikkerType != Models.Enumerations.RateltikkerTypeEnum.Geen)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)fcm.RatelTikkerBitmapData, "rt" + fcm.Naam, BitmappedItemViewModel.Type.Uitgang));
                }
                foreach (var dm in fcm.Detectoren)
                {
                    Detectoren.Add(new BitmappedItemViewModel(dm as IOElementModel, dm.Naam, BitmappedItemViewModel.Type.Detector));
                    if (dm.Wachtlicht)
                    {
                        OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)dm.WachtlichtBitmapData, "wl" + dm.Naam, BitmappedItemViewModel.Type.Uitgang));
                    }
                }

            }

            foreach (var dm in _Controller.Detectoren)
            {
                Detectoren.Add(new BitmappedItemViewModel(dm as IOElementModel, dm.Naam, BitmappedItemViewModel.Type.Detector));
                if (dm.Wachtlicht)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel((IOElementModel)dm.WachtlichtBitmapData, "wl" + dm.Naam, BitmappedItemViewModel.Type.Uitgang));
                }
            }

            foreach(var gr in _Controller.WaarschuwingsGroepen)
            {
                if(gr.Bellen)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(gr.BellenBitmapData, "wschlgr" + gr.Naam, BitmappedItemViewModel.Type.Uitgang));
                }
                if(gr.Lichten)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(gr.LichtenBitmapData, "belgr" + gr.Naam, BitmappedItemViewModel.Type.Uitgang));
                }
            }

            // Dimmen rateltikkers
            if(_Controller.Fasen.Where(x => x.RatelTikkerType != Models.Enumerations.RateltikkerTypeEnum.Geen).Any())
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(_Controller.RateltikkersDimmenCoordinaten, "rtdim", BitmappedItemViewModel.Type.Uitgang));
            }

            // Dimmen bellen
            if (_Controller.WaarschuwingsGroepen.Where(x => x.Bellen == true).Any())
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(_Controller.BellenDimmenCoordinaten, "beldim", BitmappedItemViewModel.Type.Uitgang));
            }

            // Klokperioden
            OverigeUitgangen.Add(new BitmappedItemViewModel(_Controller.PeriodenData.DefaultPeriodeBitmapData, "perdef", BitmappedItemViewModel.Type.Uitgang));
            foreach(var per in _Controller.PeriodenData.Perioden)
            {
                if(per.Type == Models.Enumerations.PeriodeTypeEnum.Groentijden || per.Type == Models.Enumerations.PeriodeTypeEnum.Overig)
                {
                    OverigeUitgangen.Add(new BitmappedItemViewModel(per.BitmapData, "per" + per.Naam, BitmappedItemViewModel.Type.Uitgang));
                }
            }

            // OV / HD
            foreach(var ov in _Controller.OVData.OVIngrepen)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(ov.OVInmeldingBitmapData, "vc" + ov.FaseCyclus, BitmappedItemViewModel.Type.Uitgang));
            }
            foreach (var hd in _Controller.OVData.HDIngrepen)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(hd.HDInmeldingBitmapData, "vchd" + hd.FaseCyclus, BitmappedItemViewModel.Type.Uitgang));
            }

            // Segment display
            for(int i = 0; i < 7; ++i)
            {
                OverigeUitgangen.Add(new BitmappedItemViewModel(
                    _Controller.Data.SegmentenDisplayBitmapData[i], 
                    _Controller.Data.SegmentenDisplayBitmapData[i].Naam, 
                    BitmappedItemViewModel.Type.Uitgang));
            }

            // IO from plugins
            foreach (var v in TLCGenPluginManager.Default.LoadedPlugins)
            {
                var pl = v as ITLCGenIOElementProvider;
                if(v != null)
                {
                    var initems = ((ITLCGenIOElementProvider)v).GetInputItems();
                    var outitems = ((ITLCGenIOElementProvider)v).GetOutputItems();
                    foreach(var i in initems)
                    {
                        OverigeIngangen.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemViewModel.Type.Ingang));
                    }
                    foreach (var o in outitems)
                    {
                        OverigeUitgangen.Add(new BitmappedItemViewModel(o, o.Naam, BitmappedItemViewModel.Type.Uitgang));
                    }
                }
            }
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
                case BitmappedItemViewModel.Type.Fase:
                    c = selected ? DefaultFaseSelectedColor : DefaultFaseColor;
                    break;
                case BitmappedItemViewModel.Type.Detector:
                    c = selected ? DefaultDetectorSelectedColor : DefaultDetectorColor;
                    break;
                case BitmappedItemViewModel.Type.Uitgang:
                    c = selected ? DefaultUitgangSelectedColor : DefaultUitgangColor;
                    break;
                case BitmappedItemViewModel.Type.Ingang:
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

        #endregion // TLCGen Message Handling

        #region Constructor

        public BitmapTabViewModel(ControllerModel controller) : base(controller)
        {
            _FloodFiller = new QueueLinearFloodFiller(null);

            Messenger.Default.Register(this, new Action<ControllerFileNameChangedMessage>(OnFileNameChanged));
        }

        #endregion // Constructor
    }
}