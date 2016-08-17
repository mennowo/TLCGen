using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloodFill;
using TLCGen.Models;
using System.Drawing;
using System.Collections.ObjectModel;

namespace TLCGen.ViewModels
{
    public class BitmappedItemViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private IOElementModel _IOElement;
        private ObservableCollection<Point> _Coordinates;
        private string _Naam;

        #endregion // Fields

        #region Properties

        public string Naam
        {
            get { return _Naam; }
            set
            {
                _Naam = value;
                OnPropertyChanged("Naam");
            }
        }

        public ObservableCollection<Point> Coordinates
        {
            get
            {
                if (_Coordinates == null)
                    _Coordinates = new ObservableCollection<Point>();
                return _Coordinates;
            }
        }

        public enum Type { Fase, Detector, Uitgang, Ingang };

        public Type IOType { get; set; }

        public bool HasCoordinates
        {
            get
            {
                return Coordinates?.Count > 0;
            }
            set
            {
                if(!value)
                {
                    if(Coordinates?.Count > 0)
                    {
                        foreach(Point p in Coordinates)
                            _ControllerVM.BitmapTabVM.FillDefaultColor(p);
                    }
                    Coordinates.Clear();
                    OnPropertyChanged("HasCoordinates");
                }
            }
        }

        #endregion // Properties

        public override string ToString()
        {
            return Naam;
        }

        private void Coordinates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (Point p in e.NewItems)
                {
                    _IOElement.BitmapCoordinaten.Add(new BitmapCoordinaatModel() { X = p.X, Y = p.Y });
                }
            }
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                List<BitmapCoordinaatModel> coords = new List<BitmapCoordinaatModel>();
                foreach (Point p in e.OldItems)
                {
                    foreach(BitmapCoordinaatModel bmcm in _IOElement.BitmapCoordinaten)
                    {
                        if (p.X == bmcm.X && p.Y == bmcm.Y)
                            coords.Add(bmcm);
                    }
                }
                foreach (BitmapCoordinaatModel bmcm in coords)
                    _IOElement.BitmapCoordinaten.Remove(bmcm);
            }
            _ControllerVM.HasChanged = true;
            OnPropertyChanged("HasCoordinates");
        }

        #region Constructor

        public BitmappedItemViewModel(ControllerViewModel controllervm, IOElementModel ioelem, string naam, Type t)
        {
            _ControllerVM = controllervm;
            _IOElement = ioelem;
            _Naam = naam;
            IOType = t;

            foreach(BitmapCoordinaatModel coord in _IOElement.BitmapCoordinaten)
            {
                Coordinates.Add(new Point(coord.X, coord.Y));
            }

            Coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        #endregion
    }
}
