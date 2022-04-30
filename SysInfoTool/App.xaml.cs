using DataTools.Win32.Usb;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace SysInfoTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        public App() : base()
        {
            AppCenter.Start("f6f17897-33f6-4910-8fc5-9638de742b77",
                   typeof(Analytics), typeof(Crashes));
        }
    }
}
