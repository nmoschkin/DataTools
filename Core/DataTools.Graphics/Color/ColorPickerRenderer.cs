using DataTools.MathTools.Polar;
using DataTools.Memory;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        private object lockObj = new object();

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
        public UniRect Bounds { get; private set; }

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
        public SKImage Bitmap { get; private set; }

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
                using (var mm = new CoTaskMemPtr())
                {
                    //var bmpinf = new SKImageInfo((int)Math.Ceiling(Bounds.Width), (int)Math.Ceiling(Bounds.Height), SKColorType.Rgba8888, SKAlphaType.Unknown);

                    var opt = new SKImageInfo()
                    {
                        AlphaType = SKAlphaType.Premul,
                        ColorType = SKColorType.Bgra8888,
                        Height = (int)Math.Ceiling(Bounds.Height),
                        Width = (int)Math.Ceiling(Bounds.Width)
                    };

                    var bmp = new SKBitmap(opt);

                    mm.FromByteArray(ImageBytes);
                    bmp.Pixels = mm.ToArray<SKColor>();
                    bmp.SetImmutable();
                    var img = SKImage.FromBitmap(bmp);

                    Bitmap = img;
                }
            }
        }

        private static bool PointInPolygon(UniPoint[] fillPoints, double x, double y)
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
            double prad;
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

        public ColorPickerRenderer(int width, int height, bool huebox, bool invert, bool vertical, bool tetrachromatic, bool suppressCreateBitmap)
        {
            if (huebox == false) throw new ArgumentException();

            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }

            Bounds = new UniRect(0, 0, width, height);
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

            double cred = invert ? 255f : 0f;
            double cblue = invert ? 0f : 255f;

            double cgreen = invert ? 255f : 0f;
            double corange = invert ? 0f : 255f;

            double wstep = 255f / width;
            double hstep = 255f / height;

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

                    el.PolyPoints = new UniPoint[1] { new UniPoint(x, y) };
                    el.Color = Color.FromArgb(color);
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new UniRect(x, y, 1, 1);
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
                fixed (void* g1 = arrColors)
                {
                    fixed (void* g2 = imageBytes)
                    {
                        Buffer.MemoryCopy(g1, g2, imageBytes.Length, imageBytes.Length);
                    }
                }
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

            Bounds = new UniRect(0, 0, width, height);
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
                            Saturation = huebar ? 1 : invert ? 1 - ((double)j / y2) : ((double)j / y2),
                            Value = value
                        };
                    }

                    ColorMath.HSVToColorRaw(hsv, ref color);

                    var el = new ColorPickerElement();

                    el.PolyPoints = new UniPoint[1] { new UniPoint(i, j) };
                    el.Color = Color.FromArgb(color);
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new UniRect(i, j, 1, 1);
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
                fixed (void* g1 = arrColors)
                {
                    fixed (void* g2 = imageBytes)
                    {
                        Buffer.MemoryCopy(g1, g2, imageBytes.Length, imageBytes.Length);
                    }
                }
            }

            if (!suppressCreateBitmap) ToBitmap();
        }

        ///// <summary>
        ///// Instantiate a hexagonal chunk color picker.
        ///// </summary>
        ///// <param name="pixelRadius">Radius of the circle in pixels.</param>
        ///// <param name="elementSize">Size of each element in partial pixels.</param>
        ///// <param name="value">Brightness value in percentage.</param>
        ///// <param name="invert">True to invert saturation.</param>
        //public ColorPickerRenderer(int pixelRadius, double elementSize, double value = 1d, bool invert = false, double rotation = 0)
        //{
        //    if (Bitmap != null)
        //    {
        //        Bitmap = null;
        //    }

        //    double x1 = elementSize / 2;
        //    double y1 = x1;

        //    double cor = 0f;

        //    double x2 = pixelRadius * 2;
        //    double y2 = x2;

        //    double stepX = elementSize + (elementSize / 2f);
        //    double stepY = (elementSize / 2f);

        //    UniPoint[] masterPoly = new UniPoint[6];

        //    var pc = new PolarCoordinates();
        //    HSVDATA hsv;

        //    int color = 0;
        //    int z;

        //    Value = value;
        //    HueOffset = rotation;

        //    Bounds = new UniRect(0, 0, pixelRadius * 2, pixelRadius * 2);
        //    InvertSaturation = invert;
        //    Mode = ColorPickerMode.HexagonWheel;

        //    Value = value;

        //    //            var bmpinf = new SKImageInfo((int)Math.Ceiling(Bounds.Width), (int)Math.Ceiling(Bounds.Height), SKColorType.Rgba8888, SKAlphaType.Unknown);

        //    var bmp = new SKBitmap((int)Math.Ceiling(Bounds.Width), (int)Math.Ceiling(Bounds.Height), false);
        //    var canvas = new SKCanvas(bmp);

        //    var br = new SKPaint();
        //    br.Color = (SKColor)0xffffffffu;

        //    canvas.DrawPaint(br);

        //    bool alt = false;

        //    pc.Arc = -30;
        //    pc.Radius = pixelRadius;

        //    masterPoly[0] = pc.ToScreenCoordinates(Bounds);

        //    cor = masterPoly[0].Y;

        //    pc.Arc = 60 - 30;
        //    masterPoly[1] = pc.ToScreenCoordinates(Bounds);

        //    pc.Arc = 120 - 30;
        //    masterPoly[2] = pc.ToScreenCoordinates(Bounds);

        //    pc.Arc = 180 - 30;
        //    masterPoly[3] = pc.ToScreenCoordinates(Bounds);

        //    pc.Arc = 240 - 30;
        //    masterPoly[4] = pc.ToScreenCoordinates(Bounds);

        //    pc.Arc = 300 - 30;
        //    masterPoly[5] = pc.ToScreenCoordinates(Bounds);

        //    for (z = 0; z < 6; z++)
        //    {
        //        masterPoly[z].Y -= cor;
        //    }

        //    cor = 0f;

        //    for (double j = y1; j < y2; j += stepY)
        //    {
        //        if (alt)
        //        {
        //            x1 = (elementSize) + (elementSize / 4f);
        //        }
        //        else
        //        {
        //            x1 = (elementSize / 2f);
        //        }

        //        alt = !alt;

        //        for (double i = x1; i < x2; i += stepX)
        //        {
        //            if (!PointInPolygon(masterPoly, i, j)) continue;

        //            pc = PolarCoordinates.ToPolarCoordinates(i - pixelRadius, j - pixelRadius);

        //            if (pc.Radius > pixelRadius)
        //            {
        //                pc.Radius = pixelRadius;
        //            }
        //            if (double.IsNaN(pc.Arc))
        //            {
        //                color = -1;
        //            }
        //            else
        //            {
        //                double arc = pc.Arc - rotation;
        //                if (arc < 0) arc += 360;

        //                hsv = new HSVDATA()
        //                {
        //                    Hue = arc,
        //                    Saturation = invert ? 1 - (pc.Radius / pixelRadius) : (pc.Radius / pixelRadius),
        //                    Value = value
        //                };

        //                ColorMath.HSVToColorRaw(hsv, ref color);
        //            }

        //            var el = new ColorPickerElement();

        //            el.PolyPoints = new UniPoint[6];
        //            el.Center = new UniPoint(i, j);
        //            el.Color = Color.FromArgb(color);
        //            el.Polar = pc;
        //            el.Shape = ColorWheelShapes.Hexagon;
        //            el.Bounds = new UniRect(i - (elementSize / 2f), j - (elementSize / 2f), (double)elementSize, (double)elementSize);
        //            el.Center = el.PolyPoints[0];

        //            pc.Arc = -30;
        //            pc.Radius = (double)elementSize / 2;

        //            el.PolyPoints[0] = pc.ToScreenCoordinates(el.Bounds);

        //            if (cor == 0f)
        //            {
        //                cor = el.PolyPoints[0].Y;
        //                stepY -= cor;
        //            }

        //            pc.Arc = 60 - 30;
        //            el.PolyPoints[1] = pc.ToScreenCoordinates(el.Bounds);

        //            pc.Arc = 120 - 30;
        //            el.PolyPoints[2] = pc.ToScreenCoordinates(el.Bounds);

        //            pc.Arc = 180 - 30;
        //            el.PolyPoints[3] = pc.ToScreenCoordinates(el.Bounds);

        //            pc.Arc = 240 - 30;
        //            el.PolyPoints[4] = pc.ToScreenCoordinates(el.Bounds);

        //            pc.Arc = 300 - 30;
        //            el.PolyPoints[5] = pc.ToScreenCoordinates(el.Bounds);

        //            for (z = 0; z < 6; z++)
        //            {
        //                el.PolyPoints[z].Y -= cor;
        //            }

        //            Elements.Add(el);

        //            br.Color = el.Color;
        //            var pth = new SKPath();

        //            pth.AddPoly(el.PolyPoints.Select(x => new SKPoint((float)x.X, (float)x.Y)).ToArray());
        //            canvas.DrawPath(pth, br);
        //        }
        //    }

        //    Bitmap = bmp;
        //}

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

            Bounds = new UniRect(0, 0, x2, y2);
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

                    el.PolyPoints = new UniPoint[1] { new UniPoint(i, j) };
                    el.Color = Color.FromArgb(color);
                    el.Polar = pc;
                    el.Shape = ColorWheelShapes.Point;
                    el.Bounds = new UniRect(i, j, 1, 1);
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
        public UniColor Color;

        /// <summary>
        /// Location In Polar Coordinates (If Applicable)
        /// </summary>
        public PolarCoordinates Polar;

        /// <summary>
        /// Center Point
        /// </summary>
        public UniPoint Center;

        /// <summary>
        /// Boundary Box
        /// </summary>
        public UniRect Bounds;

        /// <summary>
        /// Polygon Points
        /// </summary>
        public UniPoint[] PolyPoints;

        /// <summary>
        /// Element Shape
        /// </summary>
        public ColorWheelShapes Shape;
    }
}