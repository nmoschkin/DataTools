using DataTools.Graphics;

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

        private void BtnMore_Click(object sender, RoutedEventArgs e)
        {
            var color = ColorDialog.OpenDialog(Ncp.SelectedItem?.Source.Color.GetWPFColor());

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
