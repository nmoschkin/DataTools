using DataTools.Win32.Network;

using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DataTools.Desktop.Network
{
    public class NetworkAdapterViewModel : NetworkAdapter
    {
        protected BitmapSource deviceImage;

        internal NetworkAdapterViewModel() : base()
        {
        }

        /// <summary>
        /// Gets the WPF device image for the device.
        /// </summary>
        public virtual BitmapSource DeviceImage
        {
            get => deviceImage;
            set
            {
                deviceImage = value;
                OnPropertyChanged();
            }
        }

        public override Bitmap DeviceIcon
        {
            get => base.DeviceIcon;
            protected set
            {
                base.DeviceIcon = value;
                if (value != null)
                    DeviceImage = WPFBitmapTools.MakeWPFImage(value);
                else
                    DeviceImage = null;
            }
        }

        ~NetworkAdapterViewModel()
        {
            Dispose(false);
        }
    }
}