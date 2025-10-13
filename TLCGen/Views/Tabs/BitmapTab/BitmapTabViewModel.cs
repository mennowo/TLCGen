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
using TLCGen.Plugins;
using TLCGen.Models.Enumerations;
using TLCGen.Messaging.Requests;
using System.Reflection;
using System.Collections;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TLCGen.Dialogs;
using Point = System.Drawing.Point;
using TLCGen.Extensions;
using TLCGen.ModelManagement;
using TLCGen.DataAccess;

namespace TLCGen.ViewModels
{
    [TLCGenTabItem(index: 9)]
    public class BitmapTabViewModel : TLCGenTabItemViewModel
    {
        #region Fields

        private ObservableCollection<BitmappedItemViewModel> _fasen;
        private ObservableCollection<BitmappedItemViewModel> _detectoren;
        private ObservableCollection<BitmappedItemViewModel> _overigeUitgangen;
        private ObservableCollection<BitmappedItemViewModel> _overigeIngangen;
        
        private BitmappedItemViewModel _selectedItem;
        private TabItem _selectedTab;
        private EditableBitmap _editableBitmap;
        private readonly QueueLinearFloodFiller _floodFiller;
        private string _controllerFileName;
        private int _width = int.MaxValue, _height = int.MaxValue;
        
        RelayCommand<object> _setCoordinatesCommand;
        RelayCommand _refreshBitmapCommand;
        RelayCommand _resetBitmapIOCommand;
        RelayCommand<object> _resetBitmapCommand;
        RelayCommand _importDplCCommand;

        private readonly Color _defaultFillColor = Color.LightGray;
        private readonly Color _defaultFaseSelectedColor = Color.Lime;
        private readonly Color _defaultUitgangColor = Color.Blue;
        private readonly Color _defaultUitgangSelectedColor = Color.Lime;
        private readonly Color _defaultIngangColor = Color.DarkCyan;
        private readonly Color _defaultIngangSelectedColor = Color.Cyan;
        private readonly Color _testFillColor = Color.CornflowerBlue;
        private readonly Color _defaultFaseColor = Color.DarkRed;
        private readonly Color _defaultDetectorColor = Color.Yellow;
        private readonly Color _defaultDetectorSelectedColor = Color.Magenta;
        private string _bitmapFileName;

        #endregion // Fields

        #region Properties

        public override System.Windows.Media.ImageSource Icon
        {
            get
            {
                var dict = new ResourceDictionary();
                var u = new Uri("pack://application:,,,/" +
                                Assembly.GetExecutingAssembly().GetName().Name +
                                ";component/" + "Resources/TabIcons.xaml");
                dict.Source = u;
                return (System.Windows.Media.ImageSource)dict["BitmapTabDrawingImage"];
            }
        }

        public ObservableCollection<BitmappedItemViewModel> Fasen
        {
            get
            {
                if (_fasen == null)
                {
                    _fasen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _fasen;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> Detectoren
        {
            get
            {
                if (_detectoren == null)
                {
                    _detectoren = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _detectoren;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> OverigeUitgangen
        {
            get
            {
                if (_overigeUitgangen == null)
                {
                    _overigeUitgangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _overigeUitgangen;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> OverigeIngangen
        {
            get
            {
                if (_overigeIngangen == null)
                {
                    _overigeIngangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _overigeIngangen;
            }
        }

        public BitmappedItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != null && _selectedItem.HasCoordinates)
                {
                    foreach(var p in _selectedItem.Coordinates)
                        if (p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                            FillMyBitmap(p, GetFillColor(false));
                    RefreshMyBitmapImage();
                }
                _selectedItem = value;
                if (_selectedItem != null && _selectedItem.HasCoordinates)
                {
                    foreach (var p in _selectedItem.Coordinates)
                        if (p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                            FillMyBitmap(p, GetFillColor(true));
                    RefreshMyBitmapImage();
                }
                OnPropertyChanged();
                _setCoordinatesCommand?.NotifyCanExecuteChanged();
            }
        }

        public TabItem SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage MyBitmap { get; private set; }

        public string BitmapFileName
        {
            get => _bitmapFileName;
            set
            {
                _bitmapFileName = value;
                OnPropertyChanged();
                _refreshBitmapCommand?.NotifyCanExecuteChanged();
                _importDplCCommand?.NotifyCanExecuteChanged();
            }
        }

        public string ControllerFileName
        {
            get => _controllerFileName;
            set
            {
                _controllerFileName = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        #endregion // Properties

        #region Commands

        public ICommand SetCoordinatesCommand => _setCoordinatesCommand ??= new RelayCommand<object>(prm =>
            {
                var p = (Point)prm;

                var c = _editableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y);

                if(c.ToArgb().Equals(GetFillColor(true).ToArgb()))
                {
                    if (SelectedItem.HasCoordinates)
                    {
                        var coords = new List<Point>();
                        foreach (var pp in SelectedItem.Coordinates)
                        {
                            if (pp.X <= _editableBitmap.Bitmap.Width && pp.Y <= _editableBitmap.Bitmap.Height)
                            {
                                FillMyBitmap(pp, _testFillColor);
                                if (_editableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y).ToArgb() == _testFillColor.ToArgb())
                                {
                                    FillMyBitmap(pp, _defaultFillColor);
                                    coords.Add(pp);
                                }
                                else
                                {
                                    FillMyBitmap(pp, GetFillColor(true));
                                }
                            }
                        }
                        foreach(var pp in coords)
                            SelectedItem.Coordinates.Remove(pp);

                        // Broadcast model change
                        WeakReferenceMessengerEx.Default.Send(new BroadcastMessage(null));
                    }
                }
                else if (!c.ToArgb().Equals(Color.Black.ToArgb()) &&
                         !c.ToArgb().Equals(_defaultFaseColor.ToArgb()) &&
                         !c.ToArgb().Equals(_defaultDetectorColor.ToArgb()) &&
                         !c.ToArgb().Equals(_defaultDetectorSelectedColor.ToArgb()))
                {
                    SelectedItem.Coordinates.Add(p);

                    FillMyBitmap(p, GetFillColor(true));

                    // Broadcast model change
                    WeakReferenceMessengerEx.Default.Send(new BroadcastMessage(null));
                }
                RefreshMyBitmapImage();
            }, 
            prm => SelectedItem != null);

        public ICommand RefreshBitmapCommand => _refreshBitmapCommand ??= new RelayCommand(LoadBitmap, () => BitmapFileName != null);
        
        public ICommand ResetBitmapIOCommand => _resetBitmapIOCommand ??= new RelayCommand(() =>
            {
                var ok = MessageBox.Show("Alle aangeklinkt IO wordt gereset. Doorgaan?", "Bevestigen reset", MessageBoxButton.YesNo);

                if (ok != MessageBoxResult.Yes) return;

                foreach (var io in Fasen)
                {
                    io.Coordinates.RemoveAll();
                }
                foreach (var io in Detectoren)
                {
                    io.Coordinates.RemoveAll();
                }
                foreach (var io in OverigeIngangen)
                {
                    io.Coordinates.RemoveAll();
                }
                foreach (var io in OverigeUitgangen)
                {
                    io.Coordinates.RemoveAll();
                }

                // Broadcast model change
                WeakReferenceMessengerEx.Default.Send(new BroadcastMessage(null));

                LoadBitmap();
            });

        public ICommand ResetBitmapCommand => _resetBitmapCommand ??= new RelayCommand<object>(obj =>
            {
                var zb = obj as Controls.ZoomViewbox;
                zb?.Reset();
            });

        public ICommand ImportDplCCommand => _importDplCCommand ??= new RelayCommand(() =>
            {
                var dlg = new ImportDplCWindow(_Controller) {Owner = Application.Current.MainWindow};
                dlg.ShowDialog();
                LoadBitmap();

                // Broadcast model change
                WeakReferenceMessengerEx.Default.Send(new BroadcastMessage(null));
            }, 
            () => BitmapFileName != null);

        
        #endregion // Commands

        #region TabItem Overrides

        public override string DisplayName => "Bitmap";

        public override bool OnSelectedPreview()
        {
            return File.Exists(BitmapFileName);
        }

        public override void OnSelected()
        {
            // Collect all IO to be displayed in the lists
            _selectedItem = null;
            TLCGenModelManager.Default.SetSpecialIOPerSignalGroup(Controller);
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
        
        private void SetCoordinatesCommand_Executed(object prm)
        {
            var p = (Point)prm;

            var c = _editableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y);

            if(c.ToArgb().Equals(GetFillColor(true).ToArgb()))
            {
                if (SelectedItem.HasCoordinates)
                {
                    var coords = new List<Point>();
                    foreach (var pp in SelectedItem.Coordinates)
                    {
                        if (pp.X <= _editableBitmap.Bitmap.Width && pp.Y <= _editableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(pp, _testFillColor);
                            if (_editableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y).ToArgb() == _testFillColor.ToArgb())
                            {
                                FillMyBitmap(pp, _defaultFillColor);
                                coords.Add(pp);
                            }
                            else
                            {
                                FillMyBitmap(pp, GetFillColor(true));
                            }
                        }
                    }
                    foreach(var pp in coords)
                        SelectedItem.Coordinates.Remove(pp);
                }
            }
            else if (!c.ToArgb().Equals(Color.Black.ToArgb()) &&
                !c.ToArgb().Equals(_defaultFaseColor.ToArgb()) &&
                !c.ToArgb().Equals(_defaultDetectorColor.ToArgb()) &&
                !c.ToArgb().Equals(_defaultDetectorSelectedColor.ToArgb()))
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

        void RefreshBitmapCommand_Executed()
        {
            LoadBitmap();
        }

        bool RefreshBitmapCommand_CanExecute()
        {
            return BitmapFileName != null;
        }

        private void ResetBitmapCommand_Executed(object obj)
        {
            var zb = obj as Controls.ZoomViewbox;
            zb?.Reset();
        }

        private bool ResetBitmapCommand_CanExecute(object obj)
        {
            return true;
        }

        private void ResetBitmapIOCommand_Executed()
        {
            var ok = MessageBox.Show("Alle aangeklinkt IO wordt gereset. Doorgaan?", "Bevestigen reset", MessageBoxButton.YesNo);

            if (ok != MessageBoxResult.Yes) return;

            foreach (var io in Fasen)
            {
                io.Coordinates.RemoveAll();
            }
            foreach (var io in Detectoren)
            {
                io.Coordinates.RemoveAll();
            }
            foreach (var io in OverigeIngangen)
            {
                io.Coordinates.RemoveAll();
            }
            foreach (var io in OverigeUitgangen)
            {
                io.Coordinates.RemoveAll();
            }

            LoadBitmap();
        }

        private bool ResetBitmapIOCommand_CanExecute()
        {
            return true;
        }

        void ImportDplCCommand_Executed()
        {
            var dlg = new ImportDplCWindow(_Controller) {Owner = Application.Current.MainWindow};
            dlg.ShowDialog();
            LoadBitmap();
        }

        bool ImportDplCCommand_CanExecute()
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
            if (!string.IsNullOrWhiteSpace(_controllerFileName) && Controller?.Data != null && !string.IsNullOrWhiteSpace(Controller.Data.BitmapNaam))
            {
                BitmapFileName =
                    Path.Combine(
                        Path.GetDirectoryName(_controllerFileName) ?? string.Empty,
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
            var attr = prop == null ? objType.GetCustomAttribute<IOElementAttribute>() : prop.GetCustomAttribute<IOElementAttribute>();

            if (attr == null) return null;

            var cond = true;
            if (!string.IsNullOrWhiteSpace(attr.DisplayConditionProperty))
            {
                if (objType.GetProperty(attr.DisplayConditionProperty)?.GetValue(obj) is bool b)
                {
                    cond = b;
                }
            }

            if (!cond) return null;

            var name = attr.DisplayName;
            if (!string.IsNullOrWhiteSpace(attr.DisplayNameProperty))
            {
                if (objType.GetProperty(attr.DisplayNameProperty)?.GetValue(obj) is string s)
                {
                    name += s;
                }
            }
            
            var bivm = 
                prop == null 
                    ? new BitmappedItemViewModel(obj as IOElementModel, name, attr.Type) 
                    : new BitmappedItemViewModel(prop.GetValue(obj) as IOElementModel, name, attr.Type);
            
            return bivm;
        }

        private void CollectAllIO()
        {
            Fasen.Clear();
            Detectoren.Clear();
            OverigeUitgangen.Clear();
            OverigeIngangen.Clear();
            
            var done = new bool[20];
            for(var d = 0; d < 20; ++d) done[d] = false;
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
            
            foreach(var i in TLCGenControllerDataProvider.Default.CurrentGenerator.GetAllIOElements(_Controller))
            {
                switch(i.ElementType)
                {
                    case IOElementTypeEnum.FaseCyclus:
                        Fasen.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemTypeEnum.Fase));
                        break;
                    case IOElementTypeEnum.Detector:
                    case IOElementTypeEnum.SelectiveDetector:
                        Detectoren.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemTypeEnum.Detector));
                        break;
                    case IOElementTypeEnum.Output:
                        OverigeUitgangen.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemTypeEnum.Uitgang));
                        break;
                    case IOElementTypeEnum.Input:
                        OverigeIngangen.Add(new BitmappedItemViewModel(i, i.Naam, BitmappedItemTypeEnum.Ingang));
                        break;
                }
            }
        }

        private void LoadBitmap()
        {
            if(string.IsNullOrEmpty(BitmapFileName))
            {
                _editableBitmap = null;
                _width = _height = int.MaxValue;
                RefreshMyBitmapImage();
                return;
            }

            if (File.Exists(BitmapFileName))
            {
                try
                {
                    using var bitmap = new Bitmap(BitmapFileName);
                    _editableBitmap = new EditableBitmap(bitmap, PixelFormat.Format24bppRgb);
                    CorrectCoordinates();
                    _floodFiller.Bitmap = _editableBitmap;
                    FillAllIO();
                    RefreshMyBitmapImage();
                }
                catch
                {
                    // ignored
                }
            }
            else
            {
                MessageBox.Show("Bestand " + BitmapFileName + " niet gevonden.", "Bitmap niet gevonden");
            }
        }



        private void FillDefaultColor(Point p)
        {
            _floodFiller.FillColor = _defaultFillColor;
            _floodFiller.FloodFill(p);
            RefreshMyBitmapImage();
        }

        private Color GetFillColor(bool selected)
        {
            var c = new Color();
            switch (SelectedItem.IOType)
            {
                case BitmappedItemTypeEnum.Fase:
                    c = selected ? _defaultFaseSelectedColor : _defaultFaseColor;
                    break;
                case BitmappedItemTypeEnum.Detector:
                    c = selected ? _defaultDetectorSelectedColor : _defaultDetectorColor;
                    break;
                case BitmappedItemTypeEnum.Uitgang:
                    c = selected ? _defaultUitgangSelectedColor : _defaultUitgangColor;
                    break;
                case BitmappedItemTypeEnum.Ingang:
                    c = selected ? _defaultIngangSelectedColor : _defaultIngangColor;
                    break;
            }
            return c;
        }

        private void FillMyBitmap(Point p, Color c)
        {
            _floodFiller.FillColor = c;
            _floodFiller.FloodFill(p);
        }

        private void RefreshMyBitmapImage()
        {
            if(_editableBitmap == null)
            {
                MyBitmap = null;
                OnPropertyChanged(nameof(MyBitmap));
                return;
            }

            using var memory = new MemoryStream();
            MyBitmap = new BitmapImage();
            _editableBitmap.Bitmap.Save(memory, ImageFormat.Bmp);
            memory.Position = 0;

            MyBitmap.BeginInit();
            MyBitmap.StreamSource = memory;
            MyBitmap.CacheOption = BitmapCacheOption.OnLoad;
            MyBitmap.EndInit();

            OnPropertyChanged(nameof(MyBitmap));
        }

        private void CorrectCoordinates()
        {
            if (_width > _editableBitmap.Bitmap.Width || _height > _editableBitmap.Bitmap.Height)
            {
                _width = _editableBitmap.Bitmap.Width;
                _height = _editableBitmap.Bitmap.Height;
                var ioElements = _fasen.Concat(_detectoren).Concat(_overigeUitgangen).Concat(_overigeIngangen);
                foreach (var ioe in ioElements)
                {
                    var rems = ioe.Coordinates.Where(x => x.X > _editableBitmap.Bitmap.Width || x.Y > _editableBitmap.Bitmap.Height).ToList();
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
            foreach (var bivm in Fasen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (var p in bivm.Coordinates)
                    {
                        if(p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, _defaultFaseColor);
                        }
                    }
                }
            }
            foreach (var bivm in Detectoren)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (var p in bivm.Coordinates)
                    {
                        if (p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, _defaultDetectorColor);
                        }
                    }
                }
            }
            foreach (var bivm in OverigeUitgangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (var p in bivm.Coordinates)
                    {
                        if (p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, _defaultUitgangColor);
                        }
                    }
                }
            }
            foreach (var bivm in OverigeIngangen)
            {
                if (bivm.HasCoordinates)
                {
                    foreach (var p in bivm.Coordinates)
                    {
                        if (p.X <= _editableBitmap.Bitmap.Width && p.Y <= _editableBitmap.Bitmap.Height)
                        {
                            FillMyBitmap(p, _defaultIngangColor);
                        }
                    }
                }
            }
        }

        #endregion // Private Methods

        #region Collection Changed

        #endregion // Collection Changed

        #region TLCGen Message Handling

        private void OnFileNameChanged(object sender, ControllerFileNameChangedMessage message)
        {
            if (message.NewFileName == null) return;
            
            _controllerFileName = message.NewFileName;
            SetBitmapFileName();
            RefreshMyBitmapImage();
        }

        private void OnRefreshBitmapRequest(object sender, RefreshBitmapRequest request)
        {
            if (request.Coordinates?.Count > 0)
            {
                foreach (var p in request.Coordinates)
                {
                    FillDefaultColor(p);
                }
            }
            else
            {
                RefreshMyBitmapImage();
            }
        }

        private void OnFasenChanged(object sender, FasenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }


		private void OnNameChanged(object sender, NameChangedMessage message)
		{
			CollectAllIO();
			RefreshMyBitmapImage();
		}

		private void OnDetectorenChanged(object sender, DetectorenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }

        private void OnOVIngrepenChanged(object sender, PrioIngrepenChangedMessage message)
        {
            CollectAllIO();
            RefreshMyBitmapImage();
        }

        #endregion // TLCGen Message Handling

        #region Constructor

        public BitmapTabViewModel() : base()
        {
            _floodFiller = new QueueLinearFloodFiller(null);

            WeakReferenceMessengerEx.Default.Register<ControllerFileNameChangedMessage>(this, OnFileNameChanged);
            WeakReferenceMessengerEx.Default.Register<RefreshBitmapRequest>(this, OnRefreshBitmapRequest);
            WeakReferenceMessengerEx.Default.Register<FasenChangedMessage>(this, OnFasenChanged);
            WeakReferenceMessengerEx.Default.Register<DetectorenChangedMessage>(this, OnDetectorenChanged);
            WeakReferenceMessengerEx.Default.Register<PrioIngrepenChangedMessage>(this, OnOVIngrepenChanged);
            WeakReferenceMessengerEx.Default.Register<NameChangedMessage>(this, OnNameChanged);
        }

        #endregion // Constructor
    }
}