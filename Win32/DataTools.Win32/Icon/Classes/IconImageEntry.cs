// *************************************************
// DataTools C# Native Utility Library For Windows - Interop
//
// Module: Icon File.
//         Icon image file format structure classes
//         Capable of using Windows Vista and greater .PNG icon images
//         Can create a complete icon file from scratch using images you add
//
// Icons are an old old format.  They have been adapted for modern use,
// and the reason that they endure is because of the ability to succintly
// store multiple image sizes in multiple formats, in a single file.
//
// But, because the 32-bit bitmap standard came around slightly afterward,
// certain acrobatic programming translations had to be made to get one from
// the other, and back again.
//
// Remember, back in the day, icon painting and design software was its own thing.
//
// Copyright (C) 2011-2017 Nathan Moschkin
// All Rights Reserved
//
// Licensed Under the Apache 2.0 License   
// *************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using DataTools.Win32;
using DataTools.Shell.Native;
using DataTools.Win32.Memory;

namespace DataTools.Desktop
{
    /// <summary>
    /// Represents a single icon image.
    /// </summary>
    /// <remarks></remarks>
    public class IconImageEntry : IDisposable
    {
        internal ICONDIRENTRY _entry;
        internal byte[] imageBytes;
        internal nint _hIcon = nint.Zero;


        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(nint hObject);

        internal IconImageEntry()
        {
        }

        /// <summary>
        /// Gets the raw ICONDIRENTRY structure.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        internal ICONDIRENTRY EntryInfo
        {
            get
            {
                return _entry;
            }
        }

        /// <summary>
        /// Create a new image from the pointer.
        /// </summary>
        /// <param name="ptr">Pointer to the start of the ICONDIRENTRY structure.</param>
        /// <remarks></remarks>
        internal IconImageEntry(nint ptr)
        {
            MemPtr mm = ptr;
            _entry = mm.ToStruct<ICONDIRENTRY>();
            ptr = ptr + _entry.dwOffset;
            if (_entry.wBitsPixel < 24)
            {
                // Throw New InvalidDataException("Reading low-bit icons is not supported")
            }

            imageBytes = new byte[_entry.dwImageSize];
            Marshal.Copy(ptr, imageBytes, 0, _entry.dwImageSize);

            // MemCpy(_image, ptr, _entry.dwImageSize)

        }

        /// <summary>
        /// Extract an icon from an entry and a bits pointer.
        /// </summary>
        /// <param name="entry">Icon entry structure.</param>
        /// <param name="ptr">Pointer to the bitmap.</param>
        /// <remarks></remarks>
        internal IconImageEntry(ICONDIRENTRY entry, nint ptr)
        {
            _entry = entry;
            if (_entry.wBitsPixel < 24)
            {
                // Throw New InvalidDataException("Reading low-bit icons is not supported")
            }

            ptr = ptr + _entry.dwOffset;
            imageBytes = new byte[_entry.dwImageSize];
            Marshal.Copy(ptr, imageBytes, 0, _entry.dwImageSize);

            // MemCpy(_image, ptr, _entry.dwImageSize)

        }

        /// <summary>
        /// Returns the icon type.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public StandardIcons StandardIconType
        {
            get
            {
                return _entry.wIconType;
            }
        }

        /// <summary>
        /// Converts this raw icon source into a managed System.Drawing.Icon image.
        /// </summary>
        /// <returns>A new Icon object.</returns>
        /// <remarks></remarks>
        public Icon ToIcon()
        {
            Icon iconOut;

            if (IsPngFormat)
            {
                Bitmap bmp = (Bitmap)ToImage();

                var bmi = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
                var lpicon = default(ICONINFO);

                int i;

                var bm = bmi.LockBits(new Rectangle(0, 0, bmi.Width, bmi.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format1bppIndexed);

                MemPtr mm = bm.Scan0;

                int z = (int)(Math.Max(bmp.Width, 32) * bmp.Height / 8d);

                for (i = 0; i < z; i++)
                    mm.ByteAt(i) = 255;

                bmi.UnlockBits(bm);

                lpicon.hbmColor = bmp.GetHbitmap();
                lpicon.hbmMask = bmi.GetHbitmap();
                lpicon.fIcon = 1;

                var hIcon = User32.CreateIconIndirect(ref lpicon);

                if (hIcon != nint.Zero)
                {
                    iconOut = (Icon)Icon.FromHandle(hIcon).Clone();
                    User32.DestroyIcon(hIcon);
                }
                else
                {
                    iconOut = null;
                }

                DeleteObject(lpicon.hbmMask);
                DeleteObject(lpicon.hbmColor);
            }
            else
            {
                iconOut = _constructIcon();
            }

            return iconOut;
        }

        /// <summary>
        /// Retrieves the width of the icon.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Width
        {
            get
            {
                if (_entry.cWidth == 0)
                    return 256;
                else
                    return _entry.cWidth;
            }
        }

        /// <summary>
        /// Retrieves the height of the icon.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public int Height
        {
            get
            {
                if (_entry.cHeight == 0)
                    return 256;
                else
                    return _entry.cHeight;
            }
        }

        /// <summary>
        /// Returns True if the icon image data is in compressed PNG format.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool IsPngFormat
        {
            get
            {
                if (imageBytes is null)
                    return false;

                SafePtr mm = (SafePtr)imageBytes;
                int q = mm.IntAt(0L);

                // The PNG moniker
                return q == 0x474E5089;
            }
        }

        /// <summary>
        /// Returns a new System.Drawing.Image from this raw icon.
        /// </summary>
        /// <returns>A new Image object.</returns>
        /// <remarks></remarks>
        public Image ToImage()
        {
            Image result;

            if (IsPngFormat)
            {
                var s = new MemoryStream(imageBytes);
                result = Image.FromStream(s);
                
                s.Close();
            }
            else
            {
                result = Resources.IconToTransparentBitmap(_constructIcon());
            }

            return result;
        }

        /// <summary>
        /// Create a bitmap structure with bits from this raw icon image.
        /// </summary>
        /// <returns>An array of bytes that represent data to create a bitmap.</returns>
        /// <remarks></remarks>
        private byte[] _makeBitmap()
        {
            byte[] bytesOut;
            
            if (!IsPngFormat)
            {
                bytesOut = imageBytes;
                return bytesOut;
            }

            nint bmp = default;
        
            var hbmp = Resources.MakeDIBSection((Bitmap)ToImage(), ref bmp);
            
            var mm = new SafePtr();
            var bm = new BITMAPINFOHEADER();
            
            int maskSize;
            
            int w = _entry.cWidth;
            int h = _entry.cHeight;
            
            if (w == 0)
                w = 256;
            
            if (h == 0)
                h = 256;
            
            bm.biSize = 40;
            bm.biWidth = w;
            bm.biHeight = h * 2;
            bm.biPlanes = 1;
            bm.biBitCount = 32;
            bm.biSizeImage = w * h * 4;

            maskSize = (int)(Math.Max(w, 32) * h / 8d);

            mm.Alloc(bm.biSizeImage + 40 + maskSize);

            var ptr1 = mm.DangerousGetHandle() + 40;
            var ptr2 = mm.DangerousGetHandle() + 40 + bm.biSizeImage;

            Marshal.StructureToPtr(bm, mm.DangerousGetHandle(), false);
            Native.MemCpy(bmp, ptr1, bm.biSizeImage);

            bm = mm.ToStruct<BITMAPINFOHEADER>();

            _setMask(ptr1, ptr2, w, h);
            _entry.dwImageSize = (int)mm.Length;

            bytesOut = (byte[])mm;

            mm.Free();
            DeleteObject(hbmp);

            return bytesOut;
        }

        /// <summary>
        /// Construct an icon from the raw image data.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private Icon _constructIcon()
        {
            Icon _constructIconRet = default;
            if (_hIcon != nint.Zero)
            {
                User32.DestroyIcon(_hIcon);
                _hIcon = nint.Zero;
            }

            MemPtr mm = (MemPtr)imageBytes;
            var bmp = mm.ToStruct<BITMAPINFOHEADER>();

            nint hBmp;
            nint ptr;
            nint ppBits = new nint();

            var lpicon = default(ICONINFO);

            nint hicon;
            nint hBmpMask = new nint();

            bool hasMask;
            if (bmp.biHeight == bmp.biWidth * 2)
            {
                hasMask = true;
                bmp.biHeight = (int)(bmp.biHeight / 2d);
            }
            else
            {
                hasMask = false;
            }

            bmp.biSizeImage = (int)(bmp.biWidth * bmp.biHeight * (bmp.biBitCount / 8d));

            bmp.biXPelsPerMeter = (int)(24.5d * 1000d);
            bmp.biYPelsPerMeter = (int)(24.5d * 1000d);

            bmp.biClrUsed = 0;
            bmp.biClrImportant = 0;
            bmp.biPlanes = 1;
            
            Marshal.StructureToPtr(bmp, mm.Handle, false);
            
            ptr = mm.Handle + bmp.biSize;
            
            if (bmp.biSize != 40)
                return null;
            
            hBmp = User32.CreateDIBSection(nint.Zero, mm.Handle, 0U, ref ppBits, nint.Zero, 0);

            Native.MemCpy(ptr, ppBits, bmp.biSizeImage);
            
            if (hasMask)
            {
                ptr = ptr + bmp.biSizeImage;
                bmp.biBitCount = 1;
                bmp.biSizeImage = 0;
                Marshal.StructureToPtr(bmp, mm.Handle, false);
                hBmpMask = User32.CreateDIBSection(nint.Zero, mm.Handle, 0U, ref ppBits, nint.Zero, 0);
                Native.MemCpy(ptr, ppBits, (long)(Math.Max(bmp.biWidth, 32) * bmp.biHeight / 8d));
            }

            lpicon.fIcon = 1;
            lpicon.hbmColor = hBmp;
            lpicon.hbmMask = hBmpMask;
            hicon = User32.CreateIconIndirect(ref lpicon);
            DeleteObject(hBmp);
            if (hasMask)
                DeleteObject(hBmpMask);
            _constructIconRet = Icon.FromHandle(hicon);
            _hIcon = hicon;
            mm.Free();
            return _constructIconRet;
        }

        /// <summary>
        /// Apply transparency mask bits to the output image (for converting to a bitmap)
        /// </summary>
        /// <param name="hBits">A pointer to the memory address of the bitmap bits.</param>
        /// <param name="hMask">A pointer to the memory address of the mask bits.</param>
        /// <param name="Width">The width of the image.</param>
        /// <param name="Height">The height of the image.</param>
        /// <remarks></remarks>
        private void _applyMask(MemPtr hBits, MemPtr hMask, int Width, int Height)
        {
            // Masks in icon images are bitstreams wherein a single bit represents a 1 or 0 transparency
            // for an entire pixel on the screen.  In order to convert an icon into a 32 bit images,
            // we need to access each individual bit, and apply the NOT of the value to the byte-length alpha mask
            // of the bitmap.

            int x;
            int y;
            int shift;
            int bit;
            int shift2;
            int mask;

            // in transparency masks for icons, the minimum stride is 32 pixels/bits, no matter the actual size of the image.
            int boundary = Math.Max(32, Width);

            // walk every pixel of the image.
            
            for (y = 0; y < Height; y++)
            {
                for (x = 0; x < Width; x++)
                {

                    // the first shift is our position in the bitmap output.
                    // 4 bytes is 32 bits ... then we add 3 to get directly 
                    // to the alpha mask.
                    shift = 4 * (x + y * Width) + 3;

                    // we find the exact bit-wise position by modulus with 8 (the length of a byte, in bits)
                    bit = 7 - x % 8;

                    // the second shift is the position in the bitmask, byte-wise.  We subtract 1 from y before subtracting it from the
                    // height because the first scan line is 0.
                    shift2 = (int)((x + (Height - y - 1) * boundary) / 8d);

                    // we get a number that is either 1 or 0 from the mask by accessing the exact byte, and then
                    // accessing the exact bit by left-shifting its value into the 1 position.
                    mask = 1 & hMask.ByteAt(shift2) >> bit;

                    // we do a quick logical AND via multiplication with the inverse of the mask.
                    // we do this because alpha channel 0 is transparent, but transparent mask 1 is also transparent.
                    hBits.ByteAt(shift) *= (byte)(1 - mask);
                }
            }
        }

        /// <summary>
        /// Create a transparency mask from the transparent bits in an image.
        /// </summary>
        /// <param name="hBits">A pointer to the memory address of the bitmap bits.</param>
        /// <param name="hMask">A pointer to the memory address of the mask bits.</param>
        /// <param name="Width">The width of the image.</param>
        /// <param name="Height">The height of the image.</param>
        /// <remarks></remarks>
        private void _setMask(MemPtr hBits, MemPtr hMask, int Width, int Height)
        {
            // this never changes
            int numBits = 32;

            int x;
            int y;

            byte bit;

            int d;
            int e;
            int f;

            double move = numBits / 8d;

            int stride = (int)(Width * (numBits / 8d));
            int msize = (int)(Math.Max(32, Width) * Height / 8d);
            int isize = (int)(Width * Height * (numBits / 8d));

            byte[] bb;
            byte[] imgb;

            imgb = new byte[isize];
            bb = new byte[msize];

            Marshal.Copy(hBits.Handle, imgb, 0, isize);

            for (y = 0; y < Height; y++)
            {
                d = y * stride;

                for (x = 0; x < Width; x++)
                {

                    f = (int)(d + x * move);
                    e = (int)Math.Floor(x / 8d);

                    bit = (byte)(7 - x % 8);

                    if (imgb[f + 3] == 0)
                    {
                        bb[e] = (byte)(bb[e] | 1 << bit);
                    }
                }
            }

            // MemCpy(hMask.Handle, bb, msize)

            hMask.FromByteArray(bb, 0L);
        }

        /// <summary>
        /// Initialize a new raw icon image from a standard image with the specified size as either a bitmap or PNG icon.
        /// </summary>
        /// <param name="Image">Image from which to construct the icon.</param>
        /// <param name="size">The standard size of the new icon.</param>
        /// <param name="asBmp">Whether to create a bitmap icon (if false, a PNG icon is created).</param>
        /// <remarks></remarks>
        public IconImageEntry(Image Image, StandardIcons size, bool asBmp = false)
        {
            int sz = (int)size != 0 ? (int)(size) & 0xFF : 256;
            Bitmap im = (Bitmap)Image;

            if (Image.Width != sz | Image.Height != sz)
            {
                im = new Bitmap(sz, sz, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                var g = System.Drawing.Graphics.FromImage(im);

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                g.DrawImage(Image, new Rectangle(0, 0, sz, sz));

                g.Dispose();
            }

            var s = new MemoryStream();

            im.Save(s, System.Drawing.Imaging.ImageFormat.Png);

            _entry.wIconType = size;
            _entry.wBitsPixel = 32;
            _entry.wColorPlanes = 1;

            imageBytes = new byte[(int)(s.Length - 1L) + 1];
            imageBytes = s.ToArray();

            if (asBmp)
            {
                imageBytes = _makeBitmap();
            }

            _entry.dwImageSize = imageBytes.Length;

            s.Close();
        }


        // IDisposable
        private bool disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (_hIcon != nint.Zero)
                {
                    User32.DestroyIcon(_hIcon);
                }
            }

            disposedValue = true;
        }

        ~IconImageEntry()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
