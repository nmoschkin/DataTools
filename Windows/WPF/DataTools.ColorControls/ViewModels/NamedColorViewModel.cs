using DataTools.Essentials.Observable;
using DataTools.Graphics;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace DataTools.ColorControls
{
    [Flags]
    public enum CatalogOptions
    {
        Extended = 1,
        Web = 2,
        Both = 3
    }

    /// <summary>
    /// Wraps <see cref="NamedColor"/> in an MVVM ViewModel
    /// </summary>
    public class NamedColorViewModel : ObservableBase
    {
        #region Private Fields

        private static object lockObj = new object();
        private CatalogOptions catalog;
        private string category;
        private ListCollectionView colors;
        private bool isSourceList;
        private object localObj = new object();
        private NamedColorViewModel selColor;
        private NamedColor source;

        #endregion Private Fields

        #region Public Constructors

        static NamedColorViewModel()
        {
            RefreshColorCatalog();
        }

        public NamedColorViewModel(CatalogOptions catalog = CatalogOptions.Extended)
        {
            isSourceList = true;
            ChangeCatalog(catalog);
        }

        public NamedColorViewModel(NamedColor source)
        {
            isSourceList = false;
            Source = source;
        }

        #endregion Public Constructors

        #region Public Properties

        public static ReadOnlyCollection<NamedColorViewModel> GlobalCatalog { get; private set; }
        public static ReadOnlyCollection<NamedColorViewModel> WebCatalog { get; private set; }

        public ListCollectionView AllNamedColors
        {
            get => colors;
            protected set
            {
                SetProperty(ref colors, value);
            }
        }

        public CatalogOptions Catalog
        {
            get => catalog;
        }

        public string Category
        {
            get => category;
            set
            {
                SetProperty(ref category, value);
            }
        }

        public Color Color
        {
            get => source.Color.GetWPFColor();
        }

        public bool IsSourceList => isSourceList;

        public string Name
        {
            get => source.Name;
        }

        public NamedColorViewModel SelectedColor
        {
            get => selColor;
            set
            {
                SetProperty(ref selColor, value);
            }
        }

        public NamedColor Source
        {
            get => source;
            set
            {
                if (SetProperty(ref source, value))
                {
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(Color));
                    SetCategory();
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static void RefreshColorCatalog()
        {
            lock (lockObj)
            {
                var l = new List<NamedColorViewModel>();

                foreach (var clr in NamedColor.Catalog)
                {
                    l.Add(new NamedColorViewModel(clr));
                }

                GlobalCatalog = new ReadOnlyCollection<NamedColorViewModel>(l);

                l.Clear();

                foreach (var clr in NamedColor.WebCatalog)
                {
                    l.Add(new NamedColorViewModel(clr));
                }

                WebCatalog = new ReadOnlyCollection<NamedColorViewModel>(l);
            }
        }

        public void ChangeCatalog(CatalogOptions catalog)
        {
            if (!isSourceList) return;

            lock (lockObj)
            {
                var l = new List<NamedColorViewModel>();

                if ((catalog & CatalogOptions.Web) == CatalogOptions.Web)
                {
                    foreach (var clr in GlobalCatalog)
                    {
                        l.Add(clr);
                    }
                }
                if ((catalog & CatalogOptions.Extended) == CatalogOptions.Extended)
                {
                    foreach (var clr in GlobalCatalog)
                    {
                        l.Add(clr);
                    }
                }

                AllNamedColors = new ListCollectionView(l);
                AllNamedColors.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            }

            SetProperty(ref this.catalog, catalog);
        }

        #endregion Public Methods

        #region Private Methods

        private void SetCategory()
        {
            var hsv = Source.Color.ToHSV();
            if (Source.ExtraInfo == "Standard")
            {
                Category = "Standard";
            }
            else if (hsv.Hue.IsGrayScale)
            {
                Category = "Grayscale";
            }
            else if (hsv.Hue >= 330 || hsv.Hue < 30)
            {
                Category = "Red";
            }
            else if (hsv.Hue >= 30 && hsv.Hue < 90)
            {
                Category = "Yellow";
            }
            else if (hsv.Hue >= 90 && hsv.Hue < 150)
            {
                Category = "Green";
            }
            else if (hsv.Hue >= 150 && hsv.Hue < 210)
            {
                Category = "Cyan";
            }
            else if (hsv.Hue >= 210 && hsv.Hue < 270)
            {
                Category = "Blue";
            }
            else
            {
                Category = "Purple";
            }
        }

        #endregion Private Methods
    }
}