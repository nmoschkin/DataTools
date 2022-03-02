using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Win32
{
    public enum HidUsagePage : ushort
    {

        Undefined = 0x00,
        Generic = 0x01,
        Simulation = 0x02,
        VR = 0x03,
        Sport = 0x04,
        Game = 0x05,
        GenericDevice = 0x06,
        Keyboard = 0x07,
        LED = 0x08,
        Button = 0x09,
        Ordinal = 0x0A,
        Telephony = 0x0B,
        Consumer = 0x0C,
        Digitizer = 0x0D,
        Haptics = 0x0E,
        Pid = 0x0F,
        Unicode = 0x10,
        Alphanumeric = 0x14,
        Sensor = 0x20,
        LightingIllumination = 0x59,
        BarcodeScanner = 0x8C,
        WeighingDevice = 0x8D,
        MagneticStripeReader = 0x8E,
        CameraControl = 0x90,
        Arcade = 0x91,
        MicrosoftBluetoothHandsFree = 0xFFF3,
        VendorDefinedBegin = 0xFF00,
        VendorDefinedEnd = 0xFFFF

    }
}
