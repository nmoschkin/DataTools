using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage.Streams;
using System.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using System.IO;
using System.Drawing.Imaging;

namespace DataTools.WinUI
{
    public class BitmapTools
    {


        ///// <summary>
        ///// Highlight the specified icon with the specified color.
        ///// </summary>
        ///// <param name="icn">Input icon.</param>
        ///// <param name="liteColor">Highlight base color.</param>
        ///// <returns>A new highlighted Image object.</returns>
        ///// <remarks></remarks>
        //public static Image HiliteIcon(Icon icn, Color liteColor)
        //{
        //    var n = new Bitmap(icn.Width, icn.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        //    var g = System.Drawing.Graphics.FromImage(n);
        //    int lc = liteColor.ToArgb();
        //    g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, n.Width, n.Height));
        //    g.DrawIcon(icn, 0, 0);
        //    g.Dispose();
        //    var bm = new System.Drawing.Imaging.BitmapData();
        //    n.LockBits(new Rectangle(0, 0, n.Width, n.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bm);
        //    int[] b;
        //    int i;
        //    int c;
        //    b = new int[(bm.Width * bm.Height)];

        //    // take the unmanaged memory and make it something manageable and VB-like.

        //    Marshal.Copy(bm.Scan0, b, 0, bm.Stride * bm.Height);
        //    // NativeLib.Native.MemCpy(bm.Scan0, b, bm.Stride * bm.Height)

        //    c = b.Length;
        //    int stp = (int)(bm.Stride / (double)bm.Width);
        //    var hsv = new HSVDATA();
        //    var hsv2 = new HSVDATA();

        //    // convert the color to HSV.
        //    ColorMath.ColorToHSV(liteColor, ref hsv);

        //    for (i = 0; i < c; i++)
        //    {
        //        if (b[i] == 0)
        //            continue;
        //        ColorMath.ColorToHSV(Color.FromArgb(b[i]), ref hsv2);

        //        hsv2.Hue = hsv.Hue;
        //        hsv2.Saturation = hsv.Saturation;
        //        hsv2.Value *= 1.1d;

        //        b[i] = ColorMath.HSVToColor(hsv2);
        //    }

        //    Marshal.Copy(b, 0, bm.Scan0, bm.Stride * bm.Height);
        //    n.UnlockBits(bm);
        //    return n;
        //}

        /// <summary>
        /// Converts a 32 bit icon into a 32 bit Argb transparent bitmap.
        /// </summary>
        /// <param name="icn">Input icon.</param>
        /// <returns>A 32-bit Argb Bitmap object.</returns>
        /// <remarks></remarks>
        public static Bitmap IconToTransparentBitmap(Icon icn)
        {
            if (icn is null)
                return null;
            var n = new Bitmap(icn.Width, icn.Height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(n);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.Clear(Color.Transparent);
            g.DrawIcon(icn, 0, 0);
            g.Dispose();
            return n;
        }

        public static async Task<ImageSource> MakeWinUIImage(Icon img)
        {
            var _d = new IntPtr();
            return await MakeWinUIImage(IconToTransparentBitmap(img));
        }

        /// <summary>
        /// Creates a WinUI BitmapSource from a Bitmap.
        /// </summary>
        /// <param name="img">The <see cref="System.Drawing.Image"/> object to convert.</param>
        /// <param name="bitPtr">Set this to zero.</param>
        /// <param name="dpiX">The X DPI to use to create the new image (default is 96.0)</param>
        /// <param name="dpiY">The Y DPI to use to create the new image (default is 96.0)</param>
        /// <param name="createOnApplicationThread"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static async Task<ImageSource> MakeWinUIImage(Bitmap img)
        {
            if (img is null)
                return null;
            if (img.Width == 0)
                return null;

            try
            {
                using (var mem = new MemoryStream())
                {
                    img.Save(mem, ImageFormat.Bmp);
                    mem.Position = 0;

                    return await ImageLoader.LoadFromBytes(mem.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
