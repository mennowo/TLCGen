using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloodFill;
using TLCGen.Models;
using System.Drawing;
using System.Collections.ObjectModel;
using TLCGen.Messaging;
using TLCGen.Messaging.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace TLCGen.ViewModels
{
    /// <summary>
    /// Used to disclose relevant data of items that have bitmap coordinates.
    /// </summary>
    public class BitmappedItemViewModel : ViewModelBase
    {
        #region Fields
        
        private IOElementModel _IOElement;
        private ObservableCollection<Point> _Coordinates;
        private string _Naam;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Naam
        {
            get { return _Naam; }
            set
            {
                _Naam = value;
                OnPropertyChanged("Naam");
            }
        }

        /// <summary>
        /// A collection holding the bitmap coordinates for the item
        /// </summary>
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

        /// <summary>
        /// The type of item
        /// </summary>
        public Type IOType { get; set; }

        /// <summary>
        /// Indicates wether or not the item has one or more coordinates set.
        /// This value can be used in the view to display this info to the user.
        /// </summary>
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
#warning TODO
                        //foreach(Point p in Coordinates)
                        //    _ControllerVM.BitmapTabVM.FillDefaultColor(p);
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
            Messenger.Default.Send(new ControllerDataChangedMessage());
            OnPropertyChanged("HasCoordinates");
        }

        #region Constructor

        public BitmappedItemViewModel(IOElementModel ioelem, string naam, Type t)
        {
            _IOElement = ioelem;
            _Naam = naam;
            IOType = t;

            foreach (BitmapCoordinaatModel coord in _IOElement.BitmapCoordinaten)
            {
                Coordinates.Add(new Point(coord.X, coord.Y));
            }

            Coordinates.CollectionChanged += Coordinates_CollectionChanged;
        }

        #endregion
    }
}
