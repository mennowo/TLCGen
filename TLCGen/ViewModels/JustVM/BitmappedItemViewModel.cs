using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloodFill;
using TLCGen.Models;

namespace TLCGen.ViewModels
{
    public class BitmappedItemViewModel : ViewModelBase
    {
        #region Fields

        private ControllerViewModel _ControllerVM;
        private BitmapInfoModel _BitmapInfo;
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

        public int X
        {
            get { return _BitmapInfo.X; }
            set
            {
                _BitmapInfo.X = value;
                OnMonitoredPropertyChanged("X", _ControllerVM);
                OnPropertyChanged("HasCoordinates");
            }
        }

        public int Y
        {
            get { return _BitmapInfo.Y; }
            set
            {
                _BitmapInfo.Y = value;
                OnMonitoredPropertyChanged("Y", _ControllerVM);
                OnPropertyChanged("HasCoordinates");
            }
        }

        public enum Type { Fase, Detector, Uitgang, Ingang };

        public Type IOType { get; set; }

        public bool HasCoordinates
        {
            get
            {
                return _BitmapInfo.X != 0 || _BitmapInfo.Y != 0;
            }
            set
            {
                if(!value)
                {
                    if(!(_BitmapInfo.X == 0 || _BitmapInfo.Y == 0))
                        _ControllerVM.BitmapTabVM.FillDefaultColor(new System.Drawing.Point(_BitmapInfo.X, _BitmapInfo.Y));
                    _BitmapInfo.X = 0;
                    _BitmapInfo.Y = 0;
                    OnPropertyChanged("HasCoordinates");
                }
            }
        }

        #endregion // Properties

        public override string ToString()
        {
            return Naam;
        }

        #region Constructor

        public BitmappedItemViewModel(ControllerViewModel controllervm, BitmapInfoModel bmim, string naam, Type t)
        {
            _ControllerVM = controllervm;
            _BitmapInfo = bmim;
            _Naam = naam;
            IOType = t;
        }

        #endregion
    }
}
