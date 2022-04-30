﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DataTools.Graphics;
using DataTools.Windows.Extensions;
using DataTools.MathTools.PolarMath;

using static DataTools.Graphics.ColorMath;
using System.Runtime.CompilerServices;

namespace DataTools.ColorControls
{
    
    /// <summary>
    /// How to select color names.
    /// </summary>
    public enum ColorNameResolution
    {
        /// <summary>
        /// Return a color name for an exact color match, only.
        /// </summary>
        Exact,

        /// <summary>
        /// Return a color name for a closely matched color.
        /// </summary>
        Closest
    }

    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorElement : UserControl
    {
        ColorPickerRenderer cpRender;

        public delegate void ColorHitEvent(object sender, ColorHitEventArgs e);
        public event ColorHitEvent ColorHit;
        public event ColorHitEvent ColorOver;

        ColorPickerElement? selectedElement;
        bool updateForValueChange;




        /// <summary>
        /// Gets or sets a value that indicates the hue box is rendered tetrachromatically
        /// </summary>
        public bool Tetrachromatic
        {
            get { return (bool)GetValue(TetrachromaticProperty); }
            set { SetValue(TetrachromaticProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Tetrachromatic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TetrachromaticProperty =
            DependencyProperty.Register("Tetrachromatic", typeof(bool), typeof(ColorElement), new PropertyMetadata(false));



        public int HuePointerSize
        {
            get { return (int)GetValue(HuePointerSizeProperty); }
            set { SetValue(HuePointerSizeProperty, value); }
        }

        public static readonly DependencyProperty HuePointerSizeProperty =
            DependencyProperty.Register(nameof(HuePointerSize), typeof(int), typeof(ColorElement), new PropertyMetadata(5, HuePointerSizePropertyChanged));

        private static void HuePointerSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((int)e.OldValue != (int)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }




        public int HueWheelThickness
        {
            get { return (int)GetValue(HueWheelThicknessProperty); }
            set { SetValue(HueWheelThicknessProperty, value); }
        }

        public static readonly DependencyProperty HueWheelThicknessProperty =
            DependencyProperty.Register(nameof(HueWheelThickness), typeof(int), typeof(ColorElement), new PropertyMetadata(8, HueWheelThicknessPropertyChanged));

        private static void HueWheelThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((int)e.OldValue != (int)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }


        public double HueOffset
        {
            get { return (double)GetValue(HueOffsetProperty); }
            set { SetValue(HueOffsetProperty, value); }
        }

        public static readonly DependencyProperty HueOffsetProperty =
            DependencyProperty.Register(nameof(HueOffset), typeof(double), typeof(ColorElement), new PropertyMetadata(0d, HueOffsetPropertyChanged));

        private static void HueOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((double)e.OldValue != (double)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }





        public ColorNameResolution NameResolution
        {
            get { return (ColorNameResolution)GetValue(NameResolutionProperty); }
            set { SetValue(NameResolutionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NameResolution.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameResolutionProperty =
            DependencyProperty.Register("NameResolution", typeof(ColorNameResolution), typeof(ColorElement), new PropertyMetadata(ColorNameResolution.Closest, NameResolutionChanged));

        private static void NameResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((int)e.OldValue != (int)e.NewValue)
                {
                    //p.RenderPicker();
                    p.SetSelectedColor();
                }
            }
        }

        public string SelectedColorName
        {
            get { return (string)GetValue(SelectedColorNameProperty); }
            set { SetValue(SelectedColorNameProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorNameProperty =
            DependencyProperty.Register(nameof(SelectedColorName), typeof(string), typeof(ColorElement), new PropertyMetadata("", SelectedColorNamePropertyChanged));

        private static void SelectedColorNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                string nv = (string)e.NewValue;
                string ov = (string)e.OldValue;

                if (ov != nv)
                {
                    if (string.IsNullOrEmpty(nv) || nv.StartsWith("#"))
                    {
                        p.SelectedNamedColors = new List<NamedColor>();
                    }
                    else
                    {
                        p.SelectedNamedColors = NamedColor.SearchByName(nv, true);
                    }
                }
            }
        }

        public Color? SelectedColor
        {
            get { return (Color?)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorElement), new PropertyMetadata(null, SelectedColorPropertyChanged));

        private static void SelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((Color?)e.OldValue != (Color?)e.NewValue)
                {
                    p.SetSelectedColor();
                }
            }
        }


        public IReadOnlyCollection<NamedColor> SelectedNamedColors
        {
            get { return (IReadOnlyCollection<NamedColor>)GetValue(SelectedNamedColorsProperty); }
            set { SetValue(SelectedNamedColorsProperty, value); }
        }

        public static readonly DependencyProperty SelectedNamedColorsProperty =
            DependencyProperty.Register(nameof(SelectedNamedColors), typeof(IReadOnlyCollection<NamedColor>), typeof(ColorElement), new PropertyMetadata(new List<NamedColor>(), SelectedNamedColorsPropertyChanged));

        private static void SelectedNamedColorsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((IReadOnlyCollection<NamedColor>)e.OldValue != (IReadOnlyCollection<NamedColor>)e.NewValue)
                {
                    //p.RenderPicker();
                    
                }
            }
        }


        public bool InvertSaturation
        {
            get { return (bool)GetValue(InvertSaturationProperty); }
            set { SetValue(InvertSaturationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InvertSaturation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InvertSaturationProperty =
            DependencyProperty.Register(nameof(InvertSaturation), typeof(bool), typeof(ColorElement), new PropertyMetadata(false, InvertSaturationPropertyChanged));

        private static void InvertSaturationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((bool)e.OldValue != (bool)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }




        public float ElementSize
        {
            get { return (float)GetValue(ElementSizeProperty); }
            set { SetValue(ElementSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HexagonSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementSizeProperty =
            DependencyProperty.Register(nameof(ElementSize), typeof(float), typeof(ColorElement), new PropertyMetadata(1f, ElementSizePropertyChanged));

        private static void ElementSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((float)e.OldValue != (float)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }


        public double ColorValue
        {
            get { return (double)GetValue(ColorValueProperty); }
            set { SetValue(ColorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorValueProperty =
            DependencyProperty.Register(nameof(ColorValue), typeof(double), typeof(ColorElement), new PropertyMetadata(1d, ColorValuePropertyChanged));

        private static void ColorValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((double)e.OldValue != (double)e.NewValue)
                {
                    double nv = (double)e.NewValue;

                    if (nv < 0d)
                    {
                        p.ColorValue = 0d;
                        return;
                    }
                    else if (nv > 1d)
                    {
                        p.ColorValue = 1d;
                        return;
                    }

                    p.updateForValueChange = true;
                    p.InvalidateVisual();
                }
            }
        }

        public ColorPickerMode Mode
        {
            get { return (ColorPickerMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(ColorPickerMode), typeof(ColorElement), new PropertyMetadata(ColorPickerMode.Wheel, ModePropertyChanged));


        private static void ModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorElement p)
            {
                if ((ColorPickerMode)e.OldValue != (ColorPickerMode)e.NewValue)
                {
                    //p.RenderPicker();
                    p.InvalidateVisual();
                }
            }
        }

        public ColorElement()
        {
            InitializeComponent();

            // this.SizeChanged += ColorPicker_SizeChanged;
            CursorCanvas.MouseMove += PickerSite_MouseMove;
            CursorCanvas.MouseDown += PickerSite_MouseDown;
            CursorCanvas.MouseUp += PickerSite_MouseUp;
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RenderPicker();
            var p = new Pen();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            RenderPicker((int)sizeInfo.NewSize.Width, (int)sizeInfo.NewSize.Height);
        }

        private void SetSelectedColor(UniColor? selc = null)
        {
            UniColor clr;

            if (selc != null) 
            {
                clr = (UniColor)selc;
                SelectedColor = Color.FromArgb(clr.A, clr.R, clr.G, clr.B);

                return;
            }
            else if (SelectedColor == null)
            {
                SelectedColorName = null;
                Point.Visibility = Surround.Visibility = Visibility.Hidden;
                HuePicker.Visibility = Visibility.Hidden;
                return;
            }

            clr = ((Color)SelectedColor).GetUniColor();

            var nc = NamedColor.FindColor(clr, NameResolution == ColorNameResolution.Closest);

            if (nc != null)
            {
                SelectedColorName = nc.Name;
            }
            else
            {
                SelectedColorName = clr.ToString(UniColorFormatOptions.HexRgbWebFormat);
            }

            if (cpRender == null) return;

            HSVDATA hsv1 = ColorToHSV(clr);
            HSVDATA hsv2;
            HSVDATA hsv3;
            HSVDATA? hsv4 = null;
            HSVDATA? hsv5 = null;
            UniColor uc;

            ColorPickerElement cel = new ColorPickerElement();

            foreach (var c in cpRender.Elements)
            {
                uc = c.Color;

                hsv2 = ColorToHSV(uc);
                hsv3 = (hsv1 - hsv2).Abs();

                if (hsv4 == null)
                {
                    hsv4 = hsv3;
                }
                else if (hsv3 < hsv4)
                {
                    hsv5 = hsv2;
                    hsv4 = hsv3;
                    cel = c;
                }

                if (selc == uc)
                {
                    cel = c;
                    break;
                }
            }

            if (Mode == ColorPickerMode.HueWheel && !double.IsNaN(ActualWidth) && !double.IsNaN(ActualHeight) && ActualWidth != -1 && ActualHeight != -1)
            {
                if (hsv5 is HSVDATA hsv)
                {
                    Point.Visibility = Surround.Visibility = Visibility.Hidden;
                    HuePicker.Visibility = Visibility.Visible;
                    int hp = HuePointerSize;

                    PolarCoordinates pc = new PolarCoordinates();

                    double arc = hsv.Hue - HueOffset;
                    if (arc < 0) arc += 360;

                    int rad;
                    int h = (int)ActualHeight, w = (int)ActualWidth;
                     
                    if (h < w)
                    {
                        rad = h / 2;
                        w = h;
                    }
                    else
                    {
                        rad = w / 2;
                        h = w;
                    }

                    pc.Arc = arc;
                    pc.Radius = rad;

                    var lc = pc.ToScreenCoordinates(new Rect(0, 0, w, h));

                    if (lc.X < (w / 2))
                        lc.X -= hp;

                    if (lc.Y < (h / 2))
                        lc.Y -= hp;

                    HuePicker.SetValue(Canvas.LeftProperty, lc.X);
                    HuePicker.SetValue(Canvas.TopProperty, lc.Y);

                    HueSize.ScaleX = (HuePointerSize / 5);
                    HueSize.ScaleY = (HuePointerSize / 5);

                    HueAngle.Angle = pc.Arc;
                }

            }
            else
            {
                Point.Visibility = Surround.Visibility = Visibility.Visible;
                HuePicker.Visibility = Visibility.Hidden;

                Point.SetValue(Canvas.LeftProperty, (double)cel.Center.X);
                Point.SetValue(Canvas.TopProperty, (double)cel.Center.Y);

                Surround.SetValue(Canvas.LeftProperty, (double)cel.Center.X - 8);
                Surround.SetValue(Canvas.TopProperty, (double)cel.Center.Y - 8);

                Surround.Stroke = Point.Stroke = new SolidColorBrush((Color)SelectedColor);
                selectedElement = cel;

            }


        }

        bool dragPick = false;

        private void PickerSite_MouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(PickerSite);
            var c = cpRender.HitTest((int)pt.X, (int)pt.Y);

            if (c == System.Drawing.Color.Empty && e.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
            else
            {
                Cursor = Cursors.Cross;
            }

            if (dragPick)
            {
                var ev = new ColorHitEventArgs(c);

                ColorOver?.Invoke(this, ev);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SetSelectedColor(c);
                    ColorHit?.Invoke(this, ev);
                }
            }
        }

        private void PickerSite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    SelectedColor = null;
                    SetSelectedColor(null);
                    dragPick = false;

                    return;
                } 

                var pt = e.GetPosition(PickerSite);
                var c = cpRender.HitTest((int)pt.X, (int)pt.Y);

                SetSelectedColor(c);
                ColorHit?.Invoke(this, new ColorHitEventArgs(c));
                dragPick = true;
            }

        }

        private void PickerSite_MouseUp(object sender, MouseButtonEventArgs e)
        {
            dragPick = false;
        }
        private void ColorPicker_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //RenderPicker((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Point.Visibility = Surround.Visibility = Visibility.Hidden;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            SetSelectedColor();
        }

        private void RenderPicker(int w = 0, int h = 0)
        {
            var disp = Dispatcher;

            double width = this.ActualWidth;
            double height = this.ActualHeight;
            double colorVal = this.ColorValue;
            double offset = this.HueOffset;
            bool invert = this.InvertSaturation;
            bool tetra = this.Tetrachromatic;
            float esize = this.ElementSize;
            int hwt = this.HueWheelThickness;
            int ps = this.HuePointerSize;

            ColorPickerMode mode = this.Mode;

            _ = Task.Run(() =>
            {
                ColorPickerRenderer cw;

                if (w <= 0)
                {
                    if (double.IsNaN(width)) return;
                    w = (int)width;
                }
                if (h <= 0)
                {
                    if (double.IsNaN(height)) return;
                    h = (int)height;
                }

                if (w < 32 || h < 32) return;

                if (mode == ColorPickerMode.Wheel || mode == ColorPickerMode.HexagonWheel || mode == ColorPickerMode.HueWheel)
                {
                    int rad;

                    if (h < w)
                    {
                        rad = h / 2;
                        w = h;
                    }
                    else
                    {
                        rad = w / 2;
                        h = w;
                    }

                    if (mode == ColorPickerMode.Wheel)
                    {
                        cw = new ColorPickerRenderer(rad, colorVal, offset, invert);
                    }
                    else if (mode == ColorPickerMode.HueWheel)
                    {
                        rad -= ((ps / 2) + (ps / 5));
                        cw = new ColorPickerRenderer(rad, colorVal, offset, invert, false, hwt);
                    }
                    else
                    {
                        cw = new ColorPickerRenderer(rad, esize, colorVal, invert);
                    }

                }
                else
                {

                    if (mode == ColorPickerMode.HueBoxHorizontal || mode == ColorPickerMode.HueBoxVertical)
                    {
                        cw = new ColorPickerRenderer(w, h, true, invert, mode == ColorPickerMode.HueBoxVertical, tetra);
                    }
                    else
                    {
                        cw = new ColorPickerRenderer(w, h, colorVal, offset, invert, mode == ColorPickerMode.LinearVertical || mode == ColorPickerMode.HueBarVertical, false, mode == ColorPickerMode.HueBarHorizontal || mode == ColorPickerMode.HueBarVertical);
                    }
                }


                disp.Invoke(() =>
                {
                    CursorCanvas.Width = w;
                    CursorCanvas.Height = h;

                    //CursorCanvas.RenderSize = new Size(w, h);
                    cpRender = cw;
                    PickerSite.Source = DataTools.Desktop.BitmapTools.MakeWPFImage(cpRender.Bitmap);



                    UniColor? selc = null;

                    //if (updateForValueChange && SelectedColor is Color scolor && selectedElement is ColorWheelElement selem)
                    //{
                    //    var pp = selem.Center;

                    //    foreach (var testelem in cw.Elements)
                    //    {
                    //        if (testelem.Bounds.Contains(pp))
                    //        {

                    //            SetSelectedColor(new UniColor(testelem.Color.ToArgb()));
                    //            break;
                    //        }
                    //    }
                    //}

                    //updateForValueChange = false;

                    if (selc == null)
                    {
                        SetSelectedColor();
                    }

                });

            });
        }


    }
}
