using DataTools.MathTools.Polar;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DataTools.Graphics
{
    /// <summary>
    /// Unified rectangle structure for WinForms, Win32 API, and SkiaSharp.
    /// </summary>
    /// <remarks>
    /// Platform-specific libraries within this library family will implement extensions to convert to platform-specific objects.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct UniRect : INotifyPropertyChanged
    {
        private double _Left;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public double Left
        {
            get
            {
                return _Left;
            }

            set
            {
                _Left = value;
                OnPropertyChanged("Left");
                OnPropertyChanged("X");
            }
        }

        private double _Top;

        public double Top
        {
            get
            {
                return _Top;
            }

            set
            {
                _Top = value;
                OnPropertyChanged("Top");
                OnPropertyChanged("Y");
            }
        }

        private double _Width;

        public double Width
        {
            get
            {
                return _Width;
            }

            set
            {
                _Width = value;
                OnPropertyChanged("Width");
                OnPropertyChanged("Right");
                OnPropertyChanged("CX");
            }
        }

        private double _Height;

        public double Height
        {
            get
            {
                return _Height;
            }

            set
            {
                _Height = value;
                OnPropertyChanged("Height");
                OnPropertyChanged("Bottom");
                OnPropertyChanged("CY");
            }
        }

        public double Right
        {
            get
            {
                return _Width - _Left - 1d;
            }

            set
            {
                _Width = value - _Left + 1d;
                OnPropertyChanged("Height");
                OnPropertyChanged("Bottom");
                OnPropertyChanged("CY");
            }
        }

        public double Bottom
        {
            get
            {
                return _Height - _Top - 1d;
            }

            set
            {
                _Height = value - _Top + 1d;
                OnPropertyChanged("Width");
                OnPropertyChanged("Right");
                OnPropertyChanged("CX");
            }
        }

        public double X
        {
            get
            {
                return _Left;
            }
        }

        public double Y
        {
            get
            {
                return _Top;
            }
        }

        public double CX
        {
            get
            {
                return _Width;
            }
        }

        public double CY
        {
            get
            {
                return _Height;
            }
        }

        public UniRect(double x, double y, double width, double height)
        {
            _Left = x;
            _Top = y;
            _Width = width;
            _Height = height;

            PropertyChanged = null;
        }

        public UniRect(Point location, Size size)
        {
            _Left = location.X;
            _Top = location.Y;
            _Width = size.Width;
            _Height = size.Height;
            PropertyChanged = null;
        }

        public UniRect(PointF location, SizeF size)
        {
            _Left = location.X;
            _Top = location.Y;
            _Width = size.Width;
            _Height = size.Height;
            PropertyChanged = null;
        }

        public UniRect(Rectangle rectStruct)
        {
            _Left = rectStruct.Left;
            _Top = rectStruct.Top;
            _Width = rectStruct.Width;
            _Height = rectStruct.Height;
            PropertyChanged = null;
        }

        public UniRect(RectangleF rectStruct)
        {
            _Left = rectStruct.Left;
            _Top = rectStruct.Top;
            _Width = rectStruct.Width;
            _Height = rectStruct.Height;
            PropertyChanged = null;
        }

        public UniRect(LinearRect rc)
        {
            _Left = rc.Left;
            _Top = rc.Top;
            _Width = rc.Width;
            _Height = rc.Height;
            PropertyChanged = null;
        }

        event System.ComponentModel.PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}; {2}x{3}", _Left, _Top, _Width, _Height);
        }

        public static explicit operator RectangleF(UniRect operand)
        {
            return new RectangleF((float)operand._Left, (float)operand._Top, (float)operand._Width, (float)operand._Height);
        }

        public static implicit operator UniRect(RectangleF operand)
        {
            return new UniRect(operand);
        }

        public static explicit operator Rectangle(UniRect operand)
        {
            return new Rectangle((int)operand._Left, (int)operand._Top, (int)operand._Width, (int)operand._Height);
        }

        public static implicit operator UniRect(Rectangle operand)
        {
            return new UniRect(operand);
        }

        public static implicit operator double[](UniRect operand)
        {
            return new[] { operand.Left, operand.Top, operand.Right, operand.Bottom };
        }

        public static implicit operator LinearRect(UniRect operand)
        {
            return new LinearRect(operand.Left, operand.Top, operand.Width, operand.Height);
        }

        public static implicit operator UniRect(LinearRect operand)
        {
            return new UniRect(operand);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    }
}