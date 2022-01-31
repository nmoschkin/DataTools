using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

using static DataTools.Win32.User32;

namespace DataTools.Desktop
{
    public static class ShellTools
    {


        //    public static string GetChromeWindowUrl(string title)
        //    {
        //        IntPtr hwnd = FindWindow("Chrome_WidgetWin_1", title);

        //        // find the automation element
        //        AutomationElement elm = AutomationElement.FromHandle(hwnd);
        //        AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
        //          new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

        //        // if it can be found, get the value from the URL bar
        //        if (elmUrlBar != null)
        //        {
        //            AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
        //            if (patterns.Length > 0)
        //            {
        //                ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
        //                return val.Current.Value;
        //            }
        //        }

        //        return null;
        //    }

        //}



        public static string GetChromeWindowUrl(string title)
        {
            IntPtr hwnd = FindWindow("Chrome_WidgetWin_1", title);

            // find the automation element
            AutomationElement elm = AutomationElement.FromHandle(hwnd);
            AutomationElement elmUrlBar = elm.FindFirst(TreeScope.Descendants,
              new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

            // if it can be found, get the value from the URL bar
            if (elmUrlBar != null)
            {
                AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                if (patterns.Length > 0)
                {
                    ValuePattern val = (ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0]);
                    return val.Current.Value;
                }
            }

            return null;
        }
    }
}
