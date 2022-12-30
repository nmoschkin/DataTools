using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using DataTools.Desktop;

namespace SysInfoTool.Converters
{
    internal class NativeImageToSourceConverter : IValueConverter
    {
        private static readonly Dictionary<IntPtr, BitmapSource> sessionCache = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Bitmap bitmap)
            {
                if (sessionCache.TryGetValue(bitmap.GetHbitmap(), out var src))
                {
                    return src;
                }

                src = BitmapTools.MakeWPFImage(bitmap);
                sessionCache.Add(bitmap.GetHbitmap(), src);
                return src;
            }
            else if (value is System.Drawing.Icon icon)
            {
                if (sessionCache.TryGetValue(icon.Handle, out var src))
                {
                    return src;
                }

                src = BitmapTools.MakeWPFImage(icon);
                sessionCache.Add(icon.Handle, src);
                return src;
            }
            else if (value is ImageSource img)
            {
                return img;
            }
            else if (value is null)
            {
                return null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}