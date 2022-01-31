using DataTools.Desktop;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SysInfoTool.Converters
{
    internal class NativeImageToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Bitmap bitmap)
            {
                return BitmapTools.MakeWPFImage(bitmap);
            }
            else if (value is System.Drawing.Icon icon)
            {
                return BitmapTools.MakeWPFImage(icon);
            }
            else if (value is ImageSource img)
            {
                return img;
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
