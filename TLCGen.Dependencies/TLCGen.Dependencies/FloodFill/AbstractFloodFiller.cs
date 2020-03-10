using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace FloodFill
{
    // Found here:
    // http://www.codeproject.com/Articles/16405/Queue-Linear-Flood-Fill-A-Fast-Flood-Fill-Algorith

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void UpdateScreenDelegate(ref int x, ref int y);

    /// <summary>
    /// The base class that the flood fill algorithms inherit from. Implements the
    /// basic flood filler functionality that is the same across all algorithms.
    /// </summary>
    public abstract class AbstractFloodFiller
    {

        protected EditableBitmap bitmap;
        protected byte[] tolerance = new byte[] { 25, 25, 25 };
        protected Color fillColor = Color.Magenta;
        protected bool fillDiagonally = false;
        protected bool slow = false;

        //cached bitmap properties
        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;

        //internal int timeBenchmark = 0;
        internal Stopwatch watch = new Stopwatch();
        internal UpdateScreenDelegate UpdateScreen = null;

        //internal, initialized per fill
        //protected BitArray pixelsChecked;
        protected bool[] pixelsChecked;
        protected byte[] byteFillColor;
        protected byte[] startColor;
        //protected int stride;

        public AbstractFloodFiller()
        {

        }

        public AbstractFloodFiller(AbstractFloodFiller configSource)
        {
            if (configSource != null)
            {
                this.Bitmap = configSource.Bitmap;
                this.FillColor = configSource.FillColor;
                this.FillDiagonally = configSource.FillDiagonally;
                this.Slow = configSource.Slow;
                this.Tolerance = configSource.Tolerance;
            }
        }

        public bool Slow
        {
            get => slow;
            set => slow = value;
        }

        public Color FillColor
        {
            get => fillColor;
            set => fillColor = value;
        }

        public bool FillDiagonally
        {
            get => fillDiagonally;
            set => fillDiagonally = value;
        }

        public byte[] Tolerance
        {
            get => tolerance;
            set => tolerance = value;
        }

        public EditableBitmap Bitmap
        {
            get => bitmap;
            set => bitmap = value;
        }

        public abstract void FloodFill(Point pt);

        protected void PrepareForFloodFill(Point pt)
        {   
            //cache data in member variables to decrease overhead of property calls
            //this is especially important with Width and Height, as they call
            //GdipGetImageWidth() and GdipGetImageHeight() respectively in gdiplus.dll - 
            //which means major overhead.
            byteFillColor = new byte[] { fillColor.B, fillColor.G, fillColor.R };
            bitmapStride=bitmap.Stride;
            bitmapPixelFormatSize=bitmap.PixelFormatSize;
            bitmapBits = bitmap.Bits;
            bitmapWidth = bitmap.Bitmap.Width;
            bitmapHeight = bitmap.Bitmap.Height;

            pixelsChecked = new bool[bitmapBits.Length / bitmapPixelFormatSize];
        }
    }
}
