using DataTools.Shell.Native;
using DataTools.Win32;
using DataTools.Win32.Menu;

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DataTools.Desktop
{
    public static class Assoc
    {
        /// <summary>
        /// Populates the Open With menu and returns a <see cref="NativeMenu"/> object.
        /// </summary>
        /// <param name="fileName">The file name whose menu to retrieve.</param>
        /// <param name="openWithCmd">The menu item id of the Open With menu item of the parent menu.</param>
        /// <param name="hMenu">The handle of the parent menu.</param>
        /// <returns></returns>
        public static NativeMenu GetOpenWithMenu(string fileName, IntPtr openWithCmd, IntPtr hMenu)
        {
            // Create a native context menu submenu populated with "open with" items
            var col = new MenuItemBagCollection();
            var nm = new NativeMenu(hMenu);

            string ext = Path.GetExtension(fileName).ToLower();

            NativeMenuItem nmi;
            var assoc = NativeShell.EnumFileHandlers(ext);

            nm.Items.Clear();

            if (assoc is null)
            {
                nm.Destroy();
                return null;
            }

            foreach (IAssocHandler handler in assoc)
            {
                Icon icn;

                string pth = null;

                int idx;

                string uiname = null;
                string pathname = null;

                handler.GetIconLocation(out pth, out idx);

                int iix = (int)NativeShell.Shell_GetCachedImageIndex(pth, idx, 0U);

                icn = Resources.GetFileIconFromIndex(iix, (Resources.SystemIconSizes)(int)(User32.SHIL_SMALL));

                handler.GetName(out pathname);

                if (File.Exists(pathname) == false)
                    continue;

                handler.GetUIName(out uiname);

                if (icn is null)
                {
                    nmi = nm.Items.Add(uiname);
                }
                else
                {
                    nmi = nm.Items.Add(uiname, icn);
                }

                col.Add(new MenuItemBag(nmi, handler));
            }

            nm.Items.Add(null);
            nmi = nm.Items.Add("&Choose default program...");
            nmi.Id = (int)openWithCmd;
            nm.Bag = col;
            return nm;
        }
    }
}