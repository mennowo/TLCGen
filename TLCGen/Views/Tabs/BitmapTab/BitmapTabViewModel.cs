using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FloodFill;
using TLCGen.Helpers;
using System.Windows.Controls;
using TLCGen.Models;
using TLCGen.Messaging.Messages;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Messaging;
using TLCGen.Plugins;
using TLCGen.Models.Enumerations;
using TLCGen.Messaging.Requests;
using System.Reflection;
using System.Collections;
using System.Windows;
using TLCGen.Dialogs;
using Point = System.Drawing.Point;
using TLCGen.Extensions;
using TLCGen.ModelManagement;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 9)]
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
        private int _width = int.MaxValue, _height = int.MaxValue;

        private Color TestFillColor = Color.CornflowerBlue;
        private Color DefaultFillColor = Color.LightGray;
        private Color DefaultFaseColor = Color.DarkRed;
        private Color DefaultFaseSelectedColor = Color.Lime;
        private Color DefaultDetectorColor = Color.Yellow;
        private Color DefaultDetectorSelectedColor = Color.Magenta;

        private readonly Color DefaultUitgangColor = Color.Blue;
        private Color DefaultUitgangSelectedColor = Color.Lime;
        private Color DefaultIngangColor = Color.DarkCyan;
        private Color DefaultIngangSelectedColor = Color.Cyan;

        #endregion // Fields

        #region Properties

        public override System.Windows.Media.ImageSource Icon
        {
            get
            {
                System.Windows.ResourceDictionary dict = new System.Windows.ResourceDictionary();
                Uri u = new Uri("pack://application:,,,/" +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name +
                    ";component/" + "Resources/TabIcons.xaml");
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
                        if (p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                            FillMyBitmap(p, GetFillColor(false));
                    RefreshMyBitmapImage();
                }
                _SelectedItem = value;
                if (_SelectedItem != null && _SelectedItem.HasCoordinates)
                {
                    foreach (Point p in _SelectedItem.Coordinates)
                        if (p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                            FillMyBitmap(p, GetFillColor(true));
                    RefreshMyBitmapImage();
                }
                RaisePropertyChanged("SelectedItem");
            }
        }

        public TabItem SelectedTab
        {
            get { return _SelectedTab; }
            set
            {
                _SelectedTab = value;
                RaisePropertyChanged("SelectedTab");
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
                RaisePropertyChanged("IsEnabled");
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
        public ICommand RefreshBitmapCommand => _RefreshBitmapCommand ?? (_RefreshBitmapCommand =
                                                    new RelayCommand(RefreshBitmapCommand_Executed, RefreshBitmapCommand_CanExecute));

        RelayCommand _ResetBitmapCommand;
        public ICommand ResetBitmapCommand => _ResetBitmapCommand ?? (_ResetBitmapCommand =
                                                  new RelayCommand(ResetBitmapCommand_Executed, ResetBitmapCommand_CanExecute));

        RelayCommand _ImportDplCCommand;
        public ICommand ImportDplCCommand => _ImportDplCCommand ?? (_ImportDplCCommand =
                                                  new RelayCommand(ImportDplCCommand_Executed, ImportDplCCommand_CanExecute));

        
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
            _SelectedItem = null;
            TLCGenModelManager.Default.SetPrioOutputPerSignalGroup(Controller, Controller.PrioData.PrioUitgangPerFase);
            CollectAllIO();
            LoadBitmap();
        }

        public override bool CanBeEnabled()
        {
            SetBitmapFileName();

            return 
                !string.IsNullOrWhiteSpace(BitmapFileName) && 
                File.Exists(BitmapFileName) && 
                _Controller?.Data.NietGebruikenBitmap == false;
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
                        if (pp.X <= _EditableBitmap.Bitmap.Width && pp.Y <= _EditableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(pp, TestFillColor);
                            if (_EditableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y).ToArgb() == TestFillColor.ToArgb())
                            {
                                FillMyBitmap(pp, DefaultFillColor);
                                coords.Add(pp);
                            }
                            else
                            {
                                FillMyBitmap(pp, GetFillColor(true));
                            }
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
            TLCGen.Controls.ZoomViewbox zb = obj as TLCGen.Controls.ZoomViewbox;
            zb?.Reset();
        }

        private bool ResetBitmapCommand_CanExecute(object obj)
        {
            return true;
        }

        void ImportDplCCommand_Executed(object prm)
        {
            var dlg = new ImportDplCWindow(_Controller) {Owner = Application.Current.MainWindow};
            dlg.ShowDialog();
            LoadBitmap();
        }

        bool ImportDplCCommand_CanExecute(object prm)
        {
            return BitmapFileName != null;
        }

        
        #endregion // Command functionality

        #region Public Methods

        #endregion // Public Methods

        #region Private Methods

        private void SetBitmapFileName()
        {
            // Set the bitmap
            if (!string.IsNullOrWhiteSpace(_ControllerFileName) &&
                Controller != null &&
                Controller.Data != null &&
                !string.IsNullOrWhiteSpace(Controller.Data.BitmapNaam))
            {
                BitmapFileName =
                    System.IO.Path.Combine(
                        System.IO.Path.GetDirectoryName(_ControllerFileName),
                        Controller.Data.BitmapNaam.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase) ?
                        Controller.Data.BitmapNaam :
                        Controller.Data.BitmapNaam + ".bmp"
                    );
            }
            else
            {
                BitmapFileName = null;
            }
        }

        private BitmappedItemViewModel GetIOElementFromObject(object obj, PropertyInfo prop = null)
        {
            var objType = obj.GetType();
            IOElementAttribute attr = null;
            if (prop == null)
            {
                attr = objType.GetCustomAttribute<IOElementAttribute>();
            }
            else
            {
                attr = prop.GetCustomAttribute<IOElementAttribute>();
            }

            BitmappedItemViewModel bivm = null;
            if (attr != null)
            {
                bool cond = true;
                if (!string.IsNullOrWhiteSpace(attr.DisplayConditionProperty))
                {
                    cond = (bool)objType.GetProperty(attr.DisplayConditionProperty).GetValue(obj);
                }
                if (cond)
                {
                    string name = attr.DisplayName;
                    if (!string.IsNullOrWhiteSpace(attr.DisplayNameProperty))
                    {
                        name = name + (string)objType.GetProperty(attr.DisplayNameProperty).GetValue(obj);
                    }
                    if (prop == null)
                    {
                        bivm = new BitmappedItemViewModel(obj as IOElementModel, name, attr.Type);
                    }
                    else
                    {
                        bivm = new BitmappedItemViewModel(prop.GetValue(obj) as IOElementModel, name, attr.Type);
                    }
                }
            }
            return bivm;
        }

        private List<BitmappedItemViewModel> GetAllIOElements(object obj)
        {

            var l = new List<BitmappedItemViewModel>();
            if (obj == null) return l;

            var objType = obj.GetType();

            // Object as IOElement
            var bivm = GetIOElementFromObject(obj);
            if(bivm != null)
            {
                l.Add(bivm);
            }

            var properties = objType.GetProperties();
            foreach (var property in properties)
            {
                var ignore = (TLCGenIgnoreAttributeAttribute)property.GetCustomAttribute(typeof(TLCGenIgnoreAttributeAttribute));
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string) || ignore != null) continue;
                var propValue = property.GetValue(obj);
                var elems = propValue as IList;
                if (elems != null)
                {
                    l.AddRange(from object item in elems from i in GetAllIOElements(item) select i);
                }
                else
                {
                    // Property as IOElement
                    bivm = GetIOElementFromObject(obj, property);
                    if (bivm != null)
                    {
                        l.Add(bivm);
                    }
                    l.AddRange(GetAllIOElements(propValue));
                }
            }
            return l;
        }

        private void CollectAllIO()
        {
            Fasen.Clear();
            Detectoren.Clear();
            OverigeUitgangen.Clear();
            OverigeIngangen.Clear();
            
            bool[] done = new bool[20];
            for(int d = 0; d < 20; ++d) done[d] = false;
            foreach(var per in _Controller.PeriodenData.Perioden)
            {
                switch(per.Type)
                {
                    case PeriodeTypeEnum.Groentijden:
                    case PeriodeTypeEnum.StarRegelen:
                    case PeriodeTypeEnum.Overig:
                        per.BitmapDataRelevant = true;
                        per.BitmapNaam = per.Naam;
                        break;
                    default:
                        per.BitmapDataRelevant = false;
                        break;
                }
            }

            foreach(var i in GetAllIOElements(_Controller))
            {
                switch(i.IOType)
                {
                    case BitmappedItemTypeEnum.Fase:
                        Fasen.Add(i);
                        break;
                    case BitmappedItemTypeEnum.Detector:
                        Detectoren.Add(i);
                        break;
                    case BitmappedItemTypeEnum.Uitgang:
                        OverigeUitgangen.Add(i);
                        break;
                    case BitmappedItemTypeEnum.Ingang:
                        OverigeIngangen.Add(i);
                        break;
                }
            }

            // IO from plugins
            foreach (var v in TLCGenPluginManager.Default.ApplicationPlugins)
            {
                if((v.Item1 & TLCGenPluginElems.IOElementProvider) == TLCGenPluginElems.IOElementProvider)
                {
                    var pl = v.Item2 as ITLCGenElementProvider;
                    var initems = pl.GetInputItems();
                    var outitems = pl.GetOutputItems();
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
            foreach (var v in TLCGenPluginManager.Default.ApplicationParts)
            {
                if ((v.Item1 & TLCGenPluginElems.IOElementProvider) == TLCGenPluginElems.IOElementProvider)
                {
                    var pl = v.Item2 as ITLCGenElementProvider;
                    var initems = pl.GetInputItems();
                    var outitems = pl.GetOutputItems();
                    foreach (var i in initems)
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
            if(string.IsNullOrEmpty(_BitmapFileName))
            {
                _EditableBitmap = null;
                _width = _height = int.MaxValue;
                RefreshMyBitmapImage();
                return;
            }

            if (File.Exists(_BitmapFileName))
            {
                try
                {
                    using (Bitmap bitmap = new Bitmap(_BitmapFileName))
                    {
                        _EditableBitmap = new EditableBitmap(bitmap, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        CorrectCoordinates();
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
            if(_EditableBitmap == null)
            {
                _MyBitmap = null;
                RaisePropertyChanged("MyBitmap");
                return;
            }

            using (MemoryStream memory = new MemoryStream())
            {
                _MyBitmap = new BitmapImage();
                _EditableBitmap.Bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                MyBitmap.BeginInit();
                MyBitmap.StreamSource = memory;
                MyBitmap.CacheOption = BitmapCacheOption.OnLoad;
                MyBitmap.EndInit();

                RaisePropertyChanged("MyBitmap");
            }
        }

        private void CorrectCoordinates()
        {
            if (_width > _EditableBitmap.Bitmap.Width || _height > _EditableBitmap.Bitmap.Height)
            {
                _width = _EditableBitmap.Bitmap.Width;
                _height = _EditableBitmap.Bitmap.Height;
                var ioElements = _Fasen.Concat(_Detectoren).Concat(_OverigeUitgangen).Concat(_OverigeIngangen);
                foreach (var ioe in ioElements)
                {
                    var rems = ioe.Coordinates.Where(x => x.X > _EditableBitmap.Bitmap.Width || x.Y > _EditableBitmap.Bitmap.Height).ToList();
                    if (rems.Any())
                    {
                        var all = ioe.Coordinates.Where(x => true).ToList();
                        ioe.Coordinates.RemoveAll();
                        foreach (var a in all.Where(x => rems.All(x2 => x.X != x2.X && x.Y != x2.Y)))
                        {
                            ioe.Coordinates.Add(a); 
                        }
                    }
                }
            }
        }

        private void FillAllIO()
        {
            foreach (BitmappedItemViewModel bivm in Fasen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                    {
                        if(p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, DefaultFaseColor);
                        }
                    }
                }
            }
            foreach (BitmappedItemViewModel bivm in Detectoren)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                    {
                        if (p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, DefaultDetectorColor);
                        }
                    }
                }
            }
            foreach (BitmappedItemViewModel bivm in OverigeUitgangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                    {
                        if (p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, DefaultUitgangColor);
                        }
                    }
                }
            }
            foreach (BitmappedItemViewModel bivm in OverigeIngangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (Point p in bivm.Coordinates)
                    {
                        if (p.X <= _EditableBitmap.Bitmap.Width && p.Y <= _EditableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, DefaultIngangColor);
                        }
                    }
                }
            }
        }

        #endregion // Private Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        private void OnFileNameChanged(ControllerFileNameChangedMessage message)
        {
            if (message.NewFileName == null) return;
            
            _ControllerFileName = message.NewFileName;
            SetBitmapFileName();
            RefreshMyBitmapImage();
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

        private void OnFasenChanged(FasenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }


		private void OnNameChanged(NameChangedMessage message)
		{
			CollectAllIO();
			RefreshMyBitmapImage();
		}

		private void OnDetectorenChanged(DetectorenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }

        private void OnOVIngrepenChanged(PrioIngrepenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public BitmapTabViewModel() : base()
        {
            _FloodFiller = new QueueLinearFloodFiller(null);

            Messenger.Default.Register(this, new Action<ControllerFileNameChangedMessage>(OnFileNameChanged));
            Messenger.Default.Register(this, new Action<RefreshBitmapRequest>(OnRefreshBitmapRequest));
            Messenger.Default.Register(this, new Action<FasenChangedMessage>(OnFasenChanged));
            Messenger.Default.Register(this, new Action<DetectorenChangedMessage>(OnDetectorenChanged));
            Messenger.Default.Register(this, new Action<PrioIngrepenChangedMessage>(OnOVIngrepenChanged));
            Messenger.Default.Register(this, new Action<NameChangedMessage>(OnNameChanged));
        }

        #endregion // Constructor
    }
}