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
using System.Windows.Media.Imaging;
using FloodFill;
using TLCGen.Helpers;
using System.Windows.Controls;

namespace TLCGen.ViewModels
{
    public class BitmapTabViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private ObservableCollection<BitmappedItemViewModel> _Uitgangen;
        private ObservableCollection<BitmappedItemViewModel> _Ingangen;
        private BitmappedItemViewModel _SelectedItem;
        private TabItem _SelectedTab;
        private BitmapImage _MyBitmap;
        private EditableBitmap _EditableBitmap;
        private QueueLinearFloodFiller _FloodFiller;
        private string _BitmapFileName;

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

        public ObservableCollection<BitmappedItemViewModel> Uitgangen
        {
            get
            {
                if (_Uitgangen == null)
                {
                    _Uitgangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _Uitgangen;
            }
        }

        public ObservableCollection<BitmappedItemViewModel> Ingangen
        {
            get
            {
                if (_Ingangen == null)
                {
                    _Ingangen = new ObservableCollection<BitmappedItemViewModel>();
                }
                return _Ingangen;
            }
        }

        public BitmappedItemViewModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != null && _SelectedItem.HasCoordinates)
                {
                    FillMyBitmap(new Point(_SelectedItem.X, _SelectedItem.Y), GetFillColor(false));
                    RefreshMyBitmapImage();
                }
                _SelectedItem = value;
                if (_SelectedItem != null && _SelectedItem.HasCoordinates)
                {
                    FillMyBitmap(new Point(_SelectedItem.X, _SelectedItem.Y), GetFillColor(true));
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
            set
            {
                _BitmapFileName = value;
                LoadBitmap();
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

        #endregion // Commands

        #region Command functionality

        void SetCoordinatesCommand_Executed(object prm)
        {
            Point p = (Point)prm;

            Color c = _EditableBitmap.Bitmap.GetPixel((int)p.X, (int)p.Y);

            if (!c.ToArgb().Equals(Color.Black.ToArgb()) &&
                !c.ToArgb().Equals(DefaultFaseColor.ToArgb()) &&
                !c.ToArgb().Equals(DefaultFaseSelectedColor.ToArgb()) &&
                !c.ToArgb().Equals(DefaultDetectorColor.ToArgb()) &&
                !c.ToArgb().Equals(DefaultDetectorSelectedColor.ToArgb()))
            {
                if(SelectedItem.HasCoordinates)
                {
                    FillMyBitmap(new Point(SelectedItem.X, SelectedItem.Y), DefaultFillColor);
                }

                SelectedItem.X = (int)p.X;
                SelectedItem.Y = (int)p.Y;

                FillMyBitmap(p, GetFillColor(true));

                RefreshMyBitmapImage();
            }
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

        #endregion // Command functionality

        #region Public Methods

        public void FillDefaultColor(Point p)
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

        public void CollectAllIO()
        {
            if (_ControllerVM == null)
                return;

            Uitgangen.Clear();
            Ingangen.Clear();

            foreach (FaseCyclusViewModel fcvm in _ControllerVM.Fasen)
            {
                Uitgangen.Add(new BitmappedItemViewModel(_ControllerVM, fcvm.FaseCyclus.BitmapInfo, fcvm.Naam, BitmappedItemViewModel.Type.Fase));
                foreach (DetectorViewModel dvm in fcvm.Detectoren)
                {
                    Ingangen.Add(new BitmappedItemViewModel(_ControllerVM, dvm.Detector.BitmapInfo, dvm.Naam, BitmappedItemViewModel.Type.Detector));
                }

            }

            foreach (DetectorViewModel dvm in _ControllerVM.Detectoren)
            {
                Ingangen.Add(new BitmappedItemViewModel(_ControllerVM, dvm.Detector.BitmapInfo, dvm.Naam, BitmappedItemViewModel.Type.Detector));
            }
        }

        public void LoadBitmap()
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

        #endregion // Public Methods

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
            foreach (BitmappedItemViewModel bivm in Uitgangen)
            {
                if (bivm.HasCoordinates)
                {
                    FillMyBitmap(new Point(bivm.X, bivm.Y), DefaultFaseColor);
                }
            }
            foreach (BitmappedItemViewModel bivm in Ingangen)
            {
                if (bivm.HasCoordinates)
                {
                    FillMyBitmap(new Point(bivm.X, bivm.Y), DefaultDetectorColor);
                }
            }
        }

        #region Collection Changed

        private void Fasen_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (BitmappedItemViewModel fcvm in e.NewItems)
                {
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (BitmappedItemViewModel fcvm in e.OldItems)
                {
                }
            }
        }

        #endregion // Collection Changed

        #region Constructor

        public BitmapTabViewModel(ControllerViewModel controllervm)
        {
            _ControllerVM = controllervm;
            _FloodFiller = new QueueLinearFloodFiller(null);
        }

        #endregion // Constructor
    }
}