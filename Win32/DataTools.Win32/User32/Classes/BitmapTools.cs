using DataTools.Win32.Memory;

using System.Drawing;
using System.Runtime.InteropServices;

namespace DataTools.Win32
{
    public static class BitmapTools
    {
        public const long BI_RGB = 0L;
        public const long BI_RLE8 = 1L;
        public const long BI_RLE4 = 2L;
        public const long BI_BITFIELDS = 3L;
        public const long BI_JPEG = 4L;
        public const long BI_PNG = 5L;

        [DllImport("gdi32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateDIBSection(IntPtr hdc, IntPtr pbmi, uint usage, ref IntPtr ppvBits, IntPtr hSection, int offset);

        /// <summary>
        /// Gray out an icon.
        /// </summary>
        /// <param name="icn">The input icon.</param>
        /// <returns>The grayed out icon.</returns>
        /// <remarks></remarks>
        public static Image GrayIcon(Icon icn)
        {
            var n = new Bitmap(icn.Width, icn.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var g = System.Drawing.Graphics.FromImage(n);
            g.FillRectangle(Brushes.Transparent, new Rectangle(0, 0, n.Width, n.Height));
            g.DrawIcon(icn, 0, 0);
            g.Dispose();
            var bm = new System.Drawing.Imaging.BitmapData();
            var mm = new MemPtr((long)n.Width * n.Height * 4);
            bm.Stride = n.Width * 4;
            bm.Scan0 = mm;
            bm.PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            bm.Width = n.Width;
            bm.Height = n.Height;
            bm = n.LockBits(new Rectangle(0, 0, n.Width, n.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite | System.Drawing.Imaging.ImageLockMode.UserInputBuffer, System.Drawing.Imaging.PixelFormat.Format32bppArgb, bm);
            int i;
            int c;

            // Dim b() As Byte

            // ReDim b((bm.Stride * bm.Height) - 1)
            // MemCpy(b, bm.Scan0, bm.Stride * bm.Height)
            c = bm.Stride * bm.Height - 1;
            int stp = (int)(bm.Stride / (double)bm.Width);

            // For i = 3 To c Step stp
            // If b(i) > &H7F Then b(i) = &H7F
            // Next

            for (i = 3; stp >= 0 ? i <= c : i >= c; i += stp)
            {
                if (mm.ByteAt(i) > 0x7F)
                    mm.ByteAt(i) = 0x7F;
            }

            // MemCpy(bm.Scan0, b, bm.Stride * bm.Height)
            n.UnlockBits(bm);
            mm.Free();
            return n;
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
            var bmpInfo = default(BITMAPINFO);
            var mm = new MemPtr();
            int bmpSizeOf = Marshal.SizeOf(bmpInfo);
            mm.ReAlloc(bmpSizeOf + size);
            var pbmih = default(BITMAPINFOHEADER);
            pbmih.biSize = Marshal.SizeOf(pbmih);
            pbmih.biWidth = img.Width;
            pbmih.biHeight = img.Height; // positive indicates bottom-up DIB
            pbmih.biPlanes = 1;
            pbmih.biBitCount = wBitsPerPixel;
            pbmih.biCompression = (int)BI_RGB;
            pbmih.biSizeImage = size;
            pbmih.biXPelsPerMeter = (int)(24.5d * 1000d); // pixels per meter! And these values MUST be correct if you want to pass a DIB to a native menu.
            pbmih.biYPelsPerMeter = (int)(24.5d * 1000d); // pixels per meter!
            pbmih.biClrUsed = 0;
            pbmih.biClrImportant = 0;
            var pPixels = IntPtr.Zero;
            int DIB_RGB_COLORS = 0;
            Marshal.StructureToPtr(pbmih, mm.Handle, false);
            var hPreviewBitmap = BitmapTools.CreateDIBSection(IntPtr.Zero, mm.Handle, (uint)DIB_RGB_COLORS, ref pPixels, IntPtr.Zero, 0);
            bitPtr = pPixels;
            var bm = new System.Drawing.Imaging.BitmapData();
            bm = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, bm);
            var pCurrSource = bm.Scan0;

            // Our DIBsection is bottom-up...start at the bottom row...
            var pCurrDest = pPixels + (img.Width - 1) * BytesPerRow;
            // ... and work our way up
            int DestinationStride = -BytesPerRow;

            for (int curY = 0, ih = img.Height - 1; curY <= ih; curY++)
            {
                Native.MemCpy(pCurrSource, pCurrDest, BytesPerRow);
                pCurrSource = pCurrSource + bm.Stride;
                pCurrDest = pCurrDest + DestinationStride;
            }

            // Free up locked buffer.
            img.UnlockBits(bm);
            return hPreviewBitmap;
        }

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
            var n = new Bitmap(icn.Width, icn.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
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