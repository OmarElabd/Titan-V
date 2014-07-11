using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Titan_VI.Controller.Win8;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Titan_VI.Controller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BluetoothManager btManager;
        public MainPage()
        {
            this.InitializeComponent();



            btManager = new BluetoothManager();
            //btManager.AttemptEstablishConnection();
        }

        private void Button_LaunchBluetooth_Click(object sender, RoutedEventArgs e)
        {
            if (btManager.IsBluetoothEnabled())
            {
                MessageDialog dialog = new MessageDialog("Bluetooth is already enabled.");
                dialog.ShowAsync();
            }
            else
                btManager.RequestBluetooth();
        }

        private void Button_SendCommand_Click(object sender, RoutedEventArgs e)
        {
            string cmd = TextBox_CommandText.Text;

            //btManager.AttemptEstablishConnection();
            //btManager.WriteToDevice("ping", )
        }
        
        async private void Button_BTConnect_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog;

            if(await btManager.AttemptEstablishConnection())
            {
                dialog = new MessageDialog("Connection successfully established.");
            }
            else
            {
                dialog = new MessageDialog("Connection could not be established");
            }

            dialog.ShowAsync();
        }

        private void Button_StartMotors_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("go");
        }

        private void Button_MotorReverse_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("reverse");
        }

        private void Button_MotorStop_Click(object sender, RoutedEventArgs e)
        {
            btManager.WriteToDevice("stop");
        }
    }
}
