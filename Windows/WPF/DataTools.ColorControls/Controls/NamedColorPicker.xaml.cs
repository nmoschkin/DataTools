using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DataTools.ColorControls
{
    /// <summary>
    /// Interaction logic for NamedColorPicker.xaml
    /// </summary>
    public partial class NamedColorPicker : ComboBox
    {
        protected internal NamedColorViewModel vm;

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
            if (sender is NamedColorPicker ncp && e.NewValue is CatalogOptions co)
            {
                ncp.vm.ChangeCatalog(co);
            }
        }

        public new NamedColorViewModel SelectedItem
        {
            get { return (NamedColorViewModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(NamedColorViewModel), typeof(NamedColorPicker), new PropertyMetadata(null, OnSelectedItemChanged));

        protected static void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is NamedColorPicker && sender is ComboBox cb && cb.SelectedItem != e.NewValue)
            {
                cb.SelectedItem = e.NewValue;
            }
        }

        public new IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
        }

        private static readonly DependencyPropertyKey ItemsSourcePropertyKey = DependencyProperty.RegisterReadOnly("ItemsSource", typeof(IEnumerable), typeof(NamedColorPicker), new PropertyMetadata(null));

        public new static readonly DependencyProperty ItemsSourceProperty = ItemsSourcePropertyKey.DependencyProperty;

        public NamedColorPicker()
        {
            InitializeComponent();

            vm = new NamedColorViewModel(CatalogType);
            base.ItemsSource = vm.AllNamedColors;

            DependencyPropertyDescriptor pdItemsSource = DependencyPropertyDescriptor.FromProperty(ComboBox.SelectedItemProperty, typeof(NamedColorPicker));
            pdItemsSource.AddValueChanged(this, (sender, e) =>
            {
                SelectedItem = (NamedColorViewModel)base.SelectedItem;
            });
        }
    }
}