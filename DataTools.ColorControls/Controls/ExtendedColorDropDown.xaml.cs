﻿using DataTools.Graphics;

using System;
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

namespace DataTools.ColorControls
{
    /// <summary>
    /// Interaction logic for ExtendedColorDropDown.xaml
    /// </summary>
    public partial class ExtendedColorDropDown : UserControl
    {
        public ExtendedColorDropDown()
        {
            InitializeComponent();
        }


        public CatalogOptions CatalogType
        {
            get { return (CatalogOptions)GetValue(CatalogTypeProperty); }
            set { SetValue(CatalogTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CatalogType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CatalogTypeProperty =
            DependencyProperty.Register("CatalogType", typeof(CatalogOptions), typeof(NamedColorPicker), new PropertyMetadata(CatalogOptions.Extended, OnCatalogTypeChanged));


        private static void OnCatalogTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (sender is ExtendedColorDropDown exd && e.NewValue is CatalogOptions co)
            {
                exd.Ncp.vm.ChangeCatalog(co);
            }
        }

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {

            NamedColorViewModel? current = Ncp.SelectedItem as NamedColorViewModel;

            var color = ColorDialog.OpenDialog(current?.Source.Color.GetWPFColor());

            if (color != null)
            {
                var test = NamedColor.FindColor(((Color)color).GetUniColor());
                if (test != null)
                {
                    foreach (NamedColorViewModel nc in Ncp.vm.AllNamedColors)
                    {
                        if (nc.Source.Color == test)
                        {
                            Ncp.SelectedItem = nc;
                            return;
                        }
                    }
                }

                var item = new NamedColorViewModel(new NamedColor(((Color)color).GetUniColor()));
                Ncp.vm.AllNamedColors.AddNewItem(item);
                Ncp.SelectedItem = item;
            }
        }
    }
}
