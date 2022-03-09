using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
    /// Interaction logic for NamedColorPicker.xaml
    /// </summary>
    public partial class NamedColorPicker : ComboBox
    {

        internal protected NamedColorViewModel vm;


        new public NamedColorViewModel SelectedItem
        {
            get { return (NamedColorViewModel)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        new public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(NamedColorViewModel), typeof(NamedColorPicker), new PropertyMetadata(null, OnSelectedItemChanged));


        protected static void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is NamedColorPicker && sender is ComboBox cb && cb.SelectedItem != e.NewValue)
            {
                cb.SelectedItem = e.NewValue;
            }
        }

        new public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(ItemsSourceProperty);
            }
        }

        private static readonly DependencyPropertyKey ItemsSourcePropertyKey = DependencyProperty.RegisterReadOnly("ItemsSource", typeof(IEnumerable), typeof(NamedColorPicker), new PropertyMetadata(null));


        new public static readonly DependencyProperty ItemsSourceProperty = ItemsSourcePropertyKey.DependencyProperty;

        public NamedColorPicker()
        {
            InitializeComponent();

            vm = new NamedColorViewModel();
            base.ItemsSource = vm.AllNamedColors;

            DependencyPropertyDescriptor pdItemsSource = DependencyPropertyDescriptor.FromProperty(ComboBox.SelectedItemProperty, typeof(NamedColorPicker));
            pdItemsSource.AddValueChanged(this, (sender, e) =>
            {
                SelectedItem = (NamedColorViewModel)base.SelectedItem;
            });
        }

    }
}
