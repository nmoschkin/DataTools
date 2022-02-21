﻿using DataTools.MathTools.PolarMath;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTools.Graphics;
using System.Runtime.InteropServices;
using DataTools.Standard.Memory;
using DataTools.Graphics.Extensions;

namespace DataTools.Graphics
{

    public enum StartCorner
    {
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast
    }

    /// <summary>
    /// Color picker mode
    /// </summary>
    public enum ColorPickerMode
    {
        
        /// <summary>
        /// Smooth color wheel
        /// </summary>
        Wheel = 0,

        /// <summary>
        /// Smooth horizontal linear gradient
        /// </summary>
        LinearHorizontal = 1,

        /// <summary>
        /// Smooth vertical linear gradient
        /// </summary>
        LinearVertical = 2,

        /// <summary>
        /// Hexagonal chunk gradient
        /// </summary>
        HexagonWheel = 3,

        /// <summary>
        /// A toriodal wheel representing hue in arc coordinates.
        /// </summary>
        HueWheel = 4,

        /// <summary>
        /// A hue-only horizontal color bar (no saturation gradient)
        /// </summary>
        HueBarHorizontal = 5,

        /// <summary>
        /// A hue-only vertical color bar (no saturation gradient)
        /// </summary>
        HueBarVertical = 6,


        HueBoxHorizontal = 7,

        HueBoxVertical = 8
    }

    /// <summary>
    /// Color wheel element shapes
    /// </summary>
    public enum ColorWheelShapes
    {
        /// <summary>
        /// Pixel element
        /// </summary>
        Point = 1,

        /// <summary>
        /// Hexagonal element
        /// </summary>
        Hexagon = 2
    }

    /// <summary>
    /// Class to create a color picker.
    /// </summary>
    public sealed class ColorPickerRenderer
    {
        object lockObj = new object();

        /// <summary>
        /// All individual color elements in the current instance.
        /// </summary>
        public List<ColorPickerElement> Elements { get; private set; } = new List<ColorPickerElement>();

        /// <summary>
        /// The mode of the current instance.
        /// </summary>
        public ColorPickerMode Mode { get; private set; }

        /// <summary>
        /// Gets the 'value' component of the HSV color space.
        /// </summary>
        public double Value { get; private set; }
       
        /// <summary>
        /// Gets a value indicating that a linear color gradient is painted vertically.
        /// </summary>
        public bool Vertical { get; private set; }

        /// <summary>
        /// Gets the 360-degree hue offset.
        /// </summary>
        public double HueOffset { get; private set; }


        /// <summary>
        /// The bounds of the current instance
        /// </summary>
        public RectangleF Bounds { get; private set; }

        /// <summary>
        /// True to invert the location of lowest and highest saturation points.
        /// </summary>
        public bool InvertSaturation { get; private set; }

        private byte[] imageBytes;
        
        /// <summary>
        /// Gets the raw bytes of the picker image.
        /// </summary>
        public byte[] ImageBytes
        {
            get => imageBytes;
            private set
            {
                imageBytes = value;
            }
        }

        /// <summary>
        /// Gets the picker display bitmap.
        /// </summary>
        /// <remarks>
        /// This image should never be resized, stretched or transformed in any way.</remarks>
        public Bitmap Bitmap { get; private set; }


        public async Task Render()
        {
            await Task.Run(() =>
            {

            }); 
        }


        private void ToBitmap()
        {
            if (imageBytes == null) return;

            lock (lockObj)
            {
                var mm = new MemPtr();
                var bmp = new Bitmap(
                    (int)Math.Ceiling(Bounds.Width),
                    (int)Math.Ceiling(Bounds.Height),
                    PixelFormat.Format32bppArgb);

                mm.Alloc(bmp.Width * bmp.Height * 4 * 2);

                var bm = new BitmapData();

                bm.Scan0 = mm.Handle;
                bm.Stride = bmp.Width * 4;

                bm = bmp.LockBits
                    (new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite | ImageLockMode.UserInputBuffer,
                    PixelFormat.Format32bppArgb,
                    bm);

                mm.FromByteArray(ImageBytes);

                bmp.UnlockBits(bm);
                mm.Free();

                Bitmap = bmp;
            }

        }

        static bool PointInPolygon(PointF[] fillPoints, float x, float y)
        {
            // being quite honest here... I didn't even try to walk myself through this
            // I just copied it from the internet, and it works.
            // Reference: http://alienryderflex.com/polygon/

            int i, j = fillPoints.Length - 1;
            bool oddNodes = false;

            for (i = 0; i < fillPoints.Length; i++)
            {
                if (fillPoints[i].Y < y && fillPoints[j].Y >= y
                    || fillPoints[j].Y < y && fillPoints[i].Y >= y)
                {
                    if (fillPoints[i].X + (y - fillPoints[i].Y) / (fillPoints[j].Y - fillPoints[i].Y) * (fillPoints[j].X - fillPoints[i].X) < x)
                    {
                        oddNodes = !oddNodes;
                    }
                }

                j = i;
            }

            return oddNodes;
        }

        public LinearCoordinates GetCoordinates(Color c)
        {
            HSVDATA v = ColorMath.ColorToHSV(c);
            PolarCoordinates p = new PolarCoordinates(InvertSaturation ? 1 - v.Saturation : v.Saturation, v.Hue + HueOffset);
            var pt = PolarCoordinates.ToLinearCoordinates(p);

            pt.X += (Bounds.Width / 2);
            pt.Y += (Bounds.Width / 2);


            return pt;
        }

        /// <summary>
        /// Gets the color at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color HitTest(int x, int y, bool closest, out int fixX, out int fixY)
        {
            HSVDATA hsv;
            float prad;
            LinearCoordinates lin;
            PolarCoordinates pc;

            if (x < 0 || y < 0 || x > Bounds.Width || y > Bounds.Height)
            {
                if (closest)
                {
                    if (x < 0) x = 0;
                    if (y < 0) y = 0;
                    if (x > Bounds.Width) x = (int)Bounds.Width;
                    if (y > Bounds.Height) y = (int)Bounds.Height;
                }
                else
                {

                    fixX = x;
                    fixY = y;

                    return Color.Empty;
                }
            }

            fixX = x;
            fixY = y;

            switch (Mode)
            {
                case ColorPickerMode.Wheel:

                    prad = (Bounds.Width / 2);
                    pc = PolarCoordinates.ToPolarCoordinates(x - prad, y - prad);
                    if (pc.Radius > prad)
                    {
                        if (closest)
                        {
                            pc.Radius = prad;

                            lin = PolarCoordinates.ToLinearCoordinates(pc, Bounds);
                            fixX = (int)lin.X;
                            fixY = (int)lin.Y;
                        }
                        else
                        {
                            return Color.Empty;
                        }
                    }

                    hsv.Hue = pc.Arc + HueOffset;
                    hsv.Saturation = InvertSaturation ? 1 - (pc.Radius / prad) : (pc.Radius / prad);
                    hsv.Value = Value;

                    return ColorMath.HSVToColor(hsv);

                case ColorPickerMode.HueWheel:

                    prad = (Bounds.Width / 2);
                    pc = PolarCoordinates.ToPolarCoordinates(x - prad, y - prad);

                    if (pc.Radius > prad)
                    {
                        if (closest)
                        {
                            pc.Radius = prad;

                            lin = PolarCoordinates.ToLinearCoordinates(pc, Bounds);
                            fixX = (int)lin.X;
                            fixY = (int)lin.Y;
                        }
                        else
                        {
                            return Color.Empty;
                        }
                    }

                    hsv.Hue = pc.Arc + HueOffset;
                    hsv.Saturation = 1;
                    hsv.Value = Value;

                    return ColorMath.HSVToColor(hsv);
            }


            foreach (ColorPickerElement e in Elements)
            {                
                if (e.PolyPoints.Length == 1)
                {
                    var f = e.PolyPoints[0];                     
                    if (f.X == x & f.Y == y)
                        return e.Color;
                }
                else
                {
                    if (PointInPolygon(e.PolyPoints, x, y))
                    {
                        return e.Color;
                    }
                }
            }

            return Color.Empty;
        }

        /// <summary>
        /// Gets the color at the specified location.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color HitTest(int x, int y)
        {
            return HitTest(x, y, false, out _, out _);
        }

        /// <summary>
        /// Gets the color at the specified location.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public Color HitTest(Point pt)
        {
            return HitTest(pt.X, pt.Y, false, out _, out _);
        }


        public ColorPickerRenderer(int width, int height, bool huebox, bool invert, bool vertical, bool tetrachromatic = false, bool suppressCreateBitmap = false)
        {

            if (huebox == false) throw new ArgumentException();

            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            Bounds = new Rectangle(0, 0, width, height);
            InvertSaturation = invert;


            Value = 1;
            HueOffset = 0;
            Vertical = vertical;

            if (vertical)
            {
                Mode = ColorPickerMode.HueBoxVertical;
            }
            else
            {
                Mode = ColorPickerMode.HueBoxHorizontal;
            }

            List<int> rawColors = new List<int>();

            int x1 = 0;
            int x2 = width;

            int y1 = 0;
            int y2 = height;



            float cred = invert ? 255f : 0f;
            float cblue = invert ? 0f : 255f;

            float cgreen = invert ? 255f : 0f;
            float corange = invert ? 0f : 255f;

            float wstep = 255f / width;
            float hstep = 255f / height;

            byte r, g, b, o;
            int color = 0;


            for (int y = y1; y < y2; y++)
            {
                for (int x = x1; x < x2; x++)
                {
                    r = (byte)(int)Math.Round(cred);
                    g = (byte)(int)Math.Round(cgreen);
                    b = (byte)(int)Math.Round(cblue);
                    o = (byte)(int)Math.Round(corange);

                    if (tetrachromatic && y % 2 == 0 && x % 2 == 0)
                    {
                        var fred = ((cred * 1) - (corange / 2));
                        if (fred < 0) fred = 0;

                        var fgreen = ((cgreen * 1) + (corange / 2));
                        if (fgreen > 255) fgreen = 255;

                        r = (byte)(int)Math.Round(fred);
                        g = (byte)(int)Math.Round(fgreen);

                    }

                    color = unchecked((int)0xFF000000) | (int)r | ((int)g << 8) | ((int)b << 16);


                    var el = new ColorPickerElement();

                    el.PolyPoints = new PointF[1] { new PointF(x, y) };
                    el.Color = Color.FromArgb(color);
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new Rectangle(x, y, 1, 1);
                    el.Center = el.PolyPoints[0];

                    Elements.Add(el);
                    rawColors.Add(color);

                    if (vertical)
                    {
                        if (invert)
                        {
                            cgreen -= wstep;
                            corange += wstep;
                        }
                        else
                        {
                            cgreen += wstep;
                            corange -= wstep;
                        }
                    }
                    else
                    {
                        if (invert)
                        {
                            cred -= wstep;
                            cblue += wstep;
                        }
                        else
                        {
                            cred += wstep;
                            cblue -= wstep;
                        }
                    }

                }

                if (vertical)
                {
                    if (invert)
                    {
                        cred -= hstep;
                        cblue += hstep;
                    }
                    else
                    {
                        cred += hstep;
                        cblue -= hstep;
                    }

                    cgreen = invert ? 255 : 0;
                    corange = invert ? 0 : 255;
                }
                else
                {
                    if (invert)
                    {
                        cgreen -= hstep;
                        corange += hstep;
                    }
                    else
                    {
                        cgreen += hstep;
                        corange -= hstep;
                    }

                    cred = invert ? 255 : 0;
                    cblue = invert ? 0 : 255;
                }

            }



            var arrColors = rawColors.ToArray();
            imageBytes = new byte[arrColors.Length * sizeof(int)];

            Elements.Sort((ael, bel) =>
            {
                if (ael.Center.X == bel.Center.X)
                {
                    return (int)(ael.Center.Y - bel.Center.Y);
                }
                else
                {
                    return (int)(ael.Center.X - bel.Center.X);
                }
            });

            unsafe
            {
                var gch1 = GCHandle.Alloc(arrColors, GCHandleType.Pinned);
                var gch2 = GCHandle.Alloc(imageBytes, GCHandleType.Pinned);

                Buffer.MemoryCopy((void*)gch1.AddrOfPinnedObject(), (void*)gch2.AddrOfPinnedObject(), imageBytes.Length, imageBytes.Length);

                gch1.Free();
                gch2.Free();

            }

            if (!suppressCreateBitmap) ToBitmap();

        }


        /// <summary>
        /// Instantiate a linear color picker.
        /// </summary>
        /// <param name="width">Width in pixels.</param>
        /// <param name="height">Height in pixels.</param>
        /// <param name="value">Brightness value in percentage.</param>
        /// <param name="offset">Hue offset in degrees.</param>
        /// <param name="invert">True to invert saturation.</param>
        /// <param name="vertical">True to draw vertically.</param>
        public ColorPickerRenderer(int width, int height, double value = 1d, double offset = 0d, bool invert = false, bool vertical = false, bool suppressCreateBitmap = false, bool huebar = false)
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            Bounds = new Rectangle(0, 0, width, height);
            InvertSaturation = invert;

            Value = value;
            HueOffset = offset;
            Vertical = vertical;

            List<int> rawColors = new List<int>();

            int x1 = 0;
            int x2 = width;

            int y1 = 0;
            int y2 = height;

            HSVDATA hsv;

            int color = 0;
            
            if (vertical)
            {
                Mode = ColorPickerMode.LinearVertical;
            }
            else
            {
                Mode = ColorPickerMode.LinearHorizontal;
            }

            for (int j = y1; j < y2; j++)
            {
                for (int i = x1; i < x2; i++)
                {
                    double arc;

                    if (vertical)
                    {
                        arc = ((double)j / y2) * 360;
                        arc -= offset;
                        if (arc < 0) arc += 360;

                        hsv = new HSVDATA()
                        {
                            Hue = arc,
                            Saturation = huebar ? 1 : invert ? 1 - ((double)i / x2) : ((double)i / x2),
                            Value = value
                        };


                    }
                    else
                    {
                        arc = ((double)i / x2) * 360;
                        arc -= offset;
                        if (arc < 0) arc += 360;

                        hsv = new HSVDATA()
                        {
                            Hue = arc,
                            Saturation = huebar ? 1 : invert ? 1 - ((double)j/y2) : ((double)j/y2),
                            Value = value
                        };
                    }

                    ColorMath.HSVToColorRaw(hsv, ref color);

                    var el = new ColorPickerElement();

                    el.PolyPoints = new PointF[1] { new PointF(i, j) };
                    el.Color = Color.FromArgb(color);
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new Rectangle(i, j, 1, 1);
                    el.Center = el.PolyPoints[0];

                    Elements.Add(el);
                    rawColors.Add(color);
                }
            }

            var arrColors = rawColors.ToArray();
            imageBytes = new byte[arrColors.Length * sizeof(int)];

            Elements.Sort((a, b) =>
            {
                if (a.Center.X == b.Center.X)
                {
                    return (int)(a.Center.Y - b.Center.Y);
                }
                else
                {
                    return (int)(a.Center.X - b.Center.X);
                }
            });

            unsafe
            {
                var gch1 = GCHandle.Alloc(arrColors, GCHandleType.Pinned);
                var gch2 = GCHandle.Alloc(imageBytes, GCHandleType.Pinned);

                Buffer.MemoryCopy((void*)gch1.AddrOfPinnedObject(), (void*)gch2.AddrOfPinnedObject(), imageBytes.Length, imageBytes.Length);

                gch1.Free();
                gch2.Free();

            }

            if (!suppressCreateBitmap) ToBitmap();
        }

        /// <summary>
        /// Instantiate a hexagonal chunk color picker.
        /// </summary>
        /// <param name="pixelRadius">Radius of the circle in pixels.</param>
        /// <param name="elementSize">Size of each element in partial pixels.</param>
        /// <param name="value">Brightness value in percentage.</param>
        /// <param name="invert">True to invert saturation.</param>
        public ColorPickerRenderer(int pixelRadius, float elementSize, double value = 1d, bool invert = false, double rotation = 0, bool suppressCreateBitmap = false)
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            float x1 = elementSize / 2;
            float y1 = x1;

            float cor = 0f;

            float x2 = pixelRadius * 2;
            float y2 = x2;

            float stepX = elementSize + (elementSize / 2f);
            float stepY = (elementSize / 2f);

            PointF[] masterPoly = new PointF[6];

            var pc = new PolarCoordinates();
            HSVDATA hsv;

            int color = 0;
            int z;

            Value = value;
            HueOffset = rotation;

            Bounds = new RectangleF(0, 0, pixelRadius * 2, pixelRadius * 2);
            InvertSaturation = invert;
            Mode = ColorPickerMode.HexagonWheel;

            Value = value;

            var bmp = new Bitmap((int)Math.Ceiling(Bounds.Width), (int)Math.Ceiling(Bounds.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            SolidBrush br;

            br = new SolidBrush(Color.FromArgb(unchecked((int)0x00ffffff)));
            g.FillRectangle(br, Bounds);
            br.Dispose();

            bool alt = false;


            pc.Arc = -30;
            pc.Radius = pixelRadius;

            masterPoly[0] = pc.ToScreenCoordinates(Bounds);

            cor = masterPoly[0].Y;

            pc.Arc = 60 - 30;
            masterPoly[1] = pc.ToScreenCoordinates(Bounds);

            pc.Arc = 120 - 30;
            masterPoly[2] = pc.ToScreenCoordinates(Bounds);

            pc.Arc = 180 - 30;
            masterPoly[3] = pc.ToScreenCoordinates(Bounds);

            pc.Arc = 240 - 30;
            masterPoly[4] = pc.ToScreenCoordinates(Bounds);

            pc.Arc = 300 - 30;
            masterPoly[5] = pc.ToScreenCoordinates(Bounds);

            for (z = 0; z < 6; z++)
            {
                masterPoly[z].Y -= cor;
            }

            cor = 0f;

            for (float j = y1; j < y2; j += stepY)
            {
                if (alt)
                {
                    x1 = (elementSize) + (elementSize / 4f);
                }
                else
                {
                    x1 = (elementSize / 2f);
                }

                alt = !alt;

                for (float i = x1; i < x2; i += stepX)
                {

                    if (!PointInPolygon(masterPoly, i, j)) continue;

                    pc = PolarCoordinates.ToPolarCoordinates(i - pixelRadius, j - pixelRadius);

                    if (pc.Radius > pixelRadius)
                    {
                        pc.Radius = pixelRadius;
                    }
                    if (double.IsNaN(pc.Arc))
                    {
                        color = -1;
                    }
                    else
                    {
                        double arc = pc.Arc - rotation;
                        if (arc < 0) arc += 360;

                        hsv = new HSVDATA()
                        {
                            Hue = arc,
                            Saturation = invert ? 1 - (pc.Radius / pixelRadius) : (pc.Radius / pixelRadius),
                            Value = value
                        };

                        ColorMath.HSVToColorRaw(hsv, ref color);
                    }

                    var el = new ColorPickerElement();

                    el.PolyPoints = new PointF[6];
                    el.Center = new PointF(i, j);
                    el.Color = Color.FromArgb(color);
                    el.Polar = pc;
                    el.Shape = ColorWheelShapes.Hexagon;
                    el.Bounds = new RectangleF(i - (elementSize / 2f), j - (elementSize / 2f), (float)elementSize, (float)elementSize);
                    el.Center = el.PolyPoints[0];

                    pc.Arc = -30;
                    pc.Radius = (double)elementSize / 2;

                    el.PolyPoints[0] = pc.ToScreenCoordinates(el.Bounds);

                    if (cor == 0f)
                    {
                        cor = el.PolyPoints[0].Y;
                        stepY -= cor;
                    }

                    pc.Arc = 60 - 30;
                    el.PolyPoints[1] = pc.ToScreenCoordinates(el.Bounds);

                    pc.Arc = 120 - 30;
                    el.PolyPoints[2] = pc.ToScreenCoordinates(el.Bounds);

                    pc.Arc = 180 - 30;
                    el.PolyPoints[3] = pc.ToScreenCoordinates(el.Bounds);

                    pc.Arc = 240 - 30;
                    el.PolyPoints[4] = pc.ToScreenCoordinates(el.Bounds);

                    pc.Arc = 300 - 30;
                    el.PolyPoints[5] = pc.ToScreenCoordinates(el.Bounds);

                    for (z = 0; z < 6; z++)
                    {
                        el.PolyPoints[z].Y -= cor;
                    }

                    Elements.Add(el);

                    br = new SolidBrush(el.Color);
                    g.FillPolygon(br, el.PolyPoints);
                    br.Dispose();
                }

            }

            g.Dispose();
            Bitmap = bmp;

        }

        /// <summary>
        /// Instantiate a smooth color wheel.
        /// </summary>
        /// <param name="pixelRadius">Radius of the circle in pixels.</param>
        /// <param name="value">Brightness value in percentage.</param>
        /// <param name="rotation">Hue offset in degrees.</param>
        /// <param name="invert">True to invert saturation.</param>
        public ColorPickerRenderer(int pixelRadius, double value = 1d, double rotation = 0d, bool invert = false, bool suppressCreateBitmap = false, int huewheelThickness = 0)
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            List<int> rawColors = new List<int>();

            int x1 = 0;
            int x2 = pixelRadius * 2;

            int y1 = 0;
            int y2 = pixelRadius * 2;
            int hwt = 0;

            double sx, sy;

            PolarCoordinates pc;
            HSVDATA hsv;

            int color = 0;

            Value = value;
            HueOffset = rotation;

            Bounds = new Rectangle(0, 0, x2, y2);
            InvertSaturation = invert;

            if (huewheelThickness > 0)
            {
                hwt = huewheelThickness;
                Mode = ColorPickerMode.HueWheel;
            }
            else
            {
                Mode = ColorPickerMode.Wheel;
            }

            for (int j = y1; j < y2; j++)
            {
                for (int i = x1; i < x2; i++)
                {
                    sx = i - pixelRadius;
                    sy = j - pixelRadius;

                    pc = PolarCoordinates.ToPolarCoordinates(sx, sy);

                    if (pc.Radius > pixelRadius)
                    {
                        rawColors.Add(0);
                        continue;
                    }
                    else if (hwt != 0 && pc.Radius < (pixelRadius - hwt))
                    {
                        rawColors.Add(0);
                        continue;
                    }
                    if (double.IsNaN(pc.Arc))
                    {
                        color = -1;
                    }
                    else
                    {

                        double arc = pc.Arc - rotation;
                        if (arc < 0) arc += 360;

                        hsv = new HSVDATA()
                        {
                            Hue = arc,
                            Saturation = hwt > 0 ? 1 : invert ? 1 - (pc.Radius / pixelRadius) : (pc.Radius / pixelRadius),
                            Value = value
                        };

                        ColorMath.HSVToColorRaw(hsv, ref color);
                    }

                    var el = new ColorPickerElement();

                    el.PolyPoints = new PointF[1] { new PointF(i, j) };
                    el.Color = Color.FromArgb(color);
                    el.Polar = pc;
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new RectangleF(i, j, 1, 1);
                    el.Center = el.PolyPoints[0];
                    
                    Elements.Add(el);
                    rawColors.Add(color);
                }

            }

            var arrColors = rawColors.ToArray();
            imageBytes = new byte[arrColors.Length * sizeof(int)];

            unsafe
            {
                var gch1 = GCHandle.Alloc(arrColors, GCHandleType.Pinned);
                var gch2 = GCHandle.Alloc(imageBytes, GCHandleType.Pinned);

                Buffer.MemoryCopy((void*)gch1.AddrOfPinnedObject(), (void*)gch2.AddrOfPinnedObject(), imageBytes.Length, imageBytes.Length);

                gch1.Free();
                gch2.Free();

            }

            if (!suppressCreateBitmap) ToBitmap();
        }
    }

    /// <summary>
    /// Structure that represents an element on the color picker.
    /// </summary>
    public struct ColorPickerElement
    {
        /// <summary>
        /// Element Color
        /// </summary>
        public Color Color;

        /// <summary>
        /// Location In Polar Coordinates (If Applicable)
        /// </summary>
        public PolarCoordinates Polar;

        /// <summary>
        /// Center Point
        /// </summary>
        public PointF Center;

        /// <summary>
        /// Boundary Box
        /// </summary>
        public RectangleF Bounds;

        /// <summary>
        /// Polygon Points
        /// </summary>
        public PointF[] PolyPoints;

        /// <summary>
        /// Element Shape
        /// </summary>
        public ColorWheelShapes Shape;
    }

}
