


using DataTools.Desktop.Network;

using static DataTools.Text.TextTools;

namespace TestNetwork
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {


            var adapters = new ObservableAdaptersCollection();

            foreach (var adapter in adapters)
            {
                if (adapter.HasInternet == DataTools.Win32.Network.InternetStatus.HasInternet)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                Console.WriteLine(adapter.ToString() + $" ({Separate(adapter.HasInternet.ToString())})");
                Console.ResetColor();
            }


        }
    }
}