using DataTools.Win32.Memory;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using static DataTools.Desktop.BitmapTools;

namespace DataTools.Desktop
{
    public static class WPFBitmapTools

    {
        private static Dictionary<IntPtr, BitmapSource> cachedImages = new Dictionary<IntPtr, BitmapSource>();

        public static void ClearCache()
        {
            cachedImages.Clear();
        }

        public static BitmapSource LookupIcon(Icon icon)
        {
            if (cachedImages.TryGetValue(icon.Handle, out BitmapSource result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static void AddIcon(IntPtr idx, BitmapSource bmp)
        {
            if (!cachedImages.ContainsKey(idx))
            {
                cachedImages.Add(idx, bmp);
            }
        }

        public static BitmapSource MakeWPFImage(Icon img)
        {
            if (cachedImages.ContainsKey(img.Handle))
            {
                return cachedImages[img.Handle];
            }

            var _d = new IntPtr();
            var result = MakeWPFImage(img, ref _d);

            if (result != null)
            {
                cachedImages.Add(img.Handle, result);
            }

            return result;
        }

        public static BitmapSource MakeWPFImage(Bitmap img)
        {
            var _d = new IntPtr();
            return MakeWPFImage(img, ref _d);
        }

        /// <summary>
        /// Creates a WPF BitmapSource from a Bitmap.
        /// </summary>
        /// <param name="img">The <see cref="System.Drawing.Icon"/> object to convert.</param>
        /// <param name="bitPtr">Set this to zero.</param>
        /// <param name="dpiX">The X DPI to use to create the new image (default is 96.0)</param>
        /// <param name="dpiY">The Y DPI to use to create the new image (default is 96.0)</param>
        /// <param name="createOnApplicationThread"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BitmapSource MakeWPFImage(System.Drawing.Icon img, ref IntPtr bitPtr, double dpiX = 96.0d, double dpiY = 96.0d, bool createOnApplicationThread = true)
        {
            return MakeWPFImage(IconToTransparentBitmap(img), ref bitPtr, dpiX, dpiY, createOnApplicationThread);
        }

        /// <summary>
        /// Creates a WPF BitmapSource from a Bitmap.
        /// </summary>
        /// <param name="img">The <see cref="System.Drawing.Image"/> object to convert.</param>
        /// <param name="bitPtr">Set this to zero.</param>
        /// <param name="dpiX">The X DPI to use to create the new image (default is 96.0)</param>
        /// <param name="dpiY">The Y DPI to use to create the new image (default is 96.0)</param>
        /// <param name="createOnApplicationThread"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BitmapSource MakeWPFImage(Bitmap img, ref IntPtr bitPtr, double dpiX = 96.0d, double dpiY = 96.0d, bool createOnApplicationThread = true)
        {
            if (img is null)
                throw new ArgumentNullException(nameof(img));

            if (img.Width == 0)
                throw new ArgumentOutOfRangeException(nameof(img.Width));

            if (img.Height == 0)
                throw new ArgumentOutOfRangeException(nameof(img.Height));

            int BytesPerRow = (int)((double)(img.Width * 32 + 31 & ~31) / 8d);

            int size = img.Height * BytesPerRow;
            var bm = new System.Drawing.Imaging.BitmapData();

            BitmapSource bmp = null;

            try
            {
                bm = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, bm);
                if (createOnApplicationThread && System.Windows.Application.Current is object)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => bmp = System.Windows.Media.Imaging.BitmapSource.Create(img.Width, img.Height, dpiX, dpiY, System.Windows.Media.PixelFormats.Bgra32, null, bm.Scan0, size, BytesPerRow));
                }
                else
                {
                    bmp = BitmapSource.Create(img.Width, img.Height, dpiX, dpiY, System.Windows.Media.PixelFormats.Bgra32, null, bm.Scan0, size, BytesPerRow);
                }
            }
            catch
            {
                if (bm is object)
                    img.UnlockBits(bm);

                return null;
            }

            img.UnlockBits(bm);
            return bmp;
        }

        public static BitmapSource MakeWPFImage(SKImage img)
        {
            IntPtr _d = default;
            return MakeWPFImage(img, ref _d);
        }

        /// <summary>
        /// Creates a WPF BitmapSource from a Bitmap.
        /// </summary>
        /// <param name="img">The <see cref="System.Drawing.Image"/> object to convert.</param>
        /// <param name="bitPtr">Set this to zero.</param>
        /// <param name="dpiX">The X DPI to use to create the new image (default is 96.0)</param>
        /// <param name="dpiY">The Y DPI to use to create the new image (default is 96.0)</param>
        /// <param name="createOnApplicationThread"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static BitmapSource MakeWPFImage(SKImage img, ref IntPtr bitPtr, double dpiX = 96.0d, double dpiY = 96.0d, bool createOnApplicationThread = true)
        {
            if (createOnApplicationThread)
            {
                BitmapSource result = null;

                var b = new IntPtr();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    result = MakeWPFImage(img, ref b, dpiX, dpiY, false);
                });

                bitPtr = b;
                return result;
            }

            if (img is null)
                throw new ArgumentNullException(nameof(img));

            if (img.Width == 0)
                throw new ArgumentOutOfRangeException(nameof(img.Width));

            if (img.Height == 0)
                throw new ArgumentOutOfRangeException(nameof(img.Height));

            SKData encoded = img.Encode(SKEncodedImageFormat.Png, 100);
            Stream stream = encoded.AsStream();

            var bitmap = new BitmapImage();

            bitmap.BeginInit();

            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
            //var ret = Image.FromStream(stream);

            //return MakeWPFImage((Bitmap)ret);
        }

        /// <summary>
        /// Creates a System.Drawing.Bitmap image from a WPF source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Bitmap MakeBitmapFromWPF(System.Windows.Media.Imaging.BitmapSource source)
        {
            var mm = new MemPtr();
            Bitmap bmp = null;
            if (System.Windows.Application.Current is object)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    mm.Alloc(bmp.Width * bmp.Height * 4);

                    var bm = new System.Drawing.Imaging.BitmapData();

                    bm.Scan0 = mm.Handle;
                    bm.Stride = bmp.Width * 4;

                    bm = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite | System.Drawing.Imaging.ImageLockMode.UserInputBuffer, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bm);

                    source.CopyPixels(System.Windows.Int32Rect.Empty, mm.Handle, (int)mm.Length, bmp.Width * 4);

                    bmp.UnlockBits(bm);
                    mm.Free();
                });
            }
            else
            {
                bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                mm.Alloc(bmp.Width * bmp.Height * 4);

                var bm = new System.Drawing.Imaging.BitmapData();

                bm.Scan0 = mm.Handle;
                bm.Stride = bmp.Width * 4;

                bm = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite | System.Drawing.Imaging.ImageLockMode.UserInputBuffer, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bm);

                source.CopyPixels(System.Windows.Int32Rect.Empty, mm.Handle, (int)mm.Length, bmp.Width * 4);

                bmp.UnlockBits(bm);
                mm.Free();
            }

            return bmp;
        }
    }
}