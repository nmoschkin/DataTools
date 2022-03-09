using System;
using System.Collections;
using System.Collections.Generic;
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
            if (sender is ComboBox cb)
            {
                cb.SelectedItem = e.NewValue;
            }
        }

        new public ListCollectionView ItemsSource
        {
            get { return (ListCollectionView)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        new public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ListCollectionView), typeof(NamedColorPicker), new PropertyMetadata(null, OnItemsSourceChanged));

        protected static void OnItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                cb.ItemsSource = (IEnumerable)e.NewValue;
            }
        }

        public NamedColorPicker()
        {
            InitializeComponent();

            DataContext = vm = new NamedColorViewModel();            
            
            SetBinding(ItemsSourceProperty, new Binding(nameof(NamedColorViewModel.AllNamedColors)));
            SetBinding(SelectedItemProperty, new Binding(nameof(NamedColorViewModel.SelectedColor)));
        }




    }
}
