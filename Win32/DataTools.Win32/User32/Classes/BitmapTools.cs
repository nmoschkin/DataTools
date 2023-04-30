using DataTools.Win32.Memory;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    /// <summary>
    /// Various bitmap tools and constants
    /// </summary>
    public static class BitmapTools
    {
        /// <summary>
        /// No compression (Bitmap/RGB values)
        /// </summary>
        public const long BI_RGB = 0L;

        /// <summary>
        /// RLE-8 encoding
        /// </summary>
        public const long BI_RLE8 = 1L;

        /// <summary>
        /// RLE-4 encoding
        /// </summary>
        public const long BI_RLE4 = 2L;

        /// <summary>
        /// Bit fields
        /// </summary>
        public const long BI_BITFIELDS = 3L;

        /// <summary>
        /// JPEG encoding
        /// </summary>
        public const long BI_JPEG = 4L;

        /// <summary>
        /// PNG encoding
        /// </summary>
        public const long BI_PNG = 5L;

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDIBSection(
            IntPtr hdc, 
            IntPtr pbmi, 
            uint usage, 
            out IntPtr ppvBits, 
            IntPtr hSection, 
            int offset);

        /// <summary>
        /// Gray out an icon.
        /// </summary>
        /// <param name="icn">The input icon.</param>
        /// <returns>The grayed out icon.</returns>
        /// <remarks></remarks>
        public static Image GrayIcon(Icon icn)
        {
            var n = new Bitmap(
                icn.Width, 
                icn.Height, 
                PixelFormat.Format32bppArgb
                );

            var g = System.Drawing.Graphics.FromImage(n);

            g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, n.Width, n.Height));
            g.DrawIcon(icn, 0, 0);
            g.Dispose();
            
            using (var mm = new SafePtr((long)n.Width * n.Height * 4)) 
            {
                var bm = new BitmapData
                {
                    Stride = n.Width * 4,
                    Scan0 = mm,
                    PixelFormat = PixelFormat.Format32bppArgb,
                    Width = n.Width,
                    Height = n.Height
                };

                bm = n.LockBits(
                    new Rectangle(0, 0, n.Width, n.Height), 
                    ImageLockMode.ReadWrite | ImageLockMode.UserInputBuffer, 
                    PixelFormat.Format32bppArgb, 
                    bm
                    );

                var c = bm.Stride * bm.Height - 1;
                int stp = (int)(bm.Stride / (double)bm.Width);

                for (var i = 3; stp >= 0 ? i <= c : i >= c; i += stp)
                {
                    if (mm.ByteAt(i) > 0x7F)
                    {
                        mm.ByteAt(i) = 0x7F;
                    }
                }

                n.UnlockBits(bm);
                return n;
            }

        }

        /// <summary>
        /// Create a Device Independent Bitmap from an icon.
        /// </summary>
        /// <param name="icn"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IntPtr DIBFromIcon(Icon icn)
        {
            var bmp = IconToTransparentBitmap(icn);
            var _d = new IntPtr();

            return MakeDIBSection(bmp, ref _d);
        }

        /// <summary>
        /// Create a writable device-independent bitmap from the specified image.
        /// </summary>
        /// <param name="img">Image to copy.</param>
        /// <param name="bitPtr">Optional variable to receive a pointer to the bitmap bits.</param>
        /// <returns>A new DIB handle that must be destroyed with DeleteObject().</returns>
        /// <remarks></remarks>
        public static IntPtr MakeDIBSection(Bitmap img, ref IntPtr bitPtr)
        {
            // Build header.
            // adapted from C++ code examples.

            short wBitsPerPixel = 32;
            
            int BytesPerRow = (int)((double)(img.Width * wBitsPerPixel + 31 & ~31L) / 8d);
            
            int size = img.Height * BytesPerRow;
                        
            using (var mm = new SafePtr())
            {
                int bmpSizeOf = Marshal.SizeOf<BITMAPINFO>();

                var pbmih = new BITMAPINFOHEADER()
                {
                    biSize = Marshal.SizeOf<BITMAPINFOHEADER>(),
                    biWidth = img.Width,
                    biHeight = img.Height, // positive indicates bottom-up DIB
                    biPlanes = 1,
                    biBitCount = wBitsPerPixel,
                    biCompression = (int)BI_RGB,
                    biSizeImage = size,
                    biXPelsPerMeter = (int)(24.5d * 1000d), // pixels per meter! And these values MUST be correct if you want to pass a DIB to a native menu.
                    biYPelsPerMeter = (int)(24.5d * 1000d), // pixels per meter!
                    biClrUsed = 0,
                    biClrImportant = 0
                };

                var pPixels = IntPtr.Zero;
                int DIB_RGB_COLORS = 0;
                
                mm.FromStruct(pbmih);

                var hPreviewBitmap = CreateDIBSection(
                    IntPtr.Zero, 
                    mm, 
                    (uint)DIB_RGB_COLORS, 
                    out pPixels, 
                    IntPtr.Zero, 
                    0);
                
                bitPtr = pPixels;
               
                var bm = new BitmapData();

                bm = img.LockBits(
                    new Rectangle(0, 0, img.Width, img.Height), 
                    ImageLockMode.ReadWrite, 
                    PixelFormat.Format32bppPArgb, 
                    bm);

                var pCurrSource = bm.Scan0;

                // Our DIBsection is bottom-up...start at the bottom row...
                var pCurrDest = pPixels + (img.Width - 1) * BytesPerRow;
                // ... and work our way up
                int DestinationStride = -BytesPerRow;

                unsafe
                {
                    for (int curY = 0, ih = img.Height - 1; curY <= ih; curY++)
                    {
                        Buffer.MemoryCopy((void*)pCurrSource, (void*)pCurrDest, BytesPerRow, BytesPerRow);
                        pCurrSource = pCurrSource + bm.Stride;
                        pCurrDest = pCurrDest + DestinationStride;
                    }
                }

                // Free up locked buffer.
                img.UnlockBits(bm);
                return hPreviewBitmap;
            }
        }

        /// <summary>
        /// Converts a 32 bit icon into a 32 bit Argb transparent bitmap.
        /// </summary>
        /// <param name="icn">Input icon.</param>
        /// <returns>A 32-bit Argb Bitmap object.</returns>
        /// <remarks></remarks>
        public static Bitmap IconToTransparentBitmap(Icon icn)
        {
            if (icn == null) return null;

            var n = new Bitmap(
                icn.Width, 
                icn.Height, 
                PixelFormat.Format32bppArgb);

            var g = System.Drawing.Graphics.FromImage(n);
            
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            
            g.Clear(Color.Transparent);
            
            g.DrawIcon(icn, 0, 0);
            g.Dispose();

            return n;
        }
    }
}