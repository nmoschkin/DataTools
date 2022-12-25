using DataTools.Desktop;
using DataTools.Essentials.SortedLists;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SysInfoTool
{
    public partial class Programs : Window
    {
        public AllSystemFileTypes FileTypes
        {
            get
            {
                return (AllSystemFileTypes)this.GetValue(FileTypesProperty);
            }

            set
            {
                this.SetValue(FileTypesProperty, value);
            }
        }

        public static readonly DependencyProperty FileTypesProperty = DependencyProperty.Register("FileTypes", typeof(AllSystemFileTypes), typeof(Programs), new PropertyMetadata(null));

        public ObservableCollection<UIHandler> Handlers
        {
            get
            {
                return (ObservableCollection<UIHandler>)this.GetValue(HandlersProperty);
            }

            set
            {
                this.SetValue(HandlersProperty, value);
            }
        }

        public static readonly DependencyProperty HandlersProperty = DependencyProperty.Register("Handlers", typeof(ObservableCollection<UIHandler>), typeof(Programs), new PropertyMetadata(null));

        public Programs()
        {
            this.InitializeComponent();

            this.Status.Text = "Enumerating File Types...";

            _Quit.Click += _Quit_Click;
            _Close.Click += _Close_Click;
            Loaded += Programs_Loaded;
        }

        private void _Quit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void _Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TypeEnumerated(object sender, FileTypeEnumEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.Status.Text = "Enumerated " + e.Index + " of " + e.Count + " types.  " + e.Type.Extension + " - " + e.Type.Description;
            });
        }

        private void Programs_Loaded(object sender, RoutedEventArgs e)
        {
            var th = new System.Threading.Thread(() =>
            {
                AllSystemFileTypes ft = null;

                Dispatcher.Invoke(() =>
                {
                    ft = new AllSystemFileTypes();
                    Handlers = new ObservableCollection<UIHandler>();

                    FileTypes = ft;

                    this.Cursor = Cursors.Wait;
                    this.UpdateLayout();

                    ft.PopulatingUIHandlers += Ft_PopulatingUIHandlers;
                    ft.Populating += TypeEnumerated;
                });

                ft?.Populate();

                Dispatcher.Invoke(() =>
                {
                    ft.PopulatingUIHandlers -= Ft_PopulatingUIHandlers;
                    ft.Populating -= TypeEnumerated;

                    QuickSort.Sort(Handlers, (a, b) =>
                    {
                        return string.Compare(a.UIName, b.UIName);
                    });

                    this.Cursor = Cursors.Arrow;
                    this.Status.Text = "Finished.";
                });
            });

            th.SetApartmentState(System.Threading.ApartmentState.STA);
            th.IsBackground = true;
            th.Start();
        }

        private void Ft_PopulatingUIHandlers(object sender, UIHandlerEnumEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Handlers.Add(e.Handler);
            });
        }
    }
}