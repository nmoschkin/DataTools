using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32.Usb
{
    public enum HidPowerTestState
    {

        DoneAndPassed = 1, 
        DoneAndWarning = 2, 
        DoneAndError = 3, 
        Aborted = 4, 
        InProgress = 5, 
        NoTestInitiated = 6, 
    
    }
}
